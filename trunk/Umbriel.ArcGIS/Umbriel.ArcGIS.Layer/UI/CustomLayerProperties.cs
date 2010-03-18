// <copyright file="CustomLayerProperties.cs" company="Umbriel Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-08-17</date>
// <summary>CustomLayerProperties class file</summary>

namespace Umbriel.ArcGIS.Layer.UI
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using ESRI.ArcGIS.ADF.BaseClasses;
    using ESRI.ArcGIS.ADF.CATIDs;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.Controls;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Framework;
    using Umbriel.ArcGIS.Layer.Util;

    /// <summary>
    /// Command that works in ArcMap/Map/PageLayout, ArcScene/SceneControl
    /// or ArcGlobe/GlobeControl
    /// </summary>
    [Guid("3b464357-c391-4c04-9aa8-cfeb70a18378")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Umbriel.ArcGIS.Layer.CustomLayerProperties")]
    public sealed class CustomLayerProperties : BaseCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            GMxCommands.Register(regKey);
            MxCommands.Register(regKey);
            SxCommands.Register(regKey);
            ControlsCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            GMxCommands.Unregister(regKey);
            MxCommands.Unregister(regKey);
            SxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion

        private IHookHelper m_hookHelper = null;
        private IGlobeHookHelper m_globeHookHelper = null;
        private ISceneHookHelper m_sceneHookHelper = null;
        private CustomLayerPropertiesForm propertySetForm = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomLayerProperties"/> class.
        /// </summary>
        public CustomLayerProperties()
        {
            base.m_category = "Umbriel"; // localizable text
            base.m_caption = "Customize Layer Properties";  // localizable text
            base.m_message = "This should work in ArcMap/MapControl/PageLayoutControl, " +
                             "ArcScene/SceneControl, ArcGlobe/GlobeControl";   // localizable text 
            base.m_toolTip = "Customize Layer Properties";  // localizable text 
            base.m_name = "Umbriel_CustomLayerProperties";   // unique id, non-localizable (e.g. "MyCategory_MyCommand")

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
            {
                return;
            }

            // Test the hook that calls this command and disable if nothing is valid
            try
            {
                m_hookHelper = new HookHelperClass();
                m_hookHelper.Hook = hook;
                if (m_hookHelper.ActiveView == null)
                {
                    m_hookHelper = null;
                }
            }
            catch
            {
                m_hookHelper = null;
            }

            if (m_hookHelper == null)
            {
                // Can be scene or globe
                try
                {
                    m_sceneHookHelper = new SceneHookHelperClass();
                    m_sceneHookHelper.Hook = hook;
                    if (m_sceneHookHelper.ActiveViewer == null)
                    {
                        m_sceneHookHelper = null;
                    }
                }
                catch
                {
                    m_sceneHookHelper = null;
                }

                if (m_sceneHookHelper == null)
                {
                    // Can be globe
                    try
                    {
                        m_globeHookHelper = new GlobeHookHelperClass();
                        m_globeHookHelper.Hook = hook;
                        if (m_globeHookHelper.ActiveViewer == null)
                        {
                            m_globeHookHelper = null;
                        }
                    }
                    catch
                    {
                        m_globeHookHelper = null;
                    }
                }
            }

            if (m_globeHookHelper == null && m_sceneHookHelper == null && m_hookHelper == null)
            {
                base.m_enabled = false;
            }
            else
            {
                base.m_enabled = true;
            }            
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            if (m_hookHelper != null)
            {
                try
                {
                    IApplication appArcMap = (IApplication)m_hookHelper.Hook;
                    IMxDocument doc = (IMxDocument)appArcMap.Document;

                    if (doc.SelectedLayer != null)
                    {
                        ILayer layer = doc.SelectedLayer;
                        IPropertySet propertySet = null;

                        bool newPropertySet = false;

                        ILayerExtensions layerExtensions = (ILayerExtensions)layer;

                        Trace.WriteLine(layerExtensions.ExtensionCount.ToString());

                        if (!LayerExtHelper.UmbrielPropertySetExists(layerExtensions))
                        {
                            propertySet = LayerExtHelper.CreateUmbrielPropertySet();
                            newPropertySet = true;
                        }
                        else
                        {
                            propertySet = LayerExtHelper.GetUmbrielPropertySet(layerExtensions);
                        }

                        if (propertySet != null)
                        {
                            if (this.propertySetForm == null)
                            {
                                this.propertySetForm = new CustomLayerPropertiesForm(propertySet);
                            }

                            this.propertySetForm.ShowDialog();

                            if (newPropertySet)
                            {
                                layerExtensions.AddExtension(propertySet);
                            }

                            this.propertySetForm.Dispose();
                            this.propertySetForm = null;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No layer highlighted in the table of contents.", "Ooops.", MessageBoxButtons.OK);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "Umbriel Custom Layer Properties Error", System.Windows.Forms.MessageBoxButtons.OK);
                }
            }
            else if (m_sceneHookHelper != null)
            {
                throw new NotImplementedException();
            }
            else if (m_globeHookHelper != null)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
