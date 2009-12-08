

namespace Umbriel.ArcGIS.Layer.LayerFile
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Text;
    using System.Reflection;
    using System.IO;


    public class LayerfileIndexerHelper
    {

        public static string CreateNewLayerfileIndexDB()
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
                string pathOut = Path.GetTempPath() + "\\Umbriel\\layerfile.index";

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

                return pathOut;
            }
        }

        public static void BuildNewIndexFile(List<string> searchPaths)
        {
            string indexPath = GetDefaultIndexFilePath();

            if ( File.Exists(indexPath))
            {
                File.Delete(indexPath);
            }

            indexPath = CreateNewLayerfileIndexDB();

            List<string> layerFiles = new List<string>();

            foreach (string  path in searchPaths)
            {
                LayerfileIndexer indexer = new LayerfileIndexer(path);

                layerFiles.AddRange(indexer.LayerFiles);                
                
            }


        }

        public static void BuildNewIndexFile(string searchPath)
        {

        }

        /// <summary>
        /// Gets the default index file path.
        /// </summary>
        /// <returns></returns>
        private static string GetDefaultIndexFilePath()
        {
            return  Path.GetTempPath() + "\\Umbriel\\layerfile.index";
        }
    }
}
