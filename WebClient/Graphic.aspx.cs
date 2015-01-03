using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using COTES.ISTOK.ASC;
using ZedGraph.Web;
using ZedGraph;
using System.Drawing;
using AjaxControlToolkit;
using COTES.ISTOK;
using System.Collections.Generic;

namespace WebClient
{
    public partial class Graphic : TreeContentPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //IValuesRequest valuesRequest = DataService.GetValueRequest();

            //GraphParamContainer paramContainer = new GraphParamContainer();

            //try
            //{
            string sid = Parameters[Configuration.Get(Setting.IdObjectMarker)];
            int id = string.IsNullOrEmpty(sid) ? 0 : int.Parse(sid);

            UnitNode node =
                DataService.GetUnitNode(id);

            //paramContainer.Text = node.Text;
            //paramContainer.IsHistogram = node.GetType().Equals(typeof(HistogramNode));

            List<GraphParamDescriptor> graphParamList = new List<GraphParamDescriptor>();
            GraphParamDescriptor graphParam;

            foreach (var item in node.Parameters)
            {
                graphParamList.Add(new GraphParamDescriptor
                {
                    Text = item.Text,
                    ParameterId = item.ParameterId,
                    Idnum = item.Idnum,
                });
            }
            GraphParamContainer paramContainer = new GraphParamContainer(graphParamList)
            {
                Text = node.Text,
                IsHistogram = node.GetType().Equals(typeof(HistogramNode))
            };

            GraphUserControl1.ValueRequest = DataService.GetValueRequest();
            GraphUserControl1.GraphParams = paramContainer;

            //    if (!(node is GraphNode || node is HistogramNode)) return;

            //    chbxShowMarkers.Visible = node.GetType().Equals(typeof(GraphNode));

            //    var graph_pane = pane[0];

            //    DateTime from_time, to_time;
            //    if (GetCorrectDates(out from_time, out to_time))
            //    {
            //        DataTable graph_content;
            //        CurveList curves = null;
            //        if (node.GetType().Equals(typeof(GraphNode)))
            //            curves = CreateCurves(node as GraphNode,
            //                                  from_time,
            //                                  to_time,
            //                                  out graph_content);
            //        else
            //            curves = CreateCurves(node as HistogramNode,
            //                                  from_time,
            //                                  to_time,
            //                                  out graph_content);

            //        GridViewGraphData.DataSource = graph_content;
            //        GridViewGraphData.DataBind();

            //        foreach (var it in curves)
            //            graph_pane.CurveList.Add(it);
            //    }

