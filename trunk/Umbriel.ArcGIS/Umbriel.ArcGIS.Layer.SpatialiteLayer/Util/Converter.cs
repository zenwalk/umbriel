

namespace Umbriel.ArcGIS.Layer.SpatialiteLayer.Util
{
    using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

    public class Converter
    {
        public string DatabasePath { get; private set; }
        public string TableName { get; private set; }

        private SpatialiteTable Table { get; set; }
        private string PGDBPath { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="Converter"/> class.
        /// </summary>
        /// <param name="databasePath">The database path.</param>
        /// <param name="tableName">Name of the table.</param>
        public Converter(string databasePath, string tableName)
        {
            this.DatabasePath = databasePath;
            this.TableName = tableName;

            this.Table = GetSpatialTable(databasePath, tableName);
        }

        /// <summary>
        /// Converts  the spatialite table to a featureclass in a temporary, scratch workspace.
        /// </summary>
        /// <returns>IFeatureClass copy of the spatialite table</returns>
        public IFeatureClass ConvertToFeatureclass()
        {
            try
            {
                CreateTemporaryGeodatabase();

                IWorkspaceFactory factory = new AccessWorkspaceFactoryClass();
                IWorkspace workspace = factory.OpenFromFile(this.PGDBPath, 0);

                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace;

                IFields fields = CreateFields();

                // Use IFieldChecker to create a validated fields collection.
                IFieldChecker fieldChecker = new FieldCheckerClass();
                IEnumFieldError enumFieldError = null;
                IFields validatedFields = null;
                fieldChecker.ValidateWorkspace = (IWorkspace)featureWorkspace;
                fieldChecker.Validate(fields, out enumFieldError, out validatedFields);

                IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(this.TableName, validatedFields,
                    null, null, esriFeatureType.esriFTSimple, "Shape", "");

                LoadFeatureclass(featureClass);

                return featureClass;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }


        private void LoadFeatureclass(IFeatureClass featureclass)
        {
            try
            {
                SpatialLiteDB db = new SpatialLiteDB(this.DatabasePath);
                db.Open();

                string sql = "SELECT AsBinary(" + Table.GeometryColumnName + ") as wkb,* FROM " + this.TableName;

                using (SQLiteCommand command = new SQLiteCommand(sql, db.SQLiteDatabaseConn))
                {
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        System.Data.DataTable table = new System.Data.DataTable();
                        adapter.Fill(table);

                        IFeatureCursor cursor = featureclass.Insert(true);

                        foreach (DataRow row in table.Rows)
                        {
                            IFeatureBuffer feature = featureclass.CreateFeatureBuffer();

                            for (int i = 0; i < feature.Fields.FieldCount; i++)
                            {
                                IField field = feature.Fields.get_Field(i);
                                Debug.WriteLine("FieldName=" + field.Name);
                                if (field.Type != esriFieldType.esriFieldTypeGeometry
                                    & !field.Name.Equals("wkb", StringComparison.CurrentCultureIgnoreCase)
                                    & !field.Name.Equals("name", StringComparison.CurrentCultureIgnoreCase)
                                    & !field.Name.Equals(featureclass.OIDFieldName, StringComparison.CurrentCultureIgnoreCase))
                                {                                    
                                    try
                                    {
                                        Debug.WriteLine("Spatialite Field Value: " + row[field.Name].ToString());
                                        feature.set_Value(i, row[field.Name]);
                                    }
                                    catch (Exception ex)
                                    {
                                        Trace.WriteLine(ex.StackTrace);
                                    }
                                }
                                else if (field.Type == esriFieldType.esriFieldTypeGeometry)
                                {
                                    byte[] wkb = (byte[])row["wkb"];
                                    int bytesRead;
                                    object missing = Type.Missing;
                                    IGeometry outGeometry;
                                    

                                    object byteArrayObject = row["wkb"];

                                    IGeometryFactory3 factory = new GeometryEnvironment() as IGeometryFactory3;
                                    
                                    factory.CreateGeometryFromWkbVariant(byteArrayObject, out outGeometry, out bytesRead);

                                    if (outGeometry != null)
                                    {
                                        try
                                        {
                                            feature.Shape = outGeometry;
                                        }
                                        catch (Exception ex)
                                        {
                                            Trace.WriteLine(ex.StackTrace);
                                        }                                        
                                   }
                                }
                            }

                            cursor.InsertFeature(feature);
                        }


                    }
                }
                db.Close();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        private esriGeometryType GetEsriGeometryType()
        {
            switch (this.Table.GeometryType)
            {
                case "POINT":
                    return esriGeometryType.esriGeometryPoint;
                case "LINESTRING":
                    return esriGeometryType.esriGeometryPolyline;
                case "MULTIPOLYGON":
                case "POLYGON":
                    return esriGeometryType.esriGeometryPolygon;
                default:
                    throw new NotImplementedException(this.Table.GeometryType + " not implemented.");
            }
        }

        /// <summary>
        /// Creates the fields for the ESRI geodatabase featureclass from the spatialite table
        /// </summary>
        /// <returns>IFields to use when building the featureclass</returns>
        private IFields CreateFields()
        {
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;

            // Add an object ID field to the fields collection. This is mandatory for feature classes.
            IField oidField = new FieldClass();
            IFieldEdit oidFieldEdit = (IFieldEdit)oidField;
            oidFieldEdit.Name_2 = "OID";
            oidFieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsEdit.AddField(oidField);

            // Create a geometry definition (and spatial reference) for the feature class.

            IGeometryDef geometryDef = new GeometryDefClass();
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            geometryDefEdit.GeometryType_2 = GetEsriGeometryType();
            ISpatialReferenceFactory spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            ISpatialReference spatialReference = CreateSpatialReference();

            ISpatialReferenceResolution spatialReferenceResolution = (ISpatialReferenceResolution)spatialReference;
            spatialReferenceResolution.ConstructFromHorizon();
            ISpatialReferenceTolerance spatialReferenceTolerance = (ISpatialReferenceTolerance)spatialReference;
            spatialReferenceTolerance.SetDefaultXYTolerance();
            geometryDefEdit.SpatialReference_2 = spatialReference;

            // Add a geometry field to the fields collection. This is where the geometry definition is applied.
            IField geometryField = new FieldClass();
            IFieldEdit geometryFieldEdit = (IFieldEdit)geometryField;
            geometryFieldEdit.Name_2 = "Shape";
            geometryFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            geometryFieldEdit.GeometryDef_2 = geometryDef;
            fieldsEdit.AddField(geometryField);

            CreateSpatialiteFields(ref fieldsEdit);

            return fields;
        }

        /// <summary>
        /// Determines that maximum length of all text values for a sqlite field
        /// </summary>
        /// <param name="connection">sqlite  connection.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>maximum length of all text values</returns>
        private int MaxTextLength(SQLiteConnection connection, string tableName, string fieldName)
        {
            int fieldLength = 35;

            try
            {
                string sql = "select max(length(trim(" + fieldName + ")))from " + tableName;

                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    fieldLength = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }

            return fieldLength;
        }

        /// <summary>
        /// Creates the spatialite fields.
        /// </summary>
        /// <param name="fields">reference to IFieldsEdit</param>
        private void CreateSpatialiteFields(ref IFieldsEdit fields)
        {
            try
            {
                SpatialLiteDB db = new SpatialLiteDB(this.DatabasePath);
                db.Open();

                string sql = "PRAGMA table_info(\"" + this.TableName + "\")";

                using (SQLiteCommand command = new SQLiteCommand(sql, db.SQLiteDatabaseConn))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string spatialiteFieldType = reader["type"].ToString();

                            if (!(spatialiteFieldType.Equals("LINESTRING") ||
                                spatialiteFieldType.Equals("POINT") ||
                                spatialiteFieldType.Equals("POLYGON") ||
                                spatialiteFieldType.Equals("MULTIPOLYGON") ||
                                string.IsNullOrEmpty(spatialiteFieldType)))
                            {
                                IField nameField = new FieldClass();
                                IFieldEdit nameFieldEdit = (IFieldEdit)nameField;
                                nameFieldEdit.Name_2 = reader["name"].ToString();
                                esriFieldType fieldType = GetFieldType(reader["type"].ToString());
                                nameFieldEdit.Type_2 = fieldType;

                                if (fieldType.Equals(esriFieldType.esriFieldTypeString))
                                {
                                    nameFieldEdit.Length_2 = MaxTextLength(db.SQLiteDatabaseConn, this.TableName, reader["name"].ToString());
                                }

                                fields.AddField(nameField);
                            }
                        }
                    }
                }
                db.Close();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Gets the type of the esri field for the sqlite field
        /// </summary>
        /// <param name="spatialiteFieldType">Type of the sqlite field.</param>
        /// <returns></returns>
        private esriFieldType GetFieldType(string spatialiteFieldType)
        {
            switch (spatialiteFieldType)
            {
                case "INTEGER":
                case "COUNTER":
                case "AUTOINCREMENT":
                case "IDENTITY":
                case "LONG":
                case "BIGINT":
                case "INT":
                    return esriFieldType.esriFieldTypeInteger;

                case "TEXT":
                case "VARCHAR":
                case "NVARCHAR":
                case "CHAR":
                case "NTEXT":
                case "STRING":
                case "MEMO":
                case "NOTE":
                case "LONGTEXT":
                case "LONGCHAR":
                case "LONGVARCHAR":
                    return esriFieldType.esriFieldTypeString;

                case "BIT":
                case "YESNO":
                case "LOGICAL":
                case "BOOL":
                case "TINYINT":
                    return esriFieldType.esriFieldTypeSmallInteger;

                case "DOUBLE":
                case "FLOAT":
                case "DECIMAL":
                case "NUMERIC":
                case "MONEY":
                case "CURRENCY":
                    return esriFieldType.esriFieldTypeDouble;

                case "REAL":
                    return esriFieldType.esriFieldTypeSingle;

                case "TIME":
                case "DATE":
                case "TIMESTAMP":
                case "DATETIME":
                    return esriFieldType.esriFieldTypeDate;

                case "NULL":
                    return esriFieldType.esriFieldTypeInteger;
                default:
                    throw new NotImplementedException(spatialiteFieldType + "  not implemented.");
            }

        }

        /// <summary>
        /// Creates the a spatial reference for the srid
        /// </summary>
        /// <returns>ESRI ISpatialReference </returns>
        private ISpatialReference CreateSpatialReference()
        {
            ISpatialReferenceFactory spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            string esriwkt = EsriSpatialReferenceWKT(this.Table.SpatialReferenceID);

            try
            {
                ISpatialReference spatialReference;
                int bytesRead;

                spatialReferenceFactory.CreateESRISpatialReference(esriwkt, out spatialReference, out bytesRead);

                return spatialReference;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }            
        }

        /// <summary>
        /// Gets the Spatialite table.
        /// </summary>
        /// <returns>SpatialiteTable</returns>
        private SpatialiteTable GetSpatialTable(string databasePath, string tableName)
        {
            try
            {
                SpatialiteTable spatialiteTable = null;

                SpatialLiteDB db = new SpatialLiteDB(databasePath);

                db.Open();

                List<SpatialiteTable> tabNames = db.TableNames();

                foreach (SpatialiteTable table in tabNames)
                {
                    if (table.TableName.Equals(tableName))
                    {
                        spatialiteTable = table;
                    }
                }

                db.Close();

                return spatialiteTable;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Gets the ESRI spatial reference WKT.
        /// </summary>
        /// <param name="epsg">The epsg.</param>
        /// <returns>ESRI Spatial Reference WKT for epsg spatial reference code</returns>
        private string EsriSpatialReferenceWKT(int epsg)
        {
            return Umbriel.GIS.SpatialReferenceDotOrg.GetSpatialReference(epsg, Umbriel.GIS.Catalog.EPSG, Umbriel.GIS.Format.ESRIWKT);
        }

        private void CreateTemporaryGeodatabase()
        {
            try
            {
                
                //byte[] bytePath = System.Text.Encoding.Unicode.GetBytes(this.DatabasePath);
                byte[] bytePath = System.Guid.NewGuid().ToByteArray();
                string base64Databasepath = Convert.ToBase64String(bytePath);
                // System.IO.Path.GetFileNameWithoutExtension(this.DatabasePath)
                                
                IWorkspaceFactory factory = new AccessWorkspaceFactoryClass();
                IWorkspaceName workspaceName = factory.Create(System.IO.Path.GetTempPath(), base64Databasepath, null, 0);

                this.PGDBPath = System.IO.Path.GetTempPath() + "\\" + base64Databasepath + ".mdb";
            }
            catch { }
        }

        private string GetTemporaryFile(string extn)
        {
            string response = string.Empty;

            if (!extn.StartsWith("."))
                extn = "." + extn;

            response = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + extn;

            return response;
        }

    }
}
