using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbriel.ArcGIS.Spatialite
{
    public class ExporterEventArgs : EventArgs 
    {
        public ExporterEventArgs()
        {
        }

        public ExporterEventArgs(string message)
        {
            this.Message = message;
        }
        public string Message { get; set; }
    }


    public class ExporterProgressEventArgs : EventArgs
    {
        public ExporterProgressEventArgs(int value, int total)
        {
            this.Current = value;
            this.Total = total;

            if (this.Total != 0)
            {
                this.PerCent = (int)((this.Current / this.Total) * 100);
            }
        }
        public int  Total { get; set; }
        public int  Current { get; set; }
        public int PerCent { get; set; }
    }
}
