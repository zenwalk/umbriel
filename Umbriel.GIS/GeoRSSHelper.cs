// <copyright file="GeoRSSHelper.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-06-25</date>
// <summary>GeoRSSHelper class file</summary>

namespace Umbriel.GIS
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Xml.Linq;
    using System.Web;
    using System.Diagnostics;


    /// <summary>
    /// GeoRSS Helper Class
    /// </summary>
    public class GeoRSSHelper
    {
        /// <summary>
        /// Reads the georss feed into a datatable
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>DataTable of georss items</returns>
        public static DataTable ReadFeed(string uri)
        {
            Stopwatch stopwatch = new Stopwatch();

            Trace.WriteLine("Begin GeoRSSHelper.ReadFeed");
            stopwatch.Start();

            DataTable table = new DataTable("GeoRSS");

            XDocument georss = XDocument.Load(uri);

            Trace.WriteLine(georss.ToString());

            XNamespace atomNS = "http://www.w3.org/2005/Atom";
            XNamespace georssNS = "http://www.georss.org/georss";
            XNamespace geoNS = "http://www.w3.org/2003/01/geo/wgs84_pos#";

            var q = from georssitem in georss.Descendants("item")
                    select new
                    {
                        title = (string)georssitem.Element("title"),
                        link = (string)georssitem.Element("link"),
                        description = (string)georssitem.Element("description"),
                        pubDate = (string)georssitem.Element("pubDate"),
                        guid = (string)georssitem.Element("guid"),
                        pointstring = (string)georssitem.Element(georssNS + "point"),
                        latitude = (double)georssitem.Element(geoNS + "lat"),
                        longitude = (double)georssitem.Element(geoNS + "long")
                    };

            Trace.WriteLine(q.ToString());
            
            table.Columns.Add("title");
            table.Columns.Add("link");
            table.Columns.Add("description");
            table.Columns.Add("pubdate");
            table.Columns.Add("guid");
            table.Columns.Add("pointstring");
            table.Columns.Add("lat", Type.GetType("System.Double"));
            table.Columns.Add("lon", Type.GetType("System.Double"));

            foreach (var item in q)
            {
                table.Rows.Add(
                    item.title,
                    item.link,
                    item.description,
                    item.pubDate,
                    item.guid,
                    item.pointstring,
                    item.latitude,
                    item.longitude);
            }

            stopwatch.Stop();

            Trace.WriteLine(string.Format(
                "{0} rows retrieved in {1} second(s).",
                table.Rows.Count,
                stopwatch.ElapsedMilliseconds.ToFloat() / 1000));

            return table;
        }
    }

    public static class Extensions
    {
        public static float ToFloat(this string s)
        {
            float output;

            if (float.TryParse(s, out output))
            {
                return output;
            }
            else
            {
                throw new System.ArgumentException("String is not numeric.");
            }
        }

        public static float ToFloat(this object s)
        {
            return s.ToString().ToFloat();
        }
    }
}
