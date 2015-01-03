using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using COTES.ISTOK.Modules;
using NLog;

namespace COTES.ISTOK.Block
{
    class ChannelItem : MarshalByRefObject, IDataListener, IDisposable
    {
        Logger log = LogManager.GetCurrentClassLogger();

        public ChannelInfo Info { get; private set; }

        public IModuleLoader ModuleLoader { get; set; }

        public IDataListener DataListener { get; set; }

        /// <summary>
        /// Интервал ожидания останова канала (мс)
        /// </summary>
        public int JoinInterval { get; set; }

        public DateTime LastActivityTime { get; private set; }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        Timer channelTimer;

        volatile bool isProcessing;

        readonly TimeSpan StepFromLastTime = TimeSpan.FromMilliseconds(1);

        public ChannelItem(ChannelInfo info)
        {
            this.Info = info;
            GlobalDiagnosticsContext.Set("server-name", String.Format("{0}.{1}", BlockSettings.Instance.ServerName, info.Module.Name));
        }

        public String LastErrorMessage { get; set; }

        public DateTime LastErrorTime { get; set; }

        #region IDataListener Members

        public void NotifyValues(ChannelInfo channelInfo, ParameterItem parameter, IEnumerable<ParamValueItem> values)
        {
            LastActivityTime = DateTime.Now;
            var par = (from p in Info.Parameters
                       where p.Idnum == parameter.Idnum
                       select p).FirstOrDefault();

            if (par != null)
            {
                if (par.LastTime > DateTime.MinValue)
                {
                    var minTime = (from v in values select v.Time).Min();

                    if (minTime - par.LastTime > intervalLarge)
                    {
                        log.Info("Обнаружен пропуск в значениях {0} с {1} по {2}", parameter, par.LastTime, minTime);
                        values = new ParamValueItem[]
                        {
                            new ParamValueItem(par.LastTime, Quality.Bad, double.NaN)
                        }.Concat(values).ToArray();
                    }
                }

                var maxTime = (from v in values select v.Time).Max();

                if (par.LastTime < maxTime)
                {
                    par.LastTime = maxTime;
                }
            }
            if (DataListener != null)
            {
                DataListener.NotifyValues(Info, parameter, values);
            }
        }

        public void NotifyError(string errorMessage)
        {
            LastErrorMessage = errorMessage;
            LastErrorTime = DateTime.Now;
            if (DataListener != null)
            {
                DataListener.NotifyError(errorMessage);
            }
        }

        #endregion

        public AppDomain GetDomain()
        {
            return AppDomain.CurrentDomain;
        }

        IDataLoaderFactory dataLoaderFactory;
        IDataLoader dataLoader;

        private void InitDataLoader()
        {
            //try
            //{
            if (dataLoaderFactory == null)
            {
                dataLoaderFactory = ModuleLoader.LoadModule(Info.Module);
                if (dataLoaderFactory == null)
                    throw new ArgumentException(String.Format("Модуль не может быть загружен", Info.Module.Name));
            }

            if (dataLoader == null)
            {
                dataLoader = dataLoaderFactory.CreateLoader(Info);

                dataLoader.Init(Info);

                dataLoader.DataListener = this;
            }
            //}
            //catch (Exception exc)
            //{
            //    log.ErrorException(CommonProperty.ChannelMessagePrefix(Info) + "Ошибка инициализации канала сбора", exc);
            //    LastErrorMessage = exc.Message;
            //    LastErrorTime = DateTime.Now;
            //    //throw;
            //}
        }

