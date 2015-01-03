using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace COTES.ISTOK.Tests
{
    [TestFixture]
    public class ValueAggregatorTests
    {
        [Test]
        public void GetSourceRange__CorrectReturn()
        {
            DateTime startTime, endTime;
            ValueAggregator aggregator = new ValueAggregator();

            // агрегация месяца из суток
            startTime = new DateTime(2012, 03, 01, 00, 00, 00, DateTimeKind.Utc);
            endTime = new DateTime(2012, 04, 01, 00, 00, 00, DateTimeKind.Utc);
            aggregator.GetSourceRange(Interval.Day, Interval.Month, ref startTime, ref endTime);

            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 00, 00), startTime, "месяц из суток");
            Assert.AreEqual(new DateTime(2012, 04, 01, 00, 00, 00), endTime, "месяц из суток");

            // агрегация месяца из часов
            startTime = new DateTime(2012, 03, 01, 00, 00, 00, DateTimeKind.Utc);
            endTime = new DateTime(2012, 04, 01, 00, 00, 00, DateTimeKind.Utc);
            aggregator.GetSourceRange(Interval.Hour, Interval.Month, ref startTime, ref endTime);

            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 00, 00).AddMilliseconds(1), startTime, "месяц из часов");
            Assert.AreEqual(new DateTime(2012, 04, 01, 00, 00, 00).AddMilliseconds(1), endTime, "месяц из часов");

            // агрегация месяца из минут
            startTime = new DateTime(2012, 03, 01, 00, 00, 00, DateTimeKind.Utc);
            endTime = new DateTime(2012, 04, 01, 00, 00, 00, DateTimeKind.Utc);
            aggregator.GetSourceRange(Interval.Minute, Interval.Month, ref startTime, ref endTime);

            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 00, 00).AddMilliseconds(1), startTime, "месяц из минут");
            Assert.AreEqual(new DateTime(2012, 04, 01, 00, 00, 00).AddMilliseconds(1), endTime, "месяц из минут");

            // агрегация месяца из секунд
            startTime = new DateTime(2012, 03, 01, 00, 00, 00, DateTimeKind.Utc);
            endTime = new DateTime(2012, 04, 01, 00, 00, 00, DateTimeKind.Utc);
            aggregator.GetSourceRange(Interval.Second, Interval.Month, ref startTime, ref endTime);

            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 00, 00).AddMilliseconds(1), startTime, "месяц из секунд");
            Assert.AreEqual(new DateTime(2012, 04, 01, 00, 00, 00).AddMilliseconds(1), endTime, "месяц из секунд");

            // агрегация суток из часов
            startTime = new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc);
            endTime = new DateTime(2012, 01, 03, 00, 00, 00, DateTimeKind.Utc);
            aggregator.GetSourceRange(Interval.Hour, Interval.Day, ref startTime, ref endTime);

            Assert.AreEqual(new DateTime(2012, 01, 02, 00, 00, 00).AddMilliseconds(1), startTime, "сутки из часов");
            Assert.AreEqual(new DateTime(2012, 01, 03, 00, 00, 00).AddMilliseconds(1), endTime, "сутки из часов");

            // агрегация суток из минут
            startTime = new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc);
            endTime = new DateTime(2012, 01, 03, 00, 00, 00, DateTimeKind.Utc);
            aggregator.GetSourceRange(Interval.Minute, Interval.Day, ref startTime, ref endTime);

            Assert.AreEqual(new DateTime(2012, 01, 02, 00, 00, 00).AddMilliseconds(1), startTime, "сутки из минут");
            Assert.AreEqual(new DateTime(2012, 01, 03, 00, 00, 00).AddMilliseconds(1), endTime, "сутки из минут");

            // агрегация суток из секунд
            startTime = new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc);
            endTime = new DateTime(2012, 01, 03, 00, 00, 00, DateTimeKind.Utc);
            aggregator.GetSourceRange(Interval.Second, Interval.Day, ref startTime, ref endTime);

            Assert.AreEqual(new DateTime(2012, 01, 02, 00, 00, 00).AddMilliseconds(1), startTime, "сутки из секунд");
            Assert.AreEqual(new DateTime(2012, 01, 03, 00, 00, 00).AddMilliseconds(1), endTime, "сутки из секунд");

            // агрегация суток из исходных данных
            startTime = new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc);
            endTime = new DateTime(2012, 01, 03, 00, 00, 00, DateTimeKind.Utc);
            aggregator.GetSourceRange(Interval.Zero, Interval.Day, ref startTime, ref endTime);

            Assert.AreEqual(new DateTime(2012, 01, 02, 00, 00, 00).AddMilliseconds(1), startTime, "сутки из исходных данных");
            Assert.AreEqual(new DateTime(2012, 01, 03, 00, 00, 00).AddMilliseconds(1), endTime, "сутки из исходных данных");

            // агрегация часа из минут
            startTime = new DateTime(2012, 03, 01, 01, 00, 00);
            endTime = new DateTime(2012, 03, 01, 04, 00, 00);
            aggregator.GetSourceRange(Interval.Minute, Interval.Hour, ref startTime, ref endTime);

            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 00, 00).AddMilliseconds(1), startTime, "час из минут");
            Assert.AreEqual(new DateTime(2012, 03, 01, 03, 00, 00).AddMilliseconds(1), endTime, "час из минут");

            // агрегация часа из секунд
            startTime = new DateTime(2012, 03, 01, 01, 00, 00);
            endTime = new DateTime(2012, 03, 01, 04, 00, 00);
            aggregator.GetSourceRange(Interval.Second, Interval.Hour, ref startTime, ref endTime);

            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 00, 00).AddMilliseconds(1), startTime, "час из секунд");
            Assert.AreEqual(new DateTime(2012, 03, 01, 03, 00, 00).AddMilliseconds(1), endTime, "час из секунд");

            // агрегация минуты из секунд
            startTime = new DateTime(2012, 03, 01, 01, 15, 00);
            endTime = new DateTime(2012, 03, 01, 04, 32, 00);
            aggregator.GetSourceRange(Interval.Second, Interval.Minute, ref startTime, ref endTime);

            Assert.AreEqual(new DateTime(2012, 03, 01, 01, 14, 00).AddMilliseconds(1), startTime, "минута из секунд");
            Assert.AreEqual(new DateTime(2012, 03, 01, 04, 31, 00).AddMilliseconds(1), endTime, "минута из секунд");
        }

        [Test]
        public void GetRange__CorrectReturn()
        {
            DateTime time1, time2;
            ValueAggregator aggregator = new ValueAggregator();

            // агрегация месяца из суток
            time1 = new DateTime(2012, 03, 01, 00, 00, 00, DateTimeKind.Utc);
            time1 = aggregator.GetRange(time1, Interval.Day, Interval.Month);

            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 00, 00), time1, "месяц из суток");

            // агрегация месяца из часов
            time1 = new DateTime(2012, 03, 01, 00, 00, 00, DateTimeKind.Utc);
            time2 = new DateTime(2012, 03, 01, 01, 00, 00, DateTimeKind.Utc);

            time1 = aggregator.GetRange(time1, Interval.Hour, Interval.Month);
            time2 = aggregator.GetRange(time2, Interval.Hour, Interval.Month);

            Assert.AreEqual(new DateTime(2012, 02, 01, 00, 00, 00), time1, "месяц из часов");
            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 00, 00), time2, "месяц из часов");

            // агрегация месяца из минут
            time1 = new DateTime(2012, 03, 01, 00, 00, 00, DateTimeKind.Utc);
            time2 = new DateTime(2012, 03, 01, 00, 01, 00, DateTimeKind.Utc);

            time1 = aggregator.GetRange(time1, Interval.Minute, Interval.Month);
            time2 = aggregator.GetRange(time2, Interval.Minute, Interval.Month);

            Assert.AreEqual(new DateTime(2012, 02, 01, 00, 00, 00), time1, "месяц из минут");
            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 00, 00), time2, "месяц из минут");

            // агрегация месяца из секунд
            time1 = new DateTime(2012, 03, 01, 00, 00, 00, DateTimeKind.Utc);
            time2 = new DateTime(2012, 03, 01, 00, 00, 01, DateTimeKind.Utc);

            time1 = aggregator.GetRange(time1, Interval.Second, Interval.Month);
            time2 = aggregator.GetRange(time2, Interval.Second, Interval.Month);

            Assert.AreEqual(new DateTime(2012, 02, 01, 00, 00, 00), time1, "месяц из секунд");
            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 00, 00), time2, "месяц из секунд");

            // агрегация суток из часов
            time1 = new DateTime(2012, 03, 01, 00, 00, 00, DateTimeKind.Utc);
            time2 = new DateTime(2012, 03, 01, 01, 00, 00, DateTimeKind.Utc);

            time1 = aggregator.GetRange(time1, Interval.Hour, Interval.Day);
            time2 = aggregator.GetRange(time2, Interval.Hour, Interval.Day);

            Assert.AreEqual(new DateTime(2012, 02, 29, 00, 00, 00), time1, "сутки из часов");
            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 00, 00), time2, "сутки из часов");

            // агрегация суток из минут
            time1 = new DateTime(2012, 03, 01, 00, 00, 00, DateTimeKind.Utc);
            time2 = new DateTime(2012, 03, 01, 00, 01, 00, DateTimeKind.Utc);

            time1 = aggregator.GetRange(time1, Interval.Minute, Interval.Day);
            time2 = aggregator.GetRange(time2, Interval.Minute, Interval.Day);

            Assert.AreEqual(new DateTime(2012, 02, 29, 00, 00, 00), time1, "сутки из минут");
            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 00, 00), time2, "сутки из минут");

            // агрегация суток из секунд
            time1 = new DateTime(2012, 03, 01, 00, 00, 00, DateTimeKind.Utc);
            time2 = new DateTime(2012, 03, 01, 00, 00, 01, DateTimeKind.Utc);

            time1 = aggregator.GetRange(time1, Interval.Second, Interval.Day);
            time2 = aggregator.GetRange(time2, Interval.Second, Interval.Day);

            Assert.AreEqual(new DateTime(2012, 02, 29, 00, 00, 00), time1, "сутки из секунд");
            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 00, 00), time2, "сутки из секунд");

            // агрегация суток из исходных данных
            time1 = new DateTime(2012, 03, 01, 00, 00, 00, DateTimeKind.Utc);
            time2 = new DateTime(2012, 03, 01, 00, 00, 00, DateTimeKind.Utc).AddMilliseconds(1);

            time1 = aggregator.GetRange(time1, Interval.Zero, Interval.Day);
            time2 = aggregator.GetRange(time2, Interval.Zero, Interval.Day);

            Assert.AreEqual(new DateTime(2012, 02, 29, 00, 00, 00), time1, "сутки из исходных данных");
            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 00, 00), time2, "сутки из исходных данных");

            // агрегация часа из минут
            time1 = new DateTime(2012, 03, 01, 00, 00, 00, DateTimeKind.Utc);
            time2 = new DateTime(2012, 03, 01, 00, 01, 00, DateTimeKind.Utc);

            time1 = aggregator.GetRange(time1, Interval.Minute, Interval.Hour);
            time2 = aggregator.GetRange(time2, Interval.Minute, Interval.Hour);

            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 00, 00), time1, "час из минут");
            Assert.AreEqual(new DateTime(2012, 03, 01, 01, 00, 00), time2, "час из минут");

            // агрегация часа из секунд
            time1 = new DateTime(2012, 03, 01, 00, 00, 00, DateTimeKind.Utc);
            time2 = new DateTime(2012, 03, 01, 00, 00, 01, DateTimeKind.Utc);

            time1 = aggregator.GetRange(time1, Interval.Second, Interval.Hour);
            time2 = aggregator.GetRange(time2, Interval.Second, Interval.Hour);

            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 00, 00), time1, "час из секунд");
            Assert.AreEqual(new DateTime(2012, 03, 01, 01, 00, 00), time2, "час из секунд");

            // агрегация минуты из секунд
            time1 = new DateTime(2012, 03, 01, 00, 00, 00, DateTimeKind.Utc);
            time2 = new DateTime(2012, 03, 01, 00, 00, 01, DateTimeKind.Utc);

            time1 = aggregator.GetRange(time1, Interval.Second, Interval.Minute);
            time2 = aggregator.GetRange(time2, Interval.Second, Interval.Minute);

            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 00, 00), time1, "минута из секунд");
            Assert.AreEqual(new DateTime(2012, 03, 01, 00, 01, 00), time2, "минута из секунд");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException))]
        public void Aggregate_OneParameter_ValuesForTwoParameters_ThrowException()
        {
            const String valuesType = "";

            ValueAggregator aggregator = new ValueAggregator();

            IEnumerable<ParamValueItem> aggregateValues;

            var generator = new ValueAggregatorTestDataGenerator();
            var parameterValues = generator.GetSourceData(0, Interval.Minute, valuesType);
            var weightValues = generator.GetSourceData(1, Interval.Minute, valuesType);

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Minute, Interval.Hour, parameterValues, weightValues);

            // throw exception here
            aggregateValues.ToArray();
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException))]
        public void Aggregate_TwoParameter_ValuesForOneParameters_ThrowException()
        {
            const String valuesType = "";

            ValueAggregator aggregator = new ValueAggregator();

            IEnumerable<ParamValueItem> aggregateValues;

            var generator = new ValueAggregatorTestDataGenerator();
            var parameterValues = generator.GetSourceData(0, Interval.Minute, valuesType);

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Minute, Interval.Hour, parameterValues);

            // throw exception here
            aggregateValues.ToArray();
        }

        private void AssertValues(IEnumerable<ParamValueItem> expected, IEnumerable<ParamValueItem> values, String message)
        {
            const double delta = 1e-7;

            ParamValueItem[] expectedArray = expected.ToArray();
            ParamValueItem[] valuesArray = values.ToArray();

            Assert.AreEqual(expectedArray.Length, valuesArray.Length, message);

            for (int i = 0; i < expectedArray.Length; i++)
            {
                String cycleMessage = message + String.Format("\n  Expected: {0} - {1}\n  But Was:  {2} - {3}",
                                                                expectedArray[i].Time, expectedArray[i].Value,
                                                                valuesArray[i].Time, valuesArray[i].Value);

                Assert.AreEqual(expectedArray[i].Time, valuesArray[i].Time, cycleMessage);
                Assert.AreEqual(expectedArray[i].Quality, valuesArray[i].Quality, cycleMessage);
                if (double.IsNaN(expectedArray[i].Value))
                    Assert.IsNaN(valuesArray[i].Value, cycleMessage);
                else
                {
                    double d = delta;
                    double v = expectedArray[i].Value;
                    while (v > 1)
                    {
                        v /= 10;
                        d *= 10;
                    }
                    Assert.AreEqual(expectedArray[i].Value, valuesArray[i].Value, d, cycleMessage);//, delta, message);
                }
            }
        }

        [Test]
        public void Aggregate_FromRaw_ReturnAggregatedValues()
        {
            const String valuesType = "";

            ValueAggregator aggregator = new ValueAggregator();

            IEnumerable<ParamValueItem> aggregateValues;
            IEnumerable<ParamValueItem> expectedValues;

            var generator = new ValueAggregatorTestDataGenerator();
            var parameterValues = generator.GetSourceData(0, Interval.Zero, valuesType);
            var weightValues = generator.GetSourceData(1, Interval.Zero, valuesType);

            // get minute
            const String getMinuteMessageFormat = "Minute ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Zero, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Zero, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Zero, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Zero, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Zero, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Zero, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Zero, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Zero, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Zero, Interval.Minute, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Weighted));

            // get hour
            const String getHourMessageFormat = "Hour ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Zero, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Zero, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Zero, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Zero, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Zero, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Zero, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Zero, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Zero, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Zero, Interval.Hour, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Weighted));

            //// get day
            //const String getDayMessageFormat = "Day ({0})";

            //// First
            //aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Zero, Interval.Day, parameterValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.First);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.First));

            //// Last
            //aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Zero, Interval.Day, parameterValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.Last);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Last));

            //// Maximum
            //aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Zero, Interval.Day, parameterValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.Maximum);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Maximum));

            //// Minimum
            //aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Zero, Interval.Day, parameterValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.Minimum);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Minimum));

            //// Sum
            //aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Zero, Interval.Day, parameterValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.Sum);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Sum));

            //// Average
            //aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Zero, Interval.Day, parameterValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.Average);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Average));

            //// Count
            //aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Zero, Interval.Day, parameterValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.Count);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Count));

            //// Exist
            //aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Zero, Interval.Day, parameterValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.Exist);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Exist));

            //// Weight
            //aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Zero, Interval.Day, parameterValues, weightValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.Weighted);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Weighted));
        }

        [Test]
        public void Aggregate_FromRawWithBads_ReturnAggregatedValues()
        {
            const String valuesType = "WithBads";

            ValueAggregator aggregator = new ValueAggregator();

            IEnumerable<ParamValueItem> aggregateValues;
            IEnumerable<ParamValueItem> expectedValues;

            var generator = new ValueAggregatorTestDataGenerator();
            var parameterValues = generator.GetSourceData(0, Interval.Zero, valuesType);
            var weightValues = generator.GetSourceData(1, Interval.Zero, valuesType);

            // get minute
            const String getMinuteMessageFormat = "Minute ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Zero, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Zero, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Zero, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Zero, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Zero, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Zero, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Zero, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Zero, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Zero, Interval.Minute, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Minute, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Weighted));

            // get hour
            const String getHourMessageFormat = "Hour ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Zero, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Zero, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Zero, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Zero, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Zero, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Zero, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Zero, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Zero, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Zero, Interval.Hour, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Hour, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Weighted));

            //// get day
            //const String getDayMessageFormat = "Day ({0})";

            //// First
            //aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Zero, Interval.Day, parameterValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.First);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.First));

            //// Last
            //aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Zero, Interval.Day, parameterValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.Last);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Last));

            //// Maximum
            //aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Zero, Interval.Day, parameterValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.Maximum);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Maximum));

            //// Minimum
            //aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Zero, Interval.Day, parameterValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.Minimum);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Minimum));

            //// Sum
            //aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Zero, Interval.Day, parameterValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.Sum);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Sum));

            //// Average
            //aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Zero, Interval.Day, parameterValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.Average);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Average));

            //// Count
            //aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Zero, Interval.Day, parameterValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.Count);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Count));

            //// Exist
            //aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Zero, Interval.Day, parameterValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.Exist);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Exist));

            //// Weight
            //aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Zero, Interval.Day, parameterValues, weightValues);

            //expectedValues = generator.GetAggregateData(Interval.Zero, valuesType, Interval.Day, CalcAggregation.Weighted);
            //AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Weighted));    
        }

        [Test]
        public void Aggregate_FromRawSkipEntireInterval_ReturnAggregatedValues()
        {
            ValueAggregator aggregator = new ValueAggregator();

            IEnumerable<ParamValueItem> aggregateValues;
            IEnumerable<ParamValueItem> expectedValues;

            var parameterValues = new ParamValueItem[]
            {
                new ParamValueItem(new DateTime(2012,01,01,00,00,00, DateTimeKind.Utc).AddMilliseconds(250), Quality.Good, 0.218753),
                new ParamValueItem(new DateTime(2012,01,01,00,08,04, DateTimeKind.Utc), Quality.Good, 0.398232),
            };
            expectedValues = new ParamValueItem[]
            {
                new ParamValueItem(new DateTime(2012,01,01,00,01,00, DateTimeKind.Utc), Quality.Good, 0.218753),
                new ParamValueItem(new DateTime(2012,01,01,00,02,00, DateTimeKind.Utc), Quality.Good, 0.218753),
                new ParamValueItem(new DateTime(2012,01,01,00,03,00, DateTimeKind.Utc), Quality.Good, 0.218753),
                new ParamValueItem(new DateTime(2012,01,01,00,04,00, DateTimeKind.Utc), Quality.Good, 0.218753),
                new ParamValueItem(new DateTime(2012,01,01,00,05,00, DateTimeKind.Utc), Quality.Good, 0.218753),
                new ParamValueItem(new DateTime(2012,01,01,00,06,00, DateTimeKind.Utc), Quality.Good, 0.218753),
                new ParamValueItem(new DateTime(2012,01,01,00,07,00, DateTimeKind.Utc), Quality.Good, 0.218753),
                new ParamValueItem(new DateTime(2012,01,01,00,08,00, DateTimeKind.Utc), Quality.Good, 0.218753),
                new ParamValueItem(new DateTime(2012,01,01,00,09,00, DateTimeKind.Utc), Quality.Good, 0.398232),
            };

            // get minute
            const String getMinuteMessageFormat = "Minute ({0})";

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Zero, Interval.Minute, parameterValues);

            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Sum));
        }

        [Test]
        public void Aggregate_FromSecond_ReturnAggregatedValues()
        {
            const String valuesType = "";

            ValueAggregator aggregator = new ValueAggregator();

            IEnumerable<ParamValueItem> aggregateValues;
            IEnumerable<ParamValueItem> expectedValues;

            var generator = new ValueAggregatorTestDataGenerator();
            var parameterValues = generator.GetSourceData(0, Interval.Second, valuesType);
            var weightValues = generator.GetSourceData(1, Interval.Second, valuesType);

            // get minute
            const String getMinuteMessageFormat = "Minute ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Second, Interval.Minute, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Weighted));

            // get hour
            const String getHourMessageFormat = "Hour ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Second, Interval.Hour, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Weighted));

            // get day
            const String getDayMessageFormat = "Day ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Second, Interval.Day, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Weighted));
        }

        [Test]
        public void Aggregate_FromSecondWithSkips_ReturnAggregatedValues()
        {
            const String valuesType = "WithSkips";

            ValueAggregator aggregator = new ValueAggregator();

            IEnumerable<ParamValueItem> aggregateValues;
            IEnumerable<ParamValueItem> expectedValues;

            var generator = new ValueAggregatorTestDataGenerator();
            var parameterValues = generator.GetSourceData(0, Interval.Second, valuesType);
            var weightValues = generator.GetSourceData(1, Interval.Second, valuesType);

            // get minute
            const String getMinuteMessageFormat = "Minute ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Second, Interval.Minute, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Weighted));

            // get hour
            const String getHourMessageFormat = "Hour ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Second, Interval.Hour, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Weighted));

            // get day
            const String getDayMessageFormat = "Day ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Second, Interval.Day, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Weighted));
        }

        [Test]
        public void Aggregate_FromSecondWithBads_ReturnAggregatedValues()
        {
            const String valuesType = "WithBads";

            ValueAggregator aggregator = new ValueAggregator();

            IEnumerable<ParamValueItem> aggregateValues;
            IEnumerable<ParamValueItem> expectedValues;

            var generator = new ValueAggregatorTestDataGenerator();
            var parameterValues = generator.GetSourceData(0, Interval.Second, valuesType);
            var weightValues = generator.GetSourceData(1, Interval.Second, valuesType);

            // get minute
            const String getMinuteMessageFormat = "Minute ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Second, Interval.Minute, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Second, Interval.Minute, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Minute, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getMinuteMessageFormat, CalcAggregation.Weighted));

            // get hour
            const String getHourMessageFormat = "Hour ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Second, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Second, Interval.Hour, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Hour, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Weighted));

            // get day
            const String getDayMessageFormat = "Day ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Second, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Second, Interval.Day, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Second, valuesType, Interval.Day, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Weighted));
        }

        [Test]
        public void Aggregate_FromMinute_ReturnAggregatedValues()
        {
            const String valuesType = "";

            ValueAggregator aggregator = new ValueAggregator();

            IEnumerable<ParamValueItem> aggregateValues;
            IEnumerable<ParamValueItem> expectedValues;

            var generator = new ValueAggregatorTestDataGenerator();
            var parameterValues = generator.GetSourceData(0, Interval.Minute, valuesType);
            var weightValues = generator.GetSourceData(1, Interval.Minute, valuesType);

            // get hour
            const String getHourMessageFormat = "Hour ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Minute, Interval.Hour, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Weighted));

            // get day
            const String getDayMessageFormat = "Day ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Minute, Interval.Day, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Weighted));
        }

        [Test]
        public void Aggregate_FromMinuteWithSkips_ReturnAggregatedValues()
        {
            const String valuesType = "WithSkips";

            ValueAggregator aggregator = new ValueAggregator();

            IEnumerable<ParamValueItem> aggregateValues;
            IEnumerable<ParamValueItem> expectedValues;

            var generator = new ValueAggregatorTestDataGenerator();
            var parameterValues = generator.GetSourceData(0, Interval.Minute, valuesType);
            var weightValues = generator.GetSourceData(1, Interval.Minute, valuesType);

            // get hour
            const String getHourMessageFormat = "Hour ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Minute, Interval.Hour, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Weighted));

            // get day
            const String getDayMessageFormat = "Day ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Minute, Interval.Day, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Weighted));
        }

        [Test]
        public void Aggregate_FromMinuteWithBads_ReturnAggregatedValues()
        {
            const String valuesType = "WithBads";

            ValueAggregator aggregator = new ValueAggregator();

            IEnumerable<ParamValueItem> aggregateValues;
            IEnumerable<ParamValueItem> expectedValues;

            var generator = new ValueAggregatorTestDataGenerator();
            var parameterValues = generator.GetSourceData(0, Interval.Minute, valuesType);
            var weightValues = generator.GetSourceData(1, Interval.Minute, valuesType);

            // get hour
            const String getHourMessageFormat = "Hour ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Minute, Interval.Hour, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Minute, Interval.Hour, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Hour, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getHourMessageFormat, CalcAggregation.Weighted));

            // get day
            const String getDayMessageFormat = "Day ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Minute, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Minute, Interval.Day, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Minute, valuesType, Interval.Day, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Weighted));
        }

        [Test]
        public void Aggregate_FromHour_ReturnAggregatedValues()
        {
            const String valuesType = "";

            ValueAggregator aggregator = new ValueAggregator();

            IEnumerable<ParamValueItem> aggregateValues;
            IEnumerable<ParamValueItem> expectedValues;

            var generator = new ValueAggregatorTestDataGenerator();
            var parameterValues = generator.GetSourceData(0, Interval.Hour, valuesType);
            var weightValues = generator.GetSourceData(1, Interval.Hour, valuesType);

            // get day
            const String getDayMessageFormat = "Day ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Hour, Interval.Day, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Weighted));

            // get month
            const String getMonthMessageFormat = "Month ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Hour, Interval.Month, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Weighted));
        }

        [Test]
        public void Aggregate_FromHourWithSkips_ReturnAggregatedValues()
        {
            const String valuesType = "WithSkips";

            ValueAggregator aggregator = new ValueAggregator();

            IEnumerable<ParamValueItem> aggregateValues;
            IEnumerable<ParamValueItem> expectedValues;

            var generator = new ValueAggregatorTestDataGenerator();
            var parameterValues = generator.GetSourceData(0, Interval.Hour, valuesType);
            var weightValues = generator.GetSourceData(1, Interval.Hour, valuesType);

            // get day
            const String getDayMessageFormat = "Day ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Hour, Interval.Day, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Weighted));

            // get month
            const String getMonthMessageFormat = "Month ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Hour, Interval.Month, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Weighted));
        }

        [Test]
        public void Aggregate_FromHourWithBads_ReturnAggregatedValues()
        {
            const String valuesType = "WithBads";

            ValueAggregator aggregator = new ValueAggregator();

            IEnumerable<ParamValueItem> aggregateValues;
            IEnumerable<ParamValueItem> expectedValues;

            var generator = new ValueAggregatorTestDataGenerator();
            var parameterValues = generator.GetSourceData(0, Interval.Hour, valuesType);
            var weightValues = generator.GetSourceData(1, Interval.Hour, valuesType);

            // get day
            const String getDayMessageFormat = "Day ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Hour, Interval.Day, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Hour, Interval.Day, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Day, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getDayMessageFormat, CalcAggregation.Weighted));

            // get month
            const String getMonthMessageFormat = "Month ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Hour, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Hour, Interval.Month, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Hour, valuesType, Interval.Month, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Weighted));
        }

        [Test]
        public void Aggregate_FromDay_ReturnAggregatedValues()
        {
            const String valuesType = "";

            ValueAggregator aggregator = new ValueAggregator();

            IEnumerable<ParamValueItem> aggregateValues;
            IEnumerable<ParamValueItem> expectedValues;

            var generator = new ValueAggregatorTestDataGenerator();
            var parameterValues = generator.GetSourceData(0, Interval.Day, valuesType);
            var weightValues = generator.GetSourceData(1, Interval.Day, valuesType);

            // get month
            const String getMonthMessageFormat = "Month ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Day, Interval.Month, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Weighted));
        }

        [Test]
        public void Aggregate_FromDayWithSkips_ReturnAggregatedValues()
        {
            const String valuesType = "WithSkips";

            ValueAggregator aggregator = new ValueAggregator();

            IEnumerable<ParamValueItem> aggregateValues;
            IEnumerable<ParamValueItem> expectedValues;

            var generator = new ValueAggregatorTestDataGenerator();
            var parameterValues = generator.GetSourceData(0, Interval.Day, valuesType);
            var weightValues = generator.GetSourceData(1, Interval.Day, valuesType);

            // get month
            const String getMonthMessageFormat = "Month ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Day, Interval.Month, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Weighted));
        }

        [Test]
        public void Aggregate_FromDayWithBads_ReturnAggregatedValues()
        {
            const String valuesType = "WithBads";

            ValueAggregator aggregator = new ValueAggregator();

            IEnumerable<ParamValueItem> aggregateValues;
            IEnumerable<ParamValueItem> expectedValues;

            var generator = new ValueAggregatorTestDataGenerator();
            var parameterValues = generator.GetSourceData(0, Interval.Day, valuesType);
            var weightValues = generator.GetSourceData(1, Interval.Day, valuesType);

            // get month
            const String getMonthMessageFormat = "Month ({0})";

            // First
            aggregateValues = aggregator.Aggregate(CalcAggregation.First, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.First);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.First));

            // Last
            aggregateValues = aggregator.Aggregate(CalcAggregation.Last, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Last);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Last));

            // Maximum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Maximum, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Maximum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Maximum));

            // Minimum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Minimum, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Minimum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Minimum));

            // Sum
            aggregateValues = aggregator.Aggregate(CalcAggregation.Sum, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Sum);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Sum));

            // Average
            aggregateValues = aggregator.Aggregate(CalcAggregation.Average, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Average);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Average));

            // Count
            aggregateValues = aggregator.Aggregate(CalcAggregation.Count, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Count);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Count));

            // Exist
            aggregateValues = aggregator.Aggregate(CalcAggregation.Exist, Interval.Day, Interval.Month, parameterValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Exist);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Exist));

            // Weight
            aggregateValues = aggregator.Aggregate(CalcAggregation.Weighted, Interval.Day, Interval.Month, parameterValues, weightValues);

            expectedValues = generator.GetAggregateData(Interval.Day, valuesType, Interval.Month, CalcAggregation.Weighted);
            AssertValues(expectedValues, aggregateValues, String.Format(getMonthMessageFormat, CalcAggregation.Weighted));
        }
    }
}
