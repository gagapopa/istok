using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Класс для управления библиотечными функциями, расположенными во внешних подгружаемых dll
    /// </summary>
    class RemoteFunctionManager : MarshalByRefObject
    {
        private Dictionary<String, MethodInfo> funcs;
        public String FunctionsPath { get; set; }

        public bool ownDomain { get; set; }

        public override object InitializeLifetimeService()
        {
            System.Runtime.Remoting.Lifetime.ILease lease;
            Object ret = base.InitializeLifetimeService();
            if ((lease = ret as System.Runtime.Remoting.Lifetime.ILease) != null) lease.InitialLeaseTime = TimeSpan.Zero;
            return ret;
        }

        public RemoteFunctionManager()
        {
            funcs = new Dictionary<String, MethodInfo>();
        }

        /// <summary>
        /// Создать экзэмпляр класса в отдельном домене
        /// </summary>
        /// <param name="path">Путь, где будут искаться библиотеки</param>
        /// <returns></returns>
        public static RemoteFunctionManager Create(String path)
        {
            AppDomain extDomain = AppDomain.CreateDomain("PluginsDomain", null, path, path, true);
            RemoteFunctionManager manager = (RemoteFunctionManager)extDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location, typeof(RemoteFunctionManager).FullName);
            manager.FunctionsPath = path;
            manager.ownDomain = true;
            return manager;
        }

        public AppDomain GetDomain()
        {
            return AppDomain.CurrentDomain;
        }

        public static void Dispose(RemoteFunctionManager manager)
        {
            AppDomain domain = manager.GetDomain();
            if (domain != AppDomain.CurrentDomain)
                AppDomain.Unload(domain);
        }

        /// <summary>
        /// Получить список библиотечных функций
        /// </summary>
        /// <returns></returns>
        public List<Symbol> LoadFunction()
        {
            string description, category;
            List<Symbol> functions = new List<Symbol>();
            if (new DirectoryInfo(FunctionsPath).Exists)
            {
                foreach (String libraryName in Directory.GetFiles(FunctionsPath, "*.dll", System.IO.SearchOption.TopDirectoryOnly))
                {
                    Assembly tepLibrary = Assembly.LoadFrom(libraryName);
                    foreach (Type type in tepLibrary.GetTypes())
                    {
                        if (type.IsClass && type.IsPublic)
                        {
                            foreach (MethodInfo func in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                            {
                                Type[] types = null;
                                ParameterInfo[] pi = func.GetParameters();

                                description = "";
                                category = Path.GetFileName(libraryName);
                                foreach (DescriptionAttribute item in
                                    func.GetCustomAttributes(typeof(DescriptionAttribute), true))
                                {
                                    description = item.Description;
                                    break;
                                }
                                foreach (CategoryAttribute item in func.GetCustomAttributes(typeof(CategoryAttribute), true))
                                {
                                    category = item.Category;
                                    break;
                                }
                                if (pi != null)
                                {
                                    types = new Type[pi.Length];
                                    for (int i = 0; i < pi.Length; i++) types[i] = pi[i].ParameterType;
                                }
                                ParameterInfo[] pars = func.GetParameters();
                                CalcArgumentInfo[] Parameters = new CalcArgumentInfo[pars.Length];
                                for (int i = 0; i < Parameters.Length; i++)
                                {
                                    String defVal;
                                    Object val = pars[i].DefaultValue;
                                    if (val == null) defVal = null;
                                    else defVal = val.ToString();
                                    Parameters[i] = new CalcArgumentInfo(pars[i].Name, defVal, ParameterAccessor.In);
                                }
                                if (!funcs.ContainsKey(func.Name))
                                {
                                    funcs.Add(func.Name, func);
                                    functions.Add(new LibraryFunction(this, func.Name, Parameters, category, description));
                                }
                            }
                        }
                    }
                }
            }
            return functions;
        }

        /// <summary>
        /// Вызвать библиотечную функцию
        /// </summary>
        /// <param name="functionName">Имя функции</param>
        /// <param name="args">Аргументы</param>
        /// <param name="message">Сообщение в случае ошибки</param>
        /// <returns></returns>
        public SymbolValue CallFunction(String functionName, Object[] args, out CalcMessage message)
        {
            try
            {
                SymbolValue value = SymbolValue.Nothing;
                MethodInfo method;

                if (funcs.TryGetValue(functionName, out method))
                {
                    Object ret = method.Invoke(null, args);
                    value = SymbolValue.CreateValue(ret);
                }
                message = null;
                return value;
            }
            catch (TargetInvocationException exc)
            {
                Exception err = exc.InnerException;
                if (err != null)
                {
                    message = new CalcMessage(MessageCategory.Error, "Ошибка при вызове библиотечной функции {0}, {1}", functionName, err.Message);
                    return SymbolValue.Nothing;
                }
                else throw;
            }
            catch { throw; }
        }
    }
}
