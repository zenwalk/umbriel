using Umbriel.ArcMap.Addin.EditorTrack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace EditorTrackUnitTests
{
    
    
    /// <summary>
    ///This is a test class for TrackingFieldsTest and is intended
    ///to contain all TrackingFieldsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TrackingFieldsTest
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


        [TestMethod()]
        public void CreateTrackingFieldsTest()
        {
            string editorTrackFieldsFilePath = @"C:\svn_workspace\umbriel.googlecode.com\trunk\Umbriel.ArcMap.Addin\Umbriel.ArcMap.Addin.EditorTrack\EditorTrackFields.xml";
            TrackingFields target = new TrackingFields(editorTrackFieldsFilePath);

            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for ReadGlobalFields
        ///</summary>
        [TestMethod()]
        public void ReadGlobalFieldsTest()
        {
            //string editorTrackFieldsFilePath = @"C:\svn_workspace\umbriel.googlecode.com\trunk\Umbriel.ArcMap.Addin\Umbriel.ArcMap.Addin.EditorTrack\EditorTrackFields.xml";
            //TrackingFields target = new TrackingFields(editorTrackFieldsFilePath); 
            
            //target.ReadGlobalFields();

            //Assert.IsTrue((target.GlobalOnChangeFields != null && target.GlobalOnCreateFields != null));
        }


        [TestMethod()]
        public void SerializeReplacementTemplate()
        {
            ReplacementTemplate testreplacement = new ReplacementTemplate();

            testreplacement.FeatureclassName = "TestFeatureclassName";
            testreplacement.FieldReplacements.Add("DateCreate", "{Now}");
            testreplacement.FieldReplacements.Add("TESTFCCREATE", "GLB{e:NUMBER_OF_PROCESSORS}");

            

            XmlSerializer serializer = new XmlSerializer(typeof(ReplacementTemplate));
            MemoryStream memstream = new MemoryStream();
            StringWriter stringWriter = new StringWriter();


            serializer.Serialize(stringWriter, testreplacement);

            StreamReader memoryReader = new StreamReader(memstream);



            System.Diagnostics.Trace.WriteLine(stringWriter.ToString());
        }

        [TestMethod()]
        public void ReadReplacementTemplateTest()
        {
            string editorTrackFieldsFilePath = @"C:\svn_workspace\umbriel.googlecode.com\trunk\Umbriel.ArcMap.Addin\Umbriel.ArcMap.Addin.EditorTrack\EditorTrackFields.xml";
            TrackingFields target = new TrackingFields(editorTrackFieldsFilePath);

            target.ReadReplacementTemplates();

            Assert.IsTrue((target.TemplateOnChangeFields != null && target.TemplateOnCreateFields != null));
        }
    }
}
