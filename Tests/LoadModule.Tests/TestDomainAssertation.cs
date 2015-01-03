using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Tests.Block
{
    class TestDomainAssertation : MarshalByRefObject
    {
        Dictionary<String, int> domainDictionary = new Dictionary<string, int>();

        public void RegisterDomain(String id, AppDomain domain)
        {
            domainDictionary[id] = domain.Id;
        }

        public void AssertIsCurrentDomain(String id)
        {
            Assert.IsTrue(domainDictionary.ContainsKey(id));

            Assert.AreEqual(AppDomain.CurrentDomain.Id, domainDictionary[id]);
        }

        public void AssertIsNotCurrentDomain(String id)
        {
            Assert.IsTrue(domainDictionary.ContainsKey(id));

            Assert.AreNotEqual(AppDomain.CurrentDomain.Id, domainDictionary[id]);
        }

        public void AssertAreSameDomain(String id1, String id2)
        {
            Assert.IsTrue(domainDictionary.ContainsKey(id1));
            Assert.IsTrue(domainDictionary.ContainsKey(id2));

            Assert.AreEqual(domainDictionary[id2], domainDictionary[id2]);
        }

        public void AssertAreNotSameDomain(String id1, String id2)
        {
            Assert.IsTrue(domainDictionary.ContainsKey(id1));
            Assert.IsTrue(domainDictionary.ContainsKey(id2));

            Assert.AreNotEqual(domainDictionary[id2], domainDictionary[id2]);
        }
    }
}
