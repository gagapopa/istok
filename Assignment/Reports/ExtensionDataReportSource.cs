using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Assignment.Extension;
using System.Data;
using COTES.ISTOK.Extension;

namespace COTES.ISTOK.Assignment
{
    class ExtensionDataReportSource : IReportSource
    {
        ReportSourceInfo info;
        static readonly Guid ExtendedTableReportSourceID = new Guid("{E07125F8-880A-450c-93FE-001A16662BF6}");

        IUnitManager unitManager;
        //SecurityManager securityManager;
        ExtensionManager extensionManager;

        StructureReportSource structureSource;
        TimeReportSource timeReportSource;

        public ExtensionDataReportSource(
            IUnitManager unitManager,
            //SecurityManager smanager,
            ExtensionManager extensionManager)
        {
            info = new ReportSourceInfo(ExtendedTableReportSourceID, true, "Данные из расширений");
            this.unitManager = unitManager;
            //this.securityManager = smanager;
            this.extensionManager = extensionManager;
        }

        #region IReportSource Members

        public ReportSourceInfo Info
        {
            get { return info; }
        }

        public Guid[] References
        {
            get
            {
                return new Guid[] { 
                    TimeReportSource.TimeReportSourceID,
                    StructureReportSource.StructureReportSourceID 
                };
            }
        }

        public void SetReference(IReportSource source)
        {
            StructureReportSource structureReportSource = source as StructureReportSource;
            TimeReportSource valueReportSource = source as TimeReportSource;

            if (valueReportSource != null)
                timeReportSource = valueReportSource;

            if (structureReportSource != null)
                structureSource = structureReportSource;
        }

        public ReportSourceSettings CreateSettingsObject()
        {
            ExtensionDataReportSourceSettings settings = new ExtensionDataReportSourceSettings(info);

            settings.ValueSourceSetting = timeReportSource.Info.ReportSourceId;
            settings.StructureSourceSetting = structureSource.Info.ReportSourceId;
            return settings;
        }

        public void GenerateData(OperationState state, DataSet dataSet, ReportSourceSettings settings, params ReportParameter[] reportParameters)
        {
            const String parentNodeIDColumnName = "id";
            const String childNodeIDColumnName = "node_id";

            DateTime startTime, endTime;

            ExtensionDataReportSourceSettings extendedSettings = settings as ExtensionDataReportSourceSettings;
            TimeReportSourceSetting timeReportSettings = settings.GetReference(timeReportSource.Info.ReportSourceId) as TimeReportSourceSetting;
            StructureReportSourceSettings structureSettings = settings.GetReference(structureSource.Info.ReportSourceId) as StructureReportSourceSettings;

            if (extendedSettings != null)
            {
                timeReportSettings.GetReportInterval(reportParameters, out startTime, out endTime);

                Dictionary<String, DataTable> dataTableDictionary = new Dictionary<String, DataTable>();

                var commonExtensionInfoList = new List<ExtensionDataInfo>();
                var extensionInfoList = new List<ExtensionDataInfo>();

                foreach (var item in extendedSettings.ExtensionTableNames)
                {
                    ExtensionDataInfo extensionDataInfo = extensionManager.GetTabInfo(item.Key, item.Value);
                    if (extensionDataInfo.IsCommon)
                        commonExtensionInfoList.Add(extensionDataInfo);
                    else
                        extensionInfoList.Add(extensionDataInfo);
                }

                // Заполняем общие таблицы
                DataTable constructiveTable = dataSet.Tables[extendedSettings.StructTableName]; //structureSource.Info.Caption];
                if (constructiveTable != null)
                {
                    List<int> idList = new List<int>();

                    foreach (DataRow item in constructiveTable.Rows)
                    {
                        int id = Convert.ToInt32(item[parentNodeIDColumnName]);
                        idList.Add(id);
                    }
                    DataColumn parentColumn = constructiveTable.Columns[parentNodeIDColumnName]; 
                    
                    foreach (var extensionInfo in commonExtensionInfoList)//extendedSettings.ExtendedTableNames)
                    {
                        ExtensionData extendedTable = extensionManager.GetTab(extensionInfo, startTime, endTime);

                        if (extendedTable != null && extendedTable.Table != null)
                        {
                            String tableName = ExtensionTablName(extensionInfo);
                            extendedTable.Table.TableName = tableName;
                            dataTableDictionary[tableName] = extendedTable.Table;
                        }
                    }

                    // Заполняем таблицы по узлам
                    DataTable dataTable;
                    foreach (int id in idList)
                    {
                        ExtensionUnitNode node = unitManager.ValidateUnitNode<ExtensionUnitNode>(new OperationState(), id, Privileges.Read);

                        if (node != null)
                        {
                            foreach (var extensionInfo in extensionInfoList)//extendedSettings.ExtendedTableNames)
                            {
                                ExtensionData extendedTable = extensionManager.GetTab(node, extensionInfo, startTime, endTime);

                                if (extendedTable != null && extendedTable.Table != null)
                                {
                                    String tableName = ExtensionTablName(extensionInfo);
                                    if (!dataTableDictionary.TryGetValue(tableName, out dataTable))
                                    {
                                        dataTableDictionary[tableName] = dataTable = new DataTable(tableName);
                                        dataTable.Columns.Add(childNodeIDColumnName, typeof(int));
                                    }
                                    extendedTable.Table.Columns.Add(childNodeIDColumnName, typeof(int));
                                    foreach (DataRow item in extendedTable.Table.Rows)
                                    {
                                        item[childNodeIDColumnName] = id;
                                    }

                                    dataTable.Merge(extendedTable.Table);
                                }
                            }
                        }
                    }

                    foreach (var item in dataTableDictionary.Values)
                    {
                        dataSet.Tables.Add(item);
                        if (item.Columns.Contains(childNodeIDColumnName))
                        {
                            DataColumn childColumn = item.Columns[childNodeIDColumnName];
                            dataSet.Relations.Add(parentColumn, childColumn);
                        }
                    }
                }
            }
        }

        String ExtensionTablName(ExtensionDataInfo extensionDataInfo)
        {
            return String.Format("{0}_{1}", extensionDataInfo.ExtensionInfo.Caption, extensionDataInfo.Name);
        }

        public void GenerateEmptyData(OperationState state, DataSet dataSet, ReportSourceSettings settings)
        {            
        }

        #endregion
    }
}
