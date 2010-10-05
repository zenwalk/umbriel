// <copyright file="S2SInitOperation.cs" company="Umbriel Project">
// Copyright (c) 2010 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-03-16</date>
// <summary>S2SInitOperation class file </summary>

namespace Umbriel.ArcGIS.Geodatabase.sde2spatialite
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using System.Diagnostics;
    using System.Text;
    using CommandLine.Utility;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;

    /// <summary>
    /// S2SInitOperation operation class (implements ISDEOperation
    /// </summary>
    internal class S2SInitOperation : ISDEOperation
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

        #region Events

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="S2SInitOperation"/> class.
        /// </summary>
        /// <param name="args">The CommandLine.Utility.Arguments object containing all of the command line args.</param>
        public S2SInitOperation(Arguments args)
        {
            this.operationArguments = args;
            this.SetSRID();
            this.SetVerbosity();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="S2SInitOperation"/> class.
        /// </summary>
        /// <param name="args">The CommandLine.Utility.Arguments object containing all of the command line args</param>
        /// <param name="traceswitch">The traceswitch for the class</param>
        public S2SInitOperation(Arguments args, TraceSwitch traceswitch)
        {
            this.operationArguments = args;
            this.SetSRID();
            this.operationTraceSwitch = traceswitch;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the verbosity (tracel leve)
        /// </summary>
        /// <value>The trace level</value>
        public TraceLevel Verbosity { get; set; }

        /// <summary>
        /// Gets or sets the sde workspace.
        /// </summary>
        /// <value>The sde workspace</value>
        private IWorkspace Workspace { get; set; }

        /// <summary>
        /// Gets or sets the spatial reference ID.
        /// </summary>
        /// <value>The spatial reference ID.</value>
        private int SpatialReferenceID { get; set; }

        /// <summary>
        /// Gets or sets the SQ lite DB connection string.
        /// </summary>
        /// <value>The SQ lite DB connection string.</value>
        private string SQLiteDBConnectionString { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the parameterized insert SQL.
        /// </summary>
        /// <param name="fieldmappings">The fieldmapping List</param>
        /// <param name="tablename">The spatialite table name.</param>
        /// <param name="command">The sql command object</param>
        /// <returns>The parameterized insert sql string</returns>
        private string CreateParameterizedInsertSQL(List<FieldMap> fieldmappings, string tablename, SQLiteCommand command)
        {
            StringBuilder sqlFields = new StringBuilder();
            StringBuilder sqlValues = new StringBuilder();

            SQLiteParameterCollection parameters = command.Parameters;

            for (int i = 0; i < fieldmappings.Count; i++)
            {
                FieldMap fieldmap = fieldmappings[i];
                if (!i.Equals(0))
                {
                    sqlFields.Append(" , ");
                    sqlValues.Append(" , ");
                }

                sqlFields.Append(fieldmap.SpatialiteFieldname);
                sqlValues.Append(@" ? ");
            }

            // add wkt
            sqlFields.Append(" , ");
            sqlValues.Append(" , ");
            sqlFields.Append("WKT");
            sqlValues.Append(@" ? ");

            StringBuilder sqlInsert = new StringBuilder();
            sqlInsert.Append("INSERT INTO ");
            sqlInsert.Append(tablename);
            sqlInsert.Append("(");
            sqlInsert.Append(sqlFields.ToString());
            sqlInsert.Append(")");
            sqlInsert.Append(" VALUES ");
            sqlInsert.Append("(");
            sqlInsert.Append(sqlValues.ToString());
            sqlInsert.Append(")");

            Trace.WriteLineIf(Program.OperationTraceSwitch.TraceVerbose, sqlInsert);

            return sqlInsert.ToString();
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

        /// <summary>
        /// Sets the SRID value from the args
        /// </summary>
        private void SetSRID()
        {
            if (this.operationArguments != null && this.operationArguments.Contains("srid"))
            {
                this.SpatialReferenceID = -1;

                int srid = -1;

                if (int.TryParse(this.operationArguments["srid"], out srid))
                {
                    this.SpatialReferenceID = srid;
                }
            }

            this.PrintInformation("Using Spatial Reference ID: " + this.SpatialReferenceID.ToString() + ".");
        }

        /// <summary>
        /// Determins if the tables the exists in the spatialite db
        /// </summary>
        /// <param name="theDatabase">The database connection</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>true if the table exists; false if it doesn't</returns>
        private bool TableExist(SQLiteConnection theDatabase, string tableName)
        {
            SQLiteCommand cmd = new SQLiteCommand(theDatabase);
            cmd.CommandText = "SELECT name FROM sqlite_master WHERE name='" + tableName + "'";
            SQLiteDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Validates the arguments.
        /// </summary>
        /// <returns>true if valid</returns>
        private bool ValidateArguments()
        {
            bool valid = true;

            //// TODO: write validation code.

            // Validate SQLite arguments:
            // get the sqlite path argument.
            string sqlitedbpath = this.operationArguments["sqlite"] ?? string.Empty;

            if (!System.IO.File.Exists(sqlitedbpath))
            {
                Util.S2SHelper.InitializeNewSpatialiteDB(sqlitedbpath);
            }

            if (!string.IsNullOrEmpty(sqlitedbpath))
            {
                // create the connection string using the basic sqlite connection string
                this.SQLiteDBConnectionString = string.Format(Constants.SQLiteBasicConnectionString, sqlitedbpath);
            }
            else
            {
                // alternatively, obtain the entire connection string from the command line arg:
                this.SQLiteDBConnectionString = this.operationArguments["sqliteconn"] ?? string.Empty;
            }

            if (string.IsNullOrEmpty(this.SQLiteDBConnectionString))
            {
                valid = false;
            }

            return valid;
        }

        /// <summary>
        /// Opens the feature cursor.
        /// </summary>
        /// <param name="workspace">The Iworkspace.</param>
        /// <param name="whereclause">The where clause.</param>
        /// <param name="fc">The Ifeatureclass reference.</param>
        /// <param name="cursor">The IFeatureCursor.</param>
        /// <param name="featurecount">total number of features </param>
        /// <param name="spatialreferenceID">The spatialreference ID.</param>
        private void OpenFeatureCursor(IWorkspace workspace, string whereclause, out IFeatureClass fc, out IFeatureCursor cursor, out int featurecount, out int spatialreferenceID)
        {
            string geodatabaseTableName = Util.S2SHelper.GetSDETable(ref this.operationArguments);

            IFeatureWorkspace wrkspc = (IFeatureWorkspace)workspace;

            if (!string.IsNullOrEmpty(this.operationArguments["D"]))
            {
                geodatabaseTableName = this.operationArguments["D"] + "." + geodatabaseTableName;
            }

            fc = wrkspc.OpenFeatureClass(geodatabaseTableName);
            IDataset ds = (IDataset)fc;

            IGeoDataset geoDS = (IGeoDataset)ds;
            ISpatialReference spatialReference = geoDS.SpatialReference;

            spatialreferenceID = spatialReference.FactoryCode;

            if (!string.IsNullOrEmpty(whereclause))
            {
                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = whereclause;
                featurecount = fc.FeatureCount(filter);

                cursor = fc.Search(filter, false);
            }
            else
            {
                featurecount = fc.FeatureCount(null);
                cursor = fc.Search(null, false);
            }

            return;
        }

        /// <summary>
        /// Sets the verbose property based on the operation arguments.
        /// The verbose property is public and can be changed later.
        /// </summary>
        private void SetVerbosity()
        {
            // TODO: maybe look at moving verbosity handling to the program class
            if (this.operationArguments != null && this.operationArguments.Contains("v"))
            {
                string value = this.operationArguments["v"].ToLower() ?? "info";
                switch (value)
                {
                    case "off":
                        this.operationTraceSwitch.Level = TraceLevel.Off;
                        break;
                    case "error":
                        this.operationTraceSwitch.Level = TraceLevel.Error;
                        break;
                    case "warning":
                        this.operationTraceSwitch.Level = TraceLevel.Warning;
                        break;
                    case "info":
                        this.operationTraceSwitch.Level = TraceLevel.Info;
                        break;
                    case "verbose":
                    default:
                        this.operationTraceSwitch.Level = TraceLevel.Verbose;
                        break;
                }
            }
            else
            {
                this.operationTraceSwitch.Level = TraceLevel.Off;
            }
        }
        #endregion

        #region ISDEOperation Members

        /// <summary>
        /// Gets or sets the command line arguments.
        /// </summary>
        /// <value>The command line arguments.</value>
        CommandLine.Utility.Arguments ISDEOperation.CommandLineArguments
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
        /// Executes the Op
        /// </summary>
        void ISDEOperation.Execute()
        {
            int totalfeaturecount;
            bool newtable = false;

            try
            {
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

                stopwatch.Start();

                this.Workspace = Util.S2SHelper.Workspace(ref this.operationArguments);

                using (SQLiteConnection connection = new SQLiteConnection(this.SQLiteDBConnectionString))
                {
                    connection.Open();
                    this.PrintMessage("Spatialite Database Opened.");

                    IFeatureCursor cursor;
                    IFeatureClass fc;
                    int spatialreferenceID;
                    List<FieldMap> fieldmappings = null;
                    string whereclause = this.operationArguments["w"] ?? string.Empty;

                    this.OpenFeatureCursor(this.Workspace, whereclause, out fc, out cursor, out totalfeaturecount, out spatialreferenceID);

                    if (cursor != null)
                    {
                        this.PrintInformation(string.Format(Constants.InitOpFeatureClassOpenedMessage, totalfeaturecount, ((IDataset)fc).Name));

                        string tablename = Utility.ParseObjectClassName(((IDataset)fc).Name);
                        string geometryfieldname = Constants.DefaultGeometryFieldName;
                        string geometrytype = Util.S2SHelper.GetGeometryType(ref fc);
                        string spatialiteTableName = this.operationArguments["T"] ?? tablename;

                        this.PrintMessage("SDE Table Name = " + tablename);
                        this.PrintMessage("Spatialite Table Name = " + spatialiteTableName);
                        this.PrintMessage("Spatialite Geometry Field Name = " + geometryfieldname);
                        this.PrintMessage("Spatialite Geometry Type = " + geometrytype);

                        this.PrintMessage("Featureclass Spatial Reference Factory Code=" + Util.S2SHelper.GetFactoryCode(fc).ToString());

                        // load the spatial extension
                        using (SQLiteCommand extloadCommand = connection.CreateCommand())
                        {
                            // load the spatial extension:
                            extloadCommand.CommandText = @"SELECT load_extension('libspatialite-2.dll');";
                            int recordsAffected = extloadCommand.ExecuteNonQuery();

                            if (!this.TableExist(connection, spatialiteTableName))
                            {
                                this.PrintMessage("Spatialite Table Name = " + spatialiteTableName + " created.");

                                string tname = tablename;
                                
                                if (!string.IsNullOrEmpty(this.operationArguments["D"]))
                                {
                                    tname = this.operationArguments["D"] + "." + tname;
                                }

                                extloadCommand.CommandText = Util.S2SHelper.GenerateCreateTableSQL(this.Workspace, tname, spatialiteTableName, out fieldmappings, geometryfieldname);
                                extloadCommand.ExecuteNonQuery();
                                newtable = true;
                            }
                            else
                            {
                                this.PrintMessage("Spatialite Table Name = " + spatialiteTableName + " already exists.");
                                throw new TableAlreadyExistsException("Table already exists.", tablename);
                            }
                        }

                        using (SQLiteTransaction trans = connection.BeginTransaction())
                        {
                            using (SQLiteCommand insertCommand = connection.CreateCommand())
                            {
                                this.PrintMessage("Obtaining Parameterized Insert SQL Statement");
                                insertCommand.CommandText = this.CreateParameterizedInsertSQL(
                                    fieldmappings,
                                    spatialiteTableName,
                                    insertCommand);

                                this.PrintMessage("Insert SQL Statement: " + insertCommand.CommandText);

                                IFeature feature = null;
                                int featureCount = 0;

                                // add the parameters:
                                for (int f = 0; f < fieldmappings.Count; f++)
                                {
                                    insertCommand.Parameters.Add(new SQLiteParameter(fieldmappings[f].DBFieldType));
                                }

                                // add the wkt parameter
                                insertCommand.Parameters.Add(new SQLiteParameter(DbType.String));

                                this.PrintInformation("Start featureclass read.");
                                while ((feature = cursor.NextFeature()) != null)
                                {
                                    featureCount++;

                                    this.PrintMessage("Reading feature OID=" + feature.OID.ToString() + " (" + featureCount.ToString() + " of " + totalfeaturecount.ToString() + ").");

                                    for (int f = 0; f < fieldmappings.Count; f++)
                                    {
                                        int fieldindex = feature.Fields.FindField(fieldmappings[f].FeatureFieldname);

                                        IField field = feature.Fields.get_Field(fieldindex);

                                        if (feature.get_Value(fieldindex) != System.DBNull.Value && feature.get_Value(fieldindex) != null)
                                        {
                                            if (insertCommand.Parameters[f].DbType.Equals(DbType.String))
                                            {
                                                insertCommand.Parameters[f].Value = feature.get_Value(fieldindex).ToString();
                                            }
                                            else if (insertCommand.Parameters[f].DbType.Equals(DbType.Double))
                                            {
                                                insertCommand.Parameters[f].Value = Convert.ToDouble(feature.get_Value(fieldindex));
                                            }
                                            else if (insertCommand.Parameters[f].DbType.Equals(DbType.Single))
                                            {
                                                insertCommand.Parameters[f].Value = Convert.ToSingle(feature.get_Value(fieldindex));
                                            }
                                            else if (insertCommand.Parameters[f].DbType.Equals(DbType.Int32))
                                            {
                                                insertCommand.Parameters[f].Value = Convert.ToInt32(feature.get_Value(fieldindex));
                                            }
                                            else if (insertCommand.Parameters[f].DbType.Equals(DbType.DateTime))
                                            {
                                                insertCommand.Parameters[f].Value = feature.get_Value(fieldindex).ToString();
                                            }
                                            else
                                            {
                                                throw new Exception("DBType Not Implemented");
                                            }
                                        }
                                        else
                                        {
                                            insertCommand.Parameters[f].Value = System.DBNull.Value;
                                        }
                                    }

                                    // need to the WKT parameter:
                                    string wkt = string.Empty;

                                    if (!feature.Shape.IsEmpty)
                                    {
                                        IGeometry geom = feature.Shape;

                                        try
                                        {
                                            this.PrintMessage("Converting geometry to WKT...");
                                            byte[] geombytes = Util.S2SHelper.ConvertGeometryToWKB(geom);
                                            SharpMap.Geometries.IGeometry sharpGeom = SharpMap.Converters.WellKnownBinary.GeometryFromWKB.Parse(geombytes);
                                            wkt = SharpMap.Converters.WellKnownText.GeometryToWKT.Write(sharpGeom);
                                            this.PrintMessage("...conversion to WKT complete.");
                                        }
                                        catch (Exception ex)
                                        {
                                            System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                                            Console.WriteLine("Exception while converting geometry: " + ex.Message + " for objectid = " + feature.OID.ToString() + ".");
                                        }
                                    }

                                    insertCommand.Parameters[insertCommand.Parameters.Count - 1].Value = wkt;

                                    try
                                    {
                                        insertCommand.ExecuteNonQuery();
                                    }
                                    catch (SQLiteException ex)
                                    {
                                        System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                                        throw;
                                    }
                                }
                            }

                            this.PrintInformation("Committing insert transactions...");
                            trans.Commit();
                            this.PrintInformation("...commit Complete.");
                        }

                        if (newtable)
                        {
                            // create the geometry field:
                            using (SQLiteCommand updateCommand = connection.CreateCommand())
                            {
                                this.PrintInformation("Adding geometry column...");
                                string cmdtext = string.Format(Constants.SQLCreateGeometryField, spatialiteTableName, geometryfieldname, this.SpatialReferenceID.ToString(), geometrytype);
                                this.PrintMessage("SQL: " + cmdtext);
                                updateCommand.CommandText = cmdtext;
                                updateCommand.CommandType = CommandType.Text;
                                updateCommand.ExecuteNonQuery();
                                this.PrintInformation("...geometry column created.");
                            }
                        }

                        // update the geometry field:
                        using (SQLiteCommand updateCommand = connection.CreateCommand())
                        {
                            this.PrintInformation("Updating geometry from WKT...");
                            updateCommand.CommandText = string.Format(Constants.SQLUpdateGeomFromWKT, spatialiteTableName, geometryfieldname, "WKT", this.SpatialReferenceID.ToString(), geometrytype);
                            this.PrintMessage("SQL: " + updateCommand.CommandText);
                            updateCommand.ExecuteNonQuery();
                            this.PrintInformation("...geometry update from WKT complete.");
                        }
                    }

                    connection.Close();
                }

                stopwatch.Stop();

                this.PrintInformation(totalfeaturecount.ToString() + " feature(s) exported in " + ((double)stopwatch.ElapsedMilliseconds / 1000).ToString("0.0000") + " seconds.");
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
