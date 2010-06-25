using Umbriel.ArcGIS.Geodatabase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESRI.ArcGIS.Geodatabase;
using System.Collections.Generic;

namespace Umbriel.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for UtilityTest and is intended
    ///to contain all UtilityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UtilityTest
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
        ///A test for FindFeatureClass
        ///</summary>
        [TestMethod()]
        public void FindFeatureClassTest()
        {
            ArcGISTestHelper testhelper = new ArcGISTestHelper(ESRI.ArcGIS.esriSystem.esriLicenseProductCode.esriLicenseProductCodeArcView);
            testhelper.Initialize();

            IWorkspaceFactory factory = new ESRI.ArcGIS.DataSourcesGDB.SdeWorkspaceFactoryClass();
            IWorkspace workspace = factory.OpenFromFile(@"C:\Documents and Settings\cum30\Application Data\ESRI\ArcCatalog\henricogisdb.DPU.DBMS.FEATURE_QAQC.sde", 0);

            string featureclassName = "MANHOLES";
            
            bool exactMatch = true;

            List<IFeatureClass> actual = Umbriel.ArcGIS.Geodatabase.Utility.FindFeatureClass(workspace, featureclassName, exactMatch);


            System.Diagnostics.Trace.WriteLine("FindFeatureClassTest Count=" + actual.Count.ToString());

            testhelper.Cleanup();

            Assert.AreEqual(actual.Count, 1);

        }
    }
}
