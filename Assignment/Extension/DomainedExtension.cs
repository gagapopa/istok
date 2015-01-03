using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Extension;

namespace COTES.ISTOK.Assignment.Extension
{
    /// <summary>
    /// Прослойка между ExtensionManager и настоящим расширением,
    /// для того что бы скрыть последнему, что он работает в AppDomain'е
    /// </summary>
    class DomainedExtension : MarshalByRefObject, IExtension
    {
        IExtension extension;

        public DomainedExtension()
        {

        }

        public DomainedExtension(String assemblyName, String typeName)
        {
            this.AssemblyName = assemblyName;
            this.TypeName = typeName;
            LoadExtension();
        }

        public String AssemblyName { get; set; }
        public String TypeName { get; set; }
        public void LoadExtension()
        {
            if (String.IsNullOrEmpty(AssemblyName))
                throw new ArgumentNullException("AssemblyName");
            if (String.IsNullOrEmpty(TypeName))
                throw new ArgumentNullException("TypeName");

            Assembly assembly = Assembly.LoadFrom(AssemblyName);

            Type extensionType = assembly.GetType(TypeName, true);

            ConstructorInfo constructor = extensionType.GetConstructor(new Type[0]);

            if (constructor == null)
                throw new Exception(String.Format("Расширение {1} {0} не может быть загруженно, т.к. не имеет конструктора по умолчанию", AssemblyName, TypeName));

            extension = constructor.Invoke(null) as IExtension;

            if (extension == null)
                throw new Exception(String.Format("Ошибка инициализации расширения {1} {0}", AssemblyName, TypeName));
        }

        public List<String[]> FindExtensionTypes(String path)
        {
            List<String[]> retList = new List<String[]>();

            DirectoryInfo info = new DirectoryInfo(path);

            if (info.Exists)
            {
                FileInfo[] files = info.GetFiles("*.dll");
                String interfaceName = typeof(IExtension).FullName;

                foreach (var item in files)
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFile(item.FullName);
                        Type[] types = assembly.GetTypes();

                        List<Type> typesList = new List<Type>(types);
                        List<Type> extensionTypes = typesList.FindAll(t => t.GetInterface(interfaceName) != null);

                        foreach (var extensionType in extensionTypes)
                        {
                            if (extensionType.IsPublic)
                                retList.Add(new String[] { assembly.CodeBase, extensionType.FullName });
                        }
                    }
                    catch (ReflectionTypeLoadException) { }
                    catch (BadImageFormatException) { }
                }
            }
            return retList;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IExtension Members

        public IExtensionSupplier Supplier
        {
            get
            {
                return extension.Supplier;
            }
            set
            {
                extension.Supplier = value;
            }
        }

        public string Caption
        {
            get { return extension.Caption; }
        }

        public UTypeNode[] ProvidedTypes
        {
            get { return extension.ProvidedTypes; }
        }

        public ExtensionUnitNode NewUnitNode(UTypeNode type)
        {
            return extension.NewUnitNode(type);
        }

        /// <remarks>
        /// DataRow не сереализуемый, обходной костыль для домена  DomainedExtension.NewUnitNode(UTypeNode type,DataTable table,int index)
        /// </remarks>
        public ExtensionUnitNode NewUnitNode(UTypeNode type, DataRow row)
        {
            throw new NotImplementedException();
        }

        public ExtensionUnitNode NewUnitNode(UTypeNode type, DataTable table, int index)// DataRow row)
        {
            //ThreadPool.QueueUserWorkItem(failMethod);
            //new Thread(failMethod).Start();
            return extension.NewUnitNode(type, table.Rows[index]);
        }

        //void failMethod(Object state)
        //{
        //    throw new Exception();
        //}

        public void NotifyChange(ExtensionUnitNode unitNode)
        {
            extension.NotifyChange(unitNode);
        }

        public ItemProperty[] GetProperties(ExtensionUnitNode unitNode)
        {
            return extension.GetProperties(unitNode);
        }

        public bool ListSupported(UTypeNode typeNode)
        {
            return extension.ListSupported(typeNode);
        }

        public bool CodeSupported(UTypeNode typeNode)
        {
            return extension.CodeSupported(typeNode);
        }

        public bool ListCanAdd(UTypeNode typeNode)
        {
            return extension.ListCanAdd(typeNode);
        }

        public EntityStruct[] GetList(ExtensionUnitNode unitNode,UTypeNode type)
        {
            return extension.GetList(unitNode, type);
        }

        public ExtensionDataInfo[] GetAllDataInfo()
        {
            return extension.GetAllDataInfo();
        }

        public ExtensionDataInfo[] GetDataInfo()
        {
            return extension.GetDataInfo();
        }

        public ExtensionDataInfo[] GetDataInfo(ExtensionUnitNode unitNode)
        {
            return extension.GetDataInfo(unitNode);
        }

        public ExtensionData GetData(ExtensionUnitNode unitNode, ExtensionDataInfo extensionDataInfo, DateTime beginTime, DateTime endTime)
        {
            return extension.GetData(unitNode, extensionDataInfo, beginTime, endTime);
        }

        public DataTable GetMessages(MessageCategory filter, DateTime startTime, DateTime endTime)
        {
            return extension.GetMessages(filter, startTime, endTime);
        }

        #endregion
    }
}
