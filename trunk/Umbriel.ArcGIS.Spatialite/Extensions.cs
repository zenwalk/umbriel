using System;
using System.Collections.Generic;
using System.Data;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Umbriel.ArcGIS.Spatialite;
using OIDList = System.Collections.Generic.List<int>;
using LayerList = System.Collections.Generic.List<ILayer>;
using Reflect = System.Reflection;


public static class Extensions
{
    /// <summary>
    /// Gets a list of layers from the selecteditems ISet reference
    /// </summary>
    /// <param name="o">The selected items (object)</param>
    /// <returns></returns>
    public static List<ILayer> ToLayerList(this object o)
    {
        List<ILayer> layers = new List<ILayer>();

        if (o != null && o is ISet )
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
    /// Gets the string value of enum
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>string</returns>
    public static string GetStringValue(this Enum value)
    {
        // Get the type
        Type type = value.GetType();

        // Get fieldinfo for this type
        Reflect.FieldInfo fieldInfo = type.GetField(value.ToString());

        // Get the stringvalue attributes
        StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
            typeof(StringValueAttribute), false) as StringValueAttribute[];

        // Return the first if there was a match.
        return attribs.Length > 0 ? attribs[0].StringValue : null;
    }

    public static ISpatialReference GetSpatialReference(this ILayer value)
    {
        IGeoDataset g = (IGeoDataset)value;
        return g.SpatialReference;
    }

    public static string ToProj4(this ISpatialReference sr)
    {
        string proj4text = string.Empty;

        //TODO: write ToProj4 method

        return proj4text;
    }

    public static bool  Is3D(this ILayer layer)
    {        
        IFeatureLayer featureLayer = (IFeatureLayer)layer;

        IField field = featureLayer.FeatureClass.GetField(featureLayer.FeatureClass.ShapeFieldName);

        IGeometryDef geometryDefinition = field.GeometryDef;

         return  geometryDefinition.HasZ;
    }

    public static IField GetField(this IFeatureClass featureclass,  string fieldName)
    {
        return ((ITable)featureclass).GetField(fieldName);
    }
    
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
    
    public static OIDList ToOIDList(this IFeatureClass featureclass)
    {
        return ((ITable)featureclass).ToOIDList();
    }

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

    public static object[] ToObjectArray(this OIDList oidlist)
    {
       List<object> objectlist =  oidlist.ConvertAll<object> (x => x as object);
       return objectlist.ToArray(); ;        
    }

    public static void AddWKBColumn(this DataTable table)
    {
        DataColumn wkbcolumn = new DataColumn("WKB", typeof(byte[]));
        table.Columns.Add(wkbcolumn);
    }

}