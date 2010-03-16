using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Xml.Linq;

namespace Umbriel.GIS.Metadata.GISMetaLinq
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        //-o purgeanalyzesteps -f "\\w-dpu-48\dpu_sys\GIS\metadata\dpugis\xml\WATER_FITTINGS.XML" -out "\\w-dpu-48\dpu_sys\GIS\metadata\dpugis\xml\WATER_FITTINGS_OUTTEST.XML"

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

            if (ValidateArgs())
            {
                string infile = GetSwitchValue("-f");
                string outfile = GetSwitchValue("-out");

                string operation = GetSwitchValue("-o");

                switch (operation)
                {
                    case "purgeanalyzesteps":
                        PurgeAnalyzeStepsFromMetadata(infile, outfile);
                        break;
                    default:
                        Trace.WriteLine("No case for: " + operation);
                        break;
                }


            }
        }

        /// <summary>
        /// Purges the analyze steps from metadata.
        /// </summary>
        /// <param name="infile">The infile path</param>
        /// <param name="outfile">The outfile path</param>
        private static void PurgeAnalyzeStepsFromMetadata(string infile, string outfile)
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            XDocument doc = XDocument.Load(infile);
            XElement root = doc.Root;

            var d =
                (from el in root.Descendants("Process")
                 select el);

            List<XElement> deletenodes = new List<XElement>();

            
            foreach (XElement node in d)
            {
                foreach (XAttribute attribute in node.Attributes())
                {
                    if (attribute.Value.ToString().StartsWith("Analyze"))
                    {
                        deletenodes.Add(node);
                    }
                }
            }

            int removecount = 0;

            foreach (XElement node in deletenodes)
            {
                try
                {
                    // remove the node:
                    node.Remove();
                    Console.WriteLine("Delete Node: " + node.ToString());
                    Console.WriteLine();
                    removecount++;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Delete Unsuccessful: " + ex.Message);
                }                
            }

            doc.Save(outfile);

            stopwatch.Stop();

            Console.WriteLine("Delete Attempts: " + deletenodes.Count.ToString() + " Actual Deletes: " + removecount.ToString());
            Console.WriteLine("Purge Analyze Steps Duration (seconds): " + ((double)stopwatch.ElapsedMilliseconds /1000).ToString("0.0000"));

        }

        /// <summary>
        /// Validates the args.
        /// </summary>
        /// <returns></returns>
        private static bool ValidateArgs()
        {
            bool valid = true;

            //Dictionary<int, string> arguments = new Dictionary<int, string>(args);

            List<string> arguments = new List<string>(System.Environment.GetCommandLineArgs());

            try
            {
                if (!arguments.Contains("-f")
                    | !arguments.Contains("-out"))
                {
                    valid = false;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return valid;
            //// get the esri product code: 
            //try
            //{
            //    int productCode = 0;

            //    if (ArgParser.Contains("-a"))
            //    {
            //        string licenseProductCode = ArgParser.GetValue("-a");
            //        productCode = Convert.ToInt32(licenseProductCode);
            //        ESRILicenseProductCode = (esriLicenseProductCode)productCode;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    valid = false;
            //}

            // get the operation code:
        }

        private static string GetSwitchValue(string sw)
        {
            try
            {
                List<string> arguments = new List<string>(System.Environment.GetCommandLineArgs());

                int i = arguments.IndexOf(sw);

                string value = arguments[i + 1];

                return value;
            }
            catch (Exception)
            {
                
                throw;
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
                string[] resNames = assemblyID.GetManifestResourceNames();
                System.IO.Stream streamID = assemblyID.GetManifestResourceStream("Umbriel.GIS.Metadata.GISMetaLinq.usg.txt");
                System.IO.StreamReader sr = new System.IO.StreamReader(streamID);

                string usage = sr.ReadToEnd();
                Console.WriteLine(usage);

                // tack on the version info:
                string tail;
                Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string ver = v.Major.ToString() + "." + v.Minor.ToString() + "." + v.Revision.ToString() + "." + v.Build.ToString();

                tail = "GISMetaLinq.exe " + ver;

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
