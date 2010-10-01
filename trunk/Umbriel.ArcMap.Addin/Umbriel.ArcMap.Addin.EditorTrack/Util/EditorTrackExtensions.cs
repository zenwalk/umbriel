// <copyright file="EditorTrackExtensions.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-10-01</date>
// <summary>EditorTrackExtensions class file</summary>

namespace Umbriel.ArcMap.Addin.EditorTrack
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Text.RegularExpressions;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;
    using Replacements = System.Collections.Generic.Dictionary<string, object>;
    using StringDictionary = System.Collections.Generic.Dictionary<string, string>;

    /// <summary>
    /// EditorTrack Extension Methods
    /// </summary>
    public static class EditorTrackExtensions
    {
        /// <summary>
        /// Finds the environment replacements variable contained in a string.
        /// </summary>
        /// <param name="s">The input string variable</param>
        /// <returns>List of replacement variables strings</returns>
        public static List<string> FindEnvironmentVariableReplacements(this string s)
        {
            int i = 0;
            List<string> envars = new List<string>();

            string pattern = @"{e[:](.*?)}";
            foreach (Match match in Regex.Matches(s, pattern))
            {
                Trace.WriteLine(match.Value);

                envars.Add(match.Value.Replace("{e:", string.Empty).Replace("}", string.Empty));

                i++;
            }

            Trace.WriteLine(string.Format("FindEnvironmentVariableReplacements match count={0}.", i));

            return envars;
        }

        /// <summary>
        /// Converts a list of delimited strings (key,value) to a String dictionary (Dictionary string,string).
        /// </summary>
        /// <param name="list">The string list</param>
        /// <returns>
        /// String dictionary (Dictionary string,string)
        /// </returns>
        public static StringDictionary ToStringDictionary(this List<string> list)
        {
            char[] separator = { ',' };

            return list.ToStringDictionary(separator);
        }

        /// <summary>
        /// Converts a list of delimited strings (key,value) to a String dictionary (Dictionary string,string).
        /// </summary>
        /// <param name="list">The string list</param>
        /// <param name="separator">The separator</param>
        /// <returns>String dictionary (Dictionary string,string)</returns>
        public static StringDictionary ToStringDictionary(this List<string> list, char[] separator)
        {
            StringDictionary dictionary = new StringDictionary();

            foreach (string s in list)
            {
                string[] t = s.Split(separator);
                dictionary.Add(t[0], t[1]);
            }

            return dictionary;
        }

        /// <summary>
        /// Gets the replacement value from the replacement dictionary
        /// </summary>
        /// <param name="r">The input replacement dictionary</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>object that should replace the replacement variable placeholder</returns>
        public static object GetReplacementValue(this Replacements r, string fieldName)
        {
            object o = r[fieldName];

            return o;
        }

        /// <summary>
        /// Gets the name of the version.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <returns>Version Name string </returns>
        public static string GetVersionName(this IWorkspace workspace)
        {
            string versionName = string.Empty;
            try
            {
                IVersion version = workspace as IVersion;

                if (version != null)
                {
                    versionName = version.VersionName;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.StackTrace);
            }

            return versionName;
        }

        /// <summary>
        /// Replaces any replacement variable  placeholders with actual values
        /// contained in the replacement dictionary
        /// </summary>
        /// <param name="s">The input string containing placeholders</param>
        /// <param name="replacements">The replacement dictionary</param>
        /// <returns>new string updated with actual values</returns>
        public static string ReplaceMany(this string s, Replacements replacements)
        {
            StringBuilder sb = new StringBuilder(s);
            foreach (var replacement in replacements)
            {
                sb = sb.Replace(replacement.Key, replacement.Value.ToString());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts point geometry to geohash
        /// </summary>
        /// <param name="geometry">The input geometry</param>
        /// <returns>geohash string </returns>
        public static string ToGeohash(this ESRI.ArcGIS.Geometry.IGeometry geometry)
        {
            return geometry.ToGeohash(Constants.DefaultGeohashPrecision);
        }

        /// <summary>
        /// Converts point geometry to geohash
        /// </summary>
        /// <param name="geometry">The input geometry</param>
        /// <param name="precision">The precision.</param>
        /// <returns>geohash string</returns>
        public static string ToGeohash(this ESRI.ArcGIS.Geometry.IGeometry geometry, int precision)
        {
            string geohash  = string.Empty;
            if (geometry is IPoint)
            {
                IPoint point = (IPoint)geometry as IPoint;
                geohash = Umbriel.ArcMap.Editor.Util.Geohasher.CreateGeohash(point, 13);
            }

            return geohash;
        }

        /// <summary>
        /// Converts geometry into WKB
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns>byte array of a WKB geometry</returns>
        public static byte[] ToWKB(this IGeometry geometry)
        {
            IWkb wkb = geometry as IWkb;
            ITopologicalOperator oper = geometry as ITopologicalOperator;
            oper.Simplify();

            IGeometryFactory3 factory = new GeometryEnvironment() as IGeometryFactory3;
            byte[] b = factory.CreateWkbVariantFromGeometry(geometry) as byte[];
            return b;
        }
    }
}
