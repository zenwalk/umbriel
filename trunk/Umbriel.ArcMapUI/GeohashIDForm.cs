using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;

namespace Umbriel.ArcMapUI
{
    public partial class GeohashIDForm : Form
    {
        private IPoint Point { get; set; }

        public GeohashIDForm()
        {
            InitializeComponent();
        }

        public GeohashIDForm(IPoint pt)
        {
            InitializeComponent();
            comboPrecision.SelectedIndex = 0;

            this.SetPoint(pt);
            // this.Point = pt;
        }

        public void SetPoint(IPoint pt)
        {
            if (pt != null)
            {
                this.Point = pt;
                textBoxMapCoords.Text = pt.X.ToString() + " ,  " + pt.Y.ToString();

                LoadGeohashes();
            }
        }


        private void LoadGeohashes()
        {

            if (this.Point != null)
            {
                int precision = -1;

                if (!int.TryParse(comboPrecision.Text, out precision))
                {
                    precision = 13;
                }

                string geohash = Umbriel.ArcMap.Editor.Util.Geohasher.CreateGeohash(this.Point, precision);
                textBoxGeohash1.Text = geohash;

                textBoxPrecision2.Text = (precision - 1).ToString();
                textBoxGeohash2.Text = Umbriel.ArcMap.Editor.Util.Geohasher.CreateGeohash(this.Point, precision - 1);

                textBoxPrecision3.Text = (precision - 2).ToString();
                textBoxGeohash3.Text = Umbriel.ArcMap.Editor.Util.Geohasher.CreateGeohash(this.Point, precision - 2);

                textBoxPrecision4.Text = (precision - 3).ToString();
                textBoxGeohash4.Text = Umbriel.ArcMap.Editor.Util.Geohasher.CreateGeohash(this.Point, precision - 3);

                textBoxPrecision5.Text = (precision - 4).ToString();
                textBoxGeohash5.Text = Umbriel.ArcMap.Editor.Util.Geohasher.CreateGeohash(this.Point, precision - 4);
            }
        }

        private void comboPrecision_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGeohashes();
        }

        private void textBoxGeohash1_DoubleClick(object sender, EventArgs e)
        {
            LaunchURL(textBoxGeohash1.Text);
        }

        private void LaunchURL(string geohash)
        {   
            if (Util.Settings.ReadSetting("GeohashIDLaunchURLEnabled").ToBoolean())
            {
                string url = string.Format(Util.Settings.ReadSetting("GeohashIDLaunchURL"),geohash);
                System.Diagnostics.Process.Start(url);
            }
        }

        private void GeohashIDForm_Load(object sender, EventArgs e)
        {

        }
    }
}
