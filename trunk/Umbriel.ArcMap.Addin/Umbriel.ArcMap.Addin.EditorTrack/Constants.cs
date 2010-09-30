using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Replacements = System.Collections.Generic.Dictionary<string, object>;

namespace Umbriel.ArcMap.Addin.EditorTrack
{
    public static class Constants
    {
        public static string EditorTrackFieldsFileName { get { return "EditorTrackFields.xml"; } }

        public static string EnvironmentVariableRegExPattern { get { return @"{(.*?)}"; } }

        public static string EnvironmentVariableFormat { get { return @"{{e:{0}}}"; } }

        public static string GlobalName { get { return @"#GLOBAL#"; } }



        
        public static Replacements DefaultReplacements = new Replacements { 
        { "{MachineName}", System.Environment.MachineName}, 
        { "{UserName}", System.Environment.UserName }, 
        { "{DomainName}", System.Environment.UserDomainName}
        };


    }
}
