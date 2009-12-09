using Umbriel.GIS.LMStat;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SqlClient;

namespace Umbriel.UnitTests
{


    /// <summary>
    ///This is a test class for LMStatParserTest and is intended
    ///to contain all LMStatParserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LMStatParserTest
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
        ///A test for Parse
        ///</summary>
        [TestMethod()]
        public void ParseTest()
        {
            LMStatParser target = new LMStatParser(@"\\wit356\GISProjects\GIS\LM\lmstat.txt");
            DataTable actual;
            actual = target.Parse();
            Assert.IsTrue(actual.Rows.Count > 0);
        }


        /// <summary>
        ///A test for Parse
        ///</summary>
        [TestMethod()]
        public void ParseDataLoadTest()
        {
            // string connectionString = @"Provider=SQLNCLI10;Server=dpu-pbu-gism\SQLExpress;Database=working; Trusted_Connection=yes;";

            string connectionString = @"Server=dpu-pbu-gism\SQLExpress;Database=working;Trusted_Connection=True;";


            LMStatParser target = new LMStatParser(@"\\wit356\GISProjects\GIS\LM\lmstat.txt");
            DataTable actual;
            actual = target.Parse();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlSelect = "SELECT * FROM DBO.LMStat";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlSelect, connection))
                    {
                        using (new SqlCommandBuilder(adapter))
                        {
                            LMStatTable lmstatDatabaseTable = new LMStatTable();
                            adapter.Fill(lmstatDatabaseTable);

                            lmstatDatabaseTable.AddRows(actual.Rows);

                            adapter.Update(lmstatDatabaseTable);
                        }
                    }



                    connection.Close();
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                throw;
            }



            Assert.IsTrue(actual.Rows.Count > 0);

        }


        /// <summary>
        ///A test for ParseLicenseName
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Umbriel.GIS.dll")]
        public void ParseLicenseNameTest()
        {
            
            LMStatParser_Accessor target = new LMStatParser_Accessor(); // TODO: Initialize to an appropriate value
            string line = @"Users of ARC/INFO:  (Total of 50 licenses issued;  Total of 4 licenses in use)";
            string expected = @"ARC/INFO";
            string actual;
            actual = target.ParseLicenseName(line);
            Assert.AreEqual(expected, actual);

        }


        /// <summary>
        ///A test for ParceTotalLicenses
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Umbriel.GIS.dll")]
        public void ParceTotalLicensesTest()
        {
            LMStatParser_Accessor target = new LMStatParser_Accessor(); // TODO: Initialize to an appropriate value
            string line = @"Users of ARC/INFO:  (Total of 50 licenses issued;  Total of 4 licenses in use)";
            int expected = 50;
            int actual;
            actual = target.ParseTotalLicenses(line);
            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        ///A test for ParseInUseLicenses
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Umbriel.GIS.dll")]
        public void ParseInUseLicensesTest()
        {
            LMStatParser_Accessor target = new LMStatParser_Accessor(); // TODO: Initialize to an appropriate value
            string line = @"Users of ARC/INFO:  (Total of 50 licenses issued;  Total of 4 licenses in use)";
            int expected = 4;
            int actual;
            actual = target.ParseInUseLicenses(line);
            Assert.AreEqual(expected, actual);

        }




        /// <summary>
        ///A test for ParseMachineName
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Umbriel.GIS.dll")]
        public void ParseMachineNameTest()
        {
            LMStatParser_Accessor target = new LMStatParser_Accessor(@"\\wit356\GISProjects\GIS\LM\lmstat.txt");
            string expected = "27004@wit356";
            string actual;
            actual = target.ParseMachineName();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Parse
        ///</summary>
        [TestMethod()]
        public void ParseTest1()
        {
            LMStatParser target = new LMStatParser(); // TODO: Initialize to an appropriate value
            DataTable expected = null; // TODO: Initialize to an appropriate value
            DataTable actual;
            actual = target.Parse();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
