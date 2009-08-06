// <copyright file="BatchExtend.cs" company="Earth">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-08-06</date>
// <summary>Batch Extend class file</summary>

namespace Umbriel.ArcMap.Editor.UI
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using ESRI.ArcGIS.ADF.BaseClasses;
    using ESRI.ArcGIS.ADF.CATIDs;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Framework;

    /// <summary>
    /// Summary description for BatchExtend.
    /// </summary>
    [Guid("1c44c00e-1a75-4c16-aab3-c2e5b2a23a63")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Umbriel.ArcMap.Editor.UI.BatchExtend")]
    public sealed class BatchExtend : BaseCommand
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
            MxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private IApplication m_application;
        public BatchExtend()
        {
            base.m_category = "Umbriel"; 
            base.m_caption = "Extend selected lines to lines in another feature layer.";  
            base.m_message = "Extend selected lines to lines in another feature layer."; 
            base.m_toolTip = "Extend selected lines to lines in another feature layer.";  
            base.m_name = "Umbriel_BatchExtend";

            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            m_application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
            {
                base.m_enabled = true;
            }
            else
            {
                base.m_enabled = false;
            }
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add BatchExtend.OnClick implementation
            
            BatchExtendForm batchExtendForm = new BatchExtendForm(m_application);

            batchExtendForm.ShowDialog();
            
            batchExtendForm.Dispose();
        }

        #endregion
    }
}
