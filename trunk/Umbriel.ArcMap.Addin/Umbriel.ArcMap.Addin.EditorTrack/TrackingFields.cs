using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using FieldTrackDictionary = System.Collections.Generic.Dictionary<string, Umbriel.ArcMap.Addin.EditorTrack.FeatureclassFieldTracking>;
using ReplacementDictionary = System.Collections.Generic.Dictionary<string, object>;
using StringDictionary = SerializableDictionary<string, string>;
using StringList = System.Collections.Generic.List<string>;
using TemplateDictionary = System.Collections.Generic.Dictionary<string, Umbriel.ArcMap.Addin.EditorTrack.ReplacementTemplate>;

namespace Umbriel.ArcMap.Addin.EditorTrack
{
    /// <summary>
    /// Tracking Fields Class - handles reading from the xml document
    /// </summary>
    internal class TrackingFields : XmlDocument
    {
        /// <summary>
        /// Gets or sets the EditorTrackFields XML  file path.
        /// </summary>
        /// <value>The xml file path.</value>
        public string EditorTrackFieldsFilePath { get; private set; }

        /// <summary>
        /// Gets or sets the template on change fields.
        /// </summary>
        /// <value>The template on change fields.</value>
        public TemplateDictionary TemplateOnChangeFields { get; private set; }

        /// <summary>
        /// Gets or sets the template on create fields.
        /// </summary>
        /// <value>The template on create fields.</value>
        public TemplateDictionary TemplateOnCreateFields { get; private set; }

        /// <summary>
        /// Gets or sets the FieldTrackDictionary
        /// </summary>
        /// <value>FieldTrackDictionary that contains the fields that are tracked for each specific featureclass</value>
        public FieldTrackDictionary FeatureclassTrackingFields { get; private set; }

