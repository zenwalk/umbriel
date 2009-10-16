using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace Umbriel.ArcMapUI.UI
{
    /// <summary>
    /// Summary description for GeohashCalculator.
    /// </summary>
    [Guid("5bae779c-f769-4733-8b4c-8c7e8400dc6b")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Umbriel.ArcMapUI.UI.GeohashCalculator")]
    public sealed class GeohashCalculator : BaseCommand
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
        public GeohashCalculator()
        {
            base.m_category = "Umbriel"; //localizable text 
            base.m_caption = "Calculates a field to the geohash of the shape geometry.";  //localizable text 
            base.m_message = "Calculates a field to the geohash of the shape geometry.";  //localizable text
            base.m_toolTip = "Calculates a field to the geohash of the shape geometry.";  //localizable text
            base.m_name = "Umbriel_GeohashCalculator";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")

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
                base.m_enabled = true;
            else
                base.m_enabled = false;


        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            IMxDocument doc = (IMxDocument)m_application.Document;
            if (doc.SelectedLayer != null)
            {
                if (doc.SelectedLayer is IFeatureLayer)
                {
                    IFeatureLayer layer = (IFeatureLayer)doc.SelectedLayer;

                    if (layer.FeatureClass.ShapeType.Equals(esriGeometryType.esriGeometryPoint))
                    {
                        GeohashCalculatorForm form = new GeohashCalculatorForm(this.m_application);
                        form.ShowDialog();
                        form.Dispose();
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Geohash calculator only works with point layers.", "Geohash Calculator", System.Windows.Forms.MessageBoxButtons.OK);
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("You must highlight a feature layer in the Table of Contents.", "Geohash Calculator", System.Windows.Forms.MessageBoxButtons.OK);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("You must highlight a feature layer in the Table of Contents.", "Geohash Calculator", System.Windows.Forms.MessageBoxButtons.OK);
            }
        }

        #endregion
    }
}
