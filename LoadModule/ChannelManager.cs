using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using COTES.ISTOK.DiagnosticsInfo;
using NLog;

namespace COTES.ISTOK.Block
{
    /// <summary>
    /// Класс, отвечающий за загрузку/выгрузку и работоспособность каналов
    /// </summary>
    public partial class ChannelManager : ISummaryInfo
    {
        /// <summary>
        /// Текущие загруженные/запущенные каналы.
        /// null в словаре - канал не запущен
        /// </summary>
        Dictionary<ChannelInfo, ChannelItem> channelsDictionary = new Dictionary<ChannelInfo, ChannelItem>();

        private DALManager dalManager = null;
        private Logger log = LogManager.GetCurrentClassLogger();
        private volatile bool isStarted;

        private TimeSpan channelKeepTimeout = TimeSpan.FromMinutes(1);
        private Thread channelKeepThread;

        public Exception ChannelKeepException { get; protected set; }

        DataListener listener;

        public ChannelManager(DALManager dalManager, ValueBuffer valBuffer)
        {
            this.dalManager = dalManager;

            isStarted = false;
            ChannelKeepException = null;
            listener = new DataListener(valBuffer);
            listener.ChannelQueueOverflow += listener_ChannelBufferOverloaded;
            listener.ChannelQueueEmpted += listener_ChannelBufferSpaced;
        }

        void listener_ChannelBufferOverloaded(ChannelInfo channelInfo)
        {
            ChannelItem channel;

            if (channelsDictionary.TryGetValue(channelInfo, out channel)
                && channel != null)
            {
                log.Warn("Канал {0} перегружен и будет приостановлен.", channelInfo);
                StopChannel(channelInfo);
            }
        }

        void listener_ChannelBufferSpaced(ChannelInfo channelInfo)
        {
            log.Warn("Работа приостановленного канал {0} возобновляется.", channelInfo);
            StartChannel(channelInfo);
        }

        #region Диагностика
        const string caption = "Менеджер каналов";

        public DataSet GetSummaryInfo()
        {
            DataSet ds = new DataSet(caption);
            DataTable channelTable, parameterTable;
            DataRow row;

            // создаём таблицу с информацией о каналах
            channelTable = new DataTable("Каналы");
            DataColumn parentChannelColumn = channelTable.Columns.Add("Номер", typeof(int));
            channelTable.Columns.Add("Название", typeof(string));
            channelTable.Columns.Add("Библиотека", typeof(string));
            channelTable.Columns.Add("Активен", typeof(bool));
            channelTable.Columns.Add("Загружен", typeof(bool));
            channelTable.Columns.Add("Запись в БД", typeof(bool));
            channelTable.Columns.Add("Остановлен", typeof(bool));
            channelTable.Columns.Add("Длина очереди", typeof(int));
            channelTable.Columns.Add("Метод", typeof(String));
            channelTable.Columns.Add("Время начала", typeof(DateTime));
            channelTable.Columns.Add("Время конца", typeof(DateTime));
            channelTable.Columns.Add("Последняя активность", typeof(DateTime));

            ds.Tables.Add(channelTable);

            // создаём таблицу с информацией о параметрах
            parameterTable = new DataTable("Параметры");
            parameterTable.Columns.Add("Номер", typeof(int));
            parameterTable.Columns.Add("Название", typeof(string));
            DataColumn childChannelColumn = parameterTable.Columns.Add("Канал", typeof(int));
            parameterTable.Columns.Add("Время", typeof(DateTime));
            parameterTable.Columns.Add("Дней хранения", typeof(int));

            ds.Tables.Add(parameterTable);

            // связь между каналами
            ds.Relations.Add(parentChannelColumn, childChannelColumn);

            // заполняем таблицы
            foreach (var channelInfo in channelsDictionary.Keys)
            {
                ChannelItem channel = channelsDictionary[channelInfo];

                row = channelTable.Rows.Add(
                    channelInfo.Id,
                    channelInfo.Name,
                    channelInfo.Module.Name,
                    channelInfo.Active,
                    channel != null,
                    channelInfo.Storable,
                    channelInfo.Suspended,
                    listener.GetChannelBufferCount(channelInfo),
                    new DataLoadMethodTypeConverter().ConvertToString(channel == null ? DataLoadMethod.Current : channel.LoadMethod),
                    channel == null ? DateTime.MinValue : channel.StartTime,
                    channel == null ? DateTime.MinValue : channel.EndTime,
                    channel == null ? DateTime.MinValue : channel.LastActivityTime);

                foreach (var item in channelInfo.Parameters)
                {
                    row = parameterTable.Rows.Add(
                        item.Idnum,
                        item.Name,
                        channelInfo.Id,
                        item.LastTime,
                        item.Store_days);
                }
            }

            //DataTable table = GetState();
            //if (table != null) ds.Tables.Add(table);

            return ds;
        }

