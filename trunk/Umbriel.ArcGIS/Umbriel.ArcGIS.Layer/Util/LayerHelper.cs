// <copyright file="LayerHelper.cs" company="Umbriel Project">
// Copyright (c) 2008-2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com,cumminsjp@gmail.com</email>
// <date>2008-06-06</date>
// <summary>Layer Utility Class</summary>

namespace Umbriel.ArcGIS.Layer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.Catalog;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;

    /// <summary>
    /// Various static ILayer methods for ArcObjects
    /// </summary>
    public class LayerHelper
    {
        #region Public Enum
        /// <summary>
        /// Enum of Layer Type - use in place of ESRI UID
        /// </summary>
        public enum LayerType
        {
            /// <summary>
            /// ESRI All Layers
            /// </summary>
            All = 0,

            /// <summary>
            /// ESRI IDataLayer
            /// </summary>
            IDataLayer = 1,

            /// <summary>
            /// ESRI IFeatureLayer
            /// </summary>
            IFeatureLayer = 2,

            /// <summary>
            /// ESRI IGeoFeatureLayer
            /// </summary>
            IGeoFeatureLayer = 3,

            /// <summary>
            /// ESRI IGraphicsLayer
            /// </summary>
            IGraphicsLayer = 4,

            /// <summary>
            /// ESRI IFDOGraphicsLayer
            /// </summary>
            IFDOGraphicsLayer = 5,

            /// <summary>
            /// ESRI ICoverageAnnotationLayer
            /// </summary>
            ICoverageAnnotationLayer = 6,

            /// <summary>
            /// ESRI IGroupLayer
            /// </summary>
            IGroupLayer = 7
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// method that executes a spatial search on a feature layer and returns the features 
        /// in an ArrayList
        /// </summary>
        /// <param name="filter">ISpatialFilter to search the layer with</param>
        /// <param name="layer">IFeatureLayer to search on</param>
        /// <returns>ArrayList of all features</returns>
        public static ArrayList SelectFeatures(ISpatialFilter filter, IFeatureLayer layer)
        {
            ArrayList featureList = new ArrayList();

            try
            {
                IFeatureCursor cursor = layer.Search(filter, false);
                IFeature feature = null;

                while ((feature = cursor.NextFeature()) != null)
                {
                    featureList.Add(feature);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Source + ",  " + ex.Message);
            }

            return featureList;
        }

        /// <summary>
        /// Forces a layer and all parents/grandparents/etc to be turned on.
        /// </summary>
        /// <param name="map">IMap in which to search</param>
        /// <param name="lyr">ILayer to force on by turning each anscestor layer</param>
        public static void ForceLayerOn(IMap map, ILayer lyr)
        {
            ILayer parentLayer = null;

            if (!lyr.Visible)
            {
                lyr.Visible = true;
            }

            // get the parent layer:
            parentLayer = (ILayer)GetParentLayer(map, lyr);

            if (parentLayer != null)
            {
                // turn it on:
                if (!parentLayer.Visible)
                {
                    parentLayer.Visible = true;
                }

                // Force the parent layer to be turned on:
                ForceLayerOn(map, parentLayer);
            }

            return;
        }

        /// <summary>
        /// Boolean that evaluates if a layer is actually visible on the map
        /// by evaluating the layer scale dependencies of both the lyr and all 
        /// lyr ILayer ancestors
        /// </summary>
        /// <param name="map">IMap in which to search</param>
        /// <param name="lyr">the ILayer to test visibility on.</param>
        /// <returns>boolean true if visible</returns>
        public static bool IsLayerVisible(IMap map, ILayer lyr)
        {
            ILayer parentLayer = null;
            bool isVisible = false;

            try
            {
                isVisible = lyr.Visible && ScaleVisible(map, lyr);

                if (isVisible == true)
                {
                    parentLayer = GetParentLayer(map, lyr);
                    if (parentLayer != null)
                    {
                        isVisible = IsLayerVisible(map, parentLayer);
                    }
                }

                return isVisible;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ArcZona.LayerUtility.IsLayerVisible Exception: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Returns an ArrayList of Selectable Layers from an IMap reference
        /// --This method is deprecated.
        /// </summary>
        /// <param name="map">IMap in which to search</param>
        /// <returns>array list of all selectable layers </returns>
        public static ArrayList GetSelectableLayers(IMap map)
        {
            return GetSelectableLayers(map, LayerType.All);
        }

        /// <summary>
        /// Returns selectable layers from an IMap reference of a specific layer type
        /// --This method is deprecated.
        /// </summary>
        /// <param name="map">IMap in which to search</param>
        /// <param name="layerType">type of layer to return in the array list</param>
        /// <returns>array list of all selectable layers of layerType</returns>
        public static ArrayList GetSelectableLayers(IMap map, LayerType layerType)
        {
            ArrayList selectableLayers = new ArrayList();

            if (layerType == LayerType.IFeatureLayer || layerType == LayerType.IGeoFeatureLayer)
            {
                UID uidLayer = GetLayerUID(layerType);

                IEnumLayer enumLayer;
                if (uidLayer == null)
                {
                    enumLayer = map.get_Layers(null, true);
                }
                else
                {
                    enumLayer = map.get_Layers(uidLayer, true);
                }

                ILayer layer = null;

                while ((layer = enumLayer.Next()) != null)
                {
                    IFeatureLayer featLayer = (IFeatureLayer)layer;

                    if (featLayer.Selectable)
                    {
                        selectableLayers.Add(featLayer);
                    }
                }
            }

            return selectableLayers;
        }

        /// <summary>
        /// Method that searches feature layers by the featureclass name and does not use an exact match
        /// of the featureclass name
        /// </summary>
        /// <param name="map">IMap in which to search</param>
        /// <param name="featureclassName">name of the featureclass</param>
        /// <returns>List of IFeatureLayer that matches the featureclassname (not the layer name)</returns>
        public static List<IFeatureLayer> FindFeatureLayers(IMap map, string featureclassName)
        {
            return FindFeatureLayers(map, featureclassName, false);
        }

        /// <summary>
        /// Method that searches feature layers by the featureclass name:
        /// </summary>
        /// <param name="map">IMap in which to search</param>
        /// <param name="featureclassName">Search criteria for the featureclass name</param>
        /// <param name="exactMatch">Exact string/case match</param>
        /// <returns>List of IFeatureLayer that matches the featureclassname (not the layer name)</returns>
        public static List<IFeatureLayer> FindFeatureLayers(IMap map, string featureclassName, bool exactMatch)
        {
            try
            {
                List<IFeatureLayer> layerList = new List<IFeatureLayer>();

                IEnumLayer enumLayer = (IEnumLayer)map.get_Layers(null, true);

                ILayer layer = null;

                while ((layer = enumLayer.Next()) != null)
                {
                    Trace.WriteLine(layer.Name);

                    if (layer is IFeatureLayer || layer is IGeoFeatureLayer)
                    {
                        IFeatureLayer featLayer = (IFeatureLayer)layer;
                        IDataset dataset = (IDataset)featLayer;

                        if (exactMatch)
                        {
                            if (dataset.BrowseName.Equals(featureclassName))
                            {
                                layerList.Add(featLayer);
                            }
                        }
                        else
                        {
                            if (dataset.BrowseName.IndexOf(featureclassName, StringComparison.CurrentCultureIgnoreCase) >= 0)
                            {
                                layerList.Add(featLayer);
                            }
                        }
                    }
                }

                return layerList;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Finds the feature layers.
        /// </summary>
        /// <param name="map">IMap in which to search</param>
        /// <returns>List of IFeatureLayer  within an IMap</returns>
        public static List<IFeatureLayer> FindFeatureLayers(IMap map)
        {
            try
            {
                List<IFeatureLayer> layerList = new List<IFeatureLayer>();

                IEnumLayer enumLayer = (IEnumLayer)map.get_Layers(null, true);

                ILayer layer = null;

                while ((layer = enumLayer.Next()) != null)
                {
                    Trace.WriteLine(layer.Name);

                    if (layer is IFeatureLayer || layer is IGeoFeatureLayer)
                    {
                        IFeatureLayer featLayer = (IFeatureLayer)layer;
                        IDataset dataset = (IDataset)featLayer;

                        layerList.Add(featLayer);
                    }
                }

                return layerList;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Finds the feature layers.
        /// </summary>
        /// <param name="map">IMap in which to search</param>
        /// <param name="service">The service string to search for</param>
        /// <param name="exactMatch">if set to <c>true</c> [exact match].</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns>List of IFeatureLayer  within an IMap</returns>
        public static List<IFeatureLayer> FindFeatureLayers(IMap map, string service, bool exactMatch, StringComparison stringComparison)
        {
            try
            {
                List<IFeatureLayer> layerList = new List<IFeatureLayer>();
                
                //UID uid = new UIDClass();
                //uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";

                IEnumLayer layers = map.get_Layers(null, true);
                ILayer layer = null;

                string keyname = "INSTANCE";

                while ((layer = layers.Next()) != null)
                {
                    if (layer is IFeatureLayer)
                    {
                        IFeatureLayer featureLayer = (IFeatureLayer)layer;
                        //IGeoFeatureLayer geoFeatureLayer = (IGeoFeatureLayer)layer;
                        IDataLayer dataLayer = (IDataLayer)layer;
                        IDatasetName datasetName = (IDatasetName)dataLayer.DataSourceName;

                        IWorkspaceName workspaceName = (IWorkspaceName)datasetName.WorkspaceName;
                        
                        //IFeatureClass featureClass = geoFeatureLayer.FeatureClass;
                        //IDataset dataset = (IDataset)featureClass;

                        //IWorkspace workspace = dataset.Workspace;

                        IPropertySet propertySet = workspaceName.ConnectionProperties;

                        Umbriel.ArcGIS.Layer.LayerHelper.PrintPropertySet(propertySet);

                        Dictionary<string, string> properties = Layer.LayerHelper.BuildDictionary(propertySet);

                        if (properties.ContainsKey(keyname))
                        {
                            if (exactMatch)
                            {
                                if (properties[keyname].Equals(service, stringComparison))
                                {
                                    layerList.Add(featureLayer);
                                }
                            }
                            else
                            {
                                if (properties[keyname].IndexOf(service, 1, stringComparison) > -1)
                                {
                                    layerList.Add(featureLayer);
                                }
                            } 
                        }                       
                    }
                }

                return layerList;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// searches an IMap for layers matching a specific layer name
        /// </summary>
        /// <param name="map">IMap in which to search</param>
        /// <param name="layerName">name of the layer</param>
        /// <returns>List of ILayer that matches the layerName criteria</returns>
        public static List<ILayer> GetLayerList(IMap map)
        {
            try
            {
                List<ILayer> layerList = new List<ILayer>();

                IEnumLayer enumLayer = (IEnumLayer)map.get_Layers(null, true);

                ILayer layer = null;

                while ((layer = enumLayer.Next()) != null)
                {
                    //IFeatureLayer featLayer = (IFeatureLayer)layer;
                    //IDataset dataset = (IDataset)featLayer;
                    layerList.Add(layer);
                }

                return layerList;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// searches an IMap for layers matching a specific layer name
        /// </summary>
        /// <param name="map">IMap in which to search</param>
        /// <param name="layerName">name of the layer</param>
        /// <returns>List of ILayer that matches the layerName criteria</returns>
        public static List<ILayer> FindLayers(IMap map, string layerName)
        {
            return FindLayers(map, layerName, true);
        }


        /// <summary>
        /// searches an IMap for layers matching a specific layer name
        /// exact match for case-sensitive
        /// </summary>
        /// <param name="map">IMap in which to search</param>
        /// <param name="layerName">name of the layer</param>
        /// <param name="exactMatch">whether the search should be for an exact match or not</param>
        /// <returns>List of ILayer that matches the layerName criteria</returns>
        public static List<ILayer> FindLayers(IMap map, string layerName, bool exactMatch)
        {
            List<ILayer> layerList = new List<ILayer>();  // create the new list

            IEnumLayer enumLayer = (IEnumLayer)map.get_Layers(null, true); // enumerate all layers
            ILayer layer = null;

            // iterate through the layer enumeration
            while ((layer = enumLayer.Next()) != null)
            {
                bool k = false;

                // Trace gets it's own try-catch
                try
                {
                    Trace.WriteLine(layer.Name);
                    k = true;
                }
                catch
                {
                }

                // try-catch the comparison
                try
                {
                    if (exactMatch && k)
                    {
                        if (layer.Name.Equals(layerName))
                        {
                            layerList.Add(layer);
                        }
                    }
                    else
                    {
                        if (layer.Name.IndexOf(layerName, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            layerList.Add(layer);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ArcZona.LayerUtility.FindLayers Exception: " + ex.Message);
                }
            }

            return layerList;
        }

        /// <summary>
        /// Prints the property set.
        /// </summary>
        /// <param name="ps">The IpropertySet</param>
        public static void PrintPropertySet(IPropertySet propertySet)
        {

            object propsetNames = new object[propertySet.Count - 1];
            object propsetValues = new object[propertySet.Count - 1];

            propertySet.GetAllProperties(out propsetNames, out propsetValues);

            object[] propsetNameArray = (object[])propsetNames;
            object[] propsetValueArray = (object[])propsetValues;

            for (int i = 0; i < propertySet.Count; i++)
            {
                Console.WriteLine(string.Format("{0}={1}", propsetNameArray[i].ToString(),propsetValueArray[i].ToString()));
            }

        }


        public static ILayer CreateLayer(string pathLayerFile)
        {
            try
            {
                if (System.IO.File.Exists(pathLayerFile) == true)
                {
                    ILayer layer;
                    ILayerFile layerFile = new LayerFileClass();
                    layerFile.Open(pathLayerFile);
                    layer = layerFile.Layer;

                    // System.Runtime.InteropServices.Marshal.ReleaseComObject(layerFile);

                    return layer;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Umbriel.ArcGIS.Layer.Util.LayerHelper.CreateLayer  Exception: " + ex.Message + "\n\nStackTrace: " + ex.StackTrace);
                throw;
            }
        }



        /// <summary>
        /// Builds a dictionary from the property set
        /// </summary>
        /// <param name="propertySet">The property set.</param>
        /// <returns></returns>
        public static Dictionary<string,string> BuildDictionary(IPropertySet propertySet)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();

            object propsetNames = new object[propertySet.Count - 1];
            object propsetValues = new object[propertySet.Count - 1];

            propertySet.GetAllProperties(out propsetNames, out propsetValues);

            object[] propsetNameArray = (object[])propsetNames;
            object[] propsetValueArray = (object[])propsetValues;

            for (int i = 0; i < propertySet.Count; i++)
            {
                properties.Add(propsetNameArray[i].ToString(), propsetValueArray[i].ToString());
            }

            return properties;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// evaluates a layer's scale visibility
        /// </summary>
        /// <param name="map">IMap containing lyr</param>
        /// <param name="lyr">The ILayer for which visibility will be evaluated</param>
        /// <returns>boolean indicating whether the layer if visible.</returns>
        private static bool ScaleVisible(IMap map, ILayer lyr)
        {
            if (lyr.MaximumScale == 0 & lyr.MinimumScale == 0)
            {
                return true;
            }
            else if (lyr.MaximumScale != 0 & lyr.MinimumScale == 0)
            {
                if (map.MapScale > lyr.MaximumScale)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (lyr.MaximumScale == 0 & lyr.MinimumScale != 0)
            {
                if (map.MapScale < lyr.MinimumScale)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (lyr.MaximumScale != 0 & lyr.MinimumScale != 0)
            {
                if (map.MapScale < lyr.MinimumScale & map.MapScale > lyr.MaximumScale)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new Exception("No ScaleVisible Case.");
            }
        }

        /// <summary>
        /// Gets the parent layer.
        /// </summary>
        /// <param name="map">The map containing the lyr</param>
        /// <param name="lyr">The ILayer that is contained in the map. </param>
        /// <returns>ILayer that is the parent of the lyr ILayer ref</returns>
        private static ILayer GetParentLayer(IMap map, ILayer lyr)
        {
            if (map.LayerCount == 0)
            {
                return null;
            }

            UID uid = new UID();
            uid.Value = "{EDAD6644-1810-11D1-86AE-0000F8751720}";

            IEnumLayer enumLayer = map.get_Layers(uid, true);

            ICompositeLayer compositeLayer = null;

            while ((compositeLayer = (ICompositeLayer)enumLayer.Next()) != null)
            {
                for (int i = 0; i < compositeLayer.Count; i++)
                {
                    Trace.WriteLine("Composite Layer Name: " + compositeLayer.get_Layer(i).Name);
                    Trace.WriteLine("Layer Name: " + lyr.Name);

                    if (compositeLayer.get_Layer(i) == lyr)
                    {
                        Trace.WriteLine("Equal");
                        return (ILayer)compositeLayer;
                    }
                    else
                    {
                        Trace.WriteLine("NotEqual");
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the layer UID.
        /// </summary>
        /// <param name="layerType">Type of the layer.</param>
        /// <returns>UID of the Layer type</returns>
        private static UID GetLayerUID(LayerType layerType)
        {
            if (layerType == LayerType.All)
            {
                return null;
            }

            UID uid = new UIDClass();

            switch (layerType)
            {
                case LayerType.IDataLayer:
                    uid.Value = "{6CA416B1-E160-11D2-9F4E-00C04F6BC78E}";
                    break;
                case LayerType.IFeatureLayer:
                    uid.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
                    break;
                case LayerType.IGeoFeatureLayer:
                    uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
                    break;
                case LayerType.IGraphicsLayer:
                    uid.Value = "{34B2EF81-F4AC-11D1-A245-080009B6F22B}";
                    break;
                case LayerType.IFDOGraphicsLayer:
                    uid.Value = "{5CEAE408-4C0A-437F-9DB3-054D83919850}";
                    break;
                case LayerType.ICoverageAnnotationLayer:
                    uid.Value = "{0C22A4C7-DAFD-11D2-9F46-00C04F6BC78E}";
                    break;
                case LayerType.IGroupLayer:
                    uid.Value = "{EDAD6644-1810-11D1-86AE-0000F8751720}";
                    break;
                default:
                    break;
            }

            return uid;
        }
        #endregion
    }
}
