using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Umbriel.ArcGIS.Spatialite
{
    public enum EnumGeometryType: int
    {
        [StringValue("POINT")]
        POINT = 1,
        [StringValue("LINESTRING")]
        LINESTRING = 2   ,
        [StringValue("POLYGON")]
        POLYGON = 3,
        [StringValue("MULTIPOINT")]
        MULTIPOINT = 4,
        [StringValue("MULTILINESTRING")]
        MULTILINESTRING = 5,
        [StringValue("MULTIPOLYGON")]
        MULTIPOLYGON = 6,
        [StringValue("GEOMETRYCOLLECTION")]
        GEOMETRYCOLLECTION = 7, 
    }

    /// <summary>
    /// StringValueAttribute class
    /// </summary>
    public class StringValueAttribute : Attribute
    {

        #region Properties

        /// <summary>
        /// Holds the stringvalue for a value in an enum.
        /// </summary>
        public string StringValue { get; protected set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor used to init a StringValue Attribute
        /// </summary>
        /// <param name="value"></param>
        public StringValueAttribute(string value)
        {
            this.StringValue = value;
        }

        #endregion

    }
}
