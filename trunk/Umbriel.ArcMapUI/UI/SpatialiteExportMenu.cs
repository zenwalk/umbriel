using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Framework;

namespace Umbriel.ArcMapUI.UI
{
    /// <summary>
    /// Summary description for SpatialiteExportMenu.
    /// </summary>
    [Guid("676bd976-9281-41b8-8ccf-cebe4929f5c7")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Umbriel.ArcMapUI.UI.SpatialiteExportMenu")]
    public sealed class SpatialiteExportMenu : BaseMenu, IShortcutMenu
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

        public SpatialiteExportMenu()
        {
            //
            // TODO: Define your menu here by adding items
            //
            //AddItem("esriArcMapUI.ZoomInFixedCommand");
            AddItem("{b3e174ea-24bb-4e06-b0c3-2a5f8e5a7a89}"); 
            BeginGroup(); //Separator
            AddItem("{FBF8C3FB-0480-11D2-8D21-080009EE4E51}", 1); //undo command
            AddItem(new Guid("FBF8C3FB-0480-11D2-8D21-080009EE4E51"), 2); //redo command
        }

        public override string Caption
        {
            get
            {
                //TODO: Replace bar caption
                return "Spatialite";
            }
        }
        public override string Name
        {
            get
            {
                return "SpatialiteExportMenu";
            }
        }
    }
}