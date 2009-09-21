// <copyright file="BatchExtend.cs" company="Earth">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-08-06</date>
// <summary>Feature Extender class file</summary>

namespace Umbriel.ArcMap.Editor
{
    using System;
    using System.Collections.Generic;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.Framework;
    using ESRI.ArcGIS.Editor;
    using ESRI.ArcGIS.Geometry;
    using ESRI.ArcGIS.Geodatabase;

    /// <summary>
    /// enum from to line
    /// </summary>
    internal enum LineEndPoint
    {
        /// <summary>
        /// from vertex of line
        /// </summary>
        From,

        /// <summary>
        /// to vertex of line
        /// </summary>
        To
    }

    /// <summary>
    /// TargetItem structure for results from spatial query
    /// </summary>
    internal struct TargetItem
    {
        /// <summary>
        /// 
        /// </summary>
        internal IFeature targetFeature;
        internal LineEndPoint nearestEndPoint;

        internal TargetItem(IFeature feature, LineEndPoint endPoint)
        {
            targetFeature = feature;
            nearestEndPoint = endPoint;
        }

    }

    public delegate void ExtendMessageEventHandler(string message);
    public delegate void ExtendFeatureProgressHandler(int currentItemIndex, int totalItems, string message);
    public delegate void AfterExtendFeaturesHandler(int featureAttempted, int featuresExtended, int featuresNotExtended);

    public sealed class FeatureExtender
    {
        private List<IFeatureLayer> FeatureLayers { get; set; }
        private IMxDocument MxDocument { get; set; }
        private IApplication ArcMapApplication { get; set; }
        private IEditor EditorExtension { get; set; }

        private int CounterExtended { get; set; }
        private int CounterNotExtended { get; set; }

        public bool Simulation { get; set; }
        public double SearchTolerance { get; set; }


