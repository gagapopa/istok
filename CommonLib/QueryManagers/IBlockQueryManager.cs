using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace COTES.ISTOK
{
    [Serializable]
    public class BlockParameterValuesRequest
    {
        public ParameterItem[] Parameters { get; set; }

        public Interval AggregationInterval { get; set; }

        public CalcAggregation Aggregation { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool Appertured { get; set; }

        public Interval AppertureInterval { get; set; }
    }


    /// <summary>
    /// Интерфейс фасада сервера сбора данных
    /// </summary>
    [ServiceContract]
    public interface IBlockQueryManager : ITestConnection<Object>
    {
        #region Запрос значений
        /// <summary>
        /// Получить последнее значение параметра
        /// </summary>
        /// <param name="parameterID">Номер параметра</param>
        /// <returns>Параметр</returns>
        [OperationContract]
        ParamValueItem GetLastValue(int parameterID);
        #endregion

        void StopLoaderAsync();

        [OperationContract]
        ulong SetAllSprav(byte[] input);

        [OperationContract]
        ulong SetChannelSprav(int channelId, byte[] input);

        #region Диагностика
        /// <summary>
        /// Возвращает диагностический объект
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        COTES.ISTOK.DiagnosticsInfo.Diagnostics GetDiagnosticsObject();

        [OperationContract]
        System.Data.DataSet GetDiagnosticInfo();
        #endregion

        /// <summary> Запросить у загруженных модулей список параметров канала с атрибутами
        /// </summary>
        /// <param name="channelId">Идентификатор канала</param>
        /// <returns>Возвращает список параметров</returns>
        [OperationContract]
        ParameterItem[] GetParamList(int channelId);

        /// <summary>
        /// Выбирает параметры из массива, которые есть в блочном
        /// </summary>
        /// <param name="parameters">Массив параметров</param>
        /// <returns>Массив параметров, которые есть в блочном</returns>
        [OperationContract]
        ParamValueItemWithID[] FilterParameters(ParamValueItemWithID[] parameters);

        /// <summary>
        /// Регистрация массива параметров
        /// </summary>
        /// <param name="parameters">Массив параметров</param>
        /// <returns>Номер транзакции</returns>
        [OperationContract]
        int RegisterClient(ParamValueItemWithID[] parameters);

        /// <summary>
        /// Разрегистрация массива параметров
        /// </summary>
        /// <param name="taID">Номер транзакции</param>
        [OperationContract]
        void UnRegisterClient(int taID);

        [OperationContract]
        ParamValueItemWithID[] GetValuesFromBank(int taID);

        String Host { get; }
        int Port { get; }

        [OperationContract]
        ulong BeginGetValues(BlockParameterValuesRequest[] valuesRequest, bool packed);

        //#region BeginGetValues()
        ///// <summary>
        ///// Запросить значениея параметра асинхронно
        ///// </summary>
        ///// <param name="parameterID">Идентификатор параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода времени</param>
        ///// <param name="endTime">Конечное время запрашивоемого периода времени</param>
        ///// <returns>Идентификатор асинхронной операции</returns>
        //[OperationContract]
        //ulong BeginGetValues1(int parameterID, DateTime beginTime, DateTime endTime);

        ///// <summary>
        ///// Запросить значениея параметра асинхронно
        ///// </summary>
        ///// <param name="parameterID">Идентификатор параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода времени</param>
        ///// <param name="endTime">Конечное время запрашивоемого периода времени</param>
        ///// <param name="interval">Запрашиваемый интервал агрегированных данных</param>
        ///// <param name="calcAggregation">Алгоритм агрегации данных</param>
        ///// <returns>Идентификатор асинхронной операции</returns>
        //[OperationContract]
        //ulong BeginGetValues2(int parameterID, DateTime beginTime, DateTime endTime, Interval interval, CalcAggregation calcAggregation);

        ///// <summary>
        ///// Запросить значениея нескольких параметров асинхронно
        ///// </summary>
        ///// <param name="parametersIDs">Идентификаторы запрашиваемых параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода времени</param>
        ///// <param name="endTime">Конечное время запрашивоемого периода времени</param>
        ///// <returns>Идентификатор асинхронной операции</returns>
        //[OperationContract]
        //ulong BeginGetValues3(int[] parametersIDs, DateTime beginTime, DateTime endTime);

        ///// <summary>
        ///// Запросить значениея нескольких параметров асинхронно
        ///// </summary>
        ///// <param name="parametersIDs">Идентификаторы запрашиваемых параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода времени</param>
        ///// <param name="endTime">Конечное время запрашивоемого периода времени</param>
        ///// <param name="interval">Запрашиваемый интервал агрегированных данных</param>
        ///// <param name="calcAggregation">Алгоритм агрегации данных</param>
        ///// <returns>Идентификатор асинхронной операции</returns>
        //[OperationContract]
        //ulong BeginGetValues4(int[] parametersIDs, DateTime beginTime, DateTime endTime, Interval interval, CalcAggregation calcAggregation); 
        //#endregion

        //#region BeginGetPackedValues()
        ///// <summary>
        ///// Запросить значениея параметра в зажатых пачках асинхронно
        ///// </summary>
        ///// <param name="parameterID">Идентификатор параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода времени</param>
        ///// <param name="endTime">Конечное время запрашивоемого периода времени</param>
        ///// <returns>Идентификатор асинхронной операции</returns>
        //[OperationContract]
        //ulong BeginGetPackedValues1(int parameterID, DateTime beginTime, DateTime endTime);

        ///// <summary>
        ///// Запросить значениея параметра в зажатых пачках асинхронно
        ///// </summary>
        ///// <param name="parameterID">Идентификатор параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода времени</param>
        ///// <param name="endTime">Конечное время запрашивоемого периода времени</param>
        ///// <param name="interval">Запрашиваемый интервал агрегированных данных</param>
        ///// <param name="calcAggregation">Алгоритм агрегации данных</param>
        ///// <returns>Идентификатор асинхронной операции</returns>
        //[OperationContract]
        //ulong BeginGetPackedValues2(int parameterID, DateTime beginTime, DateTime endTime, Interval interval, CalcAggregation calcAggregation);

        ///// <summary>
        ///// Запросить значениея нескольких параметров в зажатых пачках асинхронно
        ///// </summary>
        ///// <param name="parametersIDs">Идентификаторы запрашиваемых параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода времени</param>
        ///// <param name="endTime">Конечное время запрашивоемого периода времени</param>
        ///// <returns>Идентификатор асинхронной операции</returns>
        //[OperationContract]
        //ulong BeginGetPackedValues3(int[] parametersIDs, DateTime beginTime, DateTime endTime);

        ///// <summary>
        ///// Запросить значениея нескольких параметров в зажатых пачках асинхронно
        ///// </summary>
        ///// <param name="parametersIDs">Идентификаторы запрашиваемых параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода времени</param>
        ///// <param name="endTime">Конечное время запрашивоемого периода времени</param>
        ///// <param name="interval">Запрашиваемый интервал агрегированных данных</param>
        ///// <param name="calcAggregation">Алгоритм агрегации данных</param>
        ///// <returns>Идентификатор асинхронной операции</returns>
        //[OperationContract]
        //ulong BeginGetPackedValues4(int[] parametersIDs, DateTime beginTime, DateTime endTime, Interval interval, CalcAggregation calcAggregation); 
        //#endregion

        //#region BeginGetAppertureValues()
        ///// <summary>
        ///// Запросить проапертуренные значениея параметра асинхронно
        ///// </summary>
        ///// <param name="parameterID">Идентификатор параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода времени</param>
        ///// <param name="endTime">Конечное время запрашивоемого периода времени</param>
        ///// <param name="parameterInterval">Интервал значений параметра для апертуры</param>
        ///// <returns>Идентификатор асинхронной операции</returns>
        //[OperationContract]
        //ulong BeginGetAppertureValues1(int parameterID, DateTime beginTime, DateTime endTime, Interval parameterInterval);

        ///// <summary>
        ///// Запросить проапертуренные значениея параметра асинхронно
        ///// </summary>
        ///// <param name="parameterID">Идентификатор параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода времени</param>
        ///// <param name="endTime">Конечное время запрашивоемого периода времени</param>
        ///// <param name="parameterInterval">Интервал значений параметра для апертуры</param>
        ///// <param name="calcInterval">Запрашиваемый интервал агрегированных данных</param>
        ///// <param name="calcAggregation">Алгоритм агрегации данных</param>
        ///// <returns>Идентификатор асинхронной операции</returns>
        //[OperationContract]
        //ulong BeginGetAppertureValues2(int parameterID, DateTime beginTime, DateTime endTime,
        //    Interval parameterInterval, Interval calcInterval, CalcAggregation calcAggregation);

        ///// <summary>
        ///// Запросить проапертуренные значениея нескольких параметров асинхронно
        ///// </summary>
        ///// <param name="parametersIDs">Идентификаторы запрашиваемых параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода времени</param>
        ///// <param name="endTime">Конечное время запрашивоемого периода времени</param>
        ///// <param name="parameterInterval">Интервал значений параметра для апертуры</param>
        ///// <returns>Идентификатор асинхронной операции</returns>
        //[OperationContract]
        //ulong BeginGetAppertureValues3(int[] parametersIDs, DateTime beginTime, DateTime endTime, Interval parameterInterval);

        ///// <summary>
        ///// Запросить проапертуренные значениея нескольких параметров асинхронно
        ///// </summary>
        ///// <param name="parametersIDs">Идентификаторы запрашиваемых параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода времени</param>
        ///// <param name="endTime">Конечное время запрашивоемого периода времени</param>
        ///// <param name="parameterInterval">Интервал значений параметра для апертуры</param>
        ///// <param name="calcInterval">Запрашиваемый интервал агрегированных данных</param>
        ///// <param name="calcAggregation">Алгоритм агрегации данных</param>
        ///// <returns>Идентификатор асинхронной операции</returns>
        //[OperationContract]
        //ulong BeginGetAppertureValues4(int[] parametersIDs, DateTime beginTime, DateTime endTime,
        //    Interval parameterInterval, Interval calcInterval, CalcAggregation calcAggregation); 
        //#endregion

        //#region BeginGetPackedAppertureValues()
        ///// <summary>
        ///// Запросить проапертуренные значениея параметра в зажатых пачках асинхронно
        ///// </summary>
        ///// <param name="parameterID">Идентификатор параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода времени</param>
        ///// <param name="endTime">Конечное время запрашивоемого периода времени</param>
        ///// <param name="parameterInterval">Интервал значений параметра для апертуры</param>
        ///// <returns>Идентификатор асинхронной операции</returns>
        //[OperationContract]
        //ulong BeginGetPackedAppertureValues1(int parameterID, DateTime beginTime, DateTime endTime, Interval parameterInterval);

        ///// <summary>
        ///// Запросить проапертуренные значениея параметра в зажатых пачках асинхронно
        ///// </summary>
        ///// <param name="parameterID">Идентификатор параметра</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода времени</param>
        ///// <param name="endTime">Конечное время запрашивоемого периода времени</param>
        ///// <param name="parameterInterval">Интервал значений параметра для апертуры</param>
        ///// <param name="calcInterval">Запрашиваемый интервал агрегированных данных</param>
        ///// <param name="calcAggregation">Алгоритм агрегации данных</param>
        ///// <returns>Идентификатор асинхронной операции</returns>
        //[OperationContract]
        //ulong BeginGetPackedAppertureValues2(int parameterID, DateTime beginTime, DateTime endTime, 
        //    Interval parameterInterval, Interval calcInterval, CalcAggregation calcAggregation);

        ///// <summary>
        ///// Запросить проапертуренные значениея нескольких параметров в зажатых пачках асинхронно
        ///// </summary>
        ///// <param name="parametersIDs">Идентификаторы запрашиваемых параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода времени</param>
        ///// <param name="endTime">Конечное время запрашивоемого периода времени</param>
        ///// <param name="parameterInterval">Интервал значений параметра для апертуры</param>
        ///// <returns>Идентификатор асинхронной операции</returns>
        //[OperationContract]
        //ulong BeginGetPackedAppertureValues3(int[] parametersIDs, DateTime beginTime, DateTime endTime, Interval parameterInterval);

        ///// <summary>
        ///// Запросить проапертуренные значениея нескольких параметров в зажатых пачках асинхронно
        ///// </summary>
        ///// <param name="parametersIDs">Идентификаторы запрашиваемых параметров</param>
        ///// <param name="beginTime">Начальное время запрашиваемого периода времени</param>
        ///// <param name="endTime">Конечное время запрашивоемого периода времени</param>
        ///// <param name="parameterInterval">Интервал значений параметра для апертуры</param>
        ///// <param name="calcInterval">Запрашиваемый интервал агрегированных данных</param>
        ///// <param name="calcAggregation">Алгоритм агрегации данных</param>
        ///// <returns>Идентификатор асинхронной операции</returns>
        //[OperationContract]
        //ulong BeginGetPackedAppertureValues4(int[] parametersIDs, DateTime beginTime, DateTime endTime,
        //    Interval parameterInterval, Interval calcInterval, CalcAggregation calcAggregation); 
        //#endregion

        #region Async Operations
        /// <summary>
        /// Получить состояние текущей выполняемой асинхронной операции
        /// </summary>
        /// <param name="operationID">Идентификатор операции</param>
        /// <returns>Состояние операции</returns>
        [OperationContract]
        OperationInfo GetAsyncOperationState(ulong operationID);

        /// <summary>
        /// Получить порцию возвращаемых результатов асинхронной операции
        /// </summary>
        /// <param name="operationID">Идентификатор операции</param>
        /// <param name="next">true, если запрашивается следующая пачка данных, false - если требуется повторить предыдущий запрос данных</param>
        /// <returns>Полученная порция данных</returns>
        [OperationContract]
        UAsyncResult GetAsyncOperationData(ulong operationID, bool next);

        /// <summary>
        /// Получить порцию сообщений асинхронной операции
        /// </summary>
        /// <param name="operationID">Идентификатор операции</param>
        /// <param name="next">true, если запрашивается следующая пачка сообщений, false - если требуется повторить предыдущий запрос сообщений</param>
        /// <returns>Полученная пачка сообщений</returns>
        [OperationContract]
        Message[] GetAsyncOperationMessages(ulong operationID, bool next);

        /// <summary>
        /// Завершить и удалить информацию о асинхронной операции 
        /// </summary>
        /// <param name="operationID">Идентификатор операции</param>
        [OperationContract]
        void EndAsyncOperation(ulong operationID);

        /// <summary>
        /// Прервать выполнение асинхронной операции
        /// </summary>
        /// <param name="operationID">Идентификатор операции</param>
        [OperationContract]
        void InteruptAsyncOperation(ulong operationID);
        #endregion

        [OperationContract]
        ModuleInfo[] GetModulesInfo();

        [OperationContract]
        void DeleteValues(int idnum, DateTime timeFrom);
    }
}
