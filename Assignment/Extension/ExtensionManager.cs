using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Extension;
using System.Threading;
using NLog;

namespace COTES.ISTOK.Assignment.Extension
{
    /// <summary>
    /// Класс для управления расширениями обмена данных с внешними системами
    /// </summary>
    class ExtensionManager : MarshalByRefObject, IExtensionSupplier
    {
        Logger log = LogManager.GetCurrentClassLogger();

        public IUnitTypeManager UnitTypeManager { get; set; }

        public IUnitManager UnitManager { get; set; }

        public ValueReceiver ValueReceiver { get; set; }

        /// <summary>
        /// Справочник доменов, в которые загружены расширения
        /// </summary>
        Dictionary<int, AppDomain> domainDictionary;

        /// <summary>
        /// Справочник активных расширении
        /// </summary>
        Dictionary<int, IExtension> extensionDictionary;

        /// <summary>
        /// Справочник типов, предоставляемых расширениями
        /// </summary>
        Dictionary<Guid, int> extensionTypeDictionary = new Dictionary<Guid, int>();

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public ExtensionManager()//IUnitManager manager, ValueReceiver receiver)
        {
            //this.unitManager = manager;
            //this.valueReceiver = receiver;

            extensionDictionary = new Dictionary<int, IExtension>();
            domainDictionary = new Dictionary<int, AppDomain>();

            Load();
        }

        /// <summary>
        /// Загрузить расширения из папки расширений
        /// </summary>
        private void Load()
        {
            AppDomain extensionDomain, domain = AppDomain.CreateDomain("extensionListLoad");

            const String defaultExtensionPath = "Extension";

            // очистить загруженные расширения, если такие есть
            extensionDictionary.Clear();
            foreach (var item in domainDictionary.Values)
            {
                AppDomain.Unload(item);
            }
            domainDictionary.Clear();

            try
            {
                String assignmentCodeBase = typeof(DomainedExtension).Assembly.CodeBase;
                String domainExtensionTypeName = typeof(DomainedExtension).FullName;

                DomainedExtension d = domain.CreateInstanceFromAndUnwrap(assignmentCodeBase, domainExtensionTypeName) as DomainedExtension;

                if (d != null)
                {
                    // в отдельном домене перебрать все файлы в указанной папке и составить список доступных расширений
                    String extensionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, defaultExtensionPath);
                    List<String[]> extensionNameList = d.FindExtensionTypes(extensionPath);
                    
                    // загрузить расширение для каждого элемента в списке
                    // если возникла ошибка при инициализации расширения, пропустить его
                    foreach (String[] item in extensionNameList)
                    {
                        extensionDomain = null;
                        try
                        {
                            if (item != null && item.Length > 1)
                            {
                                // создаем домен
                                extensionDomain = AppDomain.CreateDomain("extensionDomain");
                                extensionDomain.UnhandledException += new UnhandledExceptionEventHandler(extensionDomain_UnhandledException);

                                // инициализируем расширение
                                d = extensionDomain.CreateInstanceFromAndUnwrap(assignmentCodeBase, domainExtensionTypeName) as DomainedExtension;
                                d.AssemblyName = item[0];
                                d.TypeName = item[1];
                                d.LoadExtension();
                                d.Supplier = this;

                                // добавляем расшиение в справочники
                                extensionDictionary[extensionDomain.Id] = d;
                                domainDictionary[extensionDomain.Id] = extensionDomain;
                            }
                        }
                        catch (Exception exc)
                        {
                            log.ErrorException("", exc);
                            if (extensionDomain != null)
                                AppDomain.Unload(extensionDomain);
                        }
                    }
                }
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }

        void extensionDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            AppDomain domain = sender as AppDomain;
            log.ErrorException("Сбой при работе расширения" + domain.FriendlyName, e.ExceptionObject as Exception);
            UnloadExtension(domain.Id);
        }

