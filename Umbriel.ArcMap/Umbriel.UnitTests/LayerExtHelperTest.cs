using Umbriel.ArcGIS.Layer.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESRI.ArcGIS.esriSystem;
using System.Data;

namespace Umbriel.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for LayerExtHelperTest and is intended
    ///to contain all LayerExtHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LayerExtHelperTest
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
        ///A test for CreateUmbrielPropertySet
        ///</summary>
        [TestMethod()]
        public void CreateUmbrielPropertySetTest()
        {
            
            IPropertySet actual;
            actual = LayerExtHelper.CreateUmbrielPropertySet();
            Assert.IsNotNull( actual);
            
        }

        /// <summary>
        ///A test for ToDataTable
        ///</summary>
        [TestMethod()]
        public void ToDataTableTest()
        {
            IPropertySet propertySet = Umbriel.ArcGIS.Layer.Util.LayerExtHelper.CreateUmbrielPropertySet();
                       
            DataTable actual;
            actual = LayerExtHelper.ToDataTable(propertySet);
            Assert.IsTrue(actual.Rows.Count > 0);
            
        }
    }
}
