
namespace Umbriel.ArcGIS.Layer.SpatialiteLayer.UI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.DataSourcesGDB;
    using ESRI.ArcGIS.Framework;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;

    public partial class AddSpatialiteDataForm : Form
    {
        private IApplication ArcMapApplication { get; set; }
        private IMxDocument MxDocument { get; set; }

        public AddSpatialiteDataForm(IApplication application)
        {
            InitializeComponent();
            pictureBoxTarget.AllowDrop = true;

            this.ArcMapApplication = application;
            this.MxDocument = (IMxDocument)application.Document;
        }

        public AddSpatialiteDataForm()
        {
            InitializeComponent();
            pictureBoxTarget.AllowDrop = true;
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddSpatialiteDataForm_Load(object sender, EventArgs e)
        {

        }

        private void pictureBoxTarget_Click(object sender, EventArgs e)
        {
            // Display the treeview/results

        }

        private void pictureBoxTarget_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void pictureBoxTarget_DragDrop(object sender, DragEventArgs e)
        {
            pictureBoxTarget.Visible = true;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                Debug.WriteLine(file);
            }

            LoadFilesIntoTree(files);
        }

        private void AddSpatialiteDataForm_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
        }

        private void AddSpatialiteDataForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) Console.WriteLine(file);
        }

        private void pictureBoxTarget_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
        }

        private void LoadFilesIntoTree(string[] files)
        {
            foreach (string filePath in files)
            {
                LoadFileIntoTree(filePath);
            }
        }

        private void LoadFileIntoTree(string file)
        {
            try
            {
                TreeNode newDBNode = new TreeNode(file);
                newDBNode.Tag = file;
                treeViewCatalog.Nodes.Add(newDBNode);

                Loadtables(newDBNode);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        private void Loadtables(TreeNode node)
        {
            try
            {

                SpatialLiteDB db = new SpatialLiteDB(node.Tag.ToString());

                db.Open();

                List<SpatialiteTable> tabNames = db.TableNames();

                foreach (SpatialiteTable table in tabNames)
                {
                    TreeNode newDBNode = new TreeNode(table.TableName);

                    switch (table.GeometryType)
                    {
                        case "POINT":
                            newDBNode.ImageIndex = 1;
                            break;
                        case "LINESTRING":
                            newDBNode.ImageIndex = 2;
                            break;
                        case "MULTIPOLYGON":
                        case "POLYGON":
                            newDBNode.ImageIndex = 3;
                            break;
                        default:
                            newDBNode.ImageIndex = 0;
                            break;
                    }
                    newDBNode.SelectedImageIndex = newDBNode.ImageIndex;


                    newDBNode.Tag = node.Tag;
                    node.Nodes.Add(newDBNode);
                }

                db.Close();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        private void treeViewCatalog_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void treeViewCatalog_DragDrop(object sender, DragEventArgs e)
        {
            pictureBoxTarget.Visible = true;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            LoadFilesIntoTree(files);
        }


        private void AddSpatialiteDataForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.V)
            {
                if (Clipboard.ContainsFileDropList())
                {
                    System.Collections.Specialized.StringCollection filepaths = Clipboard.GetFileDropList();

                    foreach (string path in filepaths)
                    {
                        LoadFileIntoTree(path);
                    }
                }
                else if (Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText();
                    if (System.IO.File.Exists(text))
                    {
                        LoadFileIntoTree(text);
                    }
                }
            }
        }

        private void treeViewCatalog_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void treeViewCatalog_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode node = e.Node;

            if (node.ImageIndex >= 1 && node.ImageIndex <= 3)
            {
                Util.Converter converter = new Umbriel.ArcGIS.Layer.SpatialiteLayer.Util.Converter(node.Parent.Tag.ToString(), node.Text);
                IFeatureClass featureclass = converter.ConvertToFeatureclass();
                if (featureclass != null)
                {
                    AddLayer(featureclass);
                }
                else
                {
                    MessageBox.Show("There was a problem loading the " + node.Text + " table into ArcMap.", "Add Spatialite Data Error", MessageBoxButtons.OK);
                }
            }
        }


        private void AddLayer(IFeatureClass featureclass)
        {
            ILayer layer = new FeatureLayerClass();
            IDataset dataset = (IDataset)featureclass;
            layer.Name = dataset.Name;
            IFeatureLayer featureLayer = (IFeatureLayer)layer;
            featureLayer.FeatureClass = featureclass;
            
        }

    }
}
