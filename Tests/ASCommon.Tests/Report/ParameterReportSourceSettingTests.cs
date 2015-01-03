using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.ASC.Report
{
    [TestFixture]
    public class ParameterReportSourceSettingTests
    {
        [Test]
        public void ToBytes_FromBytes_Lossless()
        {
            ParameterReportSourceSetting setting = new ParameterReportSourceSetting((ReportSourceInfo)null);
            ParameterReportSourceSetting result = new ParameterReportSourceSetting((ReportSourceInfo)null);

            setting.Enabled = true;

            setting.ParameterIds = new int[] { 16, 345, 234 };

            byte[] bytes = setting.ToBytes();

            Assert.IsNotNull(bytes);

            result.FromBytes(bytes);

            Assert.AreEqual(setting.Enabled, result.Enabled);
            Assert.AreEqual(setting.ParameterIds.Length, result.ParameterIds.Length);
            for (int i = 0; i < setting.ParameterIds.Length; i++)
            {
                Assert.AreEqual(setting.ParameterIds[i], result.ParameterIds[i]);
            }
        }

        [Test]
        public void References_AlwaysValuesGuid()
        {
            ParameterReportSourceSetting setting = new ParameterReportSourceSetting((ReportSourceInfo)null);

            setting.ValueSourceSetting = new Guid();

            Assert.IsNotNull(setting.References);
            Assert.AreEqual(1, setting.References.Length);
            Assert.AreEqual(setting.ValueSourceSetting, setting.References[0]);
        }
    }
}
