// <copyright file="GISExtensions.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-06-25</date>
// <summary>GISExtensions class file</summary>

namespace Umbriel.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using OIDList = System.Collections.Generic.List<int>;
    using Reflect = System.Reflection;

    /// <summary>
    /// GIS Extension Methods (non-ESRI)
    /// </summary>
    public static class GISExtensions
    {
        /// <summary>
        /// Gets the string value of enum
        /// </summary>
        /// <param name="value">The Enum value.</param>
        /// <returns>string value attribute of the Enum value</returns>
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