// <copyright file="GeohashID.cs" company="Umbriel Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>unknown-see svn</date>
// <summary>
////</summary>

namespace Umbriel.ArcMapUI
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using ESRI.ArcGIS.ADF.BaseClasses;
    using ESRI.ArcGIS.ADF.CATIDs;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.Display;
    using ESRI.ArcGIS.Framework;
    using ESRI.ArcGIS.Geometry;

    /// <summary>
    /// GeohashID ArcMap Tool
    /// </summary>
    [Guid("f488694b-8fc7-4b5f-8edd-c4d39fd832d1")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Umbriel.ArcMapUI.GeohashID")]
    public sealed class GeohashID : BaseTool
    {
        #region COM Registration Function(s)
        /// <summary>
        /// Registers the function.
        /// </summary>
        /// <param name="registerType">Type of the register.</param>
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

        /// <summary>
        /// ArcGIS Application (ArcMap)
        /// </summary>
        private IApplication m_application;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeohashID"/> class.
        /// </summary>
        public GeohashID()
        {
            base.m_category = "Umbriel"; 
            base.m_caption = "ID the geohash for the map location";  
            base.m_message = "Click on the map to generate a geohash for that location";  
            base.m_toolTip = "Click on the map to generate a geohash for that location.";  
            base.m_name = "Umbriel_GeohashID";   
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

            // Disable if it is not ArcMap
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
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {            
        }

        /// <summary>
        /// This method is called when a mouse button is pressed down, when this tool is active.
        /// </summary>
        /// <param name="Button">Specifies which mouse button is pressed; 1 for the left mouse button, 2 for the right mouse button, and 4 for the middle mouse button.</param>
        /// <param name="Shift">Specifies an integer corresponding to the state of the SHIFT (bit 0), CTRL (bit 1) and ALT (bit 2) keys. When none, some, or all of these keys are pressed none, some, or all the bits get set. These bits correspond to the values 1, 2, and 4, respectively. For example, if both SHIFT and ALT were pressed, Shift would be 5.</param>
        /// <param name="X">The X coordinate, in device units, of the location of the mouse event. See the OnMouseDown Event for more details.</param>
        /// <param name="Y">The Y coordinate, in device units, of the location of the mouse event. See the OnMouseDown Event for more details.</param>
        /// <remarks>Note to inheritors: Override the OnMouseDown method if you need to perform some action when the
        /// OnMouseDown is raised when the tool is active.
        /// </remarks>
        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {            
        }

        /// <summary>
        /// This method is called when the mouse is moved while a mouse button is pressed down, when this tool is active.
        /// </summary>
        /// <param name="Button">Specifies which mouse button is pressed while the mouse is moved; 1 for the left mouse button, 2 for the right mouse button, and 4 for the middle mouse button.</param>
        /// <param name="Shift">Specifies an integer corresponding to the state of the SHIFT (bit 0), CTRL (bit 1) and ALT (bit 2) keys. When none, some, or all of these keys are pressed none, some, or all the bits get set. These bits correspond to the values 1, 2, and 4, respectively. For example, if both SHIFT and ALT were pressed, Shift would be 5.</param>
        /// <param name="X">The X coordinate, in device units, of the location of the mouse event. See the OnMouseMove Event for more details.</param>
        /// <param name="Y">The Y coordinate, in device units, of the location of the mouse event. See the OnMouseMove Event for more details.</param>
        /// <remarks>Note to inheritors: Override the OnMouseMove method if you need to perform some action when the
        /// OnMouseMove event is raised when the tool is active.</remarks>
        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            string geohash = this.CalculateGeohash(X, Y);
            m_application.StatusBar.set_Message(0, Constants.GeohashMessageCaption + "  " + geohash);
        }

        /// <summary>
        /// This method is called when a mouse button is released, when this tool is active.
        /// </summary>
        /// <param name="Button">Specifies which mouse button is released; 1 for the left mouse button, 2 for the right mouse button, and 4 for the middle mouse button.</param>
        /// <param name="Shift">Specifies an integer corresponding to the state of the SHIFT (bit 0), CTRL (bit 1) and ALT (bit 2) keys. When none, some, or all of these keys are pressed none, some, or all the bits get set. These bits correspond to the values 1, 2, and 4, respectively. For example, if both SHIFT and ALT were pressed, Shift would be 5.</param>
        /// <param name="X">The X coordinate, in device units, of the location of the mouse event. See the OnMouseUp Event for more details.</param>
        /// <param name="Y">The Y coordinate, in device units, of the location of the mouse event. See the OnMouseUp Event for more details.</param>
        /// <remarks>Note to inheritors: Override the OnMouseUp method if you need to perform some action when the
        /// OnMouseUp event is raised when the tool is active.</remarks>
        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            string geohash = this.CalculateGeohash(X, Y);
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
