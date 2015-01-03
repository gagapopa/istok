using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLogger;

namespace UnitTestProject.SimpleLogger
{
    /// <summary>
    /// Summary description for DefaultRecordFormatterTest
    /// </summary>
    [TestClass]
    public class DefaultRecordFormatterTest
    {
        public DefaultRecordFormatterTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void DefaultRecordFormatterTest_BaseTest()
        {
            var formatter = new DefaultRecordFormatter("{0} {1} {2} {3}") as IRecordFormatter;
            var result = formatter.FormatRecord(MessageLevel.Critical, DateTime.Now, "log", "message");
            Assert.IsFalse(String.IsNullOrEmpty(result));
            Assert.IsTrue(result.Contains("log"));
            Assert.IsTrue(result.Contains("message"));
        }
    }
}