        /// <summary>
        /// Выгрузить расширения и все упоминания о нем
        /// </summary>
        /// <param name="extensionID">ИД домена, в котором загружено расширение</param>
        private void UnloadExtension(int extensionID)
        {
            IExtension extension = extensionDictionary[extensionID];
            AppDomain domain = domainDictionary[extensionID];
            List<Guid> deleteTypes = new List<Guid>();

            foreach (var item in extensionTypeDictionary.Keys)
            {
                if (extensionTypeDictionary[item] == extensionID)
                    deleteTypes.Add(item);
            }
            foreach (var item in deleteTypes)
            {
                extensionTypeDictionary.Remove(item);
            }

            extensionDictionary.Remove(extensionID);
            domainDictionary.Remove(extensionID);

            AppDomain.Unload(domain);
        }

        /// <summary>
        /// Полуить типы предоставляемые текущими подгруженными расширениями
        /// </summary>
        /// <returns>Горантированное отсутсвие типов с пустым GUID'ом и повторяющихся GUID'ов</returns>
        public UTypeNode[] GetExtensionUnitType()
        {
            IExtension extension;
            List<UTypeNode> typeList = new List<UTypeNode>();
            List<Guid> doubleList;

            // получаем список подгруженных расширений
            List<int> extensionIDList = new List<int>(extensionDictionary.Keys);

            // для каждого расширения получаем список поддерживаемых типов
            // при возникновении проблем выгружаем расширение
            foreach (var extensionID in extensionIDList)
            {
                try
                {
                    if (extensionDictionary.TryGetValue(extensionID,out extension))
                    {
                        UTypeNode[] extensionType = extension.ProvidedTypes;
                        typeList.AddRange(extensionType);

                        foreach (var item in extensionType)
                            extensionTypeDictionary[item.ExtensionGUID] = extensionID; 
                    }
                }
                catch (AppDomainUnloadedException) { }
                catch (Exception exc)
                {
                    log.ErrorException("", exc);
                    UnloadExtension(extensionID);
                }
            }

            // удаление типов без GUID
            typeList.RemoveAll(t => t.ExtensionGUID == Guid.Empty);

            // удаление дублированных GUID
            doubleList = typeList.FindAll(t1 => typeList.Last(t2 => t1.ExtensionGUID == t2.ExtensionGUID) != t1).ConvertAll(t => t.ExtensionGUID);
            typeList.RemoveAll(t => doubleList.Contains(t.ExtensionGUID));

            return typeList.ToArray();
        }

        /// <summary>
        /// Создать новый узел, предоставляемый расширением
        /// </summary>
        /// <param name="type">Тип узла</param>
        /// <returns></returns>
        public ExtensionUnitNode NewUnitNode(UTypeNode type)
        {
            int extensionID;
            IExtension extension;

            if (type.ExtensionGUID == Guid.Empty)
                return null;

            if (extensionTypeDictionary.TryGetValue(type.ExtensionGUID, out extensionID)
                && extensionDictionary.TryGetValue(extensionID, out extension))
                try
                {
                    return extension.NewUnitNode(type);
                }
                catch (AppDomainUnloadedException) { }
                catch (Exception exc)
                {
                    log.ErrorException("", exc);
                    UnloadExtension(extensionID);
                }

            return null;
        }

        /// <summary>
        /// Создать узел, предоставляемый расширением, по информации полученной из БД
        /// </summary>
        /// <param name="type">Тип узла</param>
        /// <param name="row">Данные из БД</param>
        /// <returns></returns>
        public ExtensionUnitNode NewUnitNode(UTypeNode type, DataRow row)
        {
            int extensionID;
            DomainedExtension domainedExtension;
            IExtension extension;

            if (type.ExtensionGUID == Guid.Empty)
                return null;

            if (extensionTypeDictionary.TryGetValue(type.ExtensionGUID, out extensionID)
                && extensionDictionary.TryGetValue(extensionID, out extension))
                try
                {
                    if ((domainedExtension = extension as DomainedExtension) != null)
                        return domainedExtension.NewUnitNode(type, row.Table, row.Table.Rows.IndexOf(row));// row);
                    else return extension.NewUnitNode(type, row);
                }
                catch (AppDomainUnloadedException) { }
                catch (Exception exc)
                {
                    log.ErrorException("", exc);
                    UnloadExtension(extensionID);
                }

            return null;
        }

