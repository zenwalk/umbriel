// <copyright file="AddPhotoPointForm.cs" company="Umbriel Project">
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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.DataSourcesGDB;
    using ESRI.ArcGIS.Display;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Framework;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;
    using Umbriel.ArcGIS.Geodatabase;

    /// <summary>
    /// Form for end-users to specify file paths for image files that contain 
    /// GPS and compass heading information to be added to the map.
    /// </summary>
    public partial class AddPhotoPointForm : Form
    {
        // TODO: restore form dimensions / column widths
        // TODO: enable/disable MakeLayer button based on grid contents
        // TODO: cleanup / manage scratch workspaces (or don't use them).
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AddPhotoPointForm"/> class.
        /// </summary>
        public AddPhotoPointForm()
        {
            InitializeComponent();
            this.SetTooltips();

            this.AllowDrop = true;
            this.PhotoTable = new GPSPhotoTable();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddPhotoPointForm"/> class.
        /// </summary>
        /// <param name="arcmapApp">The arcmap app.</param>
        public AddPhotoPointForm(IApplication arcmapApp)
        {
            InitializeComponent();
            this.SetTooltips();

            this.AllowDrop = true;

            // LoadSettings();
            this.ArcMapApplication = arcmapApp;
            this.MxDocument = (IMxDocument)arcmapApp.Document;

            this.PhotoTable = new GPSPhotoTable();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the mx document.
        /// </summary>
        /// <value>The mx document.</value>
        public IMxDocument MxDocument { get; set; }

        /// <summary>
        /// Gets or sets the photo table.
        /// </summary>
        /// <value>The photo table.</value>
        private GPSPhotoTable PhotoTable { get; set; }

        /// <summary>
        /// Gets or sets the arc map application.
        /// </summary>
        /// <value>The arc map application.</value>
        private IApplication ArcMapApplication { get; set; }
        #endregion

        /// <summary>
        /// Handles the Click event of the buttonAddPhotos control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonAddPhotos_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = "Add Photo Files";

            string lastusedpath = Util.Settings.ReadSetting("AddPhotoPointLastUsedPath");

            if (!string.IsNullOrEmpty(lastusedpath))
            {
                openFileDialog.InitialDirectory = lastusedpath;
            }

            openFileDialog.CheckFileExists = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileNames.Length > 0)
                {
                    string path = System.IO.Path.GetDirectoryName(openFileDialog.FileNames[0]);
                    Util.Settings.WriteSetting("AddPhotoPointLastUsedPath", path);

                    this.AddPhotosToTable(openFileDialog.FileNames);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Sets the tooltips.
        /// </summary>
        private void SetTooltips()
        {
            ToolTip toolTip = new ToolTip();
            toolTip.ToolTipTitle = "Quick Add - Drag files onto this picture to add to map.";
            toolTip.SetToolTip(pictureBoxQuickAdd, "Files dropped on the green circle will be instantly added to the map with default symbology.");
        }

        /// <summary>
        /// Handles the DragEnter event of the AddPhotoPointForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
        private void AddPhotoPointForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        /// <summary>
        /// Handles the DragDrop event of the AddPhotoPointForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DragEventArgs"/> instance containing the event data.</param>
        private void AddPhotoPointForm_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                System.Drawing.Point pt = pictureBoxQuickAdd.PointToClient(new System.Drawing.Point(e.X, e.Y));

                if (pt.X >= pictureBoxQuickAdd.Location.X &&
                    pt.X <= (pictureBoxQuickAdd.Location.X + pictureBoxQuickAdd.Width) &&
                    pt.Y >= pictureBoxQuickAdd.Location.Y &&
                    pt.Y <= (pictureBoxQuickAdd.Location.Y + pictureBoxQuickAdd.Height))
                {
                    this.PhotoTable.Clear();

                    progressBar.Value = 0;
                    progressBar.Maximum = files.Length;

                    progressBar.Visible = true;

                    this.Refresh();

                    this.PhotoTable.ProgressEvent += new GPSPhotoReadProgressEventHandler(this.PhotoTable_ProgressEvent);

                    this.PhotoTable.AddPhotoFilePaths(files);

                    this.PhotoTable.ProgressEvent -= this.PhotoTable_ProgressEvent;
                    progressBar.Visible = false;

                    if (this.PhotoTable.Rows.Count > 0)
                    {
                        this.buttonMakeLayer_Click(this, null);
                        this.Close();
                    }
                }
                else
                {
                    this.AddPhotosToTable(files);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Adds the photos to list view.
        /// </summary>
        /// <param name="files">The files.</param>
        private void AddPhotosToTable(string[] files)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                progressBar.Value = 0;
                progressBar.Maximum = files.Length;

                progressBar.Visible = true;

                this.Refresh();

                this.PhotoTable.ProgressEvent += new GPSPhotoReadProgressEventHandler(this.PhotoTable_ProgressEvent);

                this.PhotoTable.AddPhotoFilePaths(files);

                this.PhotoTable.ProgressEvent -= this.PhotoTable_ProgressEvent;
                progressBar.Visible = false;

                dataGridView.DataSource = this.PhotoTable;
                this.SetColumnWidths();                
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Handles the ProgressEvent event of the PhotoTable control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Umbriel.ArcGIS.Geodatabase.ProgressEventArgs"/> instance containing the event data.</param>
        private void PhotoTable_ProgressEvent(object sender, ProgressEventArgs e)
        {
            progressBar.Value = e.Index;
        }

        /// <summary>
        /// Searches the list to see if the name already exists in the listview
        /// </summary>
        /// <param name="listView">The list view reference</param>
        /// <param name="itemName">Name of the item</param>
        /// <returns>boolean true if name exists in listview</returns>
        private bool NameExistsInList(ref ListView listView, string itemName)
        {
            bool exists = false;

            foreach (ListViewItem item in listView.Items)
            {
                if (item.Name.Equals(itemName))
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }

        /// <summary>
        /// Handles the Click event of the buttonRemoveAll control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonRemoveAll_Click(object sender, EventArgs e)
        {
            if (this.PhotoTable != null)
            {
                this.PhotoTable.Rows.Clear();
            }
        }

        /// <summary>
        /// Handles the FormClosing event of the AddPhotoPointForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        private void AddPhotoPointForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // TODO: save form dimensions / column widths
            List<string> columnsWidths = new List<string>();

            for (int i = 0; i < dataGridView.ColumnCount; i++)
            {
                columnsWidths.Add(dataGridView.Columns[i].Width.ToString());
            }

            Util.Settings.WriteSetting("AddPhotoPointColumnWidths", string.Join(",", columnsWidths.ToArray()));

            ////string value = string.Empty;

            ////for (int i = 0; i < listViewFiles.Columns.Count; i++)
            ////{
            ////    if (i > 0)
            ////    {
            ////        value += ',';
            ////    }

            ////    value += listViewFiles.Columns[i].Width.ToString();
            ////}

            ////Debug.WriteLine("AddPhotoPointForm_FormClosing: " + value);

            ////Properties.Settings.Default.AddPhotoPointColumnWidths = value;
            ////Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Handles the Click event of the buttonMakeLayer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonMakeLayer_Click(object sender, EventArgs e)
        {
            IFeatureClass featureClass = this.PhotoTable.ToFeatureClass();

            if (this.MxDocument != null && featureClass != null)
            {
                IFeatureLayer layer = new FeatureLayerClass();
                layer.Name = "GPS Photos    -     " + DateTime.Now.ToString();
                layer.FeatureClass = featureClass;

                // set the hotlink to the FullPath field
                IHotlinkContainer hotlink = (IHotlinkContainer)layer;
                hotlink.HotlinkField = "FullPath";
                hotlink.HotlinkType = esriHyperlinkType.esriHyperlinkTypeURL;

                IHTMLPopupInfo htmlPopupInfo = (IHTMLPopupInfo)layer;
                htmlPopupInfo.HTMLPopupEnabled = true;

                string xsl = this.ReadDefaultXSLT();

                if (!string.IsNullOrEmpty(xsl))
                {
                    htmlPopupInfo.HTMLPresentationStyle = esriHTMLPopupStyle.esriHTMLPopupStyleXSLStylesheet;
                    htmlPopupInfo.HTMLXSLStylesheet = xsl;                    
                }
                else
                {
                    htmlPopupInfo.HTMLPresentationStyle = esriHTMLPopupStyle.esriHTMLPopupStyleTwoColumnTable;
                }

                IGeoFeatureLayer geoFeatureLayer = (IGeoFeatureLayer)layer;
                ISimpleRenderer renderer = new SimpleRendererClass();
                renderer.Label = "Photo Locations";
                renderer.Symbol = this.GetGPSPhotoSymbol();

                IRotationRenderer rotationRenderer = (IRotationRenderer)renderer;
                rotationRenderer.RotationField = "MagneticNorth";
                rotationRenderer.RotationType = esriSymbolRotationType.esriRotateSymbolGeographic;

                geoFeatureLayer.Renderer = (IFeatureRenderer)renderer;

                this.MxDocument.FocusMap.AddLayer((ILayer)layer);
                this.MxDocument.UpdateContents();

                this.Close();
            }
        }

        /// <summary>
        /// Gets the GPS photo symbol.
        /// </summary>
        /// <returns>ISymbol for GPS Locations</returns>
        private ISymbol GetGPSPhotoSymbol()
        {
            IMultiLayerMarkerSymbol multiLayerMarkerSymbol = new MultiLayerMarkerSymbolClass();

            ICharacterMarkerSymbol viewshedSymbol = this.GetGPSPhotoViewshedSymbol();
            multiLayerMarkerSymbol.AddLayer(viewshedSymbol);

            ICharacterMarkerSymbol charSymbol = new CharacterMarkerSymbol();

            // IFontDisp fontDisp = (IFontDisp)(new StdFont());
            IRgbColor rgbColor = new RgbColor();

            // Define the color we want to use
            rgbColor.Red = 255;
            rgbColor.Green = 0;
            rgbColor.Blue = 0;

            // Define the Font we want to use            
            charSymbol.Font = ESRI.ArcGIS.Utility.Converter.ToStdFont(new System.Drawing.Font("ESRI Default Marker", 30f));

            // Set the CharacterMarkerSymbols Properties
            charSymbol.Size = 8;
            charSymbol.Angle = 0;
            charSymbol.CharacterIndex = 34;
            charSymbol.Color = rgbColor;
            charSymbol.XOffset = 0;
            charSymbol.YOffset = 0;

            multiLayerMarkerSymbol.AddLayer(charSymbol);

            return (ISymbol)multiLayerMarkerSymbol;
        }

        /// <summary>
        /// Gets the GPS photo viewshed symbol.
        /// </summary>
        /// <returns>ICharacterMarkerSymbol for GPS Photo</returns>
        private ICharacterMarkerSymbol GetGPSPhotoViewshedSymbol()
        {
            ICharacterMarkerSymbol charSymbol = new CharacterMarkerSymbol();

            // IFontDisp fontDisp = (IFontDisp)(new StdFont());
            IRgbColor rgbColor = new RgbColor();

            // Define the color we want to use
            rgbColor.Red = 255;
            rgbColor.Green = 0;
            rgbColor.Blue = 0;

            // Define the Font we want to use
            charSymbol.Font = ESRI.ArcGIS.Utility.Converter.ToStdFont(new System.Drawing.Font("ESRI Dimensioning", 30f));

            // Set the CharacterMarkerSymbols Properties
            charSymbol.Size = 30;
            charSymbol.Angle = 90;
            charSymbol.CharacterIndex = 45;
            charSymbol.Color = rgbColor;
            charSymbol.XOffset = 4;
            charSymbol.YOffset = 0;

            return charSymbol;
        }

        /// <summary>
        /// Sets the column widths.
        /// </summary>
        private void SetColumnWidths()
        {
            List<string> columnsWidths = new List<string>(
                Util.Settings.ReadSetting("AddPhotoPointColumnWidths").Split(','));

            if (columnsWidths.Count.Equals(dataGridView.ColumnCount))
            {
                for (int i = 0; i < dataGridView.ColumnCount; i++)
                {
                    dataGridView.Columns[i].Width = Convert.ToInt32(columnsWidths[i]);
                }
            }
            else
            {
                dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
        }

        /// <summary>
        /// Reads the default XSLT.
        /// </summary>
        /// <returns>XSL string </returns>
        private string ReadDefaultXSLT()
        {
            string xsl = string.Empty;
            string xsltpath = Util.Settings.AssemblyDirectory + @"\UI\DefaultGPSPhotoHTMLPopup.xslt";

            if (File.Exists(xsltpath))
            {
                TextReader reader = new StreamReader(xsltpath);
                xsl = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
            }

            return xsl;
        }

        private void pictureBoxQuickAdd_Click(object sender, EventArgs e)
        {
        }

        private void buttonMakeLayer_KeyDown(object sender, KeyEventArgs e)
        {
        }

        /// <summary>
        /// Handles the KeyDown event of the AddPhotoPointForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void AddPhotoPointForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.B)
            {
                this.PhotoTable.LoadPhotoIntoBLOB = !this.PhotoTable.LoadPhotoIntoBLOB;

                if (this.PhotoTable.LoadPhotoIntoBLOB)
                {
                    MessageBox.Show("Photo Load Enabled.", "Make Layer", MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Photo Load Disabled.", "Make Layer", MessageBoxButtons.OK);
                }
            }
        }
    }
}