        public string GetSummaryCaption()
        {
            return caption;
        }

        //public DataTable GetState()
        //{
        //    DataTable retDataTable = null;
        //    var channels = GetLoadedChannels();
        //    ChannelItem channel;

        //    foreach (ChannelInfo info in channels)
        //    {
        //        if (channelsDictionary.TryGetValue(info, out channel) && channel != null)
        //        {
        //            DataTable table = channel.GetState();
        //            if (retDataTable == null) retDataTable = table;
        //            else retDataTable.Load(table.CreateDataReader());
        //        }
        //    }
        //    return retDataTable;
        //}

        //public DataTable GetState(ChannelInfo channelInfo)
        //{
        //    ChannelItem item = null;

        //    channelsDictionary.TryGetValue(channelInfo, out item);
        //    //try
        //    //{
        //    //    item = GetChannel(id);
        //    //}
        //    //catch { }

        //    if (item == null) return null;
        //    return item.GetState();
        //}
        #endregion

        #region Запуск/Остановка ChannelManager'а

        IEnumerable<ModuleInfo> availableModuels;

        /// <summary>
        /// Запустить управление каналами
        /// </summary>
        public void StartManager()
        {
            if (isStarted) return;
            // загрузка данных каналов
            IEnumerable<ChannelInfo> channels;

            availableModuels = GetAvailableModules();

            channels = dalManager.GetChannels(availableModuels);

            foreach (var item in channels)
            {
                //foreach (var parameter in item.Parameters)
                //{
                //    var valueItem = NSI.valReceiver.GetLastValue(parameter.Idnum);

                //    if (valueItem != null)
                //    {
                //        parameter.LastTime = valueItem.Time;
                //    }
                //}

                channelsDictionary.Add(item, null);
            }
            StartChannels();

            channelKeepThread = new Thread(new ThreadStart(channelKeeper));
            channelKeepThread.Name = "Keeper";

            isStarted = true;
            channelKeepThread.Start();

        }

        /// <summary>
        /// Остановить управление каналами
        /// </summary>
        public void StopManager()
        {
            isStarted = false;
            if (channelKeepThread != null)
            {
                if (channelKeepThread.ThreadState != ThreadState.Stopped)
                {
                    channelKeepThread.Interrupt();
                    channelKeepThread.Join();
                }
            }
            // остановка всех каналов сброс данных
            StopChannels();
            channelsDictionary.Clear();
        }

        public ChannelInfo ReloadChannel(ChannelInfo channelInfo)
        {
            if (channelsDictionary.ContainsKey(channelInfo))
            {
                StopChannel(channelInfo);

                channelsDictionary.Remove(channelInfo);
            }

            availableModuels = GetAvailableModules();

            ChannelInfo info = (from i in dalManager.GetChannels(availableModuels)
                                where i.Id == channelInfo.Id
                                select i).First();

            channelsDictionary[info] = null;
            return info;
        }


