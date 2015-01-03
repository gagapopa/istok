using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using COTES.ISTOK.Block;
using COTES.ISTOK.Modules;

namespace COTES.ISTOK.Tests.Block
{
    [Serializable]
    class TestModuleLoader : IModuleLoader
    {
        public const String LoadInfoDomainID = "TestModuleLoader.LoadInfo";

        #region IModuleLoader Members

        public ModuleInfo[] LoadInfo()
        {
            if (DomainAssertation != null)
            {
                DomainAssertation.RegisterDomain(LoadInfoDomainID, AppDomain.CurrentDomain);
            }

            List<ModuleInfo> modulesList = new List<ModuleInfo>();

            foreach (var moduleName in moduleDictionary.Keys)
            {
                IDataLoaderFactory factory = moduleDictionary[moduleName];
                //ModuleInfo info = new ModuleInfo(
                //    moduleName,
                //    factory.FriendlyName,
                //    factory.GetProperties(),
                //    factory.GetParamProperties());

                ModuleInfo info = factory.Info;
                info.Name = moduleName;

                modulesList.Add(info);
            }

            return modulesList.ToArray();
        }

        public IDataLoaderFactory LoadModule(ModuleInfo module)
        {
            IDataLoaderFactory factory;
            TestDataLoaderFactory testFactory;

            moduleDictionary.TryGetValue(module.Name, out factory);

            if ((testFactory = factory as TestDataLoaderFactory) != null
                && DomainAssertation != null)
            {
                testFactory.DomainAssertation = DomainAssertation;
            }
            return factory;
        }

        #endregion

        Dictionary<String, IDataLoaderFactory> moduleDictionary = new Dictionary<String, IDataLoaderFactory>();

        public void AddModule(String moduleName, IDataLoaderFactory moduleFactory)
        {
            moduleDictionary[moduleName] = moduleFactory;
        }

        public TestDomainAssertation DomainAssertation { get; set; }
    }

    [Serializable]
    class TestDataLoaderFactory : IDataLoaderFactory
    {
        public ItemProperty[] ChannelProperties { get; set; }

        public ItemProperty[] ParamProperties { get; set; }

        public DataLoadMethod[] SupportedLoadMethods { get; set; }

        public String Name { get; set; }

        public string FriendlyName { get; set; }

        public IDataLoader DataLoader { get; set; }

        #region IDataLoaderFactory Members

        //public IEnumerable<ItemProperty> GetProperties()
        //{
        //    return ChannelProperties;
        //}

        //public IEnumerable<ItemProperty> GetParamProperties()
        //{
        //    return ParamProperties;
        //}

        public ModuleInfo Info
        {
            get { return new ModuleInfo(Name, FriendlyName, ChannelProperties, ParamProperties); }
        }

        public IEnumerable<DataLoadMethod> GetSupportedLoadMethods()
        {
            return SupportedLoadMethods;
        }

        public IDataLoader CreateLoader(ChannelInfo channelInfo)
        {
            if (DomainAssertation != null)
            {
                DomainAssertation.RegisterDomain(Name, AppDomain.CurrentDomain);
            }
            if (DataLoader != null)
            {
                return DataLoader;
            }
            else
            {
                return new TestDataLoader();
            }
        }

        #endregion


        public TestDomainAssertation DomainAssertation { get; set; }
    }

    [Serializable]
    class TestDataLoader : IDataLoader
    {
        public void Init(ChannelInfo channelInfo)
        {
            //throw new NotImplementedException();
        }

        public ParameterItem[] GetParameters()
        {
            //throw new NotImplementedException();
            return null;
        }

        public DataLoadMethod LoadMethod
        {
            get
            { 
                //throw new NotImplementedException(); 
                return DataLoadMethod.Current;
            }
        }

        public void RegisterSubscribe()
        {
            //throw new NotImplementedException();
        }

        public void UnregisterSubscribe()
        {
            //throw new NotImplementedException();
        }

        public void GetCurrent()
        {
            //throw new NotImplementedException();
        }

        public void GetArchive(DateTime startTime, DateTime endTime)
        {
            //throw new NotImplementedException();
        }

        public void SetArchiveParameterTime(ParameterItem parameter, DateTime startTime, DateTime endTime)
        {
            //throw new NotImplementedException();
        }

        public void GetArchive()
        {
            //throw new NotImplementedException();
        }

        public IDataListener DataListener { get; set; }
    }
}
