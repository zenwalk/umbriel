// <copyright file="PathAnalyzer.cs" company="Umbriel Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-09-17</date>
// <summary>PathAnalyzer class file</summary>

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
    /// Path Analyzer Class
    /// </summary>
    internal class PathAnalyzer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathAnalyzer"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        internal PathAnalyzer(FlagFactory factory)
        {
            this.AnalzerFlagFactory = factory;
        }

        /// <summary>
        /// Gets the analzer flag factory.
        /// </summary>
        /// <value>The analzer flag factory.</value>
        public FlagFactory AnalzerFlagFactory { get; private set; }

        /// <summary>
        /// Gets or sets the results table
        /// </summary>
        /// <value>The results.</value>
        private Util.CountPathsResultTable Results { get; set; }

        /// <summary>
        /// Gets or sets the super flag.
        /// </summary>
        /// <value>The super flag.</value>
        private IJunctionFlagDisplay SuperFlag { get; set; }

        /// <summary>
        /// Gets or sets the utility net analysis extension reference
        /// </summary>
        /// <value>The utility net analysis extension</value>
        private IUtilityNetworkAnalysisExt UtilityNetAnalysisExt { get; set; }
        
        /// <summary>
        /// Counts the number of possible paths from selected point features
        /// to the initial, single junction flag (super flag).
        /// </summary>
        /// <param name="layerList">The layer list.</param>
        /// <returns>A CountPathsResultTable containing the results of the analysis</returns>
        internal Util.CountPathsResultTable CountPaths(List<IFeatureLayer> layerList)
        {
            this.Results = new Umbriel.ArcMap.NetworkAnalysis.Util.CountPathsResultTable();

            this.SetSuperFlagLocation();

            // add the junction flags:
            this.AnalzerFlagFactory.AddJunctionFlags(layerList);
            
            return this.Results;
        }

        /// <summary>
        /// Converts the geometry to WKB.
        /// </summary>
        /// <param name="geometry">The geometry</param>
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
        /// Sets the super flag location.
        /// </summary>
        private void SetSuperFlagLocation()
        {
            try
            {
                // QI all the network analysis interfaces
                INetworkAnalysisExt netAnalyst = (INetworkAnalysisExt)this.UtilityNetAnalysisExt;
                INetworkAnalysisExtFlags flags = (INetworkAnalysisExtFlags)this.UtilityNetAnalysisExt;
                //// INetworkAnalysisExtBarriers barriers = (INetworkAnalysisExtBarriers)this.UtilityNetAnalysisExt;
                if (flags.EdgeFlagCount != 0 & flags.JunctionFlagCount != 1)
                {
                    throw new CustomExceptions.CountPathFlagException("Invalid initial flag configuration.", flags.JunctionFlagCount, flags.EdgeFlagCount);
                }

                IJunctionFlagDisplay junctionFlagDisplay = flags.get_JunctionFlag(0);
                this.SuperFlag = junctionFlagDisplay;

                IGeometry geometry = ((IFlagDisplay)junctionFlagDisplay).Geometry;

                byte[] wkb = ConvertGeometryToWKB(geometry);
                Properties.Settings.Default.CountPathSuperFlagWKB = wkb;
                Properties.Settings.Default.Save();

                return;               
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}
