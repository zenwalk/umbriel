// <copyright file="GeoPhoto.cs" company="Umbriel Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-09-29</date>
// <summary>
////</summary>

namespace Umbriel.GIS.Photo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Text;
    using System.Web.UI;
    using ExifLibrary;
    using Umbriel.GIS;
    
    /// <summary>
    /// GeoPhoto class
    /// </summary>
    public class GeoPhoto
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeoPhoto"/> class.
        /// </summary>
        /// <param name="path">The path to the photo</param>
        public GeoPhoto(string path)
        {
            this.PhotoDateTime = null;
            Bitmap photo = new Bitmap(path);
            this.PhotoBitmap = photo;
            this.FilePath = path;
            
            this.ReadGPSCoordinate();
            this.ReadDateTaken();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoPhoto"/> class.
        /// </summary>
        /// <param name="photo">The photo.</param>
        public GeoPhoto(Bitmap photo)
        {
            this.PhotoBitmap = photo;

            this.ReadGPSCoordinate();
        }
        #endregion

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets the image direction.
        /// </summary>
        /// <value>The image direction.</value>
        public double ImageDirection { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [magnetic north].
        /// </summary>
        /// <value><c>true</c> if [magnetic north]; otherwise, <c>false</c>.</value>
        public bool MagneticNorth { get; private set; }

        /// <summary>
        /// Gets the coordinate.
        /// </summary>
        /// <value>The coordinate.</value>
        public ISpatialCoordinate Coordinate { get; private set; }
        
        /// <summary>
        /// Gets or sets the photo date time.
        /// </summary>
        /// <value>The photo date time.</value>
        public DateTime? PhotoDateTime { get; private set; }
        

        /// <summary>
        /// Gets or sets the photo bitmap.
        /// </summary>
        /// <value>The photo bitmap.</value>
        private Bitmap PhotoBitmap { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            string v = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("File Path: ");
            try
            {
                sb.AppendLine(this.FilePath.ToString());
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.StackTrace);
            }
            
            sb.AppendLine("Image Direction: " + this.ImageDirection.ToString());
            sb.AppendLine("Is Magnetic North:: " + this.MagneticNorth.ToString());
            sb.AppendLine("Latitude: " + this.Coordinate.Latitude.ToString());
            sb.AppendLine("Longitude: " + this.Coordinate.Longitude.ToString());

            v = sb.ToString();
            return v;
        }

        /// <summary>
        /// Reads the GPS coordinate.
        /// </summary>
        private void ReadGPSCoordinate()
        {
            if (this.PhotoBitmap != null)
            {
                ISpatialCoordinate coord = null;

                Bitmap photo = this.PhotoBitmap;
                
                // Extract exif metadata
                ExifFile file = ExifFile.Read(this.FilePath);

                foreach (ExifProperty exifProperty in file.Properties)
                {
                    Trace.WriteLine(exifProperty.Name.ToString() + "=" + exifProperty.Value.ToString());
                }

                if (file.Properties.ContainsKey(ExifTag.GPSImgDirection))
                {
                    this.SetImageDirection(file.Properties[ExifTag.GPSImgDirection].Value.ToString());
                }

                if (file.Properties.ContainsKey(ExifTag.GPSLatitude) && file.Properties.ContainsKey(ExifTag.GPSLongitude))
                {
                    float lon;
                    float lat;

                    GPSLatitudeLongitude latitude = (GPSLatitudeLongitude)file.Properties[ExifTag.GPSLatitude];
                    GPSLatitudeLongitude longitude = (GPSLatitudeLongitude)file.Properties[ExifTag.GPSLongitude];

                    lon = longitude.ToFloat();
                    lat = latitude.ToFloat();

                    if (file.Properties[ExifTag.GPSLongitudeRef].Value.ToString().StartsWith("W", StringComparison.CurrentCultureIgnoreCase))
                    {
                        lon = lon * -1;   
                    }

                    if (file.Properties[ExifTag.GPSLatitudeRef].Value.ToString().StartsWith("S", StringComparison.CurrentCultureIgnoreCase))
                    {
                        lat = lat * -1;
                    }

                    if (lat != 0 & lon != 0)
                    {
                        coord = new Umbriel.GIS.Geohash.Coordinate(lon, lat);
                        this.Coordinate = coord;
                    }                    
                }
            }
            else
            {
                throw new NullReferenceException();
            }
        }


        /// <summary>
        /// Gets the date the photo was taken.
        /// </summary>
        private void ReadDateTaken()
        {
            if (this.PhotoBitmap != null)
            {
                DateTime datetaken = DateTime.Now;

                Bitmap photo = this.PhotoBitmap;

                // Extract exif metadata
                ExifFile file = ExifFile.Read(this.FilePath);

                ExifProperty exifProperty = file.Properties[ExifTag.DateTime];

                if (exifProperty != null)
                {
                    try
                    {
                        datetaken = Convert.ToDateTime(exifProperty.Value);
                        this.PhotoDateTime = datetaken;
                    }
                    catch
                    {
                    }
                }
                
                return ;         
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        /// <summary>
        /// Sets the image direction.
        /// </summary>
        /// <param name="imageDirection">The image direction.</param>
        private void SetImageDirection(string imageDirection)
        {
            if (imageDirection.Contains(@"/"))
            {
                string[] tokens = imageDirection.Split('/');
                double operand1 = 0;
                double operand2 = 0;

                if (double.TryParse(tokens[0], out operand1) && double.TryParse(tokens[1], out operand2))
                {
                    double result = operand1 / operand2;
                    this.ImageDirection = result;
                }
            }
            else
            {
                double direction = 0;

                if (double.TryParse(imageDirection, out direction))
                {
                    this.ImageDirection = direction;
                }                
            }
        }
    }
}