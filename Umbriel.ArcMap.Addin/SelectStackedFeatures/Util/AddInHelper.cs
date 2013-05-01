// <copyright file="AddInHelper.cs" company="">
// Copyright (c) 2013 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>jay.cummins@worldviewsolutions.com</email>
// <date>2013-01-04</date>
// <summary>AddInHelper class file</summary>

namespace SelectStackedFeatures
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Framework;
    using Microsoft.Win32;
    using Carto = ESRI.ArcGIS.Carto;
    using StringDictionary = System.Collections.Generic.Dictionary<string, string>;

    /// <summary>
    /// AddIn Helper Class for AddIn User Settings
    /// </summary>
    public static class AddInHelper
    {
        /// <summary>
        /// log4net log
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The delegate used to display a message on pane 0
        /// </summary>
        public static Action<int, string> UpdateStatusbarMessageMethod;

        /// <summary>
        /// AddIn ID Dictionary
        /// </summary>
        private static StringDictionary addinIDs;

        /// <summary>
        /// Gets the AddIn IDs,
        /// key=ClassName value=ID within AddIn
        /// </summary>
        public static StringDictionary AddInIDs
        {
            get
            {
                if (addinIDs == null)
                {
                    PropertyInfo[] properties = typeof(ThisAddIn.IDs).GetProperties(BindingFlags.NonPublic | BindingFlags.Static);
                    addinIDs = new StringDictionary();
                    foreach (PropertyInfo property in properties)
                    {
                        addinIDs.Add(property.Name, property.GetValue(null, null).ToString());
                    }
                }

                return addinIDs;
            }
        }



        /// <summary>
        ///The method used to display a message on pane 0
        /// </summary>
        /// <param name="message">The message.</param>
        public static void UpdateStatusbarMessage(string message)
        {
            if (UpdateStatusbarMessageMethod != null)
            {
                UpdateStatusbarMessageMethod(0, message);
                System.Windows.Forms.Application.DoEvents();
            }
        }

        /// <summary>
        /// Gets the default user setting path.
        /// </summary>
        /// <returns>path to the user setting file</returns>
        public static string GetDefaultUserSettingPath()
        {
            return System.IO.Path.Combine(
               System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
               @"VDH.ArcMap.Toolset\User.config");

        }






        /// <summary>
        /// Gets the add in execute directory (the assembly cache directory)
        /// </summary>
        /// <returns>string path to the directory where the AddIn was unpacked</returns>
        public static string GetAddInExecuteDirectory()
        {
            string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return path;
        }

        /// <summary>
        /// Gets the add in folders.
        /// </summary>
        /// <returns></returns>
        public static string GetAddInFolders()
        {
            log.Debug("Enter");

            StringBuilder sb = new StringBuilder(System.Environment.NewLine);

            try
            {
                string keypath = @"Software\ESRI\Desktop10.0\Settings\AddInFolders";


                RegistryKey hklm = Registry.LocalMachine;

                RegistryKey key = hklm.OpenSubKey(keypath, false);

                if (key != null)
                {
                    if (key.ValueCount > 0)
                    {
                        foreach (var value in key.GetValueNames())
                        {
                            sb.AppendLine(value);
                        }

                    }
                    else
                    {
                        log.Debug("No Values");
                    }
                }
                else
                {
                    log.Debug(string.Format("No Key: {0}", keypath));
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }


            return sb.ToString();
        }




        /// <summary>
        /// Shows the dockable window (common progIDs: esriEditor.AttributionCommand for Feature inspector, esriEditor.CreateFeatureDockWin for create features window)
        /// </summary>
        /// <param name="progID">The prog ID.</param>
        public static void ShowDockableWindow(string progID)
        {
            //show the create feature template window:
            IDockableWindowManager dockableWindowManager = (IDockableWindowManager)ArcMap.Application;
            UID nuid = new UIDClass() { Value = progID };
            IDockableWindow window = dockableWindowManager.GetDockableWindow(nuid);
            window.Show(true);
        }

        /// <summary>
        /// Shows the feature attribute inspector.
        /// </summary>
        public static void ShowFeatureAttributeInspector()
        {
            log.Debug("Enter");

            string progID = "esriEditor.AttributionCommand";

            log.Debug(string.Format("progID={0}", progID));
            UID uid = new UIDClass();

            uid.Value = progID;

            log.Debug(string.Format("uid={0}", uid.ToString()));

            //Type t = Type.GetTypeFromProgID("esriFramework.AppRef");
            //object o = Activator.CreateInstance(t);

            try
            {
                //IApplication application = o as IApplication;
                ICommandBars commandBars = ArcMap.Application.Document.CommandBars;

                if (commandBars != null)
                {
                    bool norecurse = false;
                    bool nocreate = false;

                    log.Debug(string.Format("commandBars.Find.uid={0}", uid.Value.ToString()));
                    log.Debug(string.Format("commandBars.Find.noRecurse={0}", norecurse));
                    log.Debug(string.Format("commandBars.Find.noCreate={0}", nocreate));

                    ICommandItem commandItem = commandBars.Find(uid, norecurse, nocreate);

                    if (commandItem == null)
                    {
                        log.Error("commandItem returned null.");
                    }
                    else
                    {
                        commandItem.Execute();
                    }

                }
                else
                {
                    log.Error("ArcMap.Application.Document.CommandBars is null.");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);

                throw;
            }



        }

        /// <summary>
        /// Gets an item in a list using the property "Name".
        /// </summary>
        /// <typeparam name="T">The type of object</typeparam>
        /// <param name="list">The list of T</param>
        /// <param name="name">The property name value</param>
        /// <returns>single type T</returns>
        private static T GetByName<T>(List<T> list, string name)
        {
            //Trace.WriteLine(
            //         "Enter {0}".FormatString(MethodBase.GetCurrentMethod().Name));

            List<T> items = (from o in list
                             where typeof(T).GetProperty("Name").GetValue(o, null).Equals(name)
                             select o).ToList<T>();

            if (items.Count.Equals(1))
            {
                return items[0];
            }
            else
            {
                return default(T);
            }
        }
    }
}
