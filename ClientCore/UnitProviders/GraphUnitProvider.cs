using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore.Utils;
using COTES.ISTOK.Extension;
using ZedGraph;

namespace COTES.ISTOK.ClientCore.UnitProviders
{
    public class GraphUnitProvider : CommonGraphUnitProvider
    {
        ExtensionDataInfo megaInfo;

        protected List<int> lstHiddenParameters = new List<int>();

        public GraphUnitProvider(StructureProvider strucProvider, GraphNode graphNode)
            : base(strucProvider, graphNode)
        {
            ExtensionDataType tabType;
            HistogramNode histogramNode = graphNode as HistogramNode;
            if (histogramNode != null)
                tabType = ExtensionDataType.Histogram;
            else
                tabType = ExtensionDataType.Graph;

            megaInfo = new ExtensionDataInfo(graphNode.Idnum.ToString(), tabType, graphNode.Text);

            if (histogramNode != null)
                megaInfo.Horizontal = histogramNode.Horizontal;
        }

        //protected override BaseUnitControl GetUnitControl(bool multitab)
        //{
        //    return new GraphUnitControl(this, megaInfo);
        //}

        #region Мякота
        CurveList curves;
        /// <summary>
        /// График собственной персоной
        /// </summary>
        public override CurveList Curves(ExtensionDataInfo tabInfo)
        {
            if (curves == null)
            {
                LineItem litem;
                Color color;
                curves = new CurveList();
                lstLineColors.Clear();
                lstLineColors.Add(Color.White);
                lstLineColors.Add(Color.Black);

                if (UnitNode is HistogramNode)
                {
                    foreach (HistogramParamNode item in this.UnitNode.Parameters)
                    {
                        litem = new LineItem(item.Text);
                        color = item.LineColor;
                        if (color == Color.Empty) color = GetColor();
                        lstLineColors.Add(color);
                        litem.Color = color;
                        litem.Tag = item.ParameterId;
                        curves.Add(litem);
                    }
                }
                else
                {
                    foreach (GraphParamNode item in this.UnitNode.Parameters)
                    {
                        litem = new LineItem(item.Text);
                        color = item.LineColor;
                        if (color == Color.Empty) color = GetColor();
                        lstLineColors.Add(color);
                        litem.Symbol.Type = (SymbolType)item.LineSymbol;
                        litem.Color = color;
                        litem.Line.Style = (System.Drawing.Drawing2D.DashStyle)item.LineStyle;
                        litem.Line.Width = (float)item.LineWidth;
                        litem.Symbol.IsVisible = GetState(megaInfo).ShowMarker;
                        litem.Tag = item.ParameterId;
                        //dicCurveParams[litem] = item;
                        curves.Add(litem);
                    }
                }
                UpdateCurvesVisibility();
            }
            return curves;
        }

        protected void UpdateCurvesVisibility()
        {
            if (curves != null)
                foreach (var item in curves)
                    item.IsVisible = !lstHiddenParameters.Contains((int)item.Tag);
        }

        public bool GetParameterVisibility(int paramId)
        {
            return !lstHiddenParameters.Contains(paramId);
        }
        public void SetParameterVisibility(int paramId, bool value)
        {
            if (!value)
            {
                if (!lstHiddenParameters.Contains(paramId))
                    lstHiddenParameters.Add(paramId);
            }
            else
                lstHiddenParameters.Remove(paramId);
            UpdateCurvesVisibility();
        }

        /// <summary>
        /// Основное хранилище данных
        /// </summary>
        Dictionary<int, List<ParamValueItem>> values = new Dictionary<int, List<ParamValueItem>>();

