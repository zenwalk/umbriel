using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using DomainList = System.Collections.Generic.List<ESRI.ArcGIS.Geodatabase.IDomain>;


namespace CopyDomain
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new CopyDomain.LicenseInitializer();

        [STAThread()]
        static void Main(string[] args)
        {

            // display the usage when
            if (System.Environment.CommandLine.IndexOf("-h", 0, System.StringComparison.CurrentCultureIgnoreCase) >= 0 |
                System.Environment.CommandLine.IndexOf("--help", 0, System.StringComparison.CurrentCultureIgnoreCase) >= 0 |
                System.Environment.CommandLine.IndexOf("/?", 0, System.StringComparison.CurrentCultureIgnoreCase) >= 0 |
                System.Environment.CommandLine.IndexOf("-help", 0, System.StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                Usage();
                return;
            }

            //ESRI License Initializer generated code.
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeArcEditor },
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
                //assume the domain is a single domain
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
                System.IO.Stream streamID = assemblyID.GetManifestResourceStream("CopyDomain.usg.txt");
                System.IO.StreamReader sr = new System.IO.StreamReader(streamID);

                string usage = sr.ReadToEnd();
                Console.WriteLine(usage);

                // tack on the version info:
                string tail;
                Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string ver = v.Major.ToString() + "." + v.Minor.ToString() + "." + v.Revision.ToString() + "." + v.Build.ToString();

                tail = "CopyDomain.exe " + ver;

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
