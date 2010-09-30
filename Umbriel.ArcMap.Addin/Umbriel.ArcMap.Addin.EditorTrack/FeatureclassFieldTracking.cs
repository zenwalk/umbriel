using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using StringDictionary = System.Collections.Generic.Dictionary<string, string>;

namespace Umbriel.ArcMap.Addin.EditorTrack
{
    public class FeatureclassFieldTracking
    {
        public StringDictionary OnChangeFields { get; set; }
        public StringDictionary OnCreateFields { get; set; }
        public string FeatureClassName { get; set; }
    }
}
