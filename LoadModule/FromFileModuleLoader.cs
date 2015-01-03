using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using COTES.ISTOK.Modules;

namespace COTES.ISTOK.Block
{
    /// <summary>
    /// Класс для загрузки модулей из файла.
    /// </summary>
    /// <remarks>
    /// Класс отмечен как сереализуемый для того, 
    /// что бы при передаче в домен модуля 
    /// производилось копирование прототипа загрузчика
    /// </remarks>
    [Serializable]
    class FromFileModuleLoader : IModuleLoader
    {
        Type baseModuleType = typeof(IDataLoaderFactory);

        public String LoadPath { get; private set; }

        public FromFileModuleLoader(String loadPath)
        {
            this.LoadPath = loadPath;
        }

        public ModuleInfo[] LoadInfo()
        {
            List<ModuleInfo> modules = new List<ModuleInfo>();
            try
            {
                DirectoryInfo dir = new DirectoryInfo(LoadPath);
                foreach (FileInfo file in dir.GetFiles("*.dll"))
                {
                    try
                    {
                        Assembly l = Assembly.LoadFrom(file.FullName);

                        IDataLoaderFactory lmod = GetFactoryFromAssembly(l);
                        if (lmod != null)
                        {
                            ModuleInfo moduleInfo = lmod.Info;

                            moduleInfo.Name = file.Name;// +moduleInfo.Name;

                            modules.Add(moduleInfo);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception) { }
            return modules.ToArray();
        }

        /// <summary>
        /// Найти и инициировать объект IDataLoaderFactory в указанной сборке.
        /// </summary>
        /// <param name="assembly">Сборка для поиска IDataLoaderFactory</param>
        /// <returns>
        /// Инициированный объект IDataLoaderFactory, 
        /// или null - если сборка не содержит подходящего класса.
        /// </returns>
        private IDataLoaderFactory GetFactoryFromAssembly(Assembly assembly)
        {
            IDataLoaderFactory lmod = null;
            Type[] t = assembly.GetTypes();
            for (int j = 0; j < t.Length; j++)
                if (t[j].GetInterface(baseModuleType.Name) != null)
                {
                    try
                    {
                        lmod = (IDataLoaderFactory)Activator.CreateInstance(t[j]);
                        break;
                    }
                    catch { }
                }
            return lmod;
        }

        public IDataLoaderFactory LoadModule(ModuleInfo module)
        {
            string fn = Path.Combine(LoadPath, module.Name);
            Assembly l = Assembly.LoadFrom(fn);

            return GetFactoryFromAssembly(l);
        }
    }

    /// <summary>
    /// Обёртка для загрузчика модулей, для самостоятельной работы загрузчика в отдельном домене.
    /// Применяется для получения информации о всех доступных модулях сбора.
    /// </summary>
    class MarshaledModuleLoader : MarshalByRefObject, IModuleLoader
    {
        /// <summary>
        /// Реальный модуль сбора. 
        /// Должен быть сереализуемым и не маршалед.
        /// </summary>
        public IModuleLoader ModuleLoader { get; set; }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IModuleLoader Members

        public ModuleInfo[] LoadInfo()
        {
            return ModuleLoader.LoadInfo();
        }

        public IDataLoaderFactory LoadModule(ModuleInfo module)
        {
            // По идее, данный класс должен применятся только для вызова LoadInfo(),
            throw new NotSupportedException();
            //return ModuleLoader.LoadModule(module);
        }

        #endregion
    }
}