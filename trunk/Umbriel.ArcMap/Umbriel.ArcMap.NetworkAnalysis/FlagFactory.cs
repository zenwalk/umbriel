// <copyright file="FlagFactory.cs" company="Umbriel Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-08-05</date>
// <summary></summary>

namespace Umbriel.ArcMap.NetworkAnalysis
{
    using System;
    using System.Collections.Generic;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.Display;
    using ESRI.ArcGIS.EditorExt;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Framework;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;
    using ESRI.ArcGIS.NetworkAnalysis;

    /// <summary>
    /// AddFlag Event Handler
    /// </summary>
    /// /// <param name="pt">IPoint of the flag that was added</param>
    public delegate void AddFlagEventHandler(IPoint pt);
    
    /// <summary>
    /// AddFlagErrorEvent Handler
    /// </summary>
    /// <param name="FeatureLayerName">Name of the feature layer containing the feature for which a flag could not be added</param>
    /// <param name="ErrorMessage">The error message</param>
    /// <param name="OID">The OID of the feature for which a flag could not be added</param>
    public delegate void AddFlagErrorEventHandler(string FeatureLayerName, string ErrorMessage, int OID);

    /// <summary>
    /// FlagFactory Class for generating flags for the Utility Analyst Extension
    /// </summary>
    internal class FlagFactory
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FlagFactory"/> class.
        /// </summary>
        /// <param name="application">The IApplication containing the utility analysis ext.</param>
        public FlagFactory(IApplication application)
        {
            this.Application = application;

            this.SnapTolerance = 1;

            this.EdgeFlagSymbol = this.GetDefaultFlagSymbol();

            // setup the reference to the utility network analysis extension
            UID uidUtilAnalystExt = new UIDClass();
            uidUtilAnalystExt.Value = "esriEditorExt.UtilityNetworkAnalysisExt";
            this.UtilityNetAnalysisExt = (IUtilityNetworkAnalysisExt)this.Application.FindExtensionByCLSID(uidUtilAnalystExt);
            this.NetworkAnalysisExt = (INetworkAnalysisExt)this.UtilityNetAnalysisExt;
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when [add flag event].
        /// </summary>
        public event AddFlagEventHandler AddFlagEvent;

        /// <summary>
        /// Occurs when [add flag error event].
        /// </summary>
        public event AddFlagErrorEventHandler AddFlagErrorEvent;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the snap tolerance.
        /// </summary>
        /// <value>The snap tolerance.</value>
        public double SnapTolerance { get; set; }

        /// <summary>
        /// Gets the add flag count.
        /// </summary>
        /// <value>The number of  flags that were added by the Add Flags methods</value>
        public int AddFlagCount { get; private set; }

        /// <summary>
        /// Gets or sets the edge flag symbol
        /// </summary>
        /// <value>The edge flag symbol.</value>
        public ISymbol EdgeFlagSymbol { get; set; }

        /// <summary>
        /// Gets or sets the IApplication reference
        /// </summary>
        /// <value>The IApplication reference</value>
        private IApplication Application { get; set; }

        /// <summary>
        /// Gets or sets the utility net analysis extension reference
        /// </summary>
        /// <value>The utility net analysis extension</value>
        private IUtilityNetworkAnalysisExt UtilityNetAnalysisExt { get; set; }

        /// <summary>
        /// Gets or sets the network analysis extension reference.
        /// </summary>
        /// <value>network analysis extension</value>
        private INetworkAnalysisExt NetworkAnalysisExt { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Adds the edge flags for the list if IFeatureLayers
        /// </summary>
        /// <param name="layers">List of IFeatureLayer </param>
        internal void AddEdgeFlags(List<IFeatureLayer> layers)
        {
            this.AddFlagCount = 0;

            foreach (IFeatureLayer layer in layers)
            {
                this.AddEdgeFlags(layer);
            }
        }

        /// <summary>
        /// Adds  edge flags for a feature layer
        /// </summary>
        /// <param name="layer">The feature layer</param>
        private void AddEdgeFlags(IFeatureLayer layer)
        {
            try
            {
                // QI all the network analysis interfaces
                INetworkAnalysisExt netAnalyst = (INetworkAnalysisExt)this.UtilityNetAnalysisExt;
                INetworkAnalysisExtFlags flags = (INetworkAnalysisExtFlags)this.UtilityNetAnalysisExt;
                INetworkAnalysisExtBarriers barriers = (INetworkAnalysisExtBarriers)this.UtilityNetAnalysisExt;

                IPointToEID point2EID = new PointToEIDClass();
                point2EID.GeometricNetwork = this.NetworkAnalysisExt.CurrentNetwork;
                point2EID.SourceMap = ((IMxDocument)this.Application.Document).FocusMap;
                point2EID.SnapTolerance = this.SnapTolerance;

                INetElements networkElements = (INetElements)this.NetworkAnalysisExt.CurrentNetwork.Network;

                IFeatureSelection featureSelection = (IFeatureSelection)layer;
                ISelectionSet selectionSet = (ISelectionSet)featureSelection.SelectionSet;

                IEnumIDs ids = selectionSet.IDs;

                int i;

                while ((i = ids.Next()) > 0)
                {
                    IFeature feature = layer.FeatureClass.GetFeature(i);

                    IPoint locationPoint;
                    int nearestEdgeID;
                    double perCent;

                    point2EID.GetNearestEdge((IPoint)feature.ShapeCopy, out nearestEdgeID, out locationPoint, out perCent);

                    if (networkElements.IsValidElement(nearestEdgeID, esriElementType.esriETEdge))
                    {
                        int userclassID;
                        int userID;
                        int usersubID;

                        networkElements.QueryIDs(nearestEdgeID, esriElementType.esriETEdge, out userclassID, out userID, out usersubID);

                        IFlagDisplay flagDisplay = new EdgeFlagDisplayClass();
                        flagDisplay.FeatureClassID = userclassID;
                        flagDisplay.FID = userID;
                        flagDisplay.SubID = usersubID;

                        flagDisplay.Geometry = locationPoint;

                        flagDisplay.Symbol = this.EdgeFlagSymbol;

                        flags.AddEdgeFlag((IEdgeFlagDisplay)flagDisplay);
                        this.AddFlagCount++;
                        this.OnAddFlag(locationPoint);
                    }
                    else
                    {
                        this.OnAddFlagError(layer.Name, "No nearby edge feature.", feature.OID);
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
        /// Adds  junction flags to all of the selected point features
        /// </summary>
        /// <param name="layers">List of feature layers that contain selected points</param>
        internal void AddJunctionFlags(List<IFeatureLayer> layers)
        {
            this.AddFlagCount = 0;

            foreach (IFeatureLayer layer in layers)
            {
                this.AddEdgeFlags(layer);
            }
        }

        /// <summary>
        /// Adds the junction flags at the locations of selected features
        /// </summary>
        /// <param name="layer">IFeatureLayer containing the feature selection</param>
        internal void AddJunctionFlags(IFeatureLayer layer)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the default flag symbol
        /// </summary>
        /// <returns>ISymbol for the green square symbol</returns>
        private ISymbol GetDefaultFlagSymbol()
        {
            RgbColor rgb = new RgbColorClass();
            rgb.Red = 0;
            rgb.Green = 255;
            rgb.Blue = 0;

            ISimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbolClass();

            simpleMarkerSymbol.Color = rgb;
            simpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSSquare;
            simpleMarkerSymbol.Outline = false;
            simpleMarkerSymbol.Size = 10;

            return (ISymbol)simpleMarkerSymbol;
        }

        /// <summary>
        /// Called when [add flag].
        /// </summary>
        /// <param name="pt">The IPoint location of the flag that was added</param>
        private void OnAddFlag(IPoint pt)
        {
            if (this.AddFlagEvent != null)
            {
                this.AddFlagEvent(pt);
            }
        }

        /// <summary>
        /// Called when [add flag error].
        /// </summary>
        /// <param name="featureLayerName">Name of the feature layer containing the feature for which a flag could not be added</param>
        /// <param name="errorMessage">The error message</param>
        /// <param name="oid">The OID of the feature for which a flag could not be added</param>
        private void OnAddFlagError(string featureLayerName, string errorMessage, int oid)
        {
            if (this.AddFlagErrorEvent != null)
            {
                this.AddFlagErrorEvent(featureLayerName, errorMessage, oid);
            }
        }

        #endregion
    }
}
