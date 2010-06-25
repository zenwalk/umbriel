using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ArcMapUI;
using Umbriel.ArcGIS;
using Umbriel.ArcGIS.Layer;
using Umbriel.Extensions;

using LayerDictionary = System.Collections.Generic.Dictionary<int, ESRI.ArcGIS.Carto.ILayer>;
using OIDList = System.Collections.Generic.List<int>;

namespace Umbriel.ArcGIS.Spatialite
{
    /// <summary>
    /// Exports a Featurelayer to a spatialite database
    /// </summary>
    public class SpatialiteExporter
    {
        public delegate void StatusMessageDelegate(object sender, ExporterEventArgs args);

        public delegate void ExportProgressChanged(object sender, ExporterProgressEventArgs args);

        public event ExportProgressChanged AttributeReadProgress = delegate { };

        public event ExportProgressChanged GeometryReadProgress = delegate { };

        public event ExportProgressChanged AttributeExportProgress = delegate { };

        public event ExportProgressChanged GeometryExportProgress = delegate { };

        public event StatusMessageDelegate StatusMessageEvent = delegate { };

        #region Fields
        private string databasePath = string.Empty;

        private string geometryFieldName = "geom";
        #endregion

        #region Constructors
        public SpatialiteExporter()
        {
            this.CommitInterval = 0;
        }

        public SpatialiteExporter(string databasePath)
        {
            this.DatabasePath = databasePath;
            this.CommitInterval = 0;
        }
        #endregion

        #region Properties
        
        public string DatabasePath
        {
            get
            {
                return databasePath;
            }
            set
            {
                databasePath = value;

                this.SpatialiteDatabase =  new SpatialLiteDB(databasePath);
            }
        }

        /// <summary>
        /// Gets or sets the name of the geometry field for the target table
        /// </summary>
        /// <value>The name of the geometry field.</value>
        public string GeometryFieldName
        {
            get
            {
                return geometryFieldName;
            }
            set
            {
                geometryFieldName = value;
            }
        }


        private Dictionary<string, string> FieldNameMap { get; set; }


        // TODO: use a commit interval while exporting:

        /// <summary>
        /// Gets or sets the commit interval.
        /// </summary>
        /// <value>The commit interval.</value>
        public int CommitInterval { get; private set; }

        /// <summary>
        /// Gets or sets the extent which is used to filter features
        /// </summary>
        /// <value>The extent</value>
        public IEnvelope Extent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [selected features only].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [selected features only]; otherwise, <c>false</c>.
        /// </value>
        public bool SelectedFeaturesOnly { get; set; }


        private SpatialLiteDB SpatialiteDatabase { get; set; }

        #endregion

        #region Methods

        public string CreateTableDDL(DataTable table)
        {
            return CreateTableDDL(table, table.TableName.Replace(' ', '_'));
        }

        public string CreateTableDDL(DataTable table, string tableName)
        {
            int c = 0; // column counter

            this.FieldNameMap = new Dictionary<string, string>();

            StringBuilder sb = new StringBuilder("CREATE TABLE [");

            sb.Append(tableName);
            sb.Append("] (");

            foreach (DataColumn col in table.Columns)
            {
                this.FieldNameMap.Add(col.ColumnName, col.ColumnName.Replace('.', '_'));

                string sqlField = FieldDDL(col, this.FieldNameMap[col.ColumnName]);

                if (sqlField != null
                    && sqlField.Length > 0
                    && sqlField.Contains(")").Equals(false))
                {
                    if (c > 0)
                    {
                        sb.Append(',');
                    }

                    sb.Append(sqlField);
                }

                c++;
            }
            sb.Append(')');

            return sb.ToString();
        }

        public static string FieldDDL(DataColumn column, string columnName)
        {
            string ddl = string.Empty;

            string nullable = "";

            // if (field.IsNullable.Equals(false))
            //nullable = " NOT NULL ";

            Console.WriteLine("DataColumn DataType: " + column.DataType.ToString());

            Dictionary<Type, string> dbtypes = new Dictionary<Type, string>();

            if (Constants.DataTypeMappings.ContainsKey(column.DataType))
            {
                ddl = columnName + " " + Constants.DataTypeMappings[column.DataType] + " " + nullable;
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Field Datatype Not Supported: " + column.DataType.ToString() + " (column name=" + column.ColumnName + ")");
            }


            return ddl;
        }

        public int FindSRID(ILayer layer)
        {
            IGeoDataset g = (IGeoDataset)layer;
            
            return this.FindSRID(g.SpatialReference, new SpatialLiteDB(this.DatabasePath));
        }

        public int FindSRID(ILayer layer, string authname)
        {
            IGeoDataset g = (IGeoDataset)layer;

            return this.FindSRID(g.SpatialReference, new SpatialLiteDB(this.DatabasePath), authname);
        }

        public int FindSRID(ILayer layer, SpatialLiteDB database)
        {
            IGeoDataset g = (IGeoDataset)layer;
            return this.FindSRID(g.SpatialReference, database);
        }
        
