using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using COTES.ISTOK.Block;
using COTES.ISTOK.Modules;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.Block
{
    [TestFixture]
    public class ChannelItemTests
    {
        [Test]
        public void Start__CallDataLoaderInit()
        {
            var moduleLoader = new TestModuleLoader();

            var testDataLoader = new Moq.Mock<IDataLoader>();

            var testDataFactory = new TestDataLoaderFactory();
            testDataFactory.DataLoader = testDataLoader.Object;

            moduleLoader.AddModule("module1", testDataFactory);

            using (ChannelItem channel = new ChannelItem(new ChannelInfo()
            {
                Module = new ModuleInfo("module1", "", null, null),
                Parameters = new ParameterItem[] { }
            }))
            {
                channel.ModuleLoader = moduleLoader;

                channel.Start();

                testDataLoader.Verify(l => l.Init(Moq.It.IsAny<ChannelInfo>()));
            }
        }

        [Test]
        public void NotifyError__SetLastErrorProperty()
        {
            //var moduleLoader = new TestModuleLoader();

            //IDataListener dataListener;
            //var testDataLoader = new TestDataLoader();

            //var testDataFactory = new TestDataLoaderFactory();
            //testDataFactory.DataLoader = testDataLoader;

            //moduleLoader.AddModule("module1", testDataFactory);

            using ( ChannelItem channel = new ChannelItem(new ChannelInfo() { Module = new ModuleInfo("module1", "", null, null) }))
            {
                //channel.ModuleLoader = moduleLoader;

                //channel.Start();

                IDataListener dataListener = channel as IDataListener;
                //dataListener = testDataLoader.DataListener;

                Assert.IsNotNull(dataListener);

                const String errorMessage = "Сбой сбора";

                dataListener.NotifyError(errorMessage);

                Assert.AreEqual(errorMessage, channel.LastErrorMessage);

                Assert.AreEqual(0, (int)(DateTime.Now - channel.LastErrorTime).TotalSeconds); 
            }
        }

        [Test]
        public void NotifyValues__SetLastActivityTimeToNow()
        {
            //var moduleLoader = new TestModuleLoader();

            //IDataListener dataListener;
            //var testDataLoader = new Moq.Mock<IDataLoader>();
            ////testDataLoader.Setup(l => l.DataListener = Moq.It.IsAny<IDataListener>()).Callback(dl => dataListener = dl);

            //var testDataFactory = new TestDataLoaderFactory();
            //testDataFactory.DataLoader = testDataLoader.Object;

            //moduleLoader.AddModule("module1", testDataFactory);

            using (ChannelItem channel = new ChannelItem(
                new ChannelInfo()
                {
                    Module = new ModuleInfo("module1", "", null, null),
                    Parameters = new ParameterItem[] { }
                }))
            {
                //channel.ModuleLoader = moduleLoader;

                //channel.Start();

                //dataListener = testDataLoader.Object.DataListener;
                IDataListener dataListener = channel as IDataListener;

                Assert.IsNotNull(dataListener);

                Assert.AreNotEqual(0, (DateTime.Now - channel.LastActivityTime).TotalSeconds);

                dataListener.NotifyValues(null, new ParameterItem(), new ParamValueItem[] { new ParamValueItem(DateTime.Now.AddHours(-1), Quality.Good, 1) });

                Assert.AreEqual(0, (int)(DateTime.Now - channel.LastActivityTime).TotalSeconds); 
            }
        }

        [Test]
        public void NotifyValues__SetParameterLastTime()
        {

            ModuleInfo moduleInfo = new ModuleInfo("module1", "", null, null);

            ChannelInfo channelInfo = new ChannelInfo()
            {
                Module = moduleInfo
            };

            ParameterItem parameter1 = new ParameterItem()
            {
                Idnum = 1,
                Name = "parameter1"
            };

            ParameterItem parameter2 = new ParameterItem()
            {
                Idnum = 2,
                Name = "parameter2"
            };
            channelInfo.Parameters = new ParameterItem[]
            {
                parameter1,
                parameter2
            };

            using (ChannelItem channel = new ChannelItem(channelInfo))
            {
                IDataListener dataListener = channel as IDataListener;

                Assert.IsNotNull(dataListener);

                Assert.AreNotEqual(0, (DateTime.Now - channel.LastActivityTime).Seconds);

                DateTime time1 = DateTime.Now.AddHours(-1);
                DateTime time2 = DateTime.Now.AddHours(-3);
                DateTime time3 = DateTime.Now.AddDays(-1);

                dataListener.NotifyValues(null, new ParameterItem() { Idnum = 1 },
                    new ParamValueItem[] 
                {
                    new ParamValueItem(time2, Quality.Good, 1),
                    new ParamValueItem(time1, Quality.Good, 2),
                    new ParamValueItem(time3, Quality.Good, 10) 
                });
                dataListener.NotifyValues(null, new ParameterItem() { Idnum = 2 },
                    new ParamValueItem[] 
                {
                    new ParamValueItem(time3, Quality.Good, 10),
                    new ParamValueItem(time2, Quality.Good, 1),
                });

                Assert.AreEqual(time1, parameter1.LastTime);
                Assert.AreEqual(time2, parameter2.LastTime); 
            }
        }

        [Test]
        public void Start_IsCurrent_CallGetDataRepeadly()
        {
            List<DateTime> callTimes = new List<DateTime>();

            var moduleLoader = new TestModuleLoader();

            var testDataLoader = new Moq.Mock<IDataLoader>();
            testDataLoader.Setup(l => l.LoadMethod).Returns(DataLoadMethod.Current);
            testDataLoader.Setup(l => l.GetCurrent()).Callback(() => callTimes.Add(DateTime.Now));

            var testDataFactory = new TestDataLoaderFactory();
            testDataFactory.DataLoader = testDataLoader.Object;

            moduleLoader.AddModule("module1", testDataFactory);

            ModuleInfo module = new ModuleInfo("module1", null, null, null);

            ChannelInfo channelInfo = new ChannelInfo()
            {
                Module = module,
                Parameters = new ParameterItem[]{
                    new ParameterItem()
                    {
                        Idnum = 1
                    },
                    new ParameterItem()
                    {
                        Idnum = 2
                    }
                },
            };
            channelInfo[CommonProperty.PauseProperty] = "1";

            using (ChannelItem channel = new ChannelItem(channelInfo))
            {
                channel.ModuleLoader = moduleLoader;

                channel.Start();

                Thread.Sleep(5000);

                channel.Stop();

                var timesArray = callTimes.ToArray();

                Assert.Greater(timesArray.Length, 4);

                DateTime time = timesArray[0];

                for (int i = 1; i < timesArray.Length; i++)
                {
                    Assert.AreEqual(1000, (timesArray[i] - time).TotalMilliseconds, 100);
                    time = timesArray[i];
                } 
            }
        }

        [Test]
        public void Stop_IsCurrent_StopCallGetDataRepeadly()
        {
            List<DateTime> callTimes = new List<DateTime>();

            var moduleLoader = new TestModuleLoader();

            var testDataLoader = new Moq.Mock<IDataLoader>();
            testDataLoader.Setup(l => l.LoadMethod).Returns(DataLoadMethod.Current);
            testDataLoader.Setup(l => l.GetCurrent()).Callback(() => callTimes.Add(DateTime.Now));

            var testDataFactory = new TestDataLoaderFactory();
            testDataFactory.DataLoader = testDataLoader.Object;

            moduleLoader.AddModule("module1", testDataFactory);

            ModuleInfo module = new ModuleInfo("module1", null, null, null);

            ChannelInfo channelInfo = new ChannelInfo()
            {
                Module = module,
                Parameters = new ParameterItem[]{
                    new ParameterItem()
                    {
                        Idnum = 1
                    },
                    new ParameterItem()
                    {
                        Idnum = 2
                    }
                },
            };
            channelInfo[CommonProperty.PauseProperty] = "1";

            using (ChannelItem channel = new ChannelItem(channelInfo))
            {
                channel.ModuleLoader = moduleLoader;

                channel.Start();

                Thread.Sleep(1500);

                channel.Stop();

                int count = callTimes.Count;

                Thread.Sleep(2000);

                Assert.AreEqual(count, callTimes.Count); 
            }
        }

        [Test]
        public void Start_IsSubscribe_CallRegisterSubscribe()
        {
            List<DateTime> callTimes = new List<DateTime>();

            var moduleLoader = new TestModuleLoader();

            var testDataLoader = new Moq.Mock<IDataLoader>();
            testDataLoader.Setup(l => l.LoadMethod).Returns(DataLoadMethod.Subscribe);
            testDataLoader.Setup(l => l.GetCurrent()).Callback(() => callTimes.Add(DateTime.Now));

            var testDataFactory = new TestDataLoaderFactory();
            testDataFactory.DataLoader = testDataLoader.Object;

            moduleLoader.AddModule("module1", testDataFactory);

            ModuleInfo module = new ModuleInfo("module1", null, null, null);

            ChannelInfo channelInfo = new ChannelInfo()
            {
                Module = module,
                Parameters = new ParameterItem[]{
                    new ParameterItem()
                    {
                        Idnum = 1
                    },
                    new ParameterItem()
                    {
                        Idnum = 2
                    }
                },
            };
            //channelInfo[ChannelItem.PauseProperty] = "1";

            using (ChannelItem channel = new ChannelItem(channelInfo))
            {
                channel.ModuleLoader = moduleLoader;

                channel.Start();

                testDataLoader.Verify(l => l.RegisterSubscribe());

                //Thread.Sleep(5000);

                //var timesArray = callTimes.ToArray();

                //Assert.Greater(timesArray.Length, 4);

                //DateTime time = timesArray[0];

                //for (int i = 1; i < timesArray.Length; i++)
                //{
                //    Assert.AreEqual((timesArray[i] - time).Milliseconds, 1000, 5);
                //    time = timesArray[i];
                //} 
            }
        }

        [Test]
        public void Stop_IsSubscribe_CallUnregisterSubscribe()
        {
            List<DateTime> callTimes = new List<DateTime>();

            var moduleLoader = new TestModuleLoader();

            var testDataLoader = new Moq.Mock<IDataLoader>();
            testDataLoader.Setup(l => l.LoadMethod).Returns(DataLoadMethod.Subscribe);
            testDataLoader.Setup(l => l.GetCurrent()).Callback(() => callTimes.Add(DateTime.Now));

            var testDataFactory = new TestDataLoaderFactory();
            testDataFactory.DataLoader = testDataLoader.Object;

            moduleLoader.AddModule("module1", testDataFactory);

            ModuleInfo module = new ModuleInfo("module1", null, null, null);

            ChannelInfo channelInfo = new ChannelInfo()
            {
                Module = module,
                Parameters = new ParameterItem[]{
                    new ParameterItem()
                    {
                        Idnum = 1
                    },
                    new ParameterItem()
                    {
                        Idnum = 2
                    }
                },
            };
            //channelInfo[ChannelItem.PauseProperty] = "1";

            using (ChannelItem channel = new ChannelItem(channelInfo))
            {
                channel.ModuleLoader = moduleLoader;

                channel.Start();

                channel.Stop();

                testDataLoader.Verify(l => l.UnregisterSubscribe());

                //Thread.Sleep(5000);

                //var timesArray = callTimes.ToArray();

                //Assert.Greater(timesArray.Length, 4);

                //DateTime time = timesArray[0];

                //for (int i = 1; i < timesArray.Length; i++)
                //{
                //    Assert.AreEqual((timesArray[i] - time).Milliseconds, 1000, 5);
                //    time = timesArray[i];
                //} 
            }
        }

        //[Test]
        //public void Start_IsArchive_CallGetArchiveDataRepeadly() { Assert.Fail(); }

        //[Test]
        //public void Stop_IsArchive_StopCallGetArchiveDataRepeadly() { Assert.Fail(); }

        [Test]
        public void Start_IsArchive_CallGetArchive()
        {
            List<DateTime> callTimes = new List<DateTime>();

            var moduleLoader = new TestModuleLoader();

            var testDataLoader = new Moq.Mock<IDataLoader>();
            testDataLoader.Setup(l => l.LoadMethod).Returns(DataLoadMethod.Archive);
            //testDataLoader.Setup(l => l.GetCurrent()).Callback(() => callTimes.Add(DateTime.Now));

            var testDataFactory = new TestDataLoaderFactory();
            testDataFactory.DataLoader = testDataLoader.Object;

            moduleLoader.AddModule("module1", testDataFactory);

            ModuleInfo module = new ModuleInfo("module1", null, null, null);

            ChannelInfo channelInfo = new ChannelInfo()
            {
                Module = module,
                Parameters = new ParameterItem[]{
                    new ParameterItem()
                    {
                        Idnum = 1,
                        LastTime = new DateTime(2012,01,01)
                    },
                    new ParameterItem()
                    {
                        Idnum = 2,
                        LastTime = new DateTime(2012,03,01)
                    }
                },
            };
            channelInfo[CommonProperty.PauseProperty] = "1";
            channelInfo[CommonProperty.CaptureIntervalNormalProperty] = "60";
            channelInfo[CommonProperty.CaptureIntervalLargeProperty] = "600";

            using (ChannelItem channel = new ChannelItem(channelInfo))
            {
                channel.ModuleLoader = moduleLoader;

                channel.Start();

                Thread.Sleep(200);

                testDataLoader.Verify(l => l.GetArchive(
                    new DateTime(2012, 01, 01).AddMilliseconds(1),
                    new DateTime(2012, 01, 01).AddMilliseconds(1).AddSeconds(600)));
            }
        }

        [Test]
        public void Start_IsArchive_StartTimeAndLastTimeAreMinValues_SetStartTimeToNowMinusIntervalLarge()
        {
            List<DateTime> callTimes = new List<DateTime>();

            var moduleLoader = new TestModuleLoader();

            var testDataLoader = new Moq.Mock<IDataLoader>();
            testDataLoader.Setup(l => l.LoadMethod).Returns(DataLoadMethod.Archive);
            //testDataLoader.Setup(l => l.GetCurrent()).Callback(() => callTimes.Add(DateTime.Now));

            var testDataFactory = new TestDataLoaderFactory();
            testDataFactory.DataLoader = testDataLoader.Object;

            moduleLoader.AddModule("module1", testDataFactory);

            ModuleInfo module = new ModuleInfo("module1", null, null, null);

            ChannelInfo channelInfo = new ChannelInfo()
            {
                Module = module,
                StartTime = DateTime.MinValue,
                Parameters = new ParameterItem[]{
                    new ParameterItem()
                    {
                        Idnum = 1,
                        LastTime =DateTime.MinValue
                    },
                    new ParameterItem()
                    {
                        Idnum = 2,
                        LastTime = DateTime.MinValue
                    }
                },
            };
            channelInfo[CommonProperty.PauseProperty] = "1";
            channelInfo[CommonProperty.CaptureIntervalNormalProperty] = "60";
            channelInfo[CommonProperty.CaptureIntervalLargeProperty] = "600";

            using (ChannelItem channel = new ChannelItem(channelInfo))
            {
                channel.ModuleLoader = moduleLoader;

                DateTime now = DateTime.Now.AddSeconds(-600);

                channel.Start();

                Thread.Sleep(200);

                // StartTime are seted to now
                Assert.GreaterOrEqual(channelInfo.StartTime, now);
                Assert.Less((channelInfo.StartTime - now).TotalMilliseconds, 250);

                DateTime t = channelInfo.StartTime;

                testDataLoader.Verify(l => l.GetArchive(t, t.AddSeconds(60)));
            }
        }

        [Test]
        public void Start_IsArchive_LastTimePlusIntervalNormalGreaterNow_NotCallGetArchive()
        {
            List<DateTime> callTimes = new List<DateTime>();

            var moduleLoader = new TestModuleLoader();

            var testDataLoader = new Moq.Mock<IDataLoader>();
            testDataLoader.Setup(l => l.LoadMethod).Returns(DataLoadMethod.Archive);
            //testDataLoader.Setup(l => l.GetCurrent()).Callback(() => callTimes.Add(DateTime.Now));

            var testDataFactory = new TestDataLoaderFactory();
            testDataFactory.DataLoader = testDataLoader.Object;

            moduleLoader.AddModule("module1", testDataFactory);

            ModuleInfo module = new ModuleInfo("module1", null, null, null);

            DateTime lastTime = DateTime.Now.AddSeconds(-5);

            ChannelInfo channelInfo = new ChannelInfo()
            {
                Module = module,
                Parameters = new ParameterItem[]{
                    new ParameterItem()
                    {
                        Idnum = 1,
                        LastTime = lastTime
                    },
                    new ParameterItem()
                    {
                        Idnum = 2,
                        LastTime = lastTime
                    }
                },
            };
            channelInfo[CommonProperty.PauseProperty] = "1";
            channelInfo[CommonProperty.CaptureIntervalNormalProperty] = "60";
            channelInfo[CommonProperty.CaptureIntervalLargeProperty] = "600";

            using (ChannelItem channel = new ChannelItem(channelInfo))
            {
                channel.ModuleLoader = moduleLoader;

                channel.Start();

                Thread.Sleep(200);

                testDataLoader.Verify(l => l.GetArchive(Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>()), Moq.Times.Never());

                channel.Stop(); 
            }
        }

        [Test]
        public void Start_IsArchive_IntervalLagre_HasntValues_ParametersLastTimeAreSame_SendBadsIncreaseLastTime()
        {
            List<DateTime> callTimes = new List<DateTime>();

            var moduleLoader = new TestModuleLoader();

            var testDataLoader = new Moq.Mock<IDataLoader>();
            testDataLoader.Setup(l => l.LoadMethod).Returns(DataLoadMethod.Archive);

            var dataListener = new Moq.Mock<IDataListener>();
            //testDataLoader.Setup(l => l.GetCurrent()).Callback(() => callTimes.Add(DateTime.Now));

            var testDataFactory = new TestDataLoaderFactory();
            testDataFactory.DataLoader = testDataLoader.Object;

            moduleLoader.AddModule("module1", testDataFactory);

            ModuleInfo module = new ModuleInfo("module1", null, null, null);

            ParameterItem parameter1= new ParameterItem()
                    {
                        Idnum = 1,
                        LastTime = new DateTime(2012, 01, 01)
                    };
            ParameterItem parameter2=new ParameterItem()
                    {
                        Idnum = 2,
                        LastTime = new DateTime(2012, 01, 01)
                    };

            ChannelInfo channelInfo = new ChannelInfo()
            {
                Module = module,
                Parameters = new ParameterItem[]{
                    parameter1,
                    parameter2
                },
            };
            channelInfo[CommonProperty.PauseProperty] = "1";
            channelInfo[CommonProperty.CaptureIntervalNormalProperty] = "60";
            channelInfo[CommonProperty.CaptureIntervalLargeProperty] = "600";

            ;
            using (ChannelItem channel = new ChannelItem(channelInfo))
            {
                channel.ModuleLoader = moduleLoader;
                channel.DataListener = dataListener.Object;

                channel.Start();

                Thread.Sleep(200);

                testDataLoader.Verify(l => l.GetArchive(
                    new DateTime(2012, 01, 01).AddMilliseconds(1), 
                    new DateTime(2012, 01, 01).AddMilliseconds(1).AddSeconds(600)));

                dataListener.Verify(l => l.NotifyValues(Moq.It.IsAny<ChannelInfo>(), Moq.It.IsAny<ParameterItem>(), Moq.It.IsAny<IEnumerable<ParamValueItem>>()), Moq.Times.Never());

                Thread.Sleep(1000);

                dataListener.Verify(l => l.NotifyValues(channelInfo, parameter1, new ParamValueItem[] { new ParamValueItem(new DateTime(2012, 01, 01).AddMilliseconds(1), Quality.Bad, 0) }));
                dataListener.Verify(l => l.NotifyValues(channelInfo, parameter2, new ParamValueItem[] { new ParamValueItem(new DateTime(2012, 01, 01).AddMilliseconds(1), Quality.Bad, 0) }));
                testDataLoader.Verify(l => l.GetArchive(
                    new DateTime(2012, 01, 01).AddMilliseconds(1).AddSeconds(60),
                    new DateTime(2012, 01, 01).AddMilliseconds(1).AddSeconds(660)));

                Assert.AreEqual(new DateTime(2012, 01, 01).AddSeconds(60), parameter1.LastTime);
                Assert.AreEqual(new DateTime(2012, 01, 01).AddSeconds(60), parameter2.LastTime);
            }
        }

        [Test]
        public void Start_IsArchive_HasntValues_ParametersLastTimeAreMinValue_SendBadsIncreaseLastTime()
        {
            List<DateTime> callTimes = new List<DateTime>();

            var moduleLoader = new TestModuleLoader();

            var testDataLoader = new Moq.Mock<IDataLoader>();
            testDataLoader.Setup(l => l.LoadMethod).Returns(DataLoadMethod.Archive);

            var dataListener = new Moq.Mock<IDataListener>();
            //testDataLoader.Setup(l => l.GetCurrent()).Callback(() => callTimes.Add(DateTime.Now));

            var testDataFactory = new TestDataLoaderFactory();
            testDataFactory.DataLoader = testDataLoader.Object;

            moduleLoader.AddModule("module1", testDataFactory);

            ModuleInfo module = new ModuleInfo("module1", null, null, null);

            ParameterItem parameter1 = new ParameterItem()
            {
                Idnum = 1,
                LastTime =DateTime.MinValue
            };
            ParameterItem parameter2 = new ParameterItem()
            {
                Idnum = 2,
                LastTime = DateTime.MinValue
            };

            ChannelInfo channelInfo = new ChannelInfo()
            {
                Module = module,
                StartTime = new DateTime(2012, 01, 01),
                Parameters = new ParameterItem[]
                {
                    parameter1,
                    parameter2
                },
            };
            channelInfo[CommonProperty.PauseProperty] = "1";
            channelInfo[CommonProperty.CaptureIntervalNormalProperty] = "60";
            channelInfo[CommonProperty.CaptureIntervalLargeProperty] = "600";

            using (ChannelItem channel = new ChannelItem(channelInfo))
            {
                channel.ModuleLoader = moduleLoader;
                channel.DataListener = dataListener.Object;

                channel.Start();

                Thread.Sleep(200);

                testDataLoader.Verify(l => l.GetArchive(new DateTime(2012, 01, 01),
                                                        new DateTime(2012, 01, 01).AddSeconds(600)));

                dataListener.Verify(l => l.NotifyValues(Moq.It.IsAny<ChannelInfo>(), 
                                                        Moq.It.IsAny<ParameterItem>(),
                                                        Moq.It.IsAny<IEnumerable<ParamValueItem>>()), 
                                    Moq.Times.Never());

                Thread.Sleep(1000);

                channel.Stop();

                dataListener.Verify(l => l.NotifyValues(channelInfo, 
                                                        parameter1, 
                                                        new ParamValueItem[] { new ParamValueItem(new DateTime(2012, 01, 01), Quality.Bad, 0) }));
                
                dataListener.Verify(l => l.NotifyValues(channelInfo, 
                                                        parameter2, 
                                                        new ParamValueItem[] { new ParamValueItem(new DateTime(2012, 01, 01), Quality.Bad, 0) }));
                
                testDataLoader.Verify(l => l.GetArchive(new DateTime(2012, 01, 01).AddMilliseconds(1).AddSeconds(60),
                                                        new DateTime(2012, 01, 01).AddMilliseconds(1).AddSeconds(660)));

                Assert.AreEqual(new DateTime(2012, 01, 01).AddSeconds(60), parameter1.LastTime);
                Assert.AreEqual(new DateTime(2012, 01, 01).AddSeconds(60), parameter2.LastTime);
            }
        }

        [Test]
        public void Start_IsArchive_IntervalLarge_HasntValues_ParametersLattTimeArentSame_SetBadsSomeParameters()
        {
            List<DateTime> callTimes = new List<DateTime>();

            var moduleLoader = new TestModuleLoader();

            var testDataLoader = new Moq.Mock<IDataLoader>();
            testDataLoader.Setup(l => l.LoadMethod).Returns(DataLoadMethod.Archive);

            var dataListener = new Moq.Mock<IDataListener>();
            //testDataLoader.Setup(l => l.GetCurrent()).Callback(() => callTimes.Add(DateTime.Now));

            var testDataFactory = new TestDataLoaderFactory();
            testDataFactory.DataLoader = testDataLoader.Object;

            moduleLoader.AddModule("module1", testDataFactory);

            ModuleInfo module = new ModuleInfo("module1", null, null, null);

            ParameterItem parameter1 = new ParameterItem()
            {
                Idnum = 1,
                LastTime = new DateTime(2012, 01, 01)
            };
            ParameterItem parameter2 = new ParameterItem()
            {
                Idnum = 2,
                LastTime = new DateTime(2012, 01, 01).AddSeconds(30)
            };

            ChannelInfo channelInfo = new ChannelInfo()
            {
                Module = module,
                Parameters = new ParameterItem[]{
                    parameter1,
                    parameter2
                },
            };
            channelInfo[CommonProperty.PauseProperty] = "1";
            channelInfo[CommonProperty.CaptureIntervalNormalProperty] = "60";
            channelInfo[CommonProperty.CaptureIntervalLargeProperty] = "600";


            using (ChannelItem channel = new ChannelItem(channelInfo))
            {
                channel.ModuleLoader = moduleLoader;
                channel.DataListener = dataListener.Object;

                channel.Start();

                Thread.Sleep(200);

                testDataLoader.Verify(l => l.GetArchive(
                    new DateTime(2012, 01, 01).AddMilliseconds(1),
                    new DateTime(2012, 01, 01).AddMilliseconds(1).AddSeconds(600)));

                dataListener.Verify(l =>
                    l.NotifyValues(
                        Moq.It.IsAny<ChannelInfo>(),
                        Moq.It.IsAny<ParameterItem>(),
                        Moq.It.IsAny<IEnumerable<ParamValueItem>>()),
                    Moq.Times.Never());

                Thread.Sleep(1000);

                dataListener.Verify(l => l.NotifyValues(channelInfo, parameter1, new ParamValueItem[] { new ParamValueItem(new DateTime(2012, 01, 01).AddMilliseconds(1), Quality.Bad, 0) }));
                dataListener.Verify(l => l.NotifyValues(channelInfo, parameter2, Moq.It.IsAny<IEnumerable<ParamValueItem>>()), Moq.Times.Never());
                testDataLoader.Verify(l => l.GetArchive(
                    new DateTime(2012, 01, 01).AddMilliseconds(1).AddSeconds(30),
                    new DateTime(2012, 01, 01).AddMilliseconds(1).AddSeconds(630)));
            }
        }

        [Test]
        public void Start_IsArchiveByParameter_CallSetParameterTimeAndGetArchive()
        {
            var moduleLoader = new TestModuleLoader();

            var testDataLoader = new Moq.Mock<IDataLoader>();
            testDataLoader.Setup(l => l.LoadMethod).Returns(DataLoadMethod.ArchiveByParameter);

            var testDataFactory = new TestDataLoaderFactory();
            testDataFactory.DataLoader = testDataLoader.Object;

            moduleLoader.AddModule("module1", testDataFactory);

            ModuleInfo module = new ModuleInfo("module1", null, null, null);

            ParameterItem parameter1 = new ParameterItem()
            {
                Idnum = 1,
                LastTime = new DateTime(2012, 01, 01)
            };
            ParameterItem parameter2 = new ParameterItem()
            {
                Idnum = 2,
                LastTime = new DateTime(2012, 03, 01)
            };
            ChannelInfo channelInfo = new ChannelInfo()
            {
                Module = module,
                Parameters = new ParameterItem[]{
                    parameter1,
                    parameter2
                },
            };
            channelInfo[CommonProperty.PauseProperty] = "1";
            channelInfo[CommonProperty.CaptureIntervalNormalProperty] = "60";
            channelInfo[CommonProperty.CaptureIntervalLargeProperty] = "600";

            using (ChannelItem channel = new ChannelItem(channelInfo))
            {
                channel.ModuleLoader = moduleLoader;

                channel.Start();

                Thread.Sleep(200);

                testDataLoader.Verify(l =>
                    l.SetArchiveParameterTime(
                        parameter1,
                        new DateTime(2012, 01, 01).AddMilliseconds(1),
                        new DateTime(2012, 01, 01).AddMilliseconds(1).AddSeconds(600)));

                testDataLoader.Verify(l => 
                    l.SetArchiveParameterTime(
                        parameter2,
                        new DateTime(2012, 03, 01).AddMilliseconds(1),
                        new DateTime(2012, 03, 01).AddMilliseconds(1).AddSeconds(600)));

                testDataLoader.Verify(l => l.GetArchive());

                // не должны вызываться методы для других режимов опроса данных
                testDataLoader.Verify(l => 
                    l.GetArchive(
                        Moq.It.IsAny<DateTime>(),
                        Moq.It.IsAny<DateTime>()),
                    Moq.Times.Never());
            }
        }

        [Test]
        public void Start_IsArchiveByParameter_AParametersLastTimePlusIntervalNormalGreaterNow_SkipParameter()
        {
            var moduleLoader = new TestModuleLoader();

            var testDataLoader = new Moq.Mock<IDataLoader>();
            testDataLoader.Setup(l => l.LoadMethod).Returns(DataLoadMethod.ArchiveByParameter);

            var testDataFactory = new TestDataLoaderFactory();
            testDataFactory.DataLoader = testDataLoader.Object;

            moduleLoader.AddModule("module1", testDataFactory);

            ModuleInfo module = new ModuleInfo("module1", null, null, null);

            ParameterItem parameter1 = new ParameterItem()
            {
                Idnum = 1,
                LastTime = DateTime.Now.AddSeconds(-10)
            };
            ParameterItem parameter2 = new ParameterItem()
            {
                Idnum = 2,
                LastTime = new DateTime(2012, 03, 01)
            };
            ChannelInfo channelInfo = new ChannelInfo()
            {
                Module = module,
                Parameters = new ParameterItem[]{
                    parameter1,
                    parameter2
                },
            };
            channelInfo[CommonProperty.PauseProperty] = "1";
            channelInfo[CommonProperty.CaptureIntervalNormalProperty] = "60";
            channelInfo[CommonProperty.CaptureIntervalLargeProperty] = "600";

            using (ChannelItem channel = new ChannelItem(channelInfo))
            {
                channel.ModuleLoader = moduleLoader;

                channel.Start();

                Thread.Sleep(200);

                // выставление времени для первого параметра не должно вызыватся
                testDataLoader.Verify(l =>
                    l.SetArchiveParameterTime(
                        parameter1,
                        Moq.It.IsAny<DateTime>(),
                        Moq.It.IsAny<DateTime>()),
                    Moq.Times.Never());

                testDataLoader.Verify(l =>
                    l.SetArchiveParameterTime(
                        parameter2,
                        new DateTime(2012, 03, 01).AddMilliseconds(1),
                        new DateTime(2012, 03, 01).AddMilliseconds(1).AddSeconds(600)));

                testDataLoader.Verify(l => l.GetArchive());

                // не должны вызываться методы для других режимов опроса данных
                testDataLoader.Verify(l =>
                    l.GetArchive(
                        Moq.It.IsAny<DateTime>(),
                        Moq.It.IsAny<DateTime>()),
                    Moq.Times.Never());
            }
        }

        [Test]
        public void Start_IsArchiveByParameter_IntervalLarge_HasntValues_SetBadIncreaseLastTime()
        {
            var moduleLoader = new TestModuleLoader();

            var testDataLoader = new Moq.Mock<IDataLoader>();
            testDataLoader.Setup(l => l.LoadMethod).Returns(DataLoadMethod.ArchiveByParameter);

            var testDataFactory = new TestDataLoaderFactory();
            testDataFactory.DataLoader = testDataLoader.Object;
            
            var dataListener = new Moq.Mock<IDataListener>();

            moduleLoader.AddModule("module1", testDataFactory);

            ModuleInfo module = new ModuleInfo("module1", null, null, null);

            ParameterItem parameter1 = new ParameterItem()
            {
                Idnum = 1,
                LastTime = new DateTime(2012, 01, 01)
            };
            ParameterItem parameter2 = new ParameterItem()
            {
                Idnum = 2,
                LastTime = new DateTime(2012, 03, 01)
            };
            ChannelInfo channelInfo = new ChannelInfo()
            {
                Module = module,
                Parameters = new ParameterItem[]{
                    parameter1,
                    parameter2
                },
            };
            channelInfo[CommonProperty.PauseProperty] = "1";
            channelInfo[CommonProperty.CaptureIntervalNormalProperty] = "60";
            channelInfo[CommonProperty.CaptureIntervalLargeProperty] = "600";

            using (ChannelItem channel = new ChannelItem(channelInfo))
            {
                channel.ModuleLoader = moduleLoader;
                channel.DataListener = dataListener.Object;

                channel.Start();

                Thread.Sleep(200);

                testDataLoader.Verify(l =>
                    l.SetArchiveParameterTime(
                        parameter1,
                        new DateTime(2012, 01, 01).AddMilliseconds(1),
                        new DateTime(2012, 01, 01).AddMilliseconds(1).AddSeconds(600)));

                testDataLoader.Verify(l =>
                    l.SetArchiveParameterTime(
                        parameter2,
                        new DateTime(2012, 03, 01).AddMilliseconds(1),
                        new DateTime(2012, 03, 01).AddMilliseconds(1).AddSeconds(600)));

                testDataLoader.Verify(l => l.GetArchive());

                Thread.Sleep(1000);

                // выставлены бэды
                dataListener.Verify(l =>
                    l.NotifyValues(
                        channelInfo,
                        parameter1, 
                        new ParamValueItem[] 
                        { 
                            new ParamValueItem(new DateTime(2012, 01, 01).AddMilliseconds(1), Quality.Bad, 0.0) 
                        }));

                dataListener.Verify(l =>
                    l.NotifyValues(
                        channelInfo,
                        parameter2,
                        new ParamValueItem[] 
                        { 
                            new ParamValueItem(new DateTime(2012, 03, 01).AddMilliseconds(1), Quality.Bad, 0.0) 
                        }));

                // следующий запрос со смещением

                testDataLoader.Verify(l =>
                    l.SetArchiveParameterTime(
                        parameter1,
                        new DateTime(2012, 01, 01).AddSeconds(60).AddMilliseconds(1),
                        new DateTime(2012, 01, 01).AddSeconds(60).AddMilliseconds(1).AddSeconds(600)));

                testDataLoader.Verify(l =>
                    l.SetArchiveParameterTime(
                        parameter2,
                        new DateTime(2012, 03, 01).AddSeconds(60).AddMilliseconds(1),
                        new DateTime(2012, 03, 01).AddSeconds(60).AddMilliseconds(1).AddSeconds(600)));

                testDataLoader.Verify(l => l.GetArchive());
            }
        }

        [Test]
        public void GetParamList__CallDataLoaderGetParams()
        {
            var moduleLoader = new TestModuleLoader();

            var testDataLoader = new Moq.Mock<IDataLoader>();
            testDataLoader.Setup(l => l.GetParameters()).Returns(new ParameterItem[] 
            { 
                new ParameterItem()
                {
                    Idnum = 1,
                    Name = "parameter1",
                    LastTime = DateTime.MinValue,                    
                },
                new ParameterItem()
                {
                    Idnum = 2,
                    Name = "parameter2",
                    LastTime = DateTime.MinValue
                }
            });

            var testDataFactory = new TestDataLoaderFactory();
            testDataFactory.DataLoader = testDataLoader.Object;

            moduleLoader.AddModule("module1", testDataFactory);

            using (ChannelItem channel = new ChannelItem(new ChannelInfo() { Module = new ModuleInfo("module1", "", null, null) }))
            {
                channel.ModuleLoader = moduleLoader;

                var parameters = channel.GetParamList();

                testDataLoader.Verify(l => l.GetParameters());

                Assert.IsNotNull(parameters);
                Assert.AreEqual(2, parameters.Length); 
            }
        }

        [Test]
        public void GetParamList__CallInitOnce()
        {
            var moduleLoader = new TestModuleLoader();

            var testDataLoader = new Moq.Mock<IDataLoader>();
            testDataLoader.Setup(l => l.GetParameters()).Returns(new ParameterItem[] 
            { 
                new ParameterItem()
                {
                    Idnum = 1,
                    Name = "parameter1",
                    LastTime = DateTime.MinValue,                    
                },
                new ParameterItem()
                {
                    Idnum = 2,
                    Name = "parameter2",
                    LastTime = DateTime.MinValue
                }
            });

            var testDataFactory = new TestDataLoaderFactory();
            testDataFactory.DataLoader = testDataLoader.Object;

            moduleLoader.AddModule("module1", testDataFactory);

            ChannelInfo channelInfo=new ChannelInfo() { Module = new ModuleInfo("module1", "", null, null) };

            using (ChannelItem channel = new ChannelItem(channelInfo))
            {
                channel.ModuleLoader = moduleLoader;

                // first call GetParamList
                var parameters = channel.GetParamList();

                testDataLoader.Verify(l => l.Init(channelInfo), Moq.Times.Once());

                testDataLoader.Verify(l => l.GetParameters(), Moq.Times.Once());

                // second call GetParamList
                parameters = channel.GetParamList();

                // still called once
                testDataLoader.Verify(l => l.Init(channelInfo), Moq.Times.Once());

                // called two times
                testDataLoader.Verify(l => l.GetParameters(), Moq.Times.Exactly(2));
            }
        }

        [Test]
        public void GetParamList_catchNotSupportedException_returnEmptyArray()
        {
            var moduleLoader = new TestModuleLoader();

            var testDataLoader = new Moq.Mock<IDataLoader>();
            testDataLoader.Setup(l => l.GetParameters()).Throws(new NotSupportedException());

            var testDataFactory = new TestDataLoaderFactory();
            testDataFactory.DataLoader = testDataLoader.Object;

            moduleLoader.AddModule("module1", testDataFactory);

            ChannelInfo channelInfo = new ChannelInfo() { Module = new ModuleInfo("module1", "", null, null) };

            using (ChannelItem channel = new ChannelItem(channelInfo))
            {
                channel.ModuleLoader = moduleLoader;

                // first call GetParamList
                var parameters = channel.GetParamList();

                Assert.IsNotNull(parameters);
                Assert.AreEqual(0, parameters.Length);
            }
        }
    }
}
