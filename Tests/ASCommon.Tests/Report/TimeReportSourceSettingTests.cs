using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.ASC.Report
{
    [TestFixture]
    public class TimeReportSourceSettingTests
    {
        [Test]
        public void ToBytes_FromBytes_Lossless()
        {
            TimeReportSourceSetting setting = new TimeReportSourceSetting((ReportSourceInfo)null);
            TimeReportSourceSetting result = new TimeReportSourceSetting((ReportSourceInfo)null);

            setting.Enabled = true;

            setting.ReportInterval = ReportTimeInterval.Mohtly;

            byte[] bytes = setting.ToBytes();

            Assert.IsNotNull(bytes);

            result.FromBytes(bytes);

            Assert.AreEqual(setting.Enabled, result.Enabled);
            Assert.AreEqual(setting.ReportInterval, result.ReportInterval);
        }
    }
}
