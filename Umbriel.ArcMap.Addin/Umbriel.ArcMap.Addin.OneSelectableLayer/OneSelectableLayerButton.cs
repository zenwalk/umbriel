// <copyright file="OneSelectableLayerButton.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-08-19</date>
// <summary>OneSelectableLayerButton class file</summary>

namespace Umbriel.ArcMap.Addin.OneSelectableLayer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using ESRI.ArcGIS.ADF.BaseClasses;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Framework;
    using ESRI.ArcGIS.SystemUI;

    /// <summary>
    /// AddIn button class that toggles mode forcing the highlighted layer to be the only selectable layer
    /// </summary>
    public class OneSelectableLayerButton : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        /// <summary>
        /// when true, only the highlighted/active layer should be selectable
        /// </summary>
        private bool forceOneSelectableLayerOn;

        /// <summary>
        /// PictureDisp for when the button is togged off
        /// </summary>
        private object offBitmapPictureDisp = null;

        /// <summary>
        /// PictureDisp for when the button is togged on
        /// </summary>
        private object onBitmapPictureDisp = null;

        /// <summary>
        /// boolean to help workaround an issue where the bitmaps can't be set in the constructor (crashing arcmap).
        /// </summary>
        private bool initBitmap = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneSelectableLayerButton"/> class.
        /// </summary>
        public OneSelectableLayerButton()
        {
            this.Checked = Properties.Settings.Default.OneSelectableLayerEnabled;
            this.forceOneSelectableLayerOn = this.Checked;

            System.Drawing.Bitmap offBitmap = new System.Drawing.Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Umbriel.ArcMap.Addin.OneSelectableLayer.Images.OneSelectableLayer_Off.bmp"));
            System.Drawing.Bitmap onBitmap = new System.Drawing.Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Umbriel.ArcMap.Addin.OneSelectableLayer.Images.OneSelectableLayer_On.bmp"));

            this.offBitmapPictureDisp = ESRI.ArcGIS.ADF.COMSupport.OLE.GetIPictureDispFromBitmap(offBitmap);
            this.onBitmapPictureDisp = ESRI.ArcGIS.ADF.COMSupport.OLE.GetIPictureDispFromBitmap(onBitmap);
        }

        /// <summary>
        /// Called when [click].
        /// </summary>
        protected override void OnClick()
        {
            ArcMap.Application.CurrentTool = null;

            this.forceOneSelectableLayerOn = !this.forceOneSelectableLayerOn;

            if (this.forceOneSelectableLayerOn)
            {
                this.SetActiveLayerSelectable();
            }

            this.UpdateBitmap();

            try
            {
                Properties.Settings.Default.OneSelectableLayerEnabled = this.forceOneSelectableLayerOn;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Called when [update].
        /// </summary>
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;

            this.Checked = this.forceOneSelectableLayerOn;

            if (this.forceOneSelectableLayerOn)
            {
                this.SetActiveLayerSelectable();
            }

            try
            {
                if (this.initBitmap)
                {
                    this.UpdateBitmap();
                    this.initBitmap = false;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// Sets the active layer selectable.
        /// </summary>
        private void SetActiveLayerSelectable()
        {
            IMxDocument doc = ArcMap.Application.Document as IMxDocument;

            if (doc.SelectedLayer != null && doc.SelectedLayer is IFeatureLayer)
            {
                this.SetAllLayersUnselectable();

                ((IFeatureLayer)doc.SelectedLayer).Selectable = true;
            }
        }

        /// <summary>
        /// Sets all layers unselectable.
        /// </summary>
        private void SetAllLayersUnselectable()
        {
            IMxDocument doc = ArcMap.Application.Document as IMxDocument;

            IEnumLayer layers = doc.FocusMap.get_Layers(null, true);

            ILayer layer = null;

            while ((layer = layers.Next()) != null)
            {
                if (layer is IFeatureLayer)
                {
                    ((IFeatureLayer)layer).Selectable = false;
                }
            }
        }

        /// <summary>
        /// Updates the buttons bitmap according to mode
        /// </summary>
        private void UpdateBitmap()
        {
            try
            {
                ICommandItem commandItem = ArcMap.Application.Document.CommandBars.Find("Umbriel_Project_Umbriel.ArcMap.Addin.OneSelectableLayer_OneSelectableLayerButton", true, true);

                if (this.forceOneSelectableLayerOn)
                {
                    commandItem.FaceID = this.onBitmapPictureDisp;
                    commandItem.Refresh();
                }
                else
                {
                    commandItem.FaceID = this.offBitmapPictureDisp;
                    commandItem.Refresh();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.StackTrace);
                throw;
            }
        }
    }
}
