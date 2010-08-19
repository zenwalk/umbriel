// <copyright file="OpenGoogleStreetView.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-08-19</date>
// <summary>OpenGoogleStreetView class file</summary>

namespace Umbriel.ArcMap.Addin.GoogleStreetView
{
    using System;
    using System.Diagnostics;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.Geometry;
    using Umbriel.GIS.Google;

    /// <summary>
    /// OpenGoogleStreetView button
    /// </summary>
    public class OpenGoogleStreetView : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGoogleStreetView"/> class.
        /// </summary>
        public OpenGoogleStreetView()
        {
            Trace.WriteLine("CTOR OpenGoogleStreetView");
        }

        /// <summary>
        /// Called when [click].
        /// </summary>
        protected override void OnClick()
        {
            Trace.WriteLine("OpenGoogleStreetView_OnClick");

            ArcMap.Application.CurrentTool = null;
            IMap map = ArcMap.Document.FocusMap;
            
            try
            {
                IMxDocument doc = ArcMap.Document;

                if (doc.FocusMap.SpatialReference != null || doc.FocusMap.SpatialReference is IUnknownCoordinateSystem)
                {
                    ISpatialReference spatialrefWGS84 = this.WGS84SpatialReference();
                    ISpatialReference spatialrefMap = doc.FocusMap.SpatialReference;

                    IEnvelope env = doc.ActiveView.Extent;

                    IPoint pt;

                    double metersPerUnit = 1;

                    if (spatialrefMap is IProjectedCoordinateSystem)
                    {
                        IProjectedCoordinateSystem pcs = (IProjectedCoordinateSystem)spatialrefMap;
                        metersPerUnit = pcs.CoordinateUnit.MetersPerUnit;
                    }

                    spatialrefWGS84.SetFalseOriginAndUnits(-180, -90, 1000000);

                    env.Project(spatialrefWGS84);

                    IArea extentArea = (IArea)env;

                    pt = extentArea.Centroid;

                    QueryStringBuilder querystring = new QueryStringBuilder();

                    querystring.MapCenterLatitude = pt.Y;
                    querystring.MapCenterLongitude = pt.X;

                    QueryStringBuilder.StreetViewParameter streetviewParameter = new QueryStringBuilder.StreetViewParameter(pt.Y, pt.X);

                    querystring.StreetView = streetviewParameter;

                    string url = querystring.ToString();

                    Trace.WriteLine("url=" + url);

                    Process.Start(url);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(
                        "A data frame spatial reference must be specified in order to use this tool.",
                        "Umbriel Google Street View",
                        System.Windows.Forms.MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);

                System.Windows.Forms.MessageBox.Show(
                    "GoogleStreetView Command Error: " + ex.Message + "\n\n" + ex.StackTrace,
                    "Umbriel.GoogleStreetView",
                     System.Windows.Forms.MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// Called when [update].
        /// </summary>
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }

        /// <summary>
        /// Creates a WGS84 Spatial Reference
        /// </summary>
        /// <returns>ISpatialReference WGS84</returns>
        private ISpatialReference WGS84SpatialReference()
        {
            Trace.WriteLine("OpenGoogleStreetView_WGS84SpatialReference");

            SpatialReferenceEnvironment spatialReferenceEnv = new SpatialReferenceEnvironmentClass();

            IGeographicCoordinateSystem geoCS = spatialReferenceEnv.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
            ISpatialReference spatialReference = geoCS;
            spatialReference.SetFalseOriginAndUnits(-180, -90, 1000000);

            return spatialReference;
        }
    }
}
