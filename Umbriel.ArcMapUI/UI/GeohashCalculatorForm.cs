// <copyright file="GeohashCalculatorForm.cs" company="Earth">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-10-16</date>
// <summary>
////</summary>

namespace Umbriel.ArcMapUI.UI
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.Editor;
    using ESRI.ArcGIS.Framework;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;

    /// <summary>
    /// Form for end-user to specify the field to calculate and other calculation options
    /// </summary>
    public partial class GeohashCalculatorForm : Form
    {
        #region Contructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeohashCalculatorForm"/> class.
        /// </summary>
        public GeohashCalculatorForm()
        {
            InitializeComponent();

            labelLayerName.Text = string.Empty;
            this.IsCalculating = false;
            this.KeyPreview = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeohashCalculatorForm"/> class.
        /// </summary>
        /// <param name="arcmapApp">The arcmap app.</param>
        public GeohashCalculatorForm(IApplication arcmapApp)
        {
            InitializeComponent();

            this.ArcMapApplication = arcmapApp;
            this.MxDocument = (IMxDocument)arcmapApp.Document;

            labelLayerName.Text = string.Empty;

            this.LoadFeatureLayer();
            this.IsCalculating = false;
            this.KeyPreview = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets IMxDocument reference
        /// </summary>
        /// <value>The IMxDocument reference </value>
        public IMxDocument MxDocument { get; set; }

        /// <summary>
        /// Gets or sets the arc map application refernece
        /// </summary>
        /// <value>IApplication reference</value>
        private IApplication ArcMapApplication { get; set; }

        /// <summary>
        /// Gets or sets the highlighted feature layer
        /// </summary>
        /// <value>IFeatureLayer on which to update a geohash field</value>
        private IFeatureLayer FeatureLayer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is calculating.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is calculating; otherwise, <c>false</c>.
        /// </value>
        private bool IsCalculating { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [cancel calculation].
        /// </summary>
        /// <value><c>true</c> if [cancel calculation]; otherwise, <c>false</c>.</value>
        private bool CancelCalculation { get; set; }

        #endregion

        /// <summary>
        /// Handles the Click event of the buttonCalculate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            IFeatureClass featureClass = this.FeatureLayer.FeatureClass;
            IDataset dataset = (IDataset)featureClass;
            IDatasetEdit datasetEdit = (IDatasetEdit)dataset;

            bool versionedWorkspace = false;

            IWorkspace workspace = dataset.Workspace;

            if (workspace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
            {
                IVersionedObject versionedObject = (IVersionedObject)dataset;
                versionedWorkspace =versionedObject.IsRegisteredAsVersioned;
            }

            IEditor editor = (IEditor)this.ArcMapApplication.FindExtensionByName("ESRI Object Editor");



            if (versionedWorkspace
                && editor.EditState.Equals(esriEditState.esriStateNotEditing))
            {
                MessageBox.Show("You must be in an edit session when editing versioned feature classes.", "Geohash Calculator", MessageBoxButtons.OK);
            }
            else if (versionedWorkspace
                && editor.EditState.Equals(esriEditState.esriStateEditing)
                && !datasetEdit.IsBeingEdited())
            {
                MessageBox.Show("You must be editing the same workspace as the highlighted layer.", "Geohash Calculator", MessageBoxButtons.OK);
            }
            else
            {
                Stack<int> objectIDs = this.GetObjectIds();

                if (objectIDs.Count > 0)
                {
                    int fieldIndex = featureClass.FindField(comboBoxFields.Text);

                    if (fieldIndex > -1)
                    {
                        int c = 0;

                        foreach (int objectID in objectIDs)
                        {
                            c++;
                            toolStripStatusLabel.Text = "Calculating   " + c.ToString() + " of " + objectIDs.Count.ToString() + " features.";
                            statusStrip.Refresh();

                            IFeature feature = featureClass.GetFeature(objectID);
                            IGeometry geometry = (IGeometry)feature.Shape;

                            string currentGeohashValue = "<Null>";

                            if (!System.DBNull.Value.Equals(feature.get_Value(fieldIndex)))
                            {
                                currentGeohashValue = feature.get_Value(fieldIndex).ToString();
                            }

                            if (geometry is IPoint)
                            {
                                IPoint point = (IPoint)geometry;
                                string geohash = Umbriel.ArcMap.Editor.Util.Geohasher.CreateGeohash(point, 13);

                                if (!currentGeohashValue.Equals(geohash))
                                {
                                    if (versionedWorkspace)
                                    {
                                        editor.StartOperation();
                                    }

                                    feature.set_Value(fieldIndex, geohash);
                                    feature.Store();

                                    if (versionedWorkspace)
                                    {
                                        editor.StopOperation("Update Geohash Field.");
                                    }
                                }
                                else
                                {
                                    toolStripStatusLabel.Text = "-- Same geohash. No Update Required --";
                                    this.statusStrip.Refresh();
                                }
                            }

                            Application.DoEvents();

                            if (this.CancelCalculation)
                            {
                                break;
                            }
                        }

                        if (this.CancelCalculation)
                        {
                            toolStripStatusLabel.Text = "-- Calculation Halted --";
                        }
                        else
                        {
                            toolStripStatusLabel.Text = "-- Calculate Finished --";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("There are no features to calculate.", "Geohash Calculator", MessageBoxButtons.OK);
                }
            }
        }

        /// <summary>
        /// Gets a stack of objectids to update
        /// </summary>
        /// <returns>integer stack of feature object ids</returns>
        private Stack<int> GetObjectIds()
        {
            Stack<int> objectIDs = new Stack<int>();
            IFeatureSelection featureSelection = (IFeatureSelection)this.FeatureLayer;

            if (checkBoxUseSelected.Checked && featureSelection.SelectionSet.Count > 0)
            {
                ISelectionSet selectionSet = featureSelection.SelectionSet;

                IEnumIDs enumObjectIDs = selectionSet.IDs;

                int i = -1;
                while ((i = enumObjectIDs.Next()) != -1)
                {
                    objectIDs.Push(i);
                }
            }
            else
            {
                IFeatureCursor cursor = this.FeatureLayer.FeatureClass.Search(null, true);
                IFeature feature = null;

                while ((feature = cursor.NextFeature()) != null)
                {
                    objectIDs.Push(feature.OID);
                }
            }

            return objectIDs;
        }

        /// <summary>
        /// Read in the selected feature layer and read in the fields
        /// </summary>
        private void LoadFeatureLayer()
        {
            if (this.MxDocument.SelectedLayer is IFeatureLayer)
            {
                this.FeatureLayer = (IFeatureLayer)this.MxDocument.SelectedLayer;
                labelLayerName.Text = this.FeatureLayer.Name;
                this.ReadAttributes();
            }
        }

        /// <summary>
        /// Reads the attributes from the feature layer
        /// </summary>
        private void ReadAttributes()
        {
            int geohashFieldIndex = -1;

            if (this.FeatureLayer != null)
            {
                IFields fields = this.FeatureLayer.FeatureClass.Fields;

                for (int i = 0; i < fields.FieldCount; i++)
                {
                    IField field = fields.get_Field(i);

                    if (field.Type.Equals(esriFieldType.esriFieldTypeString))
                    {
                        comboBoxFields.Items.Add(field.Name);

                        if (field.Name.IndexOf("geohash", StringComparison.CurrentCultureIgnoreCase) >= 0)
                        {
                            geohashFieldIndex = comboBoxFields.Items.Count - 1;
                        }
                    }
                }
            }

            if (geohashFieldIndex >= 0)
            {
                comboBoxFields.SelectedIndex = geohashFieldIndex;
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

        private void GeohashCalculatorForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)27)
            {
                this.CancelCalculation = true;
            }
        }
    }
}
