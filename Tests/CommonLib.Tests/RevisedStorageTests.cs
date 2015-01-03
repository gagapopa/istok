using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using COTES.ISTOK;

namespace COTES.ISTOK.Tests
{
    [TestFixture]
    public class RevisedStorageTests
    {
        [Test]
        public void Get_ByRevision_CorrectReturn()
        {
            RevisedStorage<String> storage = new RevisedStorage<String>();

            storage.Set(new RevisionInfo { ID = 4, Time = new DateTime(2012, 09, 1) }, "one");
            storage.Set(new RevisionInfo { ID = 65, Time = new DateTime(2012, 10, 1) }, "two");
            storage.Set(new RevisionInfo { ID = 76, Time = new DateTime(2012, 01, 1) }, "three");
            storage.Set(new RevisionInfo { ID = 86, Time = new DateTime(2012, 07, 1) }, "four");
            storage.Set(new RevisionInfo { ID = 34, Time = new DateTime(2012, 12, 1) }, "five");

            Assert.AreEqual(storage.Get(new RevisionInfo { ID = 65 }), "two");
            Assert.AreEqual(storage.Get(new RevisionInfo { ID = 4 }), "one");
            Assert.AreEqual(storage.Get(new RevisionInfo { ID = 76 }), "three");
            Assert.AreEqual(storage.Get(new RevisionInfo { ID = 86 }), "four");
            Assert.AreEqual(storage.Get(new RevisionInfo { ID = 34 }), "five");
        }

        [Test]
        public void Get_ByTime_CorrectReturn()
        {
            RevisedStorage<String> storage = new RevisedStorage<String>();

            storage.Set(new RevisionInfo { ID = 4, Time = new DateTime(2012, 09, 1) }, "one");
            storage.Set(new RevisionInfo { ID = 65, Time = new DateTime(2012, 10, 1) }, "two");
            storage.Set(new RevisionInfo { ID = 76, Time = new DateTime(2012, 01, 1) }, "three");
            storage.Set(new RevisionInfo { ID = 86, Time = new DateTime(2012, 07, 1) }, "four");
            storage.Set(new RevisionInfo { ID = 34, Time = new DateTime(2012, 12, 1) }, "five");

            Assert.AreEqual(storage.Get(new DateTime(2012, 11, 1)), "two");
            Assert.AreEqual(storage.Get(new DateTime(2012, 09, 18)), "one");
            Assert.AreEqual(storage.Get(new DateTime(2012, 04, 1)), "three");
            Assert.AreEqual(storage.Get(new DateTime(2012, 08, 1)), "four");
            Assert.AreEqual(storage.Get(new DateTime(2012, 12, 21)), "five");
        }

        [Test]
        public void Get_ByTime_CorrectReturnDefault()
        {
            RevisedStorage<String> storage = new RevisedStorage<String>();

            storage.Set(new RevisionInfo { ID = 4, Time = new DateTime(2012, 09, 1) }, "one");
            storage.Set(new RevisionInfo { ID = 65, Time = new DateTime(2012, 10, 1) }, "two");
            storage.Set(new RevisionInfo { ID = 76, Time = new DateTime(2012, 01, 1) }, "three");
            storage.Set(new RevisionInfo { ID = 86, Time = new DateTime(2012, 07, 1) }, "four");
            storage.Set(new RevisionInfo { ID = 34, Time = new DateTime(2012, 12, 1) }, "five");

            Assert.IsNull(storage.Get(new DateTime(2011, 11, 1)));
        }

        [Test]
        public void Get_EmptyStorage_ReturnDefault()
        {
            RevisedStorage<String> storage = new RevisedStorage<String>();

            Assert.IsNull(storage.Get());
        }

        [Test]
        public void Set_EmptyStorage_SetDefault()
        {
            RevisedStorage<String> storage = new RevisedStorage<String>();
          
            storage.Set("one");

            var revisions = (from r in storage select r).ToArray();

            Assert.IsNotNull(revisions);
            Assert.AreEqual(revisions.Length, 1);
            Assert.AreEqual(revisions[0], RevisionInfo.Default);
        }

        [Test]
        public void Set__SetLatest()
        {
            RevisedStorage<String> storage = new RevisedStorage<String>();

            storage.Set(new RevisionInfo { ID = 4, Time = new DateTime(2012, 09, 1) }, "one");
            storage.Set(new RevisionInfo { ID = 65, Time = new DateTime(2012, 10, 1) }, "two");
            storage.Set(new RevisionInfo { ID = 76, Time = new DateTime(2012, 01, 1) }, "three");
            storage.Set(new RevisionInfo { ID = 34, Time = new DateTime(2012, 12, 1) }, "four");
            storage.Set(new RevisionInfo { ID = 86, Time = new DateTime(2012, 07, 1) }, "five");

            storage.Set("latest");

            var revisions = (from r in storage select r).ToArray();

            Assert.IsNotNull(revisions);
            Assert.AreEqual(revisions.Length, 5);

            Assert.AreEqual(storage.Get(new RevisionInfo { ID = 65 }), "two");
            Assert.AreEqual(storage.Get(new RevisionInfo { ID = 4 }), "one");
            Assert.AreEqual(storage.Get(new RevisionInfo { ID = 76 }), "three");
            Assert.AreEqual(storage.Get(new RevisionInfo { ID = 34 }), "latest");
            Assert.AreEqual(storage.Get(new RevisionInfo { ID = 86 }), "five");
        }
    }
}