        /// <summary>
        /// Передать расширению сообщение о том, что узел был изменен
        /// </summary>
        /// <param name="extensionUnitNode">Узел, предоставляемый расширением</param>
        public void NotifyChange(ExtensionUnitNode extensionUnitNode)
        {
            int extensionID;
            IExtension extension;
            UTypeNode typeNode = UnitTypeManager.GetUnitType(new OperationState(), extensionUnitNode.Typ);

            if (typeNode != null && typeNode.ExtensionGUID != Guid.Empty
                && extensionTypeDictionary.TryGetValue(typeNode.ExtensionGUID, out extensionID)
                && extensionDictionary.TryGetValue(extensionID, out extension))
                try
                {
                    extension.NotifyChange(extensionUnitNode);
                }
                catch (AppDomainUnloadedException) { }
                catch (Exception exc)
                {
                    log.ErrorException("", exc);
                    UnloadExtension(extensionID);
                }
        }

        /// <summary>
        /// Поддерживается ли для данного узла выбор ИД из расширения
        /// </summary>
        /// <param name="extensionUnitNode">Узел, предоставляемый расширением</param>
        /// <returns></returns>
        public bool ExternalIDSupported(ExtensionUnitNode extensionUnitNode)
        {
            UTypeNode typeNode = UnitTypeManager.GetUnitType(new OperationState(), extensionUnitNode.Typ);

            return ExternalIDSupported(typeNode);
        }

        public bool ExternalIDSupported(UTypeNode typeNode)
        {
            int extensionID;
            IExtension extension;
            if (typeNode != null && typeNode.ExtensionGUID != Guid.Empty
                && extensionTypeDictionary.TryGetValue(typeNode.ExtensionGUID, out extensionID)
                && extensionDictionary.TryGetValue(extensionID, out extension))
                try
                {
                    return extension.ListSupported(typeNode);
                }
                catch (AppDomainUnloadedException) { }
                catch (Exception exc)
                {
                    log.ErrorException("", exc);
                    UnloadExtension(extensionID);
                }
            return false;
        }

        public bool ExternalCodeSupported(ExtensionUnitNode extensionUnitNode)
        {
            int extensionID;
            IExtension extension;
            UTypeNode typeNode = UnitTypeManager.GetUnitType(new OperationState(), extensionUnitNode.Typ);

            if (typeNode != null && typeNode.ExtensionGUID != Guid.Empty
                && extensionTypeDictionary.TryGetValue(typeNode.ExtensionGUID, out extensionID)
                && extensionDictionary.TryGetValue(extensionID, out extension))
                try
                {
                    return extension.CodeSupported(typeNode);
                }
                catch (AppDomainUnloadedException) { }
                catch (Exception exc)
                {
                    log.ErrorException("", exc);
                    UnloadExtension(extensionID);
                }
            return false;
        }

        /// <summary>
        /// Поддерживается ли добавление нового элемента в список ИД внешнего расширения
        /// </summary>
        /// <param name="extensionUnitNode">Узел, предоставляемый расширением</param>
        /// <returns></returns>
        public bool ExternalIDCanAdd(ExtensionUnitNode extensionUnitNode)
        {
            int extensionID;
            IExtension extension;
            UTypeNode typeNode = UnitTypeManager.GetUnitType(new OperationState(), extensionUnitNode.Typ);

            if (typeNode != null && typeNode.ExtensionGUID != Guid.Empty
                && extensionTypeDictionary.TryGetValue(typeNode.ExtensionGUID, out extensionID)
                && extensionDictionary.TryGetValue(extensionID, out extension))
                try
                {
                    return extension.ListCanAdd(typeNode);
                }
                catch (AppDomainUnloadedException) { }
                catch (Exception exc)
                {
                    log.ErrorException("", exc);
                    UnloadExtension(extensionID);
                }
            return false;
        }

