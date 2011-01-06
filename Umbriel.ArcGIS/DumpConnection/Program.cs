// <copyright file="Program.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-12-20</date>
// <summary>DumpConnection program class file</summary>


namespace DumpConnection
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Text;
    using System.Diagnostics;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.Framework;
    using ESRI.ArcGIS.DataSourcesGDB;
    using ESRI.ArcGIS.Geodatabase;
    using ConnectionStringList = System.Collections.Generic.List<string>;
    using StringList = System.Collections.Generic.List<string>;
    using FileList = System.Collections.Generic.List<System.IO.FileInfo>;

    /// <summary>
    /// 
    /// </summary>
    class Program
    {


        private static LicenseInitializer m_AOLicenseInitializer = new DumpConnection.LicenseInitializer();

        [STAThread()]
        static void Main(string[] args)
        {
            // display the usage when
            if (System.Environment.CommandLine.IndexOf("-h", 0, System.StringComparison.CurrentCultureIgnoreCase) >= 0 |
                System.Environment.CommandLine.IndexOf("--help", 0, System.StringComparison.CurrentCultureIgnoreCase) >= 0 |
                System.Environment.CommandLine.IndexOf("/?", 0, System.StringComparison.CurrentCultureIgnoreCase) >= 0 |
                System.Environment.CommandLine.IndexOf("-help", 0, System.StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                args.Length < 1)
            {
                Usage();
                return;
            }

            StringList argList = new StringList(args);

            int i = argList.IndexOf("-f");

            FileConnections.Recurse = argList.Contains("-R");

            FileConnections.SearchPath = (argList[i + 1]).Trim('"');

            FileList filesToSearch = new FileList();

            if (File.Exists(FileConnections.SearchPath))
            {
                filesToSearch.Add(new FileInfo(FileConnections.SearchPath));
            }
            else
            {
                Trace.WriteLine("FileConnections.SearchPath={0}".FormatString(FileConnections.SearchPath));
                if (Directory.Exists(FileConnections.SearchPath))
                {
                    DirectoryInfo directory = new DirectoryInfo(FileConnections.SearchPath);

                    filesToSearch.AddRange(
                        directory.GetFiles("*.mxd", (FileConnections.Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)));

                    filesToSearch.AddRange(
                        directory.GetFiles("*.lyr", (FileConnections.Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)));
                }
                else
                {
                    DirectoryInfo directory = new DirectoryInfo(System.IO.Path.GetDirectoryName(FileConnections.SearchPath));

                    string filesearchPattern = FileConnections.WildcardExtensionSearch.FormatString(System.IO.Path.GetExtension(FileConnections.SearchPath));

                    filesToSearch.AddRange(
                        directory.GetFiles(
                            filesearchPattern, (FileConnections.Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)));
                }
            }


            if (argList.Contains("-v"))
            {
                Console.WriteLine("Files to be read: ");
                Console.WriteLine("-----------------------------------");
                foreach (FileInfo fi in filesToSearch)
                {
                    Console.WriteLine(fi.FullName);
                }
                Console.WriteLine("Total: {0} -----------------------------------".FormatString(filesToSearch.Count));
            }



            //ESRI License Initializer generated code.
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeArcView },
            new esriLicenseExtensionCode[] { });


            ConnectionStringList list = new ConnectionStringList();

            foreach (FileInfo file in filesToSearch)
            {
                try
                {
                    list.AddRange(FileConnections.ReadFile(file.FullName));
                }
                catch (Exception e)
                {
                    if (argList.Contains("-v"))
                    {
                        Trace.WriteLine(e.StackTrace);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            Console.WriteLine(string.Join("\n", list.ToArray()));

            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()
            m_AOLicenseInitializer.ShutdownApplication();
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
                string[] resNames = assemblyID.GetManifestResourceNames();
                System.IO.Stream streamID = assemblyID.GetManifestResourceStream("DumpConnection.usg.txt");
                System.IO.StreamReader sr = new System.IO.StreamReader(streamID);

                string usage = sr.ReadToEnd();
                Console.WriteLine(usage);

                // tack on the version info:
                Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                string ver = "DumpConnection.exe {0}.{1}";

                Console.WriteLine(ver.FormatString(v.Major.ToString(), v.Minor.ToString()));

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

