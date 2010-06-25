// <copyright file="CustomLayerPropertiesForm.cs" company="Umbriel Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-08-17</date>
// <summary>CustomLayerPropertiesForm class file</summary>

namespace Umbriel.ArcGIS.Layer.UI
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Windows.Forms;
    using ESRI.ArcGIS.esriSystem;
    using Umbriel.ArcGIS.Layer.Util;
    using Umbriel.Extensions;
    using PropertyDictionary = System.Collections.Generic.Dictionary<string, string>;

    /// <summary>
    /// CustomLayerProperties Form to display and modify the name/values in a PropertySet
    /// </summary>
    public partial class CustomLayerPropertiesForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomLayerPropertiesForm"/> class.
        /// </summary>
        /// <param name="propertySet">IPropertySet reference</param>
        public CustomLayerPropertiesForm(IPropertySet propertySet)
        {
            InitializeComponent();
            this.PropertySet = propertySet;
            this.LoadDataGrid();
        }

        /// <summary>
        /// Gets or sets the IPropertySet property
        /// </summary>
        /// <value>The property set.</value>
        internal IPropertySet PropertySet { get; set; }

        /// <summary>
        /// Handles the Click event of the buttonClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Loads the data grid with the name/values in [this].PropertySet
        /// </summary>
        private void LoadDataGrid()
        {
            try
            {
                DataTable table = Layer.Util.LayerExtHelper.ToDataTable(this.PropertySet);

                DataView view = table.DefaultView;

                dataGridView.DataSource = view;

                dataGridView.Columns[0].Width = 150;
                dataGridView.Columns[1].Width = 340;

                dataGridView.Refresh();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Save();
        }

        /// <summary>
        /// Handles the CellValueChanged event of the dataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DataGridViewCellEventArgs"/> instance containing the event data.</param>
        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];
                string cellValue = row.Cells[0].Value.ToString();

                if (!string.IsNullOrEmpty(cellValue))
                {
                    if (cellValue.ToUpper().Contains("GUID"))
                    {
                        if (string.IsNullOrEmpty(row.Cells[1].Value.ToString()))
                        {
                            DataRow dataRow = ((DataRowView)row.DataBoundItem).Row;
                            dataRow[1] = System.Guid.NewGuid().ToString();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Handles the ColumnWidthChanged event of the dataGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DataGridViewColumnEventArgs"/> instance containing the event data.</param>
        private void dataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            // System.Diagnostics.Debug.WriteLine("Col: " + e.Column.Name + " Width=" + e.Column.Width.ToString());
        }

        /// <summary>
        /// Updates the property set with the datagridview
        /// </summary>
        private void Save()
        {
            try
            {
                DataView view = (DataView)dataGridView.DataSource;

                DataTable table = view.Table;

                this.PropertySet = table.ToPropertySet();

                this.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        private void splitContainerForm_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