        public void Start()
        {
            try
            {
                InitDataLoader();
                SetProperties();
                LastActivityTime = DateTime.Now;

                switch (dataLoader.LoadMethod)
                {
                    case DataLoadMethod.Current:
                        StartLoadThread(LoadMethod_Current);
                        break;
                    case DataLoadMethod.Subscribe:
                        dataLoader.RegisterSubscribe();
                        break;
                    case DataLoadMethod.Archive:
                        timeState = new CalcTimeState();
                        StartLoadThread(LoadMethod_Archive);
                        break;
                    case DataLoadMethod.ArchiveByParameter:
                        parametersTimeState = new Dictionary<int, CalcTimeState>();

                        foreach (var item in Info.Parameters)
                        {
                            parametersTimeState[item.Idnum] = new CalcTimeState();
                        }
                        StartLoadThread(LoadMethod_ArchiveByParameter);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exc)
            {
                log.ErrorException(CommonProperty.ChannelMessagePrefix(Info) + "Ошибка старта.", exc);
                LastErrorMessage = exc.Message;
                LastErrorTime = DateTime.Now;
                throw new Exception(exc.Message);
            }
        }

        public void Stop()
        {
            try
            {
                if (dataLoader == null) return;
                switch (dataLoader.LoadMethod)
                {
                    case DataLoadMethod.Current:
                    case DataLoadMethod.Archive:
                    case DataLoadMethod.ArchiveByParameter:
                        StopLoadThread();
                        break;
                    case DataLoadMethod.Subscribe:
                        dataLoader.UnregisterSubscribe();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exc)
            {
                log.ErrorException(CommonProperty.ChannelMessagePrefix(Info) + "Ошибка остановки.", exc);
                LastErrorMessage = exc.Message;
                LastErrorTime = DateTime.Now;
                throw new Exception(exc.Message);
            }
        }

        public ParameterItem[] GetParamList()
        {
            try
            {
                InitDataLoader();

                return dataLoader.GetParameters();
            }
            catch (NotSupportedException)
            {
                return new ParameterItem[] { };
            }
            catch (Exception exc)
            {
                log.ErrorException(CommonProperty.ChannelMessagePrefix(Info) + "Ошибка запроса параметров.", exc);
                LastErrorMessage = exc.Message;
                LastErrorTime = DateTime.Now;
                throw new Exception(exc.Message);
            }
        }

        private void StartLoadThread(TimerCallback threadStart)
        {
            channelTimer = new Timer(threadStart, null, 0, sleep);
        }

        private void StopLoadThread()
        {
            channelTimer.Change(Timeout.Infinite, Timeout.Infinite);
            channelTimer.Dispose();
            channelTimer = null;
        }

        #region Настройки опроса
        TimeSpan intervalNormal;
        TimeSpan intervalLarge;
        TimeSpan timeLag;
        int sleep;

        private void SetProperties()
        {
            sleep = GetPause();
            //если нет параметров и пауза маленькая, то проц грузится на 100%,
            //поэтому тут грязный способ притормаживания проца - 10с
            if (!Info.Parameters.Any() && sleep < 10000) sleep = 10000;
            intervalNormal = TimeSpan.FromSeconds(double.Parse(GetPropertyValue(Info, CommonProperty.CaptureIntervalNormalProperty)));
            intervalLarge = TimeSpan.FromSeconds(double.Parse(GetPropertyValue(Info, CommonProperty.CaptureIntervalLargeProperty)));
            timeLag = TimeSpan.FromSeconds(double.Parse(GetPropertyValue(Info, CommonProperty.TimeLagProperty)));
        }
        #endregion

        #region Методы опроса, выполняемые в потоке
        private void LoadMethod_Current(Object state)
        {
            lock (this)
            {
                if (isProcessing)
                {
                    return;
                }
                isProcessing = true;
            }
            try
            {
                dataLoader.GetCurrent();
            }
            catch (Exception exc)
            {
                log.ErrorException(CommonProperty.ChannelMessagePrefix(Info) + "Ошибка в ходе опроса данных", exc);
                LastErrorMessage = exc.Message;
                LastErrorTime = DateTime.Now;
            }
            finally
            {
                lock (this)
                {
                    isProcessing = false;
                }
            }
        }

        CalcTimeState timeState;
        Dictionary<int, CalcTimeState> parametersTimeState = new Dictionary<int, CalcTimeState>();

        private void LoadMethod_Archive(Object state)
        {
            lock (this)
            {
                if (isProcessing)
                {
                    return;
                }
                isProcessing = true;
            }
            try
            {
                bool timeSets = false;
                DateTime startTime = DateTime.MinValue;
                DateTime endTime = DateTime.MinValue;

                // текущие время (время запрашивать значения за которое не имеет смысла)
                // с учётом настроенного времени запаздывания
                DateTime nowTime = DateTime.Now;
                nowTime -= timeLag;

                // получаем время для запроса в цикле
                // в цикле
                while (!timeSets)
                {
                    var lastTime = DateTime.MinValue;
                    // время начала запроса расчитывается как минимальное время
                    var time = from p in Info.Parameters select p.LastTime;
                    if (time.Any()) lastTime = time.Min();
                    // выставляем старттайм канала на сейчас
                    if (lastTime == DateTime.MinValue && Info.StartTime == DateTime.MinValue)
                    {
                        Info.StartTime = nowTime - intervalLarge;
                    }
                    lastTime += StepFromLastTime;
                    if (lastTime < Info.StartTime)
                    {
                        lastTime = Info.StartTime;
                    }

                    // расчёт запрашиваемого времени
                    timeSets = CalcTime(intervalNormal,
                                        intervalLarge,
                                        timeState,
                                        lastTime,
                                        nowTime,
                                        out startTime,
                                        out endTime);
                    if (!timeSets)
                    {
                        SendBadValue(lastTime, Info.Parameters);
                    }
                }

                StartTime = startTime;
                EndTime = endTime;

                // запросить данные, если возможно
                if (endTime < nowTime)
                {
                    dataLoader.GetArchive(startTime, endTime);
                }
            }
            catch (Exception exc)
            {
                log.ErrorException(CommonProperty.ChannelMessagePrefix(Info) + "Ошибка в ходе опроса данных", exc);
                LastErrorMessage = exc.Message;
                LastErrorTime = DateTime.Now;
            }
            finally
            {
                lock (this)
                {
                    isProcessing = false;
                }
            }
        }

        private void LoadMethod_ArchiveByParameter(Object state)
        {
            lock (this)
            {
                if (isProcessing)
                {
                    return;
                }
                isProcessing = true;
            }
            try
            {
                SetProperties();

                DateTime nowTime = DateTime.Now;

                // запаздывание времени
                nowTime -= timeLag;

                DateTime minStartTime = DateTime.MaxValue;
                DateTime maxEndTime = DateTime.MinValue;

                foreach (var parameter in Info.Parameters)
                {
                    DateTime lastTime;
                    DateTime startTime = DateTime.MinValue;
                    DateTime endTime = DateTime.MinValue;
                    bool timeSets = false;

                    while (!timeSets)
                    {
                        lastTime = parameter.LastTime;
                        lastTime += StepFromLastTime;
                        if (lastTime < Info.StartTime)
                        {
                            lastTime = Info.StartTime;
                        }

                        // расчёт запрашиваемого времени
                        timeSets = CalcTime(
                              intervalNormal,
                              intervalLarge,
                              parametersTimeState[parameter.Idnum],
                              lastTime,
                              nowTime,
                              out startTime,
                              out endTime);
                        if (!timeSets)
                        {
                            SendBadValue(lastTime, Info.Parameters);
                        }
                    }

                    // выставляем время для запроса данных параметра
                    if (endTime < nowTime)
                    {
                        dataLoader.SetArchiveParameterTime(parameter, startTime, endTime);
                    }

                    if (startTime < minStartTime)
                    {
                        minStartTime = startTime;
                    }
                    if (endTime > maxEndTime)
                    {
                        maxEndTime = endTime;
                    }
                }

                StartTime = minStartTime;
                EndTime = maxEndTime;

                // запрос данных архива
                dataLoader.GetArchive();

            }
            catch (Exception exc)
            {
                log.ErrorException(CommonProperty.ChannelMessagePrefix(Info) + "Ошибка в ходе опроса данных", exc);
                LastErrorMessage = exc.Message;
                LastErrorTime = DateTime.Now;
            }
            finally
            {
                lock (this)
                {
                    isProcessing = false;
                }
            }
        }
        #endregion

        private string GetPropertyValue(IPropertyable<String> propertyContainer, ItemProperty property)
        {
            return (propertyContainer.Contains(property) ? propertyContainer[property] : property.DefaultValue.ToString());
        }

        class CalcTimeState
        {
            /// <summary>
            /// врмя последнего запроса по intervalLarge, 
            /// если запрос за этого время повторится, то необходимо
            /// сместиьть запрашиваемый интервал вперёд на intervalNormal
            /// </summary>
            public DateTime lastLargeQueryTime { get; set; }

            /// <summary>
            /// было ли добавленно значение с плохим качеством
            /// при смещении запрашиваемого интервала
            /// </summary>
            public bool BadAdded { get; set; }
        }

        private bool CalcTime(TimeSpan intervalNormal,
                              TimeSpan intervalLarge,
                              CalcTimeState calcTimeState,
                              DateTime lastTime,
                              DateTime nowTime,
                              out DateTime startTime,
                              out DateTime endTime)
        {
            TimeSpan interval;

            // если за данный интервал уже запрашивали, 
            // всавить бэд и сместится на normal
            if (lastTime == calcTimeState.lastLargeQueryTime)
            {
                startTime = DateTime.MinValue;
                endTime = DateTime.MinValue;
                return false;
            }
            else
            {
                calcTimeState.BadAdded = false;
            }

            // выбор интервала запроса
            if (nowTime - lastTime > intervalLarge)
            {
                interval = intervalLarge;
                calcTimeState.lastLargeQueryTime = lastTime;
            }
            else
            {
                interval = intervalNormal;
                calcTimeState.lastLargeQueryTime = DateTime.MinValue;
            }

            startTime = lastTime;
            endTime = lastTime + interval;
            return true;
        }

        private void SendBadValue(DateTime valueTime, IEnumerable<ParameterItem> parameters)
        {
            foreach (var parameter in parameters)
            {
                SendBadValue(valueTime, parameter);
            }
        }

        private void SendBadValue(DateTime valueTime, ParameterItem parameter)
        {
            if (valueTime > parameter.LastTime)
            {
                ParamValueItem value = new ParamValueItem(valueTime, Quality.Bad, 0);
                DateTime newLastTime;
                if (parameter.LastTime < Info.StartTime)
                {
                    newLastTime = Info.StartTime + intervalNormal;
                }
                else
                {
                    newLastTime = parameter.LastTime + intervalNormal;
                }
                NotifyValues(Info, parameter, new ParamValueItem[] { value });
                // если не можем получить данные параметра, то делаем шаг равный intervalNormal
                parameter.LastTime = newLastTime;
            }
        }

        private int GetPause()
        {
            const int milesecondsInSecond = 1000;
            double pause = double.Parse(GetPropertyValue(Info, CommonProperty.PauseProperty), System.Globalization.NumberFormatInfo.InvariantInfo);

            if (pause <= 0)
            {
                log.Error("{0}Настроеная пауза слишком мала ({1}). Модуль остановлен.", CommonProperty.ChannelMessagePrefix(Info), pause);
            }
            int sleep = (int)(pause * milesecondsInSecond);
            return sleep;
        }

        ///// <summary>
        ///// Получение данных о состоянии канала
        ///// </summary>
        ///// <returns></returns>
        //public DataTable GetState()
        //{
        //    const String chanalNameColumnName = "ChannelName";
        //    const String channelNameColumnCaption = "Канал";
        //    DataTable table;

        //    if (dataLoader != null)
        //        table = dataLoader.ModuleState();
        //    else
        //        table = new DataTable();

        //    table.TableName = "Состояние каналов";
        //    if (!table.Columns.Contains(chanalNameColumnName))
        //    {
        //        DataColumn column = table.Columns.Add(chanalNameColumnName);
        //        column.Caption = channelNameColumnCaption;
        //        foreach (DataRow row in table.Rows) row[chanalNameColumnName] = idnum;
        //    }
        //    if (!table.Columns.Contains("Название"))
        //    {
        //        table.Columns.Add("Название");
        //        foreach (DataRow row in table.Rows) row["Название"] = Name;
        //    }

        //    return table;
        //}
        #region Общие свойства каналов
        //public const String commonItemPropertyCategory = "Общие";
        //public static readonly ItemProperty CaptureIntervalNormalProperty = new ItemProperty()
        //{
        //    Name = "CaptureIntervalNormal",
        //    DisplayName = "Период нормального запроса",
        //    Description = "Период нормального запроса (в секундах).",
        //    Category = commonItemPropertyCategory,
        //    ValueType = typeof(int),
        //    DefaultValue = 60.ToString()
        //};

        //public static readonly ItemProperty CaptureIntervalLargeProperty = new ItemProperty()
        //{
        //    Name = "CaptureIntervalLarge",
        //    DisplayName = "Период увеличенного запроса",
        //    Description = "Период увеличенного запроса (в секундах)",
        //    Category = commonItemPropertyCategory,
        //    ValueType = typeof(int),
        //    DefaultValue = 600.ToString()
        //};

        //public static readonly ItemProperty BufferLimitProperty = new ItemProperty()
        //{
        //    Name = "BufferLimit",
        //    DisplayName = "Размер буфера",
        //    Description = "Максимальное количество значений параметров, по превышению которого прекращается сбор.",
        //    Category = commonItemPropertyCategory,
        //    ValueType = typeof(int),
        //    DefaultValue = 500.ToString()
        //};

        //public static readonly ItemProperty PauseProperty = new ItemProperty()
        //{
        //    Name = "Pause",
        //    DisplayName = "Задержка между запросами",
        //    Description = "Задержка в секундах между запросами.",
        //    Category = commonItemPropertyCategory,
        //    ValueType = typeof(float),
        //    DefaultValue = 1.ToString()
        //};

        //public static readonly ItemProperty TimeLagProperty = new ItemProperty()
        //{
        //    Name = "TimeLag",
        //    DisplayName = "Задержка от текущего времени",
        //    Description = "Задержка от текущего времени (сек)",
        //    Category = commonItemPropertyCategory,
        //    ValueType = typeof(int),
        //    DefaultValue = 0.ToString()
        //};

        public static IEnumerable<ItemProperty> CommonChannelProperties
        {
            get
            {
                return new ItemProperty[] { 
                    CommonProperty.CaptureIntervalNormalProperty ,
                    CommonProperty.CaptureIntervalLargeProperty ,
                    CommonProperty.BufferLimitProperty,        
                    CommonProperty.PauseProperty,
                    CommonProperty.TimeLagProperty,
                };
            }
        }
        #endregion

        #region Общие свойства параметров
        public static IEnumerable<ItemProperty> CommonParameterProperties
        {
            get
            {
                return new ItemProperty[] { };
            }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (channelTimer != null)
                StopLoadThread();
        }

        #endregion

        public DataLoadMethod LoadMethod
        {
            get
            {
                if (dataLoader == null)
                    return DataLoadMethod.Current;
                return dataLoader.LoadMethod;
            }
        }

        public DateTime StartTime { get; private set; }

        public DateTime EndTime { get; private set; }
    }
}