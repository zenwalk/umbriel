// <copyright file="Constants.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-10-01</date>
// <summary>Constants class file</summary>

namespace Umbriel.ArcMap.Addin.EditorTrack
{
    using Replacements = System.Collections.Generic.Dictionary<string, object>;

    /// <summary>
    /// Constants class
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Gets the name of the editor track fields XML file.
        /// </summary>
        /// <value>The name of the editor track fields file.</value>
        public static string EditorTrackFieldsFileName { get { return "EditorTrackFields.xml"; } }

        /// <summary>
        /// Gets the environment variable regex pattern.
        /// </summary>
        /// <value>The environment variable regex pattern.</value>
        public static string EnvironmentVariableRegExPattern { get { return @"{(.*?)}"; } }

        /// <summary>
        /// Gets the environment variable format (reverse of the EnvironmentVariableRegExPattern)
        /// Used for reconstructing the actual replacement variable string format.
        /// </summary>
        /// <value>The environment variable format.</value>
        public static string EnvironmentVariableFormat { get { return @"{{e:{0}}}"; } }

        /// <summary>
        /// Gets the name of the global string that indicates settings to be used for all
        /// featureclasses that are edited.
        /// </summary>
        /// <value>The name of the global.</value>
        public static string GlobalName { get { return @"#GLOBAL#"; } }

        /// <summary>
        /// Gets the default geohash precision.
        /// </summary>
        /// <value>The default geohash precision.</value>
        public static int DefaultGeohashPrecision { get { return 13; } }

        /// <summary>
        /// Builtin replacement values that can be determined when editing is started.
        /// </summary>
        public static Replacements DefaultReplacements = new Replacements { 
        { "{MachineName}", System.Environment.MachineName},
        { "{UserName}", System.Environment.UserName },
        { "{DomainName}", System.Environment.UserDomainName}
        };
    }
}
