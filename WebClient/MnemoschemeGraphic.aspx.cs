using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using COTES.ISTOK;
using COTES.ISTOK.ASC;
using ZedGraph;
using ZedGraph.Web;

namespace WebClient
{
    public partial class MnemoschemeGraphic : WebClientPage
    {
        ////private enum IntervalMarker : byte
        ////{
        ////    HalfAnHour,
        ////    OneHour,
        ////    FourHours,
        ////    EightHours,
        ////    Day,
        ////    Month,
        ////    Custom,
        ////}

        ////List<int> parameterIds;

        //private String CurvesSessionVarName(int mnemoschemeID)
        //{
        //    return String.Format("_mnemoscheme_graphic_curves{0}", mnemoschemeID);
        //}

        //private String ParametersSessionVarName(int mnemoschemeID)
        //{
        //    return String.Format("_mnemoscheme_graphic_parmeters{0}", mnemoschemeID);
        //}

        //private String TimeFromSessionVarName(int mnemoschemeID)
        //{
        //    return String.Format("_mnemoscheme_graphic_timefrom{0}", mnemoschemeID);
        //}

        //private String IntervalSessionVarName(int mnemoschemeID)
        //{
        //    return String.Format("_mnemoscheme_graphic_interval{0}", mnemoschemeID);
        //}

        //int mnemoschemeID;
        //List<int> parameterIds;
        //Dictionary<int,LineItem> tempCurveList;

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    //CurveList list = Session["_mnemoscheme_graphic_curves"] as CurveList;
        //    //if (parameterIds == null)
        //    //{
        //    //    parameterIds = (from k in Request.Params["params"].Split('|') select int.Parse(k)).ToList();
        //    //    DateTime timeFrom = DateTime.Parse(Request.Params["start"]);
        //    //    DateTime timeTo = DateTime.Parse(Request.Params["end"]);
        //    //    WebClient.Graphic.IntervalMarker marker = (WebClient.Graphic.IntervalMarker)int.Parse(Request.Params["interval"]);

        //    //    DropDownListInterval.SelectedIndex = (int)marker;

        //    //    SetCustomMode(DropDownListInterval.SelectedIndex == (int)WebClient.Graphic.IntervalMarker.Custom);
        //    //    if (FromTime.Text == "") FromTime.Text = timeFrom.ToString("HH:mm:ss");
        //    //    if (FromDate.Text == "") FromDate.Text = timeFrom.ToString("dd.MM.yyyy");
        //    //}
        //    mnemoschemeID = int.Parse(Request.Params["schemaid"]);
        //    if(!String.IsNullOrEmpty( Request.Params["params"]))
        //    {
        //        // добавляем переданные параметры в список
        //        var parameterIds = (from k in Request.Params["params"].Split('|') select int.Parse(k));

        //        List<int> parameterList = Session[ParametersSessionVarName(mnemoschemeID)] as List<int>;

        //        if (parameterList == null)
        //        {
        //            Session[ParametersSessionVarName(mnemoschemeID)] = parameterList = new List<int>();
        //        }
        //        foreach (var item in parameterIds)
        //        {
        //            if (!parameterList.Contains(item))
        //                parameterList.Add(item);
        //        }

        //        // корректируем настройки времени
        //        if (!String.IsNullOrEmpty(Request.Params["start"]))
        //            Session[TimeFromSessionVarName(mnemoschemeID)] = DateTime.Parse(Request.Params["start"], System.Globalization.DateTimeFormatInfo.InvariantInfo);
        //        {
        //            if (!String.IsNullOrEmpty(Request.Params["interval"]))
        //                Session[IntervalSessionVarName(mnemoschemeID)] = (WebClient.Graphic.IntervalMarker)int.Parse(Request.Params["interval"]);

        //            WebClient.Graphic.IntervalMarker marker = (WebClient.Graphic.IntervalMarker)int.Parse(Request.Params["interval"]);

        //            DropDownListInterval.SelectedIndex = (int)marker;
        //        }
        //        // перегружаем страничку уже с меньшим количеством параметров
        //        Response.Redirect(String.Format("{0}?schemaid={1}", Request.Path, mnemoschemeID));
        //    }
        //    else
        //    {
        //        DateTime timeFrom;
        //        //WebClient.Graphic.IntervalMarker marker;

        //        // достоем из Session информацию о запрашиваемом времени
        //        if (Session[TimeFromSessionVarName(mnemoschemeID)] != null)
        //            timeFrom = (DateTime)Session[TimeFromSessionVarName(mnemoschemeID)];
        //        else timeFrom = DateTime.Now;
        //        //if (Session[IntervalSessionVarName(mnemoschemeID)] != null)
        //        //    marker = (WebClient.Graphic.IntervalMarker)Session[IntervalSessionVarName(mnemoschemeID)];
        //        //else marker = Graphic.IntervalMarker.Custom;

