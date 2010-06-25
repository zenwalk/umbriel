namespace Umbriel.ArcGIS.Spatialite.UI
{
    partial class ExportToSpatiaLiteForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportToSpatiaLiteForm));
            this.checkBoxExportSelectedFeaturesOnly = new System.Windows.Forms.CheckBox();
            this.checkBoxExportCurrentExtent = new System.Windows.Forms.CheckBox();
            this.buttonExport = new System.Windows.Forms.Button();
            this.checkBoxHighlightedLayer = new System.Windows.Forms.CheckBox();
            this.treeViewLayers = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.labelLayerName = new System.Windows.Forms.Label();
            this.labelOperation = new System.Windows.Forms.Label();
            this.progressBarOperation = new System.Windows.Forms.ProgressBar();
            this.progressBarLayer = new System.Windows.Forms.ProgressBar();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxExportSelectedFeaturesOnly
            // 
            this.checkBoxExportSelectedFeaturesOnly.AutoSize = true;
            this.checkBoxExportSelectedFeaturesOnly.Checked = true;
            this.checkBoxExportSelectedFeaturesOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxExportSelectedFeaturesOnly.Location = new System.Drawing.Point(12, 9);
            this.checkBoxExportSelectedFeaturesOnly.Name = "checkBoxExportSelectedFeaturesOnly";
            this.checkBoxExportSelectedFeaturesOnly.Size = new System.Drawing.Size(206, 20);
            this.checkBoxExportSelectedFeaturesOnly.TabIndex = 4;
            this.checkBoxExportSelectedFeaturesOnly.Text = "Only Export Selected Features";
            this.checkBoxExportSelectedFeaturesOnly.UseVisualStyleBackColor = true;
            this.checkBoxExportSelectedFeaturesOnly.CheckedChanged += new System.EventHandler(this.checkBoxExportSelectedFeaturesOnly_CheckedChanged);
            // 
            // checkBoxExportCurrentExtent
            // 
            this.checkBoxExportCurrentExtent.AutoSize = true;
            this.checkBoxExportCurrentExtent.Checked = true;
            this.checkBoxExportCurrentExtent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxExportCurrentExtent.Location = new System.Drawing.Point(12, 46);
            this.checkBoxExportCurrentExtent.Name = "checkBoxExportCurrentExtent";
            this.checkBoxExportCurrentExtent.Size = new System.Drawing.Size(142, 20);
            this.checkBoxExportCurrentExtent.TabIndex = 5;
            this.checkBoxExportCurrentExtent.Text = "Only Current Extent";
            this.checkBoxExportCurrentExtent.UseVisualStyleBackColor = true;
            this.checkBoxExportCurrentExtent.CheckedChanged += new System.EventHandler(this.checkBoxExportCurrentExtent_CheckedChanged);
            // 
            // buttonExport
            // 
            this.buttonExport.Location = new System.Drawing.Point(12, 16);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(86, 31);
            this.buttonExport.TabIndex = 6;
            this.buttonExport.Text = "&Export";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // checkBoxHighlightedLayer
            // 
            this.checkBoxHighlightedLayer.AutoSize = true;
            this.checkBoxHighlightedLayer.Checked = true;
            this.checkBoxHighlightedLayer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHighlightedLayer.Location = new System.Drawing.Point(12, 84);
            this.checkBoxHighlightedLayer.Name = "checkBoxHighlightedLayer";
            this.checkBoxHighlightedLayer.Size = new System.Drawing.Size(134, 20);
            this.checkBoxHighlightedLayer.TabIndex = 7;
            this.checkBoxHighlightedLayer.Text = "Highlighted Layers";
            this.checkBoxHighlightedLayer.UseVisualStyleBackColor = true;
            this.checkBoxHighlightedLayer.CheckedChanged += new System.EventHandler(this.checkBoxHighlightedLayer_CheckedChanged);
            // 
            // treeViewLayers
            // 
            this.treeViewLayers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewLayers.Location = new System.Drawing.Point(0, 0);
            this.treeViewLayers.Name = "treeViewLayers";
            this.treeViewLayers.Size = new System.Drawing.Size(467, 271);
            this.treeViewLayers.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(186, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 16);
            this.label1.TabIndex = 9;
            this.label1.Text = "Export Preview";
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(373, 17);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(82, 30);
            this.buttonClose.TabIndex = 10;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxHighlightedLayer);
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxExportSelectedFeaturesOnly);
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxExportCurrentExtent);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(467, 475);
            this.splitContainer1.SplitterDistance = 126;
            this.splitContainer1.TabIndex = 11;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.labelLayerName);
            this.splitContainer2.Panel1.Controls.Add(this.labelOperation);
            this.splitContainer2.Panel1.Controls.Add(this.progressBarOperation);
            this.splitContainer2.Panel1.Controls.Add(this.progressBarLayer);
            this.splitContainer2.Panel1.Controls.Add(this.treeViewLayers);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.buttonClose);
            this.splitContainer2.Panel2.Controls.Add(this.buttonExport);
            this.splitContainer2.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer2_Panel2_Paint);
            this.splitContainer2.Size = new System.Drawing.Size(467, 345);
            this.splitContainer2.SplitterDistance = 271;
            this.splitContainer2.TabIndex = 12;
            // 
            // labelLayerName
            // 
            this.labelLayerName.AutoSize = true;
            this.labelLayerName.BackColor = System.Drawing.Color.Transparent;
            this.labelLayerName.Location = new System.Drawing.Point(3, 14);
            this.labelLayerName.Name = "labelLayerName";
            this.labelLayerName.Size = new System.Drawing.Size(90, 16);
            this.labelLayerName.TabIndex = 13;
            this.labelLayerName.Text = "Export Layer: ";
            this.labelLayerName.Visible = false;
            // 
            // labelOperation
            // 
            this.labelOperation.AutoSize = true;
            this.labelOperation.BackColor = System.Drawing.Color.Transparent;
            this.labelOperation.Location = new System.Drawing.Point(3, 84);
            this.labelOperation.Name = "labelOperation";
            this.labelOperation.Size = new System.Drawing.Size(119, 16);
            this.labelOperation.TabIndex = 12;
            this.labelOperation.Text = "Reading Attributes:";
            this.labelOperation.Visible = false;
            // 
            // progressBarOperation
            // 
            this.progressBarOperation.Location = new System.Drawing.Point(3, 112);
            this.progressBarOperation.Name = "progressBarOperation";
            this.progressBarOperation.Size = new System.Drawing.Size(461, 25);
            this.progressBarOperation.TabIndex = 10;
            this.progressBarOperation.Visible = false;
            // 
            // progressBarLayer
            // 
            this.progressBarLayer.Location = new System.Drawing.Point(3, 36);
            this.progressBarLayer.Name = "progressBarLayer";
            this.progressBarLayer.Size = new System.Drawing.Size(461, 25);
            this.progressBarLayer.TabIndex = 9;
            this.progressBarLayer.Visible = false;
            // 
            // ExportToSpatiaLiteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 475);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ExportToSpatiaLiteForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export to SpatiaLite";
            this.Load += new System.EventHandler(this.ExportToSpatiaLiteForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExportToSpatiaLiteForm_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxExportSelectedFeaturesOnly;
        private System.Windows.Forms.CheckBox checkBoxExportCurrentExtent;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.CheckBox checkBoxHighlightedLayer;
        private System.Windows.Forms.TreeView treeViewLayers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ProgressBar progressBarLayer;
        private System.Windows.Forms.ProgressBar progressBarOperation;
        private System.Windows.Forms.Label labelOperation;
        private System.Windows.Forms.Label labelLayerName;
    }
}