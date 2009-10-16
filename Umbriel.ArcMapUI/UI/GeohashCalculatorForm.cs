

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
    using ESRI.ArcGIS.Editor;

    public partial class GeohashCalculatorForm : Form
    {
        private IApplication ArcMapApplication { get; set; }
        private IFeatureLayer FeatureLayer { get; set; }
        public IMxDocument MxDocument { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeohashCalculatorForm"/> class.
        /// </summary>
        public GeohashCalculatorForm()
        {
            labelLayerName.Text = string.Empty;
            InitializeComponent();
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

            LoadFeatureLayer();

        }

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            IFeatureClass featureClass = this.FeatureLayer.FeatureClass;
            IDataset dataset = (IDataset)featureClass;
            IDatasetEdit datasetEdit = (IDatasetEdit)dataset;
            
            IVersionedObject versionedObject = (IVersionedObject)dataset;
            IEditor editor = (IEditor)this.ArcMapApplication.FindExtensionByName("ESRI Object Editor");

            bool versionedWorkspace = versionedObject.IsRegisteredAsVersioned;

            if (versionedObject.IsRegisteredAsVersioned
                && editor.EditState.Equals(esriEditState.esriStateNotEditing)
                )
            {
                MessageBox.Show("You must be in an edit session when editing versioned feature classes.", "Geohash Calculator", MessageBoxButtons.OK);
            }
            else if (versionedObject.IsRegisteredAsVersioned
                && editor.EditState.Equals(esriEditState.esriStateEditing)
                && !datasetEdit.IsBeingEdited()
                )
            {
                MessageBox.Show("You must be editing the same workspace as the highlighted layer.", "Geohash Calculator", MessageBoxButtons.OK);
            }
            else
            {
                Stack<int> objectIDs = GetObjectIds();

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
                        }

                        toolStripStatusLabel.Text = "-- Calculate Finished --";
                    }
                }
                else
                {
                    MessageBox.Show("There are no features to calculate.", "Geohash Calculator", MessageBoxButtons.OK);
                }
            }   
        }

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
                ReadAttributes();
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

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
