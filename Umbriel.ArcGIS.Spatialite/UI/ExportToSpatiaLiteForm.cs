using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ArcMapUI;
using Umbriel.ArcGIS.Layer;

namespace Umbriel.ArcGIS.Spatialite.UI
{
    public partial class ExportToSpatiaLiteForm : Form
    {
        private bool isLoading;
        private bool isExporting;

        public event EventHandler<EventArgs>  StartExportingEvent = delegate { };
        public event EventHandler<EventArgs> StopExportingEvent = delegate { };
        
        public Dictionary<int,ILayer> ExportLayers { get; private set; }

        public ExportToSpatiaLiteForm()
        {
            InitializeComponent();
        }

        public ExportToSpatiaLiteForm(IMxDocument document)
        {
            InitializeComponent();
            this.MxDocument = document;
        }

        public IMxDocument MxDocument { get; private set; }

        private void ExportToSpatiaLiteForm_Load(object sender, EventArgs e)
        {
            isLoading = true;

            checkBoxExportCurrentExtent.Checked = Properties.Settings.Default.ExportSpatialite_OnlyCurrentExtent;
            checkBoxExportSelectedFeaturesOnly.Checked = Properties.Settings.Default.ExportSpatialite_OnlySelectedFeatures;
            checkBoxHighlightedLayer.Checked = Properties.Settings.Default.ExportSpatialite_HighlightChecked;

            this.StartExportingEvent += new EventHandler<EventArgs>(ExportToSpatiaLiteForm_StartExportingEvent);
            this.StopExportingEvent += new EventHandler<EventArgs>(ExportToSpatiaLiteForm_StopExportingEvent);

            isLoading = false;
            isExporting = false;

            ReadMapLayers();
        }

        private void ExportToSpatiaLiteForm_StopExportingEvent(object sender, EventArgs e)
        {
            this.treeViewLayers.Visible = true;

            labelLayerName.Visible = false;
            progressBarLayer.Visible = false;
            
            labelOperation.Visible = false;
            progressBarOperation.Visible = false;

            ResetProgressBars();
        }

        private void ExportToSpatiaLiteForm_StartExportingEvent(object sender, EventArgs e)
        {
            this.treeViewLayers.Visible = false;

            labelLayerName.Visible = true;
            progressBarLayer.Visible = true;

            labelOperation.Visible = true;
            progressBarOperation.Visible = true;
        }

        private void ResetProgressBars()
        {
            progressBarLayer.Value = progressBarLayer.Minimum;
            progressBarOperation.Value = progressBarOperation.Minimum;
        }

        private Dictionary<int, ILayer> ReadMapLayers()
        {
            Dictionary<int, ILayer> layers = ReadMapLayers(checkBoxExportSelectedFeaturesOnly.Checked, checkBoxExportCurrentExtent.Checked, checkBoxHighlightedLayer.Checked);

            this.ExportLayers = layers;

            LoadLayersIntoTreeview(layers);

            return ReadMapLayers(checkBoxExportSelectedFeaturesOnly.Checked, checkBoxExportCurrentExtent.Checked, checkBoxHighlightedLayer.Checked);
        }

        private Dictionary<int, ILayer> ReadMapLayers(bool onlySelectedFeatures, bool onlyCurrentExtent, bool highlightedLayerOnly)
        {
            List<ILayer> layers = null;

            if (highlightedLayerOnly)
            {
                if (this.MxDocument.SelectedLayer != null)
                {
                    layers = new List<ILayer>();
                    layers.Add(this.MxDocument.SelectedLayer);
                }
                else
                {
                    layers = this.MxDocument.SelectedItem.ToLayerList();
                }
            }
            else
            {

                layers = LayerHelper.GetLayerList(this.MxDocument.FocusMap);
            }

            Dictionary<int, ILayer> exportLayers = new Dictionary<int, ILayer>();

            IEnvelope extent = null;

            if (onlyCurrentExtent)
            {
                extent = this.MxDocument.ActiveView.Extent;
            }


            int i = 0;

            foreach (ILayer layer in layers)
            {
                if (layer is IFeatureLayer)
                {
                    IFeatureLayer featureLayer = (IFeatureLayer)layer;
                    IFeatureSelection featureSelection = (IFeatureSelection)featureLayer;

                    bool hasSelectedFeatures = (featureSelection.SelectionSet.Count > 0);

                    bool visibleInCurrentExtent = false;
                    bool selectedVisibleInCurrentExtent = false;

                    if (extent != null)
                    {
                        ISpatialFilter filter = new SpatialFilterClass();
                        filter.Geometry = extent;
                        filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                        visibleInCurrentExtent = featureLayer.FeatureClass.FeatureCount(filter) > 0;

                        ICursor cursor;
                        featureSelection.SelectionSet.Search(filter, true, out cursor);

                        int c = 0;
                        while (cursor.NextRow() != null)
                        {
                            c++;
                            break;
                        }
                        selectedVisibleInCurrentExtent = (c > 0);
                    }


                    if (onlySelectedFeatures & onlyCurrentExtent)
                    {
                        if (selectedVisibleInCurrentExtent)
                        {
                            exportLayers.Add(++i, layer);
                        }
                    }
                    else if (onlySelectedFeatures & !onlyCurrentExtent)
                    {
                        if (hasSelectedFeatures)
                        {
                            exportLayers.Add(++i, layer);
                        }
                    }
                    else if (!onlySelectedFeatures & onlyCurrentExtent)
                    {
                        if (visibleInCurrentExtent)
                        {
                            exportLayers.Add(++i, layer);
                        }
                    }
                    else
                    {
                        exportLayers.Add(++i, layer);
                    }
                }
            }

            return exportLayers;
        }

 
        private void LoadLayersIntoTreeview(Dictionary<int, ILayer> layers)
        {
            // treeViewLayers

            treeViewLayers.Nodes.Clear();
            TreeNode rootNode = new TreeNode(this.MxDocument.ActiveView.FocusMap.Name);

            treeViewLayers.Nodes.Add(rootNode);

            foreach (KeyValuePair<int, ILayer> kvp in layers)
            {
                TreeNode node = new TreeNode(kvp.Value.Name);
                node.Tag = kvp.Key;
                node.Checked = true;

                rootNode.Nodes.Add(node);
            }

            rootNode.Expand();


        }

