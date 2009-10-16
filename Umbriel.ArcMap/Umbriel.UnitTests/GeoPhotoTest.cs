using Umbriel.GIS.Photo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Umbriel.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for GeoPhotoTest and is intended
    ///to contain all GeoPhotoTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GeoPhotoTest
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
        ///A test for ReadGPSCoordinate
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Umbriel.GIS.dll")]
        public void ReadGPSCoordinateTest()
        {
            string path = @"C:\temp\geoPhotoTesting\GPS_PhotoRename\090227111307.JPG";
           GeoPhoto_Accessor target = new GeoPhoto_Accessor(path);
            
            target.ReadGPSCoordinate();

            Umbriel.GIS.ISpatialCoordinate coord = target.Coordinate;
            
            System.Diagnostics.Debug.WriteLine(target.ToString());


            Assert.IsNotNull(coord);
        }
    }
}
