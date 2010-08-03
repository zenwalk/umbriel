using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ADF.BaseClasses;
using Umbriel.Extensions;

namespace Umbriel.ArcMapUI.UI
{
    /// <summary>
    /// Summary description for UmbrielArcMapToolbar.
    /// </summary>
    [Guid("ccd2a104-c083-428f-9e9b-e2cafcbf0977")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Umbriel.ArcMapUI.UI.UmbrielArcMapToolbar")]
    public sealed class UmbrielArcMapToolbar : BaseToolbar
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommandBars.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommandBars.Unregister(regKey);
        }

        #endregion
        #endregion

        public UmbrielArcMapToolbar()
        {
            System.Diagnostics.Trace.WriteLine("Toolbar Constructor");
            string fullPath = AssemblyDirectory + @"\UmbrielArcMapToolbar.sqlite";
            string connectionstring = "Data Source={0};Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(string.Format(connectionstring, fullPath)))
            {
                string sql = "select ProgID,SubType FROM CommandBarItems Order By Sequence";
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    connection.Open();

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string progid = reader[0].ToString();

                            if (progid.Equals("Separator", StringComparison.CurrentCultureIgnoreCase))
                            {
                                BeginGroup(); 
                            }
                            else
                            {
                                if (!reader[1].Equals(System.DBNull.Value)
                                   && reader[1].ToString().IsNumeric())
                                {
                                    int subtype = Convert.ToInt32(reader[1].ToString());
                                    AddItem(progid, subtype);
                                }
                                else
                                {
                                    AddItem(progid);
                                }
                            }
                        }
                    }
                    connection.Close();
                }
            }


            ////
            //// TODO: Define your toolbar here by adding items
            ////
            //AddItem("esriArcMapUI.ZoomInTool");
            //BeginGroup(); //Separator
            //AddItem("{FBF8C3FB-0480-11D2-8D21-080009EE4E51}", 1); //undo command
            //AddItem(new Guid("FBF8C3FB-0480-11D2-8D21-080009EE4E51"), 2); //redo command
        }

        public override string Caption
        {
            get
            {
                return "Umbriel ArcMap Toolbar";
            }
        }
        public override string Name
        {
            get
            {
                return "UmbrielArcMapToolbar";
            }
        }

        static private  string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

    }
}