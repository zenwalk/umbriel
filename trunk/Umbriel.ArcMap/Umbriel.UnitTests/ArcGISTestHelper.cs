
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;


internal class ArcGISTestHelper
{
    public IFeatureClass FeatureClassA { get; set; }
    public IFeatureClass FeatureClassB { get; set; }

    // public  esriLicenseProductCode ESRILicenseProductCode { get; set; }
    public GeoprocessingInDotNet.LicenseInitializer ESRILicenseInitializer { get; set; }
    public IFeatureWorkspace FeatureWorkspace;


    private esriLicenseProductCode ESRILicenseProductCode { get; set; }

    public ArcGISTestHelper()
    {
        this.ESRILicenseProductCode = esriLicenseProductCode.esriLicenseProductCodeArcView;

        Initialize();
        
    }

    public ArcGISTestHelper(esriLicenseProductCode licenseProductCode)
    {
        this.ESRILicenseProductCode = licenseProductCode;

        ESRILicenseProductCode = licenseProductCode;

        Initialize();
        
    }

    public void Initialize()
    {
        // ESRILicenseProductCode = esriLicenseProductCode.esriLicenseProductCodeArcView;

        ESRILicenseInitializer = new GeoprocessingInDotNet.LicenseInitializer();
        ESRILicenseInitializer.InitializeApplication(ESRILicenseProductCode);

        if (ESRILicenseInitializer.InitializedProduct > 0)
        {

        }
        else
        {
            throw new Exception("ESRI License Not Initialized");
        }
    }

    public void Cleanup()
    {
        ESRILicenseInitializer.ShutdownApplication();
    }

}