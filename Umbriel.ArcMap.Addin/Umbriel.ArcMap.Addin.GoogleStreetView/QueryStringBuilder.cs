// <copyright file="GoogleMapsQStringBuilder.cs" company="Umbriel Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>unknown-see svn</date>
// <summary> class file for google maps QueryString Builder
////</summary>

namespace Umbriel.GIS.Google
{
    using System.Text;

    /// <summary>
    ///  Google Maps Querystring builder
    /// </summary>
    public class QueryStringBuilder
    {
        /// <summary>
        /// default google maps base url
        /// </summary>
        private const string DefaultGoogleMapsBaseURL = "http://maps.google.com/maps";

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryStringBuilder"/> class.
        /// </summary>
        public QueryStringBuilder()
        {
            this.BaseURL = DefaultGoogleMapsBaseURL;
            this.SetMapType(GoogleMapTypes.map);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryStringBuilder"/> class.
        /// </summary>
        /// <param name="baseURL">The base URL.</param>
        public QueryStringBuilder(string baseURL)
        {
            this.BaseURL = baseURL;
            this.SetMapType(GoogleMapTypes.map);
        }
        #endregion

        #region Enums
        /// <summary>
        /// Enumeration of Google Map map types
        /// </summary>
        public enum GoogleMapTypes
        {
            /// <summary>
            /// Map Google Map Type
            /// </summary>
            map = 0,

            /// <summary>
            /// Satellite View Google Map Type
            /// </summary>
            satellite = 1,

            /// <summary>
            /// Hybrid View Google Map Type
            /// </summary>
            hybrid = 2,

            /// <summary>
            /// Terrain View Google Map Type
            /// </summary>
            terrain = 3,
        }

        /// <summary>
        /// Enumeration of Google Streetview map layout configurations
        /// </summary>
        public enum StreetViewMapArrangement
        {
            /// <summary>
            /// half street view, half map
            /// </summary>
            Half = 11,

            /// <summary>
            /// mostly street view, corner map
            /// </summary>
            CornerMap = 12
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the base URL.
        /// </summary>
        /// <value>The base URL.</value>
        public string BaseURL { get; private set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>The latitude.</value>
        public double MapCenterLatitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        /// <value>The longitude.</value>
        public double MapCenterLongitude { get; set; }

        /// <summary>
        /// Gets or sets the street view parameter class
        /// </summary>
        /// <value>The street view.</value>
        public StreetViewParameter StreetView { get; set; }

        /// <summary>
        /// Gets or sets the query parameter (the q querystring parameter for google maps
        /// </summary>
        /// <value>The query parameter.</value>
        private string QueryParameter { get; set; }

        /// <summary>
        /// Gets or sets the type of the map.
        /// </summary>
        /// <value>The type of the map.</value>
        private GoogleMapTypes MapType { get; set; }

        /// <summary>
        /// Gets or sets the approximate span.
        /// </summary>
        /// <value>The approximate span.</value>
        private string ApproximateSpan { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Appends the query.
        /// </summary>
        /// <param name="q">The google maps query </param>
        public void AppendQuery(string q)
        {
            this.QueryParameter = q;
        }

        /// <summary>
        /// Appends the map center.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        public void AppendMapCenter(double latitude, double longitude)
        {
            this.MapCenterLatitude = latitude;
            this.MapCenterLongitude = longitude;
        }

        /// <summary>
        /// Sets the type of the map.
        /// </summary>
        /// <param name="mapType">Type of the map.</param>
        public void SetMapType(GoogleMapTypes mapType)
        {
            this.MapType = mapType;
        }

        /// <summary>
        /// Sets the approximate span.
        /// </summary>
        /// <param name="span">The approximate span of the map</param>
        public void SetApproximateSpan(string span)
        {
            this.ApproximateSpan = span;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(this.BaseURL);
            sb.Append('?');

            if (!string.IsNullOrEmpty(this.QueryParameter))
            {
                sb.Append("&q=");
                sb.Append(this.QueryParameter);
            }

            sb.Append("&ll=");
            sb.Append(this.MapCenterLatitude);
            sb.Append(',');
            sb.Append(this.MapCenterLongitude);

            if (this.StreetView != null)
            {
                sb.Append(this.StreetView.ToString());
            }

            return sb.ToString();
        }

        #endregion

        /// <summary>
        /// Class for the Google street view parameter
        /// </summary>
        public class StreetViewParameter
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="StreetViewParameter"/> class.
            /// </summary>
            public StreetViewParameter()
            {
                this.MapArrangement = StreetViewMapArrangement.CornerMap;
                this.Rotation = 0;
                this.TiltAngle = 0;
                this.ZoomLevel = 0;
                this.PitchAngle = 5;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="StreetViewParameter"/> class.
            /// </summary>
            /// <param name="latitude">The latitude.</param>
            /// <param name="longitude">The longitude.</param>
            public StreetViewParameter(double latitude, double longitude)
            {
                this.Latitude = latitude;
                this.Longitude = longitude;
                this.MapArrangement = StreetViewMapArrangement.CornerMap;
                this.Rotation = 0;
                this.TiltAngle = 0;
                this.ZoomLevel = 0;
                this.PitchAngle = 5;
            }

            /// <summary>
            /// Gets or sets the latitude.
            /// </summary>
            /// <value>The latitude.</value>
            public double Latitude { get; set; }

            /// <summary>
            /// Gets or sets the longitude.
            /// </summary>
            /// <value>The longitude.</value>
            public double Longitude { get; set; }

            /// <summary>
            /// Gets or sets the map arrangement.
            /// </summary>
            /// <value>The map arrangement.</value>
            public StreetViewMapArrangement MapArrangement { get; set; }

            /// <summary>
            /// Gets or sets the rotation.
            /// </summary>
            /// <value>The rotation.</value>
            public double Rotation { get; set; }

            /// <summary>
            /// Gets or sets the tilt angle.
            /// </summary>
            /// <value>The tilt angle.</value>
            public double TiltAngle { get; set; }

            /// <summary>
            /// Gets or sets the zoom level.
            /// </summary>
            /// <value>The zoom level.</value>
            public int ZoomLevel { get; set; }

            /// <summary>
            /// Gets or sets the pitch angle.
            /// </summary>
            /// <value>The pitch angle.</value>
            public double PitchAngle { get; set; }

            /// <summary>
            /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </returns>
            public override string ToString()
            {
                StringBuilder sb;
                sb = new StringBuilder("&layer=c");

                sb.Append("&cbll=");
                sb.Append(this.Latitude.ToString());
                sb.Append(',');
                sb.Append(this.Longitude.ToString());

                /*cbp= Street View window that accepts 5 parameters:
Street View/map arrangement, 11=upper half Street View and lower half map, 12=mostly Street View with corner map
Rotation angle/bearing (in degrees)
Tilt angle, -90 (straight up) to 90 (straight down)
Zoom level, 0-2
Pitch (in degrees) -90 (straight up) to 90 (straight down), default 5*/

                sb.Append("&cbp=");

                sb.Append(((int)this.MapArrangement));
                sb.Append(',');

                sb.Append(this.Rotation.ToString());
                sb.Append(',');

                sb.Append(this.TiltAngle.ToString());
                sb.Append(',');

                sb.Append(this.ZoomLevel.ToString());
                sb.Append(',');

                sb.Append(this.PitchAngle.ToString());

                return sb.ToString();
            }
        }
    }
}
