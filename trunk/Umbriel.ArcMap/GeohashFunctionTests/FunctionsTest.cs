using Umbriel.GIS.GeohashFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlTypes;
using System;

namespace GeohashFunctionTests
{
    
    
    /// <summary>
    ///This is a test class for FunctionsTest and is intended
    ///to contain all FunctionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FunctionsTest
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
        ///A test for EncodeCoordinate
        ///</summary>
        [TestMethod()]
        public void EncodeCoordinateTest()
        {
            SqlDouble latitude = new SqlDouble(37.1051439);
            SqlDouble longitude = new SqlDouble(-79.4122490);
            SqlString expected = new SqlString("dnxe0fvc0kpkg");             
            SqlString actual;
            actual = Functions.EncodeCoordinate(latitude, longitude);
            Assert.AreEqual(expected, actual);
            
        }

        /// <summary>
        ///A test for DecodeGeohash
        ///</summary>
        [TestMethod()]
        public void DecodeGeohashTest()
        {
            SqlString geohash = new SqlString("dnxe0fvc0kpkg");
            SqlDouble[] expected = { 37.1051439, -79.4122490 };
            SqlDouble[] actual;
            actual = Functions.DecodeGeohash(geohash);

            System.Diagnostics.Trace.WriteLine(Math.Round((double)expected[0], 8));
            System.Diagnostics.Trace.WriteLine(Math.Round((double)actual[0], 8));

            Assert.IsTrue((Math.Round((double)expected[0], 7) == Math.Round((double)actual[0], 7)));

        }
    }
}
