// <copyright file="Geohash.cs" company="Jay Cummins">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-08-03</date>
// <summary>
// GeoHash implementation  based on David Troy's geohash-native.c : 
//
//
// geohash-native.c
// (c) 2008 David Troy
// dave@roundhousetech.com
// 
// (The MIT License)
// 
// Copyright (c) 2008 David Troy, Roundhouse Technologies LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// 'Software'), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////</summary>

namespace Umbriel.GIS
{
    using System;
    using System.Diagnostics;
    using System.Text;

    #region Interfaces
    /// <summary>
    /// IPoint2D interface
    /// </summary>
    public interface IPoint2D
    {
        /// <summary>
        /// Gets or sets the X coordinate property
        /// </summary>
        /// <value>The X coordinate value</value>
        double X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate property
        /// </summary>
        /// <value>The Y coordinate</value>
        double Y { get; set; }
    }

    /// <summary>
    /// ISpatialCoordinate interface
    /// </summary>
    public interface ISpatialCoordinate
    {
        /// <summary>
        /// Gets or sets the latitude decimal property
        /// </summary>
        /// <value>The latitude decimal value</value>
        decimal Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude decimal property
        /// </summary>
        /// <value>The longitude.</value>
        decimal Longitude { get; set; }
    }
    #endregion



    /// <summary>
    /// GeoHash class for encoding/decoding geohashes
    /// 
	/// 
	/// GeoHash implementation  based on David Troy's geohash-native.c : 
    ///     
    ///     
    /// geohash-native.c
    /// (c) 2008 David Troy
    /// dave@roundhousetech.com
    /// 
    /// (The MIT License)
    /// 
    /// Copyright (c) 2008 David Troy, Roundhouse Technologies LLC
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining
    /// a copy of this software and associated documentation files (the
    /// 'Software'), to deal in the Software without restriction, including
    /// without limitation the rights to use, copy, modify, merge, publish,
    /// distribute, sublicense, and/or sell copies of the Software, and to
    /// permit persons to whom the Software is furnished to do so, subject to
    /// the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be
    /// included in all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND,
    /// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    /// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
    /// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
    /// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
    /// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
    /// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
    /// </summary>
    public class Geohash
    {
        /// <summary>
        /// character map for geohash
        /// </summary>
        private const string BASE32 = "0123456789bcdefghjkmnpqrstuvwxyz";

