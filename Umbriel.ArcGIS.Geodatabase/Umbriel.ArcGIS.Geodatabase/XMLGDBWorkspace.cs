// <copyright file="XMLGDBWorkspace.cs" company="Umbriel Project">
// Copyright (c) 2010 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-02-??</date>
// <summary>XMLGDBWorkspace class file </summary>

namespace Umbriel.ArcGIS.Geodatabase
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;
    using ESRI.ArcGIS.DataSourcesGDB;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Geodatabase;

    /// <summary>
    /// XMLGDBWorkspace class
    /// </summary>
    [Guid("b6f6355d-801d-4653-b7eb-74ff94b23b85")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Umbriel.ArcGIS.Geodatabase.XMLGDBWorkspace")]
    public class XMLGDBWorkspace
    {
        /// <summary>
        /// XML Fragment string for the workspace connection 
        /// </summary>
        private string xmlFragment;

        /// <summary>
        /// Initializes a new instance of the <see cref="XMLGDBWorkspace"/> class.
        /// </summary>
        public XMLGDBWorkspace()
        {
        }

        #region Properties
        /// <summary>
        /// Gets the workspace.
        /// </summary>
        /// <value>The workspace.</value>
        public IWorkspace Workspace { get; private set; }

        /// <summary>
        /// Gets or sets XML Fragment Property
        /// </summary>
        public string XMLFragment
        {
            get
            {
                return this.xmlFragment;
            }

            set
            {
                try
                {
                    this.xmlFragment = value;
                    this.Workspace = this.CreateWorkspace(this.xmlFragment);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ArcZonaWorkspace.XMLFragment Exception: " + ex.Message + "\n\nStackTrace: " + ex.StackTrace);
                    throw;
                }
            }
        } // XML Fragment Property

        #endregion

        #region Methods

        /// <summary>
        /// Creates a workspace from XML fragment.
        /// This is useful for storing connection settings in a single XML Fragment in a config file
        /// </summary>
        /// <param name="xmlFrag">The XML frag.</param>
        /// <returns>
        /// An IWorkspace of the workspace settings in the XML Fragment
        /// </returns>
        public IWorkspace CreateWorkspace(string xmlFrag)
        {
            Trace.WriteLine("ArcZonaWorkspace.CreateWorkspace XMLFragment.");
            try
            {
                Hashtable hash = this.WrkspcXMLFrag2Hash(xmlFrag);
                Trace.WriteLine("ArcZonaWorkspace.CreateWorkspace Hash Item Count: " + hash.Count.ToString());

                if (hash != null)
                {
                    return this.CreateWorkspace(hash);
                }
                else
                {
                    // raise an event here:
                    return null;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ArcZonaWorkspace.CreateWorkspace Exception: " + ex.Message + "\n\nStackTrace: " + ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// creates workspace from Hash table of workspace settings:
        /// </summary>
        /// <param name="workspaceHashTable">The workspace hash table.</param>
        /// <returns>
        /// An IWorkspace of the workspace settings in the hash table param
        /// </returns>
        public IWorkspace CreateWorkspace(Hashtable workspaceHashTable)
        {
            IWorkspace workSpace = null;

            try
            {
                if ((workspaceHashTable != null) && (workspaceHashTable.Count != 0) && (workspaceHashTable.ContainsKey("TYPE") == true))
                {
                    IWorkspaceFactory workspaceFactory = null;
                    string workspaceType = workspaceHashTable["TYPE"].ToString();

                    switch (workspaceType)
                    {
                        case "FILE":

                            if (workspaceHashTable.ContainsKey("PATH") && workspaceHashTable.ContainsKey("NAME"))
                            {
                                string fileName = workspaceHashTable["NAME"].ToString();
                                string filePath = workspaceHashTable["PATH"].ToString();
                                string[] fileToken = fileName.Split('.');
                                string fileExt = fileToken[(fileToken.Length - 1)];

                                if (fileExt.ToLower().Equals("gdb"))
                                {
                                    workspaceFactory = new FileGDBWorkspaceFactoryClass();
                                }
                                else if (fileExt.ToLower().Equals("mdb"))
                                {
                                    workspaceFactory = new AccessWorkspaceFactoryClass();
                                }

                                if (workspaceFactory != null)
                                {
                                    workSpace = workspaceFactory.OpenFromFile(filePath + "\\" + fileName, 0);
                                }
                            }

                            break;

                        case "SDE":
                            workSpace = this.XMLHash2SDEWorkspace(workspaceHashTable);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ArcZonaWorkspace.CreateWorkspace Exception: " + ex.Message + "\n\nStackTrace: " + ex.StackTrace);
                throw;
            }

            return workSpace;
        }

        /// <summary>
        /// Extracts workspace settings from an XML Fragment and puts them into a hashtable:
        /// </summary>
        /// <param name="xmlFrag">The XML frag.</param>
        /// <returns>
        /// hashtable of the geodatabase workspace parameters
        /// </returns>
        private Hashtable WrkspcXMLFrag2Hash(string xmlFrag)
        {
            Trace.WriteLine("Start WrkspcXMLFrag2Hash");
            Hashtable hash = new Hashtable();

            try
            {
                NameTable nameTable = new NameTable();
                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
                namespaceManager.AddNamespace("rk", "urn:store-items");

                XmlParserContext parserContext = new XmlParserContext(null, namespaceManager, null, XmlSpace.None);
                System.Xml.XmlReader xmlReader = new System.Xml.XmlTextReader(xmlFrag, XmlNodeType.Element, parserContext);

                // iterate through the attributes of the <WORKSPACE> XML fragment and load into hashtable:
                try
                {
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            if (xmlReader.HasAttributes == true)
                            {
                                try
                                {
                                    while (xmlReader.MoveToNextAttribute())
                                    {
                                        hash.Add(xmlReader.Name.ToUpper(), xmlReader.Value);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Trace.WriteLine("ArcZonaWorkspace.WrkspcXMLFrag2Hash Exception  Inside While: " + ex.Message + "\n\nStackTrace: " + ex.StackTrace);
                                    throw;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ArcZonaWorkspace.WrkspcXMLFrag2Hash Exception: " + ex.Message + "\n\nStackTrace: " + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ArcZonaWorkspace.WrkspcXMLFrag2Hash Exception: " + ex.Message + "\n\nStackTrace: " + ex.StackTrace);
                throw;
            }

            Trace.WriteLine("End WrkspcXMLFrag2Hash");

            return hash;
        }

        /// <summary>
        /// creates an SDE workspace from the hashtable settings:
        /// </summary>
        /// <param name="hash">Hashtable containing the SDE connection parameters</param>
        /// <returns>An IWorkspace for the SDE Workspace</returns>
        private IWorkspace XMLHash2SDEWorkspace(Hashtable hash)
        {
            IWorkspace workSpace = null;

            // load the SDE setting string vars:
            string server = string.Empty, instance = string.Empty, passWord = string.Empty, userName = string.Empty, version = string.Empty, database = string.Empty, authMode = string.Empty;
            if (hash.ContainsKey("SERVER") == true)
            {
                server = hash["SERVER"].ToString();
            }

            if (hash.ContainsKey("INSTANCE") == true)
            {
                instance = hash["INSTANCE"].ToString();
            }

            if (hash.ContainsKey("PASSWORD") == true)
            {
                passWord = hash["PASSWORD"].ToString();
            }

            if (hash.ContainsKey("USER") == true)
            {
                userName = hash["USER"].ToString();
            }

            if (hash.ContainsKey("VERSION") == true)
            {
                version = hash["VERSION"].ToString();
            }

            if (hash.ContainsKey("DATABASE") == true)
            {
                database = hash["DATABASE"].ToString();
            }

            if (hash.ContainsKey("AUTH_MODE") == true)
            {
                authMode = hash["AUTH_MODE"].ToString();
            }

            Trace.WriteLine("XMLHash2SDEWorkspace.database=" + database);

            // create a property set of SDE settings:
            IPropertySet propertySet = Utility.ArcSDEConnPropSet(server, instance, userName, passWord, database, version, authMode);

            if (propertySet != null)
            {
                IWorkspaceFactory workspaceFactory;

                // create the workspace factory 
                try
                {
                    workspaceFactory = (IWorkspaceFactory)new SdeWorkspaceFactoryClass();

                    // open the workspace:
                    try
                    {
                        Trace.WriteLine("IPropertySet String= /n" + this.GetPropertySetString(propertySet));
                        ////propertySet = new PropertySet();
                        ////propertySet.SetProperty("SERVER", "wit169");
                        ////propertySet.SetProperty("INSTANCE", @"sde:sqlserver:wit169\dev2005");
                        ////propertySet.SetProperty("DATABASE", "DPU");
                        ////propertySet.SetProperty("USER", "DPU");
                        ////propertySet.SetProperty("PASSWORD", "oldman");
                        ////propertySet.SetProperty("VERSION", "sde.DEFAULT");

                        workSpace = workspaceFactory.Open(propertySet, 0);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("ArcZona.ArcZonaWorkspace_XML Hash2SDEWorkspace Open Exception: " + ex.Message);
                        Trace.WriteLine("ArcZona.ArcZonaWorkspace_XML Hash2SDEWorkspace Open StackTrace: " + ex.StackTrace);

                        Trace.WriteLine("server=" + server);
                        Trace.WriteLine("instance=" + instance);
                        Trace.WriteLine("userName=" + userName);
                        Trace.WriteLine("passWord=" + passWord);
                        Trace.WriteLine("database=" + database);
                        Trace.WriteLine("version=" + version);
                        Trace.WriteLine("authMode=" + authMode);

                        Trace.WriteLine("ArcZonaWorkspace.XMLHash2SDEWorkspace Exception: " + ex.Message + "\n\nStackTrace: " + ex.StackTrace);
                    }
                    finally
                    {
                        if (propertySet != null)
                        {
                            // Marshal.ReleaseComObject(propertySet);
                        }

                        if (workspaceFactory != null)
                        {
                            // Marshal.ReleaseComObject(workspaceFactory);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ArcZonaWorkspace.XMLHash2SDEWorkspace Exception: " + ex.Message + "\n\nStackTrace: " + ex.StackTrace);
                }
            }

            return workSpace;
        }

        /// <summary>
        /// Converts the entire property set into a single string
        /// </summary>
        /// <param name="ps">IPropertySet to convert</param>
        /// <returns>a single string</returns>
        private string GetPropertySetString(IPropertySet ps)
        {
            string propertySetString = null;
            StringBuilder sb = new StringBuilder();

            try
            {
                object propNames;
                object propValues;
                ps.GetAllProperties(out propNames, out propValues);

                object[] propNamesArray = (object[])propNames;
                object[] propValuesArray = (object[])propValues;

                Trace.WriteLine("IPropertySet Values: \n");
                for (int i = 0; i < propNamesArray.Length; i++)
                {
                    // Trace.WriteLine(propNames[i].ToString() + "=" + propValues[i].ToString());
                    sb.Append(("'" + (i + 1).ToString() + "." + propNamesArray[i] + "=" + propValuesArray[i] + "'\n"));
                }

                propertySetString = sb.ToString();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ArcZonaWorkspace.GetPropertySetString Exception: " + ex.Message + "\n\nStackTrace: " + ex.StackTrace);
                throw;
            }

            return propertySetString;
        }
        #endregion
    }
}

