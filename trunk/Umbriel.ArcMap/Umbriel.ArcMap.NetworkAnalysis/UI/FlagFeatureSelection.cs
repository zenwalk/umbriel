// <copyright file="FlagFeatureSelection.cs" company="Umbriel Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-08-05</date>
// <summary>BaseCommand class file for add flags from a feature selection</summary>

namespace Umbriel.ArcMap.NetworkAnalysis.UI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using ESRI.ArcGIS.ADF.BaseClasses;
    using ESRI.ArcGIS.ADF.CATIDs;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.Framework;

    /// <summary>
    /// FlagFeatureSelection BaseCommand for ArcMap
    /// </summary>
    [Guid("81d0b717-f1d0-4121-b5b2-35bfeaf6bdd5")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Umbriel.ArcMap.NetworkAnalysis.UI.FlagFeatureSelection")]
    public sealed class FlagFeatureSelection : BaseCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlagFeatureSelection"/> class.
        /// </summary>
        public FlagFeatureSelection()
        {
            base.m_category = "Umbriel";
            base.m_caption = "Add flags at selected feature locations.";
            base.m_message = "Add flags at selected feature locations.";
            base.m_toolTip = "Add flags at selected feature locations.";
            base.m_name = "Umbriel_FlagFeatureSelection";
            try
            {
                base.m_bitmap = Umbriel.ArcMap.NetworkAnalysis.Properties.Resources.flag_green;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Properties

        /// <summary>
        /// Gets or sets the MxDocument property
        /// </summary>
        /// <value>The IMxDocument of the arcmap MXD</value>
        private IMxDocument MxDocument { get; set; }

        /// <summary>
        /// Gets or sets IApplication reference to the ArcMap application.
        /// </summary>
        /// <value>ArcMap IApplication reference</value>
        private IApplication ArcMapApplication { get; set; }
        #endregion

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

            this.ArcMapApplication = hook as IApplication;

            this.MxDocument = (IMxDocument)this.ArcMapApplication.Document;

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
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            IMouseCursor cursor = new MouseCursorClass();
            cursor.SetCursor(2);

            DateTime start = DateTime.Now;

            IMap map = this.MxDocument.FocusMap;
            List<IFeatureLayer> layerList;

            try
            {
                layerList = ArcZona.ArcGIS.Carto.LayerUtility.FindFeatureLayers(map);

                // remove any layers that don't have a feature selection
                Util.NetworkAnalysisHelper.RemoveLayersWithNoSelections(ref layerList);

                // remove any layers that aren't point layers
                // TODO: construct UI for user to provide additional criteria for non-point layers
                Util.NetworkAnalysisHelper.RemoveLayersNonPointLayers(ref layerList);

                System.Diagnostics.Debug.WriteLine("layerList.count=" + layerList.Count.ToString());

                if (layerList.Count.Equals(0))
                {
                    MessageBox.Show("There are no layers with selected points.", "Flag Feature Selection Tool", MessageBoxButtons.OK);
                }
                else
                {
                    FlagFactory factory = new FlagFactory(this.ArcMapApplication);
                    factory.AddEdgeFlags(layerList);

                    TimeSpan elapsedTime = DateTime.Now.Subtract(start);

                    System.Diagnostics.Trace.WriteLine("Flags added in " + elapsedTime.TotalSeconds.ToString() + " seconds.");
                    this.MxDocument.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                cursor.SetCursor(0);
            }
        }

        #endregion

        #region COM Registration Function(s)
        /// <summary>
        /// Registers the function.
        /// </summary>
        /// <param name="registerType">Type of the register.</param>
        [ComRegisterFunction()]
        [ComVisible(false)]
        public static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);
        }

        /// <summary>
        /// Unregisters the function.
        /// </summary>
        /// <param name="registerType">Type of the register.</param>
        [ComUnregisterFunction()]
        [ComVisible(false)]
        public static void UnregisterFunction(Type registerType)
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
    }
}
