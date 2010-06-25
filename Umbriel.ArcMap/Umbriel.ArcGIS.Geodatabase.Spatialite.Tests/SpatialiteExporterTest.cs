using Umbriel.ArcGIS.Spatialite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESRI.ArcGIS.Carto;
using Proj4Utility;
using System.Linq;



namespace Umbriel.ArcGIS.Geodatabase.Spatialite.Tests
{


    /// <summary>
    ///This is a test class for SpatialiteExporterTest and is intended
    ///to contain all SpatialiteExporterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SpatialiteExporterTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Export
        ///</summary>
        [TestMethod()]
        public void ExportTest()
        {
            SpatialiteExporter target = new SpatialiteExporter(); // TODO: Initialize to an appropriate value
            string spatialiteDatabasePath = string.Empty; // TODO: Initialize to an appropriate value
            ILayer layer = null; // TODO: Initialize to an appropriate value
            target.Export(spatialiteDatabasePath, layer);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }



        [TestMethod]
        public void Proj4UtilityTest()
        {

            var q = (from p in Proj4Utility.Proj4Reader.GetSRIDs().AsEnumerable()
                     select p.AuthorityName).Distinct();

            var srlist = q.ToList();

            foreach (var sr in srlist)
            {
                System.Diagnostics.Trace.WriteLine(sr);
            }



            //foreach (Proj4Reader.Proj4SpatialRefSys item in Proj4Utility.Proj4Reader.GetSRIDs() 
            //{
            //    System.Diagnostics.Trace.WriteLine(item.AuthorityName);
            //}
        }


          [TestMethod]
        public void Proj4UtilityTest2284()
        {
            string wkt = "PROJCS[\"NAD_1983_StatePlane_Virginia_South_FIPS_4502_Feet\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"False_Easting\",11482916.66666666],PARAMETER[\"False_Northing\",3280833.333333333],PARAMETER[\"Central_Meridian\",-78.5],PARAMETER[\"Standard_Parallel_1\",36.76666666666667],PARAMETER[\"Standard_Parallel_2\",37.96666666666667],PARAMETER[\"Latitude_Of_Origin\",36.33333333333334],UNIT[\"Foot_US\",0.3048006096012192]]";

              SharpMap.CoordinateSystems.IInfo info =  SharpMap.Converters.WellKnownText.CoordinateSystemWktReader.Parse(wkt);


              

            //  SharpMap.Converters.WellKnownText.CoordinateSystemWktReader

            //var q = (from p in Proj4Utility.Proj4Reader.GetSRIDs().AsEnumerable()
            //         select p.AuthorityName).Distinct();

            //var srlist = q.ToList();

            //foreach (var sr in srlist)
            //{
            //    System.Diagnostics.Trace.WriteLine(sr);
            //}



            //foreach (Proj4Reader.Proj4SpatialRefSys item in Proj4Utility.Proj4Reader.GetSRIDs() 
            //{
            //    System.Diagnostics.Trace.WriteLine(item.AuthorityName);
            //}
        }


        
    }
}
