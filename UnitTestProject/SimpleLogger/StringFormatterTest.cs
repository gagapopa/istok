using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLogger;

namespace UnitTestProject.SimpleLogger
{
    /// <summary>
    /// Summary description for StringFormatterTest
    /// </summary>
    [TestClass]
    public class StringFormatterTest
    {
        public StringFormatterTest()
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
        public void StringFormatterTest_FormatStringWithoutPlaceholdens()
        {
            string format = @"{{{{}}}} {1212{} 123,-12:kjdfg}";
            string result = StringFormatter.Format(format, 0);
            Assert.AreEqual(format, result);
        }

        [TestMethod]
        public void StringFormatterTest_OneSimplePlaceholden()
        {
            string format = @"###{0}___";
            string result = StringFormatter.Format(format, 0);
            Assert.AreEqual(@"###0___", result);
        }

        [TestMethod]
        public void StringFormatterTest_OnePlaceholdenWithNegativeOffset()
        {
            string format = @"###{0,-2}___";
            string result = StringFormatter.Format(format, 0);
            Assert.AreEqual(@"###0  ___", result);
        }

        [TestMethod]
        public void StringFormatterTest_OnePlaceholdenWithFormat()
        {
            DateTime time = new DateTime(1, 2, 1);
            string format = @"###{0:dd yy MM}___";
            string result = StringFormatter.Format(format, time);
            Assert.AreEqual(@"###01 01 02___", result);
        }

        [TestMethod]
        public void StringFormatterTest_TwoPlaceholden()
        {
            string format = @"###{0}___{1}";
            string result = StringFormatter.Format(format, 1, 2);
            Assert.AreEqual(@"###1___2", result);
        }

        [TestMethod]
        public void StringFormatterTest_DefaultEquality()
        {
            string format = @"{0, -5}";
            
            string result1 = String.Format(format, 12);
            string result2 = StringFormatter.Format(format, 12);

            Assert.AreEqual(result1, result2);
        }
    }
}
