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


namespace Umbriel.ArcMap.Addin.EditorTrack
{
    public class EditorTrackToggleButton : ESRI.ArcGIS.Desktop.AddIns.Button
    {
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

        public EditorTrackToggleButton()
        {
            System.Drawing.Bitmap offBitmap = new System.Drawing.Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Umbriel.ArcMap.Addin.EditorTrack.Images.EditorTrackToggleButton_Off.png"));
            System.Drawing.Bitmap onBitmap = new System.Drawing.Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Umbriel.ArcMap.Addin.EditorTrack.Images.EditorTrackToggleButton_On.png"));

            this.offBitmapPictureDisp = ESRI.ArcGIS.ADF.COMSupport.OLE.GetIPictureDispFromBitmap(offBitmap);
            this.onBitmapPictureDisp = ESRI.ArcGIS.ADF.COMSupport.OLE.GetIPictureDispFromBitmap(onBitmap);
        }

        protected override void OnClick()
        {
            EditorTrackExtension.extensionEnabled = !EditorTrackExtension.extensionEnabled;
            this.UpdateBitmap();
        }

        protected override void OnUpdate()
        {
        }

        /// <summary>
        /// Updates the buttons bitmap according to mode
        /// </summary>
        private void UpdateBitmap()
        {
            try
            {
                ICommandItem commandItem = ArcMap.Application.Document.CommandBars.Find("Umbriel_Project_Umbriel.ArcMap.Addin.EditorTrack_EditorTrackToggleButton", true, true);

                if (EditorTrackExtension.extensionEnabled)
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
