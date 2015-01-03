using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using COTES.ISTOK.Block;

namespace COTES.ISTOK.Tests.Block
{
    //[TestFixture]
    //public class ChannelManagerTests
    //{
    //    [Test]
    //    public void GetAvailableModules__AddCommonChannelProperty()
    //    {
    //        // подменяем модули
    //        var moduleLoader = new TestModuleLoader();
    //        moduleLoader.AddModule("module1", new TestDataLoaderFactory());
    //        moduleLoader.AddModule("module2", new TestDataLoaderFactory());

    //        // тестируемый класс
    //        ChannelManager manager = new ChannelManager(null, null);
    //        manager.ModuleLoaderPrototype = moduleLoader;

    //        var modules = manager.GetAvailableModules();

    //        Assert.IsNotNull(modules);

    //        Assert.AreEqual(2, modules.Length);
    //        for (int i = 0; i < modules.Length; i++)
    //        {
    //            var module = modules[i];
    //            Assert.AreEqual(String.Format("module{0}", i + 1), module.Name);
    //            Assert.AreEqual(null, module.FriendlyName);
    //            AssertProperties(ChannelItem.CommonChannelProperties, module.ChannelProperties);
    //            AssertProperties(ChannelItem.CommonParameterProperties, module.ParameterProperties);
    //        }
    //    }

    //    public void AssertProperties(
    //        IEnumerable<ItemProperty> expected,
    //        IEnumerable<ItemProperty> actual)
    //    {
    //        if (expected == null)
    //        {
    //            Assert.IsNull(actual);
    //        }
    //        else
    //        {
    //            Assert.IsNotNull(actual);
    //        }

    //        var expectedArray = expected.ToArray();
    //        var actualArray = actual.ToArray();

    //        Assert.AreEqual(expectedArray.Count(), actualArray.Count());

    //        for (int i = 0; i < expectedArray.Length; i++)
    //        {
    //            Assert.IsNotNull(expectedArray[i]);
    //            Assert.IsNotNull(actualArray[i]);

    //            Assert.AreEqual(expectedArray[i].Name, actualArray[i].Name);
    //            Assert.AreEqual(expectedArray[i].DisplayName, actualArray[i].DisplayName);
    //            Assert.AreEqual(expectedArray[i].Description, actualArray[i].Description);
    //            Assert.AreEqual(expectedArray[i].Category, actualArray[i].Category);
    //            Assert.AreEqual(expectedArray[i].ValueType, actualArray[i].ValueType);
    //            Assert.AreEqual(expectedArray[i].HasStandardValues, actualArray[i].HasStandardValues);
    //            Assert.AreEqual(expectedArray[i].StandardValuesAreExtinct, actualArray[i].StandardValuesAreExtinct);
    //            Assert.AreEqual(expectedArray[i].DefaultValue, actualArray[i].DefaultValue);
    //        }
    //    }

    //    [Test]
    //    public void GetAvailableModules__CallModulesLoaderInSeparateDomain()
    //    {
    //        var domainAssertation = new TestDomainAssertation();

    //        // подменяем модули
    //        var moduleLoader = new TestModuleLoader();
    //        moduleLoader.DomainAssertation = domainAssertation;

    //        moduleLoader.AddModule("module1", new TestDataLoaderFactory());
    //        moduleLoader.AddModule("module2", new TestDataLoaderFactory());

    //        // тестируемый класс
    //        ChannelManager manager = new ChannelManager(null, null);
    //        manager.ModuleLoaderPrototype = moduleLoader;

    //        var modules = manager.GetAvailableModules();

    //        domainAssertation.AssertIsNotCurrentDomain(TestModuleLoader.LoadInfoDomainID);
    //    }

    //    [Test]
    //    public void StartChannel__CreateDataLoaderInSeparateDomain()
    //    {
    //        var domainAssertation = new TestDomainAssertation();

    //        // подменяем модули
    //        var moduleLoader = new TestModuleLoader();
    //        moduleLoader.DomainAssertation = domainAssertation;

    //        moduleLoader.AddModule("module1", new TestDataLoaderFactory() { Name = "module1" });
    //        moduleLoader.AddModule("module2", new TestDataLoaderFactory() { Name = "module2" });

    //        // тестируемый класс
    //        ChannelManager manager = new ChannelManager(null, null);
    //        manager.ModuleLoaderPrototype = moduleLoader;

    //        var modules = manager.GetAvailableModules();

    //        Assert.IsNotNull(modules);
    //        Assert.AreEqual(2, modules.Length);

    //        for (int i = 0; i < modules.Length; i++)
    //        {
    //            manager.StartChannel(new ChannelInfo() { Id = i, Module = modules[i] });
    //        }
    //        foreach (var item in modules)
    //        {
    //            domainAssertation.AssertIsNotCurrentDomain(item.Name);
    //        }
    //    }

    //    //[Test]
    //    //public void StartChannel__CallChannelItemStart() { Assert.Fail(); }

    //    //[Test]
    //    //public void StopChannel__CallChannelItemStop() { Assert.Fail(); }

    //    //[Test]
    //    //public void StartManager__StartAllChannelWithActiveIsTrue() { Assert.Fail(); }

    //    //[Test]
    //    //public void ReloadChannel__RefreshChannelPropertiesAndParameterLastTime() { Assert.Fail(); }

    //    //[Test]
    //    //public void GetSummaryInfo__ReturnCorrectInfo() { Assert.Fail(); }
    //}
}
