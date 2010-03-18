// <copyright file="Constants.cs" company="Umbriel Project">
// Copyright (c) 2010 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-03-14</date>
// <summary>Constants class file </summary>

namespace Umbriel.ArcGIS.Geodatabase.sde2spatialite
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// class that holds all constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Gets the InvalidOpArgMessage
        /// </summary>
        /// <value>The invalid op arg message.</value>
        public static string InvalidOpArgMessage
        {
            get
            {
                return "{0} is not a valid operation argument.  Command Usage:  sde2spatialite.exe -h";
            }
        }

        /// <summary>
        /// Gets the SQLite Basic Connection String
        /// </summary>
        /// <value>The SQ lite basic connection string.</value>
        public static string SQLiteBasicConnectionString
        {
            get
            {
                return "Data Source={0};Version=3;";
            }
        }

        /// <summary>
        /// Gets the comma-delimited list of  excluded field names.
        /// </summary>
        /// <value>The excluded field names.</value>
        public static string ExcludedFieldNames
        {
            get
            {
                return "shape.len,shape.area,shape_len,shape_area,shape";
            }
        }

        /// <summary>
        /// Gets the SQLUpdateGeomFromWKT
        /// </summary>
        /// <value>Geometry update SQL statement (from WKT)</value>
        public static string SQLUpdateGeomFromWKT
        {
            get
            {
                return "update {0} SET {1} = GeomFromText({2},{3}) WHERE WKT IS NOT NULL  AND  LENGTH(WKT) <> 0  AND WKT LIKE '{4}%'";
            }
        }

        /// <summary>
        /// Gets the SQL create geometry field.
        /// AddGeometryColumn( table String , column String , srid Integer , geom_type String , dimension Integer [ , not_null Integer ] ) : Integer
        /// http://www.gaia-gis.it/spatialite-2.3.0/spatialite-sql-2.3.0.html
        /// </summary>
        /// <value>The SQL create geometry field.</value>
        public static string SQLCreateGeometryField
        {
            get
            {
                return "SELECT AddGeometryColumn('{0}', '{1}', {2}, '{3}', 2)";
            }
        }

        /// <summary>
        /// Gets the default name of the geometry field for a spatialite geometry field
        /// </summary>
        /// <value>The default name of the geometry field.</value>
        public static string DefaultGeometryFieldName
        {
            get
            {
                return "geom";
            }
        }

        /// <summary>
        /// Gets the init op feature class opened message.
        /// </summary>
        /// <value>The init op feature class opened message.</value>
        public static string InitOpFeatureClassOpenedMessage
        {
            get
            {
                return "Featureclass Opened. {0} features in ArcGIS featureclass: {1}";
            }
        }
    }
}
