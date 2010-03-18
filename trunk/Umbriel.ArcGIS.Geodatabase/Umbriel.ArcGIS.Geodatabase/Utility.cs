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
    using System.Text;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.DataSourcesGDB;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Geodatabase;

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
            try
            {
                if (objectClassName.LastIndexOf('.') > 0)
                {
                    return objectClassName.Substring(objectClassName.LastIndexOf('.') + 1).Trim();
                }
                else
                {
                    // if there's no period, then return the objectClassName as-is
                    return objectClassName;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ParseObjectClassName  Exception: " + ex.Message + "\n\nStackTrace: " + ex.StackTrace);
                throw;
            }
        }
    }
}
