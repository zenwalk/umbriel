using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Geodatabase;
using StringDictionary = System.Collections.Generic.Dictionary<string, string>;
using TemplateDictionary = System.Collections.Generic.Dictionary<string, Umbriel.ArcMap.Addin.EditorTrack.ReplacementTemplate>;

namespace Umbriel.ArcMap.Addin.EditorTrack
{
    /// <summary>
    /// EditorTrackExtension class implementing custom ESRI Editor Extension functionalities.
    /// </summary>
    public class EditorTrackExtension : ESRI.ArcGIS.Desktop.AddIns.Extension
    {
        private static TrackingFields trackingFields;

        private static StringDictionary addinConfigSettings;

        public EditorTrackExtension()
        {
        }

        protected override void OnStartup()
        {
            IEditor theEditor = ArcMap.Editor;

            GetSettings();

            this.WireEditorEvents();

        }

        protected override void OnShutdown()
        {
        }

        #region Editor Events

        #region Shortcut properties to the various editor event interfaces
        private IEditEvents_Event Events
        {
            get { return ArcMap.Editor as IEditEvents_Event; }
        }
        private IEditEvents2_Event Events2
        {
            get { return ArcMap.Editor as IEditEvents2_Event; }
        }
        private IEditEvents3_Event Events3
        {
            get { return ArcMap.Editor as IEditEvents3_Event; }
        }
        private IEditEvents4_Event Events4
        {
            get { return ArcMap.Editor as IEditEvents4_Event; }
        }
        #endregion

        void WireEditorEvents()
        {
            //
            //  TODO: Sample code demonstrating editor event wiring
            //
            Events.OnCurrentTaskChanged += delegate
            {
                if (ArcMap.Editor.CurrentTask != null)
                    System.Diagnostics.Debug.WriteLine(ArcMap.Editor.CurrentTask.Name);
            };

            Events2.BeforeStopEditing += delegate(bool save) { OnBeforeStopEditing(save); };
            Events.OnStartEditing += delegate() { Events_OnStartEditing(); };
            
            Events.OnCreateFeature += new IEditEvents_OnCreateFeatureEventHandler(Events_OnCreateFeature);
            Events.OnChangeFeature += new IEditEvents_OnChangeFeatureEventHandler(Events_OnChangeFeature);
        }

        void Events_OnChangeFeature(IObject obj)
        {
            if (obj != null)
            {
                ReplacementTemplate globaltemplates = trackingFields.TemplateOnChangeFields[Constants.GlobalName];
                ReplacementTemplate featclasstemplates = null;

                if (trackingFields.TemplateOnCreateFields.ContainsKey((obj.Class as IDataset).Name))
                {
                    featclasstemplates = trackingFields.TemplateOnChangeFields[(obj.Class as IDataset).Name];
                }

                if (globaltemplates != null && globaltemplates.FieldReplacements != null)
                {
                    foreach (KeyValuePair<string, string> item in globaltemplates.FieldReplacements)
                    {
                        int i = obj.Fields.FindField(item.Key);

                        if (i > -1)
                        {
                            object val = this.EvaluateValue(item, trackingFields.ReplacementFieldDictionary);

                            if (val != null)
                            {
                                obj.set_Value(i, val);
                            }
                        }
                    }
                }

                if (featclasstemplates != null && featclasstemplates.FieldReplacements != null)
                {
                    foreach (KeyValuePair<string, string> item in featclasstemplates.FieldReplacements)
                    {
                        int i = obj.Fields.FindField(item.Key);

                        if (i > -1)
                        {
                            object val = this.EvaluateValue(item, trackingFields.ReplacementFieldDictionary);

                            if (val != null)
                            {
                                obj.set_Value(i, val);
                            }
                        }
                    }
                }
            }
        }

        void Events_OnStartEditing()
        {
            //it's probably pointless to keep this here--it's not likely the user is going to find this file in the cache and modify
            trackingFields = new TrackingFields(GetEditorTrackFieldsXMLPath());
        }

        void Events_OnCreateFeature(ESRI.ArcGIS.Geodatabase.IObject obj)
        {
            if (obj != null)
            {
                ReplacementTemplate globaltemplates = trackingFields.TemplateOnCreateFields[Constants.GlobalName];
                ReplacementTemplate featclasstemplates = null;

                if (trackingFields.TemplateOnCreateFields.ContainsKey((obj.Class as IDataset).Name))
                {
                    featclasstemplates = trackingFields.TemplateOnCreateFields[(obj.Class as IDataset).Name];
                }

                if (globaltemplates != null && globaltemplates.FieldReplacements != null)
                {
                    foreach (KeyValuePair<string, string> item in globaltemplates.FieldReplacements)
                    {
                        int i = obj.Fields.FindField(item.Key);

                        if (i > -1)
                        {
                            object val = this.EvaluateValue(item, trackingFields.ReplacementFieldDictionary);

                            if (val != null)
                            {
                                obj.set_Value(i, val);
                            }
                        }
                    }
                }

                if (featclasstemplates != null && featclasstemplates.FieldReplacements != null)
                {
                    foreach (KeyValuePair<string, string> item in featclasstemplates.FieldReplacements)
                    {
                        int i = obj.Fields.FindField(item.Key);

                        if (i > -1)
                        {
                            object val = this.EvaluateValue(item, trackingFields.ReplacementFieldDictionary);

                            if (val != null)
                            {
                                obj.set_Value(i, val);
                            }
                        }
                    }
                }
            }
        }

        private object EvaluateValue(KeyValuePair<string, string> kvp, Dictionary<string, object> replacementFieldDictionary)
        {
            object val = null;

            if (kvp.Value.Equals("{NOW}", StringComparison.CurrentCultureIgnoreCase))
            {
                val = System.DateTime.Now;
            }
            else
            {
                val = kvp.Value.ReplaceMany(trackingFields.ReplacementFieldDictionary);
            }

            return val;
        }

        void OnBeforeStopEditing(bool save)
        {
        }
        #endregion

        public static string GetEditorTrackFieldsXMLPath()
        {
            string path = @"{0}\{1}";

            return string.Format(path,
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                Constants.EditorTrackFieldsFileName);
        }

        public static StringDictionary GetSettings()
        {
            string path = Assembly.GetExecutingAssembly().Location + ".config";
            return GetSettings(path);
        }

        public static StringDictionary GetSettings(string configPath)
        {
            if (addinConfigSettings == null)
            {
                addinConfigSettings = new StringDictionary();

                // Trace.WriteLine("HSTHelper.Settings.configPath={0}".FormatString(configPath));

                if (File.Exists(configPath))
                {
                    System.IO.FileStream fs = new System.IO.FileStream(configPath, FileMode.Open, FileAccess.Read);

                    XmlDocument doc = new XmlDocument();
                    doc.Load(fs);

                    XmlNodeList nodes = doc.GetElementsByTagName("setting");

                    foreach (XmlNode node in nodes)
                    {
                        XmlAttribute attribute = node.Attributes["name"];

                        if (attribute != null)
                        {
                            string nodeValue = node.InnerText;

                            if (!string.IsNullOrEmpty(attribute.Name))
                            {
                                addinConfigSettings.Add(attribute.Value, nodeValue);
                            }
                        }
                    }
                }
            }

            return addinConfigSettings;
        }

        public static string GetSetting(string settingName)
        {
            string settingValue = string.Empty;

            StringDictionary settings = GetSettings();

            if (settings.ContainsKey(settingName))
            {
                settingValue = settings[settingName];
            }

            return settingValue;
        }
    }

}
