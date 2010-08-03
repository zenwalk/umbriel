// <copyright file="ArcGISExtensions.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-06-25</date>
// <summary>ArcGISExtensions class file</summary>

namespace Umbriel.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;
    using FeatureLayerList = System.Collections.Generic.List<ESRI.ArcGIS.Carto.IFeatureLayer>;
    using FeatureList = System.Collections.Generic.List<ESRI.ArcGIS.Geodatabase.IFeature>;
    using LayerList = System.Collections.Generic.List<ESRI.ArcGIS.Carto.ILayer>;
    using OIDList = System.Collections.Generic.List<int>;
    using PropertyDictionary = System.Collections.Generic.Dictionary<string, string>;
    using Reflect = System.Reflection;

    /// <summary>
    /// static class of extension methods to use with ESRI ArcObjects
    /// </summary>
    public static class ArcGISExtensions
    {




        /// <summary>
        /// Converts IFeatureCursor to a List of Features
        /// </summary>
        /// <param name="cursor">The IFeatureCursor</param>
        /// <returns> System.Collections.Generic.List of ESRI.ArcGIS.Geodatabase.IFeature</returns>
        public static FeatureList ToFeatureList(this IFeatureCursor cursor)
        {
            FeatureList features = new List<IFeature>();
            IFeature feature = null;

            while ((feature = cursor.NextFeature()) != null)
            {
                features.Add(feature);
            }

            return features;
        }

        /// <summary>
        /// Gets the name of the featureclass. 
        /// </summary>
        /// <param name="layer">The IFeatureLayer</param>
        /// <returns>string name of the featureclass.</returns>
        public static string GetFeatureclassName(this IFeatureLayer layer)
        {
            return layer.GetFeatureclassName(true);
        }

        /// <summary>
        /// Gets the name of the featureclass. If classNameOnly is set to true, the
        /// class name is parsed from the fully qualified table name (GIS.DBO.PARCELS would return PARCELS 
        /// </summary>
        /// <param name="layer">The IFeatureLayer</param>
        /// <param name="classNameOnly">if set to <c>true</c> [class name only]. </param>
        /// <returns>string name of the featureclass.</returns>
        public static string GetFeatureclassName(this IFeatureLayer layer, bool classNameOnly)
        {
            IDataset dataset = layer.FeatureClass as IDataset;

            if (dataset != null)
            {
                if (classNameOnly)
                {
                    return dataset.Name.ParseObjectClassName();
                }
                else
                {
                    return dataset.Name;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// parses the object class table name from the fully qualified object class name:
        /// </summary>
        /// <param name="objectClassName">Name of the object class.</param>
        /// <returns>
        /// A string containing only the object class name.  So and objectClassName of 'GIS.DBO.PARCELS' would be returned as PARCELS
        /// </returns>
        public static string ParseObjectClassName(this string objectClassName)
        {
            try
            {
                if (objectClassName.LastIndexOf('.') > 0)
                {
                    return objectClassName.Substring(objectClassName.LastIndexOf('.') + 1).Trim();
                }
                else
                {
                    // if there's no period, then return the objectClassName as-is
                    return objectClassName;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ParseObjectClassName  Exception: " + ex.Message + "\n\nStackTrace: " + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Creates a FeatureLayer List of any feature layers that have a feature selection
        /// </summary>
        /// <param name="map">The map to search for selected features</param>
        /// <returns>List of IFeatureLayer</returns>
        public static FeatureLayerList LayersWithSelectedFeatures(this IMap map)
        {
            FeatureLayerList layers = new List<IFeatureLayer>();

            foreach (ILayer layer in map.GetLayerList())
            {
                if (layer is IFeatureLayer)
                {
                    IFeatureLayer featureLayer = (IFeatureLayer)layer;
                    IFeatureSelection featureSelection = (IFeatureSelection)featureLayer;

                    if (featureSelection.SelectionSet.Count > 0)
                    {
                        layers.Add(featureLayer);
                    }
                }
            }

            return layers;
        }

        /// <summary>
        /// Gets the layer list.
        /// </summary>
        /// <param name="map">The map to search for layers</param>
        /// <returns>LayerList of layers in the map</returns>
        public static LayerList GetLayerList(this IMap map)
        {
            try
            {
                LayerList layerList = new LayerList();

                IEnumLayer enumLayer = (IEnumLayer)map.get_Layers(null, true);

                ILayer layer = null;

                while ((layer = enumLayer.Next()) != null)
                {
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
        /// Gets the layer list of layers that match layer name parameter using StringComparison.CurrentCultureIgnoreCase
        /// </summary>
        /// <param name="map">The map to search for layers</param>
        /// <param name="layerName">The name of the layer to match</param>
        /// <returns>LayerList of layers in the map</returns>
        public static LayerList GetLayerList(this IMap map, string layerName)
        {
            StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase;

            return map.GetLayerList(layerName, stringComparison);
        }

        /// <summary>
        /// Gets the layer list.
        /// </summary>
        /// <param name="map">The map to search for layers</param>
        /// <param name="layerName">The name of the layer to match</param>
        /// <param name="stringComparison">the type of string comparison to use when matching the layer name</param>
        /// <returns>LayerList of layers in the map</returns>
        public static LayerList GetLayerList(this IMap map, string layerName, StringComparison stringComparison)
        {
            try
            {
                LayerList layerList = new LayerList();

                IEnumLayer enumLayer = (IEnumLayer)map.get_Layers(null, true);

                ILayer layer = null;

                while ((layer = enumLayer.Next()) != null)
                {
                    if (layer.Name.Equals(layerName, stringComparison))
                    {
                        layerList.Add(layer);
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
        /// Gets a list of layers from the selecteditems ISet reference
        /// </summary>
        /// <param name="o">The selected items (object)</param>
        /// <returns>LayerList of selected layers</returns>
        public static LayerList ToLayerList(this object o)
        {
            List<ILayer> layers = new List<ILayer>();

            if (o != null && o is ISet)
            {
                ISet selectedSet = (ISet)o;

                object setItem = null;

                while ((setItem = selectedSet.Next()) != null)
                {
                    if (setItem is ILayer)
                    {
                        layers.Add((ILayer)setItem);
                    }
                }
            }

            return layers;
        }

        /// <summary>
        /// Gets the spatial reference for ILayer
        /// </summary>
        /// <param name="value">The ILayer </param>
        /// <returns>ISpatialReference of a layer</returns>
        public static ISpatialReference GetSpatialReference(this ILayer value)
        {
            IGeoDataset g = (IGeoDataset)value;
            return g.SpatialReference;
        }

        /// <summary>
        /// Converts the ISpatialReference to Proj4 //TODO: write ToProj4 method
        /// NOT IMPLEMENTED YET
        /// </summary>
        /// <param name="sr">The ISpatialReference</param>
        /// <returns>proj4 string</returns>
        public static string ToProj4(this ISpatialReference sr)
        {
            string proj4text = string.Empty;

            //// TODO: write ToProj4 method

            return proj4text;
        }

        /// <summary>
        /// Determines if the ILayer has Z in geometry definition
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <returns>true if there is a z value in the geometry definition</returns>
        public static bool Is3D(this ILayer layer)
        {
            IFeatureLayer featureLayer = (IFeatureLayer)layer;

            IField field = featureLayer.FeatureClass.GetField(featureLayer.FeatureClass.ShapeFieldName);

            IGeometryDef geometryDefinition = field.GeometryDef;

            return geometryDefinition.HasZ;
        }

        /// <summary>
        /// Gets the IField of the field name
        /// </summary>
        /// <param name="featureclass">The IFeatureClass</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>returns null of no field is found</returns>
        public static IField GetField(this IFeatureClass featureclass, string fieldName)
        {
            return ((ITable)featureclass).GetField(fieldName);
        }

        /// <summary>
        /// Gets the IField of the field name
        /// </summary>
        /// <param name="table">The ITable.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>returns null of no field is found</returns>
        public static IField GetField(this ITable table, string fieldName)
        {
            IField field = null;

            int i = table.Fields.FindField(fieldName);

            if (!i.Equals(-1))
            {
                field = table.Fields.get_Field(i);
            }

            return field;
        }

        /// <summary>
        /// Sets the value of a field
        /// </summary>
        /// <param name="feature">The GIS feature</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The new value for the field</param>
        public static void SetValue(this IFeature feature, string fieldName, object value)
        {
            int fieldIndex = feature.Fields.FindField(fieldName);

            if (fieldIndex.Equals(-1))
                throw new ArgumentException("Feature has no field named: " + fieldName);

            feature.set_Value(fieldIndex, value);
        }

        /// <summary>
        /// Converts the feature selected features to a list of ObjectIDs (OIDs)
        /// </summary>
        /// <param name="featureSelection">The feature selection.</param>
        /// <returns>OIDList of objectids</returns>
        public static OIDList ToOIDList(this IFeatureSelection featureSelection)
        {
            OIDList oidlist = new OIDList();

            IEnumIDs enumids = featureSelection.SelectionSet.IDs;

            int id = -1;

            while ((id = enumids.Next()) != -1)
            {
                oidlist.Add(id);
            }

            return oidlist;
        }

        /// <summary>
        /// Converts the feature selected features to a list of ObjectIDs (OIDs)
        /// </summary>
        /// <param name="featureclass">The IFeatureClass.</param>
        /// <returns>OIDList of objectids</returns>
        public static OIDList ToOIDList(this IFeatureClass featureclass)
        {
            return ((ITable)featureclass).ToOIDList();
        }

        /// <summary>
        /// Converts the feature selected features to a list of ObjectIDs (OIDs)
        /// </summary>
        /// <param name="table">The ITable.</param>
        /// <returns>OIDList of objectids</returns>
        public static OIDList ToOIDList(this ITable table)
        {
            IQueryFilter filter = new QueryFilterClass();
            filter.SubFields = table.OIDFieldName;

            OIDList oidlist = new OIDList();

            ICursor cursor = table.Search(filter, true);
            IRow row = null;

            while ((row = cursor.NextRow()) != null)
            {
                oidlist.Add(row.OID);
            }

            return oidlist;
        }

        /// <summary>
        /// Converts a feature cursor to a list of ObjectIDs (OIDs)
        /// </summary>
        /// <param name="cursor">The cursor.</param>
        /// <returns>OIDList of object ids of features in the feature cursor</returns>
        public static OIDList ToOIDList(this IFeatureCursor cursor)
        {
            OIDList oidlist = new OIDList();
            IFeature feature = null;

            while ((feature = cursor.NextFeature()) != null)
            {
                if (feature.HasOID)
                {
                    oidlist.Add(feature.OID);
                }
            }

            return oidlist;
        }

        /// <summary>
        /// Creates an object array from an OIDList
        /// </summary>
        /// <param name="oidlist">The oidlist.</param>
        /// <returns>an object array of the integers in the OIDList</returns>
        public static object[] ToObjectArray(this OIDList oidlist)
        {
            List<object> objectlist = oidlist.ConvertAll<object>(x => x as object);
            return objectlist.ToArray();
        }

        /// <summary>
        /// Adds the WKB column (byte[]) names WKB
        /// </summary>
        /// <param name="table">DataTable to which the new column is added.</param>
        public static void AddWKBColumn(this DataTable table)
        {
            DataColumn wkbcolumn = new DataColumn("WKB", typeof(byte[]));
            table.Columns.Add(wkbcolumn);
        }

        /// <summary>
        /// Converts an IPropertySet to a Dictionary
        /// </summary>
        /// <param name="propertySet">The IPropertySet</param>
        /// <returns>PropertyDictionary of property names/values</returns>
        public static PropertyDictionary ToDictionary(this IPropertySet propertySet)
        {
            PropertyDictionary dict = new PropertyDictionary();

            object propsetNames = new object[propertySet.Count - 1];
            object propsetValues = new object[propertySet.Count - 1];

            propertySet.GetAllProperties(out propsetNames, out propsetValues);

            object[] propsetNameArray = (object[])propsetNames;
            object[] propsetValueArray = (object[])propsetValues;

            for (int i = 0; i < propertySet.Count; i++)
            {
                dict.Add(propsetNameArray[i].ToString(), propsetValueArray[i].ToString());
            }

            return dict;
        }

        /// <summary>
        /// Converts a 2-column datatable to IPropertySet
        /// </summary>
        /// <param name="table">The DataTable (2 columns only)</param>
        /// <returns>IPropertySet of string,string</returns>
        public static IPropertySet ToPropertySet(this DataTable table)
        {
            if (table.Columns.Count > 2)
            {
                throw new ArgumentOutOfRangeException("Cannot convert tables with more than 2 columns to IPropertySet");
            }

            IPropertySet propertySet = new PropertySetClass();

            foreach (DataRow row in table.Rows)
            {
                propertySet.SetProperty(row[0].ToString(), row[1].ToString());
            }

            return propertySet;
        }

        /// <summary>
        /// Searches ILayer's layerextensions for any PropertySets that contain name/value combo
        /// and this method uses a default string comparison value of CurrentCultureIgnoreCase
        /// </summary>
        /// <param name="layer">The ILayer</param>
        /// <param name="propertysetname">The propertyset name.</param>
        /// <param name="propertysetvalue">The propertyset value.</param>
        /// <returns>List of IPropertySet</returns>
        public static List<IPropertySet> FindExtensionPropertySet(this ILayer layer, string propertysetname, string propertysetvalue)
        {
            return layer.FindExtensionPropertySet(propertysetname, propertysetvalue, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Searches ILayer's layerextensions for any PropertySets that contain name/value combo
        /// </summary>
        /// <param name="layer">The ILayer</param>
        /// <param name="propertysetname">The propertyset name.</param>
        /// <param name="propertysetvalue">The propertyset value.</param>
        /// <param name="comparison">The string comparison method</param>
        /// <returns>List of IPropertySet</returns>
        public static List<IPropertySet> FindExtensionPropertySet(this ILayer layer, string propertysetname, string propertysetvalue, StringComparison comparison)
        {
            List<IPropertySet> propertySets = new List<IPropertySet>();

            ILayerExtensions layerExtensions = (ILayerExtensions)layer;

            try
            {
                if (layerExtensions.ExtensionCount > 0)
                {
                    for (int i = 0; i < layerExtensions.ExtensionCount; i++)
                    {
                        object layerExtension = layerExtensions.get_Extension(i);

                        if (layerExtension is IPropertySet)
                        {
                            IPropertySet propertySet = (IPropertySet)layerExtension;

                            try
                            {
                                if (propertySet.GetProperty(propertysetname).ToString().Equals(propertysetvalue, comparison))
                                {
                                    propertySets.Add(propertySet);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                return propertySets;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// returns a "square" envelope around a point
        /// using the width value as the argument
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="width">width to use in constructing the envelope</param>
        /// <returns>envelope around a point</returns>
        public static IEnvelope GetEnvelope(this IPoint point, double width)
        {
            return point.GetEnvelope(width, width);
        }

        /// <summary>
        /// returns a "rectangle" envelope around a point
        /// using the width and height arguments:
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="width">width to use in constructing the envelope</param>
        /// <param name="height">The height.</param>
        /// <returns>envelope around a point</returns>
        public static IEnvelope GetEnvelope(this IPoint point, double width, double height)
        {
            IEnvelope env = null;

            if (point != null)
            {
                env = (IEnvelope)new EnvelopeClass();

                env.PutCoords(point.X - (width / 2), point.Y - (height / 2), point.X + (width / 2), point.Y + (height / 2));
            }

            return env;
        }

        /// <summary>
        /// Releases the specified com object
        /// </summary>
        /// <param name="comobject">The com object.</param>
        /// <returns>The new value of the reference count of the RCW associated with com object</returns>
        public static int ReleaseComObject(this object comobject)
        {
            int r = 0;

            if (comobject != null)
            {
                r = System.Runtime.InteropServices.Marshal.ReleaseComObject(comobject);
            }

            return r;
        }
    }
}
