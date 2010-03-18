﻿// <copyright file="EditorHelper.cs" company="Earth">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-08-06</date>
// <summary>Helper class file</summary>

namespace Umbriel.ArcMap.Editor.Util
{
    using System;
    using System.Collections.Generic;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;

    /// <summary>
    /// Helper class for Umbriel Editor Tools
    /// </summary>
    internal static class EditorHelper
    {
        /// <summary>
        /// Removes the layers with no feature selections.
        /// </summary>
        /// <param name="layers">List of IFeatureLayer</param>
        internal static void RemoveLayersWithNoSelections(ref List<IFeatureLayer> layers)
        {
            try
            {
                for (int i = layers.Count - 1; i >= 0; i--)
                {
                    IFeatureLayer layer = layers[i];

                    IFeatureSelection featureSelection = (IFeatureSelection)layer;

                    if (featureSelection.SelectionSet == null || featureSelection.SelectionSet.Count.Equals(0))
                    {
                        // remove the layer from the list:
                        layers.Remove(layer);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Removes any layers from the list that are not point layers
        /// </summary>
        /// <param name="layers">List of IFeatureLayer</param>
        internal static void RemoveLayersNonPointLayers(ref List<IFeatureLayer> layers)
        {
            try
            {
                for (int i = layers.Count - 1; i >= 0; i--)
                {
                    IFeatureLayer layer = layers[i];
                    IFeatureClass featureClass = (IFeatureClass)layer.FeatureClass;

                    if (!featureClass.ShapeType.Equals(esriGeometryType.esriGeometryPoint))
                    {
                        // remove the layer from the list:
                        layers.Remove(layer);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.StackTrace);
                throw;
            }
        }
    }
}

