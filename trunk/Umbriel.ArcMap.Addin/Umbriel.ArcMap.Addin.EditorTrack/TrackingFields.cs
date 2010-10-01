// <copyright file="TrackingFields.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-10-01</date>
// <summary>TrackingFields class file</summary>

namespace Umbriel.ArcMap.Addin.EditorTrack
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;
    using FieldTrackDictionary = System.Collections.Generic.Dictionary<string, Umbriel.ArcMap.Addin.EditorTrack.FeatureclassFieldTracking>;
    using ReplacementDictionary = System.Collections.Generic.Dictionary<string, object>;
    using StringList = System.Collections.Generic.List<string>;
    using TemplateDictionary = System.Collections.Generic.Dictionary<string, Umbriel.ArcMap.Addin.EditorTrack.ReplacementTemplate>;

    /// <summary>
    /// Tracking Fields Class - handles reading from the xml document
    /// </summary>
    internal class TrackingFields : XmlDocument
    {
        #region Fields
        /// <summary>
        /// The Edit Version Name
        /// </summary>
        private string editVersionName = string.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackingFields"/> class.
        /// </summary>
        /// <param name="editorTrackFieldsFilePath">The editor track fields file path.</param>
        public TrackingFields(string editorTrackFieldsFilePath)
        {
            if (File.Exists(editorTrackFieldsFilePath))
            {
                this.EditorTrackFieldsFilePath = editorTrackFieldsFilePath;
                this.Load(this.EditorTrackFieldsFilePath);

                this.ReadReplacementTemplates();
                this.BuildReplacementDictionary();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the EditorTrackFields XML  file path.
        /// </summary>
        /// <value>The xml file path.</value>
        public string EditorTrackFieldsFilePath { get; private set; }

        /// <summary>
        /// Gets the template on change fields.
        /// </summary>
        /// <value>The template on change fields.</value>
        public TemplateDictionary TemplateOnChangeFields { get; private set; }

        /// <summary>
        /// Gets the template on create fields.
        /// </summary>
        /// <value>The template on create fields.</value>
        public TemplateDictionary TemplateOnCreateFields { get; private set; }

        /// <summary>
        /// Gets the FieldTrackDictionary
        /// </summary>
        /// <value>FieldTrackDictionary that contains the fields that are tracked for each specific featureclass</value>
        public FieldTrackDictionary FeatureclassTrackingFields { get; private set; }

        /// <summary>
        /// Gets the replacement field dictionary.
        /// </summary>
        /// <value>The replacement field dictionary.</value>
        public ReplacementDictionary ReplacementFieldDictionary { get; private set; }

        /// <summary>
        /// Gets or sets the name of the edit version.
        /// </summary>
        /// <value>The name of the edit version.</value>
        public string EditVersionName
        {
            get
            {
                return this.editVersionName;
            }

            set
            {
                this.editVersionName = value;

                if (this.ReplacementFieldDictionary != null)
                {
                    if (this.ReplacementFieldDictionary.ContainsKey("{EDITVERSION}"))
                    {
                        this.ReplacementFieldDictionary["{EDITVERSION}"] = this.editVersionName;
                    }
                    else
                    {
                        this.ReplacementFieldDictionary.Add("{EDITVERSION}", this.editVersionName);
                    }
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Reads the replacement templates from the EditorTrackFields.xml file.
        /// </summary>
        public void ReadReplacementTemplates()
        {
            this.ReadOnCreateReplacementTemplates();
            this.ReadOnChangeReplacementTemplates();
        }

        /// <summary>
        /// Reads the 'OnCreate' replacement templates variables from the EditorTrackFields.xml file.
        /// </summary>
        private void ReadOnCreateReplacementTemplates()
        {
            XmlNodeList nodelist = this.GetElementsByTagName("OnCreate");
            TemplateDictionary templates = new TemplateDictionary(StringComparer.CurrentCultureIgnoreCase);

            foreach (XmlNode node in nodelist[0].ChildNodes)
            {
                using (XmlReader reader = new XmlNodeReader(node))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ReplacementTemplate));

                    if (serializer.CanDeserialize(reader))
                    {
                        ReplacementTemplate replacementTemplate = serializer.Deserialize(reader) as ReplacementTemplate;

                        templates.Add(replacementTemplate.FeatureclassName, replacementTemplate);
                    }
                    else
                    {
                        // do something to handle
                        throw new XmlException();
                    }
                }
            }

            this.TemplateOnCreateFields = templates;
        }

        /// <summary>
        /// Reads the 'OnChange' replacement templates variables from the EditorTrackFields.xml file.
        /// </summary>
        private void ReadOnChangeReplacementTemplates()
        {
            XmlNodeList nodelist = this.GetElementsByTagName("OnChange");
            TemplateDictionary templates = new TemplateDictionary(StringComparer.CurrentCultureIgnoreCase);

            foreach (XmlNode node in nodelist[0].ChildNodes)
            {
                using (XmlReader reader = new XmlNodeReader(node))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ReplacementTemplate));

                    if (serializer.CanDeserialize(reader))
                    {
                        ReplacementTemplate replacementTemplate = serializer.Deserialize(reader) as ReplacementTemplate;

                        templates.Add(replacementTemplate.FeatureclassName, replacementTemplate);
                    }
                    else
                    {
                        // do something to handle
                        throw new XmlException();
                    }
                }
            }

            this.TemplateOnChangeFields = templates;
        }

        /// <summary>
        /// Builds the replacement dictionary.
        /// </summary>
        private void BuildReplacementDictionary()
        {
            // the usual suspects (builtin)
            ReplacementDictionary replacementDictionary = new ReplacementDictionary(Constants.DefaultReplacements, StringComparer.CurrentCultureIgnoreCase);

            // get a list of all the different replacements
            StringList environVariableReplacements = new StringList();

            foreach (KeyValuePair<string, ReplacementTemplate> kvp in this.TemplateOnChangeFields)
            {
                environVariableReplacements.AddRange(kvp.Value.FieldReplacements.Values.ToList<string>());
            }

            foreach (KeyValuePair<string, ReplacementTemplate> kvp in this.TemplateOnCreateFields)
            {
                environVariableReplacements.AddRange(kvp.Value.FieldReplacements.Values.ToList<string>());
            }

            // just query out the {e: replacements
            StringList envrep = string.Join(",", environVariableReplacements.ToArray()).FindEnvironmentVariableReplacements();

            // remove dupes
            StringList uniqueEnvvars = (from e in envrep select e).Distinct().ToList();

            // for each of the unique replacements based on environment variable, 
            // add it to the replacementDictionary.
            foreach (string environmentVariableName in uniqueEnvvars)
            {
                try
                {
                    string v = System.Environment.GetEnvironmentVariable(environmentVariableName);
                    replacementDictionary.Add(
                        string.Format(Constants.EnvironmentVariableFormat, environmentVariableName),
                        v);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.StackTrace);
                }
            }
            
            this.ReplacementFieldDictionary = replacementDictionary;
        }
        #endregion
    }
}
