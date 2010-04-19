using Umbriel.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Umbriel.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for RandomDataGeneratorTest and is intended
    ///to contain all RandomDataGeneratorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RandomDataGeneratorTest
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
        ///A test for RandomString
        ///</summary>
        [TestMethod()]
        public void RandomStringTest()
        {
            RandomDataGenerator target = new RandomDataGenerator(); // TODO: Initialize to an appropriate value
            int size = 7; 
            
            for (int i = 0; i < 100; i++)
            {
                string actual  = target.RandomString(size);
                System.Diagnostics.Trace.WriteLine(actual);
            }
            
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RandomString
        ///</summary>
        [TestMethod()]
        public void RandomStringTest1()
        {

            RandomDataGenerator target = new RandomDataGenerator(); // TODO: Initialize to an appropriate value
            int minsize = 4; 
            int maxsize = 20; 

            for (int i = 0; i < 100; i++)
            {
                string actual = target.RandomString(minsize,maxsize);
                System.Diagnostics.Trace.WriteLine(actual);
            }

            Assert.Inconclusive("Verify the correctness of this test method.");

        }
    }
}
