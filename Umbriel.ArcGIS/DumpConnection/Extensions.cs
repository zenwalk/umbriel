// <copyright file="Extensions.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-12-20</date>
// <summary>Extensions class file</summary>

namespace DumpConnection
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.esriSystem;
    
    

    /// <summary>
    /// static class of extension methods to use with ESRI ArcObjects
    /// </summary>
    public static class Extensions
    {
        [DllImport("ole32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        static extern string ProgIDFromCLSID([In()]ref Guid clsid);

        /// <summary>
        /// Extension method for string.Format
        /// </summary>
        /// <param name="format">The string to be formatted.</param>
        /// <param name="args">object parameters.</param>
        /// <returns>formatted string</returns>
        public static string FormatString(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static string WhichCoClassAmI(this ILayer layer)
        {
            IPersist p = layer as IPersist;

            if (p != null)
            {
                Guid g;

                p.GetClassID(out g);

                return GetProgID(g);
            }
            else
            {
                return "Unknown (layer does not implement IPersist)";
            }

            //if (layer is BasemapLayerClass)
            //{
            //    return "BasemapLayerClass";
            //}
            //else if (layer is CadAnnotationLayerClass)
            //{
            //    return "CadAnnotationLayerClass";
            //}
            //else if (layer is CadastralFabricLayerClass)
            //{
            //    return "CadastralFabricLayerClass";
            //}
            //else if (layer is CadastralFabricSubLayerClass)
            //{
            //    return "CadastralFabricSubLayerClass";
            //}
            //else if (layer is CadFeatureLayerClass)
            //{
            //    return "CadFeatureLayerClass";
            //}
            //else if (layer is CadLayerClass)
            //{
            //    return "CadLayerClass";
            //}
            //else if (layer is CompositeGraphicsLayerClass)
            //{
            //    return "CompositeGraphicsLayerClass";
            //}
            //else if (layer is CoverageAnnotationLayerClass)
            //{
            //    return "CoverageAnnotationLayerClass";
            //}
            //else if (layer is DimensionLayerClass)
            //{
            //    return "DimensionLayerClass";
            //}
            //else if (layer is DummyGraduatedMarkerLayerClass)
            //{
            //    return "DummyGraduatedMarkerLayerClass";
            //}
            //else if (layer is DummyLayerClass)
            //{
            //    return "DummyLayerClass";
            //}
            //else if (layer is FDOGraphicsLayerClass)
            //{
            //    return "FDOGraphicsLayerClass";
            //}
            //else if (layer is FDOGraphicsSublayerClass)
            //{
            //    return "FDOGraphicsSublayerClass";
            //}
            //else if (layer is FeatureLayerClass)
            //{
            //    return "FeatureLayerClass";
            //}
            //else if (layer is GdbRasterCatalogLayerClass)
            //{
            //    return "GdbRasterCatalogLayerClass";
            //}
            //else if (layer is GraphicsSubLayerClass)
            //{
            //    return "GraphicsSubLayerClass";
            //}
            //else if (layer is GroupLayerClass)
            //{
            //    return "GroupLayerClass";
            //}
            //else if (layer is ImageServerLayerClass)
            //{
            //    return "ImageServerLayerClass";
            //}
            //else if (layer is IMSMapLayerClass)
            //{
            //    return "IMSMapLayerClass";
            //}
            //else if (layer is IMSSubFeatureLayerClass)
            //{
            //    return "IMSSubFeatureLayerClass";
            //}
            //else if (layer is IMSSubLayerClass)
            //{
            //    return "IMSSubLayerClass";
            //}
            //else if (layer is MapServerBasicSublayerClass)
            //{
            //    return "MapServerBasicSublayerClass";
            //}
            //else if (layer is MapServerFindSublayerClass)
            //{
            //    return "MapServerFindSublayerClass";
            //}
            //else if (layer is MapServerIdentifySublayerClass)
            //{
            //    return "MapServerIdentifySublayerClass";
            //}
            //else if (layer is MapServerLayerClass)
            //{
            //    return "MapServerLayerClass";
            //}
            //else if (layer is MapServerQuerySublayerClass)
            //{
            //    return "MapServerQuerySublayerClass";
            //}
            //else if (layer is MosaicLayerClass)
            //{
            //    return "MosaicLayerClass";
            //}
            //else if (layer is NetworkLayerClass)
            //{
            //    return "NetworkLayerClass";
            //}
            //else if (layer is NITFGraphicsLayerClass)
            //{
            //    return "NITFGraphicsLayerClass";
            //}
            //else if (layer is RasterBasemapLayerClass)
            //{
            //    return "RasterBasemapLayerClass";
            //}
            //else if (layer is RasterCatalogLayerClass)
            //{
            //    return "RasterCatalogLayerClass";
            //}
            //else if (layer is RasterLayerClass)
            //{
            //    return "RasterLayerClass";
            //}
            //else if (layer is TopologyLayerClass)
            //{
            //    return "TopologyLayerClass";
            //}
            //else if (layer is WCSLayerClass)
            //{
            //    return "WCSLayerClass";
            //}
            //else if (layer is WMSGroupLayerClass)
            //{
            //    return "WMSGroupLayerClass";
            //}
            //else if (layer is WMSLayerClass)
            //{
            //    return "WMSLayerClass";
            //}
            //else if (layer is WMSMapLayerClass)
            //{
            //    return "WMSMapLayerClass";
            //}
            //else
            //{
            //    return "Case Not Handled";
            //}

            /*
            else if (layer is TerrainLayerClass)
            {
                return "TerrainLayerClass";
            }
             else if (layer is TinLayerClass)
            {
                return "TinLayerClass";
            }
             *
             * 
             * 
             * 
             * 
             * /
            //else if (layer is ForceElementLayerClass)
            //{
            //    return "ForceElementLayer (esriDefenseSolutions)Class";
            //}
            //else if (layer is GeoVideoLayerClass)
            //{
            //    return "GeoVideoLayer (esriGlobeCore)Class";
            //}
            //else if (layer is GlobeGraphicsLayerClass)
            //{
            //    return "GlobeGraphicsLayer (esriGlobeCore)Class";
            //}
            //else if (layer is GlobeLayerClass)
            //{
            //    return "GlobeLayer (esriGlobeCore)Class";
            //}
            //else if (layer is GlobeServerLayerClass)
            //{
            //    return "GlobeServerLayer (esriGlobeCore)Class";
            //}
            //else if (layer is GraphicsLayer3DClass)
            //{
            //    return "GraphicsLayer3D (esri3DAnalyst)Class";
            //}
            //else if (layer is JoinedControlPointLayerClass)
            //{
            //    return "JoinedControlPointLayer (esriCadastralUI)Class";
            //}
            //else if (layer is JoinedLinePointLayerClass)
            //{
            //    return "JoinedLinePointLayer (esriCadastralUI)Class";
            //}
            //else if (layer is JoinedParcelLayerClass)
            //{
            //    return "JoinedParcelLayer (esriCadastralUI)Class";
            //}
            //else if (layer is JoinedParcelLineLayerClass)
            //{
            //    return "JoinedParcelLineLayer (esriCadastralUI)Class";
            //}
            //else if (layer is JoinedPointLayerClass)
            //{
            //    return "JoinedPointLayer (esriCadastralUI)Class";
            //}
            //else if (layer is KmlLayerClass)
            //{
            //    return "KmlLayer (esriGlobeCore)Class";
            //}
            //else if (layer is MADtedLayerClass)
            //{
            //    return "MADtedLayer (esriDefenseSolutions)Class";
            //}
            //else if (layer is MARasterLayerClass)
            //{
            //    return "MARasterLayer (esriDefenseSolutions)Class";
            //}
            //else if (layer is NALayerClass)
            //{
            //    return "NALayer (esriNetworkAnalyst)Class";
            //}
            //else if (layer is PacketJoinedLayerClass)
            //{
            //    return "PacketJoinedLayer (esriCadastralUI)Class";
            //}
            //else if (layer is ProcessLayerClass)
            //{
            //    return "ProcessLayer (esriGeoprocessing)Class";
            //}
            //else if (layer is SchematicLayerClass)
            //{
            //    return "SchematicLayer (esriSchematic)Class";
            //}
            //else if (layer is SearchResultsLayerClass)
            //{
            //    return "SearchResultsLayerClass";
            //}
            //else if (layer is TacticalGraphicLayerClass)
            //{
            //    return "TacticalGraphicLayerClass";
            //}
            //else if (layer is TemporalFeatureLayerClass)
            //{
            //    return "TemporalFeatureLayer (esriTrackingAnalyst)Class";
            //}

            */

        }

        public static string GetProgID(Guid guid)
        {
            string progId = ProgIDFromCLSID(ref guid);

            return progId;
        }
    }
}
