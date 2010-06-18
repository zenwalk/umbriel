using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ArcMapUI;
using Reflect=System.Reflection;
using Umbriel.ArcGIS.Layer;
using Umbriel.ArcGIS.Spatialite;


public static class Extensions
{
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
}