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
    /// Summary description for FilterSetTest
    /// </summary>
    [TestClass]
    public class FilterSetTest
    {
        private Mock<IMessageFilter> filter1;
        private Mock<IMessageFilter> filter2;
        private Mock<IMessageFilter> filter3;

        public FilterSetTest()
        {
            filter1 = new Mock<IMessageFilter>();
            filter1.Setup(m => m.IsAccepted(It.Is<MessageLevel>(l => l == MessageLevel.Critical ||
                                                                     l == MessageLevel.Error),
                                            It.Is<string>(j => j == "foo" ||
                                                               j == "baar")))
                   .Returns(false);
            filter1.Setup(m => m.IsAccepted(It.Is<MessageLevel>(l => l == MessageLevel.Debug ||
                                                                     l == MessageLevel.Info ||
                                                                     l == MessageLevel.Warning),
                                            It.Is<string>(j => j == "foo" ||
                                                               j == "baar")))
                   .Returns(true);
            filter1.Setup(m => m.IsAccepted(It.IsAny<MessageLevel>(), 
                          It.Is<string>(j => j != "foo" && j != "baar")))
                   .Returns(true);

            filter2 = new Mock<IMessageFilter>();
            filter2.Setup(m => m.IsAccepted(It.Is<MessageLevel>(l => l == MessageLevel.Debug),
                                            It.IsAny<string>()))
                   .Returns(false);
            filter2.Setup(m => m.IsAccepted(It.Is<MessageLevel>(l => l == MessageLevel.Critical ||
                                                                     l == MessageLevel.Error ||
                                                                     l == MessageLevel.Info ||
                                                                     l == MessageLevel.Warning),
                                            It.IsAny<string>()))
                   .Returns(true);

            filter3 = new Mock<IMessageFilter>();
            filter3.Setup(m => m.IsAccepted(It.Is<MessageLevel>(l => l == MessageLevel.Warning),
                                            It.Is<string>(j => j == "foo")))
                   .Returns(false);
            filter3.Setup(m => m.IsAccepted(It.Is<MessageLevel>(l => l == MessageLevel.Info ||
                                                                     l == MessageLevel.Error ||
                                                                     l == MessageLevel.Debug ||
                                                                     l == MessageLevel.Critical),
                                            It.Is<string>(j => j == "foo")))
                   .Returns(true);
            filter3.Setup(m => m.IsAccepted(It.IsAny<MessageLevel>(),
                                            It.Is<string>(j => j != "foo")))
                   .Returns(true);
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
        public void FilterSetTest_SingleFilterTest()
        {
            FilterSet set = new FilterSet(new [] {filter1.Object});

            Assert.IsFalse(set.IsAccepted(MessageLevel.Critical, "foo"));
            Assert.IsTrue(set.IsAccepted(MessageLevel.Critical, "some else"));
            Assert.IsTrue(set.IsAccepted(MessageLevel.Info, "baar"));
        }

        [TestMethod]
        public void FilterSetTest_TwoFilterTest()
        {
            FilterSet set = new FilterSet(new [] {filter1.Object, filter2.Object});

            Assert.IsFalse(set.IsAccepted(MessageLevel.Debug, "foo"));
            Assert.IsTrue(set.IsAccepted(MessageLevel.Critical, "some else"));
            Assert.IsFalse(set.IsAccepted(MessageLevel.Error, "baar"));
            Assert.IsFalse(set.IsAccepted(MessageLevel.Debug, "some else"));
        }

        [TestMethod]
        public void FilterSetTest_FreeFilterTest()
        {
            FilterSet set = new FilterSet(new []{filter1.Object, filter2.Object, filter3.Object});

            Assert.IsFalse(set.IsAccepted(MessageLevel.Warning, "foo"));
            Assert.IsTrue(set.IsAccepted(MessageLevel.Warning, "baar"));
            Assert.IsFalse(set.IsAccepted(MessageLevel.Critical, "foo"));
            Assert.IsTrue(set.IsAccepted(MessageLevel.Info, "foor"));
            Assert.IsTrue(set.IsAccepted(MessageLevel.Warning, "some else"));
        }
    }
}
