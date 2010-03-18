// <copyright file="Program.cs" company="Umbriel Project">
// Copyright (c) 2010 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-03-14</date>
// <summary>Program class file </summary>

namespace Umbriel.ArcGIS.Geodatabase.sde2spatialite
{
    using System;
    using System.Diagnostics;
    using ESRI.ArcGIS.esriSystem;

    /// <summary>
    /// The Main program class for the sde2spatialite command-line exe
    /// </summary>
    public class Program
    {
        #region Fields
        /// <summary>
        /// OperationTraceSwitch field
        /// </summary>
        private static TraceSwitch operationTraceSwitch = new TraceSwitch("S2SInitOperationTraceSwitch", "Trace switch for the Init Operation");

        /// <summary>
        /// ESRI LicenseInitializer 
        /// </summary>
        private static LicenseInitializer aolicenseInitializer = new LicenseInitializer();
        #endregion

        #region Constructors
        
        #endregion

        #region Properties
        /// <summary>
        /// Gets the operation trace switch.
        /// </summary>
        /// <value>The operation trace switch.</value>
        public static TraceSwitch OperationTraceSwitch
        {
            get
            {
                return operationTraceSwitch;
            }

            private set
            {
                operationTraceSwitch = value;
            }
        }

        /// <summary>
        /// Gets the CommandLine.Utility.Arguments.
        /// </summary>
        /// <value>The CommandLine.Utility.Arguments.</value>
        public static CommandLine.Utility.Arguments Arguments { get; private set; }

        /// <summary>
        /// Gets or sets the SDE operation
        /// </summary>
        /// <value>The SDE operation.</value>
        private static ISDEOperation Operation { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// The startup method
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        [STAThread()]
        public static void Main(string[] args)
        {
            System.Diagnostics.Debug.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(Console.Out, "sde2spatialiteconsolewriter"));

            Arguments = new CommandLine.Utility.Arguments(args);

            // show help/usage
            if (Arguments["h"] != null || Arguments["help"] != null || args.Length.Equals(0))
            {
                Console.Write(Util.S2SHelper.HelpManual());
                return;
            }

            if (Validate())
            {
                // ESRI License Initializer generated code.
                aolicenseInitializer.InitializeApplication(
                    new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeArcView },
                    new esriLicenseExtensionCode[] { });

                Operation.Execute();

                // ESRI License Initializer generated code.
                // Do not make any call to ArcObjects after ShutDownApplication()
                aolicenseInitializer.ShutdownApplication();
            }

            System.Diagnostics.Debug.Listeners.Remove("sde2spatialiteconsolewriter");
        }

        /// <summary>
        /// Validates the arguments for the program; also sets the Operation interface to an Operation class
        /// </summary>
        /// <returns>bool true if valid</returns>
        private static bool Validate()
        {
            bool valid = true;

            if (Arguments["o"] == null)
            {
                valid = false;
                Console.WriteLine("No operation argument specified.  Command Usage:  sde2spatialite.exe -h   ");
            }
            else
            {
                string operationArg = Arguments["o"].ToLower();

                switch (operationArg)
                {
                    case "wkt":
                        Operation = new S2SWktOperation(Arguments);
                        break;
                    case "init":
                        Operation = new S2SInitOperation(Arguments);
                        break;
                    default:
                        Console.WriteLine(string.Format(Constants.InvalidOpArgMessage, operationArg));
                        valid = false;
                        break;
                }
            }

            if (Operation != null && Operation.ValidArguments.Equals(false))
            {
                valid = false;
            }

            return valid;
        }
        #endregion
   }
}