            //    webObject.Title = node.Text;
            //    graph_pane.XAxis.Type = AxisType.Date;
            //    if (from_time != to_time && from_time != DateTime.MinValue)
            //    {
            //        graph_pane.XAxis.Scale.Min = ((double)new XDate(from_time));
            //        graph_pane.XAxis.Scale.Max = ((double)new XDate(to_time));
            //    }
            //}
            //catch (BaseWebClientException exp)
            //{
            //    exp.Accept(Handler.Instance);
            //}
            //catch (Exception exp)
            //{
            //    Handler.Instance.ProcessUndefined(exp);
            //}

        }
        //public enum IntervalMarker : byte
        //{
        //    HalfAnHour,
        //    OneHour,
        //    FourHours,
        //    EightHours,
        //    Day,
        //    Month,
        //    Custom,
        //}

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        SetCustomMode(DropDownListInterval.SelectedIndex == (int)IntervalMarker.Custom);
        //        if (FromTime.Text == "") FromTime.Text = "00:00:00";
        //        if (FromDate.Text == "") FromDate.Text = DateTime.Now.ToString("dd.MM.yyyy");
        //        if (ToTime.Text == "") ToTime.Text = "00:00:00";
        //        if (ToDate.Text == "") ToDate.Text = DateTime.Now.ToString("dd.MM.yyyy");

        //        SetSpecificTab(true);
        //    }
        //    catch (BaseWebClientException exp)
        //    {
        //        exp.Accept(Handler.Instance);
        //    }
        //    catch (Exception exp)
        //    {
        //        Handler.Instance.ProcessUndefined(exp);
        //    }
        //}

        //protected void Graph_RenderGraph(ZedGraphWeb webObject, 
        //                                 System.Drawing.Graphics g, 
        //                                 ZedGraph.MasterPane pane)
        //{
        //    try
        //    {
        //        string sid = Parameters[Configuration.Get(Setting.IdObjectMarker)];
        //        int id = string.IsNullOrEmpty(sid) ? 0 : int.Parse(sid);

        //        UnitNode node =
        //            DataService.GetUnitNode(id);

        //        if (!(node is GraphNode || node is HistogramNode)) return;

        //        chbxShowMarkers.Visible = node.GetType().Equals(typeof(GraphNode));

        //        var graph_pane = pane[0];

        //        DateTime from_time, to_time;
        //        if (GetCorrectDates(out from_time, out to_time))
        //        {
        //            DataTable graph_content;
        //            CurveList curves = null;
        //            if (node.GetType().Equals(typeof(GraphNode)))
        //                curves = CreateCurves(node as GraphNode,
        //                                      from_time,
        //                                      to_time,
        //                                      out graph_content);
        //            else
        //                curves = CreateCurves(node as HistogramNode,
        //                                      from_time,
        //                                      to_time,
        //                                      out graph_content);

        //            GridViewGraphData.DataSource = graph_content;
        //            GridViewGraphData.DataBind();

        //            foreach (var it in curves)
        //                graph_pane.CurveList.Add(it);
        //        }

        //        webObject.Title = node.Text;
        //        graph_pane.XAxis.Type = AxisType.Date;
        //        if (from_time != to_time && from_time != DateTime.MinValue)
        //        {
        //            graph_pane.XAxis.Scale.Min = ((double)new XDate(from_time));
        //            graph_pane.XAxis.Scale.Max = ((double)new XDate(to_time));
        //        }
        //    }
        //    catch (BaseWebClientException exp)
        //    {
        //        exp.Accept(Handler.Instance);
        //    }
        //    catch (Exception exp)
        //    {
        //        Handler.Instance.ProcessUndefined(exp);
        //    }
        //}

        //private CurveList CreateCurves(GraphNode node,
        //                               DateTime from_date,
        //                               DateTime to_date,
        //                               out DataTable value_data)
        //{
        //    CurveList result = new CurveList();
        //    value_data = new DataTable();

        //    DataTable prototype = CreatePrototypeTable();

        //    DataTable temp_table = null;
        //    LineItem temp_line = null;
        //    foreach (GraphParamNode it in node.Parameters)
        //    {
        //        var param_values = GetParameterValues(it.ParameterId,
        //                                             from_date,
        //                                             to_date);

        //        temp_table = prototype.Copy();
        //        temp_table.Columns.Add(it.Text, typeof(double));

        //        FillTable(temp_table, param_values);

        //        temp_line = new LineItem(it.Text,
        //                                 ToPointList(param_values),
        //                                 GetColor(it.LineColor),
        //                                 (SymbolType)it.LineSymbol,
        //                                 (float)it.LineWidth);

        //        temp_line.Line.Style = (System.Drawing.Drawing2D.DashStyle)it.LineStyle;
        //        temp_line.Symbol.IsVisible = chbxShowMarkers.Checked;
        //        temp_line.Tag = it.Idnum;
        //        temp_line.Link = new Link("", "", "");

        //        result.Add(temp_line);

        //        value_data.Merge(temp_table, false, MissingSchemaAction.Add);
        //    }

        //    return result;
        //}

        //private CurveList CreateCurves(HistogramNode node,
        //                               DateTime from,
        //                               DateTime to,
        //                               out DataTable value_data)
        //{
        //    CurveList result = new CurveList();
        //    value_data = new DataTable();

        //    DataTable prototype = CreatePrototypeTable();

        //    DataTable temp_table = null;
        //    BarItem temp_bar = null;
        //    foreach (HistogramParamNode it in node.Parameters)
        //    {
        //        var param_values = GetParameterValues(it.ParameterId,
        //                                             from,
        //                                             to,
        //                                             it.Aggregation);

        //        temp_table = prototype.Copy();
        //        temp_table.Columns.Add(it.Text, typeof(double));

        //        FillTable(temp_table, param_values);

        //        temp_bar = new BarItem(it.Text,
        //                               ToPointList(param_values),
        //                               GetColor(it.LineColor));

        //        temp_bar.Tag = it.Idnum;

        //        result.Add(temp_bar);

        //        value_data.Merge(temp_table, false, MissingSchemaAction.Add);
        //    }

        //    return result;
        //}

        //private static DataTable CreatePrototypeTable()
        //{
        //    DataTable prototype = new DataTable();

        //    var merge_base_column = new DataColumn(@"Date", typeof(DateTime));
        //    prototype.Columns.Add(merge_base_column);
        //    prototype.PrimaryKey = new[] { merge_base_column };

        //    return prototype;
        //}

        //private static Color GetColor(Color seted_color)
        //{
        //    return seted_color == Color.Empty ?
        //            GenerateColor() : 
        //            seted_color; 
        //}

        //private static Color GenerateColor()
        //{
        //    const int max_color_component = 255;

        //    Random rnd = new Random(DateTime.Now.Millisecond * DateTime.Now.Second /
        //                            (DateTime.Now.Hour * DateTime.Now.Day * DateTime.Now.Minute + 1));

        //    return Color.FromArgb(rnd.Next(max_color_component),
        //                          rnd.Next(max_color_component),
        //                          rnd.Next(max_color_component));
        //}

        //private IEnumerable<ParamValueItem> GetParameterValues(int id_parametr,
        //                                                         DateTime from_date,
        //                                                         DateTime to_date)
        //{

        //    return DataService.GetValues(id_parametr,
        //                                 from_date,
        //                                 to_date,
        //                                 chbxUseBlock.Checked);
        //}

        //private IEnumerable<ParamValueItem> GetParameterValues(int id_parametr,
        //                                                         DateTime from_date,
        //                                                         DateTime to_date,
        //                                                         CalcAggregation agregation)
        //{
        //    WebRemoteDataService data_service = DataService;

        //    Interval interval = Interval.Zero;

        //    UnitNode parametr_node = data_service.GetUnitNode(id_parametr);
        //    DataService.GetParameterInterval(parametr_node as ParameterNode);
        //    //if (parametr_node != null &&
        //    //    parametr_node is ParameterNode)
        //    //    interval = (parametr_node as ParameterNode).Interval;

        //    return data_service.GetValues(id_parametr,
        //                                  from_date,
        //                                  to_date,
        //                                  interval,
        //                                  agregation,
        //                                  chbxUseBlock.Checked);
        //}

        //private static void FillTable(DataTable table, 
        //                              IEnumerable<ParamValueItem> values)
        //{
        //    foreach (var it in values)
        //        table.Rows.Add(it.Time, it.Value);
        //}

        //private static IPointList ToPointList(IEnumerable<ParamValueItem> values)
        //{
        //    PointPairList result = new PointPairList();

        //    foreach (var it in values)
        //        result.Add(new XDate(it.Time),
        //                   it.Value,
        //                   string.Format("{0} {1}", it.Time, it.Value));//тут бы округлить число

        //    return result;
        //}

        //private bool GetCorrectDates(out DateTime from_date,
        //                            out DateTime to_date)
        //{
        //    return GetDates(out from_date, out to_date) && 
        //           from_date <= to_date;
        //}

        //private bool GetDates(out DateTime from_date,
        //                      out DateTime to_date)
        //{
        //    try
        //    {
        //        from_date = DateTime.Parse(FromDate.Text);
        //        TimeSpan time = TimeSpan.Parse(FromTime.Text);
        //        from_date += time;

        //        if (DropDownListInterval.SelectedIndex == (int)IntervalMarker.Custom)
        //        {
        //            to_date = DateTime.Parse(ToDate.Text);
        //            time = TimeSpan.Parse(ToTime.Text);
        //            to_date += time;
        //        }
        //        else
        //            to_date = from_date + GetTimeShiftValue();

        //        return true;
        //    }
        //    catch
        //    {
        //        from_date = to_date = DateTime.Now;

        //        return false;
        //    }
        //}

        //protected void DrawButton_Click(object sender, EventArgs e)
        //{
        //    // this handler must be empty
        //    // it is need for graph refresh 
        //}

        //protected void DropDownListInterval_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    SetCustomMode(DropDownListInterval.SelectedIndex == (int)IntervalMarker.Custom);
        //}

        //private TimeSpan GetTimeShiftValue()
        //{
        //    switch ((IntervalMarker)DropDownListInterval.SelectedIndex)
        //    {
        //        case IntervalMarker.HalfAnHour:
        //            return TimeSpan.FromMinutes(30f);
        //        case IntervalMarker.OneHour:
        //            return TimeSpan.FromHours(1f);
        //        case IntervalMarker.FourHours:
        //            return TimeSpan.FromHours(4f);
        //        case IntervalMarker.EightHours:
        //            return TimeSpan.FromHours(8f);
        //        case IntervalMarker.Day:
        //            return TimeSpan.FromDays(1f);
        //        case IntervalMarker.Month:
        //            return TimeSpan.FromDays(30f);
        //        default:
        //            {
        //                DateTime to, from;
        //                return GetCorrectDates(out from, out to) ? to - from: TimeSpan.FromTicks(0);
        //            }
        //    }
        //}

        //private void SetCustomMode(bool enable)
        //{
        //    ToDate.Visible = 
        //        ToTime.Visible = 
        //            ToCalendarButton.Visible =
        //                ToDateLabel.Visible =
        //                    ToTimeLabel.Visible =
        //                        ToLabel.Visible = enable;

        //    ToDateMasketValidator.Visible =
        //        ToTimeMaskedEditValidator.Visible =
        //            ToTimeMaskedEditValidator.Enabled =
        //                ToDateMasketValidator.Enabled = enable;
        //}

        //private void UpdateDate(TextBox date_container,
        //                        TextBox time_container,
        //                        TimeSpan shift,
        //                        bool add)
        //{
        //    try
        //    {

        //        TimeSpan one_day = TimeSpan.FromDays(1f);

        //        DateTime current_date = DateTime.Parse(date_container.Text);
        //        TimeSpan current_time = TimeSpan.Parse(time_container.Text);

        //        if (!add) shift = shift.Negate();

        //        if (Math.Abs(shift.Ticks) < one_day.Ticks)
        //        {
        //            current_time += shift;
        //            if (current_time.Ticks < 0)
        //            {
        //                current_date -= one_day;
        //                current_time = one_day + current_time; // - (-current_time)
        //            }
        //            else if (current_time >= one_day)
        //            {
        //                current_date += one_day;
        //                current_time -= one_day;
        //            }
        //        }
        //        else
        //            current_date += shift;

        //        date_container.Text = current_date.ToString(FromCalendar.Format);
        //        time_container.Text = current_time.ToString();

        //        ValidateDates();
        //    }
        //    catch { }
        //}

        //public void CorrectDates(bool add)
        //{
        //    TimeSpan shift = GetTimeShiftValue();

        //    UpdateDate(FromDate, FromTime, shift, add);

        //    if (DropDownListInterval.SelectedIndex == (int)IntervalMarker.Custom)
        //        UpdateDate(ToDate, ToTime, shift, add);
        //}

        //protected void ImageButtonSubDate_Click(object sender, ImageClickEventArgs e)
        //{
        //    CorrectDates(false);
        //}

        //protected void ImageButtonAddDate_Click(object sender, ImageClickEventArgs e)
        //{
        //    CorrectDates(true);
        //}

        //private void ValidateDates()
        //{
        //    FromTimeMaskedValidator.Validate();
        //    FromDateMasketValidator.Validate();
        //    ToTimeMaskedEditValidator.Validate();
        //    ToDateMasketValidator.Validate();
        //}
    }
}
