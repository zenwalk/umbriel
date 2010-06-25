using System;
using System.Collections.Generic;
using System.Data;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using OIDList = System.Collections.Generic.List<int>;
using LayerList = System.Collections.Generic.List<ESRI.ArcGIS.Carto.ILayer>;
using Reflect = System.Reflection;

namespace Umbriel.Extensions
{
    /// <summary>
    /// static class of extension methods to use with ESRI ArcObjects
    /// </summary>
    public static class ArcGISExtensions
    {
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
        /// <returns>ISpatialReference</returns>
        public static ISpatialReference GetSpatialReference(this ILayer value)
        {
            IGeoDataset g = (IGeoDataset)value;
            return g.SpatialReference;
        }

        /// <summary>
        /// Converts the ISpatialReference to Proj4 //TODO: write ToProj4 method
        /// </summary>
        /// <param name="sr">The sr.</param>
        /// <returns></returns>
        public static string ToProj4(this ISpatialReference sr)
        {
            string proj4text = string.Empty;

            //TODO: write ToProj4 method

            return proj4text;
        }

        /// <summary>
        /// Determines if the ILayer has Z in geometry definition
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <returns></returns>
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
        /// Creates an object array from an OIDList
        /// </summary>
        /// <param name="oidlist">The oidlist.</param>
        /// <returns></returns>
        public static object[] ToObjectArray(this OIDList oidlist)
        {
            List<object> objectlist = oidlist.ConvertAll<object>(x => x as object);
            return objectlist.ToArray(); ;
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
    }
}
