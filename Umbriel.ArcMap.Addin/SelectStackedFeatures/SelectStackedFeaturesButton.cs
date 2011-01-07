// <copyright file="SelectStackedFeaturesButton.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cum30@co.henrico.va.us</email>
// <date>2010-10-15</date>
// <summary>SelectStackedFeaturesButton class file</summary>

namespace SelectStackedFeatures
{
    using System;
    using System.Collections.Generic;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;

    /// <summary>
    /// SelectStackedFeaturesButton Class for ArcMap 10 Addin
    /// </summary>
    public class SelectStackedFeaturesButton : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectStackedFeaturesButton"/> class.
        /// </summary>
        public SelectStackedFeaturesButton()
        {
 
        }

        /// <summary>
        /// Called when [click].
        /// </summary>
        protected override void OnClick()
        {
            ArcMap.Application.CurrentTool = null;

            IMxDocument doc = (IMxDocument)ArcMap.Application.Document;
            IFeatureLayer featureLayer = null;

            if (doc.SelectedLayer != null && doc.SelectedLayer is IFeatureLayer)
            {
                featureLayer = (IFeatureLayer)doc.SelectedLayer;
            }

            if (featureLayer != null)
            {
                IGeoFeatureLayer geofeatureLayer = (IGeoFeatureLayer)featureLayer;
                int featureCount = geofeatureLayer.FeatureClass.FeatureCount(null);

                IFeatureCursor cursor = featureLayer.Search(null, false);

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
                        this.OnMessageStatus("Geometry read: " + counter.ToString() + "  of  " + featureCount.ToString());
                    }
                }

                counter = 0;
                foreach (KeyValuePair<int, byte[]> item in allGeometries)
                {
                    counter++;

                    // IGeometry geometry = item.Value;
                    byte[] wkbAnalyze = item.Value;

                    this.OnMessageStatus("Analyzing geometry " + counter.ToString() + " of " + allGeometries.Count.ToString());

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

                doc.ActiveView.Refresh();

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
        /// Called when [update].
        /// </summary>
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
        
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

        /// <summary>
        /// Called when [message status].
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnMessageStatus(string message)
        {            
            // ArcMap.Application.StatusBar.set_Message(ArcMap.Application.hWnd, message);
            // System.Windows.Forms.Application.DoEvents();            
        }
    }
}