        /// <summary>
        /// Запросить список ИД из расширения
        /// </summary>
        /// <param name="extensionUnitNode">Узел, предоставляемый расширением</param>
        /// <returns></returns>
        public EntityStruct[] GetExternalIDList(ExtensionUnitNode extensionUnitNode)
        {
            int extensionID;
            IExtension extension;
            UTypeNode typeNode = UnitTypeManager.GetUnitType(new OperationState(), extensionUnitNode.Typ);

            if (typeNode != null && typeNode.ExtensionGUID != Guid.Empty
                && extensionTypeDictionary.TryGetValue(typeNode.ExtensionGUID, out extensionID)
                && extensionDictionary.TryGetValue(extensionID, out extension))
                try
                {
                    return extension.GetList(extensionUnitNode, typeNode);
                }
                catch (AppDomainUnloadedException) { }
                catch (Exception exc)
                {
                    log.ErrorException("", exc);
                    UnloadExtension(extensionID);
                }
            return null;
        }

        /// <summary>
        /// Запросить дополнительные свойства узла из расширения
        /// </summary>
        /// <param name="extensionUnitNode">Узел, предоставляемый расширением</param>
        /// <returns></returns>
        public ItemProperty[] GetExternalProperties(ExtensionUnitNode extensionUnitNode)
        {
            int extensionID;
            IExtension extension;
            UTypeNode typeNode = UnitTypeManager.GetUnitType(new OperationState(), extensionUnitNode.Typ);

            if (typeNode != null && typeNode.ExtensionGUID != Guid.Empty
                && extensionTypeDictionary.TryGetValue(typeNode.ExtensionGUID, out extensionID)
                && extensionDictionary.TryGetValue(extensionID, out extension))
                try
                {
                    return extension.GetProperties(extensionUnitNode);
                }
                catch (AppDomainUnloadedException) { }
                catch (Exception exc)
                {
                    log.ErrorException("", exc);
                    UnloadExtension(extensionID);
                }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ExtensionDataInfo[] GetTabInfo()
        {
            List<ExtensionDataInfo> infoList = new List<ExtensionDataInfo>();
            List<IExtension> extensionsList = new List<IExtension>(extensionDictionary.Values);

            // TODO extensionDictionary и многопоточность
            foreach (int extensionID in extensionDictionary.Keys)
            {
                IExtension extension = extensionDictionary[extensionID];
                ExtensionDataInfo[] infos = extension.GetAllDataInfo();
                if (infos != null)
                {
                    SignExtendedTabInfo(extensionID, extension.Caption, infos);

                    infoList.AddRange(infos);
                }
            }
            return infoList.ToArray();
        }

        private static void SignExtendedTabInfo(int extensionID, String extensionCaption, params ExtensionDataInfo[] infos)
        {
            foreach (var item in infos)
            {
                item.ExtensionInfo = new ExtensionInfo(extensionID, extensionCaption);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extensionCaption"></param>
        /// <returns></returns>
        public ExtensionDataInfo[] GetTabInfo(String extensionCaption)
        {
            List<IExtension> extensionsList = new List<IExtension>(extensionDictionary.Values);
            IExtension extension = extensionsList.Find(e => String.Equals(e.Caption, extensionCaption));
           
            if (extension != null)
                try
                {
                    return extension.GetDataInfo();
                }
                catch (AppDomainUnloadedException) { }
                catch (Exception exc)
                {
                    int extensionID = 0;
                    foreach (var item in extensionDictionary.Keys)
                    {
                        if (extensionDictionary[item] == extension)
                        {
                            extensionID = item;
                            break;
                        }
                    }
                    if (extensionID > 0)
                    {
                        log.ErrorException("", exc);
                        UnloadExtension(extensionID);
                    }
                }

            return null;
        }

        public ExtensionDataInfo GetTabInfo(string extensionCaption, string extensionDataName)
        {
            IExtension extension = null;
            ExtensionDataInfo extensionDataInfo;
            int extensionID = -1;

            foreach (var item in extensionDictionary.Keys)
            {
                if (String.Equals(extensionDictionary[item].Caption, extensionCaption))
                {
                    extension = extensionDictionary[item];
                    extensionID = item;
                    break;
                }
            }

            if (extension != null)
            {
                try
                {
                    extensionDataInfo = extension.GetAllDataInfo().First(x => x.Name == extensionDataName);
                    SignExtendedTabInfo(extensionID, extension.Caption, extensionDataInfo);
                    return extensionDataInfo;
                }
                catch (InvalidOperationException) { }
            }
            return null;
        }

        public ExtensionDataInfo GetTabInfo(ExtensionUnitNode extensionUnitNode, string extensionDataName)
        {
            int extensionID;
            IExtension extension;
            UTypeNode typeNode = UnitTypeManager.GetUnitType(new OperationState(), extensionUnitNode.Typ);
            ExtensionDataInfo extensionDataInfo;

            if (typeNode != null && typeNode.ExtensionGUID != Guid.Empty
                && extensionTypeDictionary.TryGetValue(typeNode.ExtensionGUID, out extensionID)
                && extensionDictionary.TryGetValue(extensionID, out extension))
            {
                try
                {
                    extensionDataInfo = extension.GetAllDataInfo().First(x => x.Name == extensionDataName);
                    SignExtendedTabInfo(extensionID, extension.Caption, extensionDataInfo);
                    return extensionDataInfo;
                }
                catch (InvalidOperationException) { }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extensionCaption"></param>
        /// <param name="tabKeyword"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ExtensionData GetTab(ExtensionDataInfo extensionDataInfo, DateTime beginTime, DateTime endTime)
        {
            List<IExtension> extensionsList = new List<IExtension>(extensionDictionary.Values);
            IExtension extension = extensionsList.Find(e => String.Equals(e.Caption, extensionDataInfo.ExtensionInfo.Caption));

            if (extension != null)
                try
                {
                    return extension.GetData(null, extensionDataInfo, beginTime, endTime);
                }
                catch (AppDomainUnloadedException) { }
                catch (Exception exc)
                {
                    int extensionID = 0;
                    foreach (var item in extensionDictionary.Keys)
                    {
                        if (extensionDictionary[item] == extension)
                        {
                            extensionID = item;
                            break;
                        }
                    }
                    if (extensionID > 0)
                    {
                        log.ErrorException("", exc);
                        UnloadExtension(extensionID);
                    }
                }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extensionUnitNode">Узел, предоставляемый расширением</param>
        /// <returns></returns>
        public ExtensionDataInfo[] GetTabInfo(ExtensionUnitNode extensionUnitNode)
        {
            int extensionID;
            IExtension extension;
            UTypeNode typeNode = UnitTypeManager.GetUnitType(new OperationState(), extensionUnitNode.Typ);

            if (typeNode != null && typeNode.ExtensionGUID != Guid.Empty
                && extensionTypeDictionary.TryGetValue(typeNode.ExtensionGUID, out extensionID)
                && extensionDictionary.TryGetValue(extensionID, out extension))
                try
                {
                    return extension.GetDataInfo(extensionUnitNode);
                }
                catch (AppDomainUnloadedException) { }
                catch (Exception exc)
                {
                    log.ErrorException("", exc);
                    UnloadExtension(extensionID);
                }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extensionUnitNode">Узел, предоставляемый расширением</param>
        /// <param name="tabKeyword"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ExtensionData GetTab(ExtensionUnitNode extensionUnitNode, ExtensionDataInfo extensionDataInfo, DateTime beginTime, DateTime endTime)
        {
            int extensionID;
            IExtension extension;
            UTypeNode typeNode = UnitTypeManager.GetUnitType(new OperationState(), extensionUnitNode.Typ);

            if (typeNode != null && typeNode.ExtensionGUID != Guid.Empty
                && extensionTypeDictionary.TryGetValue(typeNode.ExtensionGUID, out extensionID)
                && extensionDictionary.TryGetValue(extensionID, out extension))
                try
                {
                    return extension.GetData(extensionUnitNode, extensionDataInfo, beginTime, endTime);
                }
                catch (AppDomainUnloadedException) { }
                catch (Exception exc)
                {
                    log.ErrorException("", exc);
                    UnloadExtension(extensionID);
                }

            return null;
        }

        public DataTable GetMessages(MessageCategory filter, DateTime beginTime, DateTime endTime)
        {
            DataTable retTable = new DataTable();

            foreach (IExtension extension in extensionDictionary.Values)
            {
                try
                {
                    DataTable table = extension.GetMessages(filter, beginTime, endTime);
                    if (table != null)
                    {
                        retTable.Merge(table);
                    }
                }
                catch (Exception exc)
                {
                    log.ErrorException("", exc);
                }
            }
            return retTable;
        }

        #region PublishValues

        Thread valuesChangedEventThread;

        public void Start()
        {
            valuesChangedEventSendlerStarted = true;
            ThreadPool.QueueUserWorkItem(ValuesChangedEventSendler);
        }

        public void Stop()
        {
            valuesChangedEventSendlerStarted = false;
            if (valuesChangedEventThread != null)
            {
                valuesChangedEventThread.Interrupt();
                if (!valuesChangedEventThread.Join(1000))
                    valuesChangedEventThread.Abort();
                valuesChangedEventThread = null;
            }
        }


        public void ValuesChanged(UnitNode[] nodes)
        {
#if EMA
            ParameterNode paramNode;
            foreach (var item in nodes)
            {
                if ((paramNode = item as ParameterNode) != null
                    && paramNode.ValueReceive)
                {
                    OnValuesChanged();
                    return;
                }
            } 
#endif
        }

        /// <summary>
        /// Время, которое мьютекс остается свободным во время вызова события
        /// </summary>
        TimeSpan eventDelay = TimeSpan.FromSeconds(1);

        private void OnValuesChanged()
        {
            systemEvent.Set();
        }

        EventWaitHandle systemEvent = new EventWaitHandle(false, EventResetMode.AutoReset);

        bool valuesChangedEventSendlerStarted;
        public void ValuesChangedEventSendler(Object state)
        {
            //создать мьютекс
            using (Mutex dataReadyMutex =
                new Mutex(true, COTES.ISTOK.ParameterReceiverExtension.ISTOKExtension.IstokDataReadMutexName))
            {
                while (valuesChangedEventSendlerStarted)
                {
                    try
                    {
                        systemEvent.WaitOne();
                        Thread.Sleep(2000);
                        // здесь вызывается события во внешнем приложении
                        dataReadyMutex.ReleaseMutex();
                        Thread.Sleep(eventDelay);
                        dataReadyMutex.WaitOne();
                        //Thread.Sleep(sendDelay);
                    }
                    catch (ThreadInterruptedException) { }
                    catch { } 
                }
            }
        }
        #endregion

        #region IExtensionSupplier Members

        ExtensionUnitNode IExtensionSupplier.GetParent(ExtensionUnitNode unitNode, Guid parentTypeGUID)
        {
            int parentType = UnitTypeManager.GetExtensionType(parentTypeGUID);

            return UnitManager.GetParentNode(new OperationState(), unitNode, parentType) as ExtensionUnitNode;
        }

        UTypeNode IExtensionSupplier.GetTypeNode(int unitTypeId)
        {
            return UnitTypeManager.GetUnitType(new OperationState(), unitTypeId);
        }

        #endregion

        public ParameterNode[] GetSourceParameters()
        {
            List<ParameterNode> sourceParameters = new List<ParameterNode>();

#if EMA
            foreach (ParameterNode param in unitManager.GetParameters())
            {
                if (param.ValueReceive)
                    sourceParameters.Add(param);
    }
#endif

            return sourceParameters.ToArray();
        }
    }
}
