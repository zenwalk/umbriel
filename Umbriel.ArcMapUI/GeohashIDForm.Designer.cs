namespace Umbriel.ArcMapUI
{
    partial class GeohashIDForm
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
            this.comboPrecision = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxMapCoords = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxGeohash1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxPrecision2 = new System.Windows.Forms.TextBox();
            this.textBoxPrecision3 = new System.Windows.Forms.TextBox();
            this.textBoxPrecision4 = new System.Windows.Forms.TextBox();
            this.textBoxPrecision5 = new System.Windows.Forms.TextBox();
            this.textBoxGeohash2 = new System.Windows.Forms.TextBox();
            this.textBoxGeohash3 = new System.Windows.Forms.TextBox();
            this.textBoxGeohash4 = new System.Windows.Forms.TextBox();
            this.textBoxGeohash5 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // comboPrecision
            // 
            this.comboPrecision.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboPrecision.FormattingEnabled = true;
            this.comboPrecision.Items.AddRange(new object[] {
            "13",
            "12",
            "11",
            "10",
            "9",
            "8",
            "7",
            "6",
            "5"});
            this.comboPrecision.Location = new System.Drawing.Point(128, 64);
            this.comboPrecision.Name = "comboPrecision";
            this.comboPrecision.Size = new System.Drawing.Size(59, 24);
            this.comboPrecision.TabIndex = 0;
            this.comboPrecision.SelectedIndexChanged += new System.EventHandler(this.comboPrecision_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(128, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Precision";
            // 
            // textBoxMapCoords
            // 
            this.textBoxMapCoords.Location = new System.Drawing.Point(128, 13);
            this.textBoxMapCoords.Name = "textBoxMapCoords";
            this.textBoxMapCoords.Size = new System.Drawing.Size(331, 20);
            this.textBoxMapCoords.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Map Coordinates:";
            // 
            // textBoxGeohash1
            // 
            this.textBoxGeohash1.Location = new System.Drawing.Point(285, 68);
            this.textBoxGeohash1.Name = "textBoxGeohash1";
            this.textBoxGeohash1.Size = new System.Drawing.Size(174, 20);
            this.textBoxGeohash1.TabIndex = 4;
            this.textBoxGeohash1.DoubleClick += new System.EventHandler(this.textBoxGeohash1_DoubleClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(282, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Geohash";
            // 
            // textBoxPrecision2
            // 
            this.textBoxPrecision2.ForeColor = System.Drawing.Color.Gray;
            this.textBoxPrecision2.Location = new System.Drawing.Point(128, 108);
            this.textBoxPrecision2.Name = "textBoxPrecision2";
            this.textBoxPrecision2.Size = new System.Drawing.Size(43, 20);
            this.textBoxPrecision2.TabIndex = 6;
            // 
            // textBoxPrecision3
            // 
            this.textBoxPrecision3.ForeColor = System.Drawing.Color.Gray;
            this.textBoxPrecision3.Location = new System.Drawing.Point(128, 134);
            this.textBoxPrecision3.Name = "textBoxPrecision3";
            this.textBoxPrecision3.Size = new System.Drawing.Size(43, 20);
            this.textBoxPrecision3.TabIndex = 7;
            // 
            // textBoxPrecision4
            // 
            this.textBoxPrecision4.ForeColor = System.Drawing.Color.Gray;
            this.textBoxPrecision4.Location = new System.Drawing.Point(128, 160);
            this.textBoxPrecision4.Name = "textBoxPrecision4";
            this.textBoxPrecision4.Size = new System.Drawing.Size(43, 20);
            this.textBoxPrecision4.TabIndex = 8;
            // 
            // textBoxPrecision5
            // 
            this.textBoxPrecision5.ForeColor = System.Drawing.Color.Gray;
            this.textBoxPrecision5.Location = new System.Drawing.Point(128, 188);
            this.textBoxPrecision5.Name = "textBoxPrecision5";
            this.textBoxPrecision5.Size = new System.Drawing.Size(43, 20);
            this.textBoxPrecision5.TabIndex = 9;
            // 
            // textBoxGeohash2
            // 
            this.textBoxGeohash2.ForeColor = System.Drawing.Color.Gray;
            this.textBoxGeohash2.Location = new System.Drawing.Point(285, 108);
            this.textBoxGeohash2.Name = "textBoxGeohash2";
            this.textBoxGeohash2.Size = new System.Drawing.Size(174, 20);
            this.textBoxGeohash2.TabIndex = 10;
            // 
            // textBoxGeohash3
            // 
            this.textBoxGeohash3.ForeColor = System.Drawing.Color.Gray;
            this.textBoxGeohash3.Location = new System.Drawing.Point(285, 134);
            this.textBoxGeohash3.Name = "textBoxGeohash3";
            this.textBoxGeohash3.Size = new System.Drawing.Size(174, 20);
            this.textBoxGeohash3.TabIndex = 11;
            // 
            // textBoxGeohash4
            // 
            this.textBoxGeohash4.ForeColor = System.Drawing.Color.Gray;
            this.textBoxGeohash4.Location = new System.Drawing.Point(285, 160);
            this.textBoxGeohash4.Name = "textBoxGeohash4";
            this.textBoxGeohash4.Size = new System.Drawing.Size(174, 20);
            this.textBoxGeohash4.TabIndex = 12;
            // 
            // textBoxGeohash5
            // 
            this.textBoxGeohash5.ForeColor = System.Drawing.Color.Gray;
            this.textBoxGeohash5.Location = new System.Drawing.Point(285, 188);
            this.textBoxGeohash5.Name = "textBoxGeohash5";
            this.textBoxGeohash5.Size = new System.Drawing.Size(174, 20);
            this.textBoxGeohash5.TabIndex = 13;
            // 
            // GeohashIDForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 268);
            this.Controls.Add(this.textBoxGeohash5);
            this.Controls.Add(this.textBoxGeohash4);
            this.Controls.Add(this.textBoxGeohash3);
            this.Controls.Add(this.textBoxGeohash2);
            this.Controls.Add(this.textBoxPrecision5);
            this.Controls.Add(this.textBoxPrecision4);
            this.Controls.Add(this.textBoxPrecision3);
            this.Controls.Add(this.textBoxPrecision2);
            this.Controls.Add(this.textBoxGeohash1);
            this.Controls.Add(this.comboPrecision);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxMapCoords);
            this.Controls.Add(this.label1);
            this.Name = "GeohashIDForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Geohash ID";
            this.Load += new System.EventHandler(this.GeohashIDForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboPrecision;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxMapCoords;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxGeohash1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxPrecision2;
        private System.Windows.Forms.TextBox textBoxPrecision3;
        private System.Windows.Forms.TextBox textBoxPrecision4;
        private System.Windows.Forms.TextBox textBoxPrecision5;
        private System.Windows.Forms.TextBox textBoxGeohash2;
        private System.Windows.Forms.TextBox textBoxGeohash3;
        private System.Windows.Forms.TextBox textBoxGeohash4;
        private System.Windows.Forms.TextBox textBoxGeohash5;
    }
}