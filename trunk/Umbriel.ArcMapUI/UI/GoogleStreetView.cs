// <copyright file="GoogleStreetView.cs" company="Umbriel Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>unknown-see svn</date>
// <summary> class file for GoogleStreetView command for ArcMap
////</summary>

namespace Umbriel.ArcMapUI.UI
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using ESRI.ArcGIS.ADF.BaseClasses;
    using ESRI.ArcGIS.ADF.CATIDs;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Framework;
    using ESRI.ArcGIS.Geometry;
    using Umbriel.GIS.Google;

    /// <summary>
    /// Summary description for GoogleStreetView.
    /// </summary>
    [Guid("640302e3-bcaf-4b85-82d3-ddd8a62af3cc")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Umbriel.ArcMapUI.UI.GoogleStreetView")]
    public sealed class GoogleStreetView : BaseCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private IApplication m_application;
        public GoogleStreetView()
        {
            base.m_category = "Umbriel";
            base.m_caption = "Launch Google Street View Website at map center.";
            base.m_message = "Launch Google Street View Website at map center.";
            base.m_toolTip = "Launch Google Street View Website at map center.";
            base.m_name = "Umbriel_GoogleStreetView";

            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            m_application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            try
            {
                IMxDocument doc = (IMxDocument)m_application.Document;

                if (doc.FocusMap.SpatialReference != null || doc.FocusMap.SpatialReference is IUnknownCoordinateSystem)
                {
                    ISpatialReference srWGS84 = Umbriel.ArcMap.Editor.Util.Geohasher.WGS84SpatialReference();
                    ISpatialReference srMap = doc.FocusMap.SpatialReference;

                    IEnvelope env = doc.ActiveView.Extent;

                    IPoint pt;
                    
                    double metersPerUnit = 1;

                    if (srMap is IProjectedCoordinateSystem)
                    {
                        IProjectedCoordinateSystem pcs = (IProjectedCoordinateSystem)srMap;
                        metersPerUnit = pcs.CoordinateUnit.MetersPerUnit;
                    }

                    srWGS84.SetFalseOriginAndUnits(-180, -90, 1000000);

                    env.Project(srWGS84);

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

        #endregion
    }
}
