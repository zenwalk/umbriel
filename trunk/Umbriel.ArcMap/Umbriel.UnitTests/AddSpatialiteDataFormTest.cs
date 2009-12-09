using Umbriel.ArcGIS.Layer.SpatialiteLayer.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Umbriel.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for AddSpatialiteDataFormTest and is intended
    ///to contain all AddSpatialiteDataFormTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AddSpatialiteDataFormTest
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
        ///A test for pictureBoxTarget_Click
        ///</summary>
        [TestMethod()]
        public void DisplayFormTest()
        {
            AddSpatialiteDataForm form = new AddSpatialiteDataForm();
            form.ShowDialog();
            Assert.IsNotNull(form);
        }


        /// <summary>
        ///A test for pictureBoxTarget_Click
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Umbriel.ArcGIS.Layer.SpatialiteLayer.dll")]
        public void pictureBoxTarget_ClickTest()
        {
            AddSpatialiteDataForm_Accessor target = new AddSpatialiteDataForm_Accessor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            EventArgs e = null; // TODO: Initialize to an appropriate value
            target.pictureBoxTarget_Click(sender, e);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