        #region Генератор рандомных цветов
        List<Color> lstLineColors = new List<Color>();
        /// <summary>
        /// Генерирует цвет, наиболее отличающийся от всех имеющихся в списке использованных цветов
        /// </summary>
        /// <returns></returns>
        private Color GetColor()
        {
            Dictionary<double, Color> dicColor = new Dictionary<double, Color>();
            int mr = 255, mg = 255, mb = 255;
            double distance = 0;
            double dist;

            for (byte r = 255; r > 0; r /= 2)
            {
                for (byte g = 255; g > 0; g /= 2)
                {
                    for (byte b = 255; b > 0; b /= 2)
                    {
                        dicColor.Clear();
                        foreach (var item in lstLineColors)
                        {
                            dist = GetDistance(r, g, b, item.R, item.G, item.B);
                            dicColor[dist] = Color.FromArgb(r, g, b);
                        }
                        dist = double.MaxValue;
                        foreach (var item in dicColor.Keys)
                            if (item < dist) dist = item;

                        if (dist > distance)
                        {
                            distance = dist;
                            mr = dicColor[dist].R;
                            mg = dicColor[dist].G;
                            mb = dicColor[dist].B;
                        }
                    }
                }
            }

            return Color.FromArgb(mr, mg, mb);
        }
        /// <summary>
        /// Считает расстояние между двумя точками
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="z1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="z2"></param>
        /// <returns></returns>
        private double GetDistance(byte x1, byte y1, byte z1, byte x2, byte y2, byte z2)
        {
            return Math.Sqrt(Math.Pow((double)x2 - (double)x1, 2)
                + Math.Pow((double)y2 - (double)y1, 2)
                + Math.Pow((double)z2 - (double)z1, 2));
        }
        #endregion

        protected void OnValuesChanged()
        {
            Dictionary<DateTime, Dictionary<int, double>> tempValues = new Dictionary<DateTime, Dictionary<int, double>>();
            Dictionary<int, double> parameterDictionary;

            // формируем промежуточный Dictionary
            foreach (int packageID in values.Keys)
            {
                foreach (var item in values[packageID])
                {
                    if (!tempValues.TryGetValue(item.Time, out parameterDictionary))
                        tempValues[item.Time] = parameterDictionary = new Dictionary<int, double>();
                    parameterDictionary[packageID] = item.Value;
                }
            }

            // переделываем Dictionary в DataTable
            DataTable table = CreateTable();
            DataRow row;
            int idx = table.Columns.IndexOf("tictime");
            foreach (var time in tempValues.Keys)
            {
                row = null;

                row = table.NewRow();
                row[idx] = time;
                table.Rows.Add(row);

                if (row != null)
                {
                    string name;

                    foreach (var parameterID in tempValues[time].Keys)
                    {
                        name = parameterID.ToString();
                        if (table.Columns.Contains(name))
                            row[name] = tempValues[time][parameterID];
                    }
                }
            }
            dataTable = table;

            OnParameterListChanged(new GraphUnitProviderEventArgs(megaInfo));
            OnValuesChanged(new GraphUnitProviderEventArgs(megaInfo));
        }
        /// <summary>
        /// Создание таблицы со всеми значениями
        /// </summary>
        /// <returns></returns>
        private DataTable CreateTable()
        {
            DataTable table = new DataTable();
            DataColumn column;

            column = table.Columns.Add("tictime", typeof(DateTime));
            column.Caption = "Время";
            table.PrimaryKey = new DataColumn[] { column };

            foreach (var item in this.UnitNode.Parameters)
            {
                column = table.Columns.Add(item.ParameterId.ToString(), typeof(double));
                column.Caption = item.Text;
            }

            return table;
        }
        #endregion

        #region Настройки компонента

        public override Color MaxColor(ExtensionDataInfo tabInfo)
        {
            HistogramNode histNode = UnitNode as HistogramNode;
            if (histNode != null)
                return histNode.MaxColor;
            return Color.Empty;
        }
        public override Color MinColor(ExtensionDataInfo tabInfo)
        {
            HistogramNode histNode = UnitNode as HistogramNode;
            if (histNode != null)
                return histNode.MinColor;
            return Color.Empty;
        }

        public DataTable dataTable;
        public override DataTable DataTable(ExtensionDataInfo tabInfo)
        { return dataTable; }

        public bool inProcess;
        public override bool InProcess(ExtensionDataInfo tabInfo)
        { return inProcess; }
        public void InProcess(ExtensionDataInfo tabInfo, bool value)
        { inProcess = value; }

