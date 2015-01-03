using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace COTES.ISTOK.Tests
{
    [TestFixture]
    public class IntervalTests
    { 
        [Test]
        public void GetTime_localInterval_()
        {
            var interval = Interval.FromString("[UTC+08]=1d");

            Assert.AreEqual("[UTC+08]=1d", interval.ToString());

            var time = new DateTime(2012, 07, 01);

            Assert.AreEqual(new DateTime(2012, 07, 02), interval.GetNextTime(time));
            Assert.AreEqual(new DateTime(2012, 06, 30), interval.GetPrevTime(time));

            time = new DateTime(2012, 07, 01, 04, 10, 30);

            Assert.AreEqual(new DateTime(2012, 07, 01), interval.NearestEarlierTime(time));
            Assert.AreEqual(new DateTime(2012, 07, 02), interval.NearestLaterTime(time));
        }

        [Test]
        public void GetTime_MSK_()
        {
            var interval = Interval.FromString("[UTC+04]=1d");
            TimeZoneInfo mskZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

            Assert.AreEqual("[UTC+04]=1d", interval.ToString());

            var time = TimeZoneInfo.ConvertTime(new DateTime(2012, 07, 01), mskZone, TimeZoneInfo.Local);
            //var time = new DateTime(2012, 07, 01);

            Assert.AreEqual(new DateTime(2012, 07, 02),
                            TimeZoneInfo.ConvertTime(interval.GetNextTime(time), TimeZoneInfo.Local, mskZone));
            Assert.AreEqual(new DateTime(2012, 06, 30),
                            TimeZoneInfo.ConvertTime(interval.GetPrevTime(time), TimeZoneInfo.Local, mskZone));

            //time = new DateTime(2012, 07, 01, 04, 10, 30);
            time = TimeZoneInfo.ConvertTime(new DateTime(2012, 07, 01, 00, 10, 30), mskZone, TimeZoneInfo.Local);

            Assert.AreEqual(new DateTime(2012, 07, 01, 00, 00, 00),
                            TimeZoneInfo.ConvertTime(interval.NearestEarlierTime(time), TimeZoneInfo.Local, mskZone));
            Assert.AreEqual(new DateTime(2012, 07, 02, 00, 00, 00),
                            TimeZoneInfo.ConvertTime(interval.NearestLaterTime(time), TimeZoneInfo.Local, mskZone));
        }

        [Test]
        public void GetTime_MSK_UTCInput_()
        {
            var interval = Interval.FromString("[UTC+04]=1d");

            Assert.AreEqual("[UTC+04]=1d", interval.ToString());

            var time = new DateTime(2012, 07, 01, 00, 00, 00, DateTimeKind.Utc);

            Assert.AreEqual(new DateTime(2012, 07, 01, 20, 00, 00), interval.GetNextTime(time));
            Assert.AreEqual(new DateTime(2012, 06, 29, 20, 00, 00), interval.GetPrevTime(time));

            time = new DateTime(2012, 07, 01, 04, 10, 30, DateTimeKind.Utc);

            Assert.AreEqual(new DateTime(2012, 06, 30, 20, 00, 00), interval.NearestEarlierTime(time));
            Assert.AreEqual(new DateTime(2012, 07, 01, 20, 00, 00), interval.NearestLaterTime(time));
        }

        [Test]
        public void GetTime_Watch_()
        {
            var interval = Interval.FromString("[UTC+08]8h=12h");
            TimeZoneInfo mskZone = TimeZoneInfo.FindSystemTimeZoneById("North Asia Standard Time");

            Assert.AreEqual("[UTC+08]8h=12h", interval.ToString());

            DateTime time;
            //time = new DateTime(2012, 07, 01);
            time = TimeZoneInfo.ConvertTime(new DateTime(2012, 07, 01, 08, 00, 00), mskZone, TimeZoneInfo.Local);

            DateTime retTime = interval.GetNextTime(time);
            DateTime correctTime = TimeZoneInfo.ConvertTime(retTime, TimeZoneInfo.Local, mskZone);
            Assert.AreEqual(new DateTime(2012, 07, 01, 20, 00, 00), correctTime);

            retTime = interval.GetPrevTime(time);
            correctTime = TimeZoneInfo.ConvertTime(retTime, TimeZoneInfo.Local, mskZone);
            Assert.AreEqual(new DateTime(2012, 06, 30, 20, 00, 00), correctTime);

            //time = new DateTime(2012, 07, 01, 04, 10, 30);
            time = TimeZoneInfo.ConvertTime(new DateTime(2012, 07, 01, 04, 10, 30), mskZone, TimeZoneInfo.Local);

            retTime = interval.NearestEarlierTime(time);
            correctTime = TimeZoneInfo.ConvertTime(retTime, TimeZoneInfo.Local, mskZone);
            Assert.AreEqual(new DateTime(2012, 06, 30, 20, 00, 00), correctTime);

            retTime = interval.GetNextTime(retTime);
            correctTime = TimeZoneInfo.ConvertTime(retTime, TimeZoneInfo.Local, mskZone);
            Assert.AreEqual(new DateTime(2012, 07, 01, 08, 00, 00), correctTime);

            retTime = interval.GetNextTime(retTime);
            correctTime = TimeZoneInfo.ConvertTime(retTime, TimeZoneInfo.Local, mskZone);
            Assert.AreEqual(new DateTime(2012, 07, 01, 20, 00, 00), correctTime);

            retTime = interval.GetNextTime(retTime);
            correctTime = TimeZoneInfo.ConvertTime(retTime, TimeZoneInfo.Local, mskZone);
            Assert.AreEqual(new DateTime(2012, 07, 02, 08, 00, 00), correctTime);
        }

        [Test]
        public void GetTime_Watch_Manual_()
        {
            var interval = Interval.FromString("[UTC+04]=1d-4h-12h-8h");

            Assert.AreEqual("[UTC+04]=1d-4h-12h-8h", interval.ToString());

            var time = new DateTime(2012, 07, 01);

            Assert.AreEqual(new DateTime(2012, 07, 01, 04, 00, 00), interval.GetNextTime(time));
            Assert.AreEqual(new DateTime(2012, 06, 30, 08, 00, 00), interval.GetPrevTime(time));

            time = new DateTime(2012, 07, 01, 04, 10, 30);

            time = interval.NearestEarlierTime(time);
            Assert.AreEqual(new DateTime(2012, 07, 01, 04, 00, 00), time);

            time = interval.GetNextTime(time);
            Assert.AreEqual(new DateTime(2012, 07, 01, 08, 00, 00), time);

            time = interval.GetNextTime(time);
            Assert.AreEqual(new DateTime(2012, 07, 01, 20, 00, 00), time);

            time = interval.GetNextTime(time);
            Assert.AreEqual(new DateTime(2012, 07, 02, 04, 00, 00), time);
        }

        [Test]
        public void GetInterval_ExactlyMonthLap_ReturnIntervalMonth()
        {
            var interval = Interval.GetInterval(
                new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc),
                new DateTime(2012, 02, 01, 00, 00, 00, DateTimeKind.Utc));

            Assert.AreEqual(Interval.Month, interval);

            interval = Interval.GetInterval(
                new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc),
                new DateTime(2012, 02, 01, 01, 00, 00, DateTimeKind.Utc));

            Assert.AreEqual(Interval.FromString("=31d1h"), interval);
        }

        [Test]
        public void GetQueryItems__()
        {
            // сутки
            var interval = Interval.FromString("[local]=1d");

            // для локального времени
            int count = interval.GetQueryValues(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            Assert.AreEqual(31, count);

            // для времени по UTC
            count = interval.GetQueryValues(new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 02, 01, 00, 00, 00, DateTimeKind.Utc));

            Assert.AreEqual(32, count);

            // вахта
            interval = Interval.FromString("[local]8h=12h");

            // для локального времени
            count = interval.GetQueryValues(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            Assert.AreEqual(62, count);

            // для времени по UTC
            count = interval.GetQueryValues(new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 02, 01, 00, 00, 00, DateTimeKind.Utc));

            Assert.AreEqual(62, count);

            // хитрый ручной ввод по вахтам
            interval = Interval.FromString("[UTC+04]=1d-4h-12h-8h");

            // для локального времени
            count = interval.GetQueryValues(new DateTime(2012, 01, 01), new DateTime(2012, 01, 02));

            Assert.AreEqual(3, count);

            // для времени по UTC
            count = interval.GetQueryValues(new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 01, 02, 00, 00, 00, DateTimeKind.Utc));

            Assert.AreEqual(3, count);

            // десятидневки
            interval = Interval.FromString("[local]=1M-10d-10d");

            // для локального времени
            count = interval.GetQueryValues(new DateTime(2012, 01, 01), new DateTime(2012, 02, 01));

            Assert.AreEqual(3, count);

            // для времени по UTC
            count = interval.GetQueryValues(new DateTime(2012, 01, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2012, 02, 01, 00, 00, 00, DateTimeKind.Utc));

            Assert.AreEqual(3, count);
        }
    }
}
