// <copyright file="FieldMap.cs" company="Umbriel Project">
// Copyright (c) 2010 All Rights Reserved
// </copyright>
// <author>Jay Cummins</author>
// <email>cumminsjp@gmail.com</email>
// <date>2010-03-14</date>
// <summary>FieldMap class file </summary>

namespace Umbriel.ArcGIS.Geodatabase.sde2spatialite
{
    using System.Data;
    using ESRI.ArcGIS.Geodatabase;

    /// <summary>
    /// Field map data class
    /// </summary>
    internal class FieldMap
    {
        #region Fields

        #endregion
        
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldMap"/> class.
        /// </summary>
        public FieldMap()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldMap"/> class.
        /// </summary>
        /// <param name="featureFieldname">The feature fieldname.</param>
        /// <param name="fieldtype">The fieldtype.</param>
        /// <param name="spatliteFieldName">Name of the spatlite field.</param>
        /// <param name="dbtype">The dbtype.</param>
        public FieldMap(string featureFieldname, esriFieldType fieldtype, string spatliteFieldName, DbType dbtype)
        {
            this.FeatureFieldname = featureFieldname;
            this.SpatialiteFieldname = spatliteFieldName;
            this.FieldType = fieldtype;
            this.DBFieldType = dbtype;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldMap"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="spatliteFieldName">Name of the spatlite field.</param>
        /// <param name="dbtype">The dbtype.</param>
        public FieldMap(IField field, string spatliteFieldName, DbType dbtype)
        {
            this.FeatureFieldname = field.Name;
            this.SpatialiteFieldname = spatliteFieldName;
            this.FieldType = field.Type;
            this.DBFieldType = dbtype;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the feature fieldname.
        /// </summary>
        /// <value>The feature fieldname.</value>
        public string FeatureFieldname { get; set; }

        /// <summary>
        /// Gets or sets the type of the field.
        /// </summary>
        /// <value>The type of the field.</value>
        public esriFieldType FieldType { get; set; }

        /// <summary>
        /// Gets or sets the spatialite fieldname.
        /// </summary>
        /// <value>The spatialite fieldname.</value>
        public string SpatialiteFieldname { get; set; }

        /// <summary>
        /// Gets or sets the type of the DB field.
        /// </summary>
        /// <value>The type of the DB field.</value>
        public DbType DBFieldType { get; set; }
        #endregion

        #region Methods

        #endregion
    }
}