        public override int ParameterCount(ExtensionDataInfo tabInfo)
        {
            if (UnitNode.Parameters == null)
                return 0;
            return UnitNode.Parameters.Length;
        }

        public override void ClearValues(ExtensionDataInfo tabInfo)
        {
            if (values != null)
                values.Clear();

            curves = null;
        }

        #endregion

        #region Работа с данными

        /// <summary>
        /// Как бы запросить данные с сервера
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="userRemoteServer"></param>
        public override void QueryGraphData(ExtensionDataInfo tabInfo, DateTime dateFrom, DateTime dateTo, bool useRemoteServer)
        {
            QueryGraphData(tabInfo, dateFrom, dateTo, useRemoteServer, false);
        }
        public AsyncOperationWatcher QueryGraphData(ExtensionDataInfo tabInfo, DateTime dateFrom, DateTime dateTo, bool useRemoteServer, bool async)
        {
            if (inProcess) return null;

            try
            {
                inProcess = true;

                GraphUnitProviderState state = GetState(megaInfo);

                if (state != null)
                {
                    state.GraphFrom = dateFrom;
                    state.GraphTo = dateTo;
                }

                List<int> lstIds = new List<int>();

                foreach (var item in this.UnitNode.Parameters)
                    lstIds.Add(item.ParameterId);

                ClearValues(tabInfo);
                return BeginGetValues(lstIds.ToArray(), dateFrom, dateTo, useRemoteServer, async);
            }
            catch
            {
                //в лог
                inProcess = false;
            }
            return null;
        }
        public override void QueryHistogramData(ExtensionDataInfo tabInfo, DateTime dateFrom, DateTime dateTo, bool useRemoteServer)
        {
            QueryHistogramData(tabInfo, dateFrom, dateTo, useRemoteServer, false);
        }
        public AsyncOperationWatcher QueryHistogramData(ExtensionDataInfo tabInfo, DateTime dateFrom, DateTime dateTo, bool useRemoteServer, bool async)
        {
            UnitNode node;
            Interval interval;

            if (inProcess) return null;

            try
            {
                inProcess = true;
                foreach (HistogramParamNode item in this.UnitNode.Parameters)
                {
                    interval = Interval.Zero;
                    //TODO: нужна асинхронная обработка
                    node = RDS.NodeDataService.GetUnitNode(item.ParameterId);
                    if (node != null && node is ParameterNode) RDS.NodeDataService.GetParameterInterval(node.Idnum);
                    return BeginGetValues(item.ParameterId, dateFrom, dateTo, interval, item.Aggregation, useRemoteServer, async);
                }
            }
            catch
            {
                //в лог
                inProcess = false;
            }
            return null;
        }

        /// <summary>
        /// Получение набора значений, запись их в ЮнитПровайдер и грид значений
        /// </summary>
        /// <param name="arrParams"></param>
        private void UpdateData(Package package)//ParamValueItemWithID[] arrParams)
        {
            Dictionary<int, List<ParamValueItem>> dicParamIds = new Dictionary<int, List<ParamValueItem>>();
            List<ParamValueItem> localValueList, valueList;

            if (package != null)
            {
                GraphUnitProviderState state = GetState(megaInfo);

                if (!values.TryGetValue(package.Id, out valueList))
                    values[package.Id] = valueList = new List<ParamValueItem>();

                if (!dicParamIds.TryGetValue(package.Id, out localValueList))
                    dicParamIds[package.Id] = localValueList = new List<ParamValueItem>();

                foreach (var item in package.Values)
                {
                    if (item.Time >= state.GraphFrom && item.Time <= state.GraphTo)
                    {
                        valueList.Add(item);
                        localValueList.Add(item);
                    }
                }
            }

            // добавляем точки на график
            lock (this)
            {
                if (package != null)
                {
                    if (!values.TryGetValue(package.Id, out valueList))
                        values[package.Id] = valueList = new List<ParamValueItem>();
                }

                CurveItem citem = null;
                CurveList curves = Curves(megaInfo);

                foreach (var item in dicParamIds.Keys)
                {
                    citem = null;
                    if (curves != null)
                    {
                        citem = curves.Find(c => c.Tag != null && c.Tag.Equals(item));

                        if (citem != null)
                        {
                            foreach (var param in dicParamIds[item])
                                citem.AddPoint((double)(new XDate(param.Time)), param.Value);
                        }
                    }
                }
            }
        }
        #endregion

