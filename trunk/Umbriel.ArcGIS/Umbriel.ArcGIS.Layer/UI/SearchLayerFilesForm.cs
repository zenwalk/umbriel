

namespace Umbriel.ArcGIS.Layer.UI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SQLite;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.Framework;
    using Umbriel.ArcGIS.Layer.LayerFile;

    public partial class SearchLayerFilesForm : Form
    {
        private IApplication ArcMapApplication { get; set; }

        public IMxDocument MxDocument { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchLayerFilesForm"/> class.
        /// </summary>
        /// <param name="application">The application.</param>
        public SearchLayerFilesForm(IApplication application)
        {
            InitializeComponent();

            this.ArcMapApplication = application;
            this.MxDocument = (IMxDocument)application.Document;
        }

        /// <summary>
        /// Handles the Click event of the buttonRebuildIndex control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonRebuildIndex_Click(object sender, EventArgs e)
        {
            List<string> layerfileIndexSearchPaths = new List<string>();


            if (layerfileIndexSearchPaths.Count.Equals(0))
            {
                MessageBox.Show("There are no search paths specified in the Layer.Config", "Search Layer Files", MessageBoxButtons.OK);
            }
            else
            {
                Cursor.Current = Cursors.WaitCursor;
                
                List<string> paths = new List<string>();

                foreach (string path in layerfileIndexSearchPaths)
                {
                    paths.Add(path);
                }

                LayerfileIndexBuilder builder = new LayerfileIndexBuilder();

                builder.Progress += new ProgressEventHandler(RebuildProgressHandler);

                builder.BuildNewIndex(paths);

                builder.Progress -= RebuildProgressHandler;
                
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Rebuilds the progress handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="total">The total.</param>
        /// <param name="mesage">The mesage.</param>
        private void RebuildProgressHandler(object sender, int progress, int total, string mesage)
        {
            toolStripStatusLabel.Text = mesage + "  " + progress.ToString() + " of " + total.ToString() + "  ";
            toolStripProgressBar.Value = progress;
            toolStripProgressBar.Maximum = total;
            statusStrip.Refresh();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonApplyFilter_Click(object sender, EventArgs e)
        {
            ApplyKeywordFilter();
        }

        private void ApplyKeywordFilter()
        {
            if (!string.IsNullOrEmpty(comboBoxFilter.Text))
            {
                string[] words = comboBoxFilter.Text.Split(' ');

                StringBuilder criteria = new StringBuilder();
                int counter = 0;
                foreach (string  word in words)
                {
                    counter++;
                    if (counter > 1)
                    {
                        criteria.Append(" OR ");
                    }

                    criteria.Append(" (lyrname LIKE '%" + word + "%' OR lyrdescription  LIKE '%" + word + "%') ");
                    
                    Cursor.Current = Cursors.WaitCursor;

                    DataTable table = new DataTable();

                    List<string> paths = new List<string>();

                    LayerfileIndexBuilder builder = new LayerfileIndexBuilder();
                    string indexPath = builder.GetDefaultIndexFilePath();

                    if (System.IO.File.Exists(indexPath))
                    {
                        string connectionString = "Data Source=" + indexPath + ";Version=3;";

                        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                        {
                            connection.Open();

                            string sql = "SELECT lyrname, lyrdescription,lyrfilename,lyrfullpath,lyrrevision,lyrgid,daterecmodified  FROM layerfile " +
                                "WHERE " + criteria.ToString();
                            System.Diagnostics.Debug.WriteLine(sql);

                            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                            {
                                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                                {
                                    adapter.Fill(table);

                                    dataGridView.DataSource = table;
                                }
                            }
                            connection.Close();
                        }
                    }

                    Cursor.Current = Cursors.Default;                    
                }
            }
        }

        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter) && e.Control.Equals(false))
            {
                this.buttonApplyFilter_Click(null, null);
            }
            else if ((e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter) && e.Control.Equals(true))
            {
                if (dataGridView.Rows.Count > 0)
                {
                    AddRowToMap(0);
                }
            }
        }

        private void AddRowToMap(int rowIndex)
        {
            System.Diagnostics.Debug.WriteLine("AddFirstRowToMap");

            if (this.MxDocument != null)
            {
                    string path = dataGridView["lyrfullpath", rowIndex].Value.ToString();
                    ILayer layer = Layer.Util.LayerExtHelper.CreateLayer(path);

                    this.MxDocument.FocusMap.AddLayer(layer);

                    this.MxDocument.ActiveView.Refresh();                    
                }
            
            }
        

        private void dataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
             AddRowToMap(e.RowIndex);
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void splitContainer2_Panel2_Resize(object sender, EventArgs e)
        {

            Point pt = new Point(splitContainer2.Panel2.Right - (buttonClose.Width + 20),   10);
            buttonClose.Location = pt;

            pt = new Point(splitContainer2.Panel2.Left+ 20,  10);
            buttonRebuildIndex.Location = pt;
        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
