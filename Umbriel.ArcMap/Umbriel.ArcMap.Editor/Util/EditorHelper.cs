// <copyright file="EditorHelper.cs" company="Umbriel Project">
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
    public  class EditorHelper
    {
        /// <summary>
        /// Removes the layers with no feature selections.
        /// </summary>
        /// <param name="layers">List of IFeatureLayer</param>
        public  static void RemoveLayersWithNoSelections(ref List<IFeatureLayer> layers)
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
        public static void RemoveLayersNonPointLayers(ref List<IFeatureLayer> layers)
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

        /// <summary>
        /// Unions the cursor geometries.
        /// </summary>
        /// <param name="cursor">The cursor.</param>
        /// <returns>a single geometry</returns>
        public static IGeometry UnionCursorGeometries(IFeatureCursor cursor)
        {
            IFeature feature = null;
            IGeometry unionGeometry = null;
            ITopologicalOperator topoOperator = null;

            while ((feature = cursor.NextFeature()) != null)
            {
                if (!feature.Shape.IsEmpty)
                {
                    if (unionGeometry != null)
                    {
                        topoOperator = (ITopologicalOperator)feature.Shape;
                        unionGeometry = topoOperator.Union(unionGeometry);
                    }
                    else
                    {
                        unionGeometry = feature.Shape;
                    }
                }
            }

            return unionGeometry;
        }


        /// <summary>
        /// Obtains the union of all feature geometires within a List of features
        /// </summary>
        /// <param name="featureList">list of features to union together</param>
        /// <returns>IGeometry of the unioned shapes contained in the parameter list</returns>
        public static IGeometry UnionGeometries(List<IFeature> featureList)
        {
                IGeometry unionGeometry = null;
                ITopologicalOperator topoOperator = null;

                foreach (IFeature feature in featureList)
                {
                    if (!feature.Shape.IsEmpty)
                    {
                        if (unionGeometry != null)
                        {
                            topoOperator = (ITopologicalOperator)feature.Shape;
                            unionGeometry = topoOperator.Union(unionGeometry);
                        }
                        else
                        {
                            unionGeometry = feature.Shape;
                        }
                    }
                }

                return unionGeometry;           
        }
    }
}