        /// <summary>
        /// Проверить запущено ли управление каналами
        /// </summary>
        public bool IsStarted
        {
            get
            {
                if (ChannelKeepException != null)
                {
                    Exception exc = ChannelKeepException;
                    ChannelKeepException = null;
                    throw exc;
                }
                return isStarted;
            }
        }

        /// <summary>
        /// Метод, выполняемый в отдельном потоки, проверяющий состояние каналов 
        /// и по необходимости их перезапускающий
        /// </summary>
        private void channelKeeper()
        {
            //bool req = false;
            bool skiponce = false; // если Exception появляется до слипа, то принудительный слип в начале потока
            DateTime nowDateTime;
            while (isStarted)
            {
                bool donotlog = false;
                try
                {
                    if (skiponce)
                    {
                        Thread.Sleep(channelKeepTimeout);
                        skiponce = false;
                    }

                    nowDateTime = DateTime.Now;

                    foreach (var channelInfo in GetLoadedChannels())
                        try
                        {
                            if (channelInfo.Active && !channelInfo.Suspended && channelInfo.Parameters.Count() > 0)
                            {
                                ChannelItem channel = channelsDictionary[channelInfo];
                                //TimeSpan channelLag;
                                if (channel == null
                                    || (nowDateTime - channel.LastActivityTime) > NSI.moduleLifeTime)
                                {
                                    if (log.IsErrorEnabled)
                                    {
                                        if (channel == null)
                                        {
                                            log.Error("Перезапуск канала {0} из-за его падения", channelInfo);
                                        }
                                        else
                                        {
                                            log.Error("Перезапуск канала {0}, т.к. он давно не отвечал на запросы (последняя активность: {2}, задержка: {1})", channelInfo, nowDateTime - channel.LastActivityTime, channel.LastActivityTime);
                                        }
                                    }
                                    ReloadChannel(channelInfo);
                                    StartChannel(channelInfo);
                                }
                            }
                        }
                        catch (System.Runtime.Remoting.RemotingException) { }
                    Thread.Sleep(channelKeepTimeout);
                }
                catch (ThreadInterruptedException) { }
                catch (Exception exc)
                {
                    log.ErrorException("Ошибка в работе менеджера каналов сбора.", exc);
                    if (!donotlog)
                        ChannelKeepException = exc;
                    skiponce = true;
                }
            }
        }
        #endregion

        private IModuleLoader moduleLoaderPrototype;

        public IModuleLoader ModuleLoaderPrototype
        {
            get
            {
                if (moduleLoaderPrototype == null)
                    moduleLoaderPrototype = new FromFileModuleLoader(NSI.LoadersPath);
                return moduleLoaderPrototype;
            }
            set { moduleLoaderPrototype = value; }
        }

        #region Запуск/Остановка каналов
        public void StartChannels()
        {
            var channels = (from c in channelsDictionary.Keys where c.Active select c).ToArray();

            foreach (ChannelInfo channelInfo in channels)//channelsDictionary.Keys)
                try
                {
                    if (channelInfo.Active)
                        StartChannel(channelInfo);
                }
                catch { }
        }
        public void StartChannel(ChannelInfo channelInfo, bool force = false)
        {
            try
            {
                var innerChannelInfo = (from i in channelsDictionary.Keys where i.Equals(channelInfo) select i).FirstOrDefault();

                if (innerChannelInfo == null)
                {
                    innerChannelInfo = channelInfo;
                }

                if (innerChannelInfo.Parameters.Count() == 0)
                {
                    log.Info("У канала {0} нет ни одного параметра.", innerChannelInfo);
                }
                else
                {
                    // set last time for all parameters
                    foreach (var parameter in innerChannelInfo.Parameters)
                    {
                        var valueItem = NSI.valReceiver.GetLastValue(parameter.Idnum);

                        if (valueItem != null)
                        {
                            parameter.LastTime = valueItem.Time;
                        }
                    }

                    ChannelItem channel = GetChannelItem(innerChannelInfo);

                    channelsDictionary[innerChannelInfo] = channel;

                    if (innerChannelInfo.Active || force)
                    {
                        channel.Start();
                        channelInfo.Suspended = false;
                        log.Info("Запущен канал {0}", innerChannelInfo);
                    }
                }
            }
            catch (Exception exc)
            {
                log.ErrorException(String.Format("Ошибка запуска канала {0}.", channelInfo), exc);
                throw;
            }
        }

