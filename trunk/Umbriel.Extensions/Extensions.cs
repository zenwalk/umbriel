// <copyright file="Extensions.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-06-25</date>
// <summary>Extensions class file</summary>

namespace Umbriel.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using OIDList = System.Collections.Generic.List<int>;
    using Reflect = System.Reflection;

    /// <summary>
    /// Other(misc) Extension Methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Determines whether the specified s is numeric.
        /// </summary>
        /// <param name="s">The string value</param>
        /// <returns>
        ///  <c>true</c> if the specified s is numeric; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumeric(this string s)
        {
            double numberValue = 0;
            return double.TryParse(s, out numberValue);
        }

        /// <summary>
        /// Determines whether the specified o is numeric.
        /// </summary>
        /// <param name="o">The object to test</param>
        /// <returns>
        ///  <c>true</c> if the specified o is numeric; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumeric(this object o)
        {
            double numberValue = 0;
            return double.TryParse(o.ToString(), out numberValue);
        }

        /// <summary>
        /// Converts a string value to boolean.
        /// </summary>
        /// <param name="s">The string value</param>
        /// <returns>boolean true/false</returns>
        public static bool ToBoolean(this string s)
        {
            int value = 0;

            if (int.TryParse(s, out value))
            {
                return value.Equals(1);
            }
            else
            {
                if (s.Length > 0)
                {
                    return s.Trim().Equals("true", System.StringComparison.CurrentCultureIgnoreCase);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Converts string to float
        /// </summary>
        /// <param name="s">The string value</param>
        /// <returns>float parsed from 's'</returns>
        public static float ToFloat(this string s)
        {
            float value = 0;

            if (s.IsNumeric())
            {
                if (float.TryParse(s, out value))
                {
                    return value;
                }
                else
                {
                    throw new System.Exception("Could not parse string value: " + s);
                }
            }
            else
            {
                throw new System.ArgumentException("String is not numeric.");
            }
        }

        /// <summary>
        /// Converts object to string then to float
        /// </summary>
        /// <param name="o">The object to convert</param>
        /// <returns>float parsed from 'o' (as String)</returns>
        public static float ToFloat(this object o)
        {
            return o.ToString().ToFloat();
        }

        /// <summary>
        /// Converts object to string then to double
        /// </summary>
        /// <param name="o">The object to convert</param>
        /// <returns>double parsed from 'o' (as String)</returns>
        public static double ToDouble(this object o)
        {
            double value = 0;

            if (o.IsNumeric())
            {
                if (double.TryParse(o.ToString(), out value))
                {
                    return value;
                }
                else
                {
                    throw new System.Exception("Could not parse object value: " + o.ToString());
                }
            }
            else
            {
                throw new System.ArgumentException("object is not numeric.");
            }
        }

        /// <summary>
        /// Extension method for string.Format
        /// </summary>
        /// <param name="format">The string to be formatted.</param>
        /// <param name="args">object parameters.</param>
        /// <returns>formatted string</returns>
        public static string FormatString(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// Extension method for string.Format
        /// </summary>
        /// <param name="format">The string to be formatted.</param>
        /// <param name="provider">The IFormatProvider</param>
        /// <param name="args">object parameters.</param>
        /// <returns>formatted string</returns>
        public static string FormatString(this string format, IFormatProvider provider, params object[] args)
        {
            return string.Format(provider, format, args);
        }        
    }
}