namespace Umbriel.ArcMap.Editor.UI
{
    partial class BatchExtendForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BatchExtendForm));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelExtendSelected = new System.Windows.Forms.Label();
            this.checkedList = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSearchTolerance = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkSimulation = new System.Windows.Forms.CheckBox();
            this.comboBoxFeatureLayers = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Enabled = false;
            this.buttonOK.Location = new System.Drawing.Point(175, 252);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 34);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "&Extend";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(256, 252);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 34);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // labelExtendSelected
            // 
            this.labelExtendSelected.AutoSize = true;
            this.labelExtendSelected.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelExtendSelected.Location = new System.Drawing.Point(7, 14);
            this.labelExtendSelected.Name = "labelExtendSelected";
            this.labelExtendSelected.Size = new System.Drawing.Size(196, 19);
            this.labelExtendSelected.TabIndex = 1;
            this.labelExtendSelected.Text = "Extend Selected Features in: ";
            // 
            // checkedList
            // 
            this.checkedList.CheckOnClick = true;
            this.checkedList.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkedList.FormattingEnabled = true;
            this.checkedList.Location = new System.Drawing.Point(206, 80);
            this.checkedList.Name = "checkedList";
            this.checkedList.Size = new System.Drawing.Size(294, 70);
            this.checkedList.TabIndex = 2;
            this.checkedList.SelectedIndexChanged += new System.EventHandler(this.checkedList_SelectedIndexChanged);
            this.checkedList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedList_ItemCheck);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(79, 179);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "... that are within ";
            // 
            // textBoxSearchTolerance
            // 
            this.textBoxSearchTolerance.Location = new System.Drawing.Point(206, 175);
            this.textBoxSearchTolerance.Name = "textBoxSearchTolerance";
            this.textBoxSearchTolerance.Size = new System.Drawing.Size(48, 23);
            this.textBoxSearchTolerance.TabIndex = 4;
            this.textBoxSearchTolerance.Text = "1";
            this.textBoxSearchTolerance.TextChanged += new System.EventHandler(this.textBoxSearchTolerance_TextChanged);
            this.textBoxSearchTolerance.Validated += new System.EventHandler(this.textBoxSearchTolerance_Validated);
            this.textBoxSearchTolerance.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxSearchTolerance_Validating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(261, 179);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 19);
            this.label2.TabIndex = 5;
            this.label2.Text = "map unit(s).";
            // 
            // checkSimulation
            // 
            this.checkSimulation.AutoSize = true;
            this.checkSimulation.Location = new System.Drawing.Point(175, 227);
            this.checkSimulation.Name = "checkSimulation";
            this.checkSimulation.Size = new System.Drawing.Size(120, 19);
            this.checkSimulation.TabIndex = 6;
            this.checkSimulation.Text = "Simulation Mode";
            this.checkSimulation.UseVisualStyleBackColor = true;
            // 
            // comboBoxFeatureLayers
            // 
            this.comboBoxFeatureLayers.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxFeatureLayers.FormattingEnabled = true;
            this.comboBoxFeatureLayers.Location = new System.Drawing.Point(206, 12);
            this.comboBoxFeatureLayers.Name = "comboBoxFeatureLayers";
            this.comboBoxFeatureLayers.Size = new System.Drawing.Size(278, 23);
            this.comboBoxFeatureLayers.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(23, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(180, 19);
            this.label3.TabIndex = 8;
            this.label3.Text = "...to the closest feature in:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripProgressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 290);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(506, 22);
            this.statusStrip.TabIndex = 9;
            this.statusStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.statusStrip_ItemClicked);
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.AutoSize = false;
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(180, 17);
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.toolStripStatusLabel.Click += new System.EventHandler(this.toolStripStatusLabel_Click);
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(200, 16);
            this.toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.toolStripProgressBar.Visible = false;
            // 
            // BatchExtendForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 312);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxFeatureLayers);
            this.Controls.Add(this.checkSimulation);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxSearchTolerance);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkedList);
            this.Controls.Add(this.labelExtendSelected);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonOK);
            this.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BatchExtendForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Batch Extend";
            this.Load += new System.EventHandler(this.BatchExtendForm_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labelExtendSelected;
        private System.Windows.Forms.CheckedListBox checkedList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSearchTolerance;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkSimulation;
        private System.Windows.Forms.ComboBox comboBoxFeatureLayers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
    }
}