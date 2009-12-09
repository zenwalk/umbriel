namespace Umbriel.ArcGIS.Layer.SpatialiteLayer.UI
{
    partial class AddSpatialiteDataForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddSpatialiteDataForm));
            this.treeViewCatalog = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.buttonExit = new System.Windows.Forms.Button();
            this.pictureBoxTarget = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTarget)).BeginInit();
            this.SuspendLayout();
            // 
            // treeViewCatalog
            // 
            this.treeViewCatalog.ImageIndex = 4;
            this.treeViewCatalog.ImageList = this.imageList;
            this.treeViewCatalog.Location = new System.Drawing.Point(13, 13);
            this.treeViewCatalog.Name = "treeViewCatalog";
            this.treeViewCatalog.SelectedImageIndex = 4;
            this.treeViewCatalog.Size = new System.Drawing.Size(254, 439);
            this.treeViewCatalog.TabIndex = 2;
            this.treeViewCatalog.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewCatalog_NodeMouseDoubleClick);
            this.treeViewCatalog.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewCatalog_DragDrop);
            this.treeViewCatalog.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCatalog_AfterSelect);
            this.treeViewCatalog.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeViewCatalog_DragEnter);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "mIconUnknownLayerType.png");
            this.imageList.Images.SetKeyName(1, "mIconPointLayer.png");
            this.imageList.Images.SetKeyName(2, "mIconLineLayer.png");
            this.imageList.Images.SetKeyName(3, "mIconPolygonLayer.png");
            this.imageList.Images.SetKeyName(4, "mActionAddSpatiaLiteLayer.png");
            // 
            // buttonExit
            // 
            this.buttonExit.AutoSize = true;
            this.buttonExit.FlatAppearance.BorderSize = 0;
            this.buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExit.Image = global::Umbriel.ArcGIS.Layer.SpatialiteLayer.Properties.Resources.exit;
            this.buttonExit.Location = new System.Drawing.Point(606, 424);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(38, 38);
            this.buttonExit.TabIndex = 1;
            this.buttonExit.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // pictureBoxTarget
            // 
            this.pictureBoxTarget.Image = global::Umbriel.ArcGIS.Layer.SpatialiteLayer.Properties.Resources.target;
            this.pictureBoxTarget.Location = new System.Drawing.Point(353, 13);
            this.pictureBoxTarget.Name = "pictureBoxTarget";
            this.pictureBoxTarget.Size = new System.Drawing.Size(215, 217);
            this.pictureBoxTarget.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxTarget.TabIndex = 0;
            this.pictureBoxTarget.TabStop = false;
            this.pictureBoxTarget.DragOver += new System.Windows.Forms.DragEventHandler(this.pictureBoxTarget_DragOver);
            this.pictureBoxTarget.Click += new System.EventHandler(this.pictureBoxTarget_Click);
            this.pictureBoxTarget.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBoxTarget_DragDrop);
            this.pictureBoxTarget.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBoxTarget_DragEnter);
            // 
            // AddSpatialiteDataForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(647, 464);
            this.Controls.Add(this.treeViewCatalog);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.pictureBoxTarget);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "AddSpatialiteDataForm";
            this.Text = "Add Spatialite Data:";
            this.Load += new System.EventHandler(this.AddSpatialiteDataForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.AddSpatialiteDataForm_DragDrop);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AddSpatialiteDataForm_KeyDown);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.AddSpatialiteDataForm_DragOver);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTarget)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxTarget;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.TreeView treeViewCatalog;
        private System.Windows.Forms.ImageList imageList;
    }
}