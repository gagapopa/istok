using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.ASC.Report
{
    [TestFixture]
    public class SimpleReportSourceSettingsTests
    {
        [Test]
        public void ToBytes_FromBytes_Lossless()
        {
            SimpleReportSourceSettings setting = new SimpleReportSourceSettings((ReportSourceInfo)null);
            SimpleReportSourceSettings result = new SimpleReportSourceSettings((ReportSourceInfo)null);

            setting.Enabled = true;

            byte[] bytes = setting.ToBytes();

            Assert.IsNotNull(bytes);

            result.FromBytes(bytes);

            Assert.AreEqual(setting.Enabled, result.Enabled);
        }
    }
}
