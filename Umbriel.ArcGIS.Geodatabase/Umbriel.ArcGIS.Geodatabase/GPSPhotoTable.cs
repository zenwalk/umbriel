// <copyright file="GPSPhotoTable.cs" company="Umbriel Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2009-09-30</date>
// <summary>
////</summary>

namespace Umbriel.ArcGIS.Geodatabase
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.DataSourcesGDB;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;

    /// <summary>
    /// GPSPhotoReadProgress EventHandler
    /// </summary>
    public delegate void GPSPhotoReadProgressEventHandler(object sender, ProgressEventArgs e);

    /// <summary>
    /// GPS Photo Table
    /// </summary>
    public class GPSPhotoTable : DataTable
    {
        #region Contructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GPSPhotoTable"/> class.
        /// </summary>
        public GPSPhotoTable()
        {
            this.PhotoFilePaths = new List<string>();
            this.CreateColumns();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GPSPhotoTable"/> class.
        /// </summary>
        /// <param name="photoFilePaths">The photo file paths.</param>
        public GPSPhotoTable(string[] photoFilePaths)
        {
            this.PhotoFilePaths = new List<string>(photoFilePaths);
            this.CreateColumns();
            this.ReadFiles(this.PhotoFilePaths);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GPSPhotoTable"/> class.
        /// </summary>
        /// <param name="photoFilePaths">The photo file paths.</param>
        public GPSPhotoTable(List<string> photoFilePaths)
        {
            this.PhotoFilePaths = photoFilePaths;
            this.CreateColumns();
            this.ReadFiles(photoFilePaths);
        }


        #endregion
        
        /// <summary>
        /// Occurs when [progress event].
        /// </summary>
        public event GPSPhotoReadProgressEventHandler ProgressEvent;

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [load photo into BLOB].
        /// </summary>
        /// <value><c>true</c> if [load photo into BLOB]; otherwise, <c>false</c>.</value>
        public bool LoadPhotoIntoBLOB { get;  set; }

        /// <summary>
        /// Gets or sets the photo file paths.
        /// </summary>
        /// <value>The photo file paths.</value>
        private List<string> PhotoFilePaths { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Adds the photo file paths.
        /// </summary>
        /// <param name="files">The list of  files.</param>
        public void AddPhotoFilePaths(string[] files)
        {
            this.ReadFiles(new List<string>(files));
        }

        /// <summary>
        /// Adds the photo file paths.
        /// </summary>
        /// <param name="files">The files.</param>
        public void AddPhotoFilePaths(List<string> files)
        {
            this.ReadFiles(files);
        }

        /// <summary>
        /// Converts the GPSPhotoTable data to a  new FeatureClass in a scratch workspace
        /// </summary>
        /// <returns>GPSPhoto IFeatureClass </returns>
        public IFeatureClass ToFeatureClass()
        {
            IFeatureClass featureClass = this.CreatePhotoFeatureclass();
            this.LoadPhotoRows(featureClass);
            return featureClass;
        }

        /// <summary>
        /// Converts the GPSPhotoTable data to a  new FeatureClass in a scratch workspace
        /// </summary>
        /// <param name="loadBlobField">if set to <c>true</c> [load BLOB field].</param>
        /// <returns>GPSPhoto IFeatureClass</returns>
        public IFeatureClass ToFeatureClass(bool loadBlobField)
        {
            bool original = this.LoadPhotoIntoBLOB;

            this.LoadPhotoIntoBLOB = loadBlobField;
            IFeatureClass featureClass = this.CreatePhotoFeatureclass();
            this.LoadPhotoRows(featureClass);

            this.LoadPhotoIntoBLOB = original;

            return featureClass;
        }

        /// <summary>
        /// Converts the GPSPhotoTable data to a  new FeatureClass
        /// </summary>
        /// <param name="workspace">The workspace where the featureclass will be created.</param>
        /// <returns>GPSPhoto IFeatureClass </returns>
        public IFeatureClass ToFeatureClass(IWorkspace workspace)
        {
            IFeatureClass featureClass = this.CreatePhotoFeatureclass(workspace);
            this.LoadPhotoRows(featureClass);
            return featureClass;
        }

        /// <summary>
        /// Loads the GPSPhotoTable data to a  new FeatureClass
        /// </summary>
        /// <param name="featureclass">The featureclass.</param>
        /// <returns>GPSPhoto IFeatureClass </returns>
        public IFeatureClass ToFeatureClass(IFeatureClass featureclass)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when [progress update].
        /// </summary>
        /// <param name="value">The value.</param>
        private void OnProgressUpdate(int value, int count)
        {
            if (this.ProgressEvent != null)
            {
                ProgressEventArgs args = new ProgressEventArgs(value, count);
                this.ProgressEvent(this, args);
            }
        }

        /// <summary>
        /// Loads the photo rows into a GPSPhoto featureclass
        /// </summary>
        /// <param name="featureclass">The featureclass.</param>
        private void LoadPhotoRows(IFeatureClass featureclass)
        {
            IDataset dataset = (IDataset)featureclass;
            IWorkspace workspace = dataset.Workspace;
            IWorkspaceEdit edit = (IWorkspaceEdit)workspace;

            IFeatureCursor cursor = featureclass.Insert(true);

            try
            {
                edit.StartEditing(true);

                foreach (DataRow row in this.Rows)
                {
                    if (!row["Latitude"].Equals(System.DBNull.Value)
                        && !row["Longitude"].Equals(System.DBNull.Value))
                    {
                        IPoint pt = new PointClass();

                        pt.PutCoords(
                            row["Longitude"].ToDouble(),
                            row["Latitude"].ToDouble());

                        IFeatureBuffer featureBuffer = featureclass.CreateFeatureBuffer();
                        featureBuffer.Shape = (IGeometry)pt;

                        if (!row["FileName"].Equals(System.DBNull.Value))
                        {
                            featureBuffer.set_Value(
                                featureBuffer.Fields.FindField("Filename"),
                                row["FileName"].ToString());
                        }

                        if (!row["Latitude"].Equals(System.DBNull.Value))
                        {
                            featureBuffer.set_Value(
                                featureBuffer.Fields.FindField("Latitude"),
                                row["Latitude"].ToDouble());
                        }

                        if (!row["Longitude"].Equals(System.DBNull.Value))
                        {
                            featureBuffer.set_Value(
                                featureBuffer.Fields.FindField("Longitude"),
                                row["Longitude"].ToDouble());
                        }

                        if (!row["MagneticDirection"].Equals(System.DBNull.Value))
                        {
                            featureBuffer.set_Value(
                                featureBuffer.Fields.FindField("MagneticNorth"),
                                row["MagneticDirection"].ToFloat());
                        }

                        if (!row["FullPath"].Equals(System.DBNull.Value))
                        {
                            string photofilepath = row["FullPath"].ToString();

                            if (!string.IsNullOrEmpty(photofilepath))
                            {
                                featureBuffer.set_Value(featureBuffer.Fields.FindField("FullPath"), photofilepath);

                                //load in photo into blob field:
                                if (this.LoadPhotoIntoBLOB &&  File.Exists(photofilepath))
                                {
                                    try
                                    {
                                        IMemoryBlobStream blobstream = new MemoryBlobStreamClass();
                                        blobstream.LoadFromFile(photofilepath);
                                        featureBuffer.set_Value(featureBuffer.Fields.FindField("PhotoBLOB"), blobstream);
                                        System.Runtime.InteropServices.Marshal.ReleaseComObject(blobstream);
                                    }
                                    catch (Exception ex)
                                    {
                                        Trace.Write(ex.StackTrace);
                                    }
                                }
                            }




                        }

                        if (!row["Modified"].Equals(System.DBNull.Value))
                        {
                            DateTime modified = Convert.ToDateTime(row["Modified"]);
                            featureBuffer.set_Value(
                                featureBuffer.Fields.FindField("FileLastModified"),
                               modified);
                        }

                        if (!row["DateTaken"].Equals(System.DBNull.Value))
                        {
                            DateTime datetaken = Convert.ToDateTime(row["DateTaken"]);
                            featureBuffer.set_Value(
                                featureBuffer.Fields.FindField("DatePhotoTaken"),
                               datetaken);
                        }



                        cursor.InsertFeature(featureBuffer);
                    }
                }

                edit.StopEditing(true);
            }
            catch (Exception ex)
            {
                if (edit.IsBeingEdited())
                {
                    edit.StopEditing(false);
                }

                Trace.WriteLine(ex.StackTrace);

                throw;
            }
        }

        /// <summary>
        /// Creates the photo featureclass in a scratch workspace
        /// </summary>
        /// <returns>IFeatureClass for GPS Photos</returns>
        private IFeatureClass CreatePhotoFeatureclass()
        {
            IScratchWorkspaceFactory scratchWorkspaceFactory = new ScratchWorkspaceFactoryClass();
            IWorkspace scratchWorkspace = scratchWorkspaceFactory.CreateNewScratchWorkspace();
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)scratchWorkspace;

            string featureclassname = this.GetUniqueFeatureClassName();

            return this.CreatePhotoFeatureclass((IWorkspace)featureWorkspace, featureclassname);
        }

        /// <summary>
        /// Creates the photo featureclass.
        /// </summary>
        /// <param name="featureclassname">The featureclassname.</param>
        /// <returns>IFeatureClass for GPS Photos</returns>
        private IFeatureClass CreatePhotoFeatureclass(string featureclassname)
        {
            IScratchWorkspaceFactory scratchWorkspaceFactory = new ScratchWorkspaceFactoryClass();
            IWorkspace scratchWorkspace = scratchWorkspaceFactory.CreateNewScratchWorkspace();
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)scratchWorkspace;

            return this.CreatePhotoFeatureclass((IWorkspace)featureWorkspace, featureclassname);
        }

        /// <summary>
        /// Creates the photo featureclass.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <param name="featureclassname">The featureclassname.</param>
        /// <returns>IFeatureClass for GPS Photos</returns>
        private IFeatureClass CreatePhotoFeatureclass(IWorkspace workspace, string featureclassname)
        {
            ISpatialReference spatialReference = Utility.GetWGS84SpatialReference();

            IFeatureClassDescription featureclassDesc = new FeatureClassDescriptionClass();
            IObjectClassDescription objectclassDesc = (IObjectClassDescription)featureclassDesc;

            try
            {
                IFeatureClass featureclass = this.CreateFeatureClass((IFeatureWorkspace)workspace, featureclassname, spatialReference);

                return featureclass;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Creates the photo featureclass.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        /// <returns>IFeatureClass for GPS Photos</returns>
        private IFeatureClass CreatePhotoFeatureclass(IWorkspace workspace)
        {
            ISpatialReference spatialReference = Utility.GetWGS84SpatialReference();

            IFeatureClassDescription featureclassDesc = new FeatureClassDescriptionClass();
            IObjectClassDescription objectclassDesc = (IObjectClassDescription)featureclassDesc;

            string featureclassname = this.GetUniqueFeatureClassName();

            try
            {
                IFeatureClass featureclass = this.CreateFeatureClass((IFeatureWorkspace)workspace, featureclassname, spatialReference);

                return featureclass;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Creates the feature class.
        /// </summary>
        /// <param name="featureWorkspace">The feature workspace.</param>
        /// <param name="nameOfFeatureClass">The name of feature class.</param>
        /// <param name="spatialReference">The spatial reference.</param>
        /// <returns>IFeatureClass for GPS Photos</returns>
        private IFeatureClass CreateFeatureClass(IFeatureWorkspace featureWorkspace, string nameOfFeatureClass, ISpatialReference spatialReference)
        {
            // This function creates a new stand-alone feature class in a supplied workspace by building all of the
            // fields from scratch. IFeatureClassDescription is used to get the InstanceClassID and ExtensionClassID.
            // Create new Fields collection with the number of fields you plan to add. Must add at least two fields
            // for a feature class; Object ID, and Shape field.
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;

            fieldsEdit.FieldCount_2 = 10;

            // Create the Object ID field.
            IField fieldUserDefined = new Field();
            IFieldEdit fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "OBJECTID";
            fieldEdit.AliasName_2 = "OBJECT ID";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsEdit.set_Field(0, fieldUserDefined);

            // Create the Shape field.
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;

            // Set up the geometry definition for the Shape field.
            IGeometryDef geometryDef = new GeometryDefClass();
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            geometryDefEdit.GeometryType_2 = ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint;

            // By setting the grid size to 0, you're allowing ArcGIS to determine the appropriate grid sizes for the feature class. 
            // If in a personal geodatabase, the grid size will be 1000. If in a file or ArcSDE geodatabase, the grid size
            // will be based on the initial loading or inserting of features.
            geometryDefEdit.GridCount_2 = 1;
            geometryDefEdit.set_GridSize(0, 0);
            geometryDefEdit.HasM_2 = false;
            geometryDefEdit.HasZ_2 = false;

            // Assign the spatial reference that was passed in, possibly from
            // IGeodatabase.SpatialReference for the containing feature dataset.
            if (spatialReference != null)
            {
                geometryDefEdit.SpatialReference_2 = spatialReference;
            }

            // Set standard field properties.
            fieldEdit.Name_2 = "SHAPE";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            fieldEdit.GeometryDef_2 = geometryDef;
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Required_2 = true;
            fieldsEdit.set_Field(1, fieldUserDefined);

            // Filename field
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "Filename";
            fieldEdit.AliasName_2 = "Filename";
            fieldEdit.Editable_2 = true;
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Length_2 = 124;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
            fieldsEdit.set_Field(2, fieldUserDefined);

            // DateTime Photo Taken field
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "DatePhotoTaken";
            fieldEdit.AliasName_2 = "DatePhotoTaken";
            fieldEdit.Editable_2 = true;
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeDate;
            fieldsEdit.set_Field(3, fieldUserDefined);

            // Modified
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "FileLastModified";
            fieldEdit.AliasName_2 = "FileLastModified";
            fieldEdit.Editable_2 = true;
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeDate;
            fieldsEdit.set_Field(4, fieldUserDefined);

            // latitude field
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "Latitude";
            fieldEdit.AliasName_2 = "Latitude";
            fieldEdit.Editable_2 = true;
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Precision_2 = 10;
            fieldEdit.Scale_2 = 6;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            fieldsEdit.set_Field(5, fieldUserDefined);

            // longitude field
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "Longitude";
            fieldEdit.AliasName_2 = "Longitude";
            fieldEdit.Editable_2 = true;
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Precision_2 = 10;
            fieldEdit.Scale_2 = 6;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            fieldsEdit.set_Field(6, fieldUserDefined);

            // Magnetic North
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "MagneticNorth";
            fieldEdit.AliasName_2 = "Magnetic North";
            fieldEdit.Editable_2 = true;
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Precision_2 = 5;
            fieldEdit.Scale_2 = 2;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeSingle;
            fieldsEdit.set_Field(7, fieldUserDefined);

            // fullpath field
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "FullPath";
            fieldEdit.AliasName_2 = "FullPath";
            fieldEdit.Editable_2 = true;
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Length_2 = 255;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
            fieldsEdit.set_Field(8, fieldUserDefined);

            // ImageHTML
            fieldUserDefined = new Field();
            fieldEdit = (IFieldEdit)fieldUserDefined;
            fieldEdit.Name_2 = "PhotoBLOB";
            fieldEdit.AliasName_2 = "PhotoBLOB";
            fieldEdit.Editable_2 = true;
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeBlob;
            fieldsEdit.set_Field(9, fieldUserDefined);

            IFeatureClassDescription featureclassDesc = new FeatureClassDescriptionClass();
            IObjectClassDescription objectclassDesc = (IObjectClassDescription)featureclassDesc;

            return featureWorkspace.CreateFeatureClass(
                nameOfFeatureClass,
                fields,
                objectclassDesc.InstanceCLSID,
                objectclassDesc.ClassExtensionCLSID,
                esriFeatureType.esriFTSimple,
                featureclassDesc.ShapeFieldName,
                string.Empty);
        }

        /// <summary>
        /// Gets the name of the unique feature class.
        /// </summary>
        /// <returns>featureclass name (photo concat with datetime ticks)</returns>
        private string GetUniqueFeatureClassName()
        {
            return "Photo" + DateTime.Now.Ticks.ToString();
        }

        /// <summary>
        /// Reads the list of photo files and loads data into DataTable
        /// </summary>
        /// <param name="files">The list of file paths</param>
        private void ReadFiles(List<string> files)
        {
            int c = 0;
            foreach (string file in files)
            {
                try
                {
                    c++;

                    // TODO: add check for existing filepath
                    FileInfo fileInfo = new FileInfo(file);

                    double lengthKb = fileInfo.Length * 0.0009765625;

                    Umbriel.GIS.Photo.GeoPhoto geoPhoto = new Umbriel.GIS.Photo.GeoPhoto(file);

                    if (geoPhoto.Coordinate != null)
                    {
                        try
                        {
                            DataRow row = this.NewRow();

                            row["FileName"] = System.IO.Path.GetFileName(file);
                            row["FileSize"] = lengthKb.ToString("0.##") + " Kb";
                            row["Type"] = System.IO.Path.GetExtension(file);
                            row["Modified"] = fileInfo.LastWriteTime.ToString();

                            if (geoPhoto.PhotoDateTime != null)
                            {
                                row["DateTaken"] = geoPhoto.PhotoDateTime;
                            }

                            row["Latitude"] = geoPhoto.Coordinate.Latitude.ToString("0.#####");
                            row["Longitude"] = geoPhoto.Coordinate.Longitude.ToString("0.#####");
                            row["MagneticDirection"] = geoPhoto.ImageDirection.ToString("0.#####");

                            row["FullPath"] = file;

                            this.Rows.Add(row);
                        }
                        catch (ConstraintException constraintException)
                        {
                            if (!constraintException.Message.Contains("FullPath"))
                            {
                                throw;
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }

                    OnProgressUpdate(c, files.Count);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Creates default columns for the photo table
        /// </summary>
        private void CreateColumns()
        {
            this.Columns.Add(new DataColumn("FileName"));
            // this.Columns.Add(new DataColumn("ImageHTML"));
            this.Columns.Add(new DataColumn("FileSize"));
            this.Columns.Add(new DataColumn("Type"));
            this.Columns.Add(new DataColumn("DateTaken"));
            this.Columns.Add(new DataColumn("Modified"));
            this.Columns.Add(new DataColumn("Latitude"));
            this.Columns.Add(new DataColumn("Longitude"));
            this.Columns.Add(new DataColumn("MagneticDirection"));

            DataColumn fullpathColumn = new DataColumn("FullPath");
            this.Columns.Add(fullpathColumn);

            UniqueConstraint uc = new UniqueConstraint(fullpathColumn, false);

            this.Constraints.Add(uc);
        }

        #endregion
    }
}
