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
    using System.Drawing;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.Framework;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.DataSourcesGDB;
    using ESRI.ArcGIS.Geometry;
    

    /// <summary>
    /// Form for end-users to specify file paths for image files that contain 
    /// GPS and compass heading information to be added to the map.
    /// </summary>
    public partial class AddPhotoPointForm : Form
    {
        private IApplication ArcMapApplication { get; set; }
        public IMxDocument MxDocument { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddPhotoPointForm"/> class.
        /// </summary>
        public AddPhotoPointForm()
        {
            InitializeComponent();
            SetTooltips();
            InitializeListView();
            this.AllowDrop = true;

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddPhotoPointForm"/> class.
        /// </summary>
        /// <param name="arcmapApp">The arcmap app.</param>
        public AddPhotoPointForm(IApplication arcmapApp)
        {
            InitializeComponent();
            SetTooltips();
            InitializeListView();
            this.AllowDrop = true;

            // LoadSettings();

            this.ArcMapApplication = arcmapApp;
            this.MxDocument = (IMxDocument)arcmapApp.Document;
        }

        private void buttonAddPhotos_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = "Add Photo Files";
            if (!string.IsNullOrEmpty(Properties.Settings.Default.AddPhotoPointLastUsedPath))
            {
                openFileDialog.InitialDirectory = Properties.Settings.Default.AddPhotoPointLastUsedPath;
            }
            openFileDialog.CheckFileExists = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

                if (openFileDialog.FileNames.Length > 0)
                {
                    string path = System.IO.Path.GetDirectoryName(openFileDialog.FileNames[0]);

                    Properties.Settings.Default.AddPhotoPointLastUsedPath = path;
                    Properties.Settings.Default.Save();

                    AddPhotosToListView(openFileDialog.FileNames);
                }
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetTooltips()
        {
            ToolTip toolTip = new ToolTip();
            toolTip.ToolTipTitle = "Quick Add - Drag files onto this picture to add to map.";
            toolTip.SetToolTip(pictureBoxQuickAdd, "Files dropped on the green circle will be instantly added to the map with default symbology.");
        }

        /// <summary>
        /// Initializes the list view.
        /// </summary>
        private void InitializeListView()
        {


            int defaultWidth = -2;

            listViewFiles.Columns.Add("Name", defaultWidth
                , HorizontalAlignment.Left);

            listViewFiles.Columns.Add("Size",
                                 defaultWidth,
                                 HorizontalAlignment.Right);

            listViewFiles.Columns.Add("Type",
                                 defaultWidth,
                                 HorizontalAlignment.Left);

            listViewFiles.Columns.Add("Modified",
                                defaultWidth,
                                 HorizontalAlignment.Left);

            listViewFiles.Columns.Add("Latitude",
                                 defaultWidth,
                            HorizontalAlignment.Left);

            listViewFiles.Columns.Add("Longitude", defaultWidth, HorizontalAlignment.Left);

            listViewFiles.Columns.Add("Compass Direction", defaultWidth, HorizontalAlignment.Left);


            SetColumnWidths();



        }

        private void AddPhotoPointForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void AddPhotoPointForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            AddPhotosToListView(files);
        }


        /// <summary>
        /// Adds the photos to list view.
        /// </summary>
        /// <param name="files">The files.</param>
        private void AddPhotosToListView(string[] files)
        {
            this.Cursor = Cursors.WaitCursor;

            try
            {
                listViewFiles.BeginUpdate();

                foreach (string file in files)
                {
                    if (!NameExistsInList(ref listViewFiles, file))
                    {
                        ListViewItem item = new ListViewItem();
                        item.Name = file;
                        item.Text = System.IO.Path.GetFileName(file);

                        FileInfo fileInfo = new FileInfo(file);

                        double lengthKb = fileInfo.Length * 0.0009765625;

                        Umbriel.GIS.Photo.GeoPhoto geoPhoto = new Umbriel.GIS.Photo.GeoPhoto(file);

                        // item.SubItems.Add(item.Text);  // filename
                        item.SubItems.Add(lengthKb.ToString("0.##") + " Kb"); // Size
                        item.SubItems.Add(System.IO.Path.GetExtension(file)); // Type
                        item.SubItems.Add(fileInfo.LastWriteTime.ToString()); // Last Modified
                        item.SubItems.Add(geoPhoto.Coordinate.Latitude.ToString("0.#####")); // Latitude
                        item.SubItems.Add(geoPhoto.Coordinate.Longitude.ToString("0.#####")); // Longitude
                        item.SubItems.Add(geoPhoto.ImageDirection.ToString("0.#####")); // compass

                        listViewFiles.Items.Add(item);
                    }

                    System.Diagnostics.Debug.WriteLine(file);
                }

                listViewFiles.EndUpdate();

                if (string.IsNullOrEmpty(Properties.Settings.Default.AddPhotoPointColumnWidths))
                {
                    listViewFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                }
                else
                {
                    SetColumnWidths();
                }

                listViewFiles.Refresh();
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

        private void buttonRemoveAll_Click(object sender, EventArgs e)
        {
            listViewFiles.Items.Clear();
        }

        private void listViewFiles_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
        }

        private void AddPhotoPointForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string value = string.Empty;

            for (int i = 0; i < listViewFiles.Columns.Count; i++)
            {
                if (i > 0)
                {
                    value += ',';
                }

                value += listViewFiles.Columns[i].Width.ToString();
            }

            Debug.WriteLine("AddPhotoPointForm_FormClosing: " + value);

            Properties.Settings.Default.AddPhotoPointColumnWidths = value;
            Properties.Settings.Default.Save();
        }

        private void SetColumnWidths()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.AddPhotoPointColumnWidths))
            {
                string[] colWidths = Properties.Settings.Default.AddPhotoPointColumnWidths.Split(',');

                for (int i = 0; i < listViewFiles.Columns.Count; i++)
                {
                    listViewFiles.Columns[i].Width = Convert.ToInt32(colWidths[i]);
                }
            }
        }

        private void pictureBoxQuickAdd_Click(object sender, EventArgs e)
        {
            if (listViewFiles.Items.Count > 0
                && this.ArcMapApplication != null)
            {
                IScratchWorkspaceFactory scratchWorkspaceFactory = new ScratchWorkspaceFactoryClass();
                IWorkspace scratchWorkspace = scratchWorkspaceFactory.CreateNewScratchWorkspace();
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)scratchWorkspace;

                ISpatialReference spatialReference = Umbriel.GIS.GeodatabaseHelper.GetWGS84SpatialReference();

                 IFeatureClass featureclass = featureWorkspace.CreateFeatureClass(
                this.GetUniqueFeatureClassName(),
                null ,
                null,
                null,
                 esriFeatureType.esriFTSimple,
                 "SHAPE",
                 string.Empty
                );
                

            }
        }

        private string GetUniqueFeatureClassName()
        {
            return "Photo" + DateTime.Now.Ticks.ToString();
        }

        private IFields CreateIFields(ISpatialReference spatialReference)
        {
            // Create fields collection
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;

            // Create the geometry field
            IGeometryDef geometryDef = new GeometryDefClass();
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            
            // Assign Geometry Definition
            geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
            geometryDefEdit.GridCount_2 = 1;
            geometryDefEdit.set_GridSize(0, 0.5);
            geometryDefEdit.AvgNumPoints_2 = 2;
            geometryDefEdit.HasM_2 = false;
            geometryDefEdit.HasZ_2 = true;
            geometryDefEdit.SpatialReference_2 = spatialReference;

            // Create OID Field
            IField fieldOID = new FieldClass();
            IFieldEdit fieldEditOID = (IFieldEdit)fieldOID;
            fieldEditOID.Name_2 = "OBJECTID";
            fieldEditOID.AliasName_2 = "OBJECTID";
            fieldEditOID.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsEdit.AddField(fieldOID);

            // Create Geometry Field
            IField fieldShape = new FieldClass();
            IFieldEdit fieldEditShape = (IFieldEdit)fieldShape;
            fieldEditShape.Name_2 = "SHAPE";
            fieldEditShape.AliasName_2 = "SHAPE";
            fieldEditShape.Type_2 = esriFieldType.esriFieldTypeGeometry;
            fieldEditShape.GeometryDef_2 = geometryDef;
            fieldsEdit.AddField(fieldShape);



            return fields;
        }


    }
}
