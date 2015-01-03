using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace COTES.ISTOK
{
    public class ValueAggregator
    {
        public void GetSourceRange(Interval sourceInterval, Interval destInterval, ref DateTime beginTime, ref DateTime endTime)
        {
            // выравниваем интервалы
            beginTime = destInterval.NearestEarlierTime(beginTime);
            endTime = destInterval.NearestEarlierTime(endTime);

            if (destInterval > sourceInterval)
            {
                if (destInterval <= Interval.Hour)
                {
                    endTime = destInterval.GetPrevTime(endTime);
                    beginTime = destInterval.GetPrevTime(beginTime);
                }
                if (sourceInterval <= Interval.Hour)
                {
                    beginTime = beginTime.AddMilliseconds(1);
                    endTime = endTime.AddMilliseconds(1);
                }
            }
        }

        public DateTime GetRange(DateTime beginTime, Interval sourceInterval, Interval destInterval)
        {
            if (sourceInterval <= Interval.Hour)
            {
                beginTime = beginTime.AddMilliseconds(-1);
            }
            beginTime = destInterval.NearestEarlierTime(beginTime);
            if (destInterval <= Interval.Hour)
                beginTime = destInterval.GetNextTime(beginTime);

            return beginTime;
        }

        /// <summary>
        /// Расчитать агрегацию
        /// </summary>
        /// <param name="aggregation">Алгоритм агрегации</param>
        /// <param name="sourceInterval">Дискретность исходных данных</param>
        /// <param name="aggregationInterval">Дискретность расчитываемых данных</param>
        /// <param name="values">Исходные данные</param>
        /// <returns>Агрегированные данные</returns>
        /// <exception cref="ArgumentNullException">Если values раввен null.</exception>
        /// <exception cref="ArgumentException">
        /// Если количество исходных данные не соответствует 
        /// требуемому количеству параметров для заданного алгоритма агрегации
        /// </exception>
        public IEnumerable<ParamValueItem> Aggregate(CalcAggregation aggregation,
                                                     Interval sourceInterval,
                                                     Interval aggregationInterval,
                                                     params IEnumerable<ParamValueItem>[] values)
        {
            // количество параметров для расчёта агрегации
            int paramsCount = GetAggregationParameterCount(aggregation);

            // проверка аргументов
            if (values == null)
                throw new ArgumentNullException();
            if (values.Length != paramsCount)
                throw new ArgumentException(String.Format("Неверное количество исходных данных для агрегации. Алгоритм: {0}, ожидается параметров: {1}", aggregation, paramsCount));
            if (aggregationInterval == Interval.Zero && aggregation != CalcAggregation.Nothing)
                throw new ArgumentException("Не задан интервал агрегации");

            if (aggregation == CalcAggregation.Nothing)
            {
                return values[0];
            }

            return AggregateMethod(aggregation, sourceInterval, aggregationInterval, paramsCount, values);
        }

        private IEnumerable<ParamValueItem> AggregateMethod(CalcAggregation aggregation,
                                                            Interval sourceInterval,
                                                            Interval aggregationInterval,
                                                            int paramsCount,
                                                            params IEnumerable<ParamValueItem>[] values)
        {
            // Получить время первого значения
            DateTime beginTime = (values.Select(e => (e.FirstOrDefault() ?? new ParamValueItem() {
				Time = DateTime.MaxValue
			}).Time)).Min();

            // выход, если нет исходных значений
            if (beginTime == DateTime.MaxValue)
            {
                yield break;
            }

            beginTime = GetRange(beginTime, sourceInterval, aggregationInterval);

            // Получить итераторы для исходных значений
            IEnumerator<ParamValueItem>[] iterator = (values.Select(e => e.GetEnumerator())).ToArray();

            for (int i = 0; i < paramsCount; i++)
            {
                if (!iterator[i].MoveNext())
                    iterator[i] = null;
            }

            // Время текущего агрегируемого значения
            DateTime valueTime = beginTime;

            // Хранилище для подготовке исходных данных для агрегации
            Dictionary<DateTime, ParamValueItem[]> sourceValuesDictionary = new Dictionary<DateTime, ParamValueItem[]>();
            ParamValueItem[] sourceValueArray;

            // Последние значения, полученные из исходных данных,
            // для заполнения пропусков
            ParamValueItem[] lastValue = new ParamValueItem[paramsCount];

            // Последнее значение перед расчитываемым интервалом,
            // используется для расчёта значений из сырых значений при пропуске целого интервала
            ParamValueItem[] firstValue = new ParamValueItem[paramsCount];

            while (iterator.Any(it => it != null))
            {
                sourceValuesDictionary.Clear();

                // Получить границы исходных данных для расчёта текущих значений
                DateTime valueBeginTime = valueTime;
                DateTime valueEndTime = aggregationInterval.GetNextTime(valueTime);

                GetSourceRange(sourceInterval, aggregationInterval, ref valueBeginTime, ref valueEndTime);

                // Подготовить исходные данные, заполняя данные по sourceInterval
                for (int i = 0; i < paramsCount; i++)
                {
                    DateTime currentTime = sourceInterval.NearestEarlierTime(valueBeginTime);
                    if (currentTime < valueBeginTime)
                    {
                        currentTime = sourceInterval.GetNextTime(currentTime);
                    }

                    firstValue[i] = lastValue[i];

                    // пропустить значения до начала интервала
                    while (iterator[i] != null
                           && iterator[i].Current.Time < valueBeginTime)
                    {
                        lastValue[i] = iterator[i].Current;
                        if (!iterator[i].MoveNext())
                            iterator[i] = null;
                    }

                    while (currentTime < valueEndTime)
                    {
                        DateTime paramValueTime = iterator[i] == null ? valueEndTime : iterator[i].Current.Time;

                        // заполнить пропуск в исходных данных
                        while (lastValue[i] != null
                               && sourceInterval > Interval.Zero
                               && currentTime < paramValueTime
                               && currentTime < valueEndTime)
                        {
                            ParamValueItem paramValue = new ParamValueItem(currentTime, lastValue[i].Quality, lastValue[i].Value);

                            if (!sourceValuesDictionary.TryGetValue(paramValue.Time, out sourceValueArray))
                            {
                                sourceValuesDictionary[paramValue.Time] = sourceValueArray = new ParamValueItem[paramsCount];
                            }
                            sourceValueArray[i] = paramValue;

                            currentTime = sourceInterval.GetNextTime(currentTime);
                        }

                        // добавить значение из исходных данных
                        if (paramValueTime < valueEndTime)
                        {
                            ParamValueItem paramValue = iterator[i].Current;

                            if (!sourceValuesDictionary.TryGetValue(paramValue.Time, out sourceValueArray))
                            {
                                sourceValuesDictionary[paramValue.Time] = sourceValueArray = new ParamValueItem[paramsCount];
                            }
                            sourceValueArray[i] = lastValue[i] = paramValue;

                            currentTime = sourceInterval.GetNextTime(paramValue.Time);

                            if (!iterator[i].MoveNext())
                                iterator[i] = null;
                        }
                        else
                        {
                            // выход из цикла при агрегации из сырых данных
                            currentTime = valueEndTime;
                        }
                    }
                }

                // Для агрегации из сырых данных, при пропуске целового интервала,
                // использовать предыдущие значение
                if (sourceValuesDictionary.Count == 0)
                {
                    sourceValuesDictionary[valueBeginTime] = sourceValueArray = new ParamValueItem[paramsCount];
                    for (int i = 0; i < paramsCount; i++)
                    {
                        sourceValueArray[i] = new ParamValueItem(firstValue[i])
                        {
                            Time = valueBeginTime
                        };
                    }
                }

                // Заполнить пропуски для исходных данных (многопараметровой агрегации) и удалить бэды
                var sourceTimes = (from t in sourceValuesDictionary.Keys orderby t select t).ToArray();
                List<DateTime> toRemoveTimes = new List<DateTime>();

                for (int j = 0; j < sourceTimes.Length; j++)
                {
                    bool hasBad = false;
                    DateTime currentTime = sourceTimes[j];

                    for (int i = 0; i < paramsCount; i++)
                    {
                        if (sourceValuesDictionary[currentTime][i] == null)
                        {
                            if (j == 0)
                            {
                                sourceValuesDictionary[currentTime][i] = new ParamValueItem(firstValue[i])
                                {
                                    Time = currentTime
                                };
                            }
                            else
                            {
                                sourceValuesDictionary[currentTime][i] = new ParamValueItem(sourceValuesDictionary[sourceTimes[j - 1]][i])
                                {
                                    Time = currentTime
                                };
                            }
                        }
                        if (sourceValuesDictionary[currentTime][i].Quality == Quality.Bad)
                        {
                            hasBad = true;
                        }
                    }

                    if (hasBad)
                    {
                        toRemoveTimes.Add(currentTime);
                    }
                }

                foreach (var item in toRemoveTimes)
                {
                    sourceValuesDictionary.Remove(item);
                }

                // готовые данные для агрегации одного значения
                var sourceValues = (from t in sourceValuesDictionary.Keys orderby t select sourceValuesDictionary[t]).ToArray();

                // данных достаточно для расчёта агрегации
                bool exists = IsExist(sourceInterval, valueBeginTime, valueEndTime, sourceValues);

                // агрегировать значение
                ParamValueItem valueItem = AggregateValue(aggregation, valueTime, exists, sourceValues);

                if (valueItem != null)
                {
                    yield return valueItem;
                }

                valueTime = aggregationInterval.GetNextTime(valueTime);
            }
        }

        private ParamValueItem AggregateValue(CalcAggregation aggregation, DateTime valueTime, bool exists, ParamValueItem[][] sourceValues)
        {
            ParamValueItem valueItem = null;

            if (!exists)
            {
                valueItem = new ParamValueItem(valueTime, Quality.Bad, double.NaN);
            }

            switch (aggregation)
            {
                case CalcAggregation.Count:
                    valueItem = new ParamValueItem(valueTime, Quality.Good, sourceValues.Length);
                    break;
                case CalcAggregation.Exist:
                    valueItem = new ParamValueItem(valueTime, Quality.Good, exists ? 1 : 0);
                    break;
                case CalcAggregation.First:
                    if (exists)
                    {
                        valueItem = FirstAggregation(valueTime, sourceValues);
                    }
                    break;
                case CalcAggregation.Last:
                    if (exists)
                    {
                        valueItem = LastAggregation(valueTime, sourceValues);
                    }
                    break;
                case CalcAggregation.Sum:
                    if (exists)
                    {
                        valueItem = SumAggregation(valueTime, sourceValues);
                    }
                    break;
                case CalcAggregation.Average:
                    if (exists)
                    {
                        valueItem = AverageAggregation(valueTime, sourceValues);
                    }
                    break;
                case CalcAggregation.Minimum:
                    if (exists)
                    {
                        valueItem = MinimumAggregation(valueTime, sourceValues);
                    }
                    break;
                case CalcAggregation.Maximum:
                    if (exists)
                    {
                        valueItem = MaximumAggregation(valueTime, sourceValues);
                    }
                    break;
                case CalcAggregation.Weighted:
                    if (exists)
                    {
                        valueItem = WeightedAggregation(valueTime, sourceValues);
                    }
                    break;
            }
            return valueItem;
        }

        private bool IsExist(Interval sourceInterval, DateTime valueBeginTime, DateTime valueEndTime, ParamValueItem[][] sourceValues)
        {
            const float dayRequiredPercent = 1f;
            const float defaultRequiredPercent = 0.7f;

            float requiredCount;

            if (sourceInterval >= Interval.Day)
            {
                requiredCount = dayRequiredPercent;
            }
            else
            {
                requiredCount = defaultRequiredPercent;
            }

            float minValuesCountF = sourceInterval.GetQueryValues(valueBeginTime, valueEndTime) * requiredCount;

            int minValuesCount = (int)minValuesCountF;

            if (minValuesCount == 0)
                ++minValuesCount;

            return sourceValues.Length >= minValuesCount;
        }

        private int GetAggregationParameterCount(CalcAggregation aggregation)
        {
            int paramsCount = 1;

            if (aggregation == CalcAggregation.Weighted)
                paramsCount = 2;
            else
                paramsCount = 1;
            return paramsCount;
        }

        /// <summary>
        /// Расчитать агрегированное значение из исходных данных для алгоритма &laquo;первое&raquo;
        /// </summary>
        /// <param name="valueTime">Время для результирующего значения</param>
        /// <param name="values">Исходные данные</param>
        /// <returns>
        /// Сагрегированное значение.
        /// <br />null, если исходные данные не удовлетворяют каким ни будь требованиям
        /// </returns>
        private ParamValueItem FirstAggregation(DateTime valueTime, ParamValueItem[][] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i][0].Quality > 0 && !double.IsNaN(values[i][0].Value))
                {
                    return new ParamValueItem(valueTime, Quality.Good, values[i][0].Value);
                }
            }
            return null;
        }

        /// <summary>
        /// Расчитать агрегированное значение из исходных данных для алгоритма &laquo;последние&raquo;
        /// </summary>
        /// <param name="valueTime">Время для результирующего значения</param>
        /// <param name="values">Исходные данные</param>
        /// <returns>
        /// Сагрегированное значение.
        /// <br />null, если исходные данные не удовлетворяют каким ни будь требованиям
        /// </returns>
        private ParamValueItem LastAggregation(DateTime valueTime, ParamValueItem[][] values)
        {
            for (int i = values.Length - 1; i >= 0; i--)
            {
                if (values[i][0].Quality > 0 && !double.IsNaN(values[i][0].Value))
                {
                    return new ParamValueItem(valueTime, Quality.Good, values[i][0].Value);
                }
            }
            return null;
        }

        /// <summary>
        /// Расчитать агрегированное значение из исходных данных для алгоритма &laquo;максимальное&raquo;
        /// </summary>
        /// <param name="valueTime">Время для результирующего значения</param>
        /// <param name="values">Исходные данные</param>
        /// <returns>
        /// Сагрегированное значение.
        /// <br />null, если исходные данные не удовлетворяют каким ни будь требованиям
        /// </returns>
        private ParamValueItem MaximumAggregation(DateTime valueTime, ParamValueItem[][] values)
        {
            double maxValue = double.NaN;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i][0].Quality > 0
                    && (double.IsNaN(maxValue) || maxValue < values[i][0].Value))
                    maxValue = values[i][0].Value;
            }

            if (!double.IsNaN(maxValue))
                return new ParamValueItem(valueTime, Quality.Good, maxValue);

            return null;
        }

        /// <summary>
        /// Расчитать агрегированное значение из исходных данных для алгоритма &laquo;минимальное&raquo;
        /// </summary>
        /// <param name="valueTime">Время для результирующего значения</param>
        /// <param name="values">Исходные данные</param>
        /// <returns>
        /// Сагрегированное значение.
        /// <br />null, если исходные данные не удовлетворяют каким ни будь требованиям
        /// </returns>
        private ParamValueItem MinimumAggregation(DateTime valueTime, ParamValueItem[][] values)
        {
            double minValue = double.NaN;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i][0].Quality > 0
                    && (double.IsNaN(minValue) || minValue > values[i][0].Value))
                    minValue = values[i][0].Value;
            }

            if (!double.IsNaN(minValue))
                return new ParamValueItem(valueTime, Quality.Good, minValue);

            return null;
        }

        /// <summary>
        /// Расчитать агрегированное значение из исходных данных для алгоритма &laquo;среднее&raquo;
        /// </summary>
        /// <param name="valueTime">Время для результирующего значения</param>
        /// <param name="values">Исходные данные</param>
        /// <returns>
        /// Сагрегированное значение.
        /// <br />null, если исходные данные не удовлетворяют каким ни будь требованиям
        /// </returns>
        private ParamValueItem AverageAggregation(DateTime valueTime, ParamValueItem[][] values)
        {
            double numerator = 0;
            int valuesCount = 0;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i][0].Quality > 0
                    && !double.IsNaN(values[i][0].Value))
                {
                    numerator += values[i][0].Value;
                    ++valuesCount;
                }
            }

            if (valuesCount > 0)
            {
                return new ParamValueItem(valueTime, Quality.Good, numerator / valuesCount);
            }
            return null;
        }

        /// <summary>
        /// Расчитать агрегированное значение из исходных данных для алгоритма &laquo;сумма&raquo;
        /// </summary>
        /// <param name="valueTime">Время для результирующего значения</param>
        /// <param name="values">Исходные данные</param>
        /// <returns>
        /// Сагрегированное значение.
        /// <br />null, если исходные данные не удовлетворяют каким ни будь требованиям
        /// </returns>
        private ParamValueItem SumAggregation(DateTime valueTime, ParamValueItem[][] values)
        {
            double val = 0.0;
            Quality qvl = Quality.Bad;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i][0].Quality > 0 && !double.IsNaN(values[i][0].Value))
                {
                    val += values[i][0].Value;
                    qvl = Quality.Good;
                }
            }

            if (qvl != Quality.Bad)
            {
                return new ParamValueItem(valueTime, qvl, val);
            }

            return null;
        }

        /// <summary>
        /// Расчитать агрегированное значение из исходных данных для алгоритма &laquo;взвешенное значение&raquo;
        /// </summary>
        /// <param name="valueTime">Время для результирующего значения</param>
        /// <param name="parameterValues"></param>
        /// <param name="weightValues"></param>
        /// <returns>
        /// Сагрегированное значение.
        /// <br />null, если исходные данные не удовлетворяют каким ни будь требованиям
        /// </returns>
        private ParamValueItem WeightedAggregation(DateTime valueTime, ParamValueItem[][] values)
        {
            double numerator = 0;
            double denominator = 0;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i][0].Quality > 0
                    && values[i][1].Quality > 0
                    && !double.IsNaN(values[i][0].Value)
                    && !double.IsNaN(values[i][1].Value))
                {
                    numerator += values[i][0].Value * values[i][1].Value;
                    denominator += values[i][1].Value;
                }
            }

            if (denominator != 0)
            {
                return new ParamValueItem(valueTime, Quality.Good, numerator / denominator);
            }
            return new ParamValueItem(valueTime, Quality.Bad, double.NaN);
        }

        /// <summary>
        /// Расчитать агрегированное значение из исходных данных для алгоритма &laquo;количества&raquo;
        /// </summary>
        /// <param name="valueTime">Время для результирующего значения</param>
        /// <param name="values">Исходные данные</param>
        /// <returns>
        /// Сагрегированное значение.
        /// <br />null, если исходные данные не удовлетворяют каким ни будь требованиям
        /// </returns>
        private ParamValueItem CountAggregation(DateTime valueTime, ParamValueItem[] values)
        {
            int count = 0;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Quality > 0
                       && !double.IsNaN(values[i].Value))
                    ++count;
            }

            return new ParamValueItem(valueTime, Quality.Good, count);
        }

        /// <summary>
        /// Расчитать агрегированное значение из исходных данных для алгоритма &laquo;существует&raquo;
        /// </summary>
        /// <param name="valueTime">Время для результирующего значения</param>
        /// <param name="values">Исходные данные</param>
        /// <returns>
        /// Сагрегированное значение.
        /// <br />null, если исходные данные не удовлетворяют каким ни будь требованиям
        /// </returns>
        private ParamValueItem ExistAggregation(int minimalCount, DateTime valueTime, ParamValueItem[] values)
        {
            int count = 0;

            for (int i = 0; count < minimalCount && i < values.Length; i++)
            {
                if (values[i].Quality > 0
                       && !double.IsNaN(values[i].Value))
                    ++count;
            }

            return new ParamValueItem(valueTime, Quality.Good, count >= minimalCount ? 1 : 0);
        }
    }
}
