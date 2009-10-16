using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Text;
using System.IO;

namespace Umbriel.GIS
{
    public static class GeoJPEGHelper
    {
        public static ISpatialCoordinate GPSCoordinate(string path)
        {
            ISpatialCoordinate coord = null;

            Bitmap photo = new Bitmap(@"C:\temp\test.JPG");
            PropertyItem[] props = photo.PropertyItems;

            Goheer.EXIF.EXIFextractor er = new Goheer.EXIF.EXIFextractor(ref photo,"\n");

            string gpsLong = er["Gps Longitude"].ToString();
            string gpsLat = er["Gps Latitude"].ToString();

            double gpsLongCoord = 0;
            double gpsLatCoord = 0;

            if (double.TryParse(gpsLong, out gpsLongCoord) && double.TryParse(gpsLat, out gpsLatCoord))
            {
                coord = new Umbriel.GIS.Geohash.Coordinate();
                coord.Latitude = (decimal)gpsLatCoord;
                coord.Longitude = (decimal)gpsLatCoord;
            }

            return coord;
        }
    }
}
