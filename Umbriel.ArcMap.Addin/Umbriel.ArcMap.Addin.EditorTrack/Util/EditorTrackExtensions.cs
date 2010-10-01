﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using StringDictionary = System.Collections.Generic.Dictionary<string, string>;
using Replacements = System.Collections.Generic.Dictionary<string, object>;

namespace Umbriel.ArcMap.Addin.EditorTrack
{
    public static class EditorTrackExtensions
    {
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

        public static StringDictionary ToStringDictionary(this List<string> list)
        {
            char[] separator = { ',' };

            return list.ToStringDictionary(separator);
        }

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

        public static object GetReplacementValue(this Replacements r, string fieldName)
        {
            object o = r[fieldName];

            return o;
        }

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

        public static string ReplaceMany(this string s, Replacements replacements)
        {
            StringBuilder sb = new StringBuilder(s);
            foreach (var replacement in replacements)
            {
                sb = sb.Replace(replacement.Key, replacement.Value.ToString());
            }
            return sb.ToString();
        }

        public static string ToGeohash(this ESRI.ArcGIS.Geometry.IGeometry geometry)
        {
            return geometry.ToGeohash(Constants.DefaultGeohashPrecision);
        }

        public static string ToGeohash(this ESRI.ArcGIS.Geometry.IGeometry geometry, int precision)
        {
            string geohash  = string.Empty;
            if (geometry is IPoint)
            {
                IPoint point = (IPoint)geometry as IPoint;
                geohash = Umbriel.ArcMap.Editor.Util.Geohasher.CreateGeohash(point,13);
            }

            return geohash;
        }

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