using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Extension;
using ZedGraph;

namespace COTES.ISTOK.ClientCore.UnitProviders
{
    /// <summary>
    /// Провайдер для рисования во вкладках данных из расширения
    /// </summary>
    public class ExtensionUnitProvider : CommonGraphUnitProvider//UnitProvider, IGraphUnitProvider
    {
        ExtensionDataInfo[] tabsInfo;

        public ExtensionUnitProvider(StructureProvider strucProvider, ExtensionUnitNode unitNode)
            : base(strucProvider, unitNode)
        {

        }

        //public override BaseUnitControl[] CreateControls(UnitTypeId[] types, bool multitab)
        //{
        //    if (!multitab && types.Contains(UnitNode.Typ))
        //    {
        //        if (tabsInfo == null)
        //        {
        //            tabsInfo = rds.GetExtensionTableInfo(unitNode as ExtensionUnitNode);
        //        }
        //        if (tabsInfo != null)
        //        {
        //            List<BaseUnitControl> controls = new List<BaseUnitControl>();
        //            foreach (ExtensionDataInfo tabInfo in tabsInfo)
        //            {
        //                var control = new GraphUnitControl(this, tabInfo);
        //                control.Text = tabInfo.Caption;
        //                controls.Add(control);
        //            }
        //            return controls.ToArray();
        //        }
        //    }
        //    return base.CreateControls(types, multitab);
        //}

        #region IGraphUnitProvider Members

        Dictionary<String, bool> inProcessDictionary;
        public override bool InProcess(ExtensionDataInfo tabInfo)
        {
            bool inProgress;

            if (inProcessDictionary == null
                || !inProcessDictionary.TryGetValue(tabInfo.Name, out inProgress))
                inProgress = false;

            return inProgress;
        }

        public override int ParameterCount(ExtensionDataInfo tabInfo)
        {
            return 0;
        }

        Dictionary<String, DataTable> dataTableDictionary;
        public override DataTable DataTable(ExtensionDataInfo tabInfo)
        {
            DataTable table;

            if (dataTableDictionary == null
                || !dataTableDictionary.TryGetValue(tabInfo.Name, out table))
                table = null;

            return table;
        }

        Dictionary<String, CurveList> curvesDictionary;
        public override CurveList Curves(ExtensionDataInfo tabInfo)
        {
            CurveList curves;

            if (curvesDictionary == null
                || !curvesDictionary.TryGetValue(tabInfo.Name, out curves))
                curves = null;

            return curves;
        }

        const String timeColumnName = "tictime";
        protected void OnValuesChanged(ExtensionDataInfo tabInfo)
        {
            ExtensionData megaTab;

            throw new NotImplementedException("OnValueChanged");

            if (megaTabDictionary != null && megaTabDictionary.TryGetValue(tabInfo.Name, out megaTab))
            {
                CurveList curves = new CurveList();

                for (int i = 0; i < megaTab.Info.Trends.Count; i++)
                {
                    ExtensionDataTrend line = megaTab.Info.Trends[i];
                    CurveItem curveItem = new LineItem(line.Caption);

                    DataView lineView = megaTab[line.Name];

                    foreach (DataRowView dataRow in lineView)
                    {
                        double x, y;

                        // считаем первую колонку х, а вторую y
                        ExtensionDataColumn xColumn = line.Columns[0];
                        ExtensionDataColumn yColumn = line.Columns[1];

                        if (xColumn.ValueType == typeof(DateTime))
                            //x = (double)new XDate(Convert.ToDateTime(dataRow[xColumn.Name]));
                            x = 0;
                        else x = Convert.ToDouble(dataRow[xColumn.Name]);

                        y = Convert.ToDouble(dataRow[yColumn.Name]);

                        //curveItem.AddPoint(x, y);
                    }

                    //curveItem.Tag = line.Name;
                    //curves.Add(curveItem);
                }

                if (dataTableDictionary == null)
                    dataTableDictionary = new Dictionary<string, DataTable>();
                dataTableDictionary[tabInfo.Name] = megaTab.Table;

                if (curvesDictionary == null)
                    curvesDictionary = new Dictionary<string, CurveList>();
                curvesDictionary[tabInfo.Name] = curves;
            }

            GraphUnitProviderEventArgs e = new GraphUnitProviderEventArgs(tabInfo);

            OnParameterListChanged(e);
            OnValuesChanged(e);
        }

        private bool ValidateMegaTab(ExtensionData megaTab)
        {
            throw new NotImplementedException();
        }

        public override void ClearValues(ExtensionDataInfo tabInfo)
        {
            //throw new NotImplementedException();
        }

        public override void QueryHistogramData(ExtensionDataInfo tabInfo, DateTime dateFrom, DateTime dateTo, bool userRemoteServer)
        {
            throw new NotImplementedException();
        }

        public override void QueryGraphData(ExtensionDataInfo tabInfo, DateTime dateFrom, DateTime dateTo, bool userRemoteServer)
        {
            throw new NotImplementedException("QueryGraphData");
            AsyncOperationWatcher<ExtensionData> watcher = null;
            RDS.ExtensionDataService.GetExtensionExtendedTable(UnitNode.Idnum, tabInfo.Name, dateFrom, dateTo);

            watcher.AddStartHandler(() => OnLockControl(new GraphUnitProviderEventArgs(tabInfo)));
            watcher.AddValueRecivedHandler(MegaTabReceived);
            watcher.AddFinishHandler(() =>
            {
                OnValuesChanged(tabInfo);
                OnUnlockControl(new GraphUnitProviderEventArgs(tabInfo));
            });
            //uniForm.RunWatcher(watcher);
        }

        Dictionary<String, ExtensionData> megaTabDictionary;
        private void MegaTabReceived(ExtensionData megaTab)
        {
            if (megaTabDictionary == null)
                megaTabDictionary = new Dictionary<string, ExtensionData>();

            megaTabDictionary[megaTab.Info.Name] = megaTab;
        }

        public override Color MaxColor(ExtensionDataInfo tabInfo)
        {
            return Color.Empty;
        }

        public override Color MinColor(ExtensionDataInfo tabInfo)
        {
            return Color.Empty;
        }

        public override bool GetScale(ExtensionDataInfo tableInfo, CurveItem citem, out double min, out double max)
        {
            min = max = 0;

            return false;
        }

        public override string GetFullName(ExtensionDataInfo tableInfo, CurveItem citem)
        {
            return citem.Label.Text;
        }

        #endregion

        //public ExtensionDataInfo[] GetExtensionTableInfo()
        //{
        //    return tabsInfo;
        //}
    }
}
