
namespace Umbriel.ArcGIS.Layer.LayerFile
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.Diagnostics;
    using System.Text;
    using System.Reflection;
    using System.IO;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.esriSystem;

    /// <summary>
    /// delegate event handler
    /// </summary>
    public delegate void ProgressEventHandler (object sender, int progress,int total,string message);

    /// <summary>
    /// Class for building the layer file index:
    /// </summary>
    public class LayerfileIndexBuilder
    {
        public event ProgressEventHandler Progress;
                
        /// <summary>
        /// Initializes a new instance of the <see cref="LayerfileIndexBuilder"/> class.
        /// </summary>
        /// <param name="indexPath">The index path.</param>
        public LayerfileIndexBuilder(string indexPath)
        {
            this.IndexPath = indexPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerfileIndexBuilder"/> class.
        /// </summary>
        public LayerfileIndexBuilder()
        {
            this.IndexPath = GetDefaultIndexFilePath();
        }

        /// <summary>
        /// Gets or sets the index path.
        /// </summary>
        /// <value>The index path.</value>
        public string IndexPath { get; set; }

        /// <summary>
        /// Builds the new index.
        /// </summary>
        /// <param name="searchPaths">The list of search paths.</param>
        public void BuildNewIndex(List<string> searchPaths)
        {

            if (File.Exists(this.IndexPath))
            {
                File.Delete(this.IndexPath);
            }

            CreateNewIndexFile(this.IndexPath);

            BuildIndex(searchPaths);
        }

        /// <summary>
        /// Builds the index file with layers
        /// </summary>
        /// <param name="searchPaths">The search paths.</param>
        public void BuildIndex(List<string> searchPaths)
        {
            if (!File.Exists(this.IndexPath))
            {
                CreateNewIndexFile(this.IndexPath);
            }
            
            List<string> layerFiles = new List<string>();

            int i = 0;


            foreach (string searchPath in searchPaths)
            {
                i++;

                LayerfileIndexer indexer = new LayerfileIndexer(searchPath);
                indexer.Search();
                layerFiles.AddRange(indexer.LayerFiles);

                OnProgressUpdate(i, searchPaths.Count, "Scanning Search Path " + searchPath);
            }

            List<string> insertSQLStatements = new List<string>();

            i = 0;
            foreach (string filePath in layerFiles)
            {
                i++;

                FileInfo fileInfo = new FileInfo(filePath);

                
                ILayerFile layerFile = new LayerFileClass();
                layerFile.Open(filePath);
                ILayer  layer = layerFile.Layer;

                ILayerGeneralProperties layerProps = (ILayerGeneralProperties)layer;
                ILayerExtensions layerExt = (ILayerExtensions) layer;
                
                string lyrGUID = "00000000-0000-0000-0000-000000000000";
                string revision = "0";

                if ( Layer.Util.LayerExtHelper.UmbrielPropertySetExists(layerExt))
                {
                    IPropertySet propertySet = Util.LayerExtHelper.GetUmbrielPropertySet(layerExt);
                    if (propertySet != null && Util.LayerExtHelper.PropertyExists(propertySet, "GUID"))
                    {
                        lyrGUID = propertySet.GetProperty("GUID").ToString();
                    }

                    if (propertySet != null && Util.LayerExtHelper.PropertyExists(propertySet, "revision"))
                    {
                        revision = propertySet.GetProperty("revision").ToString();
                    }
                }
                                 
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("INSERT INTO layerfile ");
                sql.AppendLine("(lyrgid,lyrName,lyrDescription,lyrFileName,lyrFullPath,lyrParentDir,lyrRevision,DateRecCreated,DateRecModified)");
                sql.AppendLine(" VALUES (");

                sql.AppendLine("'" + lyrGUID + "'");
                sql.AppendLine(",'" + layer.Name.Replace("'","''")  + "'");
                sql.AppendLine(",'" + layerProps.LayerDescription.Replace("'", "''") + "'");
                sql.AppendLine(",'" + Path.GetFileName(filePath) + "'");
                sql.AppendLine(",\"" + filePath + "\"");
                sql.AppendLine(",'" + Path.GetDirectoryName(filePath) + "'");
                sql.AppendLine("," + revision + "");
                                
                sql.AppendLine(",'" + SqliteDateString(DateTime.Now) + "'");
                sql.AppendLine(",'" + SqliteDateString(DateTime.Now) + "'");

                sql.AppendLine(")");

                Debug.WriteLine(sql.ToString());
                insertSQLStatements.Add(sql.ToString());

                OnProgressUpdate(i, layerFiles.Count, "Building layer data file: ");

                if (layer != null)
                {
                    // System.Runtime.InteropServices.Marshal.FinalReleaseComObject(layerFile);
                }
            }

            i = 0;

            // build insert sql statement
            using (SQLiteConnection cnn = new SQLiteConnection(this.GetDBConnectionString()))
            {
                cnn.Open();

                using (DbTransaction transaction = cnn.BeginTransaction())
                {
                    using (DbCommand cmd = cnn.CreateCommand())
                    {
                        foreach (string sql in insertSQLStatements)
                        {
                            i++;

                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();

                            OnProgressUpdate(i, insertSQLStatements.Count, "Inserting data: ");
                        }
                    }
                    transaction.Commit();
                }

                OnProgressUpdate(i, insertSQLStatements.Count, "Insert Complete!");

                cnn.Close();
            }
        }

        /// <summary>
        /// Gets the default index file path.
        /// </summary>
        /// <returns>string value for the default path for the index file</returns>
        internal string GetDefaultIndexFilePath()
        {
            return Path.GetTempPath() + "\\Umbriel\\layerfile.index";
        }

        /// <summary>
        /// Creates the new index file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private void CreateNewIndexFile(string path)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            string[] resNames = a.GetManifestResourceNames();
            Stream stream = a.GetManifestResourceStream("Umbriel.ArcGIS.Layer.LayerFile.layerfile.index");

            if (stream == null)
            {
                throw new Exception("Could not locate embedded resource: Umbriel.ArcGIS.Layer.LayerFile.layerfile.index");
            }
            else
            {
                string pathOut = path;

                string pathDir = Path.GetDirectoryName(pathOut);
                if (!Directory.Exists(pathOut))
                {
                    Directory.CreateDirectory(pathDir);
                }

                Stream fileOut = File.Create(pathOut);

                const int SIZE_BUFF = 1024;
                byte[] buffer = new Byte[SIZE_BUFF];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, SIZE_BUFF)) > 0)
                {
                    fileOut.Write(buffer, 0, bytesRead);
                }

                fileOut.Close();

                return;
            }
        }

        /// <summary>
        /// Gets the DB connection string.
        /// </summary>
        /// <returns>sqlite database connection string for the index path</returns>
        private string GetDBConnectionString()
        {
            return "Data Source=" + this.IndexPath + ";Version=3;";
        }

        /// <summary>
        /// Called when [progress update].
        /// </summary>
        /// <param name="progress">The progress of the index build</param>
        /// <param name="total">The total number of layer files</param>
        private void OnProgressUpdate(int progress, int total,string message)
        {
            if (this.Progress != null)
            {
                this.Progress(this, progress, total,message);
            }
        }
        
        /// <summary>
        /// converts the datetime to a sqlite formatted string.
        /// </summary>
        /// <param name="dateValue">The date value</param>
        /// <returns>sqlite formatted string</returns>
        private string SqliteDateString(DateTime dateValue)
        {
            return dateValue.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}

