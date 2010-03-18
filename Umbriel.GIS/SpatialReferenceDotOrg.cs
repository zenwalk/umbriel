// <copyright file="SpatialReferenceDotOrg.cs" company="Umbriel Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-11-06</date>
// <summary>
////</summary>

namespace Umbriel.GIS
{
    using System.IO;
    using System.Net;
    using System.Text;
    
    /// <summary>
    /// spatial reference catalog
    /// </summary>
    public enum Catalog
    {
        /// <summary>
        /// European Petroleum Survey Group spatial reference catalog
        /// </summary>
        EPSG = 1,

        /// <summary>
        /// ESRI's spatial reference catalog
        /// </summary>
        ESRI = 2,

        /// <summary>
        ///  IAU2000 spatial reference catalog
        /// </summary>
        IAU2000 = 3,

        /// <summary>
        /// spatial references submitted to spatialreference.org
        /// </summary>
        SRORG = 4
    }

    /// <summary>
    /// spatial reference definition format
    /// </summary>
    public enum Format
    {
        /// <summary>
        /// Proj4 format
        /// </summary>
        Proj4 = 1,

        /// <summary>
        /// OGC well known text
        /// </summary>
        OGCWKT = 2,

        /// <summary>
        /// human readable well known text
        /// </summary>
        HumanReadableWKT = 3,

        /// <summary>
        /// geography markup language
        /// </summary>
        GML = 4,

        /// <summary>
        /// ESRI well known text
        /// </summary>
        ESRIWKT = 5,

        /// <summary>
        /// USGS format
        /// </summary>
        USGS = 6,

        /// <summary>
        /// Map Server Map file
        /// </summary>
        MapServerMapfile = 7,

        /// <summary>
        /// map server python
        /// </summary>
        MapServerPython = 8,

        /// <summary>
        /// mapnik xml
        /// </summary>
        MapnikXML = 9,

        /// <summary>
        /// mapnik python
        /// </summary>
        MapnikPython = 10,

        /// <summary>
        /// postgis sql insert statement
        /// </summary>
        PostGISInsert = 11,

        /// <summary>
        /// proj4 javascript
        /// </summary>
        Proj4js = 12
    }

    /// <summary>
    /// class for obtaining various spatial reference definition formats
    /// from spatialreference.org
    /// </summary>
    public static class SpatialReferenceDotOrg
    {
        /// <summary>
        /// Gets the spatial reference text
        /// </summary>
        /// <param name="code">The spatial reference code for a catalog</param>
        /// <param name="catalog">The spatial reference catalog (EPSG, ESRI, SR-ORG, etc)</param>
        /// <param name="returnFormat">The return format for the spatial reference text (OGCWKT, ESRIWKT, PROJ4, etc</param>
        /// <returns>text spatial reference definition</returns>
        public static string GetSpatialReference(int code, Catalog catalog, Format returnFormat)
        {
            string url = "http://www.spatialreference.org/ref/" + CatalogText(catalog) + "/" + code.ToString() + "/" + FormatText(returnFormat) + "/";

            byte[] buf = new byte[8192];
            StringBuilder sb = new StringBuilder();

            // prepare the web page we will be asking for
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream resStream = response.GetResponseStream();

            string tempString = null;
            int count = 0;

            do
            {
                // fill the buffer with data
                count = resStream.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count != 0)
                {
                    // translate from bytes to ASCII text
                    tempString = Encoding.ASCII.GetString(buf, 0, count);

                    // continue building the string
                    sb.Append(tempString);
                }
            }
            while (count > 0); // any more data to read?

            return sb.ToString();
        }

        /// <summary>
        /// Gets the url text value for the catalog
        /// </summary>
        /// <param name="catalog">The catalog</param>
        /// <returns>url text value</returns>
        private static string CatalogText(Catalog catalog)
        {
            switch (catalog)
            {
                case Catalog.EPSG:
                    return "epsg";
                case Catalog.ESRI:
                    return "esri";
                case Catalog.IAU2000:
                    return "iau2000";
                case Catalog.SRORG:
                    return "sr-org";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Getsthe format text value for the url
        /// </summary>
        /// <param name="spatialreferenceFormat">The spatialreference format.</param>
        /// <returns>url text for format</returns>
        private static string FormatText(Format spatialreferenceFormat)
        {
            switch (spatialreferenceFormat)
            {
                case Format.Proj4:
                    return "proj4";
                case Format.OGCWKT:
                    return "ogcwkt";
                case Format.HumanReadableWKT:
                    return "prettywkt";
                case Format.GML:
                    return "gml";
                case Format.ESRIWKT:
                    return "esriwkt";
                case Format.USGS:
                    return "usgs";
                case Format.MapServerMapfile:
                    return "mapfile";
                case Format.MapServerPython:
                    return "mapserverpython";
                case Format.MapnikXML:
                    return "mapnik";
                case Format.MapnikPython:
                    return "mapnikpython";
                case Format.PostGISInsert:
                    return "postgis";
                case Format.Proj4js:
                    return "proj4js";
                default:
                    return string.Empty;
            }
        }
    }
}
