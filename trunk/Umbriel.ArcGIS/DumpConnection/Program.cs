using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using ConnectionStringList = System.Collections.Generic.List<string>;


namespace DumpConnection
{
    class Program
    {
        private  const string LayerConnectionString =  "LayerName={0},{1},{2}";

        private static LicenseInitializer m_AOLicenseInitializer = new DumpConnection.LicenseInitializer();
    
        [STAThread()]
        static void Main(string[] args)
        {
            string dirpath = args[0];

            string fileext = "*.lyr";

            if (args.Length >  1)
            {
                fileext = string.Format("*.{0}", args[1]);
            }


            



            //ESRI License Initializer generated code.
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeArcView },
            new esriLicenseExtensionCode[] { });


            if (File.Exists(dirpath))
            {
                Console.WriteLine(string.Join("\n", ReadFile(dirpath).ToArray()));
                m_AOLicenseInitializer.ShutdownApplication();
                return;
            }
            else
            {
                if (System.IO.Directory.Exists(dirpath).Equals(false))
                {
                    Console.WriteLine(string.Format("{0} does not exist", dirpath));
                    m_AOLicenseInitializer.ShutdownApplication();
                    return;
                }
            }

            DirectoryInfo di = new DirectoryInfo(dirpath);

            FileInfo[] files = di.GetFiles(fileext, SearchOption.AllDirectories);

            foreach (FileInfo file in files)
            {
                ConnectionStringList list = new ConnectionStringList();

                list.AddRange(ReadFile(file.FullName));

                //if (System.IO.Path.GetExtension(file.FullName).Equals(".lyr"))
                //{
                //    list = GetConnectionString(OpenLayerFile(file.FullName), file.FullName);
                //}
                //else if (System.IO.Path.GetExtension(file.FullName).Equals(".mxd"))
                //{
                //    IMapDocument mapdoc = OpenMapDocument(file.FullName);

                //    for (int i = 0; i < mapdoc.MapCount; i++)
                //    {
                //        IMap map = mapdoc.Map[i];
                //        if (map != null)
                //        {
                //            IEnumLayer layers =  map.get_Layers(null,true);
                //            ILayer maplayer = null;

                //            while ((maplayer = layers.Next()) != null)
                //            {
                //                if (maplayer != null)
                //                {
                //                    list.AddRange(GetConnectionString(maplayer, file.FullName));
                //                }
                //            }
                            
                //        }
                //    }
                //}

                Console.WriteLine(string.Join("\n", list.ToArray()));                
            }

                        

            // string filePath = @"\\w-dpu-48\dpu_gisdata\Layers\dpu\wControlValve.lyr";


            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()
            m_AOLicenseInitializer.ShutdownApplication();
        }

        private static ConnectionStringList GetConnectionString(ILayer layer,string filepath)
        {
            ConnectionStringList connectionStrings = new ConnectionStringList();

            if (layer is IFeatureLayer)
            {
                IFeatureLayer featureLayer = layer as IFeatureLayer;

                IDataset dataset = featureLayer.FeatureClass as IDataset;

                if (dataset != null)
                {
                    object propertyNames;
                    object propertyValues;

                    dataset.Workspace.ConnectionProperties.GetAllProperties(out propertyNames, out propertyValues);

                    connectionStrings.Add(
                        string.Format(LayerConnectionString, layer.Name, MakeConnectionString(propertyNames, propertyValues), filepath));
                }
                else
                {
                    connectionStrings.Add(string.Format(LayerConnectionString, layer.Name, "No Dataset Information", filepath));
                }


            }
            else if (layer is IGroupLayer)
            {
                ICompositeLayer compositeLayer = layer as ICompositeLayer;

                for (int i = 0; i < compositeLayer.Count; i++)
                {
                    ILayer subLayer = compositeLayer.Layer[i];

                    connectionStrings.AddRange(GetConnectionString(subLayer, filepath));
                }                
            }
            else
            {
            }

            return connectionStrings;
        }

        private static string MakeConnectionString(object propnames, object propvalues)
        {
            ConnectionStringList connstrings = new ConnectionStringList();

            string kvp = "{0}={1}";

            System.Array propNameArray = (System.Array)propnames;
            System.Array propValuesArray = (System.Array)propvalues;

            for (int i = 0; i < propValuesArray.Length; i++)
			{
                connstrings.Add(string.Format(kvp, propNameArray.GetValue(i), propValuesArray.GetValue(i)));
			}

            return string.Join(",", connstrings.ToArray());
        }

        private static IPropertySet GetConnectionProperties(string path)
        {
            return null;

        }

        private static IMapDocument OpenMapDocument(string path)
        {
            IMapDocument mapdoc = new MapDocumentClass();
            mapdoc.Open(path);

            return mapdoc;

            

        }

        private static ILayer  OpenLayerFile(string path)
        {
            // Create a new GxLayer
            ESRI.ArcGIS.Catalog.IGxLayer gxLayer = new ESRI.ArcGIS.Catalog.GxLayerClass();

            ESRI.ArcGIS.Catalog.IGxFile gxFile = (ESRI.ArcGIS.Catalog.IGxFile)gxLayer; //Explicit Cast

            // Set the path for where the layerfile is located on disk
            gxFile.Path = path;


            if (!(gxLayer.Layer == null))
            {
                return gxLayer.Layer;
            }
            else
            {
                return null;
            }

        }

        private static ConnectionStringList ReadFile(string path)
        {
            ConnectionStringList list = new ConnectionStringList();

            FileInfo file = new FileInfo(path);

            if (file.Exists)
            {
                if (System.IO.Path.GetExtension(file.FullName).Equals(".lyr"))
                {
                    list = GetConnectionString(OpenLayerFile(file.FullName), file.FullName);
                }
                else if (System.IO.Path.GetExtension(file.FullName).Equals(".mxd"))
                {
                    IMapDocument mapdoc = OpenMapDocument(file.FullName);

                    for (int i = 0; i < mapdoc.MapCount; i++)
                    {
                        IMap map = mapdoc.Map[i];
                        if (map != null)
                        {
                            IEnumLayer layers = map.get_Layers(null, true);
                            ILayer maplayer = null;

                            while ((maplayer = layers.Next()) != null)
                            {
                                if (maplayer != null)
                                {
                                    list.AddRange(GetConnectionString(maplayer, file.FullName));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine(string.Format("'{0}' does not exist.", path));
            }

            return list;
        }



    }
}
