using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Extension;
using ZedGraph;
using COTES.ISTOK.ClientCore.UnitProviders;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    internal partial class GraphUnitControl : BaseUnitControl
    {
        int maxVisibleParameters = 5;
        int currentParameter = 0;

        bool isHistogram { get { return tabInfo != null && tabInfo.Type == ExtensionDataType.Histogram; } }

        List<Color> lstLineColors = new List<Color>();

        private double step = 0f;
        /// <summary>
        /// Флаг, указывающий на запрет лишнего обновления графика
        /// </summary>
        bool updating = false;

        //bool isY2axis = false; int yAxis = 0;

        ExtensionDataInfo tabInfo;

        public GraphUnitControl(CommonGraphUnitProvider unitProvider/*, ExtensionDataInfo tabInfo*/)
            : base(unitProvider)
        {
            this.tabInfo = new ExtensionDataInfo("name", new ExtensionDataType()); //tabInfo;
            Init();
        }

        public void Init()
        {
            InitializeComponent();
            graph.ContextMenuBuilder += new ZedGraphControl.ContextMenuBuilderEventHandler(ModifyGraphContextMenu);
            FillTimePeriod();
        }

        protected override void DisposeControl()
        {
            try
            {
                CommonGraphUnitProvider graphUnitProvider = UnitProvider as CommonGraphUnitProvider;
                if (graphUnitProvider != null)
                {
                    graphUnitProvider.LockControl -= new EventHandler<GraphUnitProviderEventArgs>(graphUnitProvider_Lock);
                    graphUnitProvider.UnlockControl -= new EventHandler<GraphUnitProviderEventArgs>(graphUnitProvider_Unlock);
                    graphUnitProvider.ValuesChanged -= new EventHandler<GraphUnitProviderEventArgs>(graphUnitProvider_ValuesChanged);
                    graphUnitProvider.ParameterListChanged -= new EventHandler<GraphUnitProviderEventArgs>(graphUnitProvider_ParameterListChanged);
                }
            }
            catch { }
            base.DisposeControl();
        }

        public override void InitiateProcess()
        {
            CommonGraphUnitProvider graphUnitProvider = UnitProvider as CommonGraphUnitProvider;
            GraphUnitProviderState state = graphUnitProvider.GetState(tabInfo);

            if (initiated) return;

            udPageParams.Value = maxVisibleParameters;
            if (state.SplitterDistance != 0)
                splitContainer1.SplitterDistance = (int)(state.SplitterDistance * splitContainer1.Height);
            if (graphUnitProvider != null)
            {
                graphUnitProvider.LockControl += new EventHandler<GraphUnitProviderEventArgs>(graphUnitProvider_Lock);
                graphUnitProvider.UnlockControl += new EventHandler<GraphUnitProviderEventArgs>(graphUnitProvider_Unlock);
                graphUnitProvider.ValuesChanged += new EventHandler<GraphUnitProviderEventArgs>(graphUnitProvider_ValuesChanged);
                graphUnitProvider.ParameterListChanged += new EventHandler<GraphUnitProviderEventArgs>(graphUnitProvider_ParameterListChanged);

                if (datFrom.MinDate <= state.GraphFrom && state.GraphFrom <= datFrom.MaxDate)
                    datFrom.Value = state.GraphFrom;
                if (datTo.MinDate <= state.GraphFrom && state.GraphFrom <= datTo.MaxDate)
                    datTo.Value = state.GraphTo;
                cbxPeriod.SelectedValue = state.GraphPeriod;
                if (state.SortIndex < cbxSorting.Items.Count)
                    cbxSorting.SelectedIndex = state.SortIndex;
                chbxRemoteServer.Checked = state.RemoteServer;
                chbxRequestData.Checked = state.RequestData;

                CreateVisirGrid();

                UpdateGraph();
                UpdateDGV();
                if (graphUnitProvider.InProcess(tabInfo))
                    LockControls();
            }
            base.InitiateProcess();
        }

        void graphUnitProvider_ValuesChanged(object sender, GraphUnitProviderEventArgs e)
        {
            //if (tabInfo == null || tabInfo.Equals(e.TableInfo))
            {
                UpdateGraph();
                UpdateDGV();
            }
        }

        void graphUnitProvider_ParameterListChanged(object sender, GraphUnitProviderEventArgs e)
        {
            if (tabInfo == null || tabInfo.Equals(e.TableInfo))
            {
                CreateVisirGrid();
                UpdatePageInfo();
            }
        }

        void graphUnitProvider_Lock(object sender, GraphUnitProviderEventArgs e)
        {
            if (tabInfo == null || tabInfo.Equals(e.TableInfo))
            {
                LockControls();
            }
        }

        void graphUnitProvider_Unlock(object sender, GraphUnitProviderEventArgs e)
        {
            if (tabInfo == null || tabInfo.Equals(e.TableInfo))
            {
                UnlockControls();
            }
        }

        #region Дописывание элементов в контекстное меню графика
        private void ModifyGraphContextMenu(ZedGraphControl graph, ContextMenuStrip menuStrip,
            Point point,ZedGraph.ZedGraphControl.ContextMenuObjectState state)
        {
            CommonGraphUnitProvider graphUnitProvider = unitProvider as CommonGraphUnitProvider;
            ToolStripMenuItem it;

            if (!isHistogram)
            {
                it = new ToolStripMenuItem("miShowMarkers");
                it.Text = "Отображать маркеры";
                it.Checked = graphUnitProvider.GetState(tabInfo).ShowMarker;
                it.Visible = true;
                it.Click += new EventHandler(miShowMarkers_Click);
                menuStrip.Items.Add(it);
            }

            it = new ToolStripMenuItem("miShowTables");
            it.Text = "Отображать таблицы значений";
            it.Checked = !splitContainer1.Panel2Collapsed;
            it.Visible = true;
            it.Click += new EventHandler(miShowTables_Click);
            menuStrip.Items.Add(it);
        }
        void miShowMarkers_Click(object sender, EventArgs e)
        {
            CommonGraphUnitProvider graphUnitProvider = unitProvider as CommonGraphUnitProvider;
            GraphUnitProviderState state = graphUnitProvider.GetState(tabInfo);
            state.ShowMarker = !state.ShowMarker;
            ZedGraph.CurveList curves = graphUnitProvider.Curves(tabInfo);
            if (curves != null)
            {
                foreach (var item in curves)
                    if (item.IsLine)
                        ((ZedGraph.LineItem)item).Symbol.IsVisible = state.ShowMarker;
                graph.Refresh();
            }
        }
        void miShowTables_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
        }
        #endregion

        protected override void SaveUnitData()
        {
            CommonGraphUnitProvider graphUnitProvider = unitProvider as CommonGraphUnitProvider;
            GraphUnitProviderState state = graphUnitProvider.GetState(tabInfo);

            if (graphUnitProvider != null)
            {
                state.GraphFrom = datFrom.Value;
                state.GraphTo = datTo.Value;
                state.GraphPeriod = (GraphTimePeriod)cbxPeriod.SelectedValue;
                state.SortIndex = cbxSorting.SelectedIndex;
                state.RemoteServer = chbxRemoteServer.Checked;
                state.RequestData = chbxRequestData.Checked;
            }
            base.SaveUnitData();
        }

        protected override void LockControls()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => LockControls()));
                return;
            }
            base.LockControls();

            gbData.Enabled = false;
            gbInterval.Enabled = false;
            gbPages.Enabled = false;
            gbSorting.Enabled = false;
            graph.Enabled = false;
        }
        protected override void UnlockControls()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => UnlockControls()));
                return;
            }
            base.UnlockControls();

            gbData.Enabled = true;
            gbInterval.Enabled = true;
            gbPages.Enabled = true;
            gbSorting.Enabled = isHistogram;
            graph.Enabled = true;
        }

        /// <summary>
        /// Очистка списков осей Y и Y2
        /// </summary>
        void ClearGraphLines()
        {
            if (graph.GraphPane != null)
            {
                graph.GraphPane.YAxisList.Clear();
                graph.GraphPane.Y2AxisList.Clear();

                graph.GraphPane.AddYAxis("");
                graph.GraphPane.AddY2Axis("");
                graph.GraphPane.Y2Axis.IsVisible = false;
            }
            graph.AxisChange();
        }
                
        #region Visir rendering
        void VisirDraw(double x)
        {
            graph.GraphPane.GraphObjList.Clear();

            if (isHistogram) return;
            if (x < graph.GraphPane.XAxis.Scale.Min
                || x > graph.GraphPane.XAxis.Scale.Max
                || graph.GraphPane.YAxis == null) return;

            LineObj line = new LineObj(Color.Black,
                                       x,
                                       graph.GraphPane.YAxis.Scale.Min,
                                       x,
                                       graph.GraphPane.YAxis.Scale.Max);

            graph.GraphPane.GraphObjList.Add(line);
            graph.Invalidate();
            ViewValue(x);
        }
        void ViewValue(double x)
        {
            CommonGraphUnitProvider graphUnitProvider = unitProvider as CommonGraphUnitProvider;
            //valuePanel.SuspendLayout();
            try
            {
                DataGridViewRow row;
                string formatStr;
                string val;
                ZedGraph.XDate dat;

                if (!isHistogram)
                {
                    dat = new ZedGraph.XDate(x);
                    dgvValues.Columns["value"].HeaderText = "Значения на: " + dat.DateTime.ToString("G");
                }

                ZedGraph.CurveList curves = graphUnitProvider.Curves(tabInfo);
                if (curves != null)
                {
                    foreach (var item in curves)
                    {
                        formatStr = "";
                        val = "";
                        //if (cpItem.DecNumber != null) formatStr = String.Format("N{0}", cpItem.DecNumber);

                        row = null;
                        foreach (DataGridViewRow r in dgvValues.Rows)
                        {
                            if (r.Tag.Equals(item.Tag))
                            {
                                row = r;
                                break;
                            }
                        }
                        if (row != null)
                        {
                            if (isHistogram && item.NPts > 0) val = item.Points[0].Y.ToString(formatStr);
                            for (int i = 1; i < item.Points.Count; i++)
                            {
                                double x1 = item.Points[i - 1].X;
                                double x2 = item.Points[i].X;

                                if (x1 < x && x < x2)
                                {
                                    double y1 = item.Points[i - 1].Y;
                                    double y2 = item.Points[i].Y;
                                    double y = y1 + (y2 - y1) * (x - x1) / (x2 - x1);
                                    val = y.ToString(formatStr);
                                    break;
                                }
                            }

                            row.Cells["value"].Value = val;
                        }
                    }
                }
            }
            finally
            {
                //valuePanel.ResumeLayout();
            }
        }
        double GetMinorUnitValue(DateUnit unit)
        {
            double result;
            switch (unit)
            {
                case DateUnit.Millisecond:
                    result = 1f / 86400000f;
                    break;
                case DateUnit.Second:
                    result = 1f / 86400f;
                    break;
                case DateUnit.Minute:
                    result = 1f / 1440f;
                    break;
                case DateUnit.Hour:
                    result = 1f / 24f;
                    break;
                case DateUnit.Month:
                    result = 30.6001;
                    break;
                case DateUnit.Year:
                    result = 365.25;
                    break;
                default:
                    result = 1f;
                    break;
            }
            return result;
        }
        #endregion

        /// <summary>
        /// Создает грид визира
        /// </summary>
        private void CreateVisirGrid()
        {
            if (InvokeRequired) Invoke((Action)CreateVisirGrid);
            else
            {
                CommonGraphUnitProvider graphUnitProvider = unitProvider as CommonGraphUnitProvider;
                DataGridViewRow row;
                dgvValues.Rows.Clear();

                try
                {
                    updating = true;
                    ZedGraph.CurveList curves = graphUnitProvider.Curves(tabInfo);
                    if (curves != null)
                        foreach (var item in curves)
                        {
                            row = dgvValues.Rows[dgvValues.Rows.Add()];
                            row.Tag = item.Tag;
                            row.Cells["used"].Value = item.IsVisible;
                            row.Cells["param"].Value = graphUnitProvider.GetFullName(null, item);

                            foreach (DataGridViewCell cell in row.Cells)
                                cell.Style.ForeColor = item.Color;
                        }
                }
                finally
                {
                    updating = false;
                } 
            }
        }

        /// <summary>
        /// Обновление грида со всеми значениями
        /// </summary>
        public void UpdateDGV()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => UpdateDGV()));
                return;
            }
            CommonGraphUnitProvider graphUnitProvider = unitProvider as CommonGraphUnitProvider;
            DataTable table = graphUnitProvider.DataTable(tabInfo);

            if (dataGridView1.DataSource != table)
            {
                dataGridView1.SuspendLayout();
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                if (table != null)
                {
                    dataGridView1.DataSource = table;
                    foreach (DataGridViewColumn item in dataGridView1.Columns)
                    {
                        item.HeaderText = table.Columns[item.Name].Caption;
                        if (item.Name == "tictime")
                        {
                            item.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss.fff";
                            item.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        }
                        else
                        {
                            item.SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                    }
                }
                if (dataGridView1.Columns.Contains("tictime"))
                    dataGridView1.Sort(dataGridView1.Columns["tictime"], ListSortDirection.Ascending);
                dataGridView1.ResumeLayout();
            }
        }
        /// <summary>
        /// Рисовалка графика/гистограммы на основаниях выбранной страницы и сортировки
        /// </summary>
        public void UpdateGraph()
        {
            CommonGraphUnitProvider graphUnitProvider = unitProvider as CommonGraphUnitProvider;
            GraphUnitProviderState state = graphUnitProvider.GetState(tabInfo);
            
            if (graph.InvokeRequired)
            {
                graph.Invoke((Action)UpdateGraph);
                return;
            }

            if (updating) return;
            //ClearGraphLines();
            //InitGraphProperties();

            graph.GraphPane.YAxisList.Clear();
            graph.GraphPane.Y2AxisList.Clear();

            //graph.GraphPane.Title.Text = tabInfo.Caption;// this.UnitNode.Text;
            //graph.GraphPane.CurveList.Clear();

            //ZedGraph.CurveList curves=graphUnitProvider.Curves(tabInfo);
            //if (curves != null && curves.Count > 0)
            //{
            //    ZedGraph.CurveItem citem;
            //    BarItem bitem = null;

            //    List<Color> lstColors = new List<Color>();
            //    List<string> lstLabels = new List<string>();
            //    if (isHistogram) bitem = new BarItem("");
            //    var dic = CreateDictionary(1d, new { X = 0d, Y = 0d, Z = 0d, Label = "" });
            //    dic.Clear();

            //    isY2axis = false;
            //    for (int i = currentParameter; i < currentParameter + maxVisibleParameters
            //        && i < curves.Count; i++)
            //    {
            //        citem = curves[i];

            //        if (isHistogram)
            //        {
            //            lstColors.Add(citem.Color);
            //            lstLabels.Add(citem.Label.Text);
                        
            //            if (citem.NPts > 0)
            //            {
            //                PointPair point = citem.Points[0].Clone();
            //                point.Z = i - currentParameter;
            //                dic.Add(point.Y, new { X = point.X, Y = point.Y, Z = point.Z, Label = citem.Label.Text });
            //            }
            //        }
            //        else
            //        {
            //            if (isY2axis)
            //            {
            //                yAxis = graph.GraphPane.AddY2Axis(citem.Label.Text);
            //                axis = graph.GraphPane.Y2AxisList[yAxis];
            //            }
            //            else
            //            {
            //                yAxis = graph.GraphPane.AddYAxis(citem.Label.Text);
            //                axis = graph.GraphPane.YAxisList[yAxis];
            //            }

            //            axis.Title.IsVisible = false;
            //            axis.IsVisible = citem.IsVisible;
            //            axis.Color = citem.Color;
            //            citem.IsY2Axis = isY2axis;
            //            citem.YAxisIndex = yAxis;
            //            graph.GraphPane.CurveList.Add(citem);
            //            isY2axis = !isY2axis;

            //            CommonGraphUnitProvider graphProvider = unitProvider as CommonGraphUnitProvider;

            //            double min, max;

            //            if (graphProvider.GetScale(tabInfo, citem, out min, out max))
            //            {
            //                //(unitProvider as GraphUnitProvider).
            //                axis.Scale.Min = min;
            //                axis.Scale.Max = max;
            //            }
            //        }
            //    }

            //    if (isHistogram && bitem != null)
            //    {
            //        int order = 0;
            //        List<double> lstVals = new List<double>();
            //        List<string> lstLabs = new List<string>();
            //        PointPair point;
            //        foreach (var item in dic.Keys) lstVals.Add(item);

            //        if (cbxSorting.SelectedIndex == 1)
            //            if (tabInfo.Horizontal) order = -1; else order = 1;
            //        else
            //            if (cbxSorting.SelectedIndex == 2)
            //                if (tabInfo.Horizontal) order = 1; else order = -1;
            //        lstVals.Sort((x, y) => order * x.CompareTo(y));

            //        int maxColor, minColor;
            //        maxColor = lstColors.Count;
            //        lstColors.Add(graphUnitProvider.MaxColor(tabInfo));
            //        minColor = lstColors.Count;
            //        lstColors.Add(graphUnitProvider.MinColor(tabInfo));

            //        double max = double.MinValue, min = double.MaxValue;
            //        if (lstVals.Count > 0)
            //        {
            //            if (order == 1)
            //            {
            //                min = lstVals[0];
            //                max = lstVals[lstVals.Count - 1];
            //            }
            //            else
            //                if (order == -1)
            //                {
            //                    max = lstVals[0];
            //                    min = lstVals[lstVals.Count - 1];
            //                }
            //                else
            //                {
            //                    foreach (var item in lstVals)
            //                    {
            //                        if (item > max) max = item;
            //                        if (item < min) min = item;
            //                    }
            //                }
            //        }
            //        foreach (var item in lstVals)
            //        {
            //            point = new PointPair();
            //            if (item == min && graphUnitProvider.MinColor(tabInfo) != Color.Empty && min != max)
            //                point.Z = minColor;
            //            else
            //                if (item == max && graphUnitProvider.MaxColor(tabInfo) != Color.Empty && min != max)
            //                    point.Z = maxColor;
            //                else
            //                    point.Z = dic[item].Z;
            //            if (tabInfo.Horizontal)
            //            {
            //                point.X = dic[item].Y;
            //                point.Y = dic[item].Z;
            //            }
            //            else
            //            {
            //                point.X = dic[item].X;
            //                point.Y = dic[item].Y;
            //            }
            //            lstLabs.Add(dic[item].Label);
            //            bitem.AddPoint(point);
            //        }

            //        if (lstColors.Count > 1)
            //        {
            //            bitem.Bar.Fill = new Fill(lstColors.ToArray());
            //            bitem.Bar.Fill.RangeMin = 0;
            //            bitem.Bar.Fill.RangeMax = lstColors.Count - 1;
            //            bitem.Bar.Fill.Type = FillType.GradientByZ;
            //        }
            //        else
            //        {
            //            bitem.Bar.Fill = new Fill(lstColors[0]);
            //            bitem.Bar.Fill.Type = FillType.Solid;
            //        }
            //        bitem.Bar.Fill.SecondaryValueGradientColor = Color.Empty;
            //        graph.GraphPane.CurveList.Add(bitem);
            //        if (tabInfo.Horizontal)
            //            graph.GraphPane.YAxis.Scale.TextLabels = lstLabs.ToArray();
            //        else
            //            graph.GraphPane.XAxis.Scale.TextLabels = lstLabs.ToArray();
            //    }
            //}
            //if (isHistogram) ViewValue(0);
            //if (graph.GraphPane.Y2AxisList.Count == 0) graph.GraphPane.Y2AxisList.Add("");
            //if (graph.GraphPane.YAxisList.Count == 0) graph.GraphPane.YAxisList.Add("");
            //using (Graphics g = this.CreateGraphics())
            //{
            //    if (!isHistogram)
            //    {
            //        graph.GraphPane.XAxis.Scale.Min = (double)(new ZedGraph.XDate(state.GraphFrom));
            //        graph.GraphPane.XAxis.Scale.Max = (double)(new ZedGraph.XDate(state.GraphTo));

            //        SetDefaultXScale(state.GraphFrom, state.GraphTo);
            //    }
            //}
            GraphUnitProvider gprov = unitProvider as GraphUnitProvider;
            gprov.FillPane(graph.GraphPane, currentParameter, maxVisibleParameters, GraphUnitProvider.HistogramSorting.None);
            graph.AxisChange();
            if (!isHistogram) SetMinMaxDate(datFrom.Value, datTo.Value);
            graph.Refresh();
        }

        private Dictionary<TKey, TValue> CreateDictionary<TKey, TValue>(TKey key, TValue value)
        {
            return new Dictionary<TKey, TValue>() { { key, value } };
        }

        private void FillTimePeriod()
        {
            DataTable periods = new DataTable();

            periods.Columns.Add(new DataColumn("Text", typeof(string)));
            periods.Columns.Add(new DataColumn("Value", typeof(GraphTimePeriod)));

            periods.Rows.Add(GraphTimePeriodFormatter.Format(GraphTimePeriod.Minutes30),
                GraphTimePeriod.Minutes30);
            periods.Rows.Add(GraphTimePeriodFormatter.Format(GraphTimePeriod.Hours1),
                GraphTimePeriod.Hours1);
            periods.Rows.Add(GraphTimePeriodFormatter.Format(GraphTimePeriod.Hours4),
                GraphTimePeriod.Hours4);
            periods.Rows.Add(GraphTimePeriodFormatter.Format(GraphTimePeriod.Days1),
                GraphTimePeriod.Days1);
            periods.Rows.Add(GraphTimePeriodFormatter.Format(GraphTimePeriod.Days30),
                GraphTimePeriod.Days30);
            periods.Rows.Add(GraphTimePeriodFormatter.Format(GraphTimePeriod.User),
                GraphTimePeriod.User);

            cbxPeriod.DisplayMember = "Text";
            cbxPeriod.ValueMember = "Value";
            cbxPeriod.DataSource = periods;
        }

        private void queryDataBtn_Click(object sender, EventArgs e)
        {
            RequestData();
        }

        public void RequestData()
        {
            try
            {
                GraphUnitProvider graphUnitProvider = unitProvider as GraphUnitProvider;
                GraphUnitProviderState state = graphUnitProvider.GetState(tabInfo);
                state.GraphFrom = datFrom.Value;
                state.GraphTo = datTo.Value;

                graphUnitProvider.ClearValues(tabInfo);
                UpdateDGV();

                AsyncOperationWatcher watcher = null;
                if (isHistogram)
                    watcher = graphUnitProvider.QueryHistogramData(tabInfo, datFrom.Value, datTo.Value, chbxRemoteServer.Checked, true);
                else
                    watcher = graphUnitProvider.QueryGraphData(tabInfo, datFrom.Value, datTo.Value, chbxRemoteServer.Checked, true);

                if (UniForm != null && watcher != null)
                {
                    UniForm.RunWatcher(watcher);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                SaveUnitData();
            }
        }

        #region Init XY scale and Zero point
        void InitGraphProperties()
        {
            graph.GraphPane.GraphObjList.Clear();
            graph.GraphPane.XAxis.Title.Text = "   ";

            if (isHistogram)
            {
                CommonGraphUnitProvider graphUnitProvider = UnitProvider as CommonGraphUnitProvider;
                if (tabInfo.Horizontal)
                {
                    graph.GraphPane.YAxis.Type = AxisType.Text;
                }
                else
                {
                    graph.GraphPane.XAxis.Type = AxisType.Text;
                }
            }
            else
            {
                // Ось X - это дата
                graph.GraphPane.XAxis.Type = AxisType.Date;

                graph.GraphPane.XAxis.Scale.FontSpec.IsBold = true;
                graph.GraphPane.Legend.Position = LegendPos.TopFlushLeft;
                graph.GraphPane.Legend.FontSpec.Size = 12;
            }

            // tilt the x axis labels to an angle of 65 degrees
            graph.GraphPane.XAxis.Scale.FontSpec.Angle = 65;
            graph.GraphPane.XAxis.Scale.FontSpec.Size = 10;
            // Enable scrollbars if needed
            graph.IsScrollY2 = false;
            graph.IsShowHScrollBar = true;
            graph.IsShowVScrollBar = false;
            graph.IsAutoScrollRange = true;
            // Zooming
            graph.IsEnableHZoom = true;
            graph.IsEnableVZoom = false;
            graph.IsEnableWheelZoom = true;
            graph.IsZoomOnMouseCenter = true;
            // оси между собой не синхронизованы
            graph.IsSynchronizeYAxes = false;
            // Show tooltips when the mouse hovers over a point
            graph.IsShowPointValues = true;

            graph.AxisChange();
        }
        void SetZeroCoordDate()
        {
            ZedGraph.XDate dat = new ZedGraph.XDate(graph.GraphPane.XAxis.Scale.Min);
            if (graph.GraphPane.GraphObjList.Count > 0 && graph.GraphPane.GraphObjList[0] is TextObj)
                ((TextObj)graph.GraphPane.GraphObjList[0]).Text = "Дата: " + dat.ToString();
        }
        void SetDefaultXScale(DateTime minDate, DateTime maxDate)
        {
            double interval = ((TimeSpan)(maxDate - minDate)).TotalSeconds;
            if (interval == 0) interval = 0.1f; //чисто штоб не нулик
            if (interval <= 60f) // 
            {
                graph.GraphPane.XAxis.Scale.Format = "H:mm:ss fff";
                graph.GraphPane.XAxis.Scale.MinorStep = 1 / (60f / interval);
                graph.GraphPane.XAxis.Scale.MinorUnit = DateUnit.Second;
                graph.GraphPane.XAxis.Scale.MajorStep = 5 * graph.GraphPane.XAxis.Scale.MinorStep;
                graph.GraphPane.XAxis.Scale.MajorUnit = DateUnit.Second;
            }
            else if (interval <= 3600f) // час
            {
                graph.GraphPane.XAxis.Scale.Format = "H:mm:ss";
                graph.GraphPane.XAxis.Scale.MinorStep = 1 / (3600f / interval);
                graph.GraphPane.XAxis.Scale.MinorUnit = DateUnit.Minute;
                graph.GraphPane.XAxis.Scale.MajorStep = 5 * graph.GraphPane.XAxis.Scale.MinorStep;
                graph.GraphPane.XAxis.Scale.MajorUnit = DateUnit.Minute;
            }
            else if (interval <= 86400f) // 24 часа
            {
                graph.GraphPane.XAxis.Scale.Format = "dd.MM H:mm:ss";
                graph.GraphPane.XAxis.Scale.MinorStep = 1 / (86400f / interval);
                graph.GraphPane.XAxis.Scale.MinorUnit = DateUnit.Hour;
                graph.GraphPane.XAxis.Scale.MajorStep = 5 * graph.GraphPane.XAxis.Scale.MinorStep;
                graph.GraphPane.XAxis.Scale.MajorUnit = DateUnit.Hour;

            }
            else if (interval <= 2592000f) // 1 месяц
            {
                graph.GraphPane.XAxis.Scale.Format = "dd.MM H:mm:ss";
                graph.GraphPane.XAxis.Scale.MinorStep = 1 / (2592000f / interval);
                graph.GraphPane.XAxis.Scale.MinorUnit = DateUnit.Day;
                graph.GraphPane.XAxis.Scale.MajorStep = 7 * graph.GraphPane.XAxis.Scale.MinorStep;
                graph.GraphPane.XAxis.Scale.MajorUnit = DateUnit.Day;
            }
            else
            {
                graph.GraphPane.XAxis.Scale.Format = "dd.MM.yy";
                graph.GraphPane.XAxis.Scale.MinorStep = 1;
                graph.GraphPane.XAxis.Scale.MinorUnit = DateUnit.Month;
                graph.GraphPane.XAxis.Scale.MajorStep = 6 * graph.GraphPane.XAxis.Scale.MinorStep;
                graph.GraphPane.XAxis.Scale.MajorUnit = DateUnit.Month;
            }
        }
        void SetMinMaxDate(DateTime? minDate, DateTime? maxDate)
        {
            // если мин > макс, то махнуть местами
            if (minDate != null && maxDate != null && minDate > maxDate)
            {
                DateTime _mmDate = (DateTime)minDate;
                minDate = maxDate;
                maxDate = _mmDate;
            }

            if (minDate == null) minDate = DateTime.Today;

            if (maxDate == null)
            {
                maxDate = minDate;
                if (step > 0) maxDate = ((DateTime)maxDate).AddSeconds(step);
                else maxDate = ((DateTime)maxDate).AddMinutes(30);
            }


            ZedGraph.XDate _min, _max;
            _min = new ZedGraph.XDate((DateTime)minDate);
            _max = new ZedGraph.XDate((DateTime)maxDate);

            // формат даты в зависимости от журнала
            SetDefaultXScale((DateTime)minDate, (DateTime)maxDate);

            double minor = GetMinorUnitValue(graph.GraphPane.XAxis.Scale.MinorUnit);
            double shift = minor * graph.GraphPane.XAxis.Scale.MinorStep;
            graph.ScrollMinX = (double)_min;
            graph.ScrollMaxX = (double)_max;

            graph.GraphPane.XAxis.Scale.Min = (double)_min;
            graph.GraphPane.XAxis.Scale.Max = (double)_max;
            
            SetZeroCoordDate();
        }
        #endregion
        
        private void graph_Click(object sender, EventArgs e)
        {
            double x, y;
            graph.GraphPane.ReverseTransform(graph.PointToClient(MousePosition), out x, out y);
            VisirDraw(x);
        }

        private void dgvValues_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (updating)
                return;
            CommonGraphUnitProvider graphUnitProvider = unitProvider as CommonGraphUnitProvider;
            if (dgvValues.Columns["used"] != null && e.ColumnIndex == dgvValues.Columns["used"].Index)
            {
                if (dgvValues.Rows[e.RowIndex].Tag != null)
                {
                    object tag= dgvValues.Rows[e.RowIndex].Tag;
                    bool visible;
                    var item = graphUnitProvider.Curves(tabInfo).Find(i => i.Tag.Equals(tag));
                    if (item != null)
                    {
                        visible = (bool)dgvValues.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue;
                        item.IsVisible = visible;
                    }
                    UpdateGraph();
                }
            }
        }

        private void dgvValues_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvValues.IsCurrentCellDirty)
            {
                if ((bool)dgvValues.CurrentCell.FormattedValue)
                    dgvValues.CurrentCell.Value = false;
                else
                    dgvValues.CurrentCell.Value = true;

                dgvValues.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            CommonGraphUnitProvider graphUnitProvider = unitProvider as CommonGraphUnitProvider;
            if (graphUnitProvider != null)
                graphUnitProvider.GetState(tabInfo).SplitterDistance = (float)splitContainer1.SplitterDistance / splitContainer1.Height;
        }

        #region Инфо об интервале
        private void cbxPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            GraphTimePeriod period;

            if (cbxPeriod.SelectedValue == null) return;

            period = (GraphTimePeriod)cbxPeriod.SelectedValue;
            step = GraphTimePeriodFormatter.FormatInterval(period);
            switch (period)
            {
                case GraphTimePeriod.Minutes30:
                    datTo.Enabled = false;
                    break;
                case GraphTimePeriod.Hours1:
                    datTo.Enabled = false;
                    break;
                case GraphTimePeriod.Hours4:
                    datTo.Enabled = false;
                    break;
                case GraphTimePeriod.Days1:
                    datTo.Enabled = false;
                    break;
                case GraphTimePeriod.Days30:
                    datTo.Enabled = false;
                    break;
                //case GraphTimePeriod.User:
                default:
                    datTo.Enabled = true;
                    break;
            }
            UpdateDateTo();
        }

        private void UpdateDateTo()
        {
            if (step != 0)
                datTo.Value = datFrom.Value.AddSeconds(step);
        }

        private void btnNextDate_Click(object sender, EventArgs e)
        {
            MoveDate(1);//next
        }

        private void MoveDate(int direction)
        {
            double shift;

            if (step == 0)
                step = Math.Abs(datTo.Value.Subtract(datFrom.Value).TotalSeconds);

            shift = (double)direction * step;
            datFrom.Value = datFrom.Value.AddSeconds(shift);
            if (chbxRequestData.Checked) RequestData();
        }

        private void btnPrevDate_Click(object sender, EventArgs e)
        {
            MoveDate(-1);//prev
        }

        private void datFrom_ValueChanged(object sender, EventArgs e)
        {
            UpdateDateTo();
        }
        #endregion

        #region Инфо о страницах
        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (currentParameter + maxVisibleParameters < this.UnitNode.Parameters.Length)
            {
                currentParameter += maxVisibleParameters;
                UpdateGraph();
                UpdatePageInfo();
            }
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            if (currentParameter - maxVisibleParameters >= 0)
            {
                currentParameter -= maxVisibleParameters;
                UpdateGraph();
                UpdatePageInfo();
            }
        }

        private void UpdatePageInfo()
        {
            CommonGraphUnitProvider graphUnitProvider = UnitProvider as CommonGraphUnitProvider;
            int count;

            if (graphUnitProvider!=null)
            {
                count = graphUnitProvider.ParameterCount(tabInfo);
                lblPageInfo.Text = string.Format("{0}/{1}",
                        currentParameter / maxVisibleParameters + 1,
                        Math.Ceiling((decimal)count / maxVisibleParameters));
                btnNextPage.Enabled = currentParameter + maxVisibleParameters < count;
                btnPrevPage.Enabled = currentParameter - maxVisibleParameters >= 0; 
            }
        }

        private void udPageParams_ValueChanged(object sender, EventArgs e)
        {
            maxVisibleParameters = (int)udPageParams.Value;
            UpdateGraph();
            UpdatePageInfo();
        }
        #endregion

        private void cbxSorting_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateGraph();
        }

        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = unitProvider.UnitNode.Text;

            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                using (Stream s = saveFileDialog1.OpenFile())
                {
                    GridToCSV(s, dataGridView1, null, null);
                }
            }

        }

        private void graph_Resize(object sender, EventArgs e)
        {
            if (graph.Height < 50)
                graph.Height = 50;
            else if (graph.Top + graph.Height >= graph.Parent.Height)
                graph.Height = graph.Parent.Height - graph.Top - 3;
            if (graph.Width < 50)
                graph.Width = 50;
            else if (graph.Left + graph.Width >= graph.Parent.Width)
                graph.Width = graph.Parent.Width - graph.Left - 3;
        }
    }

    class ParamSettings
    {
        public int Id { get; set; }
        public bool HasAperture { get; set; }
        public double Interval { get; set; }
    }
}
