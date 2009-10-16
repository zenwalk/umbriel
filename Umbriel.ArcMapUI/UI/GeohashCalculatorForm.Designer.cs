namespace Umbriel.ArcMapUI.UI
{
    partial class GeohashCalculatorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeohashCalculatorForm));
            this.buttonCalculate = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelLayerName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxFields = new System.Windows.Forms.ComboBox();
            this.checkBoxUseSelected = new System.Windows.Forms.CheckBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCalculate
            // 
            this.buttonCalculate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCalculate.Location = new System.Drawing.Point(89, 137);
            this.buttonCalculate.Name = "buttonCalculate";
            this.buttonCalculate.Size = new System.Drawing.Size(80, 32);
            this.buttonCalculate.TabIndex = 0;
            this.buttonCalculate.Text = "C&alculate";
            this.buttonCalculate.UseVisualStyleBackColor = true;
            this.buttonCalculate.Click += new System.EventHandler(this.buttonCalculate_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClose.Location = new System.Drawing.Point(175, 137);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(80, 32);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "Layer:";
            // 
            // labelLayerName
            // 
            this.labelLayerName.AutoSize = true;
            this.labelLayerName.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLayerName.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.labelLayerName.Location = new System.Drawing.Point(61, 9);
            this.labelLayerName.Name = "labelLayerName";
            this.labelLayerName.Size = new System.Drawing.Size(83, 18);
            this.labelLayerName.TabIndex = 3;
            this.labelLayerName.Text = "LayerName";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(160, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "Select Geohash Field:";
            // 
            // comboBoxFields
            // 
            this.comboBoxFields.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxFields.FormattingEnabled = true;
            this.comboBoxFields.Location = new System.Drawing.Point(12, 75);
            this.comboBoxFields.Name = "comboBoxFields";
            this.comboBoxFields.Size = new System.Drawing.Size(313, 27);
            this.comboBoxFields.TabIndex = 5;
            // 
            // checkBoxUseSelected
            // 
            this.checkBoxUseSelected.AutoSize = true;
            this.checkBoxUseSelected.Checked = true;
            this.checkBoxUseSelected.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxUseSelected.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxUseSelected.Location = new System.Drawing.Point(25, 108);
            this.checkBoxUseSelected.Name = "checkBoxUseSelected";
            this.checkBoxUseSelected.Size = new System.Drawing.Size(256, 23);
            this.checkBoxUseSelected.TabIndex = 6;
            this.checkBoxUseSelected.Text = "Only Calculate Selected Features";
            this.checkBoxUseSelected.UseVisualStyleBackColor = true;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 181);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(337, 22);
            this.statusStrip.TabIndex = 7;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(322, 17);
            this.toolStripStatusLabel.Spring = true;
            // 
            // GeohashCalculatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(337, 203);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.checkBoxUseSelected);
            this.Controls.Add(this.comboBoxFields);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelLayerName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonCalculate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GeohashCalculatorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Geohash Calculator";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCalculate;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelLayerName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxFields;
        private System.Windows.Forms.CheckBox checkBoxUseSelected;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
    }
}