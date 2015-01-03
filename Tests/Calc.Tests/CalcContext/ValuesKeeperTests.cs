using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Calc;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.Calc
{
    [TestFixture]
    public class ValuesKeeperTests
    {
        private void AssertSymbol(double expectedValue, SymbolValue symbolValue)
        {
            Assert.IsNotNull(symbolValue);
            Assert.IsNotNull(symbolValue.GetValue());
            Assert.AreEqual(expectedValue, symbolValue.GetValue());
        }

        [Test]
        public void AddValueGetValue__AggregationAggregationNodesCheck()
        {
            ValuesKeeper keeper = new ValuesKeeper(null);

            // параметры
            ICalcNode calcNode1 = new TestCalcNode() { NodeID = 1, Name = "mambru" };
            ICalcNode calcNode2 = new TestCalcNode() { NodeID = 2, Name = "mambru" };
            ICalcNode calcNode3 = new TestCalcNode() { NodeID = 3, Name = "mambru" };

            // аргументы
            ArgumentsValues args1 = new ArgumentsValues(new { N = 1, X = 3 });
            var aggregatinNodes = new ICalcNode[] { calcNode2 };

            CalcAggregation[] errorAggregation, passAggregation;

            // агрегация с 1 параметром
            passAggregation = new CalcAggregation[]{
                CalcAggregation.First, 
                CalcAggregation.Last, 
                CalcAggregation.Minimum,
                CalcAggregation.Maximum,
                CalcAggregation.Sum, 
                CalcAggregation.Average,
                CalcAggregation.Exist,
                CalcAggregation.Count
            };
            errorAggregation = new CalcAggregation[] { CalcAggregation.Weighted };

            AssertAggregationError(keeper, new CalcNodeKey[] { new CalcNodeKey(calcNode1, args1) }, errorAggregation, passAggregation);

            // агрегация с 2 параметрами
            errorAggregation = new CalcAggregation[]{
                CalcAggregation.First, 
                CalcAggregation.Last, 
                CalcAggregation.Minimum,
                CalcAggregation.Maximum,
                CalcAggregation.Sum, 
                CalcAggregation.Average,
                CalcAggregation.Exist,
                CalcAggregation.Count
            };
            passAggregation = new CalcAggregation[] { CalcAggregation.Weighted };

            AssertAggregationError(keeper,
                new CalcNodeKey[]{
                    new CalcNodeKey(calcNode1, args1),
                    new CalcNodeKey {Node= calcNode2 }
                }, 
                errorAggregation, passAggregation);

            // агрегация с 3 параметрами
            errorAggregation = new CalcAggregation[]{
                CalcAggregation.First, 
                CalcAggregation.Last, 
                CalcAggregation.Minimum,
                CalcAggregation.Maximum,
                CalcAggregation.Sum, 
                CalcAggregation.Average,
                CalcAggregation.Exist,
                CalcAggregation.Count,
                CalcAggregation.Weighted
            };
            passAggregation = new CalcAggregation[] { };

            AssertAggregationError(keeper, 
                new CalcNodeKey[] { 
                    new CalcNodeKey(calcNode1, args1), 
                    new CalcNodeKey() { Node = calcNode2 }, 
                    new CalcNodeKey() { Node = calcNode3 } 
                }, 
                errorAggregation, passAggregation);
        }

        private static void AssertAggregationError(ValuesKeeper keeper, CalcNodeKey[] nodes, CalcAggregation[] errorAggregation, CalcAggregation[] passAggregation)
        {
            // не должно вызвать ошибки
            foreach (var aggregation in passAggregation)
            {
                keeper.AddValue(aggregation, new DateTime(2012, 01, 01), Interval.Day, SymbolValue.CreateValue(300), nodes);
                keeper.GetValue(aggregation, new DateTime(2012, 01, 01), Interval.Day, nodes);
            }

            // должно вызвать ошибку
            foreach (var aggregation in errorAggregation)
            {
                try
                {
                    keeper.AddValue(aggregation, new DateTime(2012, 01, 01), Interval.Day, SymbolValue.CreateValue(300), nodes);

                    Assert.Fail(String.Format("Агрегация {0} не должна работать при {1} параметрах", aggregation, nodes.Length));
                }
                catch (InvalidOperationException) { }
                try
                {
                    keeper.GetValue(aggregation, new DateTime(2012, 01, 01), Interval.Day, nodes);
                    Assert.Fail(String.Format("Агрегация {0} не должна работать при {1} параметрах", aggregation, nodes.Length));
                }
                catch (InvalidOperationException) { }
            }
        }

        [Test]
        public void GetValues__CoreleteWithAddValues()
        {
            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            ValuesKeeper keeper = new ValuesKeeper(calcSupplier.Object);

            // параметры
            ICalcNode calcNode1 = new TestCalcNode() { NodeID = 1, Name = "mambru" };
            ICalcNode calcNode2 = new TestCalcNode() { NodeID = 2, Name = "mambru" };
            ICalcNode calcNode3 = new TestCalcNode() { NodeID = 3, Name = "mambru" };

            IParameterInfo parameterInfo1 = new TestParameterinfo(calcNode1) { Interval = Interval.Day };
            IParameterInfo parameterInfo2 = new TestParameterinfo(calcNode2) { Interval = Interval.Day };
            IParameterInfo parameterInfo3 = new TestParameterinfo(calcNode3) { Interval = Interval.Day };

            // аргументы
            ArgumentsValues args1 = new ArgumentsValues(new { N = 1, X = 3 });
            ArgumentsValues args2= new ArgumentsValues(new { K = 1, C = 3 });
            ArgumentsValues args3 = new ArgumentsValues(new { N = 3, X = 1 });

            // добавляем значения
            keeper.AddValue(CalcAggregation.First, new DateTime(2012, 01, 01), Interval.Day, new DoubleValue(300), new CalcNodeKey(calcNode1, args1));
            keeper.AddValue(CalcAggregation.First, new DateTime(2012, 01, 02), Interval.Day, new DoubleValue(310), new CalcNodeKey(calcNode1, args2));
            keeper.AddValue(CalcAggregation.Sum, new DateTime(2012, 01, 04), Interval.Day, new DoubleValue(320), new CalcNodeKey(calcNode1, args3));
            keeper.AddValue(CalcAggregation.Average, new DateTime(2012, 01, 03), Interval.Day, new DoubleValue(330), new CalcNodeKey(calcNode2, args1));
            keeper.AddValue(CalcAggregation.Average, new DateTime(2012, 01, 10), Interval.Day, new DoubleValue(340), new CalcNodeKey(calcNode2, args1));
            keeper.AddValue(CalcAggregation.Count, new DateTime(2012, 01, 13), Interval.Day, new DoubleValue(350), new CalcNodeKey(calcNode2, args3));
            keeper.AddValue(CalcAggregation.Minimum, new DateTime(2012, 01, 08), Interval.Day, new DoubleValue(360), new CalcNodeKey(calcNode3, args2));

            keeper.AddValue(CalcAggregation.Sum, new DateTime(2012, 01, 09), Interval.Day, new DoubleValue(380), new CalcNodeKey(calcNode3, args1));

            // правильные запросы
            AssertSymbol(300, keeper.GetValue(CalcAggregation.First, new DateTime(2012, 01, 01), Interval.Day, new CalcNodeKey(calcNode1, args1)));
            AssertSymbol(310, keeper.GetValue(CalcAggregation.First, new DateTime(2012, 01, 02), Interval.Day, new CalcNodeKey(calcNode1, args2)));
            AssertSymbol(320, keeper.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 04), Interval.Day, new CalcNodeKey(calcNode1, args3)));
            AssertSymbol(330, keeper.GetValue(CalcAggregation.Average, new DateTime(2012, 01, 03), Interval.Day, new CalcNodeKey(calcNode2, args1)));
            AssertSymbol(340, keeper.GetValue(CalcAggregation.Average, new DateTime(2012, 01, 10), Interval.Day, new CalcNodeKey(calcNode2, args1)));
            AssertSymbol(350, keeper.GetValue(CalcAggregation.Count, new DateTime(2012, 01, 13), Interval.Day, new CalcNodeKey(calcNode2, args3)));
            AssertSymbol(360, keeper.GetValue(CalcAggregation.Minimum, new DateTime(2012, 01, 08), Interval.Day, new CalcNodeKey(calcNode3, args2)));
            AssertSymbol(380, keeper.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 09), Interval.Day, new CalcNodeKey(calcNode3, args1)));

            // неправильные запросы
            Assert.IsNull(keeper.GetValue(CalcAggregation.Average, new DateTime(2012, 01, 09), Interval.Day, new CalcNodeKey(calcNode3, args1)));
            Assert.IsNull(keeper.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 09), Interval.Day, new CalcNodeKey(calcNode3, args2)));
            Assert.IsNull(keeper.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 15), Interval.Day, new CalcNodeKey(calcNode3, args1)));
            Assert.IsNull(keeper.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 09), Interval.Day, new CalcNodeKey(calcNode2, args1)));
        }

        [Test]
        public void GetValues__CoreleteWithAddCalculatedValues()
        {
            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            //calcSupplier.Setup(s => s.Aggregate(
            //    Moq.It.IsAny<DateTime>(),
            //    Moq.It.IsAny<Interval>(),
            //    Moq.It.IsAny<Interval>(),
            //    Moq.It.IsAny<CalcAggregation>(),
            //    Moq.It.IsAny<List<ParamValueItem>[]>()))
            //    .Returns((ParamValueItem)null);

            ValuesKeeper keeper = new ValuesKeeper(calcSupplier.Object);

            // параметры
            ICalcNode calcNode1 = new TestCalcNode() { NodeID = 1, Name = "mambru" };
            ICalcNode calcNode2 = new TestCalcNode() { NodeID = 2, Name = "mambru" };
            ICalcNode calcNode3 = new TestCalcNode() { NodeID = 3, Name = "mambru" };

            IParameterInfo parameterInfo1 = new TestParameterinfo(calcNode1) { Interval = Interval.Day };
            IParameterInfo parameterInfo2 = new TestParameterinfo(calcNode2) { Interval = Interval.Day };
            IParameterInfo parameterInfo3 = new TestParameterinfo(calcNode3) { Interval = Interval.Day };

            // аргументы
            ArgumentsValues args1 = new ArgumentsValues(new { N = 1, X = 3 });
            ArgumentsValues args2 = new ArgumentsValues(new { K = 1, C = 3 });
            ArgumentsValues args3 = new ArgumentsValues(new { N = 3, X = 1 });

            // добавляем значения
            keeper.AddCalculatedValue(calcNode1, args1, new DateTime(2012, 01, 01), new DoubleValue(300));
            keeper.AddCalculatedValue(calcNode1, args2, new DateTime(2012, 01, 02), new DoubleValue(310));
            keeper.AddCalculatedValue(calcNode2, args1, new DateTime(2012, 01, 03), new DoubleValue(330));
            keeper.AddCalculatedValue(calcNode2, args1, new DateTime(2012, 01, 10), new DoubleValue(340));
            keeper.AddCalculatedValue(calcNode3, args3, new DateTime(2012, 01, 08), new DoubleValue(360));
            keeper.AddCalculatedValue(calcNode3, args2, new DateTime(2012, 01, 07), new DoubleValue(370));

            // правильные запросы
            AssertSymbol(300, keeper.GetValue(CalcAggregation.Nothing, new DateTime(2012, 01, 01), Interval.Day, new CalcNodeKey(calcNode1, args1)));
            AssertSymbol(310, keeper.GetValue(CalcAggregation.Nothing, new DateTime(2012, 01, 02), Interval.Day, new CalcNodeKey(calcNode1, args2)));
            AssertSymbol(330, keeper.GetValue(CalcAggregation.Nothing, new DateTime(2012, 01, 03), Interval.Day, new CalcNodeKey(calcNode2, args1)));
            AssertSymbol(340, keeper.GetValue(CalcAggregation.Nothing, new DateTime(2012, 01, 10), Interval.Day, new CalcNodeKey(calcNode2, args1)));
            AssertSymbol(360, keeper.GetValue(CalcAggregation.Nothing, new DateTime(2012, 01, 08), Interval.Day, new CalcNodeKey(calcNode3, args3)));
            AssertSymbol(370, keeper.GetValue(CalcAggregation.Nothing, new DateTime(2012, 01, 07), Interval.Day, new CalcNodeKey(calcNode3, args2)));

            // неправильные запросы
            Assert.IsNull(keeper.GetValue(CalcAggregation.Average, new DateTime(2012, 01, 09), Interval.Day, new CalcNodeKey(calcNode3, args1)));
            Assert.IsNull(keeper.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 09), Interval.Day, new CalcNodeKey(calcNode3, args2)));
            Assert.IsNull(keeper.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 15), Interval.Day, new CalcNodeKey(calcNode3, args1)));
            Assert.IsNull(keeper.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 09), Interval.Day, new CalcNodeKey(calcNode2, args1)));
        }

        [Test]
        public void GetAllCalculatedValues__ReturnsCalculatedValues()
        {
            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            calcSupplier.Setup(s => s.GetRevision(Moq.It.IsAny<DateTime>())).Returns(RevisionInfo.Default);

            // параметры
            ICalcNode calcNode1 = new TestCalcNode() { NodeID = 1, Name = "mambru" };
            ICalcNode calcNode2 = new TestCalcNode() { NodeID = 2, Name = "mambru" };
            ICalcNode calcNode3 = new TestCalcNode() { NodeID = 3, Name = "mambru" };
            ICalcNode optimizationCalcNode = new TestCalcNode() { NodeID = 12, Name = "optmimzation" };

            IOptimizationInfo optimizationInfo = new TestOptimizationInfo(optimizationCalcNode) { 
                Interval = Interval.Day, 
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument(){Name="N"},
                    new TestOptimizationArgument(){Name="X"}
                } 
            };
            IParameterInfo parameterInfo1 = new TestParameterinfo(calcNode1) { Interval = Interval.Day };
            IParameterInfo parameterInfo2 = new TestParameterinfo(calcNode2) { Optimization = optimizationInfo, Interval = Interval.Day };
            IParameterInfo parameterInfo3 = new TestParameterinfo(calcNode3) { Interval = Interval.Day };

            // аргументы
            ArgumentsValues args1 = new ArgumentsValues(new { N = 1, X = 3 });
            ArgumentsValues args2 = new ArgumentsValues(new { N = 5, X = 7 });

            ValuesKeeper keeper = new ValuesKeeper(calcSupplier.Object);

            // значения первого параметра
            for (int i = 1; i < 32; i++)
            {
                keeper.AddCalculatedValue(calcNode1, null, new DateTime(2012, 01, i), SymbolValue.CreateValue(i));
            }

            // значения второго параметра
            for (int i = 1; i < 32; i++)
            {
                keeper.AddCalculatedValue(calcNode2, args1, new DateTime(2012, 01, i), SymbolValue.CreateValue(i));
                keeper.AddCalculatedValue(calcNode2, args2, new DateTime(2012, 01, i), SymbolValue.CreateValue(2 * i - 1));
            }
 
            // значения третьего параметра
            for (int i = 1; i < 32; i++)
            {
                keeper.AddCalculatedValue(calcNode3, null, new DateTime(2012, 01, i), SymbolValue.CreateValue(32-i));
            }

            // добавляем исходные значения, которые не должны попасть в результат
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 02, 01), SymbolValue.CreateValue(23));
            keeper.AddRawValue(calcNode3, null, new DateTime(2012, 02, 01), SymbolValue.CreateValue(456));
            keeper.AddRawValue(calcNode3, null, new DateTime(2012, 02, 02), SymbolValue.CreateValue(35));

            // исходные значения, которые должны попасть в результат
            keeper.AddRawValue(calcNode2, args1, new DateTime(2012, 02, 01), SymbolValue.CreateValue(435));
            keeper.AddRawValue(calcNode2, args2, new DateTime(2012, 02, 01), SymbolValue.CreateValue(345));


            // проверяемый метод
            Package[] packages = keeper.GetAllCalculatedValues();

            Assert.IsNotNull(packages);
            Assert.AreEqual(3, packages.Length);

            // проверяем знаечения первого параметра
            Package pack = (from p in packages where p.Id == 1 select p).FirstOrDefault();
            Assert.IsNotNull(pack);

            var values = (from v in pack.Values 
                          where v.Arguments == null 
                          orderby v.Time select v).ToArray();

            Assert.AreEqual(31, values.Length);
            for (int i = 0; i < 31; i++)
            {
                Assert.AreEqual(i + 1, values[i].Value);
            }

            // проверяем значения второго параметра 
            pack = (from p in packages where p.Id == 2 select p).FirstOrDefault();

            // значения для первого аргумента
            values = (from v in pack.Values 
                      where v.Arguments != null && v.Arguments.Equals(args1)
                      orderby v.Time 
                      select v).ToArray();

            Assert.AreEqual(32, values.Length);
            for (int i = 0; i < 31; i++)
            {
                Assert.AreEqual(new DateTime(2012, 01, i + 1), values[i].Time);
                Assert.AreEqual(i + 1, values[i].Value);
            }
            Assert.AreEqual(new DateTime(2012, 02, 01), values[31].Time);
            Assert.AreEqual(435.0, values[31].Value);

            // значения для второго аргумента
            values = (from v in pack.Values
                      where v.Arguments != null && v.Arguments.Equals(args2)
                      orderby v.Time
                      select v).ToArray();

            Assert.AreEqual(32, values.Length);
            for (int i = 0; i < 31; i++)
            {
                Assert.AreEqual(new DateTime(2012, 01, i + 1), values[i].Time);
                Assert.AreEqual(2 * (i + 1) - 1, values[i].Value);
            }
            Assert.AreEqual(new DateTime(2012, 02, 01), values[31].Time);
            Assert.AreEqual(345.0, values[31].Value);
            
            // значения без аргументов (их не должно быть)
            values = (from v in pack.Values 
                      where v.Arguments == null || (!v.Arguments.Equals(args1) && !v.Arguments.Equals(args2)) 
                      orderby v.Time 
                      select v).ToArray();

            Assert.AreEqual(0, values.Length);

            // проверяем значения третьего параметра
            pack = (from p in packages where p.Id == 3 select p).FirstOrDefault();
            Assert.IsNotNull(pack);

            values = (from v in pack.Values 
                      where v.Arguments == null
                      orderby v.Time 
                      select v).ToArray();

            Assert.AreEqual(31, values.Length);
            for (int i = 0; i < 31; i++)
            {
                Assert.AreEqual(31 - i, values[i].Value);
            }
        }

        class AggregationCalcSupplier : ICalcSupplier
        {

            public RevisionInfo GetRevision(DateTime time)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<RevisionInfo> GetRevisions(DateTime startTime, DateTime endTime)
            {
                throw new NotImplementedException();
            }

            public IParameterInfo GetParameterNode(ICalcContext context, RevisionInfo revision, string parameterCode)
            {
                throw new NotImplementedException();
            }

            public ICalcNode GetParameterNode(ICalcContext context, int nodeID)
            {
                throw new NotImplementedException();
            }

            public IParameterInfo GetEmptyParameterNode(RevisionInfo revision, string parameterCode)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<ICalcNode> GetParameterNodes(ICalcContext context)
            {
                throw new NotImplementedException();
            }

            public ParamValueItem GetParameterNodeValue(ICalcContext context, IEnumerable<Tuple<ICalcNode, ArgumentsValues>> nodeArgs, CalcAggregation agreg, DateTime startTime, Interval interval, out List<Message> messages, out bool serverNotAccessible)
            {
                throw new NotImplementedException();
            }

            public List<ParamValueItem> GetParameterNodeRawValues(ICalcContext context, ICalcNode parameterInfo, ArgumentsValues args, DateTime startTime, DateTime endTime, out List<Message> messages, out bool serverNotAccessible)
            {
                throw new NotImplementedException();
            }

            public void SaveParameterNodeValue(ICalcContext context, Package[] savingValues, out List<Message> messages)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<ConstsInfo> GetConsts()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<IExternalFunctionInfo> GetExternalFunctions(ICalcContext context)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<CustomFunctionInfo> GetCustomFunction()
            {
                throw new NotImplementedException();
            }

            public DateTime GetLastTimeValue(ICalcContext context, IParameterInfo paramInfo)
            {
                throw new NotImplementedException();
            }

            //public ParamValueItem Aggregate(DateTime time, Interval sourceInterval, Interval destInterval, CalcAggregation aggregation, List<ParamValueItem>[] values)
            //{
            //    if (values == null || values.Length == 0)
            //        return null;

            //    if (values.Length > 1 && aggregation != CalcAggregation.Weighted)
            //        return null;

            //    if (sourceInterval == Interval.Hour && values[0].Count < 24)
            //        return new ParamValueItem(time, Quality.Bad, 0);
            //    if (sourceInterval == Interval.Day && values[0].Count < 31)
            //        return new ParamValueItem(time, Quality.Bad, 0);

            //    bool haveBad = (from p in values[0] where p.Quality == Quality.Bad select p).Count() > 0;

            //    switch (aggregation)
            //    {
            //        case CalcAggregation.First:
            //            if (haveBad)
            //                return new ParamValueItem(time, Quality.Bad, 0);
            //            return values[0].First();
            //        case CalcAggregation.Last:
            //            if (haveBad)
            //                return new ParamValueItem(time, Quality.Bad, 0);
            //            return values[0].OrderByDescending(v => v.Time).First();
            //        case CalcAggregation.Sum:
            //            if (haveBad)
            //                return new ParamValueItem(time, Quality.Bad, 0);
            //            return new ParamValueItem()
            //            {
            //                Quality = Quality.Good,
            //                Time = time,
            //                Value = values[0].ConvertAll(v => v.Value).Aggregate((v1, v2) => v1 + v2)
            //            };
            //        case CalcAggregation.Average:
            //            if (haveBad)
            //                return new ParamValueItem(time, Quality.Bad, 0);
            //            return new ParamValueItem()
            //            {
            //                Quality = Quality.Good,
            //                Time = time,
            //                Value = values[0].ConvertAll(v => v.Value).Aggregate((v1, v2) => v1 + v2) / values[0].Count
            //            };
            //        case CalcAggregation.Minimum:
            //            if (haveBad)
            //                return new ParamValueItem(time, Quality.Bad, 0);
            //            return values[0].OrderBy(v => v.Value).First();
            //        case CalcAggregation.Maximum:
            //            if (haveBad)
            //                return new ParamValueItem(time, Quality.Bad, 0);
            //            return values[0].OrderByDescending(v => v.Value).First();
            //        case CalcAggregation.Count:
            //            return new ParamValueItem()
            //            {
            //                Quality = Quality.Good,
            //                Time = time,
            //                Value = (from p in values[0] where p.Quality == Quality.Good select p).Count()
            //            };
            //        case CalcAggregation.Exist:
            //            return new ParamValueItem()
            //            {
            //                Quality = Quality.Good,
            //                Time = time,
            //                Value = (from p in values[0] where p.Quality == Quality.Good select p).Count() == values[0].Count ? 1 : 0
            //            };
            //        case CalcAggregation.Weighted:
            //            double nom = 0, dev = 0;
            //            if (haveBad)
            //                return new ParamValueItem(time, Quality.Bad, 0);
            //            for (int i = 0; i < values[0].Count; i++)
            //            {
            //                nom += values[0][i].Value * values[1][i].Value;
            //                dev += values[1][i].Value;
            //            }
            //            return new ParamValueItem()
            //            {
            //                Quality = Quality.Good,
            //                Time = time,
            //                Value = nom / dev
            //            };
            //        default:
            //            break;
            //    }
            //    return null;

            //}

            public ArgumentsValues[] GetManualArgValue(ICalcContext context, IOptimizationInfo optimizationInfo, ArgumentsValues baseArgs, DateTime startTime)
            {
                throw new NotImplementedException();
            }
        }

        //private ParamValueItem AggregateMethod(DateTime time, Interval sourceInterval, Interval destInterval, CalcAggregation aggregation, List<ParamValueItem>[] values)
        //{
        //    if (values == null || values.Length == 0)
        //        return null;

        //    if (values.Length > 1 && aggregation != CalcAggregation.Weighted)
        //        return null;

        //    if (sourceInterval == Interval.Hour && values[0].Count < 24)
        //        return new ParamValueItem(time, Quality.Bad, 0);
        //    if (sourceInterval == Interval.Day && values[0].Count < 31)
        //        return new ParamValueItem(time, Quality.Bad, 0);

        //    bool haveBad = (from p in values[0] where p.Quality == Quality.Bad select p).Count() > 0;

        //    switch (aggregation)
        //    {
        //        case CalcAggregation.First:
        //            if (haveBad)
        //                return new ParamValueItem(time, Quality.Bad, 0);
        //            return values[0].First();
        //        case CalcAggregation.Last:
        //            if (haveBad)
        //                return new ParamValueItem(time, Quality.Bad, 0);
        //            return values[0].OrderByDescending(v => v.Time).First();
        //        case CalcAggregation.Sum:
        //            if (haveBad)
        //                return new ParamValueItem(time, Quality.Bad, 0);
        //            return new ParamValueItem()
        //            {
        //                Quality= Quality.Good,
        //                Time = time,
        //                Value = values[0].ConvertAll(v => v.Value).Aggregate((v1, v2) => v1 + v2)
        //            };
        //        case CalcAggregation.Average:
        //            if (haveBad)
        //                return new ParamValueItem(time, Quality.Bad, 0);
        //            return new ParamValueItem()
        //            {
        //                Quality = Quality.Good,
        //                Time = time,
        //                Value = values[0].ConvertAll(v => v.Value).Aggregate((v1, v2) => v1 + v2) / values[0].Count
        //            };
        //        case CalcAggregation.Minimum:
        //            if (haveBad)
        //                return new ParamValueItem(time, Quality.Bad, 0);
        //            return values[0].OrderBy(v => v.Value).First();
        //        case CalcAggregation.Maximum:
        //            if (haveBad)
        //                return new ParamValueItem(time, Quality.Bad, 0);
        //            return values[0].OrderByDescending(v => v.Value).First();
        //        case CalcAggregation.Count:
        //            return new ParamValueItem()
        //            {
        //                Quality = Quality.Good,
        //                Time = time,
        //                Value = (from p in values[0] where p.Quality == Quality.Good select p).Count()
        //            };
        //        case CalcAggregation.Exist:
        //            return new ParamValueItem()
        //            {
        //                Quality = Quality.Good,
        //                Time = time,
        //                Value = (from p in values[0] where p.Quality == Quality.Good select p).Count() == values[0].Count ? 1 : 0
        //            };
        //        case CalcAggregation.Weighted:
        //            double nom=0, dev=0;
        //            if (haveBad)
        //                return new ParamValueItem(time, Quality.Bad, 0);
        //            for (int i = 0; i < values[0].Count; i++)
        //            {
        //                nom += values[0][i].Value * values[1][i].Value;
        //                dev += values[1][i].Value;
        //            }
        //            return new ParamValueItem()
        //            {
        //                Quality = Quality.Good,
        //                Time = time,
        //                Value = nom / dev
        //            };
        //        default:
        //            break;
        //    }
        //    return null;
        //}

        [Test]
        public void GetValues_HasUnfullSourceValues_ReturnNull()
        {
            //var calcSupplier = new Moq.Mock<ICalcSupplier>();

            //calcSupplier.Setup(s => s.Aggregate(
            //    Moq.It.IsAny<DateTime>(),
            //    Moq.It.IsAny<Interval>(),
            //    Moq.It.IsAny<Interval>(),
            //    Moq.It.IsAny<CalcAggregation>(),
            //    Moq.It.IsAny<List<ParamValueItem>[]>()))
            //    .Returns((Func<DateTime, Interval, Interval, CalcAggregation, List<ParamValueItem>[], ParamValueItem>)AggregateMethod);
            var calcSupplier = new AggregationCalcSupplier();

            ValuesKeeper keeper = new ValuesKeeper(calcSupplier);//.Object);

            // параметры
            ICalcNode calcNode1 = new TestCalcNode() { NodeID = 1, Name = "mambru" };

            IParameterInfo parameterInfo = new TestParameterinfo(calcNode1)
            {
                Calculable = true,
                Interval = Interval.Hour
            }; 

            // аргументы
            ArgumentsValues args1 = new ArgumentsValues(new { N = 1, X = 3 });

            // добавляем значения
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 00, 00, 00), new DoubleValue(1));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 01, 00, 00), new DoubleValue(2));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 02, 00, 00), new DoubleValue(3));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 03, 00, 00), new DoubleValue(4));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 04, 00, 00), new DoubleValue(5));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 05, 00, 00), new DoubleValue(6));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 06, 00, 00), new DoubleValue(7));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 07, 00, 00), new DoubleValue(8));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 08, 00, 00), new DoubleValue(9));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 09, 00, 00), new DoubleValue(10));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 10, 00, 00), new DoubleValue(11));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 11, 00, 00), new DoubleValue(12));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 12, 00, 00), new DoubleValue(13));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 13, 00, 00), new DoubleValue(14));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 14, 00, 00), new DoubleValue(15));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 15, 00, 00), new DoubleValue(16));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 19, 00, 00), new DoubleValue(20));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 20, 00, 00), new DoubleValue(21));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 21, 00, 00), new DoubleValue(22));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 22, 00, 00), new DoubleValue(23));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 01, 23, 00, 00), new DoubleValue(24));
            keeper.AddRawValue(calcNode1, args1, new DateTime(2012, 01, 02, 00, 00, 00), new DoubleValue(25));

            // правильные запросы
            Assert.IsNull(keeper.GetValue(CalcAggregation.First, new DateTime(2012, 01, 01), Interval.Day, new CalcNodeKey(calcNode1, args1)));
            Assert.IsNull(keeper.GetValue(CalcAggregation.Last, new DateTime(2012, 01, 01), Interval.Day, new CalcNodeKey(calcNode1, args1)));
            Assert.IsNull(keeper.GetValue(CalcAggregation.Minimum, new DateTime(2012, 01, 01), Interval.Day, new CalcNodeKey(calcNode1, args1)));
            Assert.IsNull(keeper.GetValue(CalcAggregation.Maximum, new DateTime(2012, 01, 01), Interval.Day, new CalcNodeKey(calcNode1, args1)));
            Assert.IsNull(keeper.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 01), Interval.Day, new CalcNodeKey(calcNode1, args1)));
            Assert.IsNull(keeper.GetValue(CalcAggregation.Average, new DateTime(2012, 01, 01), Interval.Day, new CalcNodeKey(calcNode1, args1)));
            Assert.IsNull(keeper.GetValue(CalcAggregation.Count, new DateTime(2012, 01, 01), Interval.Day, new CalcNodeKey(calcNode1, args1)));
            Assert.IsNull(keeper.GetValue(CalcAggregation.Exist, new DateTime(2012, 01, 01), Interval.Day, new CalcNodeKey(calcNode1, args1)));
        }

        [Test]
        public void GetValues_HasSourceValues_ReturnAggregatedValue()
        {
            //var calcSupplier = new Moq.Mock<ICalcSupplier>();

            //calcSupplier.Setup(s => s.Aggregate(
            //    Moq.It.IsAny<DateTime>(),
            //    Moq.It.IsAny<Interval>(),
            //    Moq.It.IsAny<Interval>(),
            //    Moq.It.IsAny<CalcAggregation>(),
            //    Moq.It.IsAny<List<ParamValueItem>[]>()))
            //    .Returns((Func<DateTime, Interval, Interval, CalcAggregation, List<ParamValueItem>[], ParamValueItem>)AggregateMethod);
            var calcSupplier = new AggregationCalcSupplier();

            ValuesKeeper keeper = new ValuesKeeper(calcSupplier);//.Object);

            // параметры
            ICalcNode calcNode = new TestCalcNode() { NodeID = 1, Name = "mambru" };

            IParameterInfo parameterInfo = new TestParameterinfo(calcNode) { Interval = Interval.Hour };

            // аргументы
            ArgumentsValues args1 = new ArgumentsValues(new { N = 1, X = 3 });

            // добавляем значения
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DoubleValue(1));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 01, 00, 00, DateTimeKind.Utc), new DoubleValue(2));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 02, 00, 00, DateTimeKind.Utc), new DoubleValue(3));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 03, 00, 00, DateTimeKind.Utc), new DoubleValue(4));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 04, 00, 00, DateTimeKind.Utc), new DoubleValue(5));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 05, 00, 00, DateTimeKind.Utc), new DoubleValue(6));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 06, 00, 00, DateTimeKind.Utc), new DoubleValue(7));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 07, 00, 00, DateTimeKind.Utc), new DoubleValue(8));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 08, 00, 00, DateTimeKind.Utc), new DoubleValue(9));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 09, 00, 00, DateTimeKind.Utc), new DoubleValue(10));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 10, 00, 00, DateTimeKind.Utc), new DoubleValue(11));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 11, 00, 00, DateTimeKind.Utc), new DoubleValue(12));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 12, 00, 00, DateTimeKind.Utc), new DoubleValue(13));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 13, 00, 00, DateTimeKind.Utc), new DoubleValue(14));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 14, 00, 00, DateTimeKind.Utc), new DoubleValue(15));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 15, 00, 00, DateTimeKind.Utc), new DoubleValue(16));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 16, 00, 00, DateTimeKind.Utc), new DoubleValue(17));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 17, 00, 00, DateTimeKind.Utc), new DoubleValue(18));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 18, 00, 00, DateTimeKind.Utc), new DoubleValue(19));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 19, 00, 00, DateTimeKind.Utc), new DoubleValue(20));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 20, 00, 00, DateTimeKind.Utc), new DoubleValue(21));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 21, 00, 00, DateTimeKind.Utc), new DoubleValue(22));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 22, 00, 00, DateTimeKind.Utc), new DoubleValue(23));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 23, 00, 00, DateTimeKind.Utc), new DoubleValue(24));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc), new DoubleValue(25));

            // правильные запросы
            SymbolValue value = keeper.GetValue(CalcAggregation.First, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1));
            Assert.IsInstanceOf<DoubleValue>(value);
            Assert.AreEqual(2.0, value.GetValue());

            value = keeper.GetValue(CalcAggregation.Last, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1));
            Assert.IsInstanceOf<DoubleValue>(value);
            Assert.AreEqual(25.0, value.GetValue());

            value = keeper.GetValue(CalcAggregation.Minimum, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1));
            Assert.IsInstanceOf<DoubleValue>(value);
            Assert.AreEqual(2.0, value.GetValue());

            value = keeper.GetValue(CalcAggregation.Maximum, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1));
            Assert.IsInstanceOf<DoubleValue>(value);
            Assert.AreEqual(25.0, value.GetValue());

            value = keeper.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1));
            Assert.IsInstanceOf<DoubleValue>(value);
            Assert.AreEqual(324.0, value.GetValue());

            value = keeper.GetValue(CalcAggregation.Average, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1));
            Assert.IsInstanceOf<DoubleValue>(value);
            Assert.AreEqual(13.5, value.GetValue());
        }

        [Test]
        public void GetValues_HasSourceValuesAggregationIsWeighted_ReturnAggregatedValue()
        {
            //var calcSupplier = new Moq.Mock<ICalcSupplier>();

            //calcSupplier.Setup(s => s.Aggregate(
            //    Moq.It.IsAny<DateTime>(),
            //    Moq.It.IsAny<Interval>(),
            //    Moq.It.IsAny<Interval>(),
            //    Moq.It.IsAny<CalcAggregation>(),
            //    Moq.It.IsAny<List<ParamValueItem>[]>()))
            //    .Returns((Func<DateTime, Interval, Interval, CalcAggregation, List<ParamValueItem>[], ParamValueItem>)AggregateMethod);
            var calcSupplier = new AggregationCalcSupplier();

            ValuesKeeper keeper = new ValuesKeeper(calcSupplier);//.Object);

            // параметры
            ICalcNode calcNode1 = new TestCalcNode() { NodeID = 1, Name = "mambru" };
            ICalcNode calcNode2 = new TestCalcNode() { NodeID = 2, Name = "weight" };

            IParameterInfo parameter1Info = new TestParameterinfo(calcNode1) { Interval = Interval.Hour };
            IParameterInfo parameter2Info = new TestParameterinfo(calcNode2) { Interval = Interval.Hour };

            // добавляем значения
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DoubleValue(1));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 01, 00, 00, DateTimeKind.Utc), new DoubleValue(2));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 02, 00, 00, DateTimeKind.Utc), new DoubleValue(3));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 03, 00, 00, DateTimeKind.Utc), new DoubleValue(4));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 04, 00, 00, DateTimeKind.Utc), new DoubleValue(5));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 05, 00, 00, DateTimeKind.Utc), new DoubleValue(6));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 06, 00, 00, DateTimeKind.Utc), new DoubleValue(7));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 07, 00, 00, DateTimeKind.Utc), new DoubleValue(8));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 08, 00, 00, DateTimeKind.Utc), new DoubleValue(9));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 09, 00, 00, DateTimeKind.Utc), new DoubleValue(10));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 10, 00, 00, DateTimeKind.Utc), new DoubleValue(11));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 11, 00, 00, DateTimeKind.Utc), new DoubleValue(12));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 12, 00, 00, DateTimeKind.Utc), new DoubleValue(13));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 13, 00, 00, DateTimeKind.Utc), new DoubleValue(14));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 14, 00, 00, DateTimeKind.Utc), new DoubleValue(15));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 15, 00, 00, DateTimeKind.Utc), new DoubleValue(16));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 16, 00, 00, DateTimeKind.Utc), new DoubleValue(17));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 17, 00, 00, DateTimeKind.Utc), new DoubleValue(18));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 18, 00, 00, DateTimeKind.Utc), new DoubleValue(19));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 19, 00, 00, DateTimeKind.Utc), new DoubleValue(20));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 20, 00, 00, DateTimeKind.Utc), new DoubleValue(21));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 21, 00, 00, DateTimeKind.Utc), new DoubleValue(22));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 22, 00, 00, DateTimeKind.Utc), new DoubleValue(23));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 01, 23, 00, 00, DateTimeKind.Utc), new DoubleValue(24));
            keeper.AddRawValue(calcNode1, null, new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc), new DoubleValue(25));

            // веса
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DoubleValue(12));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 01, 00, 00, DateTimeKind.Utc), new DoubleValue(11));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 02, 00, 00, DateTimeKind.Utc), new DoubleValue(10));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 03, 00, 00, DateTimeKind.Utc), new DoubleValue(9));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 04, 00, 00, DateTimeKind.Utc), new DoubleValue(8));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 05, 00, 00, DateTimeKind.Utc), new DoubleValue(7));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 06, 00, 00, DateTimeKind.Utc), new DoubleValue(6));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 07, 00, 00, DateTimeKind.Utc), new DoubleValue(5));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 08, 00, 00, DateTimeKind.Utc), new DoubleValue(4));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 09, 00, 00, DateTimeKind.Utc), new DoubleValue(3));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 10, 00, 00, DateTimeKind.Utc), new DoubleValue(2));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 11, 00, 00, DateTimeKind.Utc), new DoubleValue(1));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 12, 00, 00, DateTimeKind.Utc), new DoubleValue(2));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 13, 00, 00, DateTimeKind.Utc), new DoubleValue(3));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 14, 00, 00, DateTimeKind.Utc), new DoubleValue(4));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 15, 00, 00, DateTimeKind.Utc), new DoubleValue(5));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 16, 00, 00, DateTimeKind.Utc), new DoubleValue(6));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 17, 00, 00, DateTimeKind.Utc), new DoubleValue(7));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 18, 00, 00, DateTimeKind.Utc), new DoubleValue(8));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 19, 00, 00, DateTimeKind.Utc), new DoubleValue(9));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 20, 00, 00, DateTimeKind.Utc), new DoubleValue(10));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 21, 00, 00, DateTimeKind.Utc), new DoubleValue(11));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 22, 00, 00, DateTimeKind.Utc), new DoubleValue(12));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 01, 23, 00, 00, DateTimeKind.Utc), new DoubleValue(13));
            keeper.AddRawValue(calcNode2, null, new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc), new DoubleValue(14));

            // правильные запросы
            SymbolValue value = keeper.GetValue(CalcAggregation.Weighted,
                                                new DateTime(2012, 01, 01,00,00,00, DateTimeKind.Utc), 
                                                Interval.Day, 
                                                new CalcNodeKey(calcNode1, null),
                                                new CalcNodeKey(calcNode2, null));

            Assert.IsInstanceOf<DoubleValue>(value);
            Assert.AreEqual(14.76470588, (double)value.GetValue(), 1e-5);
        }

        //[Test]
        //public void GetValues_HasSourceValuesWithNothing_ReturnNothing()
        //{
        //    //var calcSupplier = new Moq.Mock<ICalcSupplier>();

        //    //calcSupplier.Setup(s => s.Aggregate(
        //    //    Moq.It.IsAny<DateTime>(),
        //    //    Moq.It.IsAny<Interval>(),
        //    //    Moq.It.IsAny<Interval>(),
        //    //    Moq.It.IsAny<CalcAggregation>(),
        //    //    Moq.It.IsAny<List<ParamValueItem>[]>()))
        //    //    .Returns((Func<DateTime, Interval, Interval, CalcAggregation, List<ParamValueItem>[], ParamValueItem>)AggregateMethod);
        //    var calcSupplier = new AggregationCalcSupplier();

        //    ValuesKeeper keeper = new ValuesKeeper(calcSupplier);//.Object);

        //    // параметры
        //    ICalcNode calcNode = new TestCalcNode() { NodeID = 1, Name = "mambru" };

        //    IParameterInfo parameterInfo = new TestParameterinfo(calcNode) { Interval = Interval.Hour };

        //    // аргументы
        //    ArgumentsValues args1 = new ArgumentsValues(new { N = 1, X = 3 });

        //    // добавляем значения
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DoubleValue(1));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 01, 00, 00, DateTimeKind.Utc), new DoubleValue(2));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 02, 00, 00, DateTimeKind.Utc), new DoubleValue(3));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 03, 00, 00, DateTimeKind.Utc), new DoubleValue(4));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 04, 00, 00, DateTimeKind.Utc), new DoubleValue(5));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 05, 00, 00, DateTimeKind.Utc), new DoubleValue(6));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 06, 00, 00, DateTimeKind.Utc), new DoubleValue(7));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 07, 00, 00, DateTimeKind.Utc), new DoubleValue(8));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 08, 00, 00, DateTimeKind.Utc), new DoubleValue(9));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 09, 00, 00, DateTimeKind.Utc), new DoubleValue(10));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 10, 00, 00, DateTimeKind.Utc), new DoubleValue(11));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 11, 00, 00, DateTimeKind.Utc), new DoubleValue(12));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 12, 00, 00, DateTimeKind.Utc), new DoubleValue(13));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 13, 00, 00, DateTimeKind.Utc), new DoubleValue(14));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 14, 00, 00, DateTimeKind.Utc), new DoubleValue(15));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 15, 00, 00, DateTimeKind.Utc), SymbolValue.Nothing);
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 16, 00, 00, DateTimeKind.Utc), new DoubleValue(17));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 17, 00, 00, DateTimeKind.Utc), new DoubleValue(18));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 18, 00, 00, DateTimeKind.Utc), new DoubleValue(19));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 19, 00, 00, DateTimeKind.Utc), new DoubleValue(20));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 20, 00, 00, DateTimeKind.Utc), new DoubleValue(21));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 21, 00, 00, DateTimeKind.Utc), new DoubleValue(22));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 22, 00, 00, DateTimeKind.Utc), new DoubleValue(23));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 23, 00, 00, DateTimeKind.Utc), new DoubleValue(24));
        //    keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc), new DoubleValue(25));

        //    // правильные запросы
        //    Assert.AreSame(SymbolValue.Nothing, keeper.GetValue(CalcAggregation.First, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1)));
        //    Assert.AreSame(SymbolValue.Nothing, keeper.GetValue(CalcAggregation.Last, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1)));
        //    Assert.AreSame(SymbolValue.Nothing, keeper.GetValue(CalcAggregation.Minimum, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1)));
        //    Assert.AreSame(SymbolValue.Nothing, keeper.GetValue(CalcAggregation.Maximum, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1)));
        //    Assert.AreSame(SymbolValue.Nothing, keeper.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1)));
        //    Assert.AreSame(SymbolValue.Nothing, keeper.GetValue(CalcAggregation.Average, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1)));
        //    Assert.AreEqual(23, keeper.GetValue(CalcAggregation.Count, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1)).GetValue());
        //    Assert.AreEqual(0, keeper.GetValue(CalcAggregation.Exist, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1)).GetValue());
        //}

        [Test]
        public void GetValues_HasSourceValuesWithBlocked_ReturnBlocked()
        {
            //var calcSupplier = new Moq.Mock<ICalcSupplier>();

            //calcSupplier.Setup(s => s.Aggregate(
            //    Moq.It.IsAny<DateTime>(),
            //    Moq.It.IsAny<Interval>(),
            //    Moq.It.IsAny<Interval>(),
            //    Moq.It.IsAny<CalcAggregation>(),
            //    Moq.It.IsAny<List<ParamValueItem>[]>()))
            //    .Returns((Func<DateTime, Interval, Interval, CalcAggregation, List<ParamValueItem>[], ParamValueItem>)AggregateMethod);
            var calcSupplier = new AggregationCalcSupplier();

            ValuesKeeper keeper = new ValuesKeeper(calcSupplier);//.Object);

            // параметры
            ICalcNode calcNode = new TestCalcNode() { NodeID = 1, Name = "mambru" };

            IParameterInfo parameterInfo = new TestParameterinfo(calcNode) { Interval = Interval.Hour };

            // аргументы
            ArgumentsValues args1 = new ArgumentsValues(new { N = 1, X = 3 });

            // добавляем значения
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DoubleValue(1));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 01, 00, 00, DateTimeKind.Utc), new DoubleValue(2));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 02, 00, 00, DateTimeKind.Utc), new DoubleValue(3));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 03, 00, 00, DateTimeKind.Utc), new DoubleValue(4));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 04, 00, 00, DateTimeKind.Utc), new DoubleValue(5));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 05, 00, 00, DateTimeKind.Utc), new DoubleValue(6));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 06, 00, 00, DateTimeKind.Utc), new DoubleValue(7));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 07, 00, 00, DateTimeKind.Utc), new DoubleValue(8));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 08, 00, 00, DateTimeKind.Utc), new DoubleValue(9));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 09, 00, 00, DateTimeKind.Utc), new DoubleValue(10));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 10, 00, 00, DateTimeKind.Utc), new DoubleValue(11));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 11, 00, 00, DateTimeKind.Utc), new DoubleValue(12));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 12, 00, 00, DateTimeKind.Utc), new DoubleValue(13));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 13, 00, 00, DateTimeKind.Utc), new DoubleValue(14));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 14, 00, 00, DateTimeKind.Utc), new DoubleValue(15));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 15, 00, 00, DateTimeKind.Utc), SymbolValue.BlockedValue);
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 16, 00, 00, DateTimeKind.Utc), new DoubleValue(17));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 17, 00, 00, DateTimeKind.Utc), new DoubleValue(18));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 18, 00, 00, DateTimeKind.Utc), new DoubleValue(19));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 19, 00, 00, DateTimeKind.Utc), new DoubleValue(20));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 20, 00, 00, DateTimeKind.Utc), new DoubleValue(21));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 21, 00, 00, DateTimeKind.Utc), new DoubleValue(22));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 22, 00, 00, DateTimeKind.Utc), new DoubleValue(23));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 01, 23, 00, 00, DateTimeKind.Utc), new DoubleValue(24));
            keeper.AddRawValue(calcNode, args1, new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc), new DoubleValue(25));

            // правильные запросы
            Assert.AreSame(SymbolValue.BlockedValue, keeper.GetValue(CalcAggregation.First, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1)));
            Assert.AreSame(SymbolValue.BlockedValue, keeper.GetValue(CalcAggregation.Last, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1)));
            Assert.AreSame(SymbolValue.BlockedValue, keeper.GetValue(CalcAggregation.Minimum, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1)));
            Assert.AreSame(SymbolValue.BlockedValue, keeper.GetValue(CalcAggregation.Maximum, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1)));
            Assert.AreSame(SymbolValue.BlockedValue, keeper.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1)));
            Assert.AreSame(SymbolValue.BlockedValue, keeper.GetValue(CalcAggregation.Average, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1)));
            Assert.AreSame(SymbolValue.BlockedValue, keeper.GetValue(CalcAggregation.Count, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1)));
            Assert.AreSame(SymbolValue.BlockedValue, keeper.GetValue(CalcAggregation.Exist, new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), Interval.Day, new CalcNodeKey(calcNode, args1)));
        }

        [Test]
        public void GetOptimal__CoreleteSetOptimal()
        {
            // параметры
            ICalcNode calcNode1 = new TestCalcNode() { NodeID = 1, Name = "mambru" };
            ICalcNode calcNode2 = new TestCalcNode() { NodeID = 2, Name = "mambru" };
            ICalcNode calcNode3 = new TestCalcNode() { NodeID = 3, Name = "mambru" };

            IOptimizationInfo optimizationInfo1 = new TestOptimizationInfo(calcNode1)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument(){Name="a", Mode= OptimizationArgumentMode.Interval, IntervalFrom=1, IntervalTo=10, IntervalStep=1},
                    new TestOptimizationArgument(){Name="b", Mode= OptimizationArgumentMode.Interval, IntervalFrom=1, IntervalTo=10, IntervalStep=1},
                    new TestOptimizationArgument(){Name="c", Mode= OptimizationArgumentMode.Interval, IntervalFrom=1, IntervalTo=10, IntervalStep=1}
                }
            };
            IOptimizationInfo optimizationInfo2 = new TestOptimizationInfo(calcNode2)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument(){Name="N", Mode= OptimizationArgumentMode.Interval, IntervalFrom=1, IntervalTo=10, IntervalStep=1},
                    new TestOptimizationArgument(){Name="K", Mode= OptimizationArgumentMode.Interval, IntervalFrom=1, IntervalTo=10, IntervalStep=1},
                }
            };

            // аргументы для первой оптимизации
            ArgumentsValues args1_1 = new ArgumentsValues(new { a = 1, b = 3, c = 5 });
            ArgumentsValues args1_2 = new ArgumentsValues(new { a = 6, b = 9, c = 8 });
            ArgumentsValues args1_3 = new ArgumentsValues(new { a = 7, b = 4, c = 4 });

            // аргументы для второй оптимизации
            ArgumentsValues args2_1 = new ArgumentsValues(new { N = 3, K = 1 });
            ArgumentsValues args2_2 = new ArgumentsValues(new { N = 9, K = 5 });
            ArgumentsValues args2_3 = new ArgumentsValues(new { N = 4, K = 7 });

            ValuesKeeper keeper = new ValuesKeeper(null);

            keeper.SetOptimal(calcNode1, null, new DateTime(2012, 01, 01), args1_1);
            keeper.SetOptimal(calcNode2, null, new DateTime(2012, 01, 01), args2_1);

            keeper.SetOptimal(calcNode1, null, new DateTime(2012, 01, 02), args1_2);
            keeper.SetOptimal(calcNode2, null, new DateTime(2012, 01, 02), args2_2);

            keeper.SetOptimal(calcNode1, null, new DateTime(2012, 01, 03), args1_3);
            keeper.SetOptimal(calcNode2, null, new DateTime(2012, 01, 03), args2_3);

            Assert.AreEqual(new ArgumentsValues(new { a = 1, b = 3, c = 5 }), keeper.GetOptimal(calcNode1, null, new DateTime(2012, 01, 01)));
            Assert.AreEqual(new ArgumentsValues(new { a = 6, b = 9, c = 8 }), keeper.GetOptimal(calcNode1, null, new DateTime(2012, 01, 02)));
            Assert.AreEqual(new ArgumentsValues(new { a = 7, b = 4, c = 4 }), keeper.GetOptimal(calcNode1, null, new DateTime(2012, 01, 03)));

            Assert.AreEqual(new ArgumentsValues(new { N = 3, K = 1 }), keeper.GetOptimal(calcNode2, null, new DateTime(2012, 01, 01)));
            Assert.AreEqual(new ArgumentsValues(new { N = 9, K = 5 }), keeper.GetOptimal(calcNode2, null, new DateTime(2012, 01, 02)));
            Assert.AreEqual(new ArgumentsValues(new { N = 4, K = 7 }), keeper.GetOptimal(calcNode2, null, new DateTime(2012, 01, 03)));
        }

        [Test]
        public void GetValue_AfterSetFail_ReturnNothing()
        {
            ICalcNode optimizationNode = new TestCalcNode() { NodeID = 34 };
            ICalcNode param1Node = new TestCalcNode() { NodeID = 1 };
            ICalcNode param2Node = new TestCalcNode() { NodeID = 2 };
            ICalcNode param3Node = new TestCalcNode() { NodeID = 3 };

            IOptimizationInfo optimizationInfo = new TestOptimizationInfo(optimizationNode)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument()
                    {
                        Name = "a"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "b"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "c"
                    }
                }
            };

            IParameterInfo param1Info = new TestParameterinfo(param1Node)
            {
                Interval = Interval.Day,
                Calculable = true,
                Optimization = optimizationInfo
            };
            IParameterInfo param2Info = new TestParameterinfo(param2Node)
            {
                Interval = Interval.Day,
                Calculable = true,
                Optimization = optimizationInfo
            }; IParameterInfo param3Info = new TestParameterinfo(param3Node)
            {
                Interval = Interval.Day,
                Calculable = true,
                Optimization = optimizationInfo
            };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            var valueKeeper = new ValuesKeeper(calcSupplier.Object);

            valueKeeper.SetFailNode(param1Info, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            SymbolValue val1 = valueKeeper.GetValue(CalcAggregation.First, new DateTime(2012, 01, 03), Interval.Month, new CalcNodeKey(param1Node, null));
            SymbolValue val2 = valueKeeper.GetValue(CalcAggregation.First, new DateTime(2012, 01, 03), Interval.Month, new CalcNodeKey(param2Node, null));

            Assert.AreSame(SymbolValue.Nothing, val1);
            Assert.IsNull(val2);
        }

        [Test]
        public void GetOptimal_AfterSetFail_ReturnNothing()
        {
            ICalcNode optimizationNode = new TestCalcNode() { NodeID = 34 };
            ICalcNode param1Node = new TestCalcNode() { NodeID = 1 };
            ICalcNode param2Node = new TestCalcNode() { NodeID = 2 };
            ICalcNode param3Node = new TestCalcNode() { NodeID = 3 };

            IOptimizationInfo optimizationInfo = new TestOptimizationInfo(optimizationNode)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument()
                    {
                        Name = "a"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "b"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "c"
                    }
                }
            };

            IParameterInfo param1Info = new TestParameterinfo(param1Node)
            {
                Interval = Interval.Day,
                Calculable = true,
                Optimization = optimizationInfo
            };
            IParameterInfo param2Info = new TestParameterinfo(param2Node)
            {
                Interval = Interval.Day,
                Calculable = true,
                Optimization = optimizationInfo
            }; IParameterInfo param3Info = new TestParameterinfo(param3Node)
            {
                Interval = Interval.Day,
                Calculable = true,
                Optimization = optimizationInfo
            };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            var valueKeeper = new ValuesKeeper(calcSupplier.Object);

            valueKeeper.SetFailNode(optimizationInfo, new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            SymbolValue val1 = valueKeeper.GetValue(CalcAggregation.First, new DateTime(2012, 01, 03), Interval.Month, new CalcNodeKey(param1Node, new ArgumentsValues(new { a = 45, b = 16, c = 17 })));
            SymbolValue val2 = valueKeeper.GetValue(CalcAggregation.Sum, new DateTime(2012, 01, 03), Interval.Month, new CalcNodeKey(param2Node, new ArgumentsValues(new { a = 13, b = 42, c = 96 })));
            ArgumentsValues optimalArgs = valueKeeper.GetOptimal(optimizationNode, null, new DateTime(2012, 01, 03));

            Assert.AreSame(SymbolValue.Nothing, val1);
            Assert.AreSame(SymbolValue.Nothing, val1);
            // Должен возвращать не null, а BadArguments
            Assert.AreSame(ArgumentsValues.BadArguments, optimalArgs);
        }

        [Test]
        public void GetOptimal_MultiLevelOptimization_ReturnNearestOptimalArgsOrParameter()
        {
            ICalcNode optimization1 = new TestCalcNode() { NodeID = 34 };
            ICalcNode optimization2 = new TestCalcNode() { NodeID = 35 };
            ICalcNode optimization3 = new TestCalcNode() { NodeID = 36 };

            var optimization1Info = new TestOptimizationInfo(optimization1)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument()
                    {
                        Name = "a"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "b"
                    }
                }
            };
            var optimization2Info = new TestOptimizationInfo(optimization2)
            {
                Interval = Interval.Day,
                Optimization = optimization1Info,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument()
                    {
                        Name = "c"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "d"
                    }
                }
            };
            var optimization3Info = new TestOptimizationInfo(optimization3)
            {
                Interval = Interval.Day,
                Optimization = optimization2Info,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument()
                    {
                        Name = "e"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "f"
                    }
                }
            };

            ArgumentsValues args;

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            var valuesKeeper = new ValuesKeeper(calcSupplier.Object);

            // при запросе оптимальных аргументов для 3-ей оптимизации, 
            // есть только оптимальные аргументы 2-ой
            // Вернуть null
            valuesKeeper.SetOptimal(
                optimization2,
                new ArgumentsValues(new { a = 10, b = 12 }),
                new DateTime(2012, 01, 01),
                new ArgumentsValues(new { a = 10, b = 12, c = 14, d = 16 }));

            args = valuesKeeper.GetOptimal(optimization3, new ArgumentsValues(new { a = 10, b = 12 }), new DateTime(2012, 01, 01));
            Assert.IsNull(args);
            
            // если добавить оптимальное решения для 3-ей оптимизации, то вернет его
            valuesKeeper.SetOptimal(
                optimization3,
                new ArgumentsValues(new { a = 10, b = 12, c = 14, d = 16 }),
                new DateTime(2012, 01, 01),
                new ArgumentsValues(new { a = 10, b = 12, c = 14, d = 16, e = 1, f = 42 }));

            args = valuesKeeper.GetOptimal(optimization3, new ArgumentsValues(new { a = 10, b = 12 }), new DateTime(2012, 01, 01));
            Assert.IsNotNull(args);
            Assert.AreEqual(new ArgumentsValues(new { a = 10, b = 12, c = 14, d = 16, e = 1, f = 42 }), args);

            // если нет никаких оптимальных аргументов, вернуть переданные базовые аргументы
            args = valuesKeeper.GetOptimal(optimization3, new ArgumentsValues(new { a = 12, b = 12 }), new DateTime(2012, 01, 01));
            Assert.IsNull(args);

        }

        [Test]
        public void GetAllCalculatedValues_AfterSetFail_ReturnEmptyCPackageAndFillNothing()
        {
            ICalcNode optimizationNode = new TestCalcNode() { NodeID = 34 };
            ICalcNode param1Node = new TestCalcNode() { NodeID = 1 };
            ICalcNode param2Node = new TestCalcNode() { NodeID = 2 };
            ICalcNode param3Node = new TestCalcNode() { NodeID = 3 };

            IOptimizationInfo optimizationInfo = new TestOptimizationInfo(optimizationNode)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument()
                    {
                        Name = "a"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "b"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "c"
                    }
                }
            };

            IParameterInfo param1Info = new TestParameterinfo(param1Node)
            {
                Interval = Interval.Day,
                Calculable = true,
                Optimization = optimizationInfo
            };
            IParameterInfo param2Info = new TestParameterinfo(param2Node)
            {
                Interval = Interval.Day,
                Calculable = true,
                Optimization = optimizationInfo
            }; IParameterInfo param3Info = new TestParameterinfo(param3Node)
            {
                Interval = Interval.Day,
                Calculable = true,
                Optimization = optimizationInfo
            };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();
            calcSupplier.Setup(c =>
                c.GetRevisions(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>()))
                .Returns(new RevisionInfo[] { RevisionInfo.Default });

            var valuesKeeper = new ValuesKeeper(calcSupplier.Object);

            valuesKeeper.SetFailNode(
                optimizationInfo, 
                new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc),
                new DateTime(2012, 02, 01, 00, 00, 00, DateTimeKind.Utc));

            var packages=valuesKeeper.GetAllCalculatedValues();

            Assert.IsNotNull(packages);
            Assert.AreEqual(4, packages.Length);

            Package package;

            foreach (int paramId in new int[] { 34, 1, 2, 3 })
            {
                package = (from p in packages where p.Id == paramId select p).FirstOrDefault();

                Assert.IsNotNull(package);

                //Assert.AreEqual(34, package.Id);
                Assert.AreEqual(31, package.Values.Count);
                for (int i = 0; i < package.Values.Count; i++)
                {
                    ParamValueItem valueItem = package.Values[i];

                    Assert.AreEqual(Quality.Bad, valueItem.Quality);
                    Assert.IsNaN(valueItem.Value);
                    Assert.AreSame(ArgumentsValues.BadArguments, valueItem.Arguments);
                    Assert.AreEqual(new DateTime(2012, 01, i + 1), valueItem.Time);
                }
            }
        }

        [Test]
        public void GetAllCalculatedValues_NodesWithBadsWithBaseArguments_SkipOrAddBadArgumentsIfPackageIsEmpty()
        {
            ICalcNode optimization1 = new TestCalcNode()
            {
                Name = "opt1",
                NodeID = 34
            };
            ICalcNode optimization2 = new TestCalcNode()
            {
                Name = "opt2",
                NodeID = 35
            };

            IOptimizationInfo optimization1Info = new TestOptimizationInfo(optimization1)
            {
                Interval = Interval.Day,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument()
                    {
                        Name = "a"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "b"
                    }
                }
            };
            IOptimizationInfo optimization2Info = new TestOptimizationInfo(optimization2)
            {
                Interval = Interval.Day,
                Optimization = optimization1Info,
                Arguments = new IOptimizationArgument[]
                {
                    new TestOptimizationArgument()
                    {
                        Name = "c"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "d"
                    }
                }
            };

            ICalcNode calcNode = new TestCalcNode() { Name = "mamburu", NodeID = 32 };
            IParameterInfo parameterInfo = new TestParameterinfo(calcNode)
            {
                Calculable = true,
                Code = "par1",
                Optimization = optimization2Info,
                Interval = Interval.Day,
                Formula = "32*53;"
            };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();
            calcSupplier.Setup(
                s => s.GetRevisions(
                    Moq.It.IsAny<DateTime>(), 
                    Moq.It.IsAny<DateTime>()))
                .Returns(new RevisionInfo[] { RevisionInfo.Default });
            calcSupplier.Setup(
                s => s.GetRevision(
                    Moq.It.IsAny<DateTime>()))
                .Returns(RevisionInfo.Default);

            var valueKeeper = new ValuesKeeper(calcSupplier.Object);

            // Отмечаем базовую оптимизацию как не имеющую решения
            valueKeeper.SetOptimal(optimization2, new ArgumentsValues(new { a = 10, b = 12 }), new DateTime(2012, 01, 01), ArgumentsValues.BadArguments);
            valueKeeper.SetOptimal(optimization2, new ArgumentsValues(new { a = 10, b = 12 }), new DateTime(2012, 01, 02), ArgumentsValues.BadArguments);
            valueKeeper.SetOptimal(optimization2, new ArgumentsValues(new { a = 12, b = 12 }), new DateTime(2012, 01, 02), ArgumentsValues.BadArguments);

            // правомерен вопрос кто должен отвечать за привидение аргументов:
            // 1. сам расчёт (как сейчас)
            // 2. ValueReceiver при сохранении/загрузки значений (потенциально параметры при сохранении/загрузки будут отличаться)
            // 3. AssignmentCalcSupplier - как компромисное промежуточное звено

            // за 1-ое число при optimization2 с аргументами a = 10, b = 12 не может быть расчитана
            // а с a = 12, b = 12 всё нормально. ожидаются пропуск бэда в конечной пачке
            valueKeeper.AddCalculatedValue(calcNode, new ArgumentsValues(new { a = 10, b = 12 }), new DateTime(2012, 01, 01), SymbolValue.Nothing);
            valueKeeper.AddCalculatedValue(calcNode, new ArgumentsValues(new { a = 12, b = 12, c = 14, d = 15 }), new DateTime(2012, 01, 01), SymbolValue.CreateValue(42));

            // за 2-ое число оптимизация не расчитывается ни при каких обстаятельствах
            // ожидается одно значение с BadArguments
            valueKeeper.AddCalculatedValue(calcNode, new ArgumentsValues(new { a = 10, b = 12 }), new DateTime(2012, 01, 02), SymbolValue.Nothing);
            valueKeeper.AddCalculatedValue(calcNode, new ArgumentsValues(new { a = 12, b = 12 }), new DateTime(2012, 01, 02), SymbolValue.Nothing);

            var packages= valueKeeper.GetAllCalculatedValues();

            Package pack = (from p in packages where p.Id == calcNode.NodeID select p).FirstOrDefault();

            Assert.IsNotNull(pack);

            Assert.AreEqual(2, pack.Values.Count);

            // проверка значений за 1-ое число
            ParamValueItem valueItem = pack.Values[0];
            Assert.AreEqual(new DateTime(2012, 01, 01), valueItem.Time);
            Assert.AreEqual(new ArgumentsValues(new { a = 12, b = 12, c = 14, d = 15 }), valueItem.Arguments);
            Assert.AreEqual(Quality.Good, valueItem.Quality);
            Assert.AreEqual(42.0, valueItem.Value);

            // проверка значений за 1-ое число
            valueItem = pack.Values[1];
            Assert.AreEqual(new DateTime(2012, 01, 02), valueItem.Time);
            Assert.AreSame(ArgumentsValues.BadArguments, valueItem.Arguments);
            Assert.AreEqual(Quality.Bad, valueItem.Quality);
            Assert.IsNaN(valueItem.Value);
        }

        [Test]
        public void IsCalculates__ReturnTrueIfValueAddedByAddCalculatedValues()
        {
            var calcNode1 = new TestCalcNode() { NodeID = 1 };
            var calcNode2 = new TestCalcNode() { NodeID = 2 };

            var optim1 = new TestCalcNode() { NodeID = 32 };

            var optimInfo1 = new TestOptimizationInfo(optim1)
            {
                Interval=Interval.Day,
                Arguments=new IOptimizationArgument[]
                {
                    new TestOptimizationArgument()
                    {
                        Name = "a"
                    },
                    new TestOptimizationArgument()
                    {
                        Name = "b"
                    }
                }
            };

            var nodeInfo1 = new TestParameterinfo(calcNode1)
            {
                Interval = Interval.Day,
                Optimization = optimInfo1
            };
            var nodeInfo2 = new TestParameterinfo(calcNode2)
            {
                Interval = Interval.Day,
                Optimization = optimInfo1
            };

            var calcSupplier = new Moq.Mock<ICalcSupplier>();

            var valuesKeeper = new ValuesKeeper(calcSupplier.Object);

            // добавляем расчитанные значения
            valuesKeeper.AddCalculatedValue(calcNode1, new ArgumentsValues(new { a = 10, b = 12 }), new DateTime(2012, 01, 01), SymbolValue.CreateValue(30));
            valuesKeeper.AddCalculatedValue(calcNode1, new ArgumentsValues(new { a = 10, b = 12 }), new DateTime(2012, 01, 02), SymbolValue.Nothing);
            valuesKeeper.AddCalculatedValue(calcNode2, new ArgumentsValues(new { a = 10, b = 12 }), new DateTime(2012, 01, 01), SymbolValue.CreateValue(15));
            valuesKeeper.AddCalculatedValue(calcNode2, new ArgumentsValues(new { a = 12, b = 12 }), new DateTime(2012, 01, 01), SymbolValue.CreateValue(52));

            // добавляем исходные значения
            valuesKeeper.AddRawValue(calcNode1, new ArgumentsValues(new { a = 14, b = 12 }), new DateTime(2012, 01, 01), SymbolValue.CreateValue(13));
            valuesKeeper.AddRawValue(calcNode1, new ArgumentsValues(new { a = 14, b = 12 }), new DateTime(2012, 01, 02), SymbolValue.CreateValue(42));
            valuesKeeper.AddRawValue(calcNode2, new ArgumentsValues(new { a = 14, b = 12 }), new DateTime(2012, 01, 01), SymbolValue.Nothing);
            valuesKeeper.AddRawValue(calcNode2, new ArgumentsValues(new { a = 12, b = 12 }), new DateTime(2012, 01, 02), SymbolValue.CreateValue(12));

            // для расчитанные значения должен вернуть true
            Assert.IsTrue(valuesKeeper.IsCalculated(calcNode1, new ArgumentsValues(new { a = 10, b = 12 }), new DateTime(2012, 01, 01)));
            Assert.IsTrue(valuesKeeper.IsCalculated(calcNode1, new ArgumentsValues(new { a = 10, b = 12 }), new DateTime(2012, 01, 02)));
            Assert.IsTrue(valuesKeeper.IsCalculated(calcNode2, new ArgumentsValues(new { a = 10, b = 12 }), new DateTime(2012, 01, 01)));
            Assert.IsTrue(valuesKeeper.IsCalculated(calcNode2, new ArgumentsValues(new { a = 12, b = 12 }), new DateTime(2012, 01, 01)));

            // для исходных значений должен вернуть false
            Assert.IsFalse(valuesKeeper.IsCalculated(calcNode1, new ArgumentsValues(new { a = 14, b = 12 }), new DateTime(2012, 01, 01)));
            Assert.IsFalse(valuesKeeper.IsCalculated(calcNode1, new ArgumentsValues(new { a = 14, b = 12 }), new DateTime(2012, 01, 02)));
            Assert.IsFalse(valuesKeeper.IsCalculated(calcNode2, new ArgumentsValues(new { a = 14, b = 12 }), new DateTime(2012, 01, 01)));
            Assert.IsFalse(valuesKeeper.IsCalculated(calcNode2, new ArgumentsValues(new { a = 12, b = 12 }), new DateTime(2012, 01, 02)));

            // если нет значений совсем нет должен вернуть false
            Assert.IsFalse(valuesKeeper.IsCalculated(calcNode1, new ArgumentsValues(new { a = 14, b = 10 }), new DateTime(2012, 01, 01)));
            Assert.IsFalse(valuesKeeper.IsCalculated(calcNode1, new ArgumentsValues(new { a = 14, b = 10 }), new DateTime(2012, 01, 02)));
            Assert.IsFalse(valuesKeeper.IsCalculated(calcNode2, new ArgumentsValues(new { a = 14, b = 10 }), new DateTime(2012, 01, 01)));
            Assert.IsFalse(valuesKeeper.IsCalculated(calcNode2, new ArgumentsValues(new { a = 12, b = 10 }), new DateTime(2012, 01, 02)));
        }
    }
}