        #region Fields
        /// <summary>
        /// integer bit array
        /// </summary>
        private static readonly int[] BITS = new int[] { 16, 8, 4, 2, 1 };

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Geohash"/> class.
        /// </summary>
        /// <param name="geohash">The geohash.</param>
        public Geohash(string geohash)
        {
            if (!string.IsNullOrEmpty(geohash))
            {
                this.GeoHashString = geohash;
                this.GeoHashCoordinate = DecodeGeoHash(this.GeoHashString.ToCharArray());
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the geohash string property
        /// </summary>
        /// <value>The geohash string value</value>
        public string GeoHashString { get; private set; }
        #endregion

        /// <summary>
        /// Gets the geohash coordinate.
        /// </summary>
        /// <value>The coordinate of the geohash </value>
        public ISpatialCoordinate GeoHashCoordinate { get; private set; }

        /// <summary>
        /// Decodes the geohash string, returns ISpatialCoordinate
        /// </summary>
        /// <param name="geohash">The geohash character array</param>
        /// <returns>ISpatialCoordinate of the geohash</returns>
        public static ISpatialCoordinate DecodeGeoHash(char[] geohash)
        {
            double[] lat = new double[2];
            double[] lon = new double[2];
            double lat_err = 0;
            double lon_err = 0;

            lat[0] = -90.0;
            lat[1] = 90.0;
            lon[0] = -180.0;
            lon[1] = 180.0;
            lat_err = 90.0;
            lon_err = 180.0;

            int hashLength = geohash.Length;

            bool is_even = true;

            for (int i = 0; i < hashLength; i++)
            {
                char c = geohash[i];

                int cd = BASE32.IndexOf(c);

                //Trace.WriteLine("char c = " + c.ToString());
                //Trace.WriteLine("int cd = " + cd.ToString());

                //Trace.WriteLine("bin cd = " + Convert.ToString(cd, 2));

                for (int j = 0; j < BITS.Length; j++)
                {
                    int mask = BITS[j];

                    if (is_even)
                    {
                        lon_err /= 2;

                        int n = Convert.ToInt32(!(((byte)cd & mask) == mask));
                        lon[n] = (lon[0] + lon[1]) / 2;
                    }
                    else
                    {
                        lat_err /= 2;

                        int n = Convert.ToInt32(!(((byte)cd & mask) == mask));
                        lat[n] = (lat[0] + lat[1]) / 2;
                    }

                    is_even = !is_even;
                }
            }

            ISpatialCoordinate coord = new Coordinate();

            coord.Latitude = (decimal)((lat[0] + lat[1]) / 2);
            coord.Longitude = (decimal)((lon[0] + lon[1]) / 2);

            return coord;
        }

        /// <summary>
        /// Encodes the coordinate.
        /// </summary>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <param name="precision">The precision of the geohash</param>
        /// <returns>geohash string </returns>
        public static string EncodeCoordinate(double latitude, double longitude, int precision)
        {
            StringBuilder hashBuilder = new StringBuilder();

            bool is_even = true;

            double[] lat = new double[2];
            double[] lon = new double[2];

            lat[0] = -90.0;
            lat[1] = 90.0;
            lon[0] = -180.0;
            lon[1] = 180.0;

            double mid;

            int bit = 0;
            int ch = 0;
            int i = 0;

            while (i < precision)
            {
                if (is_even)
                {
                    mid = (lon[0] + lon[1]) / 2;
                    if (longitude > mid)
                    {
                        ch |= BITS[bit];
                        lon[0] = mid;
                    }
                    else
                    {
                        lon[1] = mid;
                    }
                }
                else
                {
                    mid = (lat[0] + lat[1]) / 2;
                    if (latitude > mid)
                    {
                        ch |= BITS[bit];
                        lat[0] = mid;
                    }
                    else
                    {
                        lat[1] = mid;
                    }
                }

                is_even = !is_even;
                if (bit < 4)
                {
                    bit++;
                }
                else
                {
                    // geohash[i++] = BASE32[ch];
                    i++;
                    hashBuilder.Append(BASE32[ch]);
                    bit = 0;
                    ch = 0;
                }
            }

            // geohash[i] = 0;
            return hashBuilder.ToString();
        }

        /// <summary>
        /// Encodes the coordinate.
        /// </summary>
        /// <param name="spatialCoord">The spatial coordindate reference</param>
        /// <param name="precision">The precision of the geohash</param>
        /// <returns>geohash string </returns>
        public static string EncodeCoordinate(ISpatialCoordinate spatialCoord, int precision)
        {
            return EncodeCoordinate((double)spatialCoord.Latitude, (double)spatialCoord.Longitude, precision);            
        }

        /// <summary>
        /// Encodes the coordinate.
        /// </summary>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <returns>geohash string </returns>
        public static string EncodeCoordinate(double latitude, double longitude)
        {
            const int DEFAULT_PRECISION = 13;
            return EncodeCoordinate(latitude, longitude, DEFAULT_PRECISION);            
        }

        /// <summary>
        /// Coordinate Point Class
        /// </summary>
        public class Coordinate : IPoint2D, ISpatialCoordinate
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Coordinate"/> class.
            /// </summary>
            public Coordinate()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Coordinate"/> class.
            /// </summary>
            /// <param name="x">The x ordinate  value</param>
            /// <param name="y">The y ordinate  value</param>
            public Coordinate(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }

            /// <summary>
            /// Gets or sets the X coordinate property
            /// </summary>
            /// <value>The X coordinate</value>
            public double X { get; set; }

            /// <summary>
            /// Gets or sets the Y coordinate property
            /// </summary>
            /// <value>The Y coordinate</value>
            public double Y { get; set; }

            #region ISpatialCoordinate Members

            /// <summary>
            /// Gets or sets the latitude decimal property
            /// </summary>
            /// <value>The latitude decimal value</value>
            public decimal Latitude
            {
                get
                {
                    return (decimal)this.Y;
                }

                set
                {
                    this.Y = (double)value;
                }
            }

            /// <summary>
            /// Gets or sets the longitude decimal property
            /// </summary>
            /// <value>The longitude.</value>
            public decimal Longitude
            {
                get
                {
                    return (decimal)this.X;
                }

                set
                {
                    this.X = (double)value;
                }
            }

            #endregion

            /// <summary>
            /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </returns>
            public override string ToString()
            {
                return this.Latitude.ToString() + ' ' + this.Longitude.ToString();
            }
        }
    }
}

