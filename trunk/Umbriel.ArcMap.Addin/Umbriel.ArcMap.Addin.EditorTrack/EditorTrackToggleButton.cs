using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Diagnostics;
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
            Trace.WriteLine("EditorTrackToggleButton CTOR");


            System.Drawing.Bitmap offBitmap = new System.Drawing.Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Umbriel.ArcMap.Addin.EditorTrack.Images.EditorTrackToggleButton_Off.png"));
            System.Drawing.Bitmap onBitmap = new System.Drawing.Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Umbriel.ArcMap.Addin.EditorTrack.Images.EditorTrackToggleButton_On.png"));

            this.offBitmapPictureDisp = ESRI.ArcGIS.ADF.COMSupport.OLE.GetIPictureDispFromBitmap(offBitmap);
            this.onBitmapPictureDisp = ESRI.ArcGIS.ADF.COMSupport.OLE.GetIPictureDispFromBitmap(onBitmap);
            
            

            Trace.WriteLine(string.Format(
    "EditorTrackToggleButton CTOR extensionEnabled = {0}",
    EditorTrackHelper.extensionEnabled));

            this.Checked = EditorTrackHelper.extensionEnabled;
        }

        
        protected override void OnClick()
        {
            EditorTrackHelper.extensionEnabled = !EditorTrackHelper.extensionEnabled;
            this.Checked = EditorTrackHelper.extensionEnabled;
            
            Trace.WriteLine(EditorTrackHelper.extensionEnabled.FormatObjectValue("EditorTrackHelper.extensionEnabled"));
            
            //EditorTrackHelper.extensionEnabled
            

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
            Trace.WriteLine("enter UpdateBitmap");
            try
            {
                ICommandItem commandItem = ArcMap.Application.Document.CommandBars.Find("Umbriel_Project_Umbriel.ArcMap.Addin.EditorTrack_EditorTrackToggleButton", true, true);

                if (EditorTrackHelper.extensionEnabled)
                {
                    Trace.WriteLine("Setting Enabled BitMap");
                    commandItem.FaceID = this.onBitmapPictureDisp;
                    commandItem.Refresh();
                    Trace.WriteLine("After Enabled BitMap Refresh.");
                }
                else
                {
                    Trace.WriteLine("Setting Disabled BitMap");
                    commandItem.FaceID = this.offBitmapPictureDisp;
                    commandItem.Refresh();
                    Trace.WriteLine("After Disabled BitMap Refresh.");
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.StackTrace);
                throw;
            }

            Trace.WriteLine("exit  UpdateBitmap");
        }



    }
}
