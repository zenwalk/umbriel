// <copyright file="LayerExtHelper.cs" company="Earth">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-08-17</date>
// <summary>LayerExtHelper class file</summary>

namespace Umbriel.ArcGIS.Layer.Util
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.esriSystem;

    /// <summary>
    /// Static methods for Umbriel Layer Extension (Umbriel Layer Extension is a PropertySet of strings.
    /// </summary>
    internal class LayerExtHelper
    {
        /// <summary>
        /// Determines if the Umbriel property set exists in the layer extensions
        /// </summary>
        /// <param name="layerExtensions">The layer extensions.</param>
        /// <returns>true if exists, false if it doont</returns>
        internal static bool UmbrielPropertySetExists(ILayerExtensions layerExtensions)
        {
            try
            {
                bool exists = false;

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
                                string val = propertySet.GetProperty("LayerExtension").ToString();
                                if (val.Equals("umbriel", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    exists = true;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                return exists;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Creates a property sets the LayerExtension property to "Umbriel"
        /// </summary>
        /// <returns>IPropertySet for Umbriel Layer Extention</returns>
        internal static IPropertySet CreateUmbrielPropertySet()
        {
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("LayerExtension", "Umbriel");
            return propertySet;
        }

        /// <summary>
        /// Gets the umbriel IPropertySet from ILayerExtensions
        /// </summary>
        /// <param name="layerExtensions">The ILayerExtensions interface to ILayer</param>
        /// <returns>IPropertySet for Umbriel Layer Extention--returns null if no Umbriel layer extension exists</returns>
        internal static IPropertySet GetUmbrielPropertySet(ILayerExtensions layerExtensions)
        {
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
                                string val = propertySet.GetProperty("LayerExtension").ToString();
                                if (val.Equals("umbriel", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    return propertySet;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Loads the name/values in IPropertySet into a new DataTable
        /// </summary>
        /// <param name="propertySet">The property set.</param>
        /// <returns>DataTable of name/values</returns>
        internal static DataTable ToDataTable(IPropertySet propertySet)
        {
            DataTable table = new DataTable("UmbrielLayerExtension");

            DataColumn nameCol = new DataColumn("PropertyName", System.Type.GetType("System.String"));
            DataColumn valueCol = new DataColumn("PropertyValue", System.Type.GetType("System.String"));

            table.Columns.Add(nameCol);
            table.Columns.Add(valueCol);

            object propsetNames = new object[propertySet.Count - 1];
            object propsetValues = new object[propertySet.Count - 1];

            propertySet.GetAllProperties(out propsetNames, out propsetValues);

            object[] propsetNameArray = (object[])propsetNames;
            object[] propsetValueArray = (object[])propsetValues;

            for (int i = 0; i < propertySet.Count; i++)
            {
                DataRow row = table.NewRow();
                row[0] = propsetNameArray[i].ToString();
                row[1] = propsetValueArray[i].ToString();
                table.Rows.Add(row);
            }

            return table;
        }

        /// <summary>
        /// Updates the property set with the settings in datatable
        /// </summary>
        /// <param name="propertySet">The IPropertySet that needs to be updated.</param>
        /// <param name="table">DataTable of name/value pairs</param>
        internal static void UpdatePropertySet(IPropertySet propertySet, DataTable table)
        {
            try
            {
                foreach (DataRow row in table.Rows)
                {
                    propertySet.SetProperty(row[0].ToString(), row[1].ToString());
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}
