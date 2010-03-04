

namespace Umbriel.ArcGIS.Layer.ConnectionReport
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.IO;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.DataSourcesGDB;
    using ESRI.ArcGIS.GISClient;

    public class MXDAnalyzer
    {

        public string MXDFilePath { get; private set; }

        private string ReportFilePath { get; set; }

        private StreamWriter ReportFileStream { get; set; }

        public MXDAnalyzer(string mxdfilepath)
        {
            this.MXDFilePath = mxdfilepath;
            this.ReportFilePath = Path.GetTempPath() +  @"\\" +  Path.GetFileNameWithoutExtension(mxdfilepath) + ".connectionreport.txt";
        }

        public MXDAnalyzer(string mxdfilepath, string reportfilepath)
        {
            this.MXDFilePath = mxdfilepath;

            if (string.IsNullOrEmpty(reportfilepath))
            {
                this.ReportFilePath = Path.GetTempPath() + @"\\" + Path.GetFileNameWithoutExtension(mxdfilepath) + ".connectionreport.txt";
            }
            else
            {
                this.ReportFilePath = reportfilepath;
            }
        }

        /// <summary>
        /// Generates the report.
        /// </summary>
        public void GenerateReport()
        {
           this.ReportFileStream = new StreamWriter(this.ReportFilePath, true);

           Console.WriteLine("Report Path=" + this.ReportFilePath);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            FileInfo mxdfileinfo = new FileInfo(this.MXDFilePath);

            try
            {
                IMapDocument mapDocument = new MapDocumentClass();
                mapDocument.Open(this.MXDFilePath, string.Empty);

                for (int m = 0; m < mapDocument.MapCount; m++)
                {
                    IMap map = mapDocument.get_Map(m);

                    IMapLayers mapLayers = (IMapLayers)map;

                    IEnumLayer rootLayers = (IEnumLayer)map.get_Layers(null, false);

                    ILayer rootLayer = null;

                    while ((rootLayer = rootLayers.Next()) != null)
                    {
                        ReadLayer(rootLayer,null);
                    }
                }

                // mapDocument.Save(mapDocument.UsesRelativePaths, false);
                mapDocument.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            this.ReportFileStream.Close();

            Console.WriteLine("Report File Stream Closed: " + this.ReportFilePath);
        }

        private void ReadLayer(ILayer layer, ILayer parentLayer)
        {            
            string connectionstringdata = string.Empty;
            string parentlayername = "None";

            if (parentLayer != null)
            {
                parentlayername = parentLayer.Name;
            }

            if (layer is GroupLayer)
            {
                ICompositeLayer compositeLayer = (ICompositeLayer)layer;

                for (int i = 0; i < compositeLayer.Count; i++)
                {
                    ILayer sublayer = compositeLayer.get_Layer(i);
                    ReadLayer(sublayer, layer);
                }
            }
            else if (layer is IGeoFeatureLayer)
            {
                IGeoFeatureLayer geofeaturelayer = (IGeoFeatureLayer)layer;
                 connectionstringdata = GetConnectionString(geofeaturelayer);
            }
            else if (layer is ICoverageAnnotationLayer)
            {
                //ignore for now:
                Trace.WriteLine("Converage Anno:  " + layer.Name);
            }
            else if (layer is IMapServerLayer)
            {
                IMapServerLayer mapserverLayer = (IMapServerLayer)layer;
                IAGSServerObjectName serverObject;

                string url = string.Empty;
                string mapname = string.Empty;

                mapserverLayer.GetConnectionInfo(out serverObject, out url, out mapname);

                string connstring = "url={0};map={1};name={2};type={3}";

                connectionstringdata = string.Format(connstring, serverObject.URL, mapname, serverObject.Name, serverObject.Type);
            }
            else if (layer is RasterLayer)
            {
                Trace.WriteLine("Raster Layer:  " + layer.Name);
                connectionstringdata = GetConnectionString(layer);
            }
            else
            {
                Console.WriteLine("Unknown Layer Type: " + layer.Name);
            }

            FileInfo fi = new FileInfo(this.MXDFilePath);

            OnReadLayer(layer.Name, parentlayername, layer.Valid, connectionstringdata, Path.GetFileName(this.MXDFilePath),this.MXDFilePath,fi.LastWriteTime);

            Trace.WriteLine(string.Format("Layer:{0},Valid={1},ConnectionProps={2},ParentLayer={3}", layer.Name, layer.Valid.ToString(), connectionstringdata, parentlayername));
        }

        private void OnReadLayer(string layername,string parentlayername,bool layerisvalid,string connectionproperties,string mxdfilename,string mxdfilepath,DateTime lastmodifieddate )
        {
            if (this.ReportFileStream !=null)
            {
                this.ReportFileStream.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6}",layername,parentlayername,layerisvalid.ToString(),connectionproperties,mxdfilename,mxdfilepath,lastmodifieddate.ToString()));
                this.ReportFileStream.Flush();
            }
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="geofeaturelayer">The geofeaturelayer.</param>
        /// <returns></returns>
        private string GetConnectionString(IGeoFeatureLayer geofeaturelayer)
        {
            return GetConnectionString((ILayer)geofeaturelayer);
        }
        
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <returns></returns>
        private string GetConnectionString(ILayer layer )
        {
            try
            {
                string connectionstring = string.Empty;
     
                IDataLayer dataLayer = (IDataLayer)layer;
                IDatasetName datasetName = (IDatasetName)dataLayer.DataSourceName;
                IWorkspaceName workspaceName = (IWorkspaceName)datasetName.WorkspaceName;

                IPropertySet propertySet = workspaceName.ConnectionProperties;

                Dictionary<string, string> properties = Umbriel.ArcGIS.Layer.LayerHelper.BuildDictionary(propertySet);

                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, string> property in properties)
                {
                    sb.Append(property.Key);
                    sb.Append("=");
                    sb.Append(property.Value);
                    sb.Append(";");
                }

                connectionstring = sb.ToString();

                return connectionstring;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                return "ERROR: Cannot Access Connection Information";
            }
        }



    }
}
