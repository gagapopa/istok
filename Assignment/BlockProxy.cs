using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using COTES.ISTOK.ASC;
using COTES.ISTOK.DiagnosticsInfo;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Класс, берущий на себя связь с серверами сбора данных
    /// </summary>
    class BlockProxy
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// Запрашивать значения с сервера сбора данных в жатом виде
        /// </summary>
        public bool PackValues { get; set; }
        private UltimateZipper packer;
        internal ValueReceiver ValueReceiver { get; set; }
        public IUnitManager Manager { get; set; }

        /// <summary>
        /// Максимальное количество подключений блочных
        /// </summary>
        protected int MaxBlockCount { get; private set; }

#if EMA
        public const int DefaultAttemptsNumber = 10; 
#else
        public const int DefaultAttemptsNumber = 3;
#endif

        /// <summary>
        /// Количество попыток подключений к серверу сбора данных при обрывах связи
        /// </summary>
        public int AttemptsNumber { get; set; }

        /// <summary>
        /// Период ожидания при опросе состояния выполняемой операции на сервере сбора данных
        /// </summary>
        public TimeSpan StateSleep { get; set; }

        /// <summary>
        ///  Интервал времени между попытками подключения к серверу сбора данных
        /// </summary>
        public TimeSpan AttemptSleep { get; set; }

        public BlockProxy(int maxBlockCount)
        {
            MaxBlockCount = maxBlockCount;
            packer = new UltimateZipper();
            AttemptsNumber = DefaultAttemptsNumber;
            PackValues = true;
            //StateSleep = TimeSpan.FromSeconds(1);
#if EMA
            AttemptSleep = TimeSpan.FromSeconds(20); 
#else
            AttemptSleep = TimeSpan.FromSeconds(1);
#endif
        }

        #region Диагностика
        public Diagnostics GetDiagnosticsObject(string uid_block)
        {
            IBlockQueryManager block = GetQueryManager(uid_block);

            return block.GetDiagnosticsObject();

        }

        private IBlockQueryManager GetQueryManager(string uid_block)
        {
            ChannelFactory<IBlockQueryManager> factory;
            String url;

            //if (!dicBlockUIDs.TryGetValue(uid_block, out factory))
            if (!dicBlockUIDs.TryGetValue(uid_block, out url))
            {
                //throw new ArgumentException("uid_block");
                throw new ServerNotAccessibleException(String.Format("Сервер {0} не доступен", uid_block));
            }

            EndpointAddress address = new EndpointAddress(url);
            factory = new ChannelFactory<IBlockQueryManager>("NetTcpBinding_BlockQueryManager", address);
            factory.Open();
            return factory.CreateChannel();


            //if (factory.State != CommunicationState.Opened)
            //{
            //    factory.Open();
            //}

            //return factory.CreateChannel();
        }
        public Diagnostics GetDiagnosticsObject(int idnum)
        {
            foreach (var item in Blocks)
                if (item.Idnum == idnum) return GetDiagnosticsObject(item.BlockUID);
            return null;
        }
        #endregion

        public void SendDataToBlock(OperationState state,
                                    string uid_block,
                                    byte[] data)
        {
            SendDataToBlock(state, uid_block, 0, data);
        }
        public void SendDataToBlock(OperationState state,
                                    string uid_block,
                                    int channelId,
                                    byte[] data)
        {
            if (!dicBlockUIDs.ContainsKey(uid_block))
                throw new ISTOKException(string.Format("Сервер с идентификатором '{0}' не найден.", uid_block));

            IBlockQueryManager block = GetQueryManager(uid_block);
            if (block == null) return;
            //throw new Exception("Конечный сервер отсутсвует или недоступен.");
            if (data == null) return;
            //throw new Exception("Данные для отправки не корректны.");

            ulong operationID;
            if (channelId > 0)
                operationID = block.SetChannelSprav(channelId, data);
            else
                operationID = block.SetAllSprav(data);
            
            OperationInfo info;
            do
            {
                info = block.GetAsyncOperationState(operationID);
                state.Progress = info.Progress;
                state.StateString = info.StateString;
                System.Threading.Thread.Sleep(100);
            }
            while (!info.IsCompleted);
            block.EndAsyncOperation(operationID);
        }

        /// <summary>
        /// Получить последнее значение параметра
        /// </summary>
        /// <param name="param_id">Номер параметра</param>
        /// <returns>Параметр</returns>
        public ParamValueItem GetLastValue(LoadParameterNode parameter)
        {
            BlockNode bnode = Manager.GetParentNode(new OperationState(), parameter, (int)UnitTypeId.Block) as BlockNode;
            if (bnode != null)
            {
                IBlockQueryManager block = GetQueryManager(bnode.BlockUID);
                return block.GetLastValue(parameter.Idnum);
            }
            throw ThrowServerNotAccessibleException(bnode);
        }

        #region AsyncGetValues()
        ///// <summary>
        ///// Запросить значения параметра за период для запроса асинхронно
        ///// </summary>
        ///// <param name="state">Текущие состояние выполнения асинхронной операции</param>
        ///// <param name="progressToLoad">Указывает на сколько текущий метод должен сдвинуть прогресс операции</param>
        ///// <param name="parameter">Параметр</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        //public void AsyncGetValues(OperationState state, double progressToLoad, LoadParameterNode parameter,
        //    DateTime beginTime, DateTime endTime)
        //{
        //    state.AllowStartAsyncResult = true;
        //    ValuesRequestStruct[] request = assignParameters(state, new LoadParameterNode[] { parameter }, ValuesRequestMode.Raw,
        //        beginTime, endTime, Interval.Zero, Interval.Zero, CalcAggregation.Nothing);
        //    asyncGetValues(state, progressToLoad, request);

        //}

        ///// <summary>
        ///// Запросить значения параметра за период асинхронно
        ///// </summary>
        ///// <param name="state">Текущие состояние выполнения асинхронной операции</param>
        ///// <param name="progressToLoad">Указывает на сколько текущий метод должен сдвинуть прогресс операции</param>
        ///// <param name="parameter">Параметр</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        //public void AsyncGetValues(OperationState state, double progressToLoad, LoadParameterNode parameter,
        //    DateTime beginTime, DateTime endTime,
        //    Interval interval, CalcAggregation aggregation)
        //{
        //    state.AllowStartAsyncResult = true;
        //    ValuesRequestStruct[] request = assignParameters(state, new LoadParameterNode[] { parameter }, ValuesRequestMode.Aggregated,
        //        beginTime, endTime, Interval.Zero, interval, aggregation);
        //    asyncGetValues(state, progressToLoad, request);
        //}

        ///// <summary>
        ///// Запросить значения нескольких параметров за период асинхронно
        ///// </summary>
        ///// <param name="state">Текущие состояние выполнения асинхронной операции</param>
        ///// <param name="progressToLoad">Указывает на сколько текущий метод должен сдвинуть прогресс операции</param>
        ///// <param name="parameters">Массив параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        //public void AsyncGetValues(OperationState state, double progressToLoad, LoadParameterNode[] parameters,
        //    DateTime beginTime, DateTime endTime)
        //{
        //    state.AllowStartAsyncResult = true;
        //    ValuesRequestStruct[] request = assignParameters(state, parameters, ValuesRequestMode.Raw,
        //        beginTime, endTime, Interval.Zero, Interval.Zero, CalcAggregation.Nothing);
        //    asyncGetValues(state, progressToLoad, request);
        //}

        /// <summary>
        /// Запросить значения нескольких параметров за период асинхронно
        /// </summary>
        /// <param name="state">Текущие состояние выполнения асинхронной операции</param>
        /// <param name="progressToLoad">Указывает на сколько текущий метод должен сдвинуть прогресс операции</param>
        /// <param name="parameters">Массив параметров</param>
        /// <param name="beginTime">Начальное время запрашиваемого периода</param>
        /// <param name="endTime">Конечное время запрашиваемого периода</param>
        /// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        /// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        public void AsyncGetValues(OperationState state, double progressToLoad, ParameterValuesRequest[] parameters)
            //DateTime beginTime, DateTime endTime, Interval interval, CalcAggregation aggregation)
        {
            state.AllowStartAsyncResult = true;
            ValueRequestState[] request = assignParameters(state, parameters);
            asyncGetValues(state, progressToLoad, request);
        }
        #endregion

        #region GetValues()
        ///// <summary>
        ///// Запросить значения параметра за период
        ///// </summary>
        ///// <param name="parameter">Параметр</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="messages">Массив сообщений, полученных в процессе запроса данных</param>
        ///// <returns>Пакеты данных с полученными данными</returns>
        //public Package[] GetValues(LoadParameterNode parameter, DateTime beginTime, DateTime endTime, out Message[] messages)
        //{
        //    OperationState state = new OperationState();
        //    Package[] retPackage;

        //    AsyncGetValues(state, 100f, parameter, beginTime, endTime);
        //    messages = state.messages.ToArray();
        //    retPackage = new Package[state.AsyncResult.Count];
        //    for (int i = 0; i < retPackage.Length; i++)
        //    {
        //        retPackage[i] = state.AsyncResult[i] as Package;
        //    }
        //    return retPackage;
        //}

        ///// <summary>
        ///// Запросить значения параметра за период
        ///// </summary>
        ///// <param name="parameter">Параметр</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <param name="messages">Массив сообщений, полученных в процессе запроса данных</param>
        ///// <returns>Пакеты данных с полученными данными</returns>
        //public Package[] GetValues(LoadParameterNode parameter, DateTime beginTime, DateTime endTime,
        //    Interval interval, CalcAggregation aggregation, out Message[] messages)
        //{
        //    OperationState state = new OperationState();
        //    Package[] retPackage;

        //    AsyncGetValues(state, 100f, parameter, beginTime, endTime, interval, aggregation);
        //    messages = state.messages.ToArray();
        //    retPackage = new Package[state.AsyncResult.Count];
        //    for (int i = 0; i < retPackage.Length; i++)
        //    {
        //        retPackage[i] = state.AsyncResult[i] as Package;
        //    }
        //    return retPackage;
        //}

        ///// <summary>
        ///// Запросить значения нескольких параметров за период
        ///// </summary>
        ///// <param name="parameters">Массив параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="messages">Массив сообщений, полученных в процессе запроса данных</param>
        ///// <returns>Пакеты данных с полученными данными</returns>
        //public Package[] GetValues(LoadParameterNode[] parameters, DateTime beginTime, DateTime endTime, out Message[] messages)
        //{
        //    OperationState state = new OperationState();
        //    Package[] retPackage;

        //    AsyncGetValues(state, 100f, parameters, beginTime, endTime);
        //    messages = state.messages.ToArray();
        //    retPackage = new Package[state.AsyncResult.Count];
        //    for (int i = 0; i < retPackage.Length; i++)
        //    {
        //        retPackage[i] = state.AsyncResult[i] as Package;
        //    }
        //    return retPackage;
        //}


        ///// <summary>
        ///// Запросить значения нескольких параметров за период
        ///// </summary>
        ///// <param name="parameters">Массив параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <param name="messages">Массив сообщений, полученных в процессе запроса данных</param>
        ///// <returns>Пакеты данных с полученными данными</returns>
        //public Package[] GetValues(LoadParameterNode[] parameters, DateTime beginTime, DateTime endTime,
        //    Interval interval, CalcAggregation aggregation, out Message[] messages)
        //{
        //    OperationState state = new OperationState();
        //    Package[] retPackage;

        //    AsyncGetValues(state, 100f, parameters, beginTime, endTime, interval, aggregation);
        //    messages = state.messages.ToArray();
        //    retPackage = new Package[state.AsyncResult.Count];
        //    for (int i = 0; i < retPackage.Length; i++)
        //    {
        //        retPackage[i] = state.AsyncResult[i] as Package;
        //    }
        //    return retPackage;
        //}
        #endregion

        #region AsyncGetAppertureValues()
        ///// <summary>
        ///// Запросить значения параметра за период с учетом апертуры асинхронно
        ///// </summary>
        ///// <param name="state">Текущие состояние выполнения асинхронной операции</param>
        ///// <param name="progressToLoad">Указывает на сколько текущий метод должен сдвинуть прогресс операции</param>
        ///// <param name="parameter">Параметр</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        //public void AsyncGetAppertureValues(OperationState state, double progressToLoad, LoadParameterNode parameter,
        //    DateTime beginTime, DateTime endTime, Interval parameterInterval)
        //{
        //    state.AllowStartAsyncResult = true;
        //    ValuesRequestStruct[] request = assignParameters(state, new LoadParameterNode[] { parameter }, ValuesRequestMode.Appertured,
        //        beginTime, endTime, parameterInterval, Interval.Zero, CalcAggregation.Nothing);
        //    asyncGetValues(state, progressToLoad, request);
        //}

        ///// <summary>
        ///// Запросить значения параметра за период с учетом апертуры асинхронно
        ///// </summary>
        ///// <param name="state">Текущие состояние выполнения асинхронной операции</param>
        ///// <param name="progressToLoad">Указывает на сколько текущий метод должен сдвинуть прогресс операции</param>
        ///// <param name="parameter">Параметр</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <param name="aggregInterval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        //public void AsyncGetAppertureValues(OperationState state, double progressToLoad, LoadParameterNode parameter,
        //    DateTime beginTime, DateTime endTime,
        //    Interval parameterInterval, Interval aggregInterval, CalcAggregation aggregation)
        //{
        //    state.AllowStartAsyncResult = true;
        //    ValuesRequestStruct[] request = assignParameters(state, new LoadParameterNode[] { parameter }, ValuesRequestMode.Appertured | ValuesRequestMode.Aggregated,
        //        beginTime, endTime, parameterInterval, aggregInterval, aggregation);
        //    asyncGetValues(state, progressToLoad, request);
        //}

        ///// <summary>
        ///// Запросить значения нескольких параметров за период с учетом апертуры асинхронно
        ///// </summary>
        ///// <param name="state">Текущие состояние выполнения асинхронной операции</param>
        ///// <param name="progressToLoad">Указывает на сколько текущий метод должен сдвинуть прогресс операции</param>
        ///// <param name="parameters">Массив параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        //public void AsyncGetAppertureValues(OperationState state, double progressToLoad, LoadParameterNode[] parameters,
        //    DateTime beginTime, DateTime endTime, Interval parameterInterval)
        //{
        //    state.AllowStartAsyncResult = true;
        //    ValuesRequestStruct[] request = assignParameters(state, parameters, ValuesRequestMode.Appertured,
        //        beginTime, endTime, parameterInterval, Interval.Zero, CalcAggregation.Nothing);
        //    asyncGetValues(state, progressToLoad, request);
        //}

        ///// <summary>
        ///// Запросить значения нескольких параметров за период с учетом апертуры асинхронно
        ///// </summary>
        ///// <param name="state">Текущие состояние выполнения асинхронной операции</param>
        ///// <param name="progressToLoad">Указывает на сколько текущий метод должен сдвинуть прогресс операции</param>
        ///// <param name="parameters">Массив параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <param name="aggregInterval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        //public void AsyncGetAppertureValues(OperationState state, double progressToLoad, LoadParameterNode[] parameters,
        //    DateTime beginTime, DateTime endTime,
        //    Interval parameterInterval, Interval aggregInterval, CalcAggregation aggregation)
        //{
        //    state.AllowStartAsyncResult = true;
        //    ValuesRequestStruct[] request = assignParameters(state, parameters, ValuesRequestMode.Appertured | ValuesRequestMode.Aggregated,
        //        beginTime, endTime, parameterInterval, aggregInterval, aggregation);
        //    asyncGetValues(state, progressToLoad, request);
        //}
        #endregion

        #region GetAppertureValues()
        ///// <summary>
        ///// Запросить значения параметра за период с учетом апертуры
        ///// </summary>
        ///// <param name="parameter">Параметр</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <returns>Пакеты данных с полученными данными</returns>
        //public Package[] GetAppertureValues(LoadParameterNode parameter, DateTime beginTime, DateTime endTime,
        //    Interval parameterInterval, out Message[] messages)
        //{
        //    OperationState state = new OperationState();
        //    Package[] retPackage;

        //    AsyncGetAppertureValues(state, 100f, parameter, beginTime, endTime, parameterInterval);
        //    messages = state.messages.ToArray();
        //    retPackage = new Package[state.AsyncResult.Count];
        //    for (int i = 0; i < retPackage.Length; i++)
        //    {
        //        retPackage[i] = state.AsyncResult[i] as Package;
        //    }
        //    return retPackage;
        //}

        ///// <summary>
        ///// Запросить значения параметра за период с учетом апертуры
        ///// </summary>
        ///// <param name="parameter">Параметр</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <param name="aggregInterval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns>Пакеты данных с полученными данными</returns>
        //public Package[] GetAppertureValues(LoadParameterNode parameter, DateTime beginTime, DateTime endTime,
        //    Interval parameterInterval, Interval aggregInterval, CalcAggregation aggregation, out Message[] messages)
        //{
        //    OperationState state = new OperationState();
        //    Package[] retPackage;

        //    AsyncGetAppertureValues(state, 100f, parameter, beginTime, endTime, parameterInterval, aggregInterval, aggregation);
        //    messages = state.messages.ToArray();
        //    retPackage = new Package[state.AsyncResult.Count];
        //    for (int i = 0; i < retPackage.Length; i++)
        //    {
        //        retPackage[i] = state.AsyncResult[i] as Package;
        //    }
        //    return retPackage;
        //}

        ///// <summary>
        ///// Запросить значения нескольких параметров за период с учетом апертуры
        ///// </summary>
        ///// <param name="parameters">Массив параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <returns>Пакеты данных с полученными данными</returns>
        //public Package[] GetAppertureValues(LoadParameterNode[] parameters, DateTime beginTime, DateTime endTime,
        //    Interval parameterInterval, out Message[] messages)
        //{
        //    OperationState state = new OperationState();
        //    Package[] retPackage;

        //    AsyncGetAppertureValues(state, 100f, parameters, beginTime, endTime, parameterInterval);
        //    messages = state.messages.ToArray();
        //    retPackage = new Package[state.AsyncResult.Count];
        //    for (int i = 0; i < retPackage.Length; i++)
        //    {
        //        retPackage[i] = state.AsyncResult[i] as Package;
        //    }
        //    return retPackage;
        //}

        ///// <summary>
        ///// Запросить значения нескольких параметров за период с учетом апертуры
        ///// </summary>
        ///// <param name="parameters">Массив параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <param name="aggregInterval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns>Пакеты данных с полученными данными</returns>
        //public Package[] GetAppertureValues(LoadParameterNode[] parameters, DateTime beginTime, DateTime endTime,
        //    Interval parameterInterval, Interval aggregInterval, CalcAggregation aggregation, out Message[] messages)
        //{
        //    OperationState state = new OperationState();
        //    Package[] retPackage;

        //    AsyncGetAppertureValues(state, 100f, parameters, beginTime, endTime, parameterInterval, aggregInterval, aggregation);
        //    messages = state.messages.ToArray();
        //    retPackage = new Package[state.AsyncResult.Count];
        //    for (int i = 0; i < retPackage.Length; i++)
        //    {
        //        retPackage[i] = state.AsyncResult[i] as Package;
        //    }
        //    return retPackage;
        //}
        #endregion

        #region Работа с обновляемыми параметрами (типа мнемосхемы)
        public ParamValueItemWithID[] GetValuesFromBank(BlockNode node, int taID)
        {
            if (node == null) throw new ArgumentNullException("node");

            IBlockQueryManager block = GetQueryManager(node.BlockUID);

            return block.GetValuesFromBank(taID);
        }

        public int RegisterClient(BlockNode node, ParamValueItemWithID[] target)
        {
            if (node == null) throw new ArgumentNullException("node");

            IBlockQueryManager block = GetQueryManager(node.BlockUID);
            return block.RegisterClient(target);
        }

        public void UnRegisterClient(BlockNode node, int taID)
        {
            if (node == null) throw new ArgumentNullException("node");

            IBlockQueryManager block = GetQueryManager(node.BlockUID);
            block.UnRegisterClient(taID);
        }
        #endregion

        #region Работа с расписаниями
        public void FillParameterSchedules(IEnumerable<Schedule> arrSchedules)
        {
            //if (arrSchedules == null) throw new ArgumentNullException("arrSchedules");
            //foreach (var item in arrSchedules)
            //{
            //    item.Action = new AsyncDelegate(ParameterScheduleCallback);
            //}
        }

        /// <summary>
        /// Должен содеражть true, если запрос параметров для указанного ИД уже идет.
        /// Предназначен для предотвращения одновременного запроса параметров по одному и тому же расписанию.
        /// </summary>
        Dictionary<int, bool> sheduleLockDictionary = new Dictionary<int, bool>();

        //private object ParameterScheduleCallback(OperationState state, Object[] arg)
        //{
        //    Schedule schedule;
        //    UnitNode[] nodes;

        //    if (arg.Length == 0 || (schedule = arg[0] as Schedule) == null)
        //        throw new ArgumentException("Schedule object is required.");

        //    state.StateString = "Отправка запросов серверам сбора данных.";
        //    DateTime to = DateTime.Now;

        //    lock (sheduleLockDictionary)
        //    {
        //        if (sheduleLockDictionary.ContainsKey(schedule.Id) && sheduleLockDictionary[schedule.Id])
        //            return null; 
        //    }

        //    try
        //    {
        //        lock (sheduleLockDictionary)
        //        {
        //            sheduleLockDictionary[schedule.Id] = true; 
        //        }
        //        foreach (var block in blocks)
        //        {
        //            nodes = Manager.GetChildNodes(state, block, (int)UnitTypeId.Parameter);
        //            var pars = (from elem in nodes
        //                        where elem.Attributes.ContainsKey(@"schedule") &&
        //                         Convert.ToInt32(elem.GetAttribute(@"schedule")) == schedule.Id
        //                        select elem as ParameterNode).ToArray();
        //            if (pars.Length == 0) continue;

        //            // этот метод должен сам собой выполняться в потоке, асинхронная операция здесь не нужна
        //            //GlobalQueryManager.globSvcManager.asyncOperation.BeginAsyncOperation(true, (OperationState state, Object[] args) =>
        //            //    {
        //            state.StateString = "Получение значений параметров с сервера сбора данных.";
        //            foreach (var param in pars)
        //            {
        //                DateTime from = ValueReceiver.GetLastTimeValueParameter(state, param.Idnum);
        //                LoadParameterNode par = (LoadParameterNode)param;
        //                if (from == DateTime.MinValue)
        //                {
        //                    if (par.StartTime == DateTime.MinValue)
        //                    {
        //                        var tmp = Manager.GetParentNode(state, par, (int)UnitTypeId.Channel) as ChannelNode;
        //                        if (tmp != null) from = tmp.StartTime;
        //                    }
        //                    else
        //                        from = par.StartTime;
        //                }
        //                if (from == DateTime.MinValue) from = par.AggregationInterval.GetPrevTime(to);
        //                from = par.AggregationInterval.NearestEarlierTime(from);
        //                if (par.HasAperture)
        //                    AsyncGetAppertureValues(state,
        //                                            100f / pars.Length,
        //                                            par,
        //                                            from,
        //                                            to,
        //                                            par.Interval,
        //                                            par.AggregationInterval,
        //                                            par.Aggregation);
        //                else
        //                    AsyncGetValues(state,
        //                                    100f / pars.Length,
        //                                    par,
        //                                    from,
        //                                    to,
        //                                    par.AggregationInterval,
        //                                    par.Aggregation);

        //            }
        //            state.StateString = "Значения параметров с сервера сбора данных получены.";
        //            return null;
        //            //});
        //        }
        //        state.Progress = 100f;
        //        state.StateString = "Запросы серверам сбора данных отправлены.";                
        //    }
        //    finally
        //    {
        //        Package[] packs = state.AsyncResult.Cast<Package>().ToArray<Package>();
        //        state.AsyncResult.Clear();
        //        ValueReceiver.SaveValues(state, packs);
                
        //        lock (sheduleLockDictionary)
        //        {
        //            sheduleLockDictionary[schedule.Id] = false; 
        //        }
        //    }

        //    return null;
        //}
        #endregion

        private Exception ThrowServerNotAccessibleException(BlockNode node)
        {
            return new ServerNotAccessibleException(String.Format("Нет связи с сервером '{0}'", node));
        }

        /// <summary>
        /// Разбить параметры по серверам сбора и подготовить структуру для запроса данных
        /// </summary>
        /// <param name="parameters">Запрашиваемые запросы</param>
        /// <param name="requestMode">Режим запроса данных</param>
        /// <param name="beginTime">Начальное время запрашиваемого периода</param>
        /// <param name="endTime">Конечное время запрашиваемого периода</param>
        /// <param name="parameterInterval">Интервал параметра для апертуры</param>
        /// <param name="aggregInterval">Интервал агрегации</param>
        /// <param name="aggregation">Тип агрегации</param>
        /// <returns>Структуры запроса данных, каждая для своего сервера сбора данных</returns>
        private ValueRequestState[] assignParameters(
            OperationState state,
            ParameterValuesRequest[] parameters)
            //ValuesRequestMode requestMode,
            //DateTime beginTime,
            //DateTime endTime,
            //Interval parameterInterval,
            //Interval aggregInterval,
            //CalcAggregation aggregation)
        {
            Dictionary<BlockNode, List<BlockParameterValuesRequest>> parameterAllocation = new Dictionary<BlockNode, List<BlockParameterValuesRequest>>();
            List<ValueRequestState> ret = new List<ValueRequestState>();
            List<BlockParameterValuesRequest> parameterIDs;

            // группируем запросы по блочным
            foreach (ParameterValuesRequest param in parameters)
            {
                // Проверить, что все параметры распологаются на одном источнике
                var nodes = (from p in param.Parameters
                             select Manager.GetParentNode(state, p.Item1, (int)UnitTypeId.Block) as BlockNode)
                             .Distinct();

                if (nodes.Count() > 1)
                {
                    log.Warn("Запрос многопараметровой агрегации с разных источников");
                    state.AddMessage(new Message(MessageCategory.Warning, "Запрос многопараметровой агрегации с разных источников"));
                }
                else
                {
                    var bnode = nodes.First();

                    if (!parameterAllocation.TryGetValue(bnode, out parameterIDs))
                    {
                        parameterAllocation.Add(bnode, parameterIDs = new List<BlockParameterValuesRequest>());
                    }
                    parameterIDs.Add(new BlockParameterValuesRequest()
                    {
                        Parameters = (from t in param.Parameters select new ParameterItem() { Idnum = t.Item1.Idnum }).ToArray(),
                        Aggregation = param.Aggregation,
                        AggregationInterval = param.AggregationInterval,
                        StartTime = param.StartTime,
                        EndTime = param.EndTime,
                    });
                }
            }

            foreach (BlockNode node in parameterAllocation.Keys)
            {
                //ValueRequestState request = new ValueRequestState(node, parameterAllocation[node].ToArray(), PackValues);
                //request.RequestMode = requestMode;
                //request.BeginTime = beginTime;
                //request.EndTime = endTime;
                //request.ParameterInterval = parameterInterval;
                //request.AggregInterval = aggregInterval;
                //request.Aggregation = aggregation;
                var request = new ValueRequestState()
                {
                    Block = node,
                    OperationState = state,
                    Requests = parameterAllocation[node].ToArray()
                };
                ret.Add(request);
            }

            return ret.ToArray();
        }

        /// <summary>
        /// Запрос значений параметров с серверов сбора данных
        /// </summary>
        /// <param name="state">Объект состояния текущей асинхронной операции</param>
        /// <param name="valuesRequestStruct">Структуры с внутренней информации для запроса данных с сервера сбора данных</param>
        /// <returns></returns>
        private Object asyncGetValues(OperationState state, double progressToLoad, ValueRequestState[] valuesRequestStruct)
        {
            System.Threading.Thread[] threads = new System.Threading.Thread[valuesRequestStruct.Length];
            bool initial = true, interrupt = false, abort = false;
            double curProgress = state.Progress;
            while (initial || interrupt || abort)
            {
                try
                {
                    if (initial)
                    {
                        initial = false;
                        // запуск отдельного потока для каждого сервера сбора
                        for (int i = 0; i < valuesRequestStruct.Length; i++)  
                        {
                            threads[i] = new System.Threading.Thread(asyncGetValuesFromObject);
                            valuesRequestStruct[i].OperationState = state;
                            threads[i].Start(valuesRequestStruct[i]);
                        }
                    }
                    // прервать выполнение каждого потока
                    else if (interrupt)     
                    {
                        for (int i = 0; i < threads.Length; i++)
                            threads[i].Interrupt();
                        interrupt = false;
                    }
                    // принудительно прервать выполнение каждого потока
                    else if (abort)   
                    {
                        for (int i = 0; i < threads.Length; i++)
                            threads[i].Abort();
                        break;
                    }
                    int jointimeout = 500;
                    bool breakFlag = false;
                    double progress;
                    while (!breakFlag)
                    {
                        breakFlag = true;
                        // ожидание завершения каждого потока
                        for (int i = 0; i < threads.Length; i++)  
                            breakFlag &= threads[i].Join(jointimeout);
                        progress = 0;
                        for (int i = 0; i < valuesRequestStruct.Length; i++)
                            progress += AsyncOperation.MaxProgressValue / valuesRequestStruct.Length;
                        state.Progress = curProgress + progressToLoad * progress / 100f; ;
                    }
                }
                catch (System.Threading.ThreadInterruptedException) { interrupt = true; }
                catch (System.Threading.ThreadAbortException) { abort = true; }
            }
            return null;
        }

        private const double selectProgresPart = 0.4;
        private const double retrieveProgressPart = 1f - selectProgresPart;

        /// <summary>
        /// Запрос значений от сервера сбора данных в отдельном потоке
        /// </summary>
        /// <param name="threadState">аргумент, передаваемый потоку, равный ValuesRequestStruct с информацией о нужном сервере сбора и запрашиваемых значениях</param>
        private void asyncGetValuesFromObject(Object threadState)
        {
            ValueRequestState request = threadState as ValueRequestState;
            OperationState state = request.OperationState;
            OperationInfo info;
            UAsyncResult packageObject;
            Package package;
            Message[] messages;
            bool getNext;
            bool retryArgumentException = false;

            log.Trace("asyncGetValuesFromObject({0})", request);

            try
            {
                request.AttemptsLeft = AttemptsNumber;

                while (request.AttemptsLeft > 0)
                {
                    try
                    {
                        if (retryArgumentException)
                        {
                            System.Threading.Thread.Sleep(AttemptSleep);
                        }
                        //IBlockQueryManager blockManager;
                        //if (!dicBlockUIDs.TryGetValue(request.Block.BlockUID, out blockManager))
                        ////throw ThrowServerNotAccessibleException(request.Block);
                        //{ retryArgumentException = true; --request.AttemptsLeft; continue; }
                        IBlockQueryManager blockManager = GetQueryManager(request.Block.BlockUID);
                        if (state.IsInterrupted)
                        {
                            try
                            {
                                //IBlockQueryManager blockManager;
                                //if (dicBlockUIDs.TryGetValue(request.Block.BlockUID, out blockManager))
                                blockManager.InteruptAsyncOperation(request.BlockOperationID);
                                //else break;// throw ThrowServerNotAccessibleException(request.Block);
                            }
                            catch (System.Net.Sockets.SocketException) { }
                            catch (System.Runtime.Remoting.RemotingException) { }
                            catch (FaultException) { throw; }
                            catch (CommunicationException) { DetachBlcok(request.Block.BlockUID); }
                            break;
                        }
                        // запуск асинхронного запроса к блочному
                        if (request.BlockOperationID == 0)
                        {
                            request.BlockOperationID = blockManager.BeginGetValues(request.Requests, request.UsePack);
                            //switch (request.RequestMode)    // выбор операции запроса на основании входных аргументов
                            //{
                            //    case ValuesRequestMode.Raw:
                            //        if (request.UsePack)
                            //        {
                            //            if (request.ParametersIDs.Length == 1)
                            //                request.BlockOperationID = blockManager.BeginGetPackedValues1(request.ParametersIDs[0], request.BeginTime, request.EndTime);
                            //            else request.BlockOperationID = blockManager.BeginGetPackedValues3(request.ParametersIDs, request.BeginTime, request.EndTime);
                            //        }
                            //        else
                            //        {
                            //            if (request.ParametersIDs.Length == 1)
                            //                request.BlockOperationID = blockManager.BeginGetValues1(request.ParametersIDs[0], request.BeginTime, request.EndTime);
                            //            else request.BlockOperationID = blockManager.BeginGetValues3(request.ParametersIDs, request.BeginTime, request.EndTime);
                            //        } break;
                            //    case ValuesRequestMode.Aggregated:
                            //        if (request.UsePack)
                            //        {
                            //            if (request.ParametersIDs.Length == 1)
                            //                request.BlockOperationID = blockManager.BeginGetPackedValues2(request.ParametersIDs[0], request.BeginTime, request.EndTime, request.AggregInterval, request.Aggregation);
                            //            else request.BlockOperationID = blockManager.BeginGetPackedValues4(request.ParametersIDs, request.BeginTime, request.EndTime, request.AggregInterval, request.Aggregation);
                            //        }
                            //        else
                            //        {
                            //            if (request.ParametersIDs.Length == 1)
                            //                request.BlockOperationID = blockManager.BeginGetValues2(request.ParametersIDs[0], request.BeginTime, request.EndTime, request.AggregInterval, request.Aggregation);
                            //            else request.BlockOperationID = blockManager.BeginGetValues4(request.ParametersIDs, request.BeginTime, request.EndTime, request.AggregInterval, request.Aggregation);
                            //        } break;
                            //    case ValuesRequestMode.Appertured:
                            //        if (request.UsePack)
                            //        {
                            //            if (request.ParametersIDs.Length == 1)
                            //                request.BlockOperationID = blockManager.BeginGetPackedAppertureValues1(request.ParametersIDs[0], request.BeginTime, request.EndTime, request.ParameterInterval);
                            //            else request.BlockOperationID = blockManager.BeginGetPackedAppertureValues3(request.ParametersIDs, request.BeginTime, request.EndTime, request.ParameterInterval);
                            //        }
                            //        else
                            //        {
                            //            if (request.ParametersIDs.Length == 1)
                            //                request.BlockOperationID = blockManager.BeginGetAppertureValues1(request.ParametersIDs[0], request.BeginTime, request.EndTime, request.ParameterInterval);
                            //            else request.BlockOperationID = blockManager.BeginGetAppertureValues3(request.ParametersIDs, request.BeginTime, request.EndTime, request.ParameterInterval);
                            //        } break;
                            //    case ValuesRequestMode.Appertured | ValuesRequestMode.Aggregated:
                            //        if (request.UsePack)
                            //        {
                            //            if (request.ParametersIDs.Length == 1)
                            //                request.BlockOperationID = blockManager.BeginGetPackedAppertureValues2(request.ParametersIDs[0], request.BeginTime, request.EndTime, request.ParameterInterval, request.AggregInterval, request.Aggregation);
                            //            else request.BlockOperationID = blockManager.BeginGetPackedAppertureValues4(request.ParametersIDs, request.BeginTime, request.EndTime, request.ParameterInterval, request.AggregInterval, request.Aggregation);
                            //        }
                            //        else
                            //        {
                            //            if (request.ParametersIDs.Length == 1)
                            //                request.BlockOperationID = blockManager.BeginGetAppertureValues2(request.ParametersIDs[0], request.BeginTime, request.EndTime, request.ParameterInterval, request.AggregInterval, request.Aggregation);
                            //            else request.BlockOperationID = blockManager.BeginGetAppertureValues4(request.ParametersIDs, request.BeginTime, request.EndTime, request.ParameterInterval, request.AggregInterval, request.Aggregation);
                            //        } break;
                            //    default:
                            //        break;
                            //}
                        }
                        if (request.BlockOperationID != 0 && (info = blockManager.GetAsyncOperationState(request.BlockOperationID)) != null)
                        {
                            //request.Progress = selectProgresPart * info.Progress;
                            // запрос текущих сообщений
                            getNext = request.MessagesGetNext;
                            request.MessagesGetNext = false;
                            while ((messages = blockManager.GetAsyncOperationMessages(request.BlockOperationID, getNext)) != null)
                            {
                                state.AddMessage(messages);
                            }
                            request.MessagesGetNext = true;
                            // запрос значений
                            if (info.IsCompleted)
                            {
                                getNext = request.ValuesGetNext;
                                request.ValuesGetNext = false;
                                while ((packageObject = blockManager.GetAsyncOperationData(request.BlockOperationID, getNext)) != null)
                                {
                                    //if (request.UsePack)
                                    //    package = packer.Unpack(packageObject as PackedPackage);
                                    //else package = packageObject as Package;
                                    //state.AddAsyncResult(package);
                                    //++request.RetriviedPackages;
                                    if (packageObject.PackedPackage != null)
                                    {
                                        foreach (var item in packageObject.PackedPackage)
                                        {
                                            package = packer.Unpack(item);
                                            state.AddAsyncResult(package);
                                            ++request.RetriviedPackages;
                                        }
                                    }
                                    else if (packageObject.Packages != null)
                                    {
                                        foreach (var item in packageObject.Packages)
                                        {
                                            state.AddAsyncResult(item);
                                            ++request.RetriviedPackages;
                                        }
                                    }
                                    // двиганье прогрессбара
                                    //request.Progress = selectProgresPart * info.Progress +
                                    //    retrieveProgressPart * request.RetriviedPackages * 100f / info.DataCount;
                                    getNext = true;
                                }
                                request.ValuesGetNext = true; // раз дошел до сюда, то в следующий раз можно запрашивать следующую пачку

                                // забрать оставшиеся сообщения
                                getNext = request.MessagesGetNext;
                                request.MessagesGetNext = false;
                                while ((messages = blockManager.GetAsyncOperationMessages(request.BlockOperationID, getNext)) != null)
                                {
                                    state.AddMessage(messages);
                                    getNext = true;
                                }
                                request.MessagesGetNext = true;
                                // завершение запроса значений с блочного
                                blockManager.EndAsyncOperation(request.BlockOperationID);
                                break;
                            }
                        }
                        retryArgumentException = false;
                        request.AttemptsLeft = AttemptsNumber;
                        System.Threading.Thread.Sleep(StateSleep);
                    }
                    catch (ArgumentException) { if (retryArgumentException)request.BlockOperationID = 0; else throw; }
                    catch (System.Threading.ThreadInterruptedException) { }
                    catch (System.Runtime.Remoting.RemotingException) { retryArgumentException = true; --request.AttemptsLeft; }
                    catch (System.Net.Sockets.SocketException) { retryArgumentException = true; --request.AttemptsLeft; }
                    catch (FaultException) { request.BlockOperationID = 0; retryArgumentException = true; --request.AttemptsLeft; }
                    catch (CommunicationException) { retryArgumentException = true; DetachBlcok(request.Block.BlockUID); }
                }
                if (request.AttemptsLeft == 0) throw ThrowServerNotAccessibleException(request.Block);
            }
            catch (System.Threading.ThreadAbortException)
            {
                try     // при принудительной остановке потока попытатся прервать операцию
                {
                    IBlockQueryManager blockManager = GetQueryManager(request.Block.BlockUID);

                    blockManager.InteruptAsyncOperation(request.BlockOperationID);
                }
                catch (Exception) { }
            }
            catch (Exception exc) { state.AddMessage(new MessageByException(exc)); } //new Message(MessageCategory.Error, exc.Message)); }
            finally
            {
                log.Trace("asyncGetValuesFromObject() end");
            }
        }


        class ValueRequestState {
            public BlockNode Block { get; set; }

            public BlockParameterValuesRequest[] Requests { get; set; }

            /// <summary>
            /// Количетсво оставшихся попыток запроса данных
            /// </summary>
            public int AttemptsLeft { get; set; }

            /// <summary>
            /// Запрашивать следующую порцию данных или повтор последнего запроса
            /// </summary>
            public bool ValuesGetNext { get; set; }

            /// <summary>
            /// Запрашивать следующую порцию сообщений или повтор последнего запроса
            /// </summary>
            public bool MessagesGetNext { get; set; }

            /// <summary>
            /// Идентификатор асинхронной операции на сервере сбора данных
            /// </summary>
            public ulong BlockOperationID { get; set; }

            /// <summary>
            /// Состояние выполнения асинхронной операции
            /// </summary>
            public OperationState OperationState { get; set; }

            ///// <summary>
            ///// Количество пачек, полученных с сервера данных
            ///// </summary>
            //public int RetriviedPackages { get; set; }

            ///// <summary>
            ///// Текущий прогресс запроса данных для текущей операции
            ///// </summary>
            //public double Progress { get; set; }

            public bool UsePack { get; set; }

            public int RetriviedPackages { get; set; }
        }

        ///// <summary>
        ///// Режим запроса данных
        ///// </summary>
        //[Flags]
        //enum ValuesRequestMode
        //{
        //    /// <summary>
        //    /// Получить исходные данные
        //    /// </summary>
        //    Raw = 0,
        //    /// <summary>
        //    /// Получить проаппертуренные данные
        //    /// </summary>
        //    Appertured = 1,
        //    /// <summary>
        //    /// Получить агрегированные данные
        //    /// </summary>
        //    Aggregated = 2
        //}

        ///// <summary>
        ///// Хранение внутренней информации необходимой для запроса данных с сервера сбора
        ///// </summary>
        //class ValuesRequestStruct
        //{
        //    /// <summary>
        //    /// Узел сервера сбора данных
        //    /// </summary>
        //    public BlockNode Block { get; set; }

        //    /// <summary>
        //    /// Массив илентификаторов параметров
        //    /// </summary>
        //    public int[] ParametersIDs { get; set; }

        //    /// <summary>
        //    /// Использовать сжатие пачек
        //    /// </summary>
        //    public bool UsePack { get; set; }

        //    /// <summary>
        //    /// Количетсво оставшихся попыток запроса данных
        //    /// </summary>
        //    public int AttemptsLeft { get; set; }

        //    /// <summary>
        //    /// Режим запроса данных
        //    /// </summary>
        //    public ValuesRequestMode RequestMode { get; set; }

        //    /// <summary>
        //    /// Начальное время запрашиваемого периода
        //    /// </summary>
        //    public DateTime BeginTime { get; set; }

        //    /// <summary>
        //    /// Конечное время запрашиваемого периода
        //    /// </summary>
        //    public DateTime EndTime { get; set; }

        //    /// <summary>
        //    /// Интервал параметра для апертуры
        //    /// </summary>
        //    public Interval ParameterInterval { get; set; }

        //    /// <summary>
        //    /// Интервал агрегации
        //    /// </summary>
        //    public Interval AggregInterval { get; set; }

        //    /// <summary>
        //    /// Тип агрегации
        //    /// </summary>
        //    public CalcAggregation Aggregation { get; set; }

        //    /// <summary>
        //    /// Запрашивать следующую порцию данных или повтор последнего запроса
        //    /// </summary>
        //    public bool ValuesGetNext { get; set; }

        //    /// <summary>
        //    /// Запрашивать следующую порцию сообщений или повтор последнего запроса
        //    /// </summary>
        //    public bool MessagesGetNext { get; set; }

        //    /// <summary>
        //    /// Идентификатор асинхронной операции на сервере сбора данных
        //    /// </summary>
        //    public ulong BlockOperationID { get; set; }

        //    /// <summary>
        //    /// Состояние выполнения асинхронной операции
        //    /// </summary>
        //    public OperationState OperationState { get; set; }

        //    /// <summary>
        //    /// Количество пачек, полученных с сервера данных
        //    /// </summary>
        //    public int RetriviedPackages { get; set; }

        //    /// <summary>
        //    /// Текущий прогресс запроса данных для текущей операции
        //    /// </summary>
        //    public double Progress { get; set; }

        //    public ValuesRequestStruct(BlockNode block, int[] parameterIDs, bool usePack)
        //    {
        //        Block = block;
        //        ParametersIDs = parameterIDs;
        //        UsePack = usePack;
        //        ValuesGetNext = true;
        //        MessagesGetNext = true;
        //    }
        //}

        List<BlockNode> blocks = new List<BlockNode>();
        public IEnumerable<BlockNode> Blocks { get { return blocks.ToArray(); } }

        Dictionary<String, BlockCache> blockCache = new Dictionary<string, BlockCache>();
        
        //Dictionary<String, ChannelFactory<IBlockQueryManager>> dicBlockUIDs = new Dictionary<String, ChannelFactory<IBlockQueryManager>>();
        Dictionary<String, String> dicBlockUIDs = new Dictionary<String, String>();

        public bool AttachBlockE(String uid, IDictionary dicAttrs)
        {
            bool success = false;
            IBlockQueryManager qm;

            if (string.IsNullOrEmpty(uid))
                throw new ArgumentNullException("uid");
            if (dicAttrs == null) 
                throw new ArgumentException();

            string url = null;

            if (dicAttrs != null)
            {
                ChannelFactory<IBlockQueryManager> blockQueryFactory = null;
                int i = 0;
                do
                {
                    url = GetWeirdURL(dicAttrs, i++);
                    if (!string.IsNullOrEmpty(url))
                    {
                        EndpointAddress address = new EndpointAddress(url);
                        blockQueryFactory = new ChannelFactory<IBlockQueryManager>("NetTcpBinding_BlockQueryManager", address);
                        blockQueryFactory.Open();
                        qm = blockQueryFactory.CreateChannel();

                        try
                        {
                            qm.Test(null);
                        }
                        catch (CommunicationException) { 
                            //blockQueryFactory = null; 
                            url = null;
                        }
                    }
                } while (!String.IsNullOrEmpty(url) && blockQueryFactory == null);
            }
            //if (blockQueryFactory == null)
            //    return false;
            if (String.IsNullOrEmpty(url))
                return false;

            if (dicBlockUIDs.ContainsKey(uid))
            {
                //dicBlockUIDs[uid] = blockQueryFactory;
                dicBlockUIDs[uid] = url;

                success = true;
                log.Info("Обновлено подключение к серверу сбора данных.");
            }
            else
            {
                var cnt = (from elem in dicBlockUIDs.Keys
                           where dicBlockUIDs[elem] != null
                           select elem).Count();
                if (cnt < MaxBlockCount)
                {
                    //dicBlockUIDs[uid] = blockQueryFactory;
                    dicBlockUIDs[uid] = url;
                    success = true;
                    log.Info("Присоединен новый сервер сбора данных.");
                }
                else
                    log.Info("Достигнут лимит подключений серверов сбора.");
            }

            if (success)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(UdateCacheInfo, uid);
            }
            return success;
        }

        private void DetachBlcok(string uid)
        {
            dicBlockUIDs.Remove(uid);
            log.Error("Сервер сбора данных {0} отключен", uid);
        }
        /// <summary>
        /// Получить URL Блочного испульзуя полученную от него информацию.        
        /// </summary>
        /// <remarks>
        /// Используются следующие способы составить URL:
        /// <list type="number">
        /// <item>"ClientIPAddress" полученный из SinkProvidera;</item>
        /// <item>"external_ip" переданный из блочного;</item>
        /// <item>"url" переданный из блочного.</item>
        /// </list>
        /// </remarks>
        /// <param name="dicAttrs"></param>
        /// <param name="pass">Номер прохода, увеличивается если предыдущий возвращённый URL не подошёл.</param>
        /// <returns></returns>
        private string GetWeirdURL(IDictionary dicAttrs, int pass)
        {
            const String urlFormat = "net.tcp://{0}:{1}/{2}";
            string url = null;

            OperationContext context = OperationContext.Current;
            System.ServiceModel.Channels.MessageProperties prop = context.IncomingMessageProperties;
            System.ServiceModel.Channels.RemoteEndpointMessageProperty endpoint =
                prop[System.ServiceModel.Channels.RemoteEndpointMessageProperty.Name] as System.ServiceModel.Channels.RemoteEndpointMessageProperty;
            string clientIP = endpoint.Address;

            if (clientIP.Contains(':'))
            {
                clientIP = "[" + clientIP + "]";
            }

            if (pass < 1 && dicAttrs.Contains("port") && dicAttrs.Contains("rem"))
                url = string.Format(urlFormat, clientIP, dicAttrs["port"], dicAttrs["rem"]);
            else if (pass < 2 && dicAttrs.Contains("external_ip") && dicAttrs.Contains("port") && dicAttrs.Contains("rem"))
                url = string.Format(urlFormat, dicAttrs["external_ip"], dicAttrs["port"], dicAttrs["rem"]);
            else if (pass < 3 && dicAttrs.Contains("url"))
                url = dicAttrs["url"] as string;
            else if (pass < 1)
                throw new ArgumentException();
            return url;
        }

        private void UdateCacheInfo(Object state)
        {
            string uid = null;
            if (state != null) uid = state.ToString();

            lock (blockCache)
            {
                try
                {
                    IBlockQueryManager manager = GetQueryManager(uid);

                    var modules = manager.GetModulesInfo();
                    BlockCache cache = new BlockCache();
                    cache.Assign(modules);

                    blockCache[uid] = cache;
                }
                catch(Exception exc)
                {
                    log.ErrorException("Ошибка запроса метоинформации сервера сбора данных", exc);
                    if (blockCache.ContainsKey(uid))
                        blockCache.Remove(uid);
                }
            }
        }

        public IEnumerable<ItemProperty> GetModuleLibraryProperties(BlockNode blockNode, String libName)
        {
            BlockCache cache;

            if (blockCache.TryGetValue(blockNode.BlockUID, out cache))
            {
                ModuleInfo moduleInfo;
                if (cache.Modules.TryGetValue(libName, out moduleInfo))
                    return moduleInfo.ChannelProperties;
            }

            return new ItemProperty[0];
        }

        public IEnumerable<ModuleInfo> GetModuleLibNames(BlockNode blockNode)
        {
            BlockCache cache;

            if (blockCache.TryGetValue(blockNode.BlockUID, out cache))
                return cache.Modules.Values.ToArray();

            return null;
        }

        public void AddBlock(BlockNode newnode)
        {
            foreach (var item in blocks)
            {
                if (item.Idnum == newnode.Idnum)
                {
                    blocks.Remove(item);
                    break;
                }
            }
            blocks.Add(newnode);
            //if (newnode.Attributes.ContainsKey(CommonData.BlockUIDProperty))
            //{
            //    IBlockQueryManager qm;
            //    if (dicBlockUIDs.TryGetValue(newnode.Attributes[CommonData.BlockUIDProperty], out qm))
            //    {
            //        //нужен lock?
            //        ((BlockNode)newnode).SetBlockManager(qm);
            //    }
            //}
        }

        public string[] GetBlockUIDs()
        {
            string[] arr = new string[dicBlockUIDs.Keys.Count];
            //return GNSI.lstBlockUIDs.ToArray();
            dicBlockUIDs.Keys.CopyTo(arr, 0);

            return arr;
        }

        public void GetParameters(OperationState state, ChannelNode channelNode)
        {
            BlockNode block = Manager.GetParentNode(state, channelNode, (int)UnitTypeId.Block) as BlockNode;
            //IBlockQueryManager qmanager;

            try
            {
                IBlockQueryManager qmanager = GetQueryManager(block.BlockUID);

                //if (dicBlockUIDs.TryGetValue(block.BlockUID, out qmanager))
                //{
                    List<ParameterItem> paramList = new List<ParameterItem>();
                    LoadParameterNode paramNode;
                    ParameterItem[] pars = qmanager.GetParamList(channelNode.Idnum);

                    foreach (var item in pars)
                    {
                        bool exclude = false;
                        String tag = item.GetPropertyValue("code");

                        UnitNode[] nodes = Manager.GetUnitNodes(state, channelNode.NodesIds);
                        if (nodes != null)
                            for (int i = 0; !exclude && i < nodes.Length; i++)
                            {
                                exclude = (paramNode = nodes[i] as LoadParameterNode) != null && paramNode.LoadCode.Equals(tag);
                            }
                        if (!exclude) paramList.Add(item);
                    }
                    if (paramList.Count > 0) state.AddAsyncResult(paramList.ToArray());
                //}
            }
            catch { }
        }

        public BlockNode GetBlock(string uid)
        {
            return blocks.Find(b => b.BlockUID == uid);
        }

        public void DeleteLoadValues(OperationState state, UnitNode unitNode, DateTime timeFrom)
        {
            //IBlockQueryManager qmanager;
            BlockNode block;

            if (unitNode.Typ == (int)UnitTypeId.Channel)
                block = Manager.GetParentNode(state, unitNode, (int)UnitTypeId.Block) as BlockNode;
            else block = unitNode as BlockNode;

            try
            {
                //if (block != null && dicBlockUIDs.TryGetValue(block.BlockUID, out qmanager))
                if (block != null)
                {
                    IBlockQueryManager qmanager = GetQueryManager(block.BlockUID);

                    int idnum;

                    if (unitNode.Typ == (int)UnitTypeId.Channel)
                        idnum = unitNode.Idnum;
                    else idnum = 0;

                    qmanager.DeleteValues(idnum, timeFrom);
                }
            }
            catch { }
        }

        public bool Attached(string blockUID)
        {
            //ChannelFactory<IBlockQueryManager> blockManager;
            String blockManager;
            return dicBlockUIDs.TryGetValue(blockUID, out blockManager) && blockManager != null;
        }
    }

    class BlockCache
    {
        public Dictionary<String, ModuleInfo> Modules { get; private set; }

        public BlockCache()
        {
            Modules = new Dictionary<string, ModuleInfo>();
        }

        public void Assign(IEnumerable<ModuleInfo> modules)
        {
            foreach (var module in modules)
            {
                Modules[module.Name] = module;
            }
        }
    }
}