        /// <summary>
        /// Создаёт ChannelItem в отдельном домене. 
        /// И выставляет все необходимые зависимости.
        /// </summary>
        /// <param name="channelInfo">Информация о создаваемом канале</param>
        /// <returns></returns>
        private ChannelItem GetChannelItem(ChannelInfo channelInfo)
        {
            ChannelItem channel;

            AppDomain domain = CreateDomain(String.Format("({0}) {1}", channelInfo.Id, channelInfo.Name));
            Type channelItemType = typeof(ChannelItem);

            channel = domain.CreateInstanceAndUnwrap(
                channelItemType.Assembly.ToString(),
                channelItemType.ToString(), false, BindingFlags.CreateInstance, null,
                new Object[] { channelInfo }, null, null) as ChannelItem;

            // передаём каналу загрузчик модулей
            // при передачи черз ремоутинг, он должен скопироваться
            channel.ModuleLoader = ModuleLoaderPrototype;

            // передаём ссылку на приёмник значений
            channel.DataListener = listener;
            return channel;
        }

        public void StopChannels()
        {
            foreach (var item in GetLoadedChannels())
            {
                try
                {
                    StopChannel(item);
                }
                catch { }
            }
        }
        public void StopChannel(ChannelInfo channelInfo)
        {
            try
            {
                ChannelItem channel;

                var innerChannelInfo = (from i in channelsDictionary.Keys where i.Equals(channelInfo) select i).FirstOrDefault();

                if (innerChannelInfo != null && channelsDictionary.TryGetValue(innerChannelInfo, out channel) && channel != null)
                {
                    innerChannelInfo.Suspended = true;
                    AppDomain domain = channel.GetDomain();
                    channel.Stop();
                    channelsDictionary[innerChannelInfo] = null;
                    AppDomain.Unload(domain);

                    log.Info("Остановлен канал {0}", innerChannelInfo);
                }
            }
            catch (Exception exc)
            {
                log.ErrorException(String.Format("Ошибка остановки канала {0}.", channelInfo), exc);
                //throw;
            }
        }
        #endregion

        /// <summary>
        /// Получить список загруженных каналов
        /// </summary>
        /// <returns>Массив номеров каналов</returns>
        public ChannelInfo[] GetLoadedChannels()
        {
            return channelsDictionary.Keys.ToArray();
        }