        //        // берем из сесии кэш
        //        parameterIds = Session[ParametersSessionVarName(mnemoschemeID)] as List<int>;
        //        tempCurveList = Session[CurvesSessionVarName(mnemoschemeID)] as Dictionary<int, LineItem>;

        //        if (tempCurveList == null)
        //            Session[CurvesSessionVarName(mnemoschemeID)] = tempCurveList = new Dictionary<int, LineItem>();

        //        //WebClient.Graphic.IntervalMarker marker = (WebClient.Graphic.IntervalMarker)int.Parse(Request.Params["interval"]);
        //        //WebClient.Graphic.IntervalMarker marker = Graphic.IntervalMarker.Custom;
                

        //        //DropDownListInterval.SelectedIndex = (int)marker;

        //        SetCustomMode(DropDownListInterval.SelectedIndex == (int)WebClient.Graphic.IntervalMarker.Custom);
        //        if (FromTime.Text == "") FromTime.Text = timeFrom.ToString("HH:mm:ss");
        //        if (FromDate.Text == "") FromDate.Text = timeFrom.ToString("dd.MM.yyyy");
        //    }
        //    //if (ToTime.Text == "") ToTime.Text = timeTo.ToString("HH:mm:ss");
        //    //if (ToDate.Text == "") ToDate.Text = timeTo.ToString("dd.MM.yyyy");

        //    //SetSpecificTab(true);
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

        //protected void Graph_RenderGraph(ZedGraphWeb webObject,
        //                                 System.Drawing.Graphics g,
        //                                 ZedGraph.MasterPane pane)
        //{
        //    try
        //    {
        //        List<int> parameterIds = Session[ParametersSessionVarName(mnemoschemeID)] as List<int>;

        //        List<ParameterNode> parameterList = new List<ParameterNode>();
        //        if (parameterIds != null)
        //            foreach (var item in parameterIds)
        //            {
        //                ParameterNode parameter = DataService.GetUnitNode(item) as ParameterNode;
        //                if (parameter != null)
        //                    parameterList.Add(parameter);
        //            }
        //        //string sid = Parameters[Configuration.Get(Setting.IdObjectMarker)];
        //        //int id = string.IsNullOrEmpty(sid) ? 0 : int.Parse(sid);

        //        //UnitNode node =
        //        //    DataService.GetUnitNode(id);

        //        //if (!(node is GraphNode || node is HistogramNode)) return;

        //        //chbxShowMarkers.Visible = node.GetType().Equals(typeof(GraphNode));

        //        var graph_pane = pane[0];

        //        DateTime from_time, to_time;
        //        if (GetCorrectDates(out from_time, out to_time))
        //        {
        //            DataTable graph_content;
        //            CurveList curves = null;
        //            //if (node.GetType().Equals(typeof(GraphNode)))
        //            curves = CreateCurves(parameterList, //node as GraphNode,
        //                                  from_time,
        //                                  to_time,
        //                                  out graph_content);
        //            //else
        //            //    curves = CreateCurves(node as HistogramNode,
        //            //                          from_time,
        //            //                          to_time,
        //            //                          out graph_content);

        //            GridViewGraphData.DataSource = graph_content;
        //            GridViewGraphData.DataBind();

        //            foreach (var it in curves)
        //                graph_pane.CurveList.Add(it);
        //        }

        //        //webObject.Title = node.Text;
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

        //private CurveList CreateCurves(IEnumerable<ParameterNode> parameters,//GraphNode node,
        //                               DateTime from_date,
        //                               DateTime to_date,
        //                               out DataTable value_data)
        //{
        //    CurveList result = new CurveList();
        //    value_data = new DataTable();

        //    DataTable prototype = CreatePrototypeTable();

        //    DataTable temp_table = null;
        //    LineItem temp_line = null;

        //    //result.Conta

        //    foreach (ParameterNode it in parameters)
        //    {
        //        var param_values = GetParameterValues(it.Idnum,
        //                                             from_date,
        //                                             to_date);

        //        temp_table = prototype.Copy();
        //        temp_table.Columns.Add(it.Text, typeof(double));

        //        FillTable(temp_table, param_values);

        //        if (tempCurveList.TryGetValue(it.Idnum, out temp_line))
        //        {
        //            temp_line.Clear();
        //            var points = ToPointList(param_values);
        //            for (int i = 0; i < points.Count; i++)
        //            {
        //                temp_line.AddPoint(points[i]);
        //            }
        //        }
        //        else
        //        {
        //            temp_line = new LineItem(it.Text,
        //                                     ToPointList(param_values),
        //                                     GetColor(it.LineColor),
        //                                     SymbolType.Default, 1);//(SymbolType)it.LineSymbol,
        //            tempCurveList[it.Idnum] = temp_line;

