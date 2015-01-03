using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
using SimpleLogger;

namespace UnitTestProject.SimpleLogger
{
    /// <summary>
    /// Summary description for DefaultMessageFormatterTest
    /// </summary>
    [TestClass]
    public class DefaultMessageFormatterTest
    {
        private IMessageFormatter formatter = new DefaultMessageFormatter();

        public DefaultMessageFormatterTest()
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
        public void DefaultMessageFormatterTest_ValidationResultTest()
        {
            var value = formatter.Format(MessageLevel.Error, "Fooobaaar");
            AssertValue(value);
            value = formatter.Format(MessageLevel.Debug, new Exception("lol", new Exception("lol1")));
            AssertValue(value);
            value = formatter.Format(MessageLevel.Critical, new Exception(), "foooo");
            AssertValue(value);
        }

        [TestMethod]
        public void DefaultMessageFormatterTest_DifferentFormatTest()
        {
            var exception = new Exception();
            var value1 = formatter.Format(MessageLevel.Critical, exception);
            var value2 = formatter.Format(MessageLevel.Info, exception);
            Assert.AreNotEqual(value1, value2);
        }

        private static void AssertValue(string value)
        {
            Assert.AreEqual(String.IsNullOrEmpty(value), false);
            Assert.AreNotEqual(value, "");
        }
    }
}
