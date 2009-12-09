using Umbriel.ArcGIS.Layer.LayerFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Umbriel.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for LayerfileIndexerHelperTest and is intended
    ///to contain all LayerfileIndexerHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LayerfileIndexerHelperTest
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
        ///A test for CreateNewLayerfileIndexDB
        ///</summary>
        [TestMethod()]
        public void CreateNewLayerfileIndexDBTest()
        {
            string path =  @"\\Wit356\gisdata\Layer_Files";
            string returnValue = string.Empty;
            returnValue = LayerfileIndexerHelper.CreateNewLayerfileIndexDB();
            Assert.IsTrue(System.IO.File.Exists(returnValue));
        }
    }
}
