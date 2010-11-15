// <copyright file="Program.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-11-10</date>
// <summary>CopyDomain program class file</summary>

namespace CopyDomain
{
    using System;
    using System.Diagnostics;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Geodatabase;
    using DomainList = System.Collections.Generic.List<ESRI.ArcGIS.Geodatabase.IDomain>;

    /// <summary>
    /// CopyDomain Program Class 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// ESRI LicenseInitializer
        /// </summary>
        private static LicenseInitializer esriLicenseInitializer = new CopyDomain.LicenseInitializer();

        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args">command line args.</param>
        [STAThread()]
        public static void Main(string[] args)
        {
            // display the usage when
            if (System.Environment.CommandLine.IndexOf("-h", 0, System.StringComparison.CurrentCultureIgnoreCase) >= 0 |
                System.Environment.CommandLine.IndexOf("--help", 0, System.StringComparison.CurrentCultureIgnoreCase) >= 0 |
                System.Environment.CommandLine.IndexOf("/?", 0, System.StringComparison.CurrentCultureIgnoreCase) >= 0 |
                System.Environment.CommandLine.IndexOf("-help", 0, System.StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                !args.Length.Equals(3))
            {
                Usage();
                return;
            }

            // ESRI License Initializer generated code.
            esriLicenseInitializer.InitializeApplication(
                new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeArcEditor },
            new esriLicenseExtensionCode[] { });

            DomainList domains = new DomainList();

            IWorkspace originalWorkspace = args[0].ToWorkspace();
            IWorkspace targetWorkspace = args[1].ToWorkspace();

            IWorkspaceDomains2 originalWorkspaceDomains = originalWorkspace as IWorkspaceDomains2;
            IWorkspaceDomains2 targetWorkspaceDomains = targetWorkspace as IWorkspaceDomains2;

            string domain = args[2];

            if (domain.Trim().Equals("*"))
            {
                domains = originalWorkspaceDomains.Domains.ToDomainList();
            }
            else if (domain.IndexOf(',') > 0)
            {
                string[] tokens = domain.Split(',');

                foreach (string item in tokens)
                {
                    try
                    {
                        domains.Add(originalWorkspaceDomains.get_DomainByName(item));
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.StackTrace);
                    }
                }
            }
            else
            {
                // assume the domain is a single domain
                    try
                    {
                        domains.Add(originalWorkspaceDomains.get_DomainByName(domain));
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.StackTrace);
                    }
            }
            
            foreach (IDomain d in domains)
            {                
                Console.Write(Constants.CopyStartMessage.FormatString(d.Name));

                try
                {
                    IClone clone = d as IClone;
                    IDomain newdomain = clone.Clone() as IDomain;
                    targetWorkspaceDomains.AddDomain(newdomain);
                    Console.WriteLine("success!\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine("failed.");
                    Console.WriteLine(Constants.GeneralErrorMessage.FormatString(e.Message));
                    Console.WriteLine();
                    System.Diagnostics.Trace.WriteLine(e.StackTrace);
                }
            }

            // Do not make any call to ArcObjects after ShutDownApplication()
            esriLicenseInitializer.ShutdownApplication();
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
                System.IO.Stream streamID = assemblyID.GetManifestResourceStream("CopyDomain.usg.txt");
                System.IO.StreamReader sr = new System.IO.StreamReader(streamID);

                string usage = sr.ReadToEnd();
                Console.WriteLine(usage);

                // tack on the version info:
                Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                string ver = "CopyDomain.exe {0}.{1}";

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
