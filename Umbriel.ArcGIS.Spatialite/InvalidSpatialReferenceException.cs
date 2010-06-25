    using System;
    
    

namespace Umbriel.ArcGIS.Spatialite
{
    public class InvalidSpatialReferenceException : ApplicationException
    {
        public InvalidSpatialReferenceException(string message, int srid, string authname): base(message)
        {
            this.AuthName = authname;
            this.SRID = srid;
        }

        #region Properties
        public int SRID { get; private set; }

        public string AuthName { get; private set; }
        #endregion
    }
}