        #region Events
        public event ExtendMessageEventHandler ExtendMessageEvent;
        public event ExtendFeatureProgressHandler ExtendFeatureProgressEvent;
        public event AfterExtendFeaturesHandler AfterExtendFeaturesEvent;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureExtender"/> class.
        /// </summary>
        /// <param name="layers">The layers.</param>
        /// <param name="arcmapApplication">The arcmap application.</param>
        public FeatureExtender(List<IFeatureLayer> layers, IApplication arcmapApplication)
        {
            this.FeatureLayers = layers;
            this.ArcMapApplication = arcmapApplication;
            this.MxDocument = (IMxDocument)arcmapApplication.Document;
            this.EditorExtension = (IEditor)this.ArcMapApplication.FindExtensionByName("ESRI Object Editor");
            this.Simulation = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureExtender"/> class.
        /// </summary>
        /// <param name="layers">The layers.</param>
        /// <param name="arcmapApplication">The arcmap application.</param>
        /// <param name="searchTolerance">The search tolerance.</param>
        public FeatureExtender(List<IFeatureLayer> layers, IApplication arcmapApplication, double searchTolerance)
        {
            this.SearchTolerance = searchTolerance;
            this.FeatureLayers = layers;
            this.ArcMapApplication = arcmapApplication;
            this.MxDocument = (IMxDocument)arcmapApplication.Document;
            this.EditorExtension = (IEditor)this.ArcMapApplication.FindExtensionByName("ESRI Object Editor");
            this.Simulation = true;
        }
        #endregion


        public void Extend(IFeatureSelection featureSelection)
        {
            Extend(featureSelection, this.SearchTolerance);
        }

        public void Extend(IFeatureSelection featureSelection, double searchTolerance)
        {
            IFeatureLayer featureLayer = (IFeatureLayer)featureSelection;
            IEnumIDs enumIDs = featureSelection.SelectionSet.IDs;
            int i = -1;
            int c = 0;
            int t = featureSelection.SelectionSet.Count;

            this.CounterExtended = 0;
            this.CounterNotExtended = 0;

            while ((i = enumIDs.Next()) != -1)
            {
                try
                {
                    c++;
                    OnExtendFeatureProgress(c, t, "Begin");
                    IFeature feature = featureLayer.FeatureClass.GetFeature(i);
                    Extend(feature, searchTolerance);
                    OnExtendFeatureProgress(c, t, "End");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("Exception on " + c.ToString() + " of " + t.ToString());
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                }
            }

            OnAfterExtendFeatures(t, this.CounterExtended, this.CounterNotExtended);
        }

        /// <summary>
        /// Extends the specified feature.
        /// </summary>
        /// <param name="feature">The feature.</param>
        public void Extend(IFeature feature)
        {
            Extend(feature, this.SearchTolerance);
        }

        /// <summary>
        /// Extends the specified feature.
        /// </summary>
        /// <param name="feature">The feature to extend</param>
        /// <param name="searchTolerance">The search tolerance.</param>
        public void Extend(IFeature feature, double searchTolerance)
        {
            try
            {
                IPolyline polyLine = (IPolyline)feature.Shape;

                // TODO: drop a TargetItem from the list if it has been extended.
                foreach (IFeatureLayer layer in this.FeatureLayers)
                {
                    List<TargetItem> targetFeatures = GetTargetFeatures(layer, searchTolerance, (IGeometry)polyLine);

                    if (targetFeatures.Count.Equals(1))
                    {
                        TargetItem targetFeatureItem = targetFeatures[0];

                        IConstructCurve constructCurve = new PolylineClass();

                        esriSegmentExtension ext = esriSegmentExtension.esriNoExtension;

                        if (targetFeatures[0].nearestEndPoint.Equals(LineEndPoint.From))
                        {
                            ext = esriSegmentExtension.esriExtendAtFrom;
                        }
                        else
                        {
                            ext = esriSegmentExtension.esriExtendAtTo;
                        }

                        bool extPerformed = false;

                        constructCurve.ConstructExtended((ICurve)polyLine, (ICurve)targetFeatureItem.targetFeature.Shape, (int)esriCurveExtension.esriRelocateEnds, ref extPerformed);

                        if (extPerformed)
                        {
                            IPolyline newLine = (IPolyline)constructCurve;

                            System.Diagnostics.Debug.WriteLine("original length=  " + polyLine.Length.ToString());
                            System.Diagnostics.Debug.WriteLine("newLine length=  " + newLine.Length.ToString());

                            OnExtendMessage("Feature {OID} extended!!!", feature);

                            if (!this.Simulation)
                            {
                                UpdateGeometry(feature, (IGeometry)newLine);
                            }

                            CounterExtended++;
                        }
                        else
                        {
                            OnExtendMessage("Feature {OID}  could not be extended.", feature);
                            CounterNotExtended++;
                        }
                    }
                    else if (targetFeatures.Count.Equals(0))
                    {
                        OnExtendMessage("Feature {OID}  is not within any features in: " + layer.Name, feature);
                        CounterNotExtended++;
                    }
                    else
                    {
                        // TODO: handle multiple targets.
                        OnExtendMessage("Feature {OID}  is within multiple features in: " + layer.Name + ".  Cannot extend this feature.", feature);
                        CounterNotExtended++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }


        /// <summary>
        /// Gets the target features.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="searchTolerance">The search tolerance.</param>
        /// <param name="featureGeom">The feature geom.</param>
        /// <returns></returns>
        private List<TargetItem> GetTargetFeatures(IFeatureLayer layer, double searchTolerance, IGeometry featureGeom)
        {
            IPolyline polyLine = (IPolyline)featureGeom;

            ISpatialFilter filter = new SpatialFilterClass();
            IGeometry geometry;
            IFeatureCursor cursor;
            IFeature cursorFeature;
            ITopologicalOperator topoOperator;

            List<TargetItem> targetFeatures = new List<TargetItem>();

            //first, try the from point:
            topoOperator = (ITopologicalOperator)polyLine.FromPoint;
            geometry = topoOperator.Buffer(searchTolerance);
            filter.Geometry = geometry;
            filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

            cursor = layer.Search(filter, false);

            while ((cursorFeature = cursor.NextFeature()) != null)
            {
                targetFeatures.Add(new TargetItem(cursorFeature, LineEndPoint.From));
            }

            ReleaseCOMObject(cursor);

            //if the count is empty, try it with the other end:
            if (targetFeatures.Count.Equals(0))
            {
                topoOperator = (ITopologicalOperator)polyLine.ToPoint;
                geometry = topoOperator.Buffer(searchTolerance);
                filter.Geometry = geometry;
                filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                cursor = layer.Search(filter, false);

                while ((cursorFeature = cursor.NextFeature()) != null)
                {
                    targetFeatures.Add(new TargetItem(cursorFeature, LineEndPoint.To));
                }

                ReleaseCOMObject(cursor);
            }

            return targetFeatures;
        }

        /// <summary>
        /// Called when [extend message].
        /// </summary>
        /// <param name="message">The extend status message</param>
        private void OnExtendMessage(string message, IFeature feature)
        {
            if (this.ExtendMessageEvent != null)
            {
                string oidstring = string.Empty;
                if (feature != null && feature.HasOID)
                {
                    oidstring = "(OID=" + feature.OID.ToString() + ")";
                }

                message = message.Replace("{OID}", oidstring);

                this.ExtendMessageEvent(message);
            }
        }

        /// <summary>
        /// Called when [extend feature progress].
        /// </summary>
        /// <param name="i">The current index of the feature being extended.</param>
        /// <param name="t">The total number of features being extended.</param>
        /// <param name="message">The message.</param>
        private void OnExtendFeatureProgress(int i, int t, string message)
        {
            if (this.ExtendFeatureProgressEvent != null)
            {
                this.ExtendFeatureProgressEvent(i, t, message);
            }
        }

        /// <summary>
        /// Called when [after extend features handler].
        /// </summary>
        /// <param name="total">The total.</param>
        /// <param name="extended">The extended.</param>
        /// <param name="failed">The failed.</param>
        private void OnAfterExtendFeatures(int total, int extended, int failed)
        {
            if (this.AfterExtendFeaturesEvent != null)
            {
                this.AfterExtendFeaturesEvent(total, extended, failed);
            }
        }
        private void UpdateGeometry(IFeature feature, IGeometry newGeometry)
        {
            if (this.EditorExtension.EditState == esriEditState.esriStateEditing)
            {
                this.EditorExtension.StartOperation();
                feature.Shape = newGeometry;
                feature.Store();
                this.EditorExtension.StopOperation("Extend Feature");
            }
            else
            {
                throw new Exception("Unable to update geometry.  You must be editing in order to update geometry.");
            }

        }

        private void ReleaseCOMObject(object o)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(o);
            }
            catch
            {
            }
        }
    }
}
