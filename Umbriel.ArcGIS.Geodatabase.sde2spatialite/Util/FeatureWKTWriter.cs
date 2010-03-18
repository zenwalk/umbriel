// <copyright file="FeatureWKTWriter.cs" company="Umbriel Project">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-03-14</date>
// <summary>FeatureclassWKTWriter class file</summary>

namespace Umbriel.ArcGIS.Geodatabase.sde2spatialite
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;

    /// <summary>
    /// FeatureclassWKTWriter class
    /// </summary>
    public class FeatureclassWKTWriter
    {
        #region Fields

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureclassWKTWriter"/> class.
        /// </summary>
        public FeatureclassWKTWriter()
        {
            this.Delimiter = ',';
            this.Limit = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureclassWKTWriter"/> class.
        /// </summary>
        /// <param name="delimiter">The delimiter.</param>
        /// <param name="limit">The limit.</param>
        public FeatureclassWKTWriter(char delimiter, int limit)
        {
            this.Delimiter = delimiter;
            this.Limit = limit;
        }
        #endregion

        #region Events
        /// <summary>
        /// StatusMessage Delegate
        /// </summary>
        /// <param name="message">message text</param>
        public delegate void StatusMessageDelegate(string message);

        /// <summary>
        /// New Data Line Delegate
        /// </summary>
        /// <param name="message">message text</param>
        public delegate void NewDataLineDelegate(string message);

        /// <summary>
        /// Occurs when [status message event].
        /// </summary>
        public event StatusMessageDelegate StatusMessageEvent;

        /// <summary>
        /// Occurs when [new data line event].
        /// </summary>
        public event NewDataLineDelegate NewDataLineEvent;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of the geometry field.
        /// </summary>
        /// <value>The name of the geometry field.</value>
        public string GeometryFieldName { get; set; }

        /// <summary>
        /// Gets or sets the where clause.
        /// </summary>
        /// <value>The where clause.</value>
        public string WhereClause { get; set; }

        /// <summary>
        /// Gets or sets the column names.
        /// </summary>
        /// <value>The column names.</value>
        public string ColumnNames { get; set; }

        /// <summary>
        /// Gets or sets the delimiter.
        /// </summary>
        /// <value>The delimiter.</value>
        public char Delimiter { get; set; }

        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        /// <value>The limit.</value>
        public long Limit { get; set; }
        #endregion

        #region Methods

        #endregion

        /// <summary>
        /// Reads all features and converts to WKT in the geodatabase table
        /// </summary>
        /// <param name="workspace">The ESRI workspace.</param>
        /// <param name="geodatabaseTableName">Name of the geodatabase table.</param>
        public void ReadAll(IWorkspace workspace, string geodatabaseTableName)
        {
            string sql = string.Empty;
            DateTime start = DateTime.Now;
            this.OnUpdateStatusMessage("Read All Data Start: " + start.ToString());

            try
            {
                IFeatureWorkspace wrkspc = (IFeatureWorkspace)workspace;

                IFeatureClass fc = wrkspc.OpenFeatureClass(geodatabaseTableName);
                IDataset ds = (IDataset)fc;

                IGeoDataset geoDS = (IGeoDataset)ds;
                ISpatialReference spatialReference = geoDS.SpatialReference;

                StringBuilder columnHeaders = new StringBuilder("Geometry");

                // prepare a list of field IDs
                List<long> fieldIDs = new List<long>();
                if (this.ColumnNames != null)
                {
                    string[] colNames = this.ColumnNames.Split(this.Delimiter);

                    foreach (string columnName in colNames)
                    {
                        try
                        {
                            int fieldIndex = fc.Fields.FindField(columnName);
                            if (fieldIndex > -1)
                            {
                                columnHeaders.Append(',');
                                columnHeaders.Append(columnName);
                                fieldIDs.Add(fieldIndex);
                            }
                            else
                            {
                                throw new InvalidFieldException("Field Name Does not exist: " + columnName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.StackTrace);
                            throw;
                        }
                    }
                }

                int spatialreferenceWKID = spatialReference.FactoryCode;

                IFeatureCursor cur = null;
                int featureTotalCount = 0;

                bool recycle = true;

                if (this.WhereClause != null)
                {
                    IQueryFilter filter = new QueryFilterClass();
                    filter.WhereClause = this.WhereClause;
                    featureTotalCount = fc.FeatureCount(filter);

                    cur = fc.Search(filter, recycle);
                }
                else
                {
                    featureTotalCount = fc.FeatureCount(null);
                    cur = fc.Search(null, recycle);
                }

                string featureTotalCountStr = featureTotalCount.ToString();

                IFeature feature = null;
                List<string> transactionList = new List<string>();

                long featureCount = 0;

                // TODO: make this an option
                this.OnNewDataLine(columnHeaders.ToString()); // write the column headers out

                while ((feature = cur.NextFeature()) != null)
                {
                    featureCount++;

                    StringBuilder dataLine = new StringBuilder();
                    string wkt = string.Empty;

                    this.OnUpdateStatusMessage("Reading " + featureCount.ToString() + " of " + featureTotalCountStr + "...");

                    if (!feature.Shape.IsEmpty)
                    {
                        IGeometry geom = feature.Shape;
                        int factoryCode = geom.SpatialReference.FactoryCode;

                        try
                        {
                            byte[] geombytes = Util.S2SHelper.ConvertGeometryToWKB(geom);

                            SharpMap.Geometries.IGeometry sharpGeom = SharpMap.Converters.WellKnownBinary.GeometryFromWKB.Parse(geombytes);
                            wkt = SharpMap.Converters.WellKnownText.GeometryToWKT.Write(sharpGeom);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.StackTrace);
                            Console.WriteLine("Exception while converting geometry: " + ex.Message);
                            throw;
                        }
                    }

                    dataLine.Append("\"");
                    dataLine.Append(wkt);
                    dataLine.Append("\"");

                    foreach (long fieldIndex in fieldIDs)
                    {
                        if (!feature.get_Value((int)fieldIndex).Equals(System.DBNull.Value))
                        {
                            string fieldValue = feature.get_Value((int)fieldIndex).ToString();
                            dataLine.Append(',');
                            dataLine.Append("\"");
                            dataLine.Append(fieldValue);
                            dataLine.Append("\"");
                        }
                    }

                    this.OnNewDataLine(dataLine.ToString());

                    if (this.Limit > 0 && featureCount.Equals(this.Limit))
                    {
                        break;
                    }
                }

                DateTime stop = DateTime.Now;

                TimeSpan timeSpan = stop.Subtract(start);

                this.OnUpdateStatusMessage("Read All Data Complete in: " + timeSpan.ToString());

                return;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Called when [new data line].
        /// </summary>
        /// <param name="data">The string of WKT + attribute data</param>
        private void OnNewDataLine(string data)
        {
            if (this.NewDataLineEvent != null)
            {
                this.NewDataLineEvent(data);
            }
        }

        /// <summary>
        /// Called when [update status message].
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnUpdateStatusMessage(string message)
        {
            if (this.StatusMessageEvent != null)
            {
                this.StatusMessageEvent(message);
            }
        }
    }
}