        public int FindSRID(ISpatialReference sr, SpatialLiteDB database)
        {
            return FindSRID(sr, database, "esri");
        }

        public int FindSRID(ISpatialReference sr, SpatialLiteDB database, string authname)
        {
            int srid = -1;

            srid = sr.FactoryCode;

            // string s = Umbriel.GIS.SpatialReferenceDotOrg.GetSpatialReference(sr.FactoryCode, Umbriel.GIS.Format.Proj4);
            
            srid = database.FindSRID(srid, authname);

            return srid;
        }

        public void Export(Dictionary<int, ILayer> layers)
        {
            this.Export(this.DatabasePath, layers);
        }

        public void Export(ILayer layer)
        {
            this.Export(this.DatabasePath, layer);
        }

        public void Export(ILayer layer, int srid)
        {
            this.Export(this.DatabasePath, srid, layer);
        }

        public void Export(ILayer layer, int srid, int commitinterval)
        {
            this.CommitInterval = commitinterval;

            this.Export(layer, srid);
        }

        public void Export(ILayer layer, OIDList oids, int srid, int commitinterval)
        {
            IGeometryFactory3 factory = new GeometryEnvironment() as IGeometryFactory3;

            IFeatureLayer featureLayer = (IFeatureLayer)layer;
            IDataset dataset = (IDataset)featureLayer.FeatureClass;

            GeoDataTableAdapter.GeoAdapter adapter = new GeoDataTableAdapter.GeoAdapter(dataset.Workspace);


            this.CommitInterval = commitinterval;
            int i = 0;

            string tableName = layer.Name.Replace(' ', '_');

            while (i < oids.Count)
            {
                int elementcount;

                if (i + this.CommitInterval > oids.Count)
                {
                    elementcount = oids.Count - i;
                }
                else
                {
                    elementcount = this.CommitInterval;
                }

                OIDList oidrange = oids.GetRange(i, elementcount);

                object[] vals = oidrange.ToObjectArray();

                DataTable table =  adapter.GetAttributeTable(dataset.Name, featureLayer.FeatureClass.OIDFieldName, vals);
                table.AddWKBColumn();

                foreach (DataRow row in table.Rows)
                {
                    int oid = Convert.ToInt32(row[featureLayer.FeatureClass.OIDFieldName]);
                    IFeature feature = featureLayer.FeatureClass.GetFeature(oid);

                    byte[] wkbgeometry = (byte[])factory.CreateWkbVariantFromGeometry(feature.ShapeCopy);

                    row["WKB"] = wkbgeometry;
                }

                Write(table, tableName, srid, featureLayer.FeatureClass);

                i += this.CommitInterval;
            }            
        }

        public void Export(string spatialiteDatabasePath, LayerDictionary layers)
        {
            foreach (KeyValuePair<int, ILayer> item in layers)
            {
                this.Export(spatialiteDatabasePath, item.Value);
            }
        }
        
        public void Export(string spatialiteDatabasePath, ILayer layer)
        {
            SpatialLiteDB db = new SpatialLiteDB(spatialiteDatabasePath);
            int srid = this.FindSRID(layer, db);
            this.Export(spatialiteDatabasePath, srid, layer);
        }
                
        public void Export(string spatialiteDatabasePath, int srid, ILayer layer)
        {
            try
            {
                this.DatabasePath = spatialiteDatabasePath;

                if (srid > 0)
                {
                    IGeometryFactory3 factory = new GeometryEnvironment() as IGeometryFactory3;

                    IFeatureLayer featureLayer = (IFeatureLayer)layer;

                    IDataset dataset = (IDataset)featureLayer.FeatureClass;

                    GeoDataTableAdapter.GeoAdapter adapter = new GeoDataTableAdapter.GeoAdapter(dataset.Workspace);

                    DataTable table = null;
                    ITable featureTable = (ITable)featureLayer.FeatureClass;

                    if (this.Extent != null)
                    {
                        ISpatialFilter filter = new SpatialFilterClass();
                        filter.Geometry = this.Extent;
                        filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                        filter.SubFields = featureLayer.FeatureClass.OIDFieldName;

                        IFeatureCursor cursor = featureLayer.FeatureClass.Search(filter, true);

                        IFeature feature = null;
                        object[] vals = new object[featureLayer.FeatureClass.FeatureCount(filter)];

                        int w = 0;

                        while ((feature = cursor.NextFeature()) != null)
                        {
                            vals[w++] = feature.OID;
                        }

                        table = adapter.GetAttributeTable(dataset.Name, featureLayer.FeatureClass.OIDFieldName, vals);
                    }
                    else
                    {
                        // no events for the adapter, so just 1 step for now.
                        // TODO: update GeoDataTableAdapter w/ events.
                        this.OnAttributeReadProgress(0, 1);
                        table = adapter.GetAttributeTable(featureTable, dataset.Name.Replace('.', '_'));
                        this.OnAttributeReadProgress(1, 1);
                    }

                    DataColumn wkbcolumn = new DataColumn("WKB", typeof(byte[]));
                    table.Columns.Add(wkbcolumn);

                    int totalRows = table.Rows.Count;
                    int i = 0;
                    this.OnGeometryReadProgress(i, totalRows);

                    foreach (DataRow row in table.Rows)
                    {
                        this.OnGeometryReadProgress(++i, totalRows);

                        int oid = Convert.ToInt32(row[featureLayer.FeatureClass.OIDFieldName]);

                        IFeature feature = featureLayer.FeatureClass.GetFeature(oid);

                        byte[] wkbgeometry = (byte[])factory.CreateWkbVariantFromGeometry(feature.ShapeCopy);

                        row["WKB"] = wkbgeometry;
                    }

                    WriteTable(this.SpatialiteDatabase, srid, table, featureLayer.FeatureClass);
                }
                else
                {

                }

                // this.SpatialiteDatabase.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }

        }


        private void WriteTable(SpatialLiteDB database, int srid, DataTable table, IFeatureClass fc)
        {
            string geometryColumn = "geom";
            string tableName = table.TableName.Replace(' ', '_');

            string s = CreateTableDDL(table);
            database.CreateTable(s);


            database.InsertEvent += new SpatialLiteDB.InsertEventHandler(database_InsertEvent);
            
            database.Insert(table, this.FieldNameMap);

            try
            {

                this.OnStatusMessage(string.Format("Adding geometry column ({0}) with SRID: {1} to table: {2}",
                    geometryColumn,
                    srid,
                    tableName));

                database.AddGeometryColumn(
                                                        tableName,
                                                        geometryColumn,
                                                        srid,
                                                        Constants.SpatialiteGeometryType[fc.ShapeType]);
                
                    this.OnStatusMessage(string.Format("Geometry column ({0}) with SRID: {1} successfully added to table: {2}!",
                        geometryColumn,
                        srid,
                        tableName));

                    this.OnStatusMessage(string.Format("Updating geometry column ({0}) in table: {1}",
                        geometryColumn,
                        tableName));
                

                this.OnGeometryExportProgress(0, 1);

                database.UpdateGeometryColumnFromWKB(
                    tableName,
                    geometryColumn,
                    srid,
                    "wkb");

                this.OnGeometryExportProgress(1, 1);

                this.OnStatusMessage(string.Format("Sucessfully update geometry column ({0}) in table: {1}!",
                    geometryColumn,
                    tableName));
            }
            catch (Exception)
            {
                throw;
            }


            database.InsertEvent -= database_InsertEvent;
        }

        private void Write(DataTable table, string tableName, int srid,IFeatureClass featureclass)
        {
            if (!this.SpatialiteDatabase.TableExists(tableName))
            {
                string ddl = CreateTableDDL(table,tableName);
                this.SpatialiteDatabase.CreateTable(ddl);



                //add the geometry column
                this.OnStatusMessage(string.Format("Adding geometry column ({0}) with SRID: {1} to table: {2}",
                    this.GeometryFieldName,
                    srid,
                    tableName));

                //this.SpatialiteDatabase.AddGeometryColumn(
                //                        tableName,
                //                        this.geometryFieldName,
                //                        srid,
                //                        Constants.SpatialiteGeometryType[featureclass.ShapeType]);

                this.OnStatusMessage(string.Format("Geometry column ({0}) with SRID: {1} successfully added to table: {2}!",
                    this.GeometryFieldName,
                    srid,
                    tableName));
            }


            // insert the records:
            this.SpatialiteDatabase.Insert(table,tableName, this.FieldNameMap);



            //update geometry field with the WKB field
            try
            {

                this.OnGeometryExportProgress(0, 1);

                 //this.SpatialiteDatabase.UpdateGeometryColumnFromWKB(
                 //   tableName,
                 //   this.GeometryFieldName,
                 //   srid,
                 //   "wkb");

                this.OnGeometryExportProgress(1, 1);

                this.OnStatusMessage(string.Format("Sucessfully update geometry column ({0}) in table: {1}!",
                    this.GeometryFieldName,
                    tableName));
            }
            catch (Exception)
            {
                throw;
            }


        }

        void database_InsertEvent(object sender, SpatialiteProgressEventArgs args)
        {
            this.OnAttributeExportProgress(args.Current, args.Total);
        }




        /// <summary>
        /// Called when [status message].
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnStatusMessage(string message)
        {
            this.StatusMessageEvent(this,
                new ExporterEventArgs(message));
        }

        private void OnAttributeReadProgress(int c, int t)
        {
            this.AttributeReadProgress(this,
                new ExporterProgressEventArgs(c, t));
        }

        private void OnGeometryReadProgress(int c, int t)
        {
            this.GeometryReadProgress(this,
                new ExporterProgressEventArgs(c, t));
        }

        private void OnAttributeExportProgress(int c, int t)
        {
            this.AttributeExportProgress(this,
                new ExporterProgressEventArgs(c, t));
        }

        private void OnGeometryExportProgress(int c, int t)
        {
            this.GeometryExportProgress(this,
                new ExporterProgressEventArgs(c, t));
        }


        #endregion
    }
}
