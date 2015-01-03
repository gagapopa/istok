using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Block;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.Block
{
    [TestFixture]
    public class ValueBufferTests
    {
        private ParameterItem CreateParameterItem()
        {
            return new ParameterItem()
            {
                Idnum = 3,
                Name = "Test1",
            };
        }

        private IEnumerable<ParamValueItem> GenerateValues(DateTime startTime, int count)
        {
            const int timeStep = 5;
            Random rnd = new Random();
            DateTime currentTime = startTime;

            for (int i = 0; i < count; i++)
            {
                yield return new ParamValueItem(currentTime.AddSeconds(i * timeStep), Quality.Good, rnd.NextDouble());
            }
        }

        [Test]
        public void AddValue_NoPackages_InitNewPackage()
        {
            ParameterItem testParam = CreateParameterItem();

            NSI.valReceiver = new TestValueReceiver();

            ValueBuffer buffer = new ValueBuffer();

            var values = new ParamValueItem[]
            {
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 05), Quality.Good, 6.7),
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 10), Quality.Good, 7.8),
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 15), Quality.Good, 8.9),
            };

            buffer.AddValue(testParam, values);

            // current package in buffer
            Package bufferPackage = buffer.GetPackage(testParam.Idnum);

            Assert.IsNotNull(bufferPackage);

            Assert.AreEqual(testParam.Idnum, bufferPackage.Id);
            Assert.AreEqual(values.Length, bufferPackage.Values.Count);
            Assert.AreEqual(values.First().Time, bufferPackage.DateFrom);
            Assert.AreEqual(values.Last().Time, bufferPackage.DateTo);
        }

        [Test]
        public void AddValue_OnlyPrevPackage_UseIt()
        {
            ParameterItem testParam = CreateParameterItem();

            TestValueReceiver valueReceiver = new TestValueReceiver();

            NSI.valReceiver = valueReceiver;

            var package = new Package()
            {
                Id = testParam.Idnum,
                Values = new List<ParamValueItem>(new ParamValueItem[]
                {
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 26, 45), Quality.Good, 3.4),
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 26, 50), Quality.Good, 4.5),
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 26, 55), Quality.Good, 5.6),
                })
            };
            package.Normailze();
            valueReceiver.SavePackage(package);

            var values = new ParamValueItem[]
            {
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 05), Quality.Good, 6.7),
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 10), Quality.Good, 7.8),
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 15), Quality.Good, 8.9),
            };

            // tested object
            ValueBuffer buffer = new ValueBuffer();

            buffer.AddValue(testParam, values);

            // current package in buffer
            Package bufferPackage = buffer.GetPackage(testParam.Idnum);

            Assert.IsNotNull(bufferPackage);
            Assert.AreSame(bufferPackage, package);

            Assert.AreEqual(testParam.Idnum, bufferPackage.Id);
            Assert.AreEqual(6, bufferPackage.Values.Count);
            Assert.AreEqual(package.Values.First().Time, bufferPackage.DateFrom);
            Assert.AreEqual(values.Last().Time, bufferPackage.DateTo);
        }

        [Test]
        public void AddValue_OnlyCurrentPackage_UseIt()
        {
            ParameterItem testParam = CreateParameterItem();

            TestValueReceiver valueReceiver = new TestValueReceiver();

            NSI.valReceiver = valueReceiver;

            var package = new Package()
            {
                Id = testParam.Idnum,
                Values = new List<ParamValueItem>(new ParamValueItem[]
                {
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 05), Quality.Good, 3.4),
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 10), Quality.Good, 4.5),
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 15), Quality.Good, 5.6),
                })
            };
            package.Normailze();
            valueReceiver.SavePackage(package);

            var values = new ParamValueItem[]
            {
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 20), Quality.Good, 6.7),
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 25), Quality.Good, 7.8),
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 30), Quality.Good, 8.9),
            };

            // tested object
            ValueBuffer buffer = new ValueBuffer();

            buffer.AddValue(testParam, values);

            // current package in buffer
            Package bufferPackage = buffer.GetPackage(testParam.Idnum);

            Assert.IsNotNull(bufferPackage);
            Assert.AreSame(bufferPackage, package);

            Assert.AreEqual(testParam.Idnum, bufferPackage.Id);
            Assert.AreEqual(6, bufferPackage.Values.Count);
            Assert.AreEqual(package.Values.First().Time, bufferPackage.DateFrom);
            Assert.AreEqual(values.Last().Time, bufferPackage.DateTo);
        }

        [Test]
        public void AddValue_OnlyNextPackage_UseIt()
        {
            ParameterItem testParam = CreateParameterItem();

            TestValueReceiver valueReceiver = new TestValueReceiver();

            NSI.valReceiver = valueReceiver;

            var package = new Package()
            {
                Id = testParam.Idnum,
                Values = new List<ParamValueItem>(new ParamValueItem[]
                {
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 20), Quality.Good, 6.7),
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 25), Quality.Good, 7.8),
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 30), Quality.Good, 8.9),
                })
            };
            package.Normailze();
            valueReceiver.SavePackage(package);
            DateTime dateTo = package.Values.Last().Time;

            var values = new ParamValueItem[]
            {
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 05), Quality.Good, 3.4),
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 10), Quality.Good, 4.5),
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 15), Quality.Good, 5.6),
            };

            // tested object
            ValueBuffer buffer = new ValueBuffer();

            buffer.AddValue(testParam, values);

            // current package in buffer
            Package bufferPackage = buffer.GetPackage(testParam.Idnum);

            Assert.IsNotNull(bufferPackage);
            Assert.AreSame(bufferPackage, package);

            Assert.AreEqual(testParam.Idnum, bufferPackage.Id);
            Assert.AreEqual(6, bufferPackage.Values.Count);
            Assert.AreEqual(values.First().Time, bufferPackage.DateFrom);
            Assert.AreEqual(dateTo, bufferPackage.DateTo);
        }

        [Test]
        public void AddValue_AllPackages_UseCurrent()
        {
            ParameterItem testParam = CreateParameterItem();

            var prevPackage = new Package()
            {
                Id = testParam.Idnum,
                Values = new List<ParamValueItem>(new ParamValueItem[]
                {
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 26, 45), Quality.Good, 3.4),
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 26, 50), Quality.Good, 4.5),
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 26, 55), Quality.Good, 5.6),
                })
            };
            prevPackage.Normailze();

            var currPackage = new Package()
            {
                Id = testParam.Idnum,
                Values = new List<ParamValueItem>(new ParamValueItem[]
                {
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 05), Quality.Good, 3.4),
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 10), Quality.Good, 4.5),
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 30), Quality.Good, 5.6),
                })
            };
            currPackage.Normailze();

            var nextPackage = new Package()
            {
                Id = testParam.Idnum,
                Values = new List<ParamValueItem>(new ParamValueItem[]
                {
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 35), Quality.Good, 6.7),
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 40), Quality.Good, 7.8),
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 45), Quality.Good, 8.9),
                })
            };
            nextPackage.Normailze();

            TestValueReceiver valueReceiver = new TestValueReceiver();

            NSI.valReceiver = valueReceiver;

            valueReceiver.SavePackage(prevPackage);
            valueReceiver.SavePackage(currPackage);
            valueReceiver.SavePackage(nextPackage);
            valueReceiver.SavePackageLog.Clear();

            var values = new ParamValueItem[]
            {
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 15), Quality.Good, 3.4),
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 20), Quality.Good, 4.5),
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 25), Quality.Good, 5.6),
            };

            // tested object
            ValueBuffer buffer = new ValueBuffer();

            buffer.AddValue(testParam, values);

            // current package in buffer
            Package bufferPackage = buffer.GetPackage(testParam.Idnum);

            Assert.AreNotSame(bufferPackage, prevPackage);
            Assert.AreSame(bufferPackage, currPackage);
            Assert.AreNotSame(bufferPackage, nextPackage);

            Assert.AreEqual(testParam.Idnum, bufferPackage.Id);
            Assert.AreEqual(6, bufferPackage.Values.Count);
        }

        [Test]
        public void AddValue_PrevCurrentExcessCount_MoveValueToNext()
        {
            // setup test data
            ParameterItem parameter = CreateParameterItem();

            Package prevPackage = new Package()
            {
                Id = parameter.Idnum,
                Values = GenerateValues(new DateTime(2013, 01, 15, 03, 27, 05), NSI.DBPackageSize).ToList()
            };
            prevPackage.Normailze();

            Package currPackage = new Package()
            {
                Id = parameter.Idnum,
                Values = GenerateValues(prevPackage.DateTo.AddSeconds(5), NSI.DBPackageSize).ToList()
            };
            currPackage.Normailze();

            Package nextPackage = new Package()
            {
                Id = parameter.Idnum,
                Values = GenerateValues(currPackage.DateTo.AddSeconds(5), NSI.DBPackageSize / 3).ToList()
            };
            nextPackage.Normailze();

            DateTime currentTime = currPackage.DateTo.AddSeconds(-172);

            var values = new ParamValueItem[]
            {
                new ParamValueItem(currentTime.AddSeconds(5), Quality.Good, 6.7),
                new ParamValueItem(currentTime.AddSeconds(10), Quality.Good, 7.8),
                new ParamValueItem(currentTime.AddSeconds(15), Quality.Good, 8.9),
            };

            var tesValueReceiver = new TestValueReceiver();
            NSI.valReceiver = tesValueReceiver;
            NSI.valReceiver.SavePackage(prevPackage);
            NSI.valReceiver.SavePackage(currPackage);
            NSI.valReceiver.SavePackage(nextPackage);
            tesValueReceiver.SavePackageLog.Clear();

            // tested object
            ValueBuffer buffer = new ValueBuffer();

            // tested method
            buffer.AddValue(parameter, values);

            Package package = buffer.GetPackage(parameter.Idnum);

            // currPackage are current package :)
            Assert.IsNotNull(package);
            Assert.AreNotSame(prevPackage, package);
            Assert.AreSame(currPackage, package);
            Assert.AreNotSame(nextPackage, package);

            // currPackage count is max and all added values in currPackages
            Assert.AreEqual(NSI.DBPackageSize, package.Count);
            foreach (var item in values)
            {
                Assert.IsTrue(package.Values.Contains(item));
            }
            // currPackage.DateTo and nextPackage.DateFrom are shifted and nextPackage.Count increase
            Assert.AreEqual(currPackage.DateTo, nextPackage.DateFrom);
            Assert.AreEqual(NSI.DBPackageSize / 3 + values.Length, nextPackage.Count);

            // check if NSI.valReceiver.SavePackage(currPackage) is occurred
            Assert.GreaterOrEqual(tesValueReceiver.SavePackageLog.Count, 1);
            Assert.AreSame(nextPackage, tesValueReceiver.SavePackageLog.First());
        }

        [Test]
        public void AddValue_AllPackagesExcessCount_SplitCurrentNext()
        {
            // setup test data
            ParameterItem parameter = CreateParameterItem();

            Package prevPackage = new Package()
            {
                Id = parameter.Idnum,
                Values = GenerateValues(new DateTime(2013, 01, 15, 03, 27, 05), NSI.DBPackageSize).ToList()
            };
            prevPackage.Normailze();

            Package currPackage = new Package()
            {
                Id = parameter.Idnum,
            };
            currPackage.Values.AddRange(GenerateValues(prevPackage.DateTo.AddSeconds(5), NSI.DBPackageSize / 2));
            DateTime skipTime = currPackage.Values.Last().Time;
            currPackage.Values.AddRange(GenerateValues(skipTime.AddHours(2), NSI.DBPackageSize / 2));
            currPackage.Normailze();

            Package nextPackage = new Package()
            {
                Id = parameter.Idnum,
                Values = GenerateValues(currPackage.DateTo.AddSeconds(5), NSI.DBPackageSize).ToList()
            };
            nextPackage.Normailze();

            var values = new ParamValueItem[]
            {
                new ParamValueItem(skipTime.AddSeconds(5), Quality.Good, 6.7),
                new ParamValueItem(skipTime.AddSeconds(10), Quality.Good, 7.8),
                new ParamValueItem(skipTime.AddSeconds(15), Quality.Good, 8.9),
            };

            var tesValueReceiver = new TestValueReceiver();
            NSI.valReceiver = tesValueReceiver;
            NSI.valReceiver.SavePackage(prevPackage);
            NSI.valReceiver.SavePackage(currPackage);
            NSI.valReceiver.SavePackage(nextPackage);
            tesValueReceiver.SavePackageLog.Clear();

            // tested object
            ValueBuffer buffer = new ValueBuffer();

            // tested method
            buffer.AddValue(parameter, values);

            Package package = buffer.GetPackage(parameter.Idnum);

            // prevPackage is not changed
            Assert.AreEqual(NSI.DBPackageSize, prevPackage.Count);

            // currPackage and nextPackage splited by third (currPackage added values)
            Assert.AreEqual(NSI.DBPackageSize * 2 / 3 + values.Length, currPackage.Count);
            Assert.AreEqual(NSI.DBPackageSize * 2 / 3, nextPackage.Count);

            // package is new package
            Assert.AreNotSame(prevPackage, package);
            Assert.AreSame(currPackage, package);
            Assert.AreNotSame(nextPackage, package);

            // all 3 packages are saved
            Assert.AreEqual(3, tesValueReceiver.SavePackageLog.Count);
            Assert.IsTrue(tesValueReceiver.SavePackageLog.Contains(currPackage));
            Assert.IsTrue(tesValueReceiver.SavePackageLog.Contains(nextPackage));

            // get new package from save
            var newPackage = (from p in tesValueReceiver.SavePackageLog where p != currPackage && p != nextPackage select p).FirstOrDefault();

            Assert.IsNotNull(newPackage);
            Assert.AreEqual(currPackage.DateTo, newPackage.DateFrom);
            Assert.AreEqual(newPackage.DateTo, nextPackage.DateFrom);
        }

        [Test]
        public void AddValue_PrevCurrentPackagesExcessCount_InitNewPackage()
        {
            // setup test data
            ParameterItem parameter = CreateParameterItem();

            Package prevPackage = new Package()
            {
                Id = parameter.Idnum,
                Values = GenerateValues(new DateTime(2013, 01, 15, 03, 27, 05), NSI.DBPackageSize).ToList()
            };
            prevPackage.Normailze();

            Package currPackage = new Package()
            {
                Id = parameter.Idnum,
                Values = GenerateValues(prevPackage.DateTo.AddSeconds(5), NSI.DBPackageSize).ToList()
            };
            currPackage.Normailze();

            DateTime currentTime = currPackage.DateTo;

            var values = new ParamValueItem[]
            {
                new ParamValueItem(currentTime.AddSeconds(5), Quality.Good, 6.7),
                new ParamValueItem(currentTime.AddSeconds(10), Quality.Good, 7.8),
                new ParamValueItem(currentTime.AddSeconds(15), Quality.Good, 8.9),
            };

            var tesValueReceiver = new TestValueReceiver();
            NSI.valReceiver = tesValueReceiver;
            NSI.valReceiver.SavePackage(prevPackage);
            NSI.valReceiver.SavePackage(currPackage);
            tesValueReceiver.SavePackageLog.Clear();

            // tested object
            ValueBuffer buffer = new ValueBuffer();

            // tested method
            buffer.AddValue(parameter, values);

            Package package = buffer.GetPackage(parameter.Idnum);

            Assert.IsNotNull(package);
            Assert.AreNotSame(prevPackage, package);
            Assert.AreNotSame(currPackage, package);

            Assert.AreEqual(values.First().Time, package.DateFrom);
            Assert.AreEqual(values.Last().Time, package.DateTo);
            Assert.AreEqual(values.Length, package.Count);

            // dateTo setted before init new package
            Assert.AreEqual(package.DateFrom, currPackage.DateTo);

            // check if NSI.valReceiver.SavePackage(currPackage) is occurred
            Assert.AreEqual(1, tesValueReceiver.SavePackageLog.Count);
            Assert.AreSame(currPackage, tesValueReceiver.SavePackageLog.First());
        }

        [Test]
        public void AddValue_ValueInMiddle_AddValue()
        {    
            // setup test data
            ParameterItem parameter = CreateParameterItem();

            Package prevPackage = new Package()
            {
                Id = parameter.Idnum,
                Values = GenerateValues(new DateTime(2013, 01, 15, 03, 27, 05), NSI.DBPackageSize).ToList()
            };
            prevPackage.Normailze();

            Package currPackage = new Package()
            {
                Id = parameter.Idnum,
            };
            currPackage.Values = GenerateValues(prevPackage.DateTo.AddSeconds(5), NSI.DBPackageSize / 4).ToList();
            DateTime cupTime = currPackage.Values.Last().Time.AddSeconds(5);
            currPackage.Values.AddRange(GenerateValues(cupTime.AddSeconds(5), NSI.DBPackageSize / 4));
            currPackage.Normailze();

            DateTime startPackageTime = currPackage.DateFrom;
            DateTime endPackageTime = currPackage.DateTo;
            int packageCount = currPackage.Count;

            DateTime currentTime = currPackage.DateTo;

            var values = new ParamValueItem[]
            {
                new ParamValueItem(cupTime, Quality.Good, 6.7),
            };

            var tesValueReceiver = new TestValueReceiver();
            NSI.valReceiver = tesValueReceiver;
            NSI.valReceiver.SavePackage(prevPackage);
            NSI.valReceiver.SavePackage(currPackage);
            tesValueReceiver.SavePackageLog.Clear();

            // tested object
            ValueBuffer buffer = new ValueBuffer();

            // tested method
            buffer.AddValue(parameter, values);

            Package package = buffer.GetPackage(parameter.Idnum);

            Assert.IsNotNull(package);
            Assert.AreNotSame(prevPackage, package);
            Assert.AreSame(currPackage, package);

            Assert.AreEqual(startPackageTime, package.DateFrom);
            Assert.AreEqual(endPackageTime, package.DateTo);
            Assert.AreEqual(packageCount + 1, package.Count);

            Assert.IsTrue(package.Values.Contains(values.First()));
        }

        [Test]
        public void AddValue_ValueInMiddleAppertured_DontAddValue()
        {
            // setup test data
            ParameterItem parameter = CreateParameterItem();
            parameter.SetPropertyValue(CommonData.ApertureProperty, "3%");

            Package prevPackage = new Package()
            {
                Id = parameter.Idnum,
                Values = GenerateValues(new DateTime(2013, 01, 15, 03, 27, 05), NSI.DBPackageSize).ToList()
            };
            prevPackage.Normailze();

            Package currPackage = new Package()
            {
                Id = parameter.Idnum,
            };
            currPackage.Values = GenerateValues(prevPackage.DateTo.AddSeconds(5), NSI.DBPackageSize / 4).ToList();
            ParamValueItem cupValue = currPackage.Values.Last();
            currPackage.Values.AddRange(GenerateValues(cupValue.Time.AddHours(1), NSI.DBPackageSize / 4));
            currPackage.Normailze();

            DateTime startPackageTime = currPackage.DateFrom;
            DateTime endPackageTime = currPackage.DateTo;
            int packageCount = currPackage.Count;

            DateTime currentTime = currPackage.DateTo;

            var values = new ParamValueItem[]
            {
                new ParamValueItem(cupValue.Time.AddSeconds(5), Quality.Good, cupValue.Value + cupValue.Value * 0.01),
            };

            var tesValueReceiver = new TestValueReceiver();
            NSI.valReceiver = tesValueReceiver;
            NSI.valReceiver.SavePackage(prevPackage);
            NSI.valReceiver.SavePackage(currPackage);
            tesValueReceiver.SavePackageLog.Clear();

            // tested object
            ValueBuffer buffer = new ValueBuffer();

            buffer.RegisterParameters(new ParameterItem[] { parameter }, new Dictionary<String, String>());

            // tested method
            buffer.AddValue(parameter, values);

            Package package = buffer.GetPackage(parameter.Idnum);

            Assert.IsNotNull(package);
            Assert.AreNotSame(prevPackage, package);
            Assert.AreSame(currPackage, package);

            Assert.AreEqual(startPackageTime, package.DateFrom);
            Assert.AreEqual(endPackageTime, package.DateTo);
            Assert.AreEqual(packageCount, package.Count);

            Assert.IsFalse(package.Values.Contains(values.First()));
            Assert.IsTrue(package.Values.Contains(cupValue));
        }

        [Test]
        public void AddValue_ValueExistsWithTime_RenewValue()
        {
            // setup test data
            ParameterItem parameter = CreateParameterItem();

            Package prevPackage = new Package()
            {
                Id = parameter.Idnum,
                Values = GenerateValues(new DateTime(2013, 01, 15, 03, 27, 05), NSI.DBPackageSize).ToList()
            };
            prevPackage.Normailze();

            Package currPackage = new Package()
            {
                Id = parameter.Idnum,
            };
            currPackage.Values = GenerateValues(prevPackage.DateTo.AddSeconds(5), NSI.DBPackageSize / 4).ToList();
            DateTime cupTime = currPackage.Values.Last().Time.AddSeconds(5);
            currPackage.Values.AddRange(GenerateValues(cupTime, NSI.DBPackageSize / 4));
            currPackage.Normailze();

            DateTime startPackageTime = currPackage.DateFrom;
            DateTime endPackageTime = currPackage.DateTo;
            int packageCount = currPackage.Count;

            DateTime currentTime = currPackage.DateTo;

            var values = new ParamValueItem[]
            {
                new ParamValueItem(cupTime, Quality.Good, 6.7),
            };

            var tesValueReceiver = new TestValueReceiver();
            NSI.valReceiver = tesValueReceiver;
            NSI.valReceiver.SavePackage(prevPackage);
            NSI.valReceiver.SavePackage(currPackage);
            tesValueReceiver.SavePackageLog.Clear();

            // tested object
            ValueBuffer buffer = new ValueBuffer();

            // tested method
            buffer.AddValue(parameter, values);

            Package package = buffer.GetPackage(parameter.Idnum);

            Assert.IsNotNull(package);
            Assert.AreNotSame(prevPackage, package);
            Assert.AreSame(currPackage, package);

            Assert.AreEqual(startPackageTime, package.DateFrom);
            Assert.AreEqual(endPackageTime, package.DateTo);
            Assert.AreEqual(packageCount, package.Count);

            var valuesWithTime = (from v in package.Values where v.Time == cupTime select v).ToArray();
            Assert.AreEqual(1, valuesWithTime.Length);
            var valueInPackage = valuesWithTime.First();
            Assert.AreEqual(values.First().Time, valueInPackage.Time);
            Assert.AreEqual(values.First().Value, valueInPackage.Value);
            Assert.AreEqual(values.First().Quality, valueInPackage.Quality);
        }

        [Test]
        public void AddValue_ValueToDifferentPackages_SavePackageBeforeSwitch()
        {
            // setup test data
            ParameterItem testParam = CreateParameterItem();

            DateTime[] startTimes = new DateTime[]
            {
                new DateTime(2013, 03, 15, 03, 27, 05),
                new DateTime(2013, 02, 15, 03, 27, 05),
                new DateTime(2013, 04, 15, 03, 27, 05),
                new DateTime(2013, 01, 15, 03, 27, 05),
                new DateTime(2013, 05, 15, 03, 27, 05),
            };

            var tesValueReceiver = new TestValueReceiver();
            NSI.valReceiver = tesValueReceiver;
            int packageLength = NSI.DBPackageSize / 3;

            for (int i = 0; i < startTimes.Length; i++)
            {
                // usable package
                Package package = new Package()
                {
                    Id = testParam.Idnum,
                    Values = GenerateValues(startTimes[i], packageLength).ToList()
                };
                package.Normailze();
                tesValueReceiver.SavePackage(package);

                // separate package
                package = new Package()
                {
                    Id = testParam.Idnum,
                    Values = GenerateValues(startTimes[i].AddDays(1), packageLength).ToList()
                };
                package.Normailze();
                tesValueReceiver.SavePackage(package);
            }
            tesValueReceiver.SavePackageLog.Clear();

            var values = new ParamValueItem[] 
            { 
                new ParamValueItem(startTimes[0].AddSeconds(5 + 5 * packageLength), Quality.Good, 4.5),
                new ParamValueItem(startTimes[1].AddSeconds(5 + 5 * packageLength), Quality.Good, 5.6),
                new ParamValueItem(startTimes[2].AddSeconds(5 + 5 * packageLength), Quality.Good, 6.7),
                new ParamValueItem(startTimes[3].AddSeconds(5 + 5 * packageLength), Quality.Good, 7.8),
                new ParamValueItem(startTimes[4].AddSeconds(5 + 5 * packageLength), Quality.Good, 8.9),
            };

            // tested object
            ValueBuffer buffer = new ValueBuffer();

            // tested method
            buffer.AddValue(testParam, values);

            // the first 4 packages mast be saved
            Assert.AreEqual(startTimes.Length - 1, tesValueReceiver.SavePackageLog.Count);
            for (int i = 0; i < startTimes.Length ; i++)
            {
                Package package;

                // fifth package's in buffer
                if (i == startTimes.Length - 1)
                {
                    package = buffer.GetPackage(testParam.Idnum);
                }
                else
                {
                    package = tesValueReceiver.SavePackageLog[i]; 
                }

                Assert.IsNotNull(package);
                Assert.AreEqual(startTimes[i], package.DateFrom);
                Assert.AreEqual(values[i].Time, package.DateTo);
                Assert.AreEqual(packageLength + 1, package.Count);

                var lastValue = package.Values.Last();

                Assert.IsNotNull(lastValue);
                Assert.AreEqual(values[i].Time, lastValue.Time);
                Assert.AreEqual(values[i].Quality, lastValue.Quality);
                Assert.AreEqual(values[i].Value, lastValue.Value);
            }
        }

        [Test]
        public void AddValue__RaiseUpdateValue()
        {
            ParameterItem testParam = CreateParameterItem();

            TestValueReceiver valueReceiver = new TestValueReceiver();

            NSI.valReceiver = valueReceiver;

            var package = new Package()
            {
                Id = testParam.Idnum,
                Values = new List<ParamValueItem>(new ParamValueItem[]
                {
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 05), Quality.Good, 3.4),
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 10), Quality.Good, 4.5),
                    new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 15), Quality.Good, 5.6),
                })
            };
            package.Normailze();
            valueReceiver.SavePackage(package);

            var values = new ParamValueItem[]
            {
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 20), Quality.Good, 6.7),
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 25), Quality.Good, 7.8),
                new ParamValueItem(new DateTime(2013, 01, 15, 03, 27, 30), Quality.Good, 8.9),
            };

            // tested object
            ValueBuffer buffer = new ValueBuffer();

            List<ParamValueItem> updateParameters = new List<ParamValueItem>();
            buffer.UpdateValue += (p, v) =>
            {
                if (testParam.Equals(p))
                {
                    updateParameters.Add(v);
                }
            };

            buffer.AddValue(testParam, values);

            Assert.AreEqual(1, updateParameters.Count);
            Assert.AreEqual(values.Last().Time, updateParameters.Last().Time);
            Assert.AreEqual(values.Last().Value, updateParameters.Last().Value);
        }
    }
}
