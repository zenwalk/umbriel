
namespace Umbriel.ArcGIS.PurgeDomainValues
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text;

    using ArcZona.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Catalog;
    using ESRI.ArcGIS.DataSourcesGDB;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Geodatabase;
    using Umbriel.ArcGIS;

    class Program
    {
        /// <summary>
        /// Gets or sets the Domain Name.
        /// </summary>
        /// <value>The Domain Name</value>
        internal static string DomainName { get; set; }

        /// <summary>
        /// Gets or sets the ESRI license product code.
        /// </summary>
        /// <value>The ESRI license product code.</value>
        internal static esriLicenseProductCode ESRILicenseProductCode { get; set; }

        /// <summary>
        /// Gets or sets the arg parser.
        /// </summary>
        /// <value>The arg parser.</value>
        internal static CommandLine.Utility.CommandArguments ArgParser { get; set; }

        /// <summary>
        /// Gets or sets the ESRI license initializer.
        /// </summary>
        /// <value>The ESRI license initializer.</value>
        internal static GeoprocessingInDotNet.LicenseInitializer ESRILicenseInitializer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="sdeMetaProgram"/> is verbose.
        /// </summary>
        /// <value><c>true</c> if verbose; otherwise, <c>false</c>.</value>
        internal static bool Verbose { get; set; }

        [STAThread()]
        static void Main(string[] args)
        {
            // s = ArcZona.AZUtil.ParseGeoDBName(@"sde:sqlserver:DPU-PBU-GIS77\sqlexpress");
            // s = ArcZona.AZUtil.ParseGeoDBName("sde:oracle10g:/:dpu_admin");
            // s = ArcZona.AZUtil.ParseGeoDBName("5151:dpu_admin");

            /* Sample Command Line Params:
             *  -n  ssDomainOwner -v -g "\OWNEDBY_Example.gdb" 
             *  
             * */
            try
            {
                ESRILicenseProductCode = esriLicenseProductCode.esriLicenseProductCodeArcView;
                ArgParser = new CommandLine.Utility.CommandArguments(System.Environment.GetCommandLineArgs());

                // display the usage when
                if (System.Environment.CommandLine.IndexOf("-h", 0, System.StringComparison.CurrentCultureIgnoreCase) >= 0 |
                    System.Environment.CommandLine.IndexOf("--help", 0, System.StringComparison.CurrentCultureIgnoreCase) >= 0 |
                    System.Environment.CommandLine.IndexOf("/?", 0, System.StringComparison.CurrentCultureIgnoreCase) >= 0 |
                    System.Environment.CommandLine.IndexOf("-help", 0, System.StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    Usage();
                    return;
                }

                if (ValidateArgs() == true)
                {
                    ESRILicenseInitializer = new GeoprocessingInDotNet.LicenseInitializer();

                    ConsoleWriteLine("Arguments Validated!!");
                    ConsoleWriteLine("Initializing ArcObjects License...");

                    ESRILicenseInitializer.InitializeApplication(ESRILicenseProductCode);

                    if (ESRILicenseInitializer.InitializedProduct > 0)
                    {
                        ConsoleWriteLine("License Initialized.");

                        IWorkspace ws = UmbrielArcGISHelper.GetWorkspace(ArgParser);
                        IWorkspaceDomains2 workspaceDomains = (IWorkspaceDomains2)ws;


                        IDomain domain = workspaceDomains.get_DomainByName(DomainName);
                        ICodedValueDomain codedDomain = (ICodedValueDomain)domain;


                        Stack<string> domainValues = new Stack<string>();

                        for (int i = codedDomain.CodeCount -1; i >=0  ; i--)
                        {
                            codedDomain.DeleteCode(codedDomain.get_Value(i));
                        }

                        workspaceDomains.AlterDomain(domain);

                        




                    }
                    else
                    {
                        Console.WriteLine("\a");
                        Console.WriteLine("Could not initialize ESRI License.");
                    }

                    // Do not make any call to ArcObjects after ShutDownApplication()
                    ConsoleWriteLine("Releasing ArcObjects License...");
                    ESRILicenseInitializer.ShutdownApplication();
                    ConsoleWriteLine("ArcObjects License Released.");
                }
                else
                {
                    Usage();
                    return;
                }
            }
            catch (Exception ex)
            {
                // beep!
                Console.WriteLine("\a");
                Console.WriteLine("An error has occurred: \n\n");
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                // try to shut down the ESRI license
                if (ESRILicenseInitializer != null)
                {
                    if (ESRILicenseInitializer.InitializedProduct > 0)
                    {
                        ESRILicenseInitializer.ShutdownApplication();
                    }
                }
            }



            /* SDEMeta sleeps for 2000 milliseconds by default,
             * so that when called from a batch, the license manager has a chance to return 
             * the license to the pool before the next SDEMeta attempts to check it out.
            */
            if (!ArgParser.Contains("-nosleep"))
            {
                string sleep = ArgParser.GetValue("sleep");

                try
                {
                    int sleepDuration = 2000;
                    if (!String.IsNullOrEmpty(sleep))
                    {
                        sleepDuration = Convert.ToInt32(sleep);
                    }

                    System.Threading.Thread.Sleep(sleepDuration);
                }
                catch
                {
                }
            }
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
                System.IO.Stream streamID = assemblyID.GetManifestResourceStream("Umbriel.ArcGIS.PurgeDomainValues.usg.txt");

                System.IO.StreamReader sr = new System.IO.StreamReader(streamID);

                string usage = sr.ReadToEnd();
                Console.WriteLine(usage);

                // tack on the version info:
                string tail;
                Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string ver = v.Major.ToString() + "." + v.Minor.ToString() + "." + v.Revision.ToString() + "." + v.Build.ToString();

                tail = "PurgeDomainValues.exe " + ver;

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

        /// <summary>
        /// Validate Command Line Arguments
        /// </summary>
        /// <returns>true if arguments are valid</returns>
        private static bool ValidateArgs()
        {
            bool valid = true;

            // get the esri product code: 
            try
            {
                int productCode = 0;

                if (ArgParser.Contains("-a"))
                {
                    string licenseProductCode = ArgParser.GetValue("-a");
                    productCode = Convert.ToInt32(licenseProductCode);
                    ESRILicenseProductCode = (esriLicenseProductCode)productCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                valid = false;
            }

            if (ArgParser.Contains("-n"))
            {
                DomainName = ArgParser.GetValue("-n");
            }
            else
            {
                valid = false;
            }

            // verbose argument:
            try
            {
                Verbose = ArgParser.Contains("-v");
            }
            catch
            {
                Verbose = false;
            }

            return valid;
        }

        /// <summary>
        /// Writes line to Console after checking for verbose argument:
        /// </summary>
        /// <param name="lineText">The text that is to be written to console</param>
        private static void ConsoleWriteLine(string lineText)
        {
            // if no file is specified, we don't stream out any data to console...only the export file contents:
            if (Verbose == true)
            {
                Console.WriteLine(lineText);
            }
        }


    }
}
