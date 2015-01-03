using System;
using System.Collections.Generic;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Контекст расчёта
    /// </summary>
    public interface ICalcContext : IDisposable
    {
        OperationState OperationState { get; }

        /// <summary>
        /// Таблица символов
        /// </summary>
        SymbolTable SymbolTable { get; }

        /// <summary>
        /// Хранилище расчитанных и закэшированных значений
        /// </summary>
        IValuesKeeper ValuesKeeper { get; }

        /// <summary>
        /// Начальное время всего расчёта
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// Конечное время всего расчёта
        /// </summary>
        DateTime EndTime { get; }

        /// <summary>
        /// Установить время расчёта
        /// </summary>
        /// <param name="startTime">Начальное время</param>
        /// <param name="endTime">Конечное время</param>
        void SetTime(DateTime startTime, DateTime endTime);

        /// <summary>
        /// Максимальное количество проходов цикла для борьбы с бесконечными циклами
        /// </summary>
        int MaxLoopCount { get; set; }

        /// <summary>
        /// Перерасчитывать всё
        /// </summary>
        bool RecalcAll { get; set; }

        /// <summary>
        /// Добавить сообщения
        /// </summary>
        /// <param name="messages"></param>
        void AddMessage(IEnumerable<Message> messages);

        /// <summary>
        /// Добавить сообщение
        /// </summary>
        /// <param name="messageAB"></param>
        void AddMessage(Message messageAB);
        
        /// <summary>
        /// Добавить параметры в список расчитываемых
        /// </summary>
        /// <param name="calcNode"></param>
        void AddParametersToCalc(IEnumerable<ICalcNode> calcNode);

        /// <summary>
        /// Проверить является ли указанный параметр в списке для расчёта. В том числе и уже расчитанные параметры
        /// </summary>
        /// <param name="calcNode">Проверяемый параметр</param>
        /// <returns></returns>
        bool IsParameterToCalc(ICalcNode calcNode);

        /// <summary>
        /// Перейти к расчёту следующего запланированного узла
        /// </summary>
        /// <returns></returns>
        bool NextNode();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        CalcPosition GetIdentifier();
        
        /// <summary>
        /// Получить значения аргументов оптимизации, введенные вручную
        /// </summary>
        /// <param name="optimizationInfo"></param>
        /// <param name="arguments"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        ArgumentsValues[] GetManualArgValues(IOptimizationInfo optimizationInfo, ArgumentsValues arguments, DateTime startTime);

        /// <summary>
        /// Получить ревизию по времени
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        RevisionInfo GetRevision(DateTime time);

        /// <summary>
        /// Запустить расчёт параметров с указанными значениями аргументов, за указанное время
        /// </summary>
        /// <param name="startTime">Начальное время</param>
        /// <param name="endTime">Конечное время</param>
        /// <param name="calcNodes">Искомые параметры</param>
        /// <returns>Возвращает false, при ошибке</returns>
        bool Call(DateTime startTime, DateTime endTime, params CalcNodeKey[] calcNodes);

        /// <summary>
        /// Запустить расчёт контекста выполнения
        /// </summary>
        /// <param name="callContext"></param>
        /// <returns>Возвращает false, при ошибке</returns>
        bool Call(ICallContext callContext);

        /// <summary>
        /// Выйти из расчёта текущего контекста выполнения
        /// </summary>
        void Return();

        /// <summary>
        /// Получить состояние параметра
        /// </summary>
        /// <param name="nodeInfo">Информация о параметре</param>
        /// <returns></returns>
        ICalcState GetState(ICalcNode calcNode, RevisionInfo revision); //ICalcNodeInfo nodeInfo);

        /// <summary>
        /// Получить информацию о параметре расчёта по коду
        /// </summary>
        /// <param name="revision">Ревизия</param>
        /// <param name="parameterCode">Код параметра</param>
        /// <returns></returns>
        IParameterInfo GetParameterNode(RevisionInfo revision, string parameterCode);

        /// <summary>
        /// Проверить требуется ли расчитать параметр 
        /// не смотра на то есть или нет у него значения
        /// </summary>
        /// <param name="calcNodeInfo">Расчитываемый парамтр</param>
        /// <returns></returns>
        bool ForceCalc(ICalcNodeInfo calcNodeInfo);

        String ContextStateReport();
    }
}
