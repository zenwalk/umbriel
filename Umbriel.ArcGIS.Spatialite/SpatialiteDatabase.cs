
namespace Umbriel.ArcGIS.Spatialite
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.Diagnostics;
    using System.Text;
    using System.Reflection;
    using System.IO;

    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.DataSourcesFile;
    using ESRI.ArcGIS.DataSourcesGDB;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;
    using Umbriel.ArcGIS.Spatialite;
    using Umbriel.Extensions;

    public class SpatialLiteDB
    {
        public delegate void StatusMessageDelegate(string message);

        public delegate void InsertEventHandler(object sender, SpatialiteProgressEventArgs args);

        public event StatusMessageDelegate StatusMessageEvent = delegate { };

        public event InsertEventHandler InsertEvent = delegate { };
        
        #region Fields
        private string databasePath;

        #endregion

        #region Automatic Properties


        internal SQLiteConnection SQLiteDatabaseConn { get; set; }
        public IWorkspace Workspace { get; set; }

        public int CommitInterval { get; set; }
        public string GeometryFieldName { get; set; }
        public string WhereClause { get; set; }

        public string ConnectionString { get; private set; }


        public string DatabaseFilePath
        {
            get
            {
                return databasePath;
            }
            set
            {
                databasePath = value;
                this.SetConnectionString(databasePath);
            }
        }

        #endregion

        #region Contructors

        public SpatialLiteDB()
        {
            this.CommitInterval = 1000;
            this.GeometryFieldName = "Geometry";

        }

        public SpatialLiteDB(string databasePath)
        {
            this.CommitInterval = 1000;
            this.GeometryFieldName = "Geometry";

            this.DatabaseFilePath = databasePath;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Extracts an embedded file out of a given assembly.
        /// </summary>
        /// <param name="assemblyName">The namespace of you assembly.</param>
        /// <param name="fileName">The name of the file to extract.</param>
        /// <returns>A stream containing the file data.</returns>
        public static Stream GetEmbeddedFile(string assemblyName, string fileName)
        {
            try
            {
                System.Reflection.Assembly a = System.Reflection.Assembly.Load(assemblyName);
                Stream str = a.GetManifestResourceStream(assemblyName + "." + fileName);

                if (str == null)
                    throw new Exception("Could not locate embedded resource '" + fileName + "' in assembly '" + assemblyName + "'");
                return str;
            }
            catch (Exception e)
            {
                throw new Exception(assemblyName + ": " + e.Message);
            }
        }

        /// <summary>
        /// Copies the stream.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        public static void CopyStream(Stream input, Stream output)
        {
            // Insert null checking here for production
            byte[] buffer = new byte[8192];

            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }

        }

        /// <summary>
        /// Creates the new spatialite database.
        /// </summary>
        /// <param name="path">The database path</param>
        public static void CreateNewDatabase(string path)
        {
            if (File.Exists(path))
            {
                throw new ArgumentException("Database File already exists: " + path);
            }
            
            // HACK: using an empty spatialite database in the resource file (geometry_columns  and spatial_ref_sys tables already created)
            using (Stream input = new MemoryStream(Properties.Resources.spatialite))
            {
                using (Stream output = File.Create(path))
                {
                    CopyStream(input, output);
                }
            }
        }

        /// <summary>
        /// Opens the connection to the sqlite database (always loads the spatial extension):
        /// </summary>
        private void Open()
        {
            this.SQLiteDatabaseConn.Open();
            LoadSpatialLiteExtension();  //load the spatial extension
        }

        /// <summary>
        /// Drops the table from the spatialite database
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        public void DropTable(string tableName)
        {
            if (this.SQLiteDatabaseConn.State == ConnectionState.Open && tableName != null && tableName.Length > 0)
            {
                try
                {


                    string sql = "DROP TABLE " + tableName;


                    SQLiteCommand cmd = new SQLiteCommand(sql, this.SQLiteDatabaseConn);

                    int result = cmd.ExecuteNonQuery();

                    cmd.Dispose();

                    return;




                }
                catch (SQLiteException exSQLite)
                {
                    //C onsole.WriteLine(" SQLite Exception: " + exSQLite.Message);
                    System.Diagnostics.Trace.WriteLine(exSQLite.StackTrace);

                    if (exSQLite.Message.Contains("no such table"))
                    {
                        return;
                    }
                    else
                        throw;




                    //return;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                    throw;
                }

            }


        }
        
        //public string GenerateCreateTableSQL(DataTable table)
        //{
        //    StringBuilder sqlStatement = new StringBuilder();


        //    try
        //    {




        //        sqlStatement.Append("CREATE TABLE ");
        //        sqlStatement.Append(table.TableName);
        //        sqlStatement.Append("(");

        //        int i = 0;

        //        foreach (DataColumn col in table.Columns)
        //        {
        //            i++;

        //            string sqlField = FieldSQL(col);

        //            if (sqlField != null
        //      && sqlField.Length > 0
        //      && sqlField.Contains(")").Equals(false))
        //            {
        //                if (i > 0)
        //                    sqlStatement.Append(',');

        //                sqlStatement.Append(sqlField);

        //            }




        //        }

        //        sqlStatement.Append(')');

        //        System.Diagnostics.Trace.WriteLine(" Create Table SQL Statement: " + sqlStatement.ToString());
        //    }
        //    catch (System.Runtime.InteropServices.COMException exCOM)
        //    {
        //        System.Diagnostics.Trace.WriteLine(exCOM.Message);


        //        throw;


        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }









        //    return sqlStatement.ToString();
        //}

        /// <summary>
        /// Closes this connection to the sqlite database.
        /// </summary>
        private void Close()
        {
            if (this.SQLiteDatabaseConn != null 
                && this.SQLiteDatabaseConn.State != ConnectionState.Closed)
            {
                this.SQLiteDatabaseConn.Close();
            }
        }

        public void CreateTable(string sql)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, this.SQLiteDatabaseConn))
                {
                    this.SQLiteDatabaseConn.Open();
                    int result = cmd.ExecuteNonQuery();
                }                
            }
            catch (SQLiteException exSQLite)
            {
                System.Diagnostics.Trace.WriteLine(exSQLite.StackTrace);
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                this.SQLiteDatabaseConn.Close();
            }
        }
        
        public void AddGeometryColumn(string tableName, string columnName, int srid, EnumGeometryType geometryType)
        {
            AddGeometryColumn(tableName, columnName, srid, geometryType,2);
        }

        public void AddGeometryColumn(string tableName, string columnName, int srid, EnumGeometryType geometryType, int dimension)
        {
            String addGeometryColumnClause = String.Format("('{0}', '{1}', {2}, '{3}', {4})",
                                                               tableName,
                                                               columnName,
                                                               srid,
                                                               geometryType.GetStringValue(),
                                                               dimension);

            Trace.WriteLine("AddGeometryColumn: " + string.Format("SELECT AddGeometryColumn {0};", addGeometryColumnClause));

            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(
                    string.Format("SELECT AddGeometryColumn {0};", addGeometryColumnClause),
                    this.SQLiteDatabaseConn))
                {
                    this.SQLiteDatabaseConn.Open();

                    object result = cmd.ExecuteScalar();

                    if (Convert.ToInt32(result) == 0)
                    {
                        throw new SQLiteException(string.Format(
                                   "Cannot create geometry column with type of '{0}', srid:{1}",
                                   geometryType.GetStringValue(), srid));
                    }
                }
            }
            catch (SQLiteException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                if (this.SQLiteDatabaseConn.State != ConnectionState.Closed)
                {
                    this.Close();
                }                
            }
        }

        public void UpdateGeometryColumnFromWKB(string tableName, string columnName, int srid, string wkbColumnName)
        {
            // "UPDATE [{0}] SET {1} = GeomFromWKB({2},{3})";
            // Constants.UpdateGeometryFromWKBSQL


            this.SQLiteDatabaseConn.Open();

            try
            {
                string sql = string.Format(Constants.UpdateGeometryFromWKBSQL, tableName, columnName, wkbColumnName, srid);

                Trace.WriteLine("Update: " + sql);

                using (SQLiteCommand cmd = new SQLiteCommand(
                    sql,
                    this.SQLiteDatabaseConn))
                {
                    object result = cmd.ExecuteScalar();
                }
            }
            catch (SQLiteException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                this.Close();
            }


        }
        
        public void Insert(DataTable table, Dictionary<string,string> fieldNameMap)
        {
            this.Insert(table, table.TableName.Replace(' ', '_'),fieldNameMap);
        }

        public void Insert(DataTable table, string tablename, Dictionary<string,string> fieldNameMap)
        {
            string insertSQL = this.CreateParameterInsert(table, tablename, fieldNameMap);

            using (SQLiteCommand command = this.SQLiteDatabaseConn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = insertSQL;

                foreach (DataColumn col in table.Columns)
                {
                    command.Parameters.Add(
                        new SQLiteParameter("@" + fieldNameMap[col.ColumnName]));
                }


                try
                {
                    this.Open();

                    using (SQLiteTransaction trans = this.SQLiteDatabaseConn.BeginTransaction())
                    {
                        int totalRows = table.Rows.Count;
                        int r = 0;

                        foreach (DataRow row in table.Rows)
                        {
                            for (int i = 0; i < command.Parameters.Count; i++)
                            {
                                command.Parameters[i].Value = row[i];
                            }

                            int result = command.ExecuteNonQuery();

                            if (result.Equals(1))
                            {
                                this.OnInsert(r, totalRows);
                                r++;
                            }
                        }

                        trans.Commit();                        
                    }
                }
                finally
                {
                    this.Close();
                }

     
            }
        }



        public void AddDataTable(DataTable table,Dictionary<string,string> fieldMap)
        {
            this.Insert(table, fieldMap);
        }


        public int FindSRID(string proj4text)
        {
            int srid = -1;

            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(
                    Constants.SelectSRIDByProj4text,
                    this.SQLiteDatabaseConn))
                {

                    this.Open();

                    cmd.Parameters.AddWithValue("@proj4text", proj4text);

                    object result = cmd.ExecuteScalar();

                    if (Convert.ToInt32(result) == 0)
                    {
                        throw new SQLiteException(string.Format(
                                   "Spatial reference doesn't exist \n{0}.",
                                   proj4text));
                    }

                    srid = Convert.ToInt32(result);
                }
            }
            catch (SQLiteException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                this.Close();
            }

            return srid;
        }

        public int FindSRID(int srid)
        {
            return this.FindSRID(srid, "esri");
        }

        public int FindSRID(int srid, string authname)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(
                    Constants.SelectSRIDByAuthSRID,
                    this.SQLiteDatabaseConn))
                {
                    cmd.Parameters.AddWithValue(@"@authname", authname);
                    cmd.Parameters.AddWithValue(@"@authsrid", srid);

                    this.Open();

                    object result = cmd.ExecuteScalar();

                    if (Convert.ToInt32(result) == 0)
                    {
                        throw new InvalidSpatialReferenceException(
                            string.Format(
                                   "Spatial reference doesn't exist \n{0}-{1}.",
                                   authname, srid),
                                   srid,
                                   authname);
                    }

                    srid = Convert.ToInt32(result);
                }
            }
            catch (SQLiteException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                this.Close();
            }

            return srid;
        }


        /// <summary>
        /// Checks to see if the table exists in the database
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>true if the table exists</returns>
        public bool TableExists(string tableName)
        {
            bool exists = false;

            string sql = "SELECT name FROM sqlite_master WHERE name='{0}'";

            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(
                    string.Format(sql,tableName),
                    this.SQLiteDatabaseConn))
                {
                    this.Open();

                    SQLiteDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        exists = true;
                    }
                }
            }
            catch (SQLiteException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                this.Close();
            }

            return exists;
        }

        public bool ColumnExists(string tableName, string columnName)
        {
            bool exists = false;

            string sql = "PRAGMA table_info('{0}')";

            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(
                    string.Format(sql, tableName),
                    this.SQLiteDatabaseConn))
                {
                    this.Open();

                    SQLiteDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        if (rdr["name"].ToString().Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            exists = true;
                        }
                    }
                }
            }
            catch (SQLiteException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                this.Close();
            }

            return exists;
        }


        private string CreateParameterInsert(DataTable table, string tablename, Dictionary<string, string> fieldNameMap)
        {
            StringBuilder fields = new StringBuilder();
            StringBuilder values = new StringBuilder();

            int p = 0;

            foreach (DataColumn col in table.Columns)
            {
                if (p > 0)
                {
                    values.Append(',');
                    fields.Append(',');
                }

                values.Append('@');



                values.Append(fieldNameMap[col.ColumnName]);
                fields.Append(fieldNameMap[col.ColumnName]);
                
                p++;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("INSERT INTO [");
            sb.Append(tablename);
            sb.Append("] (");
            sb.Append(fields.ToString());
            sb.Append(") VALUES (");

            sb.Append(values.ToString());

            sb.Append(")");

            return sb.ToString();
        }

        /*
        public List<SpatialiteTable> TableNames()
        {
            List<SpatialiteTable> tableNames = new List<SpatialiteTable>();

            string sql = @"select *
                from geometry_columns
                order by f_table_name";

            using (SQLiteCommand command = new SQLiteCommand(sql, this.SQLiteDatabaseConn))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SpatialiteTable table = new SpatialiteTable();

                        try
                        {
                            table.TableName = reader["f_table_name"].ToString();
                            table.GeometryColumnName = reader["f_geometry_column"].ToString();
                            table.GeometryType = reader["type"].ToString();

                            if (!System.DBNull.Value.Equals(reader["coord_dimension"]))
                            {
                                table.CoordinateDimension = Convert.ToInt32(reader["coord_dimension"]);
                            }

                            if (!System.DBNull.Value.Equals(reader["srid"]))
                            {
                                table.SpatialReferenceID = Convert.ToInt32(reader["srid"]);
                            }

                            if (!System.DBNull.Value.Equals(reader["spatial_index_enabled"]))
                            {
                                table.SpatialIndexEnabled = Convert.ToBoolean(reader["spatial_index_enabled"]);
                            }

                            tableNames.Add(table);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.StackTrace);
                            throw;
                        }
                    }
                }
            }

            return tableNames;
        }
         * */

        /// <summary>
        /// Deletes the rows from the table:
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public bool DeleteRows(string tableName)
        {
            try
            {
                SQLiteCommand cmd = this.SQLiteDatabaseConn.CreateCommand();

                StringBuilder sql = new StringBuilder("DELETE FROM ");
                sql.Append(tableName);
                sql.Append(';');

                cmd.CommandText = sql.ToString();
                cmd.CommandType = CommandType.Text;
                int result = cmd.ExecuteNonQuery();

                return true;


            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }





        }

        /// <summary>
        /// Loads the spatial lite extension.
        /// </summary>
        public void LoadSpatialLiteExtension()
        {
            if (this.SQLiteDatabaseConn != null)
            {
                if (this.SQLiteDatabaseConn.State == ConnectionState.Open)
                {
                    string exePath = Process.GetCurrentProcess().MainModule.FileName;

                    string exeDir = System.IO.Path.GetDirectoryName(exePath);
                    string spatiaLiteLibDir = exeDir + @"\ExtLib\";

                    try
                    {
                        using (SQLiteCommand command = this.SQLiteDatabaseConn.CreateCommand())
                        {
                            //load the spatial extension:
                            command.CommandText = @"SELECT load_extension('libspatialite-2.dll');";
                            int recordsAffected = command.ExecuteNonQuery();
                        }
                    }
                    catch (System.Data.SQLite.SQLiteException sqliteException)
                    {
                        System.Diagnostics.Trace.WriteLine(sqliteException.ToString());
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        #endregion

        #region Private  Properties

        #endregion

        #region Private Methods

        private void OnInsert(int rowIndex, int total)
        {
            this.InsertEvent(this,
                new SpatialiteProgressEventArgs(rowIndex, total));            
        }

        private string GetDBConnectionString()
        {
            return this.ConnectionString;
        }

        private void OnUpdateStatusMessage(string message)
        {
            if (StatusMessageEvent != null)
                StatusMessageEvent(message);
        }

        /// <summary>
        /// Sets the connection string.
        /// </summary>
        /// <param name="path">The path.</param>
        private void SetConnectionString(string path)
        {
            this.ConnectionString = string.Format(Properties.Settings.Default.BasicSQliteConnectionString, path);
            this.SQLiteDatabaseConn = new SQLiteConnection(this.ConnectionString);
        }

        /// <summary>
        /// Executes the SQL
        /// not very efficient since a new command is created for each row
        /// when i get more time, i'll approach the inserts in a different way
        /// http://sqlite.phxsoftware.com/forums/t/134.aspx
        /// 
        /// </summary>
        /// <param name="transactionList">The transaction list.</param>
        private void ExecuteSQL(List<string> transactionList)
        {

            try
            {

                using (SQLiteTransaction trans = this.SQLiteDatabaseConn.BeginTransaction())
                {

                    foreach (string sql in transactionList)
                    {
                        using (SQLiteCommand cmd = trans.Connection.CreateCommand())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = sql;
                            Trace.WriteLine(sql);

                            int recordsAffected = cmd.ExecuteNonQuery();

                            if (recordsAffected <= 0)
                            {
                                throw new Exception("SQL Execute Problem.");
                            }


                        }

                    }

                    trans.Commit();
                }



                return;
            }
            catch (SQLiteException exSQLite)
            {
                Trace.WriteLine(exSQLite.Message);
                throw;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }

        }


        /// <summary>
        /// Generates the SQL field portion for a create table statement
        /// </summary>
        /// <param name="field">IField</param>
        /// <returns></returns>
        private string FieldSQL(IField field)
        {
            string nullable = "";
            if (field.IsNullable.Equals(false))
                nullable = " NOT NULL ";

            string fieldName = field.Name.Replace('.', '_');

            switch (field.Type)
            {
                case esriFieldType.esriFieldTypeBlob:
                    return fieldName + " BLOB " + nullable;
                    break;
                case esriFieldType.esriFieldTypeDate:
                    return fieldName + " TEXT " + nullable;
                    break;
                case esriFieldType.esriFieldTypeDouble:
                    return fieldName + " REAL " + nullable;
                    break;
                case esriFieldType.esriFieldTypeGUID:
                    return fieldName + " TEXT " + nullable;
                    break;
                case esriFieldType.esriFieldTypeGeometry:
                    switch (field.GeometryDef.GeometryType)
                    {
                        case esriGeometryType.esriGeometryPoint:
                            return this.GeometryFieldName + " POINT ";
                            break;

                        case esriGeometryType.esriGeometryPolygon:
                            return this.GeometryFieldName + " POLYGON ";
                            break;

                        case esriGeometryType.esriGeometryPolyline:
                            return this.GeometryFieldName + " LINESTRING ";
                            break;

                        case esriGeometryType.esriGeometryLine:
                            break;
                        case esriGeometryType.esriGeometryAny:

                        case esriGeometryType.esriGeometryBag:

                        case esriGeometryType.esriGeometryBezier3Curve:

                        case esriGeometryType.esriGeometryCircularArc:

                        case esriGeometryType.esriGeometryEllipticArc:

                        case esriGeometryType.esriGeometryEnvelope:


                        case esriGeometryType.esriGeometryMultiPatch:

                        case esriGeometryType.esriGeometryMultipoint:

                        case esriGeometryType.esriGeometryNull:

                        case esriGeometryType.esriGeometryPath:
                        case esriGeometryType.esriGeometryRay:

                        case esriGeometryType.esriGeometryRing:

                        case esriGeometryType.esriGeometrySphere:

                        case esriGeometryType.esriGeometryTriangleFan:

                        case esriGeometryType.esriGeometryTriangleStrip:

                        case esriGeometryType.esriGeometryTriangles:

                        default:
                            Console.WriteLine(fieldName + " skipped.  " + field.GeometryDef.GeometryType.ToString() + " not supported.");
                            return null;
                            break;
                    }
                    return null;
                    break;
                case esriFieldType.esriFieldTypeGlobalID:
                    return fieldName + " TEXT " + nullable;
                    break;
                case esriFieldType.esriFieldTypeInteger:
                    return fieldName + " INTEGER " + nullable;
                    break;
                case esriFieldType.esriFieldTypeOID:
                    return fieldName + " INTEGER " + nullable;
                    break;
                case esriFieldType.esriFieldTypeRaster:
                    return null;
                    break;
                case esriFieldType.esriFieldTypeSingle:
                    return fieldName + " REAL " + nullable;
                    return null;
                    break;
                case esriFieldType.esriFieldTypeSmallInteger:
                    return fieldName + " INTEGER " + nullable;
                    break;
                case esriFieldType.esriFieldTypeString:
                    return fieldName + " TEXT " + nullable;
                    break;
                case esriFieldType.esriFieldTypeXML:
                    return fieldName + " TEXT " + nullable;
                    break;
                default:
                    return null;
                    break;
            }



        }


        //private string FieldSQL(DataColumn column)
        //{
        //    string nullable = "";


        //    if (!column.AllowDBNull)
        //        nullable = " NOT NULL ";

        //    string fieldName = column.ColumnName.Replace('.', '_');

        //    switch (System.Type.GetTypeCode(column.DataType))
        //    {
        //        case TypeCode.Boolean:
        //            return fieldName + " INTEGER " + nullable;
        //            break;
        //        case TypeCode.Byte:
        //            return null;
        //            break;
        //        case TypeCode.Char:
        //            return fieldName + " TEXT " + nullable;
        //            break;
        //        case TypeCode.DBNull:
        //            return null;
        //            break;
        //        case TypeCode.DateTime:
        //            return fieldName + " TEXT " + nullable;
        //            break;
        //        case TypeCode.Decimal:
        //            return fieldName + " REAL " + nullable;
        //            break;
        //        case TypeCode.Double:
        //            return fieldName + " REAL " + nullable;
        //            break;
        //        case TypeCode.Empty:
        //            return null;
        //            break;
        //        case TypeCode.Int16:
        //            return fieldName + " INTEGER " + nullable;
        //            break;
        //        case TypeCode.Int32:
        //            return fieldName + " INTEGER " + nullable;
        //            break;
        //        case TypeCode.Int64:
        //            return fieldName + " INTEGER " + nullable;
        //            break;
        //        case TypeCode.Object:
        //            return null;
        //            break;
        //        case TypeCode.SByte:
        //            return fieldName + " INTEGER " + nullable;
        //            break;
        //        case TypeCode.Single:
        //            return fieldName + " REAL " + nullable;
        //            break;
        //        case TypeCode.String:
        //            return fieldName + " TEXT " + nullable;
        //            break;
        //        case TypeCode.UInt16:
        //            return fieldName + " INTEGER " + nullable;
        //            break;
        //        case TypeCode.UInt32:
        //            return fieldName + " INTEGER " + nullable;
        //            break;
        //        case TypeCode.UInt64:
        //            return fieldName + " INTEGER " + nullable;
        //            break;
        //        default:
        //            break;
        //    }






        //}





        #endregion            
    }


}
