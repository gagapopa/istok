using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleLogger;

namespace UnitTestProject.SimpleLogger
{
    /// <summary>
    /// Summary description for LoggerContainerTest
    /// </summary>
    [TestClass]
    public class LoggerContainerTest
    {
        #region Test
        public LoggerContainerTest()
        {
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

        #endregion Test

        [TestMethod]
        public void LoggerContainerTest_LogerExistTest()
        {
            var mock = new Mock<ILogger>();
            var container = new LoggerContainer(mock.Object);
            container.Message(MessageLevel.Debug, new Exception());
        }

        [TestMethod]
        public void LoggerContainerTest_LogerNotExistTest()
        {
            var container = new LoggerContainer(null);
            container.Message(MessageLevel.Critical, new Exception());
        }
    }
}
