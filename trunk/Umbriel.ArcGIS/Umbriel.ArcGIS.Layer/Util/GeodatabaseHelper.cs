

namespace Umbriel.GIS
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ESRI.ArcGIS.Carto;
    using ESRI.ArcGIS.Catalog;
    using ESRI.ArcGIS.DataSourcesGDB;
    using ESRI.ArcGIS.esriSystem;
    using ESRI.ArcGIS.Geodatabase;
    using ESRI.ArcGIS.Geometry;

    /// <summary>
    /// GeodatabaseHelper static class
    /// </summary>
    public static class GeodatabaseHelper
    {
        /// <summary>
        /// Creates a Featureclass in workspace
        /// </summary>
        /// <param name="objectWorkspace">workspace where the featureclass will be created</param>
        /// <param name="name">the featureclass name</param>
        /// <param name="spatialReference">spatial reference of the new featureclass</param>
        /// <param name="featureType">the type of feature type created</param>
        /// <param name="geometryType">the geometry type of the shape field of the featureclass</param>
        /// <param name="fields">the IFields for the new featureclass</param>
        /// <param name="uidCLSID">GUID CLSID</param>
        /// <param name="uidCLSEXT">extension GUID</param>
        /// <param name="configWord">SDE configuration keyword</param>
        /// <returns>A reference to the new featureclass (IFeatureClass)</returns>
        public static IFeatureClass CreateFeatureClass(
                                            object objectWorkspace,
                                             string name,
                                             ISpatialReference spatialReference,
                                             esriFeatureType featureType,
                                             esriGeometryType geometryType,
                                             IFields fields,
                                             UID uidCLSID,
                                             UID uidCLSEXT,
                                             string configWord)
        {
            // Check for invalid parameters.
            if (objectWorkspace == null)
            {
                throw new Exception("[objectWorkspace] cannot be null");
            }

            if (!((objectWorkspace is IWorkspace) ||
               (objectWorkspace is IFeatureDataset)))
            {
                throw new Exception("[objectWorkspace] must be IWorkspace or IFeatureDataset");
            }

            if (name.Equals(string.Empty))
            {
                throw new Exception("[name] cannot be empty");
            }

            if ((objectWorkspace is IWorkspace) && (spatialReference == null))
            {
                throw new Exception("[spatialReference] cannot be null for StandAlong FeatureClasses");
            }

            // Set ClassID (if Null)
            if (uidCLSID == null)
            {
                uidCLSID = new UIDClass();
                switch (featureType)
                {
                    case esriFeatureType.esriFTSimple:
                        uidCLSID.Value = "{52353152-891A-11D0-BEC6-00805F7C4268}";
                        break;
                    case esriFeatureType.esriFTSimpleJunction:
                        geometryType = esriGeometryType.esriGeometryPoint;
                        uidCLSID.Value = "{CEE8D6B8-55FE-11D1-AE55-0000F80372B4}";
                        break;
                    case esriFeatureType.esriFTComplexJunction:
                        uidCLSID.Value = "{DF9D71F4-DA32-11D1-AEBA-0000F80372B4}";
                        break;
                    case esriFeatureType.esriFTSimpleEdge:
                        geometryType = esriGeometryType.esriGeometryPolyline;
                        uidCLSID.Value = "{E7031C90-55FE-11D1-AE55-0000F80372B4}";
                        break;
                    case esriFeatureType.esriFTComplexEdge:
                        geometryType = esriGeometryType.esriGeometryPolyline;
                        uidCLSID.Value = "{A30E8A2A-C50B-11D1-AEA9-0000F80372B4}";
                        break;
                    case esriFeatureType.esriFTAnnotation:
                        geometryType = esriGeometryType.esriGeometryPolygon;
                        uidCLSID.Value = "{E3676993-C682-11D2-8A2A-006097AFF44E}";
                        break;
                    case esriFeatureType.esriFTDimension:
                        geometryType = esriGeometryType.esriGeometryPolygon;
                        uidCLSID.Value = "{496764FC-E0C9-11D3-80CE-00C04F601565}";
                        break;
                }
            }

            // Set uidCLSEXT (if Null)
            if (uidCLSEXT == null)
            {
                switch (featureType)
                {
                    case esriFeatureType.esriFTAnnotation:
                        uidCLSEXT = new UIDClass();
                        uidCLSEXT.Value = "{24429589-D711-11D2-9F41-00C04F6BC6A5}";
                        break;
                    case esriFeatureType.esriFTDimension:
                        uidCLSEXT = new UIDClass();
                        uidCLSEXT.Value = "{48F935E2-DA66-11D3-80CE-00C04F601565}";
                        break;
                }
            }

            // Add Fields
            if (fields == null)
            {
                // Create fields collection
                fields = new FieldsClass();
                IFieldsEdit fieldsEdit = (IFieldsEdit)fields;

                // Create the geometry field
                IGeometryDef geometryDef = new GeometryDefClass();
                IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;

                // Assign Geometry Definition
                geometryDefEdit.GeometryType_2 = geometryType;
                geometryDefEdit.GridCount_2 = 1;
                geometryDefEdit.set_GridSize(0, 0.5);
                geometryDefEdit.AvgNumPoints_2 = 2;
                geometryDefEdit.HasM_2 = false;
                geometryDefEdit.HasZ_2 = true;
                if (objectWorkspace is IWorkspace)
                {
                    // If this is a STANDALONE FeatureClass then add spatial reference.
                    geometryDefEdit.SpatialReference_2 = spatialReference;
                }

                // Create OID Field
                IField fieldOID = new FieldClass();
                IFieldEdit fieldEditOID = (IFieldEdit)fieldOID;
                fieldEditOID.Name_2 = "OBJECTID";
                fieldEditOID.AliasName_2 = "OBJECTID";
                fieldEditOID.Type_2 = esriFieldType.esriFieldTypeOID;
                fieldsEdit.AddField(fieldOID);

                // Create Geometry Field
                IField fieldShape = new FieldClass();
                IFieldEdit fieldEditShape = (IFieldEdit)fieldShape;
                fieldEditShape.Name_2 = "SHAPE";
                fieldEditShape.AliasName_2 = "SHAPE";
                fieldEditShape.Type_2 = esriFieldType.esriFieldTypeGeometry;
                fieldEditShape.GeometryDef_2 = geometryDef;
                fieldsEdit.AddField(fieldShape);
            }

            // Locate Shape Field
            string stringShapeFieldName = string.Empty;
            for (int i = 0; i <= fields.FieldCount - 1; i++)
            {
                if (fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                {
                    stringShapeFieldName = fields.get_Field(i).Name;
                    break;
                }
            }

            if (stringShapeFieldName == string.Empty)
            {
                throw new Exception("Cannot locate geometry field in FIELDS");
            }

            IFeatureClass featureClass = null;

            if (objectWorkspace is IWorkspace)
            {
                // Create a STANDALONE FeatureClass
                IWorkspace workspace = (IWorkspace)objectWorkspace;
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace;

                featureClass = featureWorkspace.CreateFeatureClass(name, fields, uidCLSID, uidCLSEXT, featureType, stringShapeFieldName, configWord);
            }
            else if (objectWorkspace is IFeatureDataset)
            {
                IFeatureDataset featureDataset = (IFeatureDataset)objectWorkspace;
                featureClass = featureDataset.CreateFeatureClass(name, fields, uidCLSID, uidCLSEXT, featureType, stringShapeFieldName, configWord);
            }

            // Return FeatureClass
            return featureClass;
        } // Creates a Featureclass in workspace

        /// <summary>
        /// Creates a WGS84 spatial reference.
        /// </summary>
        /// <returns>ISpatialReference for WGS84 geo</returns>
        public static ISpatialReference GetWGS84SpatialReference()
        {
            SpatialReferenceEnvironment spatialReferenceEnv = new SpatialReferenceEnvironmentClass();

            IGeographicCoordinateSystem geoCS = spatialReferenceEnv.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
            ISpatialReference spatialReference = geoCS;
            spatialReference.SetFalseOriginAndUnits(-180, -90, 1000000);

            return spatialReference;
        }



    }
}
