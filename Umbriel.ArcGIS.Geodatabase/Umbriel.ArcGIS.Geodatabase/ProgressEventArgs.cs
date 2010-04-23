﻿

namespace Umbriel.ArcGIS.Geodatabase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ProgressEventArgs :  EventArgs
    {
        public ProgressEventArgs(int index, int count)
        {
            this.Index = index;
            this.Count = count;
        }

        public int Index { get; private set; }

        public int Count { get; private set; }
    }
}
