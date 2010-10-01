// <copyright file="EditorTrackExtension.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-10-01</date>
// <summary>EditorTrackExtensions class file</summary>

//// TODO: implement {GEOHASH},{WKB},{WKT},{XCOORD},{YCOORD},{ZCOORD}

namespace Umbriel.ArcMap.Addin.EditorTrack
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using ESRI.ArcGIS.Editor;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Geodatabase;
    using StringDictionary = System.Collections.Generic.Dictionary<string, string>;

    /// <summary>
    /// EditorTrackExtension class implementing custom ESRI Editor Extension functionalities.
    /// </summary>
    public class EditorTrackExtension : ESRI.ArcGIS.Desktop.AddIns.Extension
    {
        /// <summary>
        /// TrackingFields object - tracking settings from the EditorTrackFields.xml file
        /// </summary>
        private static TrackingFields trackingFields;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorTrackExtension"/> class.
        /// </summary>
        public EditorTrackExtension()
        {
        }

        #region Properties

        #region Shortcut properties to the various editor event interfaces

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <value>The events.</value>
        private IEditEvents_Event Events
        {
            get { return ArcMap.Editor as IEditEvents_Event; }
        }

        /// <summary>
        /// Gets the events2.
        /// </summary>
        /// <value>The events2.</value>
        private IEditEvents2_Event Events2
        {
            get { return ArcMap.Editor as IEditEvents2_Event; }
        }

        /// <summary>
        /// Gets the events3.
        /// </summary>
        /// <value>The events3.</value>
        private IEditEvents3_Event Events3
        {
            get { return ArcMap.Editor as IEditEvents3_Event; }
        }

        /// <summary>
        /// Gets the events4.
        /// </summary>
        /// <value>The events4.</value>
        private IEditEvents4_Event Events4
        {
            get { return ArcMap.Editor as IEditEvents4_Event; }
        }
        #endregion

        #endregion

        #region Methods

        #region Editor Events        

        /// <summary>
        /// Wires the editor events.
        /// </summary>
        public void WireEditorEvents()
        {
            this.Events.OnCurrentTaskChanged += delegate
            {
                if (ArcMap.Editor.CurrentTask != null)
                {
                    System.Diagnostics.Debug.WriteLine(ArcMap.Editor.CurrentTask.Name);
                }
            };

            this.Events2.BeforeStopEditing += delegate(bool save) { this.OnBeforeStopEditing(save); };
            this.Events.OnStartEditing += delegate { this.Events_OnStartEditing(); };

            this.Events.OnCreateFeature += new IEditEvents_OnCreateFeatureEventHandler(this.Events_OnCreateFeature);
            this.Events.OnChangeFeature += new IEditEvents_OnChangeFeatureEventHandler(this.Events_OnChangeFeature);
        }
        
        /// <summary>
        /// Method called when editing starts
        /// </summary>
        public void Events_OnStartEditing()
        {
            // it's probably pointless to keep this here--it's not likely the user is going to find this file in the cache and modify
            trackingFields = new TrackingFields(GetEditorTrackFieldsXMLPath());
            trackingFields.EditVersionName = ArcMap.Editor.EditWorkspace.GetVersionName();
        }

        /// <summary>
        /// Called when [before stop editing].
        /// </summary>
        /// <param name="save">if set to <c>true</c> [save].</param>
        public void OnBeforeStopEditing(bool save)
        {
        }

        /// <summary>
        /// Method called when a feature is created.
        /// </summary>
        /// <param name="obj">The object that was created</param>
        public void Events_OnCreateFeature(ESRI.ArcGIS.Geodatabase.IObject obj)
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
                            object val = this.EvaluateValue(obj, item, trackingFields.ReplacementFieldDictionary);

                            if (val != null)
                            {
                                if (val is byte[])
                                {
                                    IMemoryBlobStreamVariant memoryBlobStream = new MemoryBlobStreamClass();
                                    memoryBlobStream.ImportFromVariant(val);

                                    obj.set_Value(i, memoryBlobStream);
                                }
                                else
                                {
                                    obj.set_Value(i, val);
                                }
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
                            object val = this.EvaluateValue(obj, item, trackingFields.ReplacementFieldDictionary);

                            if (val != null)
                            {
                                obj.set_Value(i, val);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method called when a feature is modified.
        /// </summary>
        /// <param name="obj">The object that was created</param>
        public void Events_OnChangeFeature(IObject obj)
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
                            object val = this.EvaluateValue(obj, item, trackingFields.ReplacementFieldDictionary);

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
                            object val = this.EvaluateValue(obj, item, trackingFields.ReplacementFieldDictionary);

                            if (val != null)
                            {
                                obj.set_Value(i, val);
                            }
                        }
                    }
                }
            }
        }
        
        #endregion

        /// <summary>
        /// Called when [startup].
        /// </summary>
        protected override void OnStartup()
        {
            IEditor theEditor = ArcMap.Editor;

            this.WireEditorEvents();
        }

        /// <summary>
        /// Called when [shutdown].
        /// </summary>
        protected override void OnShutdown()
        {
        }

        /// <summary>
        /// Gets the editor track fields XML path.
        /// </summary>
        /// <returns>path of the EditorTrackFields.xml file</returns>
        private static string GetEditorTrackFieldsXMLPath()
        {
            string path = @"{0}\{1}";

            return string.Format(
                path,
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                Constants.EditorTrackFieldsFileName);
        }

        /// <summary>
        /// Evaluates the value.
        /// </summary>
        /// <param name="obj">The object that was created/changed</param>
        /// <param name="kvp">key value pair of the replacement variable </param>
        /// <param name="replacementFieldDictionary">The replacement field dictionary.</param>
        /// <returns>object containing the value to entered as the field value</returns>
        private object EvaluateValue(IObject obj, KeyValuePair<string, string> kvp, Dictionary<string, object> replacementFieldDictionary)
        {
            object val = null;

            if (kvp.Value.Equals("{NOW}", StringComparison.CurrentCultureIgnoreCase))
            {
                val = System.DateTime.Now;
            }
            else if (kvp.Value.Equals("{WKB}", StringComparison.CurrentCultureIgnoreCase))
            {
                if (obj is IFeature)
                {
                    IFeature feature = obj as IFeature;
                    if (feature != null)
                    {
                        if (!feature.Shape.IsEmpty)
                        {
                            val = feature.Shape.ToWKB();
                        }
                    }
                }
            }
            else if (kvp.Value.Equals("{GEOHASH}", StringComparison.CurrentCultureIgnoreCase))
            {
                if (obj is IFeature)
                {
                    IFeature feature = obj as IFeature;
                    if (feature != null)
                    {
                        if (!feature.Shape.IsEmpty)
                        {
                            val = feature.Shape.ToGeohash();
                        }
                    }
                }
            }
            else
            {
                val = kvp.Value.ReplaceMany(trackingFields.ReplacementFieldDictionary);
            }

            return val;
        }
        
        #endregion
    }
}
