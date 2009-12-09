

namespace Umbriel.ArcGIS.Layer.SpatialiteLayer
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Text;

    public class SpatialiteTable
    {
        public string TableName { get;  set; }
        public string GeometryColumnName { get;  set; }
        public string GeometryType { get;  set; }
        public int CoordinateDimension { get;  set; }
        public int SpatialReferenceID { get;  set; }
        public bool SpatialIndexEnabled { get;  set; }

        public SpatialiteTable( )
        {
 
        }

    }
}