        private void checkBoxHighlightedLayer_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading)
                ReadMapLayers();
        }

        private void checkBoxExportCurrentExtent_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading)
                ReadMapLayers();
        }

        private void checkBoxExportSelectedFeaturesOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoading)
                ReadMapLayers();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ExportToSpatiaLiteForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.ExportSpatialite_OnlyCurrentExtent = checkBoxExportCurrentExtent.Checked;
            Properties.Settings.Default.ExportSpatialite_OnlySelectedFeatures = checkBoxExportSelectedFeaturesOnly.Checked;
            Properties.Settings.Default.ExportSpatialite_HighlightChecked = checkBoxHighlightedLayer.Checked;
            Properties.Settings.Default.Save();


            this.StartExportingEvent -= ExportToSpatiaLiteForm_StartExportingEvent;
            this.StopExportingEvent -= ExportToSpatiaLiteForm_StopExportingEvent;

        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            isExporting = true;
            this.StartExportingEvent(this, new EventArgs());            

            try
            {
                string path = @"c:\temp\" + System.Guid.NewGuid().ToString() + ".spatialite";
                SpatialLiteDB.CreateNewDatabase(path);

                SpatialiteExporter exporter = new SpatialiteExporter(path);

                if (this.checkBoxHighlightedLayer.Checked)
                {
                    exporter.Extent =  this.MxDocument.ActiveView.Extent;
                }

                
                if (this.ExportLayers != null)
                {
                    progressBarLayer.Minimum = 0;
                    progressBarLayer.Maximum = this.ExportLayers.Count;

                    int k = 0;

                    foreach (KeyValuePair<int, ILayer> kvp in this.ExportLayers)
                    {
                        progressBarLayer.Value = ++k;
                        
                        labelLayerName.Text = string.Format("Exporting Layer: {0} ({1} of {2})", kvp.Value.Name, k, this.ExportLayers.Count);
                        labelLayerName.Refresh();

                        try
                        {
                            progressBarOperation.Value = 0;
                            progressBarOperation.Minimum = 0;

                            exporter.AttributeExportProgress += new SpatialiteExporter.ExportProgressChanged(exporter_AttributeExportProgress);
                            exporter.GeometryExportProgress += new SpatialiteExporter.ExportProgressChanged(exporter_GeometryExportProgress);
                            exporter.GeometryReadProgress += new SpatialiteExporter.ExportProgressChanged(exporter_GeometryReadProgress);
                            exporter.AttributeReadProgress += new SpatialiteExporter.ExportProgressChanged(exporter_AttributeReadProgress);

                            exporter.Export(kvp.Value);

                            exporter.AttributeExportProgress -= exporter_AttributeExportProgress;
                            exporter.GeometryExportProgress -= exporter_GeometryExportProgress;
                            exporter.GeometryReadProgress -= exporter_GeometryReadProgress;
                            exporter.AttributeReadProgress -= exporter_AttributeReadProgress;

                        }
                        catch (SQLiteException ex)
                        {
                            string message = "Sqlite/Spatialite Exception: \n\n{0}\n\n";

                            MessageBox.Show(string.Format(message, ex.Message),
                                "Export To Spatialite Exception",
                                MessageBoxButtons.OK);
                        }
                        catch (Exception)
                        {
                            throw;
                        }

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                isExporting = false;
                this.StopExportingEvent(this, new EventArgs());            
            }
        }

        void exporter_AttributeReadProgress(object sender, ExporterProgressEventArgs args)
        {
            labelOperation.Text = string.Format("Attribute read {0} of {1} complete.", args.Current, args.Total);

            progressBarOperation.Maximum = args.Total; 
            progressBarOperation.Value = args.Current;
            
        }

        void exporter_GeometryReadProgress(object sender, ExporterProgressEventArgs args)
        {
            labelOperation.Text = string.Format("Geometry read {0} of {1} complete.", args.Current, args.Total);
            labelOperation.Refresh();

            progressBarOperation.Maximum = args.Total; 
            progressBarOperation.Value = args.Current;            
        }

        void exporter_GeometryExportProgress(object sender, ExporterProgressEventArgs args)
        {
            labelOperation.Text = string.Format("Geometry export {0} of {1} complete.", args.Current, args.Total);

            progressBarOperation.Maximum = args.Total;
            progressBarOperation.Value = args.Current;           
        }

        void exporter_AttributeExportProgress(object sender, ExporterProgressEventArgs args)
        {
            labelOperation.Text = string.Format("Attribute export {0} of {1} complete.", args.Current, args.Total);

            progressBarOperation.Maximum = args.Total;
            progressBarOperation.Value = args.Current;           
        }
    }
}
