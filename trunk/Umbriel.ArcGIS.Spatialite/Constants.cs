namespace Umbriel.ArcGIS.Spatialite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class Constants
    {

    //    UPDATE [{0}] SET {1} = GeomFromWKB({2},{3})

        public static string SelectSRIDByProj4text
        {
            get
            {
                return "select srid from spatial_ref_sys where proj4text = @proj4text";
            }
        }

        public static string UpdateGeometryFromWKBSQL
        {
            get
            {
                return "UPDATE [{0}] SET {1} = GeomFromWKB({2},{3})";
            }
        }

        public static string BasicSQliteConnectionString
        {
            get
            {
                return "Data Source={0};Version=3;";
            }
        }

        public static string SelectGeometryColumnsSQL
        {
            get
            {
                return "SELECT f_table_name,f_geometry_column,type,coord_dimension, srid,spatial_index_enabled FROM geometry_columns";
            }
        }

        public static string SelectGeometryColumnSQL
        {
            get
            {
                return "SELECT f_table_name,f_geometry_column,type,coord_dimension, srid,spatial_index_enabled FROM geometry_columns WHERE f_table_name = '{0}'";
            }
        }

        public static string DefaultObjectIDFieldName
        {
            get
            {
                return "OBJECTID";
            }
        }

        public static string SelectTableSpatialExtent
        {
            get
            {
                return @"select 
	Min(MbrMinX({1})) as minx
	,Max(MbrMaxX({1})) as maxx
	,Min(MbrMinY({1})) as miny
	,Max(MbrMaxY({1})) as maxy
from {0}
";
            }
        }

        public static string SelectObjectIDsCriteriaSQL
        {
            get
            {
                return "select {1} from {0} WHERE {2}";
            }
        }

        public static string SelectObjectIDsSQL
        {
            get
            {
                return "select {1} from {0}";
            }
        }

        public static Dictionary<Type, string> DataTypeMappings
        {
            get
            {
                Dictionary<Type, string> d = new Dictionary<Type, string>();

                d.Add(typeof(System.Decimal), "NUMERIC");
                d.Add(typeof(System.Single), "REAL");
                d.Add(typeof(System.Double), "REAL");
                d.Add(typeof(System.String), "TEXT");
                d.Add(typeof(System.Byte[]), "BLOB");
                d.Add(typeof(System.Int32), "INTEGER");
                d.Add(typeof(System.DateTime), "NUMERIC");

                



                //                Field Datatype Not Supported: System.Decimal (column name=OBJECTID)
                //Field Datatype Not Supported: System.String (column name=MONUMENT)
                //Field Datatype Not Supported: System.Double (column name=NORTHING_SFT)
                //Field Datatype Not Supported: System.Double (column name=EASTING_SFT)
                //Field Datatype Not Supported: System.Double (column name=ELEVATION_SFT)
                //Field Datatype Not Supported: System.Double (column name=NORTHING_M)
                //Field Datatype Not Supported: System.Double (column name=EASTING_M)
                //Field Datatype Not Supported: System.Double (column name=ELEVATION_M)
                //Field Datatype Not Supported: System.String (column name=LATITUDE)
                //Field Datatype Not Supported: System.String (column name=LONGITUDE)
                //Field Datatype Not Supported: System.Double (column name=ELLIPSOID_HT_SFT)
                //Field Datatype Not Supported: System.Double (column name=GRID_SF)
                //Field Datatype Not Supported: System.Double (column name=ELEV_SF)
                //Field Datatype Not Supported: System.Double (column name=COMBINED_SF)
                //Field Datatype Not Supported: System.String (column name=CONVERGENCE)
                //Field Datatype Not Supported: System.String (column name=DESCRIPTION)
                //Field Datatype Not Supported: System.String (column name=WEB_PAGE)
                //Field Datatype Not Supported: System.Byte[] (column name=WKB)


                return d;
            }
        }


        public static Dictionary<ESRI.ArcGIS.Geometry.esriGeometryType, string> GeometryType
        {
            get
            {
                Dictionary<ESRI.ArcGIS.Geometry.esriGeometryType, string> d = new Dictionary<ESRI.ArcGIS.Geometry.esriGeometryType, string>();

                d.Add(ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon, "POLYGON");
                d.Add(ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline, "LINESTRING");
                d.Add(ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint, "POINT");

                return d;
            }
        }

        public static Dictionary<ESRI.ArcGIS.Geometry.esriGeometryType, EnumGeometryType> SpatialiteGeometryType
        {
            get
            {
                Dictionary<ESRI.ArcGIS.Geometry.esriGeometryType, EnumGeometryType> d = new Dictionary<ESRI.ArcGIS.Geometry.esriGeometryType, EnumGeometryType>();

                d.Add(ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon,EnumGeometryType.POLYGON);
                d.Add(ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline, EnumGeometryType.LINESTRING);
                d.Add(ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint, EnumGeometryType.POINT);

                return d;
            }
        }
    }
}