        //            //(float)it.LineWidth);

        //            temp_line.Line.Style = System.Drawing.Drawing2D.DashStyle.Solid; //(System.Drawing.Drawing2D.DashStyle)it.LineStyle;
        //            temp_line.Tag = it.Idnum;
        //            temp_line.Link = new Link("", "", "");
        //        }
        //        temp_line.Symbol.IsVisible = chbxShowMarkers.Checked;
        //        result.Add(temp_line);

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

        //private IEnumerable<ParamValueItem> GetParameterValues(int id_parametr,
        //                                                         DateTime from_date,
        //                                                         DateTime to_date)
        //{

        //    return DataService.GetValues(id_parametr,
        //                                 from_date,
        //                                 to_date,
        //                                 chbxUseBlock.Checked);
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

        //private static Color GetColor(Color seted_color)
        //{
        //    return ColorIsEmpty(seted_color) ? // seted_color == Color.Empty ?
        //            GenerateColor() :
        //            seted_color;
        //}

        //private static bool ColorIsEmpty(Color color)
        //{
        //    return color.A == 0 && color.R == 0 && color.G == 0 && color.B == 0;
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

        ////private CurveList CreateCurves(HistogramNode node,
        ////                               DateTime from,
        ////                               DateTime to,
        ////                               out DataTable value_data)
        ////{
        ////    CurveList result = new CurveList();
        ////    value_data = new DataTable();

        ////    DataTable prototype = CreatePrototypeTable();

        ////    DataTable temp_table = null;
        ////    BarItem temp_bar = null;
        ////    foreach (HistogramParamNode it in node.Parameters)
        ////    {
        ////        var param_values = GetParameterValues(it.ParameterId,
        ////                                             from,
        ////                                             to,
        ////                                             it.Aggregation);

        ////        temp_table = prototype.Copy();
        ////        temp_table.Columns.Add(it.Text, typeof(double));

        ////        FillTable(temp_table, param_values);

        ////        temp_bar = new BarItem(it.Text,
        ////                               ToPointList(param_values),
        ////                               GetColor(it.LineColor));

        ////        temp_bar.Tag = it.Idnum;

        ////        result.Add(temp_bar);

        ////        value_data.Merge(temp_table, false, MissingSchemaAction.Add);
        ////    }

        ////    return result;
        ////}

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

        //        if (DropDownListInterval.SelectedIndex == (int)WebClient.Graphic.IntervalMarker.Custom)
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

        //private TimeSpan GetTimeShiftValue()
        //{
        //    switch ((WebClient.Graphic.IntervalMarker)DropDownListInterval.SelectedIndex)
        //    {
        //        case WebClient.Graphic.IntervalMarker.HalfAnHour:
        //            return TimeSpan.FromMinutes(30f);
        //        case WebClient.Graphic.IntervalMarker.OneHour:
        //            return TimeSpan.FromHours(1f);
        //        case WebClient.Graphic.IntervalMarker.FourHours:
        //            return TimeSpan.FromHours(4f);
        //        case WebClient.Graphic.IntervalMarker.EightHours:
        //            return TimeSpan.FromHours(8f);
        //        case WebClient.Graphic.IntervalMarker.Day:
        //            return TimeSpan.FromDays(1f);
        //        case WebClient.Graphic.IntervalMarker.Month:
        //            return TimeSpan.FromDays(30f);
        //        default:
        //            {
        //                DateTime to, from;
        //                return GetCorrectDates(out from, out to) ? to - from : TimeSpan.FromTicks(0);
        //            }
        //    }
        //}

        //public void CorrectDates(bool add)
        //{
        //    TimeSpan shift = GetTimeShiftValue();

        //    UpdateDate(FromDate, FromTime, shift, add);

        //    if (DropDownListInterval.SelectedIndex == (int)WebClient.Graphic.IntervalMarker.Custom)
        //        UpdateDate(ToDate, ToTime, shift, add);
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

        //protected void DrawButton_Click(object sender, EventArgs e)
        //{
        //    // this handler must be empty
        //    // it is need for graph refresh 
        //}

        //protected void ClearButton_Click(object sender, EventArgs e)
        //{
        //    parameterIds.Clear();
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

        //protected void DropDownListInterval_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    SetCustomMode(DropDownListInterval.SelectedIndex == (int)WebClient.Graphic.IntervalMarker.Custom);
        //}
    }
}