        public ReplacementDictionary ReplacementFieldDictionary { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackingFields"/> class.
        /// </summary>
        /// <param name="editorTrackFieldsFilePath">The editor track fields file path.</param>
        public TrackingFields(string editorTrackFieldsFilePath)
        {
            if (File.Exists(editorTrackFieldsFilePath))
            {
                //this.GlobalOnChangeFields = new StringDictionary();
                //this.GlobalOnCreateFields = new StringDictionary();

                this.EditorTrackFieldsFilePath = editorTrackFieldsFilePath;
                this.Load(this.EditorTrackFieldsFilePath);


                this.ReadReplacementTemplates();
                this.ReadGlobalFields();
                //this.ReadFeatureclassFields();

                this.BuildReplacementDictionary();
            }
        }

        /// <summary>
        /// Reads the global fields from the field tracking xml file
        /// and sets the GlobalOnChangeFields and  GlobalOnCreateFields properties
        /// </summary>
        public void ReadGlobalFields()
        {
            //try
            //{
            //    StringCollection fieldstrings = null;
            //    XmlNodeList nodelist = this.GetElementsByTagName("global");

            //    //read the OnChange fields
            //    using (XmlReader reader = new XmlNodeReader(nodelist[0]["OnChange"]["ArrayOfString"]))
            //    {
            //        XmlSerializer serializer = new XmlSerializer(typeof(StringCollection));

            //        if (serializer.CanDeserialize(reader))
            //        {
            //            fieldstrings = serializer.Deserialize(reader) as StringCollection;

            //            List<string> list = fieldstrings.Cast<string>().ToList();

            //            this.GlobalOnChangeFields = list.ToStringDictionary();
            //        }
            //        else
            //        {
            //            //do something to handle
            //            throw new XmlException();
            //        }
            //    }

            //    //read the OnCreate fields
            //    using (XmlReader reader = new XmlNodeReader(nodelist[0]["OnCreate"]["ArrayOfString"]))
            //    {
            //        XmlSerializer serializer = new XmlSerializer(typeof(StringCollection));

            //        if (serializer.CanDeserialize(reader))
            //        {
            //            fieldstrings = serializer.Deserialize(reader) as StringCollection;

            //            List<string> list = fieldstrings.Cast<string>().ToList();

            //            this.GlobalOnCreateFields = list.ToStringDictionary();
            //        }
            //        else
            //        {
            //            //do something to handle
            //            throw new XmlException();
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        /// <summary>
        /// Reads the featureclass fields from the field tracking xml file
        /// and sets the FeatureclassTrackingFields property
        /// </summary>
        public void ReadFeatureclassFields()
        {
            //try
            //{
            //    StringCollection fieldstrings = null;

            //    XmlNodeList nodelist = this.SelectNodes("/config/featureclasses/featureclass");

            //    FieldTrackDictionary featureclassTrackingFields = new FieldTrackDictionary(StringComparer.CurrentCultureIgnoreCase);

            //    foreach (XmlNode node in nodelist)
            //    {
            //        FeatureclassFieldTracking fieldTracking = new FeatureclassFieldTracking();

            //        //read the OnChange fields
            //        using (XmlReader reader = new XmlNodeReader(node["OnChange"]["ArrayOfString"]))
            //        {
            //            XmlSerializer serializer = new XmlSerializer(typeof(StringCollection));

            //            if (serializer.CanDeserialize(reader))
            //            {
            //                fieldstrings = serializer.Deserialize(reader) as StringCollection;

            //                List<string> list = fieldstrings.Cast<string>().ToList();

            //                fieldTracking.FeatureClassName = node.Attributes["name"].ToString();
            //                fieldTracking.OnChangeFields = list.ToStringDictionary();
            //            }
            //            else
            //            {
            //                //do something to handle
            //                throw new XmlException();
            //            }
            //        }

            //        // read the OnCreate fields
            //        using (XmlReader reader = new XmlNodeReader(nodelist[0]["OnCreate"]["ArrayOfString"]))
            //        {
            //            XmlSerializer serializer = new XmlSerializer(typeof(StringCollection));

            //            if (serializer.CanDeserialize(reader))
            //            {
            //                fieldstrings = serializer.Deserialize(reader) as StringCollection;

            //                List<string> list = fieldstrings.Cast<string>().ToList();

            //                fieldTracking.FeatureClassName = node.Attributes["name"].ToString();
            //                fieldTracking.OnCreateFields = list.ToStringDictionary();
            //            }
            //            else
            //            {
            //                //do something to handle
            //                throw new XmlException();
            //            }
            //        }

            //        featureclassTrackingFields.Add(fieldTracking.FeatureClassName, fieldTracking);
            //    }

            //    this.FeatureclassTrackingFields = featureclassTrackingFields;
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public void ReadReplacementTemplates()
        {
            this.ReadOnCreateReplacementTemplates();
            this.ReadOnChangeReplacementTemplates();
        }

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
                        //fieldstrings = serializer.Deserialize(reader) as StringCollection;
                        ReplacementTemplate replacementTemplate = serializer.Deserialize(reader) as ReplacementTemplate;

                        templates.Add(replacementTemplate.FeatureclassName, replacementTemplate);
                    }
                    else
                    {
                        //do something to handle
                        throw new XmlException();
                    }
                }
            }

            this.TemplateOnCreateFields = templates;
        }

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
                        //fieldstrings = serializer.Deserialize(reader) as StringCollection;
                        ReplacementTemplate replacementTemplate = serializer.Deserialize(reader) as ReplacementTemplate;

                        templates.Add(replacementTemplate.FeatureclassName, replacementTemplate);
                    }
                    else
                    {
                        //do something to handle
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
            
            //get a list of all the different replacements
            StringList environVariableReplacements = new StringList();

            foreach ( KeyValuePair<string,ReplacementTemplate> kvp in this.TemplateOnChangeFields)
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

            //TODO: need to evaluate the remainder of the non-environment variables
            // exclude: {NOW},{GEOHASH},{WKB},{WKT},{XCOORD},{YCOORD},{ZCOORD}
            

            this.ReplacementFieldDictionary = replacementDictionary;
        }
    }
}
