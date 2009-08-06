// <copyright file="BatchExtendForm.cs" company="Earth">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-08-06</date>
// <summary>BatchExtendForm class file</summary>

namespace Umbriel.ArcMap.Editor.UI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using System.Windows.Forms;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.Framework;

    public partial class BatchExtendForm : Form
    {
        private IApplication ArcMapApplication { get; set; }
        public IMxDocument MxDocument { get; set; }
        private List<IFeatureLayer> AvailableEditableFeatureLayers { get; set; }
        private Dictionary<int, IFeatureLayer> AvailableLineFeatureLayers { get; set; }

        public double SearchTolerance { get; private set; }

        public BatchExtendForm()
        {
            InitializeComponent();
        }

        public BatchExtendForm(IApplication arcmapApp)
        {
            InitializeComponent();

            SetTooltips();

            LoadSettings();

            this.textBoxSearchTolerance.Text = this.SearchTolerance.ToString();

            this.ArcMapApplication = arcmapApp;
            this.MxDocument = (IMxDocument)arcmapApp.Document;
            LoadFeatureLayers();
            LoadEditFeatureLayer();
        }

        private void BatchExtendForm_Load(object sender, EventArgs e)
        {

        }

        private void LoadFeatureLayers()
        {
            if (this.MxDocument != null)
            {
                List<string> layerNames;

                string layerNamestring = Properties.Settings.Default.BatchExtendFeatLyrNames;

                if (layerNamestring.Length > 0)
                {
                    string[] lyrNames = layerNamestring.Split('~');
                    layerNames = new List<string>(lyrNames);
                }
                else
                {
                    layerNames = new List<string>();
                }


                List<IFeatureLayer> layers = ArcZona.ArcGIS.Carto.LayerUtility.FindFeatureLayers(this.MxDocument.FocusMap);

                Dictionary<int, IFeatureLayer> lineLayers = new Dictionary<int, IFeatureLayer>();


                foreach (IFeatureLayer layer in layers)
                {
                    if (layer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryLine
                        || layer.FeatureClass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                    {
                        int i = checkedList.Items.Add(layer.Name, layerNames.Contains(layer.Name));
                        lineLayers.Add(i, layer);
                    }
                }


                this.AvailableLineFeatureLayers = lineLayers;
                ExtendButtonEnabled();

            }
        }

        private void LoadEditFeatureLayer()
        {
            comboBoxFeatureLayers.Items.Clear();

            if (this.MxDocument != null)
            {
                List<IFeatureLayer> layers = ArcZona.ArcGIS.Carto.LayerUtility.FindFeatureLayers(this.MxDocument.FocusMap);
                Umbriel.ArcMap.Editor.Util.EditorHelper.RemoveLayersWithNoSelections(ref layers);
                this.AvailableEditableFeatureLayers = layers;

                foreach (IFeatureLayer layer in layers)
                {
                    comboBoxFeatureLayers.Items.Add(layer.Name);
                }

                if (comboBoxFeatureLayers.Items.Count.Equals(1))
                {
                    comboBoxFeatureLayers.SelectedIndex = 0;
                }
            }

            ExtendButtonEnabled();

        }
        private void checkedList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ExtendButtonEnabled();
        }

        private void checkedList_ItemCheck(object sender, ItemCheckEventArgs e)
        {


        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxSearchTolerance_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBoxSearchTolerance_Validating(object sender, CancelEventArgs e)
        {
            double result;

            if (!double.TryParse(textBoxSearchTolerance.Text, out result))
            {
                e.Cancel = true;
                textBoxSearchTolerance.Text = this.SearchTolerance.ToString();
            }
            else
            {
                if (result <= 0)
                {
                    e.Cancel = true;
                    textBoxSearchTolerance.Text = this.SearchTolerance.ToString();
                }
            }
        }

        private void textBoxSearchTolerance_Validated(object sender, EventArgs e)
        {
            double result;

            if (double.TryParse(textBoxSearchTolerance.Text, out result))
            {
                this.SearchTolerance = result;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            SaveSettings();

            if (comboBoxFeatureLayers.SelectedIndex >= 0)
            {
                toolStripProgressBar.Visible = true;

                IFeatureLayer editFeatureLayer = this.AvailableEditableFeatureLayers[comboBoxFeatureLayers.SelectedIndex];

                List<IFeatureLayer> selectedLayers = new List<IFeatureLayer>();

                foreach (int i in checkedList.CheckedIndices)
                {
                    selectedLayers.Add(this.AvailableLineFeatureLayers[i]);
                }

                FeatureExtender featureExtender = new FeatureExtender(selectedLayers, this.ArcMapApplication);

                //wire the feature extender events
                featureExtender.AfterExtendFeaturesEvent += new AfterExtendFeaturesHandler(AfterBatchExtendFinishHandler);
                featureExtender.ExtendFeatureProgressEvent += new ExtendFeatureProgressHandler(BatchExtendProgress);
                featureExtender.ExtendMessageEvent += new ExtendMessageEventHandler(BatchExtendMessage);
                IFeatureSelection featureSelection = (IFeatureSelection)editFeatureLayer;

                featureExtender.Simulation = checkSimulation.Checked;
                featureExtender.SearchTolerance = this.SearchTolerance;

                //Extend all of the selected features
                featureExtender.Extend(featureSelection);


                //release feature extender events
                featureExtender.AfterExtendFeaturesEvent -= AfterBatchExtendFinishHandler;
                featureExtender.ExtendFeatureProgressEvent -= BatchExtendProgress;


                this.MxDocument.ActiveView.Refresh();
            }



        }

        private void SaveSettings()
        {

            Properties.Settings.Default.BatchExtendSearchTolerance = this.SearchTolerance;

            Properties.Settings.Default.BatchExtendSimulation = this.checkSimulation.Checked;

            StringBuilder layerNameSetting = new StringBuilder();

            int i = 0;
            foreach (object item in checkedList.CheckedItems)
            {
                if (item.ToString().Length > 0)
                {
                    i++;
                    if (i > 1)
                    {
                        layerNameSetting.Append(',');
                    }

                    layerNameSetting.Append(item.ToString());
                }
            }

            Properties.Settings.Default.BatchExtendFeatLyrNames = layerNameSetting.ToString();
            Properties.Settings.Default.Save();

        }

        private void LoadSettings()
        {
            this.checkSimulation.Checked = Properties.Settings.Default.BatchExtendSimulation;
            this.SearchTolerance = Properties.Settings.Default.BatchExtendSearchTolerance;
        }

        private void ExtendButtonEnabled()
        {
            buttonOK.Enabled = (checkedList.CheckedItems.Count > 0) && (comboBoxFeatureLayers.SelectedIndex >= 0);
        }

        private void SetTooltips()
        {
            ToolTip toolTip = new ToolTip();
            toolTip.ToolTipTitle = "Batch Extend Simulation Mode";
            toolTip.SetToolTip(checkSimulation, "When checked, the Batch Simulation will run but will not update the feature geometry.");

        }
        private void AfterBatchExtendFinishHandler(int total, int extended, int notExtended)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("Batch Extend Results: ");
            message.AppendLine("Attempted to extend " + total.ToString() + " features.");
            message.AppendLine("\tSuccess: " + extended.ToString());
            message.AppendLine("\tFailed: " + notExtended.ToString());

            MessageBox.Show(message.ToString(), "Batch Extend", MessageBoxButtons.OK);

        }

        private void BatchExtendProgress(int currentItemIndex, int totalItems, string message)
        {

            if (toolStripProgressBar.Maximum != totalItems)
            {
                toolStripProgressBar.Maximum = totalItems;
            }

            toolStripProgressBar.Value = currentItemIndex;
            toolStripProgressBar.Visible = true;
        }

        private void BatchExtendMessage(string message)
        {
            toolStripStatusLabel.Text = message;
            statusStrip.Refresh();
           
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void statusStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripStatusLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
