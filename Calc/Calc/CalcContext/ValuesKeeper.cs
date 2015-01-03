using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Хранилище значений параметров. 
    /// Содержит сагрегированные значения для всех параметров, встреченных в расчёте, 
    /// а также исходные значения (в том числе только что расчитанные) для расчитываемых параметров.
    /// </summary>
    class ValuesKeeper : IValuesKeeper
    {
        /// <summary>
        /// Связь с основной системой.
        /// Используется для получения списуа ревизий
        /// и для расчёта агрегации на основании имеющихся исходных данных
        /// </summary>
        ICalcSupplier calcSupplier;

        ValueAggregator valueAggregator;

        /// <summary>
        /// Ключ, для индексации агрегированных значений в хранилище
        /// </summary>
        class ValueKey
        {
            /// <summary>
            /// Алгоритм агрегации
            /// </summary>
            public CalcAggregation Aggregation { get; set; }

            /// <summary>
            /// Время значение
            /// </summary>
            public DateTime Time { get; set; }

            /// <summary>
            /// Интервал агрегации
            /// </summary>
            public Interval Period { get; set; }

            /// <summary>
            /// Параметры агрегации
            /// </summary>
            public IEnumerable<CalcNodeKey> Nodes
            {
                get
                {
                    return calcNodeKeyList.AsReadOnly();
                }
            }

            List<CalcNodeKey> calcNodeKeyList = new List<CalcNodeKey>();

            /// <summary>
            /// Добавить параметр в список параметров агрегации
            /// </summary>
            /// <param name="item"></param>
            public void AddNodes(CalcNodeKey item)
            {
                calcNodeKeyList.Add(item);
            }

            public override bool Equals(object obj)
            {
                ValueKey key = obj as ValueKey;

                if (key != null)
                {
                    bool ret = Aggregation.Equals(key.Aggregation)
                        && Time.Equals(key.Time)
                        && Period.Equals(key.Period)
                        && calcNodeKeyList.Count.Equals(key.calcNodeKeyList.Count);

                    for (int i = 0; ret && i < calcNodeKeyList.Count; i++)
                    {
                        ret = calcNodeKeyList[i].Equals(key.calcNodeKeyList[i]);
                    }
                    return ret;
                }
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                int hash = 0;

                foreach (var item in Nodes)
                {
                    hash += item.GetHashCode();
                }

                return Aggregation.GetHashCode() + Time.GetHashCode() + Period.GetHashCode() + hash;
            }
        }

        /// <summary>
        /// Хранилище агрегированных данных
        /// </summary>
        Dictionary<ValueKey, SymbolValue> valuesBank;

        /// <summary>
        /// Хранилище исходных данных (в том числе и только что расчитанных).
        /// Хранится в виде: (параметр, время) => (значение, true, если значение расчитанно).
        /// </summary>
        /// <remarks>bool'овский аргумент означает посчитанное это значение (и должно возвращаться по GetAllCalculatedValues()) или просто сырое.</remarks>
        Dictionary<CalcNodeKey, Dictionary<DateTime, Tuple<SymbolValue, bool>>> rawValuesBank;

        /// <summary>
        /// Хранилище оптимальных аргументов оптимизации.
        /// Хранится в виде: (параметр, время) => аргументы.
        /// </summary>
        Dictionary<CalcNodeKey, Dictionary<DateTime, ArgumentsValues>> optimalArgsBank;

        Dictionary<ICalcNode, List<TimeRange>> failBank;

        public ValuesKeeper(ICalcSupplier calcSupplier)
        {
            this.calcSupplier = calcSupplier;

            valuesBank = new Dictionary<ValueKey, SymbolValue>();

            rawValuesBank = new Dictionary<CalcNodeKey, Dictionary<DateTime, Tuple<SymbolValue, bool>>>();

            optimalArgsBank = new Dictionary<CalcNodeKey, Dictionary<DateTime, ArgumentsValues>>();

            failBank = new Dictionary<ICalcNode, List<TimeRange>>();

            valueAggregator = new ValueAggregator();
        }

        /// <summary>
        /// Сопоставляет алгоритм агрегации и параметры агрегации
        /// </summary>
        /// <param name="aggregation">Алгоритм агрегации</param>
        /// <param name="aggregationNodes">Параметры агрегации</param>
        /// <exception cref="InvalidOperationException">Число параметров не соответствует требуемому для агрегации</exception>
        private void CheckAggregation(CalcAggregation aggregation, CalcNodeKey[] aggregationNodes)
        {
            int count = aggregationNodes == null ? 0 : aggregationNodes.Length;

            int expectedCount = aggregation == CalcAggregation.Weighted ? 2 : 1;

            if (count != expectedCount)
                throw new InvalidOperationException("Число параметров не соответствует требуемому для агрегации");
        }

        public void AddCalculatedValue(ICalcNode calcNode, ArgumentsValues args, DateTime time, SymbolValue symbolValue)
        {
            AddRawValue(calcNode, args, time, symbolValue, true);
        }

        public void ClearAggregation(ICalcNode calcNode, ArgumentsValues args, DateTime time)
        {
            var nodeKey = new CalcNodeKey(calcNode, args);
            var nodeInfo = calcNode.Revisions.Get(time);

            var deleteKeys = from k in valuesBank.Keys
                             where k.Nodes.Contains(nodeKey) && Interval.Cross(k.Time, k.Period, time, nodeInfo.Interval)
                             select k;

            foreach (var key in deleteKeys.ToArray())
            {
                valuesBank.Remove(key);
            }
        }

        public void SetFailNode(ICalcNodeInfo nodeInfo, DateTime startTime, DateTime endTime)
        {
            bool extend = false;
            List<TimeRange> timeIntervals;

            if (!failBank.TryGetValue(nodeInfo.CalcNode, out timeIntervals))
                failBank[nodeInfo.CalcNode] = timeIntervals = new List<TimeRange>();

            foreach (var interval in timeIntervals)
            {
                if ((interval.Start <= startTime && startTime <= interval.End)
                    || (interval.Start <= endTime && endTime <= interval.End)
                    || (startTime < interval.Start && interval.End < endTime))
                {
                    interval.Start = interval.Start < startTime ? interval.Start : startTime;
                    interval.End = interval.End > endTime ? interval.End : endTime;
                    extend = true;
                    break;
                }
            }

            //var newIntervals=(from interval in timeIntervals

            if (!extend)
            {
                timeIntervals.Add(new TimeRange(startTime, endTime));
            }

            // для оптимизации выставить флаг для дочерних элементов
            IOptimizationInfo optimizationInfo = nodeInfo as IOptimizationInfo;
            if (optimizationInfo != null)
            {
                if (optimizationInfo.ChildOptimization != null)
                    foreach (var item in optimizationInfo.ChildOptimization)
                    {
                        SetFailNode(item, startTime, endTime);
                    }
                if (optimizationInfo.ChildParameters != null)
                    foreach (var item in optimizationInfo.ChildParameters)
                    {
                        SetFailNode(item, startTime, endTime);
                    }
            }
        }

        private bool CheckFailNode(ICalcNode calcNode, DateTime startTime, DateTime endTime)
        {
            List<TimeRange> timeIntervals;

            if (failBank.TryGetValue(calcNode, out timeIntervals))
            {
                foreach (var interval in timeIntervals)
                {
                    if ((interval.Start <= startTime && startTime <= interval.End)
                        || (interval.Start <= endTime && endTime <= interval.End)
                        || (startTime < interval.Start && interval.End < endTime))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckFailNode(DateTime startTime, DateTime endTime, CalcNodeKey[] aggregationNodes)
        {
            bool ret = false;

            foreach (var item in aggregationNodes)
            {
                ret |= CheckFailNode(item.Node, startTime, endTime);
            }
            return ret;
        }

        public Package[] GetAllCalculatedValues()
        {
            // Для условных параметров, если в пачке есть хотябы 1 расчитаное значение, 
            // то нужно добавлять и исходные значения
            // из-за подхода к хранению значений

            Dictionary<RevisionInfo, Dictionary<ICalcNodeInfo, List<Tuple<ParamValueItem, bool>>>> revisionDictionary = new Dictionary<RevisionInfo, Dictionary<ICalcNodeInfo, List<Tuple<ParamValueItem, bool>>>>();
            Dictionary<ICalcNodeInfo, List<Tuple<ParamValueItem, bool>>> packageDictionary;

            foreach (var key in rawValuesBank.Keys)
            {
                List<Tuple<ParamValueItem, bool>> vals = new List<Tuple<ParamValueItem, bool>>();
                foreach (var time in rawValuesBank[key].Keys)
                {
                    vals.Add(Tuple.Create(
                        GetParamReceiveItem(time, key.Arguments, rawValuesBank[key][time].Item1),
                        rawValuesBank[key][time].Item2));
                }

                // разбиваем значения по ревизиям
                foreach (var valueItem in vals)
                {
                    if (valueItem.Item1 != null)
                    {
                        RevisionInfo revision = calcSupplier.GetRevision(valueItem.Item1.Time);
                        ICalcNodeInfo nodeInfo = key.Node.Revisions.Get(revision);
                        List<Tuple<ParamValueItem, bool>> pack;

                        if (!revisionDictionary.TryGetValue(revision, out packageDictionary))
                            revisionDictionary[revision] = packageDictionary = new Dictionary<ICalcNodeInfo, List<Tuple<ParamValueItem, bool>>>();

                        if (!packageDictionary.TryGetValue(nodeInfo, out pack))
                            packageDictionary[nodeInfo] = pack = new List<Tuple<ParamValueItem, bool>>();

                        pack.Add(valueItem);
                    }
                }
            }

            // нормализуем аргументы параметров
            foreach (var revision in revisionDictionary.Keys)
            {
                foreach (var calcNodeInfo in revisionDictionary[revision].Keys)
                {
                    String[] args = GetArguments(calcNodeInfo);

                    var values = revisionDictionary[revision][calcNodeInfo];

                    //// если нет ни одного расчитанного значения пропускаем пачку
                    //if (values.Count(t => t.Item2) == 0)
                    //    continue;

                    // если параметр не условный, удалить исходные параметры
                    if (args == null)
                    {
                        values.RemoveAll(t => !t.Item2);
                    }
                    else 
                    {
                        // если нет ни одного расчитанного значения пропускаем пачку
                        if (values.Count(t => t.Item2) == 0)
                        {
                            values.Clear();
                        }
                        else
                        {
                            // нормализуем аргументы условных параметров
                            var toDeleteValues = (from v in values
                                                  where v.Item1.Arguments == null || !v.Item1.Arguments.CorrespondTo(args)
                                                  select v).ToList();

                            List<Tuple<ParamValueItem, bool>> toAddValues = new List<Tuple<ParamValueItem, bool>>();

                            values.RemoveAll(v => toDeleteValues.Contains(v));

                            var times = (from v in toDeleteValues select v.Item1.Time).Distinct();

                            foreach (var time in times)
                            {
                                var findedItem = values.Find(v => v.Item1.Time.Equals(time) && v.Item1.Arguments.CorrespondTo(args));

                                if (findedItem == null)
                                {
                                    toAddValues.Add(Tuple.Create(
                                        new ParamValueItem(ArgumentsValues.BadArguments, time, Quality.Bad, double.NaN),
                                        true));
                                }
                            }

                            values.AddRange(toAddValues);
                        }
                    }
                }
            }

            // дополняем промежуточную таблицу бэдами для параметров с ошибками на стадии компиляции
            List<TimeRange> failTimes;

            foreach (var calcNode in failBank.Keys)
            {
                failTimes = failBank[calcNode];
                foreach (var item in failTimes)
                {
                    var revisions = calcSupplier.GetRevisions(item.Start, item.End).ToArray();

                    DateTime startTime = item.Start, endTime;
                    for (int i = 0; i < revisions.Length; i++)
                    {
                        RevisionInfo revision = revisions[i];
                        ICalcNodeInfo calcNodeInfo = calcNode.Revisions.Get(revision);

                        startTime = calcNodeInfo.Interval.NearestEarlierTime(/*calcNodeInfo.StartTime,*/ startTime);
                        if (i + 1 < revisions.Length)
                            endTime = revisions[i + 1].Time;
                        else
                            endTime = item.End;

                        if (calcNodeInfo.Interval != Interval.Zero && startTime < endTime)
                        {
                            List<Tuple<ParamValueItem, bool>> package;

                            if (!revisionDictionary.TryGetValue(revision, out packageDictionary))
                                revisionDictionary[revision] = packageDictionary = new Dictionary<ICalcNodeInfo, List<Tuple<ParamValueItem, bool>>>();

                            if (!packageDictionary.TryGetValue(calcNodeInfo, out package))
                                packageDictionary[calcNodeInfo] = package = new List<Tuple<ParamValueItem, bool>>();

                            while (startTime < endTime)
                            {
                                var key = new CalcNodeKey(calcNode, null);

                                if (!rawValuesBank.ContainsKey(key)
                                    || !rawValuesBank[key].ContainsKey(startTime))
                                {
                                    ParamValueItem valueItem = new ParamValueItem(ArgumentsValues.BadArguments, startTime, Quality.Bad, double.NaN);

                                    package.Add(Tuple.Create(valueItem, true));
                                }
                                if (calcNodeInfo.Interval == Interval.Zero)
                                {
                                    startTime = endTime;
                                }
                                else
                                {
                                    startTime = calcNodeInfo.Interval.GetNextTime(startTime);
                                }
                            }
                        }
                    }
                }
            }

            // сбрасываем все в список
            List<Package> packageList = new List<Package>();

            foreach (var dict in revisionDictionary.Values)
            {
                foreach (var calcNodeInfo in dict.Keys)
                {
                    Package pack = new Package() { Id = calcNodeInfo.CalcNode.NodeID };
                    pack.Values.AddRange(from t in dict[calcNodeInfo] select t.Item1);

                    packageList.Add(pack);
                }
            }
            return packageList.ToArray();
        }

        /// <summary>
        /// Получить список аргументов требуемых для данного узла
        /// </summary>
        /// <param name="calcNodeInfo">Узел расчёта</param>
        /// <returns></returns>
        private string[] GetArguments(ICalcNodeInfo calcNodeInfo)
        {
            List<String> args = new List<string>();

            if (calcNodeInfo.Optimization != null)
            {
                var subArgs = GetArguments(calcNodeInfo.Optimization);
                if (subArgs != null)
                    args.AddRange(subArgs);
            }

            var optimizationInfo = calcNodeInfo as IOptimizationInfo;

            if (optimizationInfo != null)
            {
                args.AddRange(from a in optimizationInfo.Arguments select a.Name);
            }

            if (args.Count == 0)
                return null;

            return args.ToArray();
        }

        public void AddRawValue(ICalcNode calcNode, ArgumentsValues args, DateTime time, SymbolValue symbolValue)
        {
            AddRawValue(calcNode, args, time, symbolValue, false);
        }

        /// <summary>
        /// Добавить исходное значение в хранилище
        /// </summary>
        /// <param name="calcNode">Параметр</param>
        /// <param name="args">Аргументы для условного параметра. Для обычных параметров null</param>
        /// <param name="time">Время значения</param>
        /// <param name="value">Значение</param>
        /// <param name="isCalculated">Является ли значение только что посчитанным</param>
        private void AddRawValue(ICalcNode calcNode, ArgumentsValues args, DateTime time, SymbolValue value, bool isCalculated)
        {
            Dictionary<DateTime, Tuple<SymbolValue, bool>> timeDictionary;

            CalcNodeKey key = new CalcNodeKey()
            {
                Node = calcNode,
                Arguments = args
            };

            if (!rawValuesBank.TryGetValue(key, out timeDictionary))
                rawValuesBank[key] = timeDictionary = new Dictionary<DateTime, Tuple<SymbolValue, bool>>();

            timeDictionary[time] = Tuple.Create(value, isCalculated);
        }

        public SymbolValue GetRawValue(ICalcNode calcNode, ArgumentsValues args, DateTime time)
        {
            if (CheckFailNode(calcNode, time, time))
                return SymbolValue.Nothing;

            SymbolValue value = null;
            Tuple<SymbolValue, bool> tuple;
            Dictionary<DateTime, Tuple<SymbolValue, bool>> timeDictionary;

            CalcNodeKey key = new CalcNodeKey()
            {
                Node = calcNode,
                Arguments = args
            };

            if (rawValuesBank.TryGetValue(key, out timeDictionary)
                && timeDictionary.TryGetValue(time, out tuple))
                value = tuple.Item1;

            return value;
        }

        public bool IsCalculated(
            ICalcNode calcNode,
            ArgumentsValues arguments,
            DateTime time)
        {
            bool value;
            Tuple<SymbolValue, bool> tuple;
            Dictionary<DateTime, Tuple<SymbolValue, bool>> timeDictionary;

            CalcNodeKey key = new CalcNodeKey()
            {
                Node = calcNode,
                Arguments = arguments
            };

            if (rawValuesBank.TryGetValue(key, out timeDictionary)
                && timeDictionary.TryGetValue(time, out tuple))
                value = tuple.Item2;
            else
                value = false;

            return value;
        }

        public void AddValue(CalcAggregation aggregation, DateTime time, Interval period, SymbolValue value, params CalcNodeKey[] aggregationNodes)
        {
            CheckAggregation(aggregation, aggregationNodes);

            ValueKey key = new ValueKey()
            {
                Aggregation = aggregation,
                Time = time,
                Period = period
            };

            foreach (var item in aggregationNodes)
            {
                key.AddNodes(item);
            }

            valuesBank[key] = value;
        }

        public SymbolValue GetValue(CalcAggregation aggregation, DateTime time, Interval period, params CalcNodeKey[] aggregationNodes)
        {
            if (CheckFailNode(time, period.GetNextTime(time), aggregationNodes))
                return SymbolValue.Nothing;

            if (aggregation == CalcAggregation.Nothing)
                return GetRawValue(aggregationNodes[0].Node, aggregationNodes[0].Arguments, time);

            CheckAggregation(aggregation, aggregationNodes);

            ValueKey key = new ValueKey()
            {
                Aggregation = aggregation,
                Time = time,
                Period = period
            };

            foreach (var item in aggregationNodes)
            {
                key.AddNodes(item);
            }

            SymbolValue value;

            if (!valuesBank.TryGetValue(key, out value))
            {
                List<List<ParamValueItem>> valueToAggregationList = new List<List<ParamValueItem>>();
                Interval sourceInterval = period;

                foreach (var calcNodeKey in aggregationNodes)
                {
                    ICalcNodeInfo nodeInfo = calcNodeKey.Node.Revisions.Get(time);

                    DateTime startTime, endTime;

                    sourceInterval = nodeInfo.Interval;

                    // корректируем время исходных значений
                    startTime = time;
                    endTime = period.GetNextTime(time);
                    valueAggregator.GetSourceRange(nodeInfo.Interval, period, ref startTime, ref endTime);

                    var times = GetTimes(calcNodeKey.Node, startTime, endTime);

                    // получаем исходные значения
                    bool hasBlocked;
                    var valuesToAggregation = GetRawValues(calcNodeKey, times/*, out hasNothing*/, out hasBlocked);

                    //calcSupplier.GetParameterNodeRawValues(calcCon

                    if (hasBlocked)
                        return SymbolValue.BlockedValue;
                    else if (valuesToAggregation == null || valuesToAggregation.Count == 0)
                        return value;
                    else
                        valueToAggregationList.Add(valuesToAggregation);
                }

                // попытка посчитать агрегацию
                ParamValueItem valueItem = valueAggregator.Aggregate(aggregation, 
                                                                     sourceInterval, 
                                                                     period,
                                                                     valueToAggregationList.ToArray()).FirstOrDefault();

                if (valueItem != null && valueItem.Quality == Quality.Good)
                {
                    value = SymbolValue.CreateValue(valueItem.Value);

                    // сохраняем удачно сагрегированное значение
                    AddValue(aggregation, time, period, value, aggregationNodes);
                }
                else if (valueItem != null && valueItem.Quality == Quality.Bad)
                {
                    value = SymbolValue.Nothing;

                    // сохраняем удачно сагрегированное значение
                    AddValue(aggregation, time, period, value, aggregationNodes);
                }
            }

            return value;
        }

        private IEnumerable<DateTime> GetTimes(ICalcNode calcNode, DateTime startTime, DateTime endTime)
        {
            Stack<DateTime> timeStack = new Stack<DateTime>();
            ICalcNodeInfo calcNodeInfo;

            DateTime time = endTime.AddMilliseconds(-1);

            while (time >= startTime)
            {
                calcNodeInfo = calcNode.Revisions.Get(time);
                if (calcNodeInfo.Interval == Interval.Zero)
                    break;
                time = calcNodeInfo.Interval.NearestEarlierTime(/*calcNodeInfo.StartTime,*/ time);

                timeStack.Push(time);

                time = calcNodeInfo.Interval.GetPrevTime(time);
            }

            return timeStack;
        }

        /// <summary>
        /// Получить исходные данные параметра за запрашиваемый интервал
        /// </summary>
        /// <param name="calcNodeKey">Параметр</param>
        /// <param name="times">Времена для которых требуется получить значения</param>
        /// <param name="hasBlocked">Выставляется в true, если одно из исходных значений равно SymbolValue.BlockedValue</param>
        /// <returns></returns>
        private List<ParamValueItem> GetRawValues(
            CalcNodeKey calcNodeKey, 
            IEnumerable<DateTime> times,
            out bool hasBlocked)
        {
            Dictionary<DateTime, Tuple<SymbolValue, bool>> timeDictionary;

            hasBlocked = false;

            List<ParamValueItem> retList = new List<ParamValueItem>();
            var key = new CalcNodeKey(calcNodeKey.Node, calcNodeKey.Arguments);

            foreach (var time in times)
            {
                if (CheckFailNode(calcNodeKey.Node, time, time))
                {
                    retList.Add(GetParamReceiveItem(time, calcNodeKey.Arguments, SymbolValue.Nothing));
                }
                else
                {
                    ArgumentsValues args;
                    Tuple<SymbolValue, bool> valTuple;
                    var node = calcNodeKey.Node.Revisions.Get(time);

                    // уточняем аргументы при неявной передаче аргументов
                    if (node.Optimization != null
                        && node.Optimization.Calculable
                        && NeededArgs(node.Optimization, calcNodeKey.Arguments)
                        && (args = GetOptimal(node.Optimization.CalcNode, calcNodeKey.Arguments, time)) != null)
                    {
                        key.Arguments = args;
                    }
                    else
                    {
                        key.Arguments = calcNodeKey.Arguments;
                    }

                    if (key.Arguments == ArgumentsValues.BadArguments)
                    {
                        retList.Add(GetParamReceiveItem(time, calcNodeKey.Arguments, SymbolValue.Nothing));
                    }
                    else if (rawValuesBank.TryGetValue(key, out timeDictionary)
                        && timeDictionary.TryGetValue(time, out valTuple))
                    {
                        SymbolValue val = valTuple.Item1;

                        if (val == SymbolValue.BlockedValue)
                        {
                            hasBlocked = true;
                            break;
                        }
                        else
                        {
                            retList.Add(GetParamReceiveItem(time, key.Arguments, val));
                        }
                    }
                    //else if (!node.Calculable)
                    //{
                    //    retList.Add(GetParamReceiveItem(time, key.Arguments, SymbolValue.Nothing));
                    //}
                    else
                    {
                        return null;
                    }
                }
            }
            return retList;
        }

        /// <summary>
        /// Проверить достаточно ли переданных аргументов для получения значения оптимизации
        /// </summary>
        /// <param name="optimizationIndo"></param>
        /// <param name="args"></param>
        /// <returns>Возвращает true, если переданных аргументов не достаточно</returns>
        private bool NeededArgs(IOptimizationInfo optimizationIndo, ArgumentsValues args)
        {
            var info = optimizationIndo;

            IEnumerable<String> argNames = new String[] { };

            while (info != null)
            {
                argNames = argNames.Union(from a in info.Arguments select a.Name);
                info = info.Optimization;
            }

            var argArr=argNames.ToArray();

            return (args == null && argArr.Length > 0) 
                || (args != null && !args.CorrespondTo(argNames.ToArray()));
        }

        /// <summary>
        /// Создать ParamValueItem по времени и значению параметра
        /// </summary>
        /// <param name="time">Время значения</param>
        /// <param name="value">Значение параметра</param>
        /// <returns></returns>
        private ParamValueItem GetParamReceiveItem(DateTime time, ArgumentsValues args, SymbolValue value)
        {
            Quality quality;
            double val;

            if (value == SymbolValue.BlockedValue)
                return null;
            if (value == SymbolValue.Nothing)
                quality = Quality.Bad;
            else quality = Quality.Good;

            try
            {
                if (quality == Quality.Good)
                    val = Convert.ToDouble(value.GetValue());
                else val = double.NaN;
            }
            catch (InvalidCastException)
            {
                quality = Quality.Bad;
                val = double.NaN;
            }

            return new ParamValueItem(args, time, quality, val);
        }

        public void SetOptimal(ICalcNode calcNode, ArgumentsValues args, DateTime time, ArgumentsValues optimalArgs)
        {
            Dictionary<DateTime, ArgumentsValues> optimalValues;
            CalcNodeKey key = new CalcNodeKey()
            {
                Node = calcNode,
                Arguments = args
            };

            if (!optimalArgsBank.TryGetValue(key, out optimalValues))
                optimalArgsBank[key] = optimalValues = new Dictionary<DateTime, ArgumentsValues>();

            optimalValues[time] = optimalArgs;
        }

        public ArgumentsValues GetOptimal(ICalcNode calcNode, ArgumentsValues args, DateTime time)
        {
            if (CheckFailNode(calcNode, time, time))
                return ArgumentsValues.BadArguments;

            // запросить, если требуется оптимальные аргументы базовой оптимизации
            var nodeInfo = calcNode.Revisions.Get(time);

            if (nodeInfo.Optimization != null
                && (args == null || !args.Include(GetArguments(nodeInfo.Optimization))))
            {
                args = GetOptimal(nodeInfo.Optimization.CalcNode, args, time);
            }

            Dictionary<DateTime, ArgumentsValues> optimalValues;
            ArgumentsValues optimalArgs = null;
            CalcNodeKey key = new CalcNodeKey()
            {
                Node = calcNode,
                Arguments = args != null && args.Count == 0 ? null : args
            };

            if (!optimalArgsBank.TryGetValue(key, out optimalValues)
                || !optimalValues.TryGetValue(time, out optimalArgs))
                optimalArgs = null;

            return optimalArgs;
        }
    }
}
