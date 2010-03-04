

namespace Umbriel.ArcGIS.Layer.ConnectionReport
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using ESRI.ArcGIS.esriSystem;

    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new Umbriel.ArcGIS.Layer.ConnectionReport.LicenseInitializer();

        public static CommandLine.Utility.Arguments Arguments { get; private set; }

        public static string ReportFilePath { get; private set; }

        public static string ExcludeDBPath { get; private set; }

        //-d "\\w-dpu-48\data\AdminHome\ARM03\Image 5_30_07\Desktop\StreetLights"  -o "\\w-dpu-48\dpu_sys\GIS\wdpu48.data.AdminHome.connectionreport.txt" -mindate 2006-05-15 -maxdate 2006-05-31

        [STAThread()]
        static void Main(string[] args)
        {

            if (args.Length.Equals(0))
            {
                Usage();
                return;
            }

            //ESRI License Initializer generated code.
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeArcView },
            new esriLicenseExtensionCode[] { });

            Arguments = new CommandLine.Utility.Arguments(args);


            string mxdfilepath = string.Empty;
            string directorypath = string.Empty;
            string excludedbpath = string.Empty;
            
            if (Arguments["mxd"] != null)
            {
                mxdfilepath = Arguments["mxd"];
            }
            else if (Arguments["d"] != null)
            {
                directorypath = Arguments["d"];
            }

            if (Arguments["x"] != null)
            {
                ExcludeDBPath = Arguments["x"];
            }

            if (!string.IsNullOrEmpty(mxdfilepath))
            {
                AnalyzeMXD(mxdfilepath);
            }
            else if (!string.IsNullOrEmpty(directorypath))
            {
                if (Arguments["o"] != null)
                {
                    ReportFilePath = Arguments["o"];
                }

                AnalyzeMXDDirectory(directorypath);
            }
            
            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()
            m_AOLicenseInitializer.ShutdownApplication();
        }


        /// <summary>
        /// Analyzes the MXD.
        /// </summary>
        /// <param name="mxdfilepath">The mxdfilepath.</param>
        private static void AnalyzeMXD(string mxdfilepath)
        {
            Console.WriteLine("Analyzing: " + mxdfilepath);
            string reportfilepath = string.Empty;

            if (Arguments["o"] != null)
            {
                reportfilepath = Arguments["o"];
            }

            if (!ExcludeMXD(mxdfilepath))
            {
                MXDAnalyzer analyzer = new MXDAnalyzer(mxdfilepath, reportfilepath);

                analyzer.GenerateReport();

                Console.WriteLine("Analysis of " + mxdfilepath + "complete.");
            }
            else
            {
                Console.WriteLine("Exclude " + mxdfilepath );
            }
            return;
        }

        /// <summary>
        /// Excludes the MXD.
        /// </summary>
        /// <param name="mxdfilepath">The mxdfilepath.</param>
        /// <returns></returns>
        private static bool ExcludeMXD(string mxdfilepath)
        {
            bool exclude = false;
            if (!string.IsNullOrEmpty(ExcludeDBPath))
            {
                using (System.Data.SQLite.SQLiteConnection connection = new System.Data.SQLite.SQLiteConnection())
                {
                    connection.ConnectionString = "Data Source=" + ExcludeDBPath + ";Version=3;";
                    connection.Open();

                    string sql = "select count(*) from exclude where excludeitem = '{0}'";
                    string path = mxdfilepath.Replace("'", "''");

                    using (System.Data.SQLite.SQLiteCommand command = new System.Data.SQLite.SQLiteCommand(string.Format(sql,path),connection ))
                    {
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            int recordcount = Convert.ToInt32(result);
                            exclude = recordcount > 0;
                        }
                    }

                    connection.Close();
                }

            }

            return exclude;
        }

        /// <summary>
        /// Analyzes the MXD directory.
        /// </summary>
        /// <param name="directorypath">The directorypath.</param>
        private static void AnalyzeMXDDirectory(string directorypath)
        {
            DirectoryInfo di = new DirectoryInfo(directorypath);

            if (Arguments.Contains("s"))
            {
                WalkDirectoryTree(di, "*.mxd");
            }
            else
            {
                foreach (FileInfo fi in di.GetFiles("*.mxd"))
                {
                    if (ValidateDateLastModified(fi.LastWriteTime))
                    {
                        AnalyzeMXD(fi.FullName);
                    }
                }
            }
        }

        static void WalkDirectoryTree(DirectoryInfo root, string pattern)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder
            try
            {
                files = root.GetFiles(pattern);
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.
                //log.Add(e.Message);
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    // In this example, we only access the existing FileInfo object. If we
                    // want to open, delete or modify the file, then
                    // a try-catch block is required here to handle the case
                    // where the file has been deleted since the call to TraverseTree().
                    if (ValidateDateLastModified(fi.LastWriteTime))
                    {
                        AnalyzeMXD(fi.FullName);
                    }
                }

                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    WalkDirectoryTree(dirInfo, pattern);
                }
            }
        }

        private static bool ValidateDateLastModified(DateTime dateLastModified)
        {
            bool validdate = true;

            if (Arguments["mindate"] != null & Arguments["maxdate"] != null)
            {
                DateTime mindate = Convert.ToDateTime(Arguments["mindate"]);
                DateTime maxdate = Convert.ToDateTime(Arguments["maxdate"]);

                if (!(dateLastModified >= mindate & dateLastModified <= maxdate))
                {
                    validdate = false;
                }
            }
            else if (Arguments["mindate"] == null & Arguments["maxdate"] != null)
            {
                DateTime maxdate = Convert.ToDateTime(Arguments["maxdate"]);
                if (dateLastModified >= maxdate)
                {
                    validdate = false;
                }
            }
            else if (Arguments["mindate"] != null & Arguments["maxdate"] == null)
            {
                DateTime mindate = Convert.ToDateTime(Arguments["mindate"]);
                if (dateLastModified <= mindate)
                {
                    validdate = false;
                }
            }


            return validdate;
        }

        /// <summary>
        /// Writes the usage to the console
        /// </summary>
        /// <returns>string containing the usage text to be displayed</returns>
        private static string Usage()
        {
            try
            {
                System.Reflection.Assembly assemblyID = System.Reflection.Assembly.GetExecutingAssembly();
                System.IO.Stream streamID = assemblyID.GetManifestResourceStream("Umbriel.ArcGIS.Layer.ConnectionReport.usg.txt");
                System.IO.StreamReader sr = new System.IO.StreamReader(streamID);

                string usage = sr.ReadToEnd();
                Console.WriteLine(usage);

                // tack on the version info:
                string tail;
                Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string ver = v.Major.ToString() + "." + v.Minor.ToString() + "." + v.Revision.ToString() + "." + v.Build.ToString();

                tail = assemblyID.ManifestModule.Name + ' ' + ver;

                Console.WriteLine(tail);

                return usage;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error displaying usage text: ");
                Console.WriteLine(ex.StackTrace);
                Trace.WriteLine(ex.StackTrace);
                return string.Empty;
            }
        }
    }
}