        /// <summary>
        /// Возвращает список параметров канала
        /// </summary>
        /// <param name="channelInfo">Номер канала</param>
        /// <returns>Массив номеров каналов</returns>
        public ParameterItem[] GetParameters(ChannelInfo channelInfo)
        {
            ParameterItem[] res = new ParameterItem[] { };

            try
            {
                bool channelCreated = false;
                ChannelItem channel;

                if (!channelsDictionary.TryGetValue(channelInfo, out channel)
                    || channel == null)
                {
                    channel = GetChannelItem(channelInfo);
                    channelCreated = true;
                }

                if (channel != null)
                {
                    res = channel.GetParamList();
                }

                if (channelCreated)
                {
                    AppDomain domain = channel.GetDomain();
                    channel.Dispose();
                    AppDomain.Unload(domain);
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("Ошибка запроса параметров", ex);
                throw;
            }

            return res;
        }

        public ChannelStatus GetChannelState(ChannelInfo channelInfo)
        {
            ChannelStatus status = ChannelStatus.Unloaded;
            ChannelItem channel;//= GetChannel(channelInfo);

            channelsDictionary.TryGetValue(channelInfo, out channel);

            if (channel != null) status |= ChannelStatus.Started;
            if (!channelInfo.Active) status |= ChannelStatus.Blocked;
            if (channelInfo.Storable) status |= ChannelStatus.Storable;

            if (channel != null)
            {
                //if (!channel.IsStopped)


                if (!String.IsNullOrEmpty(channel.LastErrorMessage)) status |= ChannelStatus.HasErrors;
            }
            return status;
        }

        public ModuleInfo[] GetAvailableModules()
        {
            AppDomain domain = null;
            try
            {
                // В отдельном домене создаём обёртку для загрузчика модулей
                domain = CreateDomain("GetModulesInfo");
                MarshaledModuleLoader infoLoader = domain.CreateInstanceAndUnwrap(
                    typeof(MarshaledModuleLoader).Assembly.ToString(),
                    typeof(MarshaledModuleLoader).ToString()) as MarshaledModuleLoader;

                // передаём загрузчик в созданный домен
                infoLoader.ModuleLoader = ModuleLoaderPrototype;

                ModuleInfo[] modules = infoLoader.LoadInfo();

                foreach (var module in modules)
                {
                    var props = new List<ItemProperty>(module.ChannelProperties);

                    foreach (var item in ChannelItem.CommonChannelProperties)
                    {
                        props.Add(item);
                    }
                    module.ChannelProperties = props.ToArray();
                }

                return modules;
            }
            catch (Exception exc)
            {
                log.ErrorException("Ошибка запроса доступных модулей сбора", exc);
            }
            finally
            {
                if (domain != null)
                    AppDomain.Unload(domain);
            }
            return null;
        }

        /// <summary>
        /// Создать новый домен.
        /// </summary>
        /// <remarks>
        /// Выставляет рабочую папку для нового домена по пути, где расположена выполняемая сборка.
        /// </remarks>
        /// <param name="domainName">Имя домена</param>
        /// <returns></returns>
        private static AppDomain CreateDomain(String domainName)
        {
            AppDomainSetup appDomainInfo = new AppDomainSetup();
            String codeBase = Assembly.GetExecutingAssembly().CodeBase;
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[^:]+://+");
            codeBase = regex.Replace(codeBase, String.Empty);
            appDomainInfo.ApplicationBase = System.IO.Path.GetDirectoryName(codeBase);

            AppDomain domain = AppDomain.CreateDomain(domainName, AppDomain.CurrentDomain.Evidence, appDomainInfo);
            return domain;
        }

        public void DeleteValues(ChannelInfo channelInfo, DateTime timeFrom)
        {
            // останавливаем канал(ы)
            if (channelInfo == null)
                StopChannels();
            else
                StopChannel(channelInfo);

            // составляем список параметров для удаления значений
            List<ParameterItem> parameters = new List<ParameterItem>();

            if (channelInfo != null)
            {
                parameters.AddRange(channelInfo.Parameters);
            }
            else
            {
                foreach (var info in channelsDictionary.Keys)
                    parameters.AddRange(info.Parameters);
            }

            // удаляем значения
            NSI.valReceiver.DeleteValues(parameters.ToArray(), timeFrom);

            // сбрасываем последнее время параметра
            foreach (var param in channelInfo.Parameters)
            {
                
                param.LastTime = DateTime.MinValue;
            }

            // составляем список перезапускаемых каналов
            ChannelInfo[] channelList;

            if (channelInfo != null)
                channelList = new ChannelInfo[] { channelInfo };
            else channelList = channelsDictionary.Keys.ToArray();

            foreach (ChannelInfo info in channelList)
            {
                ReloadChannel(info);
                StartChannel(info);
            }
        }
    }
}
