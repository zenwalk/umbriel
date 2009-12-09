using Umbriel.ArcGIS.Layer.LayerFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Umbriel.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for LayerfileIndexBuilderTest and is intended
    ///to contain all LayerfileIndexBuilderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LayerfileIndexBuilderTest
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
        ///A test for BuildIndex
        ///</summary>
        [TestMethod()]
        public void BuildIndexTest()
        {
            LayerfileIndexBuilder target = new LayerfileIndexBuilder(); // TODO: Initialize to an appropriate value
            List<string> searchPaths = new List<string>(); // TODO: Initialize to an appropriate value
            searchPaths.Add(@"\\Wit356\gisdata\Layer_Files\Misc");

            target.BuildIndex(searchPaths);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for BuildNewIndex
        ///</summary>
        [TestMethod()]
        public void BuildNewIndexTest()
        {
            LayerfileIndexBuilder target = new LayerfileIndexBuilder(); // TODO: Initialize to an appropriate value
            List<string> searchPaths = new List<string>(); // TODO: Initialize to an appropriate value
            searchPaths.Add(@"\\Wit356\gisdata\Layer_Files");

            target.BuildNewIndex(searchPaths);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
