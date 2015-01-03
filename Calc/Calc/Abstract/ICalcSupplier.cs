using System;
using System.Collections.Generic;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Связка сервера расчета и вызывающей системы
    /// </summary>
    public interface ICalcSupplier
    {
        RevisionInfo GetRevision(DateTime time);

        IEnumerable<RevisionInfo> GetRevisions(DateTime startTime, DateTime endTime);

        /// <summary>
        /// Получить параметр по коду
        /// </summary>
        /// <param name="parameterCode">Код параметра</param>
        /// <returns>Параметр</returns>
        IParameterInfo GetParameterNode(ICalcContext context, RevisionInfo revision, String parameterCode);

        ICalcNode GetParameterNode(ICalcContext context, int nodeID);

        /// <summary>
        /// Создать пустой параметр с указанным кодом.
        /// Применяется при поиске зависимостей, когда параметр по коду не найден.
        /// </summary>
        /// <param name="parameterCode">Код параметра</param>
        /// <returns></returns>
        IParameterInfo GetEmptyParameterNode(RevisionInfo revision, String parameterCode);

        /// <summary>
        /// Получить все параметры
        /// </summary>
        /// <returns></returns>
        IEnumerable<ICalcNode> GetParameterNodes(ICalcContext context);

        /// <summary>
        /// Запросить значение параметра
        /// </summary>
        /// <param name="node">Узел параметра</param>
        /// <param name="agreg">Запрашиваемая агрегация</param>
        /// <param name="startTime">Начальное время запроса</param>
        /// <param name="interval">Запрашиваемый интервал</param>
        /// <param name="serverNotAccessible">Выставляется в true, если сервер сбора не доступен</param>
        /// <returns>Запрашиваемое значение</returns>
        ParamValueItem GetParameterNodeValue(ICalcContext context,
                                             IEnumerable<Tuple<ICalcNode, ArgumentsValues>> nodeArgs,
                                             CalcAggregation agreg,
                                             DateTime startTime,
                                             Interval interval,
                                             out List<Message> messages,
                                             out bool serverNotAccessible);

        /// <summary>
        /// Запросить исходные значения параметра
        /// </summary>
        /// <param name="parameterInfo">Узел параметра</param>
        /// <param name="args">Аргументы условного параметра, null для обычного</param>
        /// <param name="startTime">Начальное время запроса</param>
        /// <param name="endTime">Конечное время запроса</param>
        /// <param name="messages">Сообщения полученные при запросе значений</param>
        /// <param name="serverNotAccessible">true, если сервер, хранящий запрашиваемые значения, недоступен</param>
        /// <returns></returns>
        List<ParamValueItem> GetParameterNodeRawValues(ICalcContext context,
                                                        ICalcNode parameterInfo,
                                                        ArgumentsValues args, 
                                                        DateTime startTime, 
                                                        DateTime endTime,
                                                        out List<Message> messages,
                                                        out bool serverNotAccessible);

        /// <summary>
        /// Сохранить посчитанные значения
        /// </summary>
        /// <param name="savingValues">Набор пачек значений для сохранения</param>
        void SaveParameterNodeValue(ICalcContext context, Package[] savingValues, out List<Message> messages);

        /// <summary>
        /// Получить список констант
        /// </summary>
        /// <returns></returns>
        IEnumerable<ConstsInfo> GetConsts();

        /// <summary>
        /// Получить внешнии функции
        /// </summary>
        /// <returns></returns>
        IEnumerable<IExternalFunctionInfo> GetExternalFunctions(ICalcContext context);

        /// <summary>
        /// Получить глобальные пользовательские функции
        /// </summary>
        /// <returns></returns>
        IEnumerable<CustomFunctionInfo> GetCustomFunction();

        /// <summary>
        /// Получить время последнего значения параметра, хранящиеся в БД
        /// </summary>
        /// <param name="paramInfo">Запрашиваемый параметр</param>
        /// <returns>Последние время значения</returns>
        DateTime GetLastTimeValue(ICalcContext context, IParameterInfo paramInfo);

        /// <summary>
        /// Запросить аргументы оптимизации, введенные вручную.
        /// </summary>
        /// <param name="optimizationInfo">Оптимизация</param>
        /// <param name="baseArgs">Аргументы базовой оптимизации</param>
        /// <param name="startTime">Искомое время</param>
        /// <returns></returns>
        ArgumentsValues[] GetManualArgValue(ICalcContext context, IOptimizationInfo optimizationInfo, ArgumentsValues baseArgs, DateTime startTime);
    }
}
