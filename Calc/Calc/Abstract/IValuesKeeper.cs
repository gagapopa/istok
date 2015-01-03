using System;
using System.Collections.Generic;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Хранилище значений параметров. 
    /// Содержит сагрегированные значения для всех параметров, встреченных в расчёте, 
    /// а также исходные значения (в том числе только что расчитанные) для расчитываемых параметров.
    /// </summary>
    public interface IValuesKeeper
    {
        /// <summary>
        /// Добавить в хранилище расчитанное значение
        /// </summary>
        /// <param name="calcNode">Параметр</param>
        /// <param name="args">Аргументы для условного параметра. Для обычных параметров null</param>
        /// <param name="time">Время расчитанного значения</param>
        /// <param name="symbolValue">Значение</param>
        void AddCalculatedValue(
            ICalcNode calcNode, 
            ArgumentsValues args, 
            DateTime time, 
            SymbolValue symbolValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeInfo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        void SetFailNode(ICalcNodeInfo nodeInfo, DateTime startTime, DateTime endTime);

        /// <summary>
        /// Получить все расчитанные значения, сформированные в пачки для дальнейшего сохранения.
        /// </summary>
        /// <returns>Пачки для дальнейшего сохранения, разбитые по параметрам/аргументам/ревизиям</returns>
        Package[] GetAllCalculatedValues();

        /// <summary>
        /// Проверить был ли данный параметр расчитан в текущем расчёте
        /// </summary>
        /// <param name="calcNode">Узел расчёта</param>
        /// <param name="arguments">Значения аргументов условного параметра или оптимизации</param>
        /// <param name="time">Время расчёта</param>
        /// <returns>
        /// Если параметр уже был расчитан за указанное время 
        /// с указанными значениями аргументов, вернуть true.
        /// <br />
        /// В противном случае возвращает false, 
        /// даже если в системе есть значение для данного параметра
        /// полученное из БД
        /// </returns>
        bool IsCalculated(
            ICalcNode calcNode,
            ArgumentsValues arguments,
            DateTime time);

        /// <summary>
        /// Добавить исходное значения параметра
        /// </summary>
        /// <param name="calcNode">Параметр</param>
        /// <param name="args">Аргументы для условного параметра. Для обычных параметров null</param>
        /// <param name="time">Время значения</param>
        /// <param name="symbolValue">Значение</param>
        void AddRawValue(
            ICalcNode calcNode,
            ArgumentsValues args,
            DateTime time,
            SymbolValue symbolValue);

        /// <summary>
        /// Получить исходное значение параметра
        /// </summary>
        /// <param name="calcNode">Параметр</param>
        /// <param name="arguments">Аргументы для условного параметра. Для обычных параметров null</param>
        /// <param name="time">Время значения</param>
        /// <returns>Значение</returns>
        SymbolValue GetRawValue(
            ICalcNode calcNode, 
            ArgumentsValues arguments,
            DateTime time);

        /// <summary>
        /// Добавить сагрегированного значение
        /// </summary>
        /// <param name="aggregation">Алгоритм агрегации</param>
        /// <param name="time">Время значения</param>
        /// <param name="period">Интервал агрегации</param>
        /// <param name="value">Сагрегированное значение</param>
        /// <param name="aggregationNodes">Агрегируемые параметры</param>
        void AddValue(
            CalcAggregation aggregation, 
            DateTime time,
            Interval period,
            SymbolValue value,
            params CalcNodeKey[] aggregationNodes);

        /// <summary>
        /// Получить сагрегированное значение 
        /// </summary>
        /// <param name="aggregation">Алгоритм агрегации</param>
        /// <param name="time">Время значения</param>
        /// <param name="period">Интервал агрегации</param>
        /// <param name="aggregationNodes">Агрегируемые параметры</param>
        /// <returns>
        /// null - если не хватает иходных данных;
        /// <br />SymbolValue.Nothing - агрегация не может быть посчитанна;
        /// <br />Сагрегированное значение.
        /// </returns>
        SymbolValue GetValue(
            CalcAggregation aggregation, 
            DateTime time, 
            Interval period,
            params CalcNodeKey[] aggregationNodes);

        /// <summary>
        /// Получить оптимальный набор аргументов для оптимизации
        /// </summary>
        /// <param name="calcNode">Узел оптимизации</param>
        /// <param name="args">Аргументы для условного параметра. Для обычных параметров null</param>
        /// <param name="time"></param>
        /// <returns>Оптимальные аргументы</returns>
        ArgumentsValues GetOptimal(
            ICalcNode calcNode,
            ArgumentsValues args,
            DateTime time);

        /// <summary>
        /// Установиь оптимальный набор аргументов для оптимизации
        /// </summary>
        /// <param name="calcNode">Узел оптимизации</param>
        /// <param name="args">Аргументы для условного параметра. Для обычных параметров null</param>
        /// <param name="time"></param>
        /// <param name="optimalArgs">Оптимальные аргументы</param>
        void SetOptimal(
            ICalcNode calcNode, 
            ArgumentsValues args, 
            DateTime time, 
            ArgumentsValues optimalArgs);

        /// <summary>
        /// Удалить все агрегированны значения для указанного параметра.
        /// </summary>
        /// <remarks>
        /// Все агрегированные значения параметра с указанными значениями аргументов,
        /// чьи периоды агрегации включают указанное время, будут удалены из кэша значений
        /// </remarks>
        /// <param name="calcNode">Узел расчёта</param>
        /// <param name="args">Значения аргументов условного параметра или оптимизации</param>
        /// <param name="time">Время параметра</param>
        void ClearAggregation(ICalcNode calcNode, ArgumentsValues args, DateTime time);
    }
}
