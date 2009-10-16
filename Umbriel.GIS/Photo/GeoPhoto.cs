// <copyright file="GeoPhoto.cs" company="Earth">
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
    using Umbriel.GIS;

    public class GeoPhoto
    {
        public string FilePath { get; private set; }
        public double ImageDirection { get; private set; }
        public bool MagneticNorth { get; private set; }
        
        public ISpatialCoordinate Coordinate { get; private set; }

        private Bitmap PhotoBitmap { get; set; }

        public GeoPhoto(string path)
        {            
            Bitmap photo = new Bitmap(path);
            this.PhotoBitmap = photo;
            
            ReadGPSCoordinate();
        }

        public GeoPhoto(Bitmap photo)
        {
            this.PhotoBitmap = photo;

            ReadGPSCoordinate();
        }

        public override string ToString()
        {
            string v = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append("File Path: ");
            try
            {
                sb.AppendLine(this.FilePath.ToString());
            }
            catch{}
            
            sb.AppendLine("Image Direction: " + this.ImageDirection.ToString());
            sb.AppendLine("Is Magnetic North:: " + this.MagneticNorth.ToString());
            sb.AppendLine("Latitude: " + this.Coordinate.Latitude.ToString());
            sb.AppendLine("Longitude: " + this.Coordinate.Longitude.ToString());

            v = sb.ToString();
            return v;
        }

        private void ReadGPSCoordinate()
        {
            if (this.PhotoBitmap != null)
            {
                ISpatialCoordinate coord = null;

                Bitmap photo = this.PhotoBitmap;

                Goheer.EXIF.EXIFextractor er = new Goheer.EXIF.EXIFextractor(ref photo, "\n");

                string gpsLong = er["Gps Longitude"].ToString();
                string gpsLat = er["Gps Latitude"].ToString();

                string imageDirection = er["Gps ImgDir"].ToString();
                SetImageDirection(imageDirection);

                string imageDirectionRef = er["Gps ImgDirRef"].ToString();

                    this.MagneticNorth = (imageDirectionRef.StartsWith("M"));




                int longRef = 1, latRef = 1;


                if (er["Gps LongitudeRef"].ToString().Equals("W", StringComparison.CurrentCultureIgnoreCase))
                {
                    longRef = -1;
                }

                if (er["Gps LatitudeRef"].ToString().Equals("S", StringComparison.CurrentCultureIgnoreCase))
                {
                    latRef = -1;
                }

                double gpsLongCoord = 0;
                double gpsLatCoord = 0;
                double gpsImageDirection = 0;

                if (double.TryParse(gpsLong, out gpsLongCoord) && double.TryParse(gpsLat, out gpsLatCoord))                   
                {
                    coord = new Umbriel.GIS.Geohash.Coordinate();
                    coord.Latitude = (decimal)gpsLatCoord * latRef;
                    coord.Longitude = (decimal)gpsLongCoord * longRef;
                    this.Coordinate = coord;
                }

            }
            else
            {
                throw new NullReferenceException();
            }
        }

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