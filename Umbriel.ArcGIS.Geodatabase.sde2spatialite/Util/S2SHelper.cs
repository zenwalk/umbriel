// <copyright file="S2SHelper.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-03-14</date>
// <summary>S2SHelper class file</summary>

namespace Umbriel.ArcGIS.Geodatabase.sde2spatialite.Util
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using ESRI.ArcGIS.DataSourcesGDB;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;
    using SysIO = System.IO;

    /// <summary>
    /// static class for sde2spatialite exe
    /// </summary>
    internal static class S2SHelper
    {
        /// <summary>
        /// generates the help manual/usage string
        /// </summary>
        /// <returns>help manual string</returns>
        internal static string HelpManual()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("sde2sL \t Feature class to Spatial Lite Table Utility");
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine("sde2sL -h");
            sb.AppendLine("\n\n");
            sb.AppendLine("sde2sL -o <init> \t-l <table,column> [-V <version_name>]");
            sb.AppendLine("\t\t\t -sqlite <Path to SQL Lite Database>");
            sb.AppendLine("\t\t\t -s <server_name>");
            sb.AppendLine("\t\t\t -i <service>");
            sb.AppendLine("\t\t\t -u <user_name>");
            sb.AppendLine("\t\t\t -srid <Spatialite Spatial Reference ID>");
            sb.AppendLine("\t\t\t -T <Spatialite Table Name>");
            sb.AppendLine("\t\t\t -p <password>");
            sb.AppendLine("\t\t\t -D <database>");
            sb.AppendLine("\t\t\t -w <where clause>");
            sb.AppendLine("\t\t\t -product   <ESRI ArcGIS Product>");

            sb.AppendLine("sde2sL -o <wkt> \t-l <table,column> [-V <version_name>]");
            sb.AppendLine("\t\t\t -c <table_col1,table_col2...table_coln>");
            sb.AppendLine("\t\t\t -s <server_name>");
            sb.AppendLine("\t\t\t -i <service>");
            sb.AppendLine("\t\t\t -u <user_name>");
            sb.AppendLine("\t\t\t -p <password>");
            sb.AppendLine("\t\t\t -D <database>");
            sb.AppendLine("\t\t\t -w <where clause>");
            sb.AppendLine("\t\t\t -product   <ESRI ArcGIS Product>");

            sb.AppendLine("\n\n\t\t\tESRI License Product Codes:");
            sb.AppendLine("\t\t\tConstant                          Value  Description ");
            sb.AppendLine("\t\t\t--------------------------------- ------  ------------------------------------------");
            sb.AppendLine("\t\t\tesriLicenseProductCodeEngine        10   Engine Product Code ");
            sb.AppendLine("\t\t\tesriLicenseProductCodeEngineGeoDB   20   Engine Enterprise GeoDatabase Product Code ");
            sb.AppendLine("\t\t\tesriLicenseProductCodeArcServer     30   ArcServer Product Code ");
            sb.AppendLine("\t\t\tesriLicenseProductCodeArcView       40   ArcView Product Code ");
            sb.AppendLine("\t\t\tesriLicenseProductCodeArcEditor     50   ArcEditor Product Code ");
            sb.AppendLine("\t\t\tesriLicenseProductCodeArcInfo       60   ArcInfo Product Code ");

            return sb.ToString();
        }

        /// <summary>
        /// Creates a workspace to the SDE database from the command line args
        /// </summary>
        /// <param name="args">The command line args.</param>
        /// <returns>IWorkspace interface to the SDEWorkspace</returns>
        internal static IWorkspace Workspace(ref CommandLine.Utility.Arguments args)
        {
            // TODO: Maybe extend this to work with personal/file geodb in the future. Shouldn't be too difficult.
            IWorkspace workspace = null;

            string server = args["s"] ?? string.Empty;
            string instance = args["i"] ?? string.Empty;
            string user = args["u"] ?? string.Empty;
            string password = args["p"] ?? string.Empty;
            string database = args["D"] ?? string.Empty;
            string authmode = args["m"] ?? string.Empty;
            string version = args["V"] ?? string.Empty;

            IPropertySet propertyset = Utility.ArcSDEConnPropSet(server, instance, user, password, database, version, authmode);

            IWorkspaceFactory factory = new SdeWorkspaceFactoryClass();

            workspace = factory.Open(propertyset, 0);

            return workspace;
        }

        /// <summary>
        /// Converts the geometry to WKB.
        /// </summary>
        /// <param name="geometry">The ESRI geometry interface</param>
        /// <returns>WKB byte array</returns>
        internal static byte[] ConvertGeometryToWKB(IGeometry geometry)
        {
            IWkb wkb = geometry as IWkb;
            ITopologicalOperator oper = geometry as ITopologicalOperator;
            oper.Simplify();

            IGeometryFactory3 factory = new GeometryEnvironment() as IGeometryFactory3;
            byte[] b = factory.CreateWkbVariantFromGeometry(geometry) as byte[];

            return b;
        }

        /// <summary>
        /// Gets the SDE table parameter from the command line args
        /// </summary>
        /// <param name="args">The  command line args.</param>
        /// <returns>sde table name</returns>
        internal static string GetSDETable(ref CommandLine.Utility.Arguments args)
        {
            try
            {
                if (args != null)
                {
                    string tabcol = args["l"] ?? string.Empty;

                    if (!string.IsNullOrEmpty(tabcol))
                    {
                        return tabcol.Split(',')[0];
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// creates the create table SQL statement for the featureclass
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="spatialiteTableName">Name of the spatialite table.</param>
        /// <param name="fieldmappings">The fieldmappings.</param>
        /// <param name="geometryfieldname">The geometryfieldname.</param>
        /// <returns>create table SQL Statement</returns>
        internal static string GenerateCreateTableSQL(IWorkspace workspace, string tableName, string spatialiteTableName, out List<FieldMap> fieldmappings, string geometryfieldname)
        {
            StringBuilder sqlStatement = new StringBuilder();
            fieldmappings = new List<FieldMap>();

            try
            {
                IFeatureWorkspace wrkspc = (IFeatureWorkspace)workspace;

                IFeatureClass fc = wrkspc.OpenFeatureClass(tableName);
                IDataset ds = (IDataset)fc;

                string tablename = Utility.ParseObjectClassName(ds.Name);

                sqlStatement.Append("CREATE TABLE ");
                sqlStatement.Append(spatialiteTableName);
                sqlStatement.Append("(");

                int fieldCount = fc.Fields.FieldCount;

                for (int i = 0; i < fieldCount; i++)
                {
                    IField field = fc.Fields.get_Field(i);

                    if (!ExcludeByFieldName(field.Name))
                    {
                        string sqlField = FieldSQL(field);

                        if (sqlField != null
                            && sqlField.Length > 0
                            && sqlField.Contains(")").Equals(false))
                        {
                            if (i > 0)
                            {
                                sqlStatement.Append(',');
                            }

                            sqlStatement.Append(sqlField);

                            FieldMap fieldmap = new FieldMap(field, field.Name, Util.S2SHelper.GetDBDataType(field));
                            fieldmappings.Add(fieldmap);
                        }
                    }
                }

                // add a WKT field
                sqlStatement.Append(",WKT TEXT");
                sqlStatement.Append(')');
                                
                Trace.WriteLineIf(Program.OperationTraceSwitch.TraceVerbose, "SQL: Create Table Statement: \n" + sqlStatement.ToString());
            }
            catch (System.Runtime.InteropServices.COMException excom)
            {
                Trace.WriteLineIf(Program.OperationTraceSwitch.TraceError, excom.Message);
                throw;
            }
            catch (Exception e)
            {
                Trace.WriteLineIf(Program.OperationTraceSwitch.TraceError, e.Message);
                throw;
            }

            return sqlStatement.ToString();
        }

        /// <summary>
        /// Excludes the name of the by field.
        /// </summary>
        /// <param name="fieldname">The fieldname.</param>
        /// <returns>boolean if excluded</returns>
        internal static bool ExcludeByFieldName(string fieldname)
        {
            // TODO: will need to accept this config setting via command-line arg
            List<string> nameList = new List<string>(Constants.ExcludedFieldNames.Split(','));
            return nameList.Contains(fieldname, StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Determines whether [contains case insensitive] [the specified source].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// <c>true</c> if [contains case insensitive] [the specified source]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool ContainsCaseInsensitive(this string source, string value)
        {
            int results = source.IndexOf(value, StringComparison.CurrentCultureIgnoreCase);
            return results == -1 ? false : true;
        }

        /// <summary>
        /// Gets the factory code.
        /// </summary>
        /// <param name="fc">The IFeatureClass</param>
        /// <returns>ESRI factory code</returns>
        internal static int GetFactoryCode(IFeatureClass fc)
        {
            try
            {
                IDataset dataset = (IDataset)fc;
                IGeoDataset geodataset = (IGeoDataset)dataset;
                ISpatialReference spatialreference = geodataset.SpatialReference;

                return spatialreference.FactoryCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                return 0;
            }
        }

        /// <summary>
        /// Gets DBType for an esriFieldType to use for spatialite
        /// </summary>
        /// <param name="field">The ESRI Ifield.</param>
        /// <returns>DbType to use in spatialite</returns>
        internal static DbType GetDBDataType(IField field)
        {
            switch (field.Type)
            {
                case esriFieldType.esriFieldTypeBlob:
                    return DbType.Binary;
                case esriFieldType.esriFieldTypeDate:
                    return DbType.DateTime;
                case esriFieldType.esriFieldTypeDouble:
                    return DbType.Double;
                case esriFieldType.esriFieldTypeGUID:
                    return DbType.String;
                case esriFieldType.esriFieldTypeGeometry:
                    return DbType.Binary;
                case esriFieldType.esriFieldTypeGlobalID:
                    return DbType.String;
                case esriFieldType.esriFieldTypeInteger:
                    return DbType.Int32;
                case esriFieldType.esriFieldTypeOID:
                    return DbType.Int32;
                case esriFieldType.esriFieldTypeRaster:
                    return DbType.Binary;
                case esriFieldType.esriFieldTypeSingle:
                    return DbType.Single;
                case esriFieldType.esriFieldTypeSmallInteger:
                    return DbType.Int32;
                case esriFieldType.esriFieldTypeString:
                    return DbType.String;
                case esriFieldType.esriFieldTypeXML:
                    return DbType.String;
                default:
                    return DbType.Object;
            }
        }

        /// <summary>
        /// Gets the spatialite geometry type of the IFeatureClass
        /// </summary>
        /// <param name="fc">The IFeatureClass.</param>
        /// <returns>geometry type string (e.g. POINT|LINESTRING|POLYGON|MULTIPOINT)</returns>
        internal static string GetGeometryType(ref IFeatureClass fc)
        {
            esriGeometryType geomType = fc.ShapeType;

            switch (geomType)
            {
                case esriGeometryType.esriGeometryPoint:
                    return "POINT";

                case esriGeometryType.esriGeometryPath:
                case esriGeometryType.esriGeometryPolyline:
                case esriGeometryType.esriGeometryLine:
                    return "LINESTRING";

                case esriGeometryType.esriGeometryEnvelope:
                case esriGeometryType.esriGeometryPolygon:
                    return "POLYGON";

                case esriGeometryType.esriGeometryMultipoint:
                    return "MULTIPOINT";

                case esriGeometryType.esriGeometryAny:                    
                case esriGeometryType.esriGeometryBag:                    
                case esriGeometryType.esriGeometryBezier3Curve:                    
                case esriGeometryType.esriGeometryCircularArc:                    
                case esriGeometryType.esriGeometryEllipticArc:                    
                case esriGeometryType.esriGeometryMultiPatch:
                case esriGeometryType.esriGeometryNull:
                case esriGeometryType.esriGeometryRay:
                case esriGeometryType.esriGeometryRing:
                case esriGeometryType.esriGeometrySphere:
                case esriGeometryType.esriGeometryTriangleFan:
                case esriGeometryType.esriGeometryTriangleStrip:
                case esriGeometryType.esriGeometryTriangles:
                    
                default:
                    throw new ArgumentException(geomType.ToString() + " not supported with sde2spatialite.exe");
            }
        }

        /// <summary>
        /// Initializes the new spatialite DB.
        /// </summary>
        /// <param name="path">The path of the new spatialite database</param>
        internal static void InitializeNewSpatialiteDB(string path)
        {
            try
            {
                SysIO.FileStream fs = new SysIO.FileStream(path, SysIO.FileMode.Create, SysIO.FileAccess.ReadWrite);
                SysIO.BinaryWriter bw = new SysIO.BinaryWriter(fs);

                bw.Write(Resource.SpatRes.emptydb);

                bw.Close();

                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Fields the SQL geometry.
        /// </summary>
        /// <param name="geomtype">The geomtype.</param>
        /// <param name="geometryFieldName">Name of the geometry field.</param>
        /// <returns>Spatialite SQL for geometry field (used in Create table SQL statement) </returns>
        private static string FieldSQLGeometry(esriGeometryType geomtype, string geometryFieldName)
        {
            switch (geomtype)
            {
                case esriGeometryType.esriGeometryPoint:
                    return geometryFieldName + " POINT ";
                case esriGeometryType.esriGeometryPolygon:
                    return geometryFieldName + " POLYGON ";
                case esriGeometryType.esriGeometryPolyline:
                    return geometryFieldName + " LINESTRING ";
                case esriGeometryType.esriGeometryLine:
                    return geometryFieldName + " LINESTRING ";
                case esriGeometryType.esriGeometryAny:

                case esriGeometryType.esriGeometryBag:

                case esriGeometryType.esriGeometryBezier3Curve:

                case esriGeometryType.esriGeometryCircularArc:

                case esriGeometryType.esriGeometryEllipticArc:

                case esriGeometryType.esriGeometryEnvelope:
                    
                case esriGeometryType.esriGeometryMultiPatch:

                case esriGeometryType.esriGeometryMultipoint:

                case esriGeometryType.esriGeometryNull:

                case esriGeometryType.esriGeometryPath:
                case esriGeometryType.esriGeometryRay:

                case esriGeometryType.esriGeometryRing:

                case esriGeometryType.esriGeometrySphere:

                case esriGeometryType.esriGeometryTriangleFan:

                case esriGeometryType.esriGeometryTriangleStrip:

                case esriGeometryType.esriGeometryTriangles:

                default:
                    Console.WriteLine(geometryFieldName + " skipped.  " + geomtype.ToString() + " not supported.");
                    return null;
            }
        }

        /// <summary>
        /// Generates the SQL field portion for a create table statement
        /// </summary>
        /// <param name="field">ESRI IField</param>
        /// <param name="geometryFieldName">Name of the geometry field.</param>
        /// <returns>Spatialite SQL for field (used in Create table SQL statement)</returns>
        private static string FieldSQL(IField field, string geometryFieldName)
        {
            string nullable = string.Empty;
            //// TODO: uncomment out nullable code:
            //// if (field.IsNullable.Equals(false))
            ////    nullable = " NOT NULL ";

            string fieldName = field.Name.Replace('.', '_');

            switch (field.Type)
            {
                case esriFieldType.esriFieldTypeBlob:
                    return fieldName + " BLOB " + nullable;
                case esriFieldType.esriFieldTypeDate:
                    return fieldName + " TEXT " + nullable;
                case esriFieldType.esriFieldTypeDouble:
                    return fieldName + " REAL " + nullable;
                case esriFieldType.esriFieldTypeGUID:
                    return fieldName + " TEXT " + nullable;
                case esriFieldType.esriFieldTypeGeometry:
                    return FieldSQLGeometry(field.GeometryDef.GeometryType, geometryFieldName);
                case esriFieldType.esriFieldTypeGlobalID:
                    return fieldName + " TEXT " + nullable;
                case esriFieldType.esriFieldTypeInteger:
                    return fieldName + " INTEGER " + nullable;
                case esriFieldType.esriFieldTypeOID:
                    return fieldName + " INTEGER " + nullable;
                case esriFieldType.esriFieldTypeRaster:
                    return null;
                case esriFieldType.esriFieldTypeSingle:
                    return fieldName + " REAL " + nullable;
                case esriFieldType.esriFieldTypeSmallInteger:
                    return fieldName + " INTEGER " + nullable;
                case esriFieldType.esriFieldTypeString:
                    return fieldName + " TEXT " + nullable;
                case esriFieldType.esriFieldTypeXML:
                    return fieldName + " TEXT " + nullable;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Generates the SQL field portion for a create table statement
        /// </summary>
        /// <param name="field">ESRI IField</param>
        /// <returns>Spatialite SQL for field (used in Create table SQL statement)</returns>
        private static string FieldSQL(IField field)
        {
            return FieldSQL(field, "geom");
        }
    }
}
