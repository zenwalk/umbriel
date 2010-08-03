// <copyright file="Utility.cs" company="Umbriel Project">
// Copyright (c) 2010 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-03-14</date>
// <summary>Utility class file </summary>

namespace Umbriel.ArcGIS.Geodatabase
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;
    using Umbriel.Extensions;
    
    /// <summary>
    /// Geodatabase Utility Class
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Creates an IPropertySet from parameters
        /// </summary>
        /// <param name="server">The name of the ArcGIS Server</param>
        /// <param name="instance">The name of the instance on the ArcGIS Server.  
        /// For Service connections, ESRI default is 5151.  If there are additional services,
        /// usually they will be 5152, 5153,etc.  
        /// For Direct Connects, the service connections vary.  Here's an example of one for SQL Server 2008: sde:sqlserver:ProdArcGISServer\SQL2008 </param>
        /// <param name="user">database user</param>
        /// <param name="password">user password</param>
        /// <param name="database">Database (don't need it for Oracle, you do need it for SQL Server...not sure about the other RDBMSs)</param>
        /// <param name="version">The version that you want to use.</param>
        /// <param name="authmode">DBMS is database authentication.  OSA is operating system.  SQL Server can have mixed.</param>
        /// <returns>Returns an IPropertySet</returns>
        public static IPropertySet ArcSDEConnPropSet(
            string server,
            string instance,
            string user,
            string password,
            string database,
            string version,
            string authmode)
        {
            try
            {
                IPropertySet propertSet = new PropertySetClass();

                if (server.Length > 0)
                {
                    propertSet.SetProperty("SERVER", server.Trim());
                }

                if (user.Length > 0)
                {
                    propertSet.SetProperty("USER", user.Trim());
                }

                if (instance.Length > 0)
                {
                    propertSet.SetProperty("INSTANCE", instance.Trim());
                }

                if (password.Length > 0)
                {
                    propertSet.SetProperty("PASSWORD", password.Trim());
                }

                if (database.Length > 0)
                {
                    propertSet.SetProperty("DATABASE", database.Trim());
                }

                if (version.Length > 0)
                {
                    propertSet.SetProperty("VERSION", version.Trim());
                }

                if (authmode.Length > 0)
                {
                    propertSet.SetProperty("AUTHENTICATION_MODE", authmode.Trim());
                }

                return propertSet;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("AZGDBUtil.ArcSDEConnPropSet  Exception: " + ex.Message + "\n\nStackTrace: " + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// parses the object class table name from the fully qualified object class name:
        /// </summary>
        /// <param name="objectClassName">Name of the object class.</param>
        /// <returns>
        /// A string containing only the object class name.  So and objectClassName of 'GIS.DBO.PARCELS' would be returned as PARCELS
        /// </returns>
        public static string ParseObjectClassName(string objectClassName)
        {
            return objectClassName.ParseObjectClassName();

            //try
            //{
            //    if (objectClassName.LastIndexOf('.') > 0)
            //    {
            //        return objectClassName.Substring(objectClassName.LastIndexOf('.') + 1).Trim();
            //    }
            //    else
            //    {
            //        // if there's no period, then return the objectClassName as-is
            //        return objectClassName;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Trace.WriteLine("ParseObjectClassName  Exception: " + ex.Message + "\n\nStackTrace: " + ex.StackTrace);
            //    throw;
            //}
        }
        
        /// <summary>
        /// Finds featureclass in workspace using featureclassName parameter and instr
        /// </summary>
        /// <param name="workspace">IWorkspace of the workspace to search</param>
        /// <param name="featureclassName">the name of the featureclass to search for</param>
        /// <returns>a list of all IFeatureClass matching the input parameters</returns>
        public static List<IFeatureClass> FindFeatureClass(IWorkspace workspace, string featureclassName)
        {
            return FindFeatureClass(workspace, featureclassName, false);
        }

        /// <summary>
        /// Finds featureclass in workspace with an exact match
        /// </summary>
        /// <param name="workspace">IWorkspace of the workspace to search</param>
        /// <param name="featureclassName">the name of the featureclass to search for</param>
        /// <param name="exactMatch">boolean indicating whether the search is for exactmatch or for the featureclass name to exist somewhere in the featureclass name</param>
        /// <returns>a list of all IFeatureClass matching the input parameters</returns>
        public static List<IFeatureClass> FindFeatureClass(IWorkspace workspace, string featureclassName, bool exactMatch)
        {
            List<IFeatureClass> listFeatureClasses = new List<IFeatureClass>();

            try
            {
                // search the featureclasses first:
                IEnumDataset enumDataset = (IEnumDataset)workspace.get_Datasets(esriDatasetType.esriDTFeatureClass);

                IDataset dataset = null;
                while ((dataset = enumDataset.Next()) != null)
                {
                    string[] objectNameTokens = dataset.Name.Split('.');
                    string objectName = objectNameTokens[objectNameTokens.Length - 1];

                    if (exactMatch)
                    {
                        if (objectName.Equals(featureclassName))
                        {
                            listFeatureClasses.Add((IFeatureClass)dataset);
                        }
                    }
                    else
                    {
                        if (objectName.IndexOf(featureclassName, StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            listFeatureClasses.Add((IFeatureClass)dataset);
                        }
                    }
                }

                // next, search inside Feature Datasets:
                enumDataset = (IEnumDataset)workspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                dataset = null;
                while ((dataset = enumDataset.Next()) != null)
                {
                    IFeatureDataset featureDataset = (IFeatureDataset)dataset;

                    IEnumDataset subDatasets = featureDataset.Subsets;
                    IDataset subDataset = null;
                    while ((subDataset = subDatasets.Next()) != null)
                    {
                        string[] objectNameTokens = subDataset.Name.Split('.');
                        string objectName = objectNameTokens[objectNameTokens.Length - 1];

                        if (exactMatch)
                        {
                            if (objectName.Equals(featureclassName))
                            {
                                listFeatureClasses.Add((IFeatureClass)subDataset);
                            }
                        }
                        else
                        {
                            if (objectName.IndexOf(featureclassName, StringComparison.CurrentCultureIgnoreCase) >= 0)
                            {
                                listFeatureClasses.Add((IFeatureClass)subDataset);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }

            return listFeatureClasses;
        }

        /// <summary>
        /// returns the closest IFeauture from a list of IFeatures to a point:
        /// </summary>
        /// <param name="pt">the point from which obtain the closest feature</param>
        /// <param name="objectList">The list of features to compare against...the return IFeature will be contained in this list</param>
        /// <returns>The closes IFeature on objectList</returns>
        public static IFeature GetClosestFeature(IPoint pt, IList<IFeature> objectList)
        {
            try
            {
                IFeature closestFeature = null;
                double featureDistance = -1;

                IProximityOperator proximity = (IProximityOperator)pt;

                foreach (IFeature feature in objectList)
                {
                    if (closestFeature == null)
                    {
                        closestFeature = feature;
                    }
                    else
                    {
                        double dist = proximity.ReturnDistance(feature.Shape);

                        if ((dist < featureDistance) || featureDistance.Equals(-1))
                        {
                            featureDistance = dist;
                            closestFeature = feature;
                        }
                    }

                    closestFeature = closestFeature ?? feature;
                }

                return closestFeature;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Creates a WGS84 spatial reference.
        /// </summary>
        /// <returns>ISpatialReference for WGS84 geo</returns>
        public static ISpatialReference GetWGS84SpatialReference()
        {
            SpatialReferenceEnvironment spatialReferenceEnv = new SpatialReferenceEnvironmentClass();

            IGeographicCoordinateSystem geoCS = spatialReferenceEnv.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
            ISpatialReference spatialReference = geoCS;
            spatialReference.SetFalseOriginAndUnits(-180, -90, 1000000);

            return spatialReference;
        }
    }
}
