using Umbriel.ArcGIS.Layer.SpatialiteLayer.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Umbriel.UnitTests
{


    /// <summary>
    ///This is a test class for ConverterTest and is intended
    ///to contain all ConverterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConverterTest
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
        ///A test for CreateTemporaryGeodatabase
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Umbriel.ArcGIS.Layer.SpatialiteLayer.dll")]
        public void CreateTemporaryGeodatabaseTest()
        {
            Converter_Accessor target = new Converter_Accessor(@"c:\temp\swsa.sqlite", "swsa");
            target.CreateTemporaryGeodatabase();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for EsriSpatialReferenceWKT
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Umbriel.ArcGIS.Layer.SpatialiteLayer.dll")]
        public void EsriSpatialReferenceWKTTest_2284()
        {
            Converter_Accessor target = new Converter_Accessor(@"c:\temp\swsa.sqlite", "swsa");
            int epsg = 2284; // TODO: Initialize to an appropriate value
            string expected = "PROJCS[\"NAD83 / Virginia South (ftUS)\",GEOGCS[\"GCS_North_American_1983\",DATUM[\"D_North_American_1983\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Lambert_Conformal_Conic\"],PARAMETER[\"standard_parallel_1\",37.96666666666667],PARAMETER[\"standard_parallel_2\",36.76666666666667],PARAMETER[\"latitude_of_origin\",36.33333333333334],PARAMETER[\"central_meridian\",-78.5],PARAMETER[\"false_easting\",11482916.667],PARAMETER[\"false_northing\",3280833.333],UNIT[\"Foot_US\",0.30480060960121924]]";
            string actual;
            actual = target.EsriSpatialReferenceWKT(epsg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        [DeploymentItem("Umbriel.ArcGIS.Layer.SpatialiteLayer.dll")]
        public void EsriSpatialReferenceWKTTest_4326()
        {
            Converter_Accessor target = new Converter_Accessor(@"c:\temp\swsa.sqlite", "swsa");
            int epsg = 4326; // TODO: Initialize to an appropriate value
            string expected = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223563]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]]";
            string actual;
            actual = target.EsriSpatialReferenceWKT(epsg);
            Assert.AreEqual(expected, actual);
        }



        /// <summary>
        ///A test for CreateTemporaryGeodatabase
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Umbriel.ArcGIS.Layer.SpatialiteLayer.dll")]
        public void CreateTemporaryGeodatabaseTest1()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            Converter_Accessor target = new Converter_Accessor(param0); // TODO: Initialize to an appropriate value
            target.CreateTemporaryGeodatabase();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