        public override bool GetScale(ExtensionDataInfo tableInfo, CurveItem citem, out double min, out double max)
        {
            if (citem.Tag is int)
            {
                int parameterID = (int)citem.Tag;
                GraphParamNode paramNode = this.UnitNode.Parameters.First(p => p.ParameterId == parameterID) as GraphParamNode;

                if (paramNode != null
                    && paramNode.MinValue != null
                    && paramNode.MaxValue != null)
                {
                    min = (double)paramNode.MinValue;
                    max = (double)paramNode.MaxValue;
                    if (double.IsNaN(min)) min = 0;
                    if (double.IsNaN(max)) max = 1.0;
                    return true;
                }
            }

            min = 0;
            max = 1.0;

            return false;
        }

        public override string GetFullName(ExtensionDataInfo tableInfo, CurveItem citem)
        {
            if (citem.Tag is int)
            {
                int parameterID = (int)citem.Tag;
                ChildParamNode paramNode = this.UnitNode.Parameters.First(p => p.ParameterId == parameterID);

                if (paramNode != null)
                {
                    return paramNode.ParameterFullName;
                }
            }

            return citem.Label.Text;
        }

        #region WebClient
        public enum HistogramSorting
        {
            None,
            Inc,
            Dec
        }
        public void SetDefaultXScale(GraphPane pane, DateTime minDate, DateTime maxDate)
        {
            double interval = ((TimeSpan)(maxDate - minDate)).TotalSeconds;
            if (interval == 0) interval = 0.1f; //чисто штоб не нулик
            if (interval <= 60f) // 
            {
                pane.XAxis.Scale.Format = "H:mm:ss fff";
                pane.XAxis.Scale.MinorStep = 1 / (60f / interval);
                pane.XAxis.Scale.MinorUnit = DateUnit.Second;
                pane.XAxis.Scale.MajorStep = 5 * pane.XAxis.Scale.MinorStep;
                pane.XAxis.Scale.MajorUnit = DateUnit.Second;
            }
            else if (interval <= 3600f) // час
            {
                pane.XAxis.Scale.Format = "H:mm:ss";
                pane.XAxis.Scale.MinorStep = 1 / (3600f / interval);
                pane.XAxis.Scale.MinorUnit = DateUnit.Minute;
                pane.XAxis.Scale.MajorStep = 5 * pane.XAxis.Scale.MinorStep;
                pane.XAxis.Scale.MajorUnit = DateUnit.Minute;
            }
            else if (interval <= 86400f) // 24 часа
            {
                pane.XAxis.Scale.Format = "dd.MM H:mm:ss";
                pane.XAxis.Scale.MinorStep = 1 / (86400f / interval);
                pane.XAxis.Scale.MinorUnit = DateUnit.Hour;
                pane.XAxis.Scale.MajorStep = 5 * pane.XAxis.Scale.MinorStep;
                pane.XAxis.Scale.MajorUnit = DateUnit.Hour;

            }
            else if (interval <= 2592000f) // 1 месяц
            {
                pane.XAxis.Scale.Format = "dd.MM H:mm:ss";
                pane.XAxis.Scale.MinorStep = 1 / (2592000f / interval);
                pane.XAxis.Scale.MinorUnit = DateUnit.Day;
                pane.XAxis.Scale.MajorStep = 7 * pane.XAxis.Scale.MinorStep;
                pane.XAxis.Scale.MajorUnit = DateUnit.Day;
            }
            else
            {
                pane.XAxis.Scale.Format = "dd.MM.yy";
                pane.XAxis.Scale.MinorStep = 1;
                pane.XAxis.Scale.MinorUnit = DateUnit.Month;
                pane.XAxis.Scale.MajorStep = 6 * pane.XAxis.Scale.MinorStep;
                pane.XAxis.Scale.MajorUnit = DateUnit.Month;
            }
        }
        void InitGraphProperties(GraphPane pane)
        {
            pane.GraphObjList.Clear();
            pane.XAxis.Title.Text = "   ";

            if (megaInfo.Type == ExtensionDataType.Histogram)
            {
                if (megaInfo.Horizontal)
                {
                    pane.YAxis.Type = AxisType.Text;
                }
                else
                {
                    pane.XAxis.Type = AxisType.Text;
                }
            }
            else
            {
                // Ось X - это дата
                pane.XAxis.Type = AxisType.Date;

                pane.XAxis.Scale.FontSpec.IsBold = true;
                pane.Legend.Position = LegendPos.TopFlushLeft;
                pane.Legend.FontSpec.Size = 12;
            }

            // tilt the x axis labels to an angle of 65 degrees
            pane.XAxis.Scale.FontSpec.Angle = 65;
            pane.XAxis.Scale.FontSpec.Size = 10;
        }
        private Dictionary<TKey, TValue> CreateDictionary<TKey, TValue>(TKey key, TValue value)
        {
            return new Dictionary<TKey, TValue>() { { key, value } };
        }
        public void SetDefaultScale(GraphPane pane)
        {
            foreach (var item in pane.YAxisList)
            {
                item.Scale.MaxAuto = true;
                item.Scale.MinAuto = true;
                item.Scale.MajorStepAuto = true;
                item.Scale.MinorStepAuto = true;
                item.CrossAuto = true;
                item.Scale.MagAuto = true;
                item.Scale.FormatAuto = true;
            }
            foreach (var item in pane.Y2AxisList)
            {
                item.Scale.MaxAuto = true;
                item.Scale.MinAuto = true;
                item.Scale.MajorStepAuto = true;
                item.Scale.MinorStepAuto = true;
                item.CrossAuto = true;
                item.Scale.MagAuto = true;
                item.Scale.FormatAuto = true;
            }
            pane.XAxis.Scale.MaxAuto = true;
            pane.XAxis.Scale.MinAuto = true;
            pane.XAxis.Scale.MajorStepAuto = true;
            pane.XAxis.Scale.MinorStepAuto = true;
            pane.XAxis.CrossAuto = true;
            pane.XAxis.Scale.MagAuto = true;
            pane.XAxis.Scale.FormatAuto = true;
            pane.AxisChange();
        }
        public GraphData GetData(int currentParameter, int maxVisibleParameters)
        {
            GraphData gd = new GraphData();
            LineInfo linfo;
            LineData ldata;

            CurveList curves = Curves(megaInfo);
            if (curves != null && curves.Count > 0)
            {
                for (int i = currentParameter; i < currentParameter + maxVisibleParameters
                    && i < curves.Count; i++)
                {
                    linfo = new LineInfo()
                    {
                        Caption = curves[i].Label.Text,
                        Color = curves[i].Color,
                        IsVisible = curves[i].IsVisible,
                        ParameterId = (int)curves[i].Tag
                    };

                    ldata = new LineData();
                    for (int j = 0; j < curves[i].Points.Count; j++)
                    {
                        ldata.Add(new XDate(curves[i].Points[j].X), curves[i].Points[j].Y);
                    }
                    gd.AddLine(linfo, ldata);
                }
            }

            return gd;
        }
        public void FillPane(GraphPane pane, int currentParameter, int maxVisibleParameters, HistogramSorting sort)
        {
            GraphUnitProviderState state = GetState(megaInfo);
            Axis axis;

            bool isY2axis;
            bool isHistogram = megaInfo.Type == ExtensionDataType.Histogram;

            int yAxis;

            InitGraphProperties(pane);

            pane.Title.Text = megaInfo.Caption;// this.UnitNode.Text;
            pane.CurveList.Clear();

            CurveList curves = Curves(megaInfo);
            if (curves != null && curves.Count > 0)
            {
                CurveItem citem;
                BarItem bitem = null;

                List<Color> lstColors = new List<Color>();
                List<string> lstLabels = new List<string>();
                if (isHistogram) bitem = new BarItem("");
                var dic = CreateDictionary(1d, new { X = 0d, Y = 0d, Z = 0d, Label = "" });
                dic.Clear();

                isY2axis = false;
                for (int i = currentParameter; i < currentParameter + maxVisibleParameters
                    && i < curves.Count; i++)
                {
                    citem = curves[i];

                    if (isHistogram)
                    {
                        lstColors.Add(citem.Color);
                        lstLabels.Add(citem.Label.Text);

                        if (citem.NPts > 0)
                        {
                            PointPair point = citem.Points[0].Clone();
                            point.Z = i - currentParameter;
                            dic.Add(point.Y, new { X = point.X, Y = point.Y, Z = point.Z, Label = citem.Label.Text });
                        }
                    }
                    else
                    {
                        if (isY2axis)
                        {
                            yAxis = pane.AddY2Axis(citem.Label.Text);
                            axis = pane.Y2AxisList[yAxis];
                        }
                        else
                        {
                            yAxis = pane.AddYAxis(citem.Label.Text);
                            axis = pane.YAxisList[yAxis];
                        }

                        axis.Title.IsVisible = false;
                        axis.IsVisible = citem.IsVisible;
                        axis.Color = citem.Color;
                        citem.IsY2Axis = isY2axis;
                        citem.YAxisIndex = yAxis;
                        pane.CurveList.Add(citem);
                        isY2axis = !isY2axis;

                        double min, max;

                        if (GetScale(megaInfo, citem, out min, out max))
                        {
                            axis.Scale.Min = min;
                            axis.Scale.Max = max;
                        }
                    }
                }

                if (isHistogram && bitem != null)
                {
                    int order = 0;
                    List<double> lstVals = new List<double>();
                    List<string> lstLabs = new List<string>();
                    PointPair point;
                    foreach (var item in dic.Keys) lstVals.Add(item);

                    if (sort == HistogramSorting.Inc)
                        if (megaInfo.Horizontal) order = -1; else order = 1;
                    else
                        if (sort == HistogramSorting.Dec)
                            if (megaInfo.Horizontal) order = 1; else order = -1;
                    lstVals.Sort((x, y) => order * x.CompareTo(y));

                    int maxColor, minColor;
                    maxColor = lstColors.Count;
                    lstColors.Add(MaxColor(megaInfo));
                    minColor = lstColors.Count;
                    lstColors.Add(MinColor(megaInfo));

                    double max = double.MinValue, min = double.MaxValue;
                    if (lstVals.Count > 0)
                    {
                        if (order == 1)
                        {
                            min = lstVals[0];
                            max = lstVals[lstVals.Count - 1];
                        }
                        else
                            if (order == -1)
                            {
                                max = lstVals[0];
                                min = lstVals[lstVals.Count - 1];
                            }
                            else
                            {
                                foreach (var item in lstVals)
                                {
                                    if (item > max) max = item;
                                    if (item < min) min = item;
                                }
                            }
                    }
                    foreach (var item in lstVals)
                    {
                        point = new PointPair();
                        if (item == min && MinColor(megaInfo) != Color.Empty && min != max)
                            point.Z = minColor;
                        else
                            if (item == max && MaxColor(megaInfo) != Color.Empty && min != max)
                                point.Z = maxColor;
                            else
                                point.Z = dic[item].Z;
                        if (megaInfo.Horizontal)
                        {
                            point.X = dic[item].Y;
                            point.Y = dic[item].Z;
                        }
                        else
                        {
                            point.X = dic[item].X;
                            point.Y = dic[item].Y;
                        }
                        lstLabs.Add(dic[item].Label);
                        bitem.AddPoint(point);
                    }

                    if (lstColors.Count > 1)
                    {
                        bitem.Bar.Fill = new Fill(lstColors.ToArray());
                        bitem.Bar.Fill.RangeMin = 0;
                        bitem.Bar.Fill.RangeMax = lstColors.Count - 1;
                        bitem.Bar.Fill.Type = FillType.GradientByZ;
                    }
                    else
                    {
                        bitem.Bar.Fill = new Fill(lstColors[0]);
                        bitem.Bar.Fill.Type = FillType.Solid;
                    }
                    bitem.Bar.Fill.SecondaryValueGradientColor = Color.Empty;
                    pane.CurveList.Add(bitem);
                    if (megaInfo.Horizontal)
                        pane.YAxis.Scale.TextLabels = lstLabs.ToArray();
                    else
                        pane.XAxis.Scale.TextLabels = lstLabs.ToArray();
                }
            }
            //if (isHistogram) ViewValue(0);
            if (pane.Y2AxisList.Count == 0) pane.Y2AxisList.Add("");
            if (pane.YAxisList.Count == 0) pane.YAxisList.Add("");

            if (!isHistogram)
            {
                pane.XAxis.Scale.Min = (double)(new ZedGraph.XDate(state.GraphFrom));
                pane.XAxis.Scale.Max = (double)(new ZedGraph.XDate(state.GraphTo));

                SetDefaultXScale(pane, state.GraphFrom, state.GraphTo);
            }

            //pane.AxisChange();
            SetDefaultScale(pane);
        }
        #endregion

