// <copyright file="LMStatParser.cs" company="Earth">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-11-13</date>
// <summary>
////</summary>


namespace Umbriel.GIS.LMStat
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;

    /// <summary>
    /// struct for license information parsed from an lmstat file
    /// </summary>
    internal struct LicenseType
    {
        /// <summary>
        /// Name of the license
        /// </summary>
        internal string LicenseName;

        /// <summary>
        /// line number in the lmstat text file
        /// </summary>
        internal int LineLocation;

        /// <summary>
        /// total licenses available 
        /// </summary>
        internal int TotalLicenses;

        /// <summary>
        /// in use licenses
        /// </summary>
        internal int InUseLicenses;

        /// <summary>
        /// date/time of the lmstat file
        /// </summary>
        internal DateTime StatDateTime;

        /// <summary>
        /// name of the license manager server
        /// </summary>
        internal string ServerName;
    }

    /// <summary>
    /// Parser class for an LMSTAT text file
    /// </summary>
    public class LMStatParser
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LMStatParser"/> class.
        /// </summary>
        public LMStatParser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LMStatParser"/> class.
        /// </summary>
        /// <param name="path">The full file path</param>
        public LMStatParser(string path)
        {
            this.StatsFilePath = path;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the stats file path.
        /// </summary>
        /// <value>The stats file path.</value>
        public string StatsFilePath { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Parses the lmstats file in the StatsFilePath
        /// </summary>
        /// <returns>datatable of licenses</returns>
        public DataTable Parse()
        {
            if (!System.IO.File.Exists(this.StatsFilePath))
            {
                throw new FileNotFoundException("LMStatParser: File not found", this.StatsFilePath);
            }
            else
            {
                try
                {
                    DataTable table = this.NewLicenseTable();
                    List<LicenseType> licenses = this.GetLicenseTypes();

                    foreach (LicenseType lic in licenses)
                    {
                        DataRow row = table.NewRow();

                        row["MachineName"] = lic.ServerName;
                        row["LicenseName"] = lic.LicenseName;
                        row["TotalLicense"] = lic.TotalLicenses;
                        row["InUseLicense"] = lic.InUseLicenses;
                        row["StatusDateTime"] = lic.StatDateTime;

                        table.Rows.Add(row);
                    }

                    return table;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                    throw;
                }      
            }       
        }

        /// <summary>
        /// Creates a newlicense table.
        /// </summary>
        /// <returns>datatable for licenses</returns>
        private DataTable NewLicenseTable()
        {
            try
            {
                DataTable table = new DataTable();
                DataColumn dc = null;

                dc = new DataColumn("MachineName", System.Type.GetType("System.String"));
                table.Columns.Add(dc);

                dc = new DataColumn("LicenseName", System.Type.GetType("System.String"));
                table.Columns.Add(dc);

                dc = new DataColumn("TotalLicense", System.Type.GetType("System.Int32"));
                table.Columns.Add(dc);

                dc = new DataColumn("InUseLicense", System.Type.GetType("System.Int32"));
                table.Columns.Add(dc);

                dc = new DataColumn("StatusDateTime", System.Type.GetType("System.DateTime"));
                table.Columns.Add(dc);

                return table;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Gets the license types.
        /// </summary>
        /// <returns>list of license types w/ attributes for each license (total, in-use)</returns>
        private List<LicenseType> GetLicenseTypes()
        {
            try
            {
                List<LicenseType> licTypes = new List<LicenseType>();
                FileInfo fileInfo = new FileInfo(this.StatsFilePath);

                TextReader reader = new StreamReader(this.StatsFilePath);

                string line = string.Empty;
                int counter = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    counter++;

                    if (line.Length > 0)
                    {
                        if (line.IndexOf("Users of ") > -1)
                        {
                            LicenseType licType = new LicenseType();
                            licType.LineLocation = counter;
                            licType.LicenseName = this.ParseLicenseName(line);
                            licType.TotalLicenses = this.ParseTotalLicenses(line);
                            licType.InUseLicenses = this.ParseInUseLicenses(line);
                            licType.StatDateTime = fileInfo.LastWriteTime;
                            licType.ServerName = this.ParseMachineName();

                            licTypes.Add(licType);
                        }

                        Console.WriteLine(counter.ToString() + ":  " + line);
                    }
                }

                reader.Close();

                return licTypes;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Parses the name of the license
        /// </summary>
        /// <param name="line">The line of text containing the license name</param>
        /// <returns>name of the license</returns>
        private string ParseLicenseName(string line)
        {
            try
            {
                // e.g. Users of ARC/INFO:  (Total of 50 licenses issued;  Total of 4 licenses in use)
                string usersofstring = "Users of ";
                int colon = line.IndexOf(':');
                int usersof = line.IndexOf("Users of ");
                int startIndex = usersof + usersofstring.Length;

                int length = colon - startIndex;

                System.Diagnostics.Debug.WriteLine(line.Substring(startIndex, length));

                return line.Substring(startIndex, length);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Parses the number of total licenses
        /// </summary>
        /// <param name="line">The line of text containing  the license count</param>
        /// <returns>count of total licenses</returns>
        private int ParseTotalLicenses(string line)
        {
            try
            {
                // e.g. Users of ARC/INFO:  (Total of 50 licenses issued;  Total of 4 licenses in use)
                string startMarker = "(Total of ";
                int indexStartMarker = line.IndexOf(startMarker);
                int startIndex = indexStartMarker + startMarker.Length;

                // single space is the end marker
                string endMarker = " ";
                int indexEndMarker = line.IndexOf(endMarker, startIndex);

                int length = indexEndMarker - startIndex;

                System.Diagnostics.Debug.WriteLine(line.Substring(startIndex, length));
                string subString = line.Substring(startIndex, length);

                int totalLicenseCount = -1;

                if (!int.TryParse(subString, out totalLicenseCount))
                {
                    return -1;
                }
                else
                {
                    return totalLicenseCount;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }      
        }

        /// <summary>
        /// Parses the in use licenses
        /// </summary>
        /// <param name="line">The line of text containing  the license count</param>
        /// <returns>count of in-use licenses</returns>
        private int ParseInUseLicenses(string line)
        {
            try
            {
                // e.g. Users of ARC/INFO:  (Total of 50 licenses issued;  Total of 4 licenses in use)
                string startMarker = ";  Total of ";
                int indexStartMarker = line.IndexOf(startMarker);
                int startIndex = indexStartMarker + startMarker.Length;

                // single space is the end marker
                string endMarker = " license";
                int indexEndMarker = line.IndexOf(endMarker, startIndex);

                int length = indexEndMarker - startIndex;

                if (length <= 0)
                {
                    System.Diagnostics.Debug.WriteLine("Zero Length");
                }

                System.Diagnostics.Debug.WriteLine(line.Substring(startIndex, length));

                string subString = line.Substring(startIndex, length);

                int inuseLicenseCount = -1;

                if (!int.TryParse(subString, out inuseLicenseCount))
                {
                    return -1;
                }
                else
                {
                    return inuseLicenseCount;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }           
        }

        /// <summary>
        /// Parses the name of the machine from the lmstat file
        /// </summary>
        /// <returns>machine name string</returns>
        private string ParseMachineName()
        {
            try
            {
                string name = string.Empty;

                TextReader reader = new StreamReader(this.StatsFilePath);
                string text = reader.ReadToEnd();
                reader.Close();

                string startMarker = "License server status: ";
                int indexStartMarker = text.IndexOf(startMarker);
                int startIndex = indexStartMarker + startMarker.Length;

                string endMarker = "\r\n";
                int indexEndMarker = text.IndexOf(endMarker, startIndex);

                int length = indexEndMarker - startIndex;

                System.Diagnostics.Debug.WriteLine(text.Substring(startIndex, length));

                string subString = text.Substring(startIndex, length);

                return subString;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }                  
        }
        #endregion
    }
}
