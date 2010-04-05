// <copyright file="Functions.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-04-05</date>
// <summary>Functions class file</summary>

/*
 * http://weblogs.asp.net/farhank/archive/2008/07/04/creating-clr-sql-user-defined-function-to-validate-values-using-regular-expression.aspx
 */
namespace Umbriel.GIS.GeohashFunctions
{
    using System.Data.SqlTypes;
    using System.Text.RegularExpressions;
    using Microsoft.SqlServer.Server;

    /// <summary>
    /// Geohash static methods
    /// </summary>
    public partial class Functions
    {
        /// <summary>
        /// Encodes the coordinate.
        /// </summary>
        /// <param name="latitude">The latitude</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns>sqlstring geohash of the coordinate</returns>
        [SqlFunction(Name = "EncodeCoordinate")]
        public static SqlString EncodeCoordinate(SqlDouble latitude, SqlDouble longitude)
        {
            double lat = (double)latitude;
            double lon = (double)longitude;

            string geohash = Umbriel.GIS.Geohash.EncodeCoordinate(lat, lon);

            SqlString sqlgeohash = new SqlString(geohash);

            return sqlgeohash;
        }

        /// <summary>
        /// Encodes the coordinate.
        /// </summary>
        /// <param name="geohash">The geohash.</param>
        /// <returns>x,y coordinate of the geohash</returns>
        [SqlFunction(Name = "DecodeGeohash")]
        public static SqlDouble[] DecodeGeohash(SqlString geohash)
        {
            ISpatialCoordinate coord = Geohash.DecodeGeoHash(geohash.ToString().ToCharArray());

            SqlDouble[] coordinate = { new SqlDouble((double)coord.Latitude), new SqlDouble((double)coord.Longitude) };

            return coordinate;
        }
        
        /// <summary>
        /// Geohashes the longitude.
        /// </summary>
        /// <param name="geohash">The geohash.</param>
        /// <returns>The Longitude value of a lat/long coordinate</returns>
        [SqlFunction(Name = "GeohashLongitude")]
        public static SqlDouble GeohashLongitude(SqlString geohash)
        {
            ISpatialCoordinate coord = Geohash.DecodeGeoHash(geohash.ToString().ToCharArray());
            return new SqlDouble((double)coord.Longitude);
        }

        /// <summary>
        /// Geohashes the latitude.
        /// </summary>
        /// <param name="geohash">The geohash.</param>
        /// <returns>The latitude value of a lat/long coordinate</returns>
        [SqlFunction(Name = "GeohashLatitude")]
        public static SqlDouble GeohashLatitude(SqlString geohash)
        {
            ISpatialCoordinate coord = Geohash.DecodeGeoHash(geohash.ToString().ToCharArray());
            return new SqlDouble((double)coord.Latitude);
        }
    }
}