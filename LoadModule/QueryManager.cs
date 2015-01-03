using System;
using System.Linq;
using System.ServiceModel;
using COTES.ISTOK.Block.Redundancy;
using COTES.ISTOK.DiagnosticsInfo;

namespace COTES.ISTOK.Block
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    class QueryManager : MarshalByRefObject, IBlockQueryManager, IRedundancy, IDisposable
    {
        private AsyncOperation asyncOperation = new AsyncOperation(NSI.StartNum);

        Diagnostics blockDiagnostics;

        public QueryManager(Diagnostics blockDiagnostics)
        {
            this.blockDiagnostics = blockDiagnostics;
        }

        /// <summary>
        /// Имя компьютера сервиса
        /// </summary>
        public string Host { get; internal set; }
        /// <summary>
        /// Порт сервиса
        /// </summary>
        public int Port { get; internal set; }

        #region Диагностика
        /// <summary>
        /// Возвращает диагностический объект
        /// </summary>
        /// <returns></returns>
        public COTES.ISTOK.DiagnosticsInfo.Diagnostics GetDiagnosticsObject()
        {
            return blockDiagnostics;
        }

        public System.Data.DataSet GetDiagnosticInfo()
        {
            return blockDiagnostics.GetAllInfo();
        }
        #endregion

        #region Работа с каналами
        //public void LoadChannels()
        //{
        //    if (NSI.chanManager == null)
        //        return;

        //    NSI.chanManager.LoadChannels();
        //}
        //public void LoadChannel(int id)
        //{
        //    if (NSI.chanManager == null)
        //        return;

        //    NSI.chanManager.LoadChannel(id);
        //}
        //public void UnloadChannels()
        //{
        //    if (NSI.chanManager == null)
        //        return;

        //    NSI.chanManager.UnloadChannels();
        //}
        //public void UnloadChannel(int id)
        //{
        //    if (NSI.chanManager == null)
        //        return;

        //    NSI.chanManager.UnloadChannel(id);
        //}
        //public void StartChannels()
        //{
        //    if (NSI.chanManager == null)
        //        return;

        //    NSI.chanManager.StartChannels();
        //}
        //public void StartChannel(int id)
        //{
        //    if (NSI.chanManager == null)
        //        return;

        //    NSI.chanManager.StartChannel(id);
        //}
        //public void StopChannels()
        //{
        //    if (NSI.chanManager == null)
        //        return;

        //    NSI.chanManager.StopChannels();
        //}
        //public void StopChannel(int id)
        //{
        //    if (NSI.chanManager == null)
        //        return;

        //    NSI.chanManager.StopChannel(id);
        //}
        //public int[] GetLoadedChannels()
        //{
        //    if (NSI.chanManager == null)
        //        return null;

        //    return NSI.chanManager.GetLoadedChannels();
        //}
        ////public int[] GetChannelIds()
        ////{
        ////    return NSI.chanManager.GetChannels();
        ////}
        ////метод для дебага
        ////public ChannelItem GetChannel(int id)
        ////{
        ////    return NSI.chanManager.GetChannel(id);
        ////}
        #endregion

        //#region GetAppertureValues()
        ///// <summary>
        ///// Запросить значения параметра за период с учетом апертуры
        ///// </summary>
        ///// <param name="param_id">ИД параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <returns></returns>
        //public ulong BeginGetAppertureValues1(int param_id, DateTime beginTime, DateTime endTime, Interval parameterInterval)
        //{
        //    //return NSI.valReceiver.GetAppertureValues(param_id, beginTime, endTime, parameterInterval);
        //    return asyncOperation.BeginAsyncOperation(Guid.Empty, asyncGetAppertureValues, param_id, beginTime, endTime, parameterInterval);
        //}
        ///// <summary>
        ///// Запросить значения параметра за период с учетом апертуры
        ///// </summary>
        ///// <param name="param_id">ИД параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <param name="aggregInterval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns></returns>
        //public ulong BeginGetAppertureValues2(int param_id, DateTime beginTime, DateTime endTime,
        //    Interval parameterInterval, Interval aggregInterval, CalcAggregation aggregation)
        //{
        //    //return NSI.valReceiver.GetAppertureValues(param_id, beginTime, endTime, parameterInterval,
        //    //    aggregInterval, aggregation);
        //    return asyncOperation.BeginAsyncOperation(Guid.Empty, asyncGetAppertureValues, param_id, beginTime, endTime, parameterInterval, aggregInterval, aggregation);
        //}
        ///// <summary>
        ///// Запросить значения нескольких параметров за период с учетом апертуры
        ///// </summary>
        ///// <param name="param_ids">Массив ИД параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <returns></returns>
        //public ulong BeginGetAppertureValues3(int[] param_ids, DateTime beginTime, DateTime endTime, Interval parameterInterval)
        //{
        //    //return NSI.valReceiver.GetAppertureValues(param_ids, beginTime, endTime, parameterInterval);
        //    return asyncOperation.BeginAsyncOperation(Guid.Empty, asyncGetMultiplyAppertureValues, param_ids, beginTime, endTime, parameterInterval);
        //}
        ///// <summary>
        ///// Запросить значения нескольких параметров за период с учетом апертуры
        ///// </summary>
        ///// <param name="param_ids">Массив ИД параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <param name="aggregInterval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns></returns>
        //public ulong BeginGetAppertureValues4(int[] param_ids, DateTime beginTime, DateTime endTime,
        //    Interval parameterInterval, Interval aggregInterval, CalcAggregation aggregation)
        //{
        //    //return NSI.valReceiver.GetAppertureValues(param_ids, beginTime, endTime,
        //    //    parameterInterval, aggregInterval, aggregation);
        //    return asyncOperation.BeginAsyncOperation(Guid.Empty, asyncGetMultiplyAppertureValues, param_ids, beginTime, endTime, parameterInterval, aggregInterval, aggregation);
        //}

        //private Object asyncGetAppertureValues(OperationState state, params Object[] pars)
        //{
        //    if (NSI.valReceiver == null)
        //        return null;

        //    int param_id;
        //    DateTime beginTime, endTime;
        //    Interval parameterInterval, aggregInterval;
        //    CalcAggregation aggregation;
        //    double progressToLoad = AsyncOperation.MaxProgressValue;

        //    if (pars == null) throw new ArgumentNullException("pars");
        //    param_id = (int)pars[0];
        //    beginTime = (DateTime)pars[1];
        //    endTime = (DateTime)pars[2];
        //    parameterInterval = (Interval)pars[3];
        //    if (pars.Length > 5)
        //    {
        //        aggregInterval = (Interval)pars[4];
        //        aggregation = (CalcAggregation)pars[5];
        //        NSI.valReceiver.AsyncGetAppertureValues(state, progressToLoad, param_id, beginTime, endTime, parameterInterval, aggregInterval, aggregation);
        //    }
        //    else NSI.valReceiver.AsyncGetAppertureValues(state, progressToLoad, param_id, beginTime, endTime, parameterInterval);

        //    return null;
        //}

        //private Object asyncGetMultiplyAppertureValues(OperationState state, params Object[] pars)
        //{
        //    if (NSI.valReceiver == null)
        //        return null;

        //    int[] param_ids;
        //    DateTime beginTime, endTime;
        //    Interval parameterInterval, aggregInterval;
        //    CalcAggregation aggregation;
        //    double progressToLoad = AsyncOperation.MaxProgressValue;

        //    if (pars == null) throw new ArgumentNullException("pars");
        //    param_ids = (int[])pars[0];
        //    beginTime = (DateTime)pars[1];
        //    endTime = (DateTime)pars[2];
        //    parameterInterval = (Interval)pars[3];
        //    if (pars.Length > 5)
        //    {
        //        aggregInterval = (Interval)pars[4];
        //        aggregation = (CalcAggregation)pars[5];
        //        NSI.valReceiver.AsyncGetAppertureValues(state, progressToLoad, param_ids, beginTime, endTime, parameterInterval, aggregInterval, aggregation);
        //    }
        //    else NSI.valReceiver.AsyncGetAppertureValues(state, progressToLoad, param_ids, beginTime, endTime, parameterInterval);

        //    return null;
        //}
        //#endregion

        //#region GetPackedAppertureValues()
        ///// <summary>
        ///// Запросить значения параметра за период с учетом апертуры
        ///// </summary>
        ///// <param name="param_id">ИД параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <returns></returns>
        //public ulong BeginGetPackedAppertureValues1(int param_id, DateTime beginTime, DateTime endTime, Interval parameterInterval)
        //{
        //    //return NSI.valReceiver.GetPackedAppertureValues(param_id, beginTime, endTime, parameterInterval);
        //    return asyncOperation.BeginAsyncOperation(Guid.Empty, asyncGetPackedAppertureValues, param_id, beginTime, endTime, parameterInterval);
        //}
        ///// <summary>
        ///// Запросить значения параметра за период с учетом апертуры
        ///// </summary>
        ///// <param name="param_id">ИД параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <param name="aggregInterval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns></returns>
        //public ulong BeginGetPackedAppertureValues2(int param_id, DateTime beginTime, DateTime endTime,
        //    Interval parameterInterval, Interval aggregInterval, CalcAggregation aggregation)
        //{
        //    //return NSI.valReceiver.GetPackedAppertureValues(param_id, beginTime, endTime, parameterInterval,
        //    //    aggregInterval, aggregation);
        //    return asyncOperation.BeginAsyncOperation(Guid.Empty, asyncGetPackedAppertureValues, param_id, beginTime, endTime, parameterInterval, aggregInterval, aggregation);
        //}
        ///// <summary>
        ///// Запросить значения нескольких параметров за период с учетом апертуры
        ///// </summary>
        ///// <param name="param_ids">Массив ИД параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <returns></returns>
        //public ulong BeginGetPackedAppertureValues3(int[] param_ids, DateTime beginTime, DateTime endTime, Interval parameterInterval)
        //{
        //    //return NSI.valReceiver.GetPackedAppertureValues(param_ids, beginTime, endTime, parameterInterval);
        //    return asyncOperation.BeginAsyncOperation(Guid.Empty, asyncGetPackedMultiplyAppertureValues, param_ids, beginTime, endTime,
        //        parameterInterval);
        //}
        ///// <summary>
        ///// Запросить значения нескольких параметров за период с учетом апертуры
        ///// </summary>
        ///// <param name="param_ids">Массив ИД параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="parameterInterval">Исходный интервал параметра</param>
        ///// <param name="aggregInterval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns></returns>
        //public ulong BeginGetPackedAppertureValues4(int[] param_ids, DateTime beginTime, DateTime endTime,
        //    Interval parameterInterval, Interval aggregInterval, CalcAggregation aggregation)
        //{
        //    //return NSI.valReceiver.GetPackedAppertureValues(param_ids, beginTime, endTime,
        //    //    parameterInterval, aggregInterval, aggregation);
        //    return asyncOperation.BeginAsyncOperation(Guid.Empty, asyncGetPackedMultiplyAppertureValues, param_ids, beginTime, endTime,
        //        parameterInterval, aggregInterval, aggregation);
        //}

        //private Object asyncGetPackedAppertureValues(OperationState state, params Object[] pars)
        //{
        //    int param_id;
        //    DateTime beginTime, endTime;
        //    Interval parameterInterval, aggregInterval;
        //    CalcAggregation aggregation;
        //    double progressToLoad = AsyncOperation.MaxProgressValue;

        //    if (pars == null) throw new ArgumentNullException("pars");
        //    param_id = (int)pars[0];
        //    beginTime = (DateTime)pars[1];
        //    endTime = (DateTime)pars[2];
        //    parameterInterval = (Interval)pars[3];
        //    if (pars.Length > 5)
        //    {
        //        aggregInterval = (Interval)pars[4];
        //        aggregation = (CalcAggregation)pars[5];
        //        NSI.valReceiver.AsyncGetPackedAppertureValues(state, progressToLoad, param_id, beginTime, endTime, parameterInterval, aggregInterval, aggregation);
        //    }
        //    else NSI.valReceiver.AsyncGetPackedAppertureValues(state, progressToLoad, param_id, beginTime, endTime, parameterInterval);

        //    return null;
        //}

        //private Object asyncGetPackedMultiplyAppertureValues(OperationState state, params Object[] pars)
        //{
        //    int[] param_ids;
        //    DateTime beginTime, endTime;
        //    Interval parameterInterval, aggregInterval;
        //    CalcAggregation aggregation;
        //    double progressToLoad = AsyncOperation.MaxProgressValue;

        //    if (pars == null) throw new ArgumentNullException("pars");
        //    param_ids = (int[])pars[0];
        //    beginTime = (DateTime)pars[1];
        //    endTime = (DateTime)pars[2];
        //    parameterInterval = (Interval)pars[3];
        //    if (pars.Length > 5)
        //    {
        //        aggregInterval = (Interval)pars[4];
        //        aggregation = (CalcAggregation)pars[5];
        //        NSI.valReceiver.AsyncGetPackedAppertureValues(state, progressToLoad, param_ids, beginTime, endTime, parameterInterval, aggregInterval, aggregation);
        //    }
        //    else NSI.valReceiver.AsyncGetPackedAppertureValues(state, progressToLoad, param_ids, beginTime, endTime, parameterInterval);

        //    return null;
        //}
        //#endregion

        #region Асинхронные методы
        /// <summary>
        /// Получить состояние текущей выполняемой асинхронной операции
        /// </summary>
        /// <param name="operationID">Идентификатор операции</param>
        /// <returns>Состояние операции</returns>
        public OperationInfo GetAsyncOperationState(ulong operationId)
        {
            return asyncOperation.GetAsyncOperationState(operationId);
        }

        /// <summary>
        /// Получить порцию возвращаемых результатов асинхронной операции
        /// </summary>
        /// <param name="operationID">Идентификатор операции</param>
        /// <param name="next">true, если запрашивается следующая пачка данных, false - если требуется повторить предыдущий запрос данных</param>
        /// <returns>Полученная порция данных</returns>
        public UAsyncResult GetAsyncOperationData(ulong operationID, bool next)
        {
            Object obj = asyncOperation.GetAsyncOperationData(operationID, next);
            if (obj == null)
            {
                return null;
            }
            var ret = new UAsyncResult()
            {
                Packages = obj is Package ? new Package[] { obj as Package } : obj as Package[],
                PackedPackage = obj is PackedPackage ? new PackedPackage[] { obj as PackedPackage } : obj as PackedPackage[]
            };
            return ret;
        }

        /// <summary>
        /// Получить порцию сообщений асинхронной операции
        /// </summary>
        /// <param name="operationID">Идентификатор операции</param>
        /// <param name="next">true, если запрашивается следующая пачка сообщений, false - если требуется повторить предыдущий запрос сообщений</param>
        /// <returns>Полученная пачка сообщений</returns>
        public Message[] GetAsyncOperationMessages(ulong operationID, bool next)
        {
            return asyncOperation.GetAsyncOperationMessages(operationID, next);
        }

        public bool WaitAsyncOperation(ulong operationID, int timeout)
        {
            return asyncOperation.WaitEndOperation(operationID, timeout);
        }

        public void WaitAsyncOperation(ulong operationID)
        {
            asyncOperation.WaitEndOperation(operationID);
        }

        /// <summary>
        /// Завершить и удалить информацию о асинхронной операции 
        /// </summary>
        /// <param name="operationID">Идентификатор операции</param>
        /// <exception cref="System.Exception">Если операция завершилась ошибкой</exception>
        /// <exception cref="System.ArgumentException">Операции с таким идентификатором не существует</exception>
        public void EndAsyncOperation(ulong operationId)
        {
            asyncOperation.EndAsyncOperation(operationId);
        }

        /// <summary>
        /// Прервать выполнение асинхронной операции
        /// </summary>
        /// <param name="operationID">Идентификатор операции</param>
        public void InteruptAsyncOperation(ulong operationID)
        {
            asyncOperation.InteruptAsyncOperation(operationID);
        }
        #endregion

        #region GetValues()
        /// <summary>
        /// Получить последнее значение параметра
        /// </summary>
        /// <param name="param_id">Номер параметра</param>
        /// <returns>Параметр</returns>
        public ParamValueItem GetLastValue(int param_id)
        {
            return NSI.valReceiver.GetLastValue(param_id);
        }

        public ulong BeginGetValues(BlockParameterValuesRequest[] valuesRequest, bool packed)
        {
            return asyncOperation.BeginAsyncOperation(Guid.Empty, (OperationState state, Object[] args) =>
                {
                    NSI.valReceiver.GetValues(state, valuesRequest, packed);
                    return null;
                });
        }

        ///// <summary>
        ///// Запросить значения параметра за период
        ///// </summary>
        ///// <param name="param_id">ИД параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <returns></returns>
        //public ulong BeginGetValues1(int param_id, DateTime beginTime, DateTime endTime)
        //{
        //    //return NSI.valReceiver.GetValues(param_id, beginTime, endTime);
        //    return asyncOperation.BeginAsyncOperation(Guid.Empty, asyncGetValues, param_id, beginTime, endTime);
        //}

        ///// <summary>
        ///// Запросить значения параметра за период
        ///// </summary>
        ///// <param name="param_id">ИД параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns></returns>
        //public ulong BeginGetValues2(int param_id, DateTime beginTime, DateTime endTime,
        //    Interval interval, CalcAggregation aggregation)
        //{
        //    //return NSI.valReceiver.GetValues(param_id, beginTime, endTime, interval, aggregation);
        //    return asyncOperation.BeginAsyncOperation(Guid.Empty, asyncGetValues, param_id, beginTime, endTime, interval, aggregation);
        //}
        ///// <summary>
        ///// Запросить значения нескольких параметров за период
        ///// </summary>
        ///// <param name="param_ids">Массив ИД параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <returns></returns>
        //public ulong BeginGetValues3(int[] param_ids, DateTime beginTime, DateTime endTime)
        //{
        //    //return NSI.valReceiver.GetValues(param_ids, beginTime, endTime);
        //    return asyncOperation.BeginAsyncOperation(Guid.Empty, asyncGetMultiplyValues, param_ids, beginTime, endTime);
        //}
        ///// <summary>
        ///// Запросить значения нескольких параметров за период
        ///// </summary>
        ///// <param name="param_ids">Массив ИД параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns></returns>
        //public ulong BeginGetValues4(int[] param_ids, DateTime beginTime, DateTime endTime, Interval interval, CalcAggregation aggregation)
        //{
        //    //return NSI.valReceiver.GetValues(param_ids, beginTime, endTime, interval, aggregation);
        //    return asyncOperation.BeginAsyncOperation(Guid.Empty, asyncGetMultiplyValues, param_ids, beginTime, endTime, interval, aggregation);
        //}

        //private Object asyncGetValues(OperationState state, params Object[] pars)
        //{
        //    int param_id;
        //    DateTime beginTime, endTime;
        //    Interval interval;
        //    CalcAggregation aggregation;
        //    double progressToLoad = AsyncOperation.MaxProgressValue;

        //    if (pars == null) throw new ArgumentNullException("pars");
        //    param_id = (int)pars[0];
        //    beginTime = (DateTime)pars[1];
        //    endTime = (DateTime)pars[2];
        //    if (pars.Length > 4)
        //    {
        //        interval = (Interval)pars[3];
        //        aggregation = (CalcAggregation)pars[4];
        //        NSI.valReceiver.AsyncGetValues(state, progressToLoad, param_id, beginTime, endTime, interval, aggregation);
        //    }
        //    else NSI.valReceiver.AsyncGetValues(state, progressToLoad, param_id, beginTime, endTime);

        //    return null;
        //}

        //private Object asyncGetMultiplyValues(OperationState state, params Object[] pars)
        //{
        //    int[] param_ids;
        //    DateTime beginTime, endTime;
        //    Interval interval;
        //    CalcAggregation aggregation;
        //    double progressToLoad = AsyncOperation.MaxProgressValue;

        //    if (pars == null) throw new ArgumentNullException("pars");
        //    param_ids = (int[])pars[0];
        //    beginTime = (DateTime)pars[1];
        //    endTime = (DateTime)pars[2];
        //    if (pars.Length > 4)
        //    {
        //        interval = (Interval)pars[3];
        //        aggregation = (CalcAggregation)pars[4];
        //        NSI.valReceiver.AsyncGetValues(state, progressToLoad, param_ids, beginTime, endTime, interval, aggregation);
        //    }
        //    else NSI.valReceiver.AsyncGetValues(state, progressToLoad, param_ids, beginTime, endTime);

        //    return null;
        //}
        #endregion

        //#region GetPackedValues()
        ///// <summary>
        ///// Запросить значения параметра за период
        ///// </summary>
        ///// <param name="param_id">ИД параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <returns></returns>
        //public ulong BeginGetPackedValues1(int param_id, DateTime beginTime, DateTime endTime)
        //{
        //    //return NSI.valReceiver.GetPackedValues(param_id, beginTime, endTime);
        //    return asyncOperation.BeginAsyncOperation(Guid.Empty, asyncGetPackedValues, param_id, beginTime, endTime);
        //}

        ///// <summary>
        ///// Запросить значения параметра за период
        ///// </summary>
        ///// <param name="param_id">ИД параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns></returns>
        //public ulong BeginGetPackedValues2(int param_id, DateTime beginTime, DateTime endTime,
        //    Interval interval, CalcAggregation aggregation)
        //{
        //    //return NSI.valReceiver.GetPackedValues(param_id, beginTime, endTime, interval, aggregation);
        //    return asyncOperation.BeginAsyncOperation(Guid.Empty, asyncGetPackedValues, param_id, beginTime, endTime, interval, aggregation);
        //}

        ///// <summary>
        ///// Запросить значения нескольких параметров за период
        ///// </summary>
        ///// <param name="param_ids">Массив ИД параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <returns></returns>
        //public ulong BeginGetPackedValues3(int[] param_ids, DateTime beginTime, DateTime endTime)
        //{
        //    //return NSI.valReceiver.GetPackedValues(param_ids, beginTime, endTime);
        //    return asyncOperation.BeginAsyncOperation(Guid.Empty, asyncGetMultiplyPackedValues, param_ids, beginTime, endTime);
        //}

        ///// <summary>
        ///// Запросить значения нескольких параметров за период
        ///// </summary>
        ///// <param name="param_ids">Массив ИД параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода</param>
        ///// <param name="endTime">Конечное время запрашиваемого периода</param>
        ///// <param name="interval">Интервал агрегации (с каким периодом будут возвращаемые значения)</param>
        ///// <param name="aggregation">Тип агрегации, применяемый к значениям</param>
        ///// <returns></returns>
        //public ulong BeginGetPackedValues4(int[] param_ids, DateTime beginTime, DateTime endTime, Interval interval, CalcAggregation aggregation)
        //{
        //    //return NSI.valReceiver.GetPackedValues(param_ids, beginTime, endTime, interval, aggregation);
        //    return asyncOperation.BeginAsyncOperation(Guid.Empty, asyncGetMultiplyPackedValues, param_ids, beginTime, endTime, interval, aggregation);
        //}

        //private Object asyncGetPackedValues(OperationState state, params Object[] pars)
        //{
        //    int param_id;
        //    DateTime beginTime, endTime;
        //    Interval interval;
        //    CalcAggregation aggregation;
        //    double progressToLoad = AsyncOperation.MaxProgressValue;

        //    if (pars == null) throw new ArgumentNullException("pars");
        //    param_id = (int)pars[0];
        //    beginTime = (DateTime)pars[1];
        //    endTime = (DateTime)pars[2];
        //    if (pars.Length > 4)
        //    {
        //        interval = (Interval)pars[3];
        //        aggregation = (CalcAggregation)pars[4];
        //        NSI.valReceiver.AsyncGetPackedValues(state, progressToLoad, param_id, beginTime, endTime, interval, aggregation);
        //    }
        //    else NSI.valReceiver.AsyncGetPackedValues(state, progressToLoad, param_id, beginTime, endTime);

        //    return null;
        //}

        //private Object asyncGetMultiplyPackedValues(OperationState state, params Object[] pars)
        //{
        //    int[] param_ids;
        //    DateTime beginTime, endTime;
        //    Interval interval;
        //    CalcAggregation aggregation;
        //    double progressToLoad = AsyncOperation.MaxProgressValue;

        //    if (pars == null) throw new ArgumentNullException("pars");
        //    param_ids = (int[])pars[0];
        //    beginTime = (DateTime)pars[1];
        //    endTime = (DateTime)pars[2];
        //    if (pars.Length > 4)
        //    {
        //        interval = (Interval)pars[3];
        //        aggregation = (CalcAggregation)pars[4];
        //        NSI.valReceiver.AsyncGetPackedValues(state, progressToLoad, param_ids, beginTime, endTime, interval, aggregation);
        //    }
        //    else NSI.valReceiver.AsyncGetPackedValues(state, progressToLoad, param_ids, beginTime, endTime);

        //    return null;
        //}
        //#endregion

        public void StopLoaderAsync()
        {
            // останавливаем все каналы и не ждем
            //NSI.channel.StopAsync();
        }
        public ulong SetAllSprav(byte[] input)
        {
            return asyncOperation.BeginAsyncOperation(Guid.Empty, (state, args) =>
            {
                state.StateString = "Suspend";
                NSI.redundancyServer.Suspend();
                state.StateString = "Stop channels";

                NSI.chanManager.StopManager();
                state.StateString = "Upload";
                NSI.dalManager.UploadInfo(input, true);
                //infoUpdating = false;
                state.StateString = "Start channels";
                NSI.chanManager.StartManager();
                state.StateString = "Continue";
                NSI.redundancyServer.Continue();
                return null;

            });
            // плохое решение
            //NSI.scheduler.ReloadScheduleFromDB();
            //NSI.blockDAL.deleteExcessValues();
        }
        public ulong SetChannelSprav(int channelId, byte[] input)
        {
            return asyncOperation.BeginAsyncOperation(Guid.Empty, (state, args) =>
            {
                ChannelInfo info = NSI.chanManager.GetLoadedChannels().Where(i => i.Id == channelId).FirstOrDefault();

                NSI.chanManager.StopChannel(info);
                //UnloadChannel(channelId);
                NSI.dalManager.RemoveChannel(channelId);
                NSI.dalManager.UploadInfo(input, false);
                //infoUpdating = false;
                //LoadChannel(channelId);
                if (info != null)
                {
                    NSI.chanManager.ReloadChannel(info);
                    NSI.chanManager.StartChannel(info);
                }
                return null;
            });
            // плохое решение
            //NSI.scheduler.ReloadScheduleFromDB();
        }

        /// <summary> Запросить у загруженных модулей список параметров канала с атрибутами
        /// </summary>
        /// <param name="channelId">Идентификатор канала</param>
        /// <returns>Возвращает список параметров</returns>
        public ParameterItem[] GetParamList(int channelId)
        {
            ChannelInfo info = NSI.chanManager.GetLoadedChannels().Where(i => i.Id == channelId).FirstOrDefault();

            return NSI.chanManager.GetParameters(info);
        }

        /// <summary>        
        /// This is to insure that when created as a Singleton, the first instance never dies,
        /// regardless of the expired time.
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region ITestConnection Members

        public bool Test(Object state)
        {
            return true;
        }

        #endregion

        #region Schema parts
        /// <summary>
        /// Выбирает параметры из массива, которые есть в блочном
        /// </summary>
        /// <param name="parameters">Массив параметров</param>
        /// <returns>Массив параметров, которые есть в блочном</returns>
        public ParamValueItemWithID[] FilterParameters(ParamValueItemWithID[] parameters)
        {
            //return NSI.chanManager.FilterParameters(parameters);
            return parameters;
        }

        /// <summary>       
        /// Регистрация массива параметров
        /// </summary>
        /// <param name="parameters">Массив параметров</param>
        /// <returns>Номер транзакции</returns>
        public int RegisterClient(ParamValueItemWithID[] parameters)
        {
            if (NSI.parRegistrator == null)
                return -1;

            return NSI.parRegistrator.Register(parameters);
        }
        /// <summary>
        /// Разрегистрация массива параметров
        /// </summary>
        /// <param name="taID">Номер транзакции</param>
        public void UnRegisterClient(int taID)
        {
            if (NSI.parRegistrator != null)
            {
                NSI.parRegistrator.Unregister(taID);
            }
        }
        /// <summary>
        /// Получение массива значений обновляемого параметра
        /// </summary>
        /// <param name="taID">Номер транзакции</param>
        /// <returns>Массив параметров со значениями</returns>
        public ParamValueItemWithID[] GetValuesFromBank(int taID)
        {
            if (NSI.parRegistrator == null)
                return null;
            return NSI.parRegistrator.GetValues(taID);
        }
        #endregion

        #region IRedundancy Members

        public string UID
        {
            get
            {
                return BlockSettings.Instance.ServerName;//OldClientSettings.Instance.LoadKey("Settings/UniqueServerName");
            }
        }
        /// <summary>
        /// Возвращает состояние сервера
        /// </summary>
        /// <returns></returns>
        public ServerState GetState()
        {
            if (NSI.redundancyServer == null)
                return ServerState.Offline;

            return NSI.redundancyServer.GetState();
        }
        /// <summary>
        /// Возвращает информацию о сервере
        /// </summary>
        /// <returns></returns>
        public ServerInfo GetInfo()
        {
            if (NSI.redundancyServer == null)
                return null;

            return NSI.redundancyServer.GetInfo();
        }

        public ServerInfo[] GetServers()
        {
            if (NSI.redundancyServer == null)
                return null;

            return NSI.redundancyServer.GetServers();
        }

        public ServerInfo[] AttachServer(ServerInfo serverInfo, TransferDirection direction)
        {
            if (NSI.redundancyServer == null)
                return null;

            return NSI.redundancyServer.AttachServer(serverInfo, direction);
        }

        public void AddCommands(RedundancyCommand[] commands)
        {
            if (NSI.redundancyServer == null)
                return;

            NSI.redundancyServer.AddCommands(commands);
        }

        //public PublicationInfo GetReplicationPublication()
        //{
        //    return NSI.redundancyServer.GetReplicationPublication();
        //}

        public string GetConnectionString()
        {
            if (NSI.redundancyServer == null)
                return null;

            return NSI.redundancyServer.GetConnectionString();
        }

        public PublicationInfo GetPublicationInfo()
        {
            if (NSI.redundancyServer == null)
                return null;

            return NSI.redundancyServer.GetPublicationInfo();
        }

        #endregion

        public void SendDataToGlobal(int id)
        {
            try
            {
                IGlobal global = NSI.conInspector.GlobalServer;

                if (global == null)
                    return;
                //global.
            }
            catch (Exception)
            {
                //
            }
        }

        public ModuleInfo[] GetModulesInfo()
        {
            if (NSI.chanManager == null)
                return null;

            return NSI.chanManager.GetAvailableModules();
        }

        //public String[] GetModuleLibNames()
        //{
        //    if (NSI.chanManager == null)
        //        return null;

        //    return NSI.chanManager.GetModuleLibNames();
        //}

        //public ItemProperty[] GetModuleLibraryProperties(String libName)
        //{
        //    if (NSI.chanManager == null)
        //        return null;

        //    return NSI.chanManager.GetItemProperty(libName);
        //    //throw new NotImplementedException();
        //}

        public void DeleteValues(int idnum, DateTime timeFrom)
        {
            if (NSI.chanManager == null)
                return;

            ChannelInfo info = NSI.chanManager.GetLoadedChannels().Where(i => i.Id == idnum).FirstOrDefault();

            NSI.chanManager.DeleteValues(info, timeFrom);
        }

        #region IDisposable Members

        public void Dispose()
        {
            asyncOperation.Dispose();
        }

        #endregion
    }
}
