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
    /// Summary description for SyncLogger
    /// </summary>
    [TestClass]
    public class SyncLoggerTest
    {
        private Mock<IJournal> mock_journal;
        private Mock<IMessageFormatter> mock_formatter;
        private Mock<IMessageFilter> mock_filter;

        public SyncLoggerTest()
        {
            mock_journal = new Mock<IJournal>();
            mock_journal.Setup(m => m.Name).Returns("foo");
            mock_journal.Setup(m => m.Write(It.IsAny<MessageLevel>(),
                                            It.IsAny<DateTime>(),
                                            It.IsAny<string>(),
                                            It.IsAny<string>()));

            mock_formatter = new Mock<IMessageFormatter>();
            mock_formatter.Setup(m => m.Format(It.IsAny<MessageLevel>(),
                                               It.IsAny<Exception>())).Returns("foo");
            mock_formatter.Setup(m => m.Format(It.IsAny<MessageLevel>(),
                                               It.IsAny<object>())).Returns("foo");
            mock_formatter.Setup(m => m.Format(It.IsAny<MessageLevel>(),
                                               It.IsAny<object>())).Returns("foo");
            mock_formatter.Setup(m => m.Format(It.IsAny<MessageLevel>(),
                                               It.IsAny<Exception>(),
                                               It.IsAny<object>())).Returns("bar");

            mock_filter = new Mock<IMessageFilter>();
            mock_filter.Setup(m => m.IsAccepted(It.IsAny<MessageLevel>(),
                                                It.IsAny<string>())).Returns(true);
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
        public void SyncLoggerTest_BaseTest()
        {
            var logger = new SyncLogger("some", 
                                        new[] { mock_journal.Object }, 
                                        mock_formatter.Object, 
                                        mock_filter.Object);
            logger.Message(MessageLevel.Critical, new Exception(), "lol");
        }

        [TestMethod]
        public void SyncLoggerTest_StackOverflowBugRegressionTest()
        {
            var logger = new SyncLogger("some",
                                        new[] { mock_journal.Object },
                                        mock_formatter.Object,
                                        mock_filter.Object);

            logger.Message(MessageLevel.Warning, "{0}", "lol");
            logger.Message(MessageLevel.Info, new Exception(), "{0}", "lol");
        }

        [TestMethod]
        public void SyncLoggerText_BraceInMessageBugRegressionTest()
        {
            var logger = new SyncLogger("some",
                                        new[] { mock_journal.Object },
                                        mock_formatter.Object,
                                        mock_filter.Object);

            logger.Message(MessageLevel.Critical, "{ ___");
            logger.Message(MessageLevel.Critical, "} {0} {}}}", 1);
        }
    }
}
