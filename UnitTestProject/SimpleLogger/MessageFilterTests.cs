using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLogger;

namespace UnitTestProject.SimpleLogger
{
    /// <summary>
    /// Summary description for MessageFilterTests
    /// </summary>
    [TestClass]
    public class MessageFilterTests
    {
        public MessageFilterTests()
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
        public void MessageFilterTest_AcceptAllTest()
        {
            MessageFilter filter = new MessageFilter(MessageLevel.Critical | 
                                                     MessageLevel.Debug | 
                                                     MessageLevel.Error | 
                                                     MessageLevel.Info | 
                                                     MessageLevel.Warning, 
                                                     new string[]{});

            Assert.IsTrue(filter.IsAccepted(MessageLevel.Critical, "Lol"));
            Assert.IsTrue(filter.IsAccepted(MessageLevel.Debug, "some"));
            Assert.IsTrue(filter.IsAccepted(MessageLevel.Error, "pop"));
            Assert.IsTrue(filter.IsAccepted(MessageLevel.Info, "foo"));
            Assert.IsTrue(filter.IsAccepted(MessageLevel.Warning, "bar"));
        }

        [TestMethod]
        public void MessageFilterTest_AcceptForNameTest()
        {
            MessageFilter filter = new MessageFilter(MessageLevel.Info |
                                                     MessageLevel.Warning,
                                                     new[] {"Some"});

            Assert.IsFalse(filter.IsAccepted(MessageLevel.Critical, "Some"));
            Assert.IsTrue(filter.IsAccepted(MessageLevel.Critical, "lol"));
            Assert.IsTrue(filter.IsAccepted(MessageLevel.Info, "lol"));
            Assert.IsTrue(filter.IsAccepted(MessageLevel.Info, "Some"));
        }
    }
}
