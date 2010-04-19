// <copyright file="Geohasher.cs" company="Umbriel Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-08-25</date>
// <summary>Geohasher class file</summary>

namespace Umbriel.ArcMap.Editor.Util
{
    using ESRI.ArcGIS.Geometry;
    using ESRI.ArcGIS.esriSystem;

    /// <summary>
    /// Creates a geohash from an ESRI IPoint
    /// </summary>
    public class Geohasher
    {
        /// <summary>
        /// default precision when none is specified
        /// </summary>
        private const int DefaultPrecision = 13;


        /// <summary>
        /// Creates the geohash for IPoint
        /// </summary>
        /// <param name="pt">IPoint (must contain spatial reference)</param>
        /// <param name="precision">The precision.</param>
        /// <returns>geohash string</returns>
        public static string CreateGeohash(IPoint pt,  int precision)
        {
            IClone source = (IClone)pt;
            IClone clone = source.Clone();

            IPoint clonePoint = (IPoint)clone;

            ISpatialReference spatialReference = WGS84SpatialReference();

            clonePoint.Project(spatialReference);

            string geohash = Umbriel.GIS.Geohash.EncodeCoordinate(clonePoint.Y, clonePoint.X, precision);

            return geohash;
        }

        /// <summary>
        /// Creates the geohash for IPoint
        /// </summary>
        /// <param name="pt">IPoint (must contain spatial reference)</param>
        /// <param name="spatialReference">The spatial reference to project to</param>
        /// <param name="precision">The precision.</param>
        /// <returns>geohash string</returns>
        public static string CreateGeohash(IPoint pt, ISpatialReference spatialReference, int precision)
        {
            pt.Project(spatialReference);
            string geohash = Umbriel.GIS.Geohash.EncodeCoordinate(pt.Y, pt.X, precision);

            return geohash;            
        }

        /// <summary>
        /// Creates the geohash for IPoint
        /// </summary>
        /// <param name="pt">IPoint (must contain spatial reference)</param>
        /// <param name="spatialReference">The spatial reference to project to</param>
        /// <returns>geohash string</returns>
        public static string CreateGeohash(IPoint pt, ISpatialReference spatialReference)
        {
            return CreateGeohash(pt, spatialReference, DefaultPrecision);
        }

        /// <summary>
        /// Creates the geohash for IPoint
        /// </summary>
        /// <param name="pt">IPoint (must contain spatial reference)</param>
        /// <returns>geohash string</returns>
        public static string CreateGeohash(IPoint pt)
        {
            ISpatialReference spatialReference = WGS84SpatialReference();
            return CreateGeohash(pt, spatialReference, DefaultPrecision);
        }

        /// <summary>
        /// Creates a WGS84 Spatial Reference
        /// </summary>
        /// <returns>ISpatialReference WGS84</returns>
        public static ISpatialReference WGS84SpatialReference()
        {
            SpatialReferenceEnvironment spatialReferenceEnv = new SpatialReferenceEnvironmentClass();

            IGeographicCoordinateSystem geoCS = spatialReferenceEnv.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
            ISpatialReference spatialReference = geoCS;
            spatialReference.SetFalseOriginAndUnits(-180, -90, 1000000);

            return spatialReference;
        }
    }
}
