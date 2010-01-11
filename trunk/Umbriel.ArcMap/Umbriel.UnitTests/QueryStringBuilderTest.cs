using Umbriel.GIS.Google;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Umbriel.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for QueryStringBuilderTest and is intended
    ///to contain all QueryStringBuilderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class QueryStringBuilderTest
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
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            QueryStringBuilder target = new QueryStringBuilder(); // TODO: Initialize to an appropriate value

            target.AppendMapCenter(-96.097927,47.160832);
            target.SetMapType(QueryStringBuilder.GoogleMapTypes.map);
            target.SetApproximateSpan("0.047738,0.077162");


            
            string actual;
            actual = target.ToString();
            System.Diagnostics.Trace.WriteLine(target.ToString());


            Assert.IsNotNull(target.ToString());

            
            
        }
    }
}
