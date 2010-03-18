// <copyright file="LayerfileIndexer.cs" company="Umbriel Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-11-09</date>
// <summary>LayerfileIndexer class file</summary>

namespace Umbriel.ArcGIS.Layer.LayerFile
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Text;

    /// <summary>
    /// indexes layer files for a path
    /// </summary>
    public class LayerfileIndexer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayerfileIndexer"/> class.
        /// </summary>
        /// <param name="searchPath">The search path.</param>
        public LayerfileIndexer(string searchPath)
        {
            this.SearchPath = searchPath;
            this.LayerFiles = new List<string>();
            this.LayerFileExtension = "lyr";
        }

        /// <summary>
        /// Gets or sets the layer file extension.
        /// </summary>
        /// <value>The layer file extension.</value>
        public string LayerFileExtension { get; set; }

        /// <summary>
        /// Gets or sets the search path.
        /// </summary>
        /// <value>The search path.</value>
        public string SearchPath { get; set; }

        /// <summary>
        /// Gets the collection of layer files
        /// </summary>
        /// <value>The layer file collection</value>
        public List<string> LayerFiles { get; private set; }

        /// <summary>
        /// Searches for layer files
        /// </summary>
        public void Search()
        {
            this.DirectorySearch(this.SearchPath);
        }

        /// <summary>
        /// Search for layer files and adds paths to a string collection
        /// </summary>
        /// <param name="path">The path to search</param>
        private void DirectorySearch(string path)
        {
            try
            {

                foreach (string f in Directory.GetFiles(path, "*." + this.LayerFileExtension))
                {
                    this.LayerFiles.Add(f);
                }

                foreach (string d in Directory.GetDirectories(path))
                {
                    this.DirectorySearch(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }
    }
}