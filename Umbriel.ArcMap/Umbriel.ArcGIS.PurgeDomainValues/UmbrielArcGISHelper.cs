// <copyright file="UmbrielArcGISHelper.cs" company="Umbriel Project">
// Copyright (c) 2009 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <phone>202-905-2625</phone>
// <date>2009-09-21</date>
// <summary>SDEMeta Helper Class File
//   Revision History:
//   Name:             Date:                  Description:
//   JCummins      2009-09-21     initial creation
// </summary>

namespace Umbriel.ArcGIS
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using ArcZona.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Catalog;
    using ESRI.ArcGIS.DataSourcesGDB;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Geodatabase;

    public static class UmbrielArcGISHelper
    {
        /// <summary>
        /// Gets the workspace.
        /// </summary>
        /// <returns>IWorkspace for the geodatabase</returns>
        internal static IWorkspace GetWorkspace(CommandLine.Utility.CommandArguments argParser)
        {
            try
            {
                IWorkspace ws = null; // workspace interface

                // Get the workspace reference
                if (argParser.Contains("-g"))
                {
                    string gdbPath = argParser.GetValue("-g");

                    // test for personal geodatabase (access database)
                    if (gdbPath.Trim().ToLower().EndsWith("mdb"))
                    {
                        if (File.Exists(gdbPath))
                        {
                            IWorkspaceFactory workspaceFactory = new AccessWorkspaceFactoryClass();
                            ws = workspaceFactory.OpenFromFile(gdbPath, 0);
                        }
                        else
                        {
                            throw new FileNotFoundException("Missing geodatabase.", gdbPath);
                        }
                    }
                    else if (gdbPath.Trim().ToLower().EndsWith("gdb"))
                    {
                        // file geodatabase (directory ending in .gdb)
                        if (Directory.Exists(gdbPath))
                        {
                            IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactoryClass();
                            ws = workspaceFactory.OpenFromFile(gdbPath, 0);
                        }
                        else
                        {
                            throw new DirectoryNotFoundException("Missing geodatabase: " + gdbPath);
                        }
                    }
                }
                else
                {
                    // sde connection
                    string server = string.Empty;
                    string instance = string.Empty;
                    string username = string.Empty;
                    string passwrd = string.Empty;
                    string database = string.Empty;
                    string authmode = string.Empty;
                    string version = string.Empty;

                    if (argParser.Contains("-s"))
                    {
                        server = argParser.GetValue("-s");
                    }
                    else
                    {
                        server = Environment.GetEnvironmentVariable("SDESERVER");
                    }


                    if (argParser.Contains("-i"))
                    {
                        instance = argParser.GetValue("-i");
                    }
                    else
                    {
                        instance = Environment.GetEnvironmentVariable("SDEINSTANCE");
                    }

                    if (argParser.Contains("-u"))
                    {
                        username = argParser.GetValue("-u");
                    }
                    else
                    {
                        username = Environment.GetEnvironmentVariable("SDEUSER");
                    }

                    if (argParser.Contains("-p"))
                    {
                        passwrd = argParser.GetValue("-p");
                    }
                    else
                    {
                        passwrd = Environment.GetEnvironmentVariable("SDEPASSWORD");
                    }


                    if (argParser.Contains("-D"))
                    {
                        database = argParser.GetValue("-D");
                    }
                    else
                    {
                        database = Environment.GetEnvironmentVariable("SDEDATABASE");
                    }

                    if (argParser.Contains("-u") && argParser.Contains("-p"))
                    {
                        authmode = "DBMS";
                    }
                    else
                    {
                        authmode = "OSA";
                    }

                    if (argParser.Contains("-V"))
                    {
                        version = argParser.GetValue("-V");
                    }

                    string servInstance = string.Empty;
                    if (server.Length > 0)
                    {
                        servInstance = server + ":" + instance;
                    }
                    else
                    {
                        servInstance = instance;
                    }
                    
                    IPropertySet ps = AZGDBUtil.ArcSDEConnPropSet(server, instance, username, passwrd, database, version, authmode);
                    IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactoryClass();
                    try
                    {
                        ws = workspaceFactory.Open(ps, 0);
                    
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }

                return ws;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// Opens the object class.
        /// </summary>
        /// <param name="ws">The workspace.</param>
        /// <param name="objectclassName">Name of the objectclass.</param>
        /// <returns>IObjectClass reference</returns>
        internal static IObjectClass OpenObjectClass(IWorkspace ws, string objectclassName)
        {
            IObjectClass objectClass = null;

            objectClass = OpenFeatureClass(ws, objectclassName);

            if (objectClass == null)
            {
                objectClass = OpenTable(ws, objectclassName);
            }

            if (objectClass == null)
            {
                objectClass = OpenFeatureDataset(ws, objectclassName);
            }

            return objectClass;
        }

        /// <summary>
        /// Opens the feature class.
        /// </summary>
        /// <param name="ws">The workspace</param>
        /// <param name="featureclassName">Name of the featureclass.</param>
        /// <returns>IObjectClass reference</returns>
        private static IObjectClass OpenFeatureClass(IWorkspace ws, string featureclassName)
        {
            IObjectClass objectClass = null;
            IFeatureWorkspace fws = (IFeatureWorkspace)ws;

            // first, try to open it as a featureclass:
            try
            {
                IFeatureClass fc = fws.OpenFeatureClass(featureclassName);
                objectClass = (IObjectClass)fc;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

            return objectClass;
        }

        /// <summary>
        /// Opens the feature dataset.
        /// </summary>
        /// <param name="ws">The workspace</param>
        /// <param name="datasetName">Name of the dataset.</param>
        /// <returns>IObjectClass reference</returns>
        private static IObjectClass OpenFeatureDataset(IWorkspace ws, string datasetName)
        {
            IObjectClass objectClass = null;
            IFeatureWorkspace fws = (IFeatureWorkspace)ws;

            // first, try to open it as a featureclass:
            try
            {
                IFeatureDataset fd = fws.OpenFeatureDataset(datasetName);
                objectClass = (IObjectClass)fd;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

            return objectClass;
        }

        /// <summary>
        /// Opens the table.
        /// </summary>
        /// <param name="ws">The workspace</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>IObjectClass reference</returns>
        private static IObjectClass OpenTable(IWorkspace ws, string tableName)
        {
            IObjectClass objectClass = null;
            IFeatureWorkspace fws = (IFeatureWorkspace)ws;

            try
            {
                ITable table = fws.OpenTable(tableName);
                objectClass = (IObjectClass)table;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

            return objectClass;
        }

    }
}
