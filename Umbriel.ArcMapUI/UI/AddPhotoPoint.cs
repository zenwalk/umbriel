// <copyright file="AddPhotoPoint.cs" company="Umbriel Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-09-30</date>
// <summary>
////</summary>

namespace Umbriel.ArcMapUI.UI
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using ESRI.ArcGIS.ADF.BaseClasses;
    using ESRI.ArcGIS.ADF.CATIDs;
    using ESRI.ArcGIS.Framework;
    using ESRI.ArcGIS.ArcMapUI;

    /// <summary>
    /// Summary description for AddPhotoPoint.
    /// </summary>
    [Guid("673bf48e-e5c1-462b-afd4-c8807e779a4f")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Umbriel.ArcMapUI.UI.AddPhotoPoint")]
    public sealed class AddPhotoPoint : BaseCommand
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

        private AddPhotoPointForm AddPhotoForm { get; set; }

        public AddPhotoPoint()
        {
            base.m_category = "Umbriel"; //localizable text 
            base.m_caption = "Add a photo point using the GPS coordinate in EXIF Header";  //localizable text 
            base.m_message = "Add a photo point using the GPS coordinate in EXIF Header";  //localizable text
            base.m_toolTip = "Add a photo point using the GPS coordinate in EXIF Header";  //localizable text
            base.m_name = "Umbriel_AddPhotoPoint";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")

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
            if (this.AddPhotoForm == null)
            {
               this.AddPhotoForm  = new AddPhotoPointForm(); 
           }
            
            this.AddPhotoForm.Show();
        }

        #endregion
    }
}
