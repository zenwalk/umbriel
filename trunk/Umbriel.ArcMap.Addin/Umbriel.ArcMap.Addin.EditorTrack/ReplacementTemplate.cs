using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;



namespace Umbriel.ArcMap.Addin.EditorTrack
{   
    
    public class ReplacementTemplate
    {
        public ReplacementTemplate()
        {

            this.FieldReplacements = new SerializableDictionary<string, string>();
        }

        
        public string FeatureclassName { get; set; }


        public SerializableDictionary<string, string> FieldReplacements { get; set; }
   }
}
