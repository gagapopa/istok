using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using System.Data;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Источник данных конкретных параметров
    /// </summary>
    class ParameterReportSource : IReportSource
    {
        ReportSourceInfo info;
        static readonly Guid StructureReportSourceID = new Guid("{CBE5068A-FFB5-44be-A11C-6DA882097DC7}");

        IUnitManager unitManager;

        ValueReceiver valueReceiver;

        TimeReportSource valueSource;

        public ParameterReportSource(IUnitManager unitManager, ValueReceiver vreceiver)
        {
            info = new ReportSourceInfo(StructureReportSourceID, true, "Значения параметров");
            this.unitManager = unitManager;
            this.valueReceiver = vreceiver;
        }

        #region IReportSource Members

        public ReportSourceInfo Info
        {
            get { return info; }
        }

        public Guid[] References
        {
            get { return new Guid[] { TimeReportSource.TimeReportSourceID }; }
        }

        public void SetReference(IReportSource source)
        {
            TimeReportSource reportSource = source as TimeReportSource;

            if (reportSource != null)
                valueSource = reportSource;
        }

        public ReportSourceSettings CreateSettingsObject()
        {
            ParameterReportSourceSetting settings = new ParameterReportSourceSetting(info);

            if (valueSource != null)
                settings.ValueSourceSetting = valueSource.Info.ReportSourceId;

            return settings;
        }

        public void GenerateData(OperationState state, DataSet dataSet, ReportSourceSettings settings, params ReportParameter[] reportParameters)
        {
            if (valueSource != null)
            {
                ParameterReportSourceSetting reportSetting = settings as ParameterReportSourceSetting;
                TimeReportSourceSetting timeReportSettings = settings.GetReference(valueSource.Info.ReportSourceId) as TimeReportSourceSetting;

                if (reportSetting != null && timeReportSettings != null)
                {
                    DateTime timeFrom, timeTo;
                    timeReportSettings.GetReportInterval(reportParameters, out timeFrom, out timeTo);
                    List<ParameterNode> parameterList = GetParameters(state, reportSetting);

                    valueReceiver.AsyncGetValues(state,
                                                 0f,
                                                 (from p in parameterList
                                                  select new ParameterValuesRequest()
                                                  {
                                                      Parameters = new Tuple<ParameterNode, ArgumentsValues>[]
                                                      {
                                                          Tuple.Create(p, (ArgumentsValues)null)
                                                      },
                                                      StartTime = timeFrom,
                                                      EndTime = timeTo,
                                                  }).ToArray(),
                                                 true,
                                                 true);

                    Package[] packageValues;

                    if (state.AsyncResult != null)
                        packageValues = (from x in state.AsyncResult where x is Package select x as Package).ToArray();
                    else
                        packageValues = new Package[0];

                    DataTable valuesTable = packagesToDataTable(parameterList, packageValues);
                    valuesTable.TableName = info.Caption;
                    dataSet.Tables.Add(valuesTable);
                }
            }
        }

        private List<ParameterNode> GetParameters(OperationState state, ParameterReportSourceSetting reportSetting)
        {
            ParameterNode parameterNode;
            List<ParameterNode> parameterList = new List<ParameterNode>();
            foreach (var parameterID in reportSetting.ParameterIds)
            {
                parameterNode = unitManager.ValidateUnitNode<ParameterNode>(state, parameterID, Privileges.Read);

                if (parameterNode != null)
                    parameterList.Add(parameterNode);
            }
            return parameterList;
        }

        public void GenerateEmptyData(OperationState state, DataSet dataSet, ReportSourceSettings settings)
        {
            const int testCount = 10;

            ParameterReportSourceSetting reportSetting = settings as ParameterReportSourceSetting;

            List<ParameterNode> parameterList = GetParameters(state, reportSetting);

            var interval = Interval.Hour;
            DateTime endTime = interval.NearestEarlierTime(DateTime.Now);
            DateTime startTime = interval.Multiply(testCount).GetPrevTime(endTime);
            DateTime time;

            Random rand = new Random();

            var packageValues = new List<Package>();
            foreach (var item in parameterList)
            {
                List<ParamValueItem> values = new List<ParamValueItem>();
                time = startTime;
                while (time < endTime)
                {
                    values.Add(new ParamValueItem(time, Quality.Good, rand.NextDouble()));
                    time = interval.GetTime(time, 1);
                }
                packageValues.Add(new Package(item.Idnum, values));
            }

            DataTable valuesTable = packagesToDataTable(parameterList, packageValues.ToArray());
            valuesTable.TableName = info.Caption;
            dataSet.Tables.Add(valuesTable);
        }

        #endregion

        /// <summary>
        /// Преобразует полученные из системы Package в DataSet
        /// </summary>
        /// <param name="parameterList">Параметров</param>
        /// <param name="packageValues">Пачки данных</param>
        /// <returns></returns>
        private DataTable packagesToDataTable(List<ParameterNode> parameterList, Package[] packageValues)
        {
            const String timeColumnName = "time";
            DataTable table = new DataTable();
            table.TableName = info.Caption;

            table.Columns.Add(timeColumnName, typeof(DateTime));

            Dictionary<DateTime, Dictionary<int, ParamValueItem>> tableDictionary = new Dictionary<DateTime, Dictionary<int, ParamValueItem>>();
            Dictionary<int, ParamValueItem> timeDictionary;

            foreach (var package in packageValues)
            {
                foreach (var item in package.Values)
                {
                    if (!tableDictionary.TryGetValue(item.Time, out timeDictionary))
                        tableDictionary[item.Time] = timeDictionary = new Dictionary<int, ParamValueItem>();

                    timeDictionary[package.Id] = item;
                }
            }

            var timeList = new List<DateTime>(tableDictionary.Keys);
            timeList.Sort();

            foreach (var item in parameterList)
            {
                String columnName = item.Code;
                if (String.IsNullOrEmpty(item.Code))
                    columnName = unitManager.GetFullName(item);

                DataColumn column = table.Columns.Add(item.Idnum.ToString(), typeof(double));
                column.Caption = columnName;
            }

            foreach (var time in timeList)
            {
                DataRow dataRow = table.Rows.Add();
                timeDictionary = tableDictionary[time];
                dataRow[timeColumnName] = time;
                foreach (var parameterID in timeDictionary.Keys)
                {
                    dataRow[parameterID.ToString()] = timeDictionary[parameterID].Value;
                }
            }

            return table;
        }
    }
}