        #region DataRequest
        protected AsyncOperationWatcher BeginGetValues(int[] ids, DateTime beginTime,
            DateTime endTime, bool useBlockValues, bool async)
        {
            inProcess = true;
            if (async)
            {
                var op = RDS.ValuesDataService.BeginGetValues(ids, beginTime, endTime, useBlockValues);
                AsyncOperationWatcher<UAsyncResult> watcher = new AsyncOperationWatcher<UAsyncResult>(op, RDS);
                watcher.AddFinishHandler(() =>
                {
                    OnValuesChanged();
                    OnUnlockControl(new GraphUnitProviderEventArgs(megaInfo));
                    inProcess = false;
                });
                watcher.AddValueRecivedHandler((x) =>
                {
                    if (x != null && x.Packages != null)
                        foreach (var item in x.Packages)
                            UpdateData(item);
                });
                watcher.AddStartHandler(() =>
                {
                    OnLockControl(new GraphUnitProviderEventArgs(megaInfo));
                });
                return watcher;
            }
            else
            {
                IEnumerable<Package> res = RDS.ValuesDataService.GetValues(ids, beginTime, endTime, useBlockValues);
                foreach (var item in res)
                    UpdateData(item);
                //DisplayValue();
                OnValuesChanged();
                inProcess = false;
                return null;
            }
        }
        protected AsyncOperationWatcher BeginGetValues(int parameterId, DateTime dateFrom, DateTime dateTo,
            Interval interval, CalcAggregation aggregation, bool useBlockValues, bool async)
        {
            inProcess = true;
            if (async)
            {
                var op = RDS.ValuesDataService.BeginGetValues(
                    parameterId, dateFrom, dateTo, interval, aggregation, useBlockValues);
                AsyncOperationWatcher<UAsyncResult> watcher = new AsyncOperationWatcher<UAsyncResult>(op, RDS);
                watcher.AddFinishHandler(() =>
                {
                    OnValuesChanged();
                    inProcess = false;
                });
                watcher.AddValueRecivedHandler((x) =>
                {
                    if (x != null && x.Packages != null)
                        foreach (var item in x.Packages)
                            UpdateData(item);
                });
                return watcher;
            }
            else
            {

                IEnumerable<Package> res = RDS.ValuesDataService.GetValues(parameterId, dateFrom, dateTo, interval, aggregation, useBlockValues);
                foreach (var item in res)
                    UpdateData(item);
                OnValuesChanged();
                inProcess = false;
                return null;
            }
        }
        #endregion

        public bool DataReady { get; set; }
    }
}
