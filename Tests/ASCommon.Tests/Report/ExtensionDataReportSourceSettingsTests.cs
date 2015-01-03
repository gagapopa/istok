using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Extension;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.ASC.Report
{
    [TestFixture]
    public class ExtensionDataReportSourceSettingsTests
    {
        [Test]
        public void ToBytes_FromBytes_Lossless()
        {
            ExtensionDataReportSourceSettings setting = new ExtensionDataReportSourceSettings((ReportSourceInfo)null);
            ExtensionDataReportSourceSettings result = new ExtensionDataReportSourceSettings((ReportSourceInfo)null);

            ExtensionDataInfo firstInfo = new ExtensionDataInfo("First Info", ExtensionDataType.Graph);
            ExtensionDataInfo secondInfo = new ExtensionDataInfo("Second Info", ExtensionDataType.Histogram);
            ExtensionDataInfo thirdInfo = new ExtensionDataInfo("Third Info", ExtensionDataType.Graph);
            ExtensionDataInfo fourthInfo = new ExtensionDataInfo("Fourth Info", ExtensionDataType.Table);
            ExtensionDataInfo fifthInfo = new ExtensionDataInfo("Fifth Info", ExtensionDataType.Graph);

            ExtensionInfo extensionInfo = new ExtensionInfo(1, "test extension");

            firstInfo.ExtensionInfo = extensionInfo;
            secondInfo.ExtensionInfo = extensionInfo;
            thirdInfo.ExtensionInfo = extensionInfo;
            fourthInfo.ExtensionInfo = extensionInfo;
            fifthInfo.ExtensionInfo = extensionInfo;

            setting.Enabled = true;

            setting.CheckTabInfo(firstInfo,true);
            setting.CheckTabInfo(thirdInfo, true);
            setting.CheckTabInfo(fourthInfo, true);
            setting.StructTableName = "extension";

            byte[] bytes = setting.ToBytes();

            Assert.IsNotNull(bytes);

            result.FromBytes(bytes);

            Assert.AreEqual(setting.Enabled, result.Enabled);
            Assert.IsTrue(setting.Checked(firstInfo));
            Assert.IsFalse(setting.Checked(secondInfo));
            Assert.IsTrue(setting.Checked(thirdInfo));
            Assert.IsTrue(setting.Checked(fourthInfo));
            Assert.IsFalse(setting.Checked(fifthInfo));
        }

        [Test]
        public void References_AlwaysValuesAndStructureGUID()
        {
            ExtensionDataReportSourceSettings setting = new ExtensionDataReportSourceSettings((ReportSourceInfo)null);

            setting.ValueSourceSetting = new Guid();
            setting.StructureSourceSetting = new Guid();

            Assert.IsNotNull(setting.References);
            Assert.AreEqual(2, setting.References.Length);
            Assert.AreEqual(setting.ValueSourceSetting, setting.References[0]);
            Assert.AreEqual(setting.StructureSourceSetting, setting.References[1]);
        }
    }
}
