using System;
using System.Collections.Generic;
using System.Data;
using OIDList = System.Collections.Generic.List<int>;
using Reflect = System.Reflection;

namespace Umbriel.Extensions
{
    /// <summary>
    /// GIS Extension Methods (non-ESRI)
    /// </summary>
    public static class GISExtensions
    {
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
}