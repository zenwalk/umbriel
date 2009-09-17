using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;



namespace Umbriel.ArcMapUI
{
    /// <summary>
    /// Summary description for GeohashID.
    /// </summary>
    [Guid("f488694b-8fc7-4b5f-8edd-c4d39fd832d1")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Umbriel.ArcMapUI.GeohashID")]
    public sealed class GeohashID : BaseTool
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
        private const string  MessageCaption = "Geohash: ";
        private IApplication m_application;
        public GeohashID()
        {
            base.m_category = "Umbriel"; //localizable text 
            base.m_caption = "ID the geohash for the map location";  //localizable text 
            base.m_message = "Click on the map to generate a geohash for that location";  //localizable text
            base.m_toolTip = "Click on the map to generate a geohash for that location.";  //localizable text
            base.m_name = "Umbriel_GeohashID";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), GetType().Name + ".cur");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            m_application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            string geohash = CalculateGeohash(X, Y);
            m_application.StatusBar.set_Message(0, MessageCaption + "  " +  geohash);
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            string geohash = CalculateGeohash(X, Y);
            System.Diagnostics.Debug.WriteLine(geohash);

            IMxDocument doc = (IMxDocument)this.m_application.Document;
            IActiveView activeView = doc.ActiveView;
            IScreenDisplay screenDisplay = (IScreenDisplay)activeView.ScreenDisplay;

            IPoint point = screenDisplay.DisplayTransformation.ToMapPoint(X, Y);

            if (point != null)
            {
                GeohashIDForm form = new GeohashIDForm(point);
                form.Show();
            }
            




            //Clipboard.SetText(geohash);
        }

        /// <summary>
        /// Calculates the geohash.
        /// </summary>
        /// <param name="X">The X coordinate from Mouse</param>
        /// <param name="Y">The Y coordinate from Mouse</param>
        /// <returns></returns>
        private string CalculateGeohash(int X, int Y)
        {
            IMxDocument doc = (IMxDocument)this.m_application.Document;
            IActiveView activeView = doc.ActiveView;
            IScreenDisplay screenDisplay = (IScreenDisplay)activeView.ScreenDisplay;

            IPoint point = screenDisplay.DisplayTransformation.ToMapPoint(X, Y);

            string geohash = Umbriel.ArcMap.Editor.Util.Geohasher.CreateGeohash(point);

            return geohash;
        }
        #endregion
    }
}
