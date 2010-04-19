using Umbriel.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Umbriel.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for StreetAddressGeneratorTest and is intended
    ///to contain all StreetAddressGeneratorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StreetAddressGeneratorTest
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
        ///A test for GenerateSingleAddress
        ///</summary>
        [TestMethod()]
        public void GenerateSingleAddressTest()
        {
            StreetAddressGenerator target = new StreetAddressGenerator(); 
            
            string actual;
            actual = target.GenerateSingleAddress();
            System.Diagnostics.Trace.WriteLine("GenerateSingleAddressTest: " + actual);
            Assert.IsFalse(string.IsNullOrEmpty(actual));            
        }


        [TestMethod()]
        public void GenerateSingleAddressesTest()
        {
            StreetAddressGenerator target = new StreetAddressGenerator();

            int count = 100;
            int counter = 0;
            for (int i = 0; i < count; i++)
            {
                string actual;
                actual = target.GenerateSingleAddress();
                System.Diagnostics.Trace.WriteLine("GenerateSingleAddressesTest: " + actual);

                if (!string.IsNullOrEmpty(actual))
                {
                    counter++;
                }
            }

            Assert.IsTrue(counter == count);

        }
    }
}
