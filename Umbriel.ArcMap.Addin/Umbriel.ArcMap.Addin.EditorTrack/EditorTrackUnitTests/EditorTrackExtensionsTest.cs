using Umbriel.ArcMap.Addin.EditorTrack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EditorTrackUnitTests
{
    
    
    /// <summary>
    ///This is a test class for EditorTrackExtensionsTest and is intended
    ///to contain all EditorTrackExtensionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EditorTrackExtensionsTest
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
        ///A test for FindEnvironmentVariableReplacements
        ///</summary>
        [TestMethod()]
        public void FindEnvironmentVariableReplacementsTest()
        {
            string s = "{e:USERPROFILE},{e:ALLUSERSPROFILE},{e:HOMEDRIVE}";
            
            List<string> envars = EditorTrackExtensions.FindEnvironmentVariableReplacements(s);

            Assert.IsTrue(envars.Count.Equals(3) && 
                envars[0].Equals("USERPROFILE") & envars[1].Equals("ALLUSERSPROFILE") & envars[2].Equals("HOMEDRIVE"));
        }
    }
}
