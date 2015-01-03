using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using NUnit.Framework;

namespace COTES.ISTOK.Tests.ASC.Report
{
    [TestFixture]
    public class StructureReportSourceSettingsTests
    {
        [Test]
        public void ToBytes_FromBytes_Lossless()
        {
            StructureReportSourceSettings setting = new StructureReportSourceSettings((ReportSourceInfo)null);
            StructureReportSourceSettings result = new StructureReportSourceSettings((ReportSourceInfo)null);

            setting.Enabled = true;

            setting.Tables = FillStructureReportTables(true);

            byte[] bytes = setting.ToBytes();

            Assert.IsNotNull(bytes);

            result.FromBytes(bytes);

            Assert.AreEqual(setting.Enabled, result.Enabled);
            Assert.AreEqual(setting.TablesCount, result.TablesCount);

            for (int i = 0; i < setting.TablesCount; i++)
            {
                Assert.AreEqual(setting.Tables[i].TableName, result.Tables[i].TableName);
                Assert.AreEqual(setting.Tables[i].RootNode, result.Tables[i].RootNode);
                Assert.AreEqual(setting.Tables[i].RootFromAnotherTable, result.Tables[i].RootFromAnotherTable);
                Assert.AreEqual(setting.Tables[i].RootNodeID, result.Tables[i].RootNodeID);
                Assert.AreEqual(setting.Tables[i].RootNodeMinLevel, result.Tables[i].RootNodeMinLevel);
                Assert.AreEqual(setting.Tables[i].RootNodeMaxLevel, result.Tables[i].RootNodeMaxLevel);

                Assert.AreEqual(setting.Tables[i].RootNodeTypeFilter.Length, result.Tables[i].RootNodeTypeFilter.Length);
                for (int j = 0; j < setting.Tables[i].RootNodeTypeFilter.Length; j++)
                {
                    Assert.AreEqual(setting.Tables[i].RootNodeTypeFilter[j], result.Tables[i].RootNodeTypeFilter[j]);

                }

                Assert.AreEqual(setting.Tables[i].QueryValues, result.Tables[i].QueryValues);
                Assert.AreEqual(setting.Tables[i].NodeMinLevel, result.Tables[i].NodeMinLevel);
                Assert.AreEqual(setting.Tables[i].NodeMaxLevel, result.Tables[i].NodeMaxLevel);

                Assert.AreEqual(setting.Tables[i].NodeTypeFilter.Length, result.Tables[i].NodeTypeFilter.Length);
                for (int j = 0; j < setting.Tables[i].NodeTypeFilter.Length; j++)
                {
                    Assert.AreEqual(setting.Tables[i].NodeTypeFilter[j], result.Tables[i].NodeTypeFilter[j]);

                }
            }
        }

        [Test]
        public void QueryValues_NoOneTableQueryValues_False()
        {
            StructureReportSourceSettings setting = new StructureReportSourceSettings((ReportSourceInfo)null);

            setting.Tables = FillStructureReportTables(false);

            Assert.IsFalse(setting.QueryValues);
        }

        public void QueryValues_AnyOneTableQueryValues_True()
        {
            StructureReportSourceSettings setting = new StructureReportSourceSettings((ReportSourceInfo)null);

            setting.Tables = FillStructureReportTables(true);

            Assert.IsTrue(setting.QueryValues);
        }

        [Test]
        public void References_QueryValuesIsFalse_Empty()
        {
            StructureReportSourceSettings setting = new StructureReportSourceSettings((ReportSourceInfo)null);

            setting.ValueSourceSetting = new Guid();
            setting.Tables = FillStructureReportTables(false);

            Assert.IsTrue(setting.References == null || setting.References.Length == 0);
        }

        [Test]
        public void References_QueryValuesIsTrue_ValuesSourceSetting()
        {
            StructureReportSourceSettings setting = new StructureReportSourceSettings((ReportSourceInfo)null);

            setting.ValueSourceSetting = new Guid();
            setting.Tables = FillStructureReportTables(true);

            Assert.IsNotNull(setting.References);
            Assert.AreEqual(1, setting.References.Length);
            Assert.AreEqual(setting.ValueSourceSetting, setting.References[0]);
        }

        private static StructureReportTable[] FillStructureReportTables(bool queryValues)
        {
            StructureReportSourceSettings sourceSettings = null;
            StructureReportTable[] tables = new StructureReportTable[] { 
                new StructureReportTable()//sourceSettings)
                {
                    TableName = "table1",
                    RootNode = StructureReportTable.RootNodeMethod.AnotherTable,
                    RootFromAnotherTable =3,
                    RootNodeID =34,
                    RootNodeMinLevel =-1,
                    RootNodeMaxLevel =0,
                    RootNodeTypeFilter =new int[]{ (int)UnitTypeId.Channel},
                    QueryValues =false,
                    NodeMinLevel =0,
                    NodeMaxLevel =3,
                    NodeTypeFilter =new int[]{ (int)UnitTypeId.Parameter}
                },
                new StructureReportTable()//sourceSettings)
                {
                    TableName = "table2",
                    RootNode = StructureReportTable.RootNodeMethod.PreSet,
                    RootFromAnotherTable =3,
                    RootNodeID =78,
                    RootNodeMinLevel =3,
                    RootNodeMaxLevel =7,
                    RootNodeTypeFilter =new int[]{ (int)UnitTypeId.Channel},
                    QueryValues =false,
                    NodeMinLevel =0,
                    NodeMaxLevel =3,
                    NodeTypeFilter =new int[]{ (int)UnitTypeId.Parameter}
                },
                new StructureReportTable()//sourceSettings)
                {
                    TableName = "table3",
                    RootNode = StructureReportTable.RootNodeMethod.AnotherTable,
                    RootFromAnotherTable =3,
                    RootNodeID =34,
                    RootNodeMinLevel =-1,
                    RootNodeMaxLevel =0,
                    RootNodeTypeFilter =new int[]{ (int)UnitTypeId.Channel},
                    QueryValues =queryValues,
                    NodeMinLevel =0,
                    NodeMaxLevel =3,
                    NodeTypeFilter =new int[]{ (int)UnitTypeId.Parameter}
                },
                new StructureReportTable()//sourceSettings)
                {
                    TableName = "table4",
                    RootNode = StructureReportTable.RootNodeMethod.Request,
                    RootFromAnotherTable =3,
                    RootNodeID =34,
                    RootNodeMinLevel =-1,
                    RootNodeMaxLevel =0,
                    RootNodeTypeFilter =new int[]{ (int)UnitTypeId.Channel},
                    QueryValues =false,
                    NodeMinLevel =0,
                    NodeMaxLevel =3,
                    NodeTypeFilter =new int[]{ (int)UnitTypeId.Parameter}
                }
            };
            return tables;
        }
    }
}
