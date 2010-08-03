// <copyright file="SelectStackGeometries.cs" company="Umbriel Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>unknown-see svn</date>
// <summary> class file for SelectStackGeometries command for ArcMap
////</summary>

namespace Umbriel.ArcMapUI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using ESRI.ArcGIS.ADF.BaseClasses;
    using ESRI.ArcGIS.ADF.CATIDs;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.Framework;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;

    /// <summary>
    /// SelectStackGeometries ArcMap Command
    /// </summary>
    [Guid("3d87d6c5-d800-4717-8414-dea69ec0e1c7")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Umbriel.ArcMapUI.SelectStackGeometries")]
    public sealed class SelectStackGeometries : BaseCommand
    {
        #region COM Registration Function(s)
        /// <summary>
        /// Registers the function.
        /// </summary>
        /// <param name="registerType">Type of the register.</param>
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

        /// <summary>
        /// Unregisters the function.
        /// </summary>
        /// <param name="registerType">Type of the register.</param>
        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);
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

        /// <summary>
        /// ArcMap IApplication ref
        /// </summary>
        private IApplication m_application;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectStackGeometries"/> class.
        /// </summary>
        public SelectStackGeometries()
        {
            System.Diagnostics.Trace.WriteLine("SelectStackGeometries Constructor");

            base.m_category = "Umbriel";
            base.m_caption = "Select Stacked Geometries";
            base.m_message = "Highlight the layer you wish to analyze and press this button.";
            base.m_toolTip = "Highlight the layer you wish to analyze and press this button.";
            base.m_name = "Umbriel_SelectStackGeometries";

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
            {
                return;
            }

            m_application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
            {
                base.m_enabled = true;
            }
            else
            {
                base.m_enabled = false;
            }
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            IMxDocument mxDoc = (IMxDocument)m_application.Document;
            IFeatureLayer featureLayer = null;

            if (mxDoc.SelectedLayer != null && mxDoc.SelectedLayer is IFeatureLayer)
            {
                featureLayer = (IFeatureLayer)mxDoc.SelectedLayer;
            }

            if (featureLayer != null)
            {
                IGeoFeatureLayer geofeatureLayer = (IGeoFeatureLayer)featureLayer;
                int featureCount = geofeatureLayer.FeatureClass.FeatureCount(null);

                IFeatureCursor cursor = featureLayer.Search(null, false);

                // Dictionary<int, IGeometry> allGeometries = new Dictionary<int, IGeometry>();
                Dictionary<int, byte[]> allGeometries = new Dictionary<int, byte[]>();

                List<int> oids = new List<int>();

                IFeature feature = null;
                int counter = 0;
                while ((feature = cursor.NextFeature()) != null)
                {
                    counter++;

                    byte[] wkb = ConvertGeometryToWKB(feature.Shape);

                    allGeometries.Add(feature.OID, wkb);

                    if (counter % 500 == 0)
                    {
                        OnMessageStatus("Geometry read: " + counter.ToString() + "  of  " + featureCount.ToString());
                    }
                }

                counter = 0;
                foreach (KeyValuePair<int, byte[]> item in allGeometries)
                {
                    counter++;

                    // IGeometry geometry = item.Value;
                    byte[] wkbAnalyze = item.Value;

                    OnMessageStatus("Analyzing geometry " + counter.ToString() + " of " + allGeometries.Count.ToString());

                    foreach (KeyValuePair<int, byte[]> checkItem in allGeometries)
                    {
                        if (checkItem.Key != item.Key)
                        {
                            try
                            {
                                if (UnsafeCompare(checkItem.Value, wkbAnalyze))
                                {
                                    // System.Diagnostics.Trace.WriteLine("Stack Found!");
                                    oids.Add(checkItem.Key);
                                }

                                //if (Convert.ToBase64String(wkbAnalyze) == Convert.ToBase64String(checkItem.Value))
                                //{
                                //    System.Diagnostics.Trace.WriteLine("Stack Found!");
                                //}          
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                            }
                        }
                    }
                }

                if (oids.Count > 0)
                {
                    IFeatureSelection featureSelection = (IFeatureSelection)featureLayer;
                    featureSelection.Clear();

                    foreach (int g in oids)
                    {
                        featureSelection.SelectionSet.Add(g);
                    }
                }

                mxDoc.ActiveView.Refresh();

                System.Windows.Forms.MessageBox.Show(
    "Stack Finding complete! Analyzed " + counter.ToString() + " geometries and found " + oids.Count.ToString() + " stacked features.",
    "Select Stack Geometries",
    System.Windows.Forms.MessageBoxButtons.OK);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(
                    "You must highlight a feature layer in the Table of Contents.",
                    "Select Stack Geometries",
                    System.Windows.Forms.MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// Called when [message status].
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnMessageStatus(string message)
        {
            m_application.StatusBar.set_Message(0, message);
        }

        #endregion

        /// <summary>
        /// Converts the geometry to WKB.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns>WKB byte array</returns>
        private static byte[] ConvertGeometryToWKB(IGeometry geometry)
        {
            IWkb wkb = geometry as IWkb;
            ITopologicalOperator oper = geometry as ITopologicalOperator;
            oper.Simplify();

            IGeometryFactory3 factory = new GeometryEnvironment() as IGeometryFactory3;
            byte[] b = factory.CreateWkbVariantFromGeometry(geometry) as byte[];
            return b;
        }

        /// <summary>
        /// Unsafes the compare.
        /// </summary>
        /// <param name="a1">The first byte array.</param>
        /// <param name="a2">The second byte array.</param>
        /// <returns>boolean true if equal</returns>
        private static unsafe bool UnsafeCompare(byte[] a1, byte[] a2)
        {
            if (a1 == null || a2 == null || a1.Length != a2.Length)
            {
                return false;
            }
            fixed (byte* p1 = a1, p2 = a2)
            {
                byte* x1 = p1, x2 = p2;
                int l = a1.Length;
                for (int i = 0; i < l / 8; i++, x1 += 8, x2 += 8)
                    if (*((long*)x1) != *((long*)x2)) return false;
                if ((l & 4) != 0) { if (*((int*)x1) != *((int*)x2)) return false; x1 += 4; x2 += 4; }
                if ((l & 2) != 0) { if (*((short*)x1) != *((short*)x2)) return false; x1 += 2; x2 += 2; }
                if ((l & 1) != 0) if (*((byte*)x1) != *((byte*)x2)) return false;
                return true;
            }
        }
    }
}
