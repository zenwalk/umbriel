// <copyright file="S2SWktOperation.cs" company="Umbriel Project">
// Copyright (c) 2010 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-03-14</date>
// <summary>S2SWktOperation class file </summary>

namespace Umbriel.ArcGIS.Geodatabase.sde2spatialite
{
    using System;
    using System.Diagnostics;
    using CommandLine.Utility;
    using ESRI.ArcGIS.Geodatabase;
    using Umbriel.ArcGIS.Geodatabase;

    /// <summary>
    /// WKT Operation Class (inhereits ISDEOperation)
    /// </summary>
    internal class S2SWktOperation : ISDEOperation
    {
        #region Fields
        /// <summary>
        /// Command line arg class
        /// </summary>
        private Arguments operationArguments;

        /// <summary>
        /// Trace Switch for this operation class
        /// </summary>
        private TraceSwitch operationTraceSwitch = new TraceSwitch("S2SInitOperationTraceSwitch", "Trace switch for the Init Operation");
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="S2SWktOperation"/> class.
        /// </summary>
        /// <param name="args">The CommandLine.Utility.Arguments object containing all of the command line args</param>
        public S2SWktOperation(Arguments args)
        {
            this.operationArguments = args;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the sde workspace.
        /// </summary>
        /// <value>The sde workspace</value>
        private IWorkspace Workspace { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Validates the arguments for the WKT Operation
        /// </summary>
        /// <returns>true if valid</returns>
        private bool ValidateArguments()
        {
            bool valid = true;

            //// TODO: write validation code (S2SWktOperation)

            return valid;
        }

        /// <summary>
        /// Writes text to the console
        /// </summary>
        /// <param name="message">The message.</param>
        private void NewDataHandler(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Prints info  messages based on OperationTraceSwitch
        /// </summary>
        /// <param name="value">The message to print.</param>
        private void PrintInformation(string value)
        {
            Trace.WriteLineIf(this.operationTraceSwitch.TraceInfo, value);
        }

        /// <summary>
        /// Prints verbose messages based on OperationTraceSwitch
        /// </summary>
        /// <param name="value">The message to print.</param>
        private void PrintMessage(string value)
        {
            Trace.WriteLineIf(this.operationTraceSwitch.TraceVerbose, value);
        }        
        #endregion           

        #region ISDEOperation Members

        /// <summary>
        /// Gets or sets the command line arguments.
        /// </summary>
        /// <value>The command line arguments.</value>
        Arguments ISDEOperation.CommandLineArguments
        {
            get
            {
                return this.operationArguments;
            }

            set
            {
                this.operationArguments = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [valid arguments].
        /// </summary>
        /// <value><c>true</c> if [valid arguments]; otherwise, <c>false</c>.</value>
        bool ISDEOperation.ValidArguments
        {
            get
            {
                return this.ValidateArguments();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        void ISDEOperation.Execute()
        {
            Console.WriteLine("Execute Method");

            try
            {
                this.Workspace = Util.S2SHelper.Workspace(ref this.operationArguments);

                if (this.Workspace != null)
                {
                    FeatureclassWKTWriter wktWriter = new FeatureclassWKTWriter();

                    wktWriter.NewDataLineEvent += new FeatureclassWKTWriter.NewDataLineDelegate(this.NewDataHandler);

                    string columns = this.operationArguments["c"] ?? string.Empty;
                    
                    if (!string.IsNullOrEmpty(columns))
                    {
                        wktWriter.ColumnNames = columns;
                    }

                    // get the row limit value
                    string rowlimit = this.operationArguments["limit"];
                    if (rowlimit != null)
                    {
                        int r = 0;
                        if (int.TryParse(rowlimit, out r))
                        {
                            wktWriter.Limit = r;
                        }
                    }

                    wktWriter.ReadAll(this.Workspace, Util.S2SHelper.GetSDETable(ref this.operationArguments));
                    
                    wktWriter.NewDataLineEvent -= this.NewDataHandler;
                }
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