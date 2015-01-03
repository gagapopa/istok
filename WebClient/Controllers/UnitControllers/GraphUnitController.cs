using COTES.ISTOK.ClientCore.UnitProviders;
using COTES.ISTOK.ClientCore.Utils;
using COTES.ISTOK.Extension;
using COTES.ISTOK.WebClient.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZedGraph;
using ZedGraph.Web;

namespace COTES.ISTOK.WebClient.Controllers.UnitControllers
{
    public class GraphUnitController : UnitController
    {
        //[HttpPost]
        public ActionResult GetContent(SessionKeeper sKeeper, int pid)
        {
            ViewData[Properties.Resources.strPageId] = pid;
            return View(sKeeper.GetStrucProvider(pid).CurrentUnitProvider);
        }

        [HttpPost]
        public void QueryData(SessionKeeper sKeeper, int pid, string dateFrom, string dateTo, bool useRemote, bool online)
        {
            GraphUnitProvider prov = sKeeper.GetStrucProvider(pid).CurrentUnitProvider as GraphUnitProvider;
            GraphUnitProviderState state = prov.GetState(null);
            DateTime dFrom, dTo;
            ParseDates(sKeeper, pid, dateFrom, dateTo, out dFrom, out dTo);
            state.RemoteServer = useRemote;
            if (online)
            {
                TimeSpan span = dTo.Subtract(dFrom);
                dTo = DateTime.Now;
                dFrom = dTo.Subtract(span);
            }
            prov.QueryGraphData(null, dFrom, dTo, useRemote);
        }

        public ActionResult GetGraph(SessionKeeper sKeeper, int pid, int curPar, int maxPar)
        {
            GraphUnitProvider prov = sKeeper.GetStrucProvider(pid).CurrentUnitProvider as GraphUnitProvider;
            
            var web = new ZedGraphWeb();
            web.Height = 450;
            web.Width = 800;
            web.RenderGraph += (w, g, masterPane) =>
            {
                GraphPane myPane = masterPane[0];
                myPane.Y2AxisList.Clear();
                myPane.YAxisList.Clear();
                //var xData = new double[] { 1, 2, 3 };
                //var yData = new double[] { 10, 60, 50 };
                //myPane.AddCurve("Allocated", xData, yData, Color.Black);
                prov.FillPane(myPane, curPar, maxPar, GraphUnitProvider.HistogramSorting.None);
            };
            var ms = new MemoryStream();
            web.CreateGraph(ms, ImageFormat.Png);
            return new FileContentResult(ms.ToArray(), "image/png");
        }

        [HttpPost]
        public ActionResult GetGraphData(SessionKeeper sKeeper, int pid, int curPar, int maxPar)
        {
            GraphUnitProvider prov = sKeeper.GetStrucProvider(pid).CurrentUnitProvider as GraphUnitProvider;
            GraphData gd = prov.GetData(curPar, maxPar);

            System.TimeSpan span;
            System.DateTime time;
            //var d = new List<object>();
            var arr = new List<object>();
            var dic = new Dictionary<string, object>();

            for (int i = 0; i < gd.LineInfo.Length; i++)
            {
                arr.Clear();
                foreach (var val in gd.LineData[i].Values)
                {
                    span = new System.TimeSpan(System.DateTime.Parse("1/1/1970").Ticks);
                    time = val.X.Subtract(span);
                    if (double.IsNaN(val.Y))
                        arr.Add(null);
                    else
                        arr.Add(new object[] { (long)(time.Ticks / 10000), val.Y });
                }
                //d.Add(new
                //{
                //    label = gd.LineInfo[i].Caption,
                //    color = ParameterStyleGenerator.FormatColor(gd.LineInfo[i].Color),
                //    data = arr.ToArray(),
                //    yaxis = i + 1
                //});
                dic[gd.LineInfo[i].ParameterId.ToString()] = new
                {
                    label = gd.LineInfo[i].Caption,
                    color = ParameterStyleGenerator.FormatColor(gd.LineInfo[i].Color),
                    data = arr.ToArray(),
                    yaxis = i + 1,
                    parameterId = gd.LineInfo[i].ParameterId
                };
            }

            //var r = Json(d.ToArray(), "application/json");
            var r = Json(dic, "application/json");
            return r;
        }

        [HttpPost]
        public void SetVisibility(SessionKeeper sKeeper, int pid, int id, bool value)
        {
            GraphUnitProvider prov = sKeeper.GetStrucProvider(pid).CurrentUnitProvider as GraphUnitProvider;
            prov.SetParameterVisibility(id, value);
        }

        [HttpPost]
        public ActionResult GetTables(SessionKeeper sKeeper, int pid)
        {
            ViewData[Properties.Resources.strPageId] = pid;
            return View(sKeeper.GetStrucProvider(pid).CurrentUnitProvider);
        }

        [HttpPost]
        public void ChangeOnPage(SessionKeeper sKeeper, int pid, int onPage)
        {
            GraphUnitProvider prov = sKeeper.GetStrucProvider(pid).CurrentUnitProvider as GraphUnitProvider;
            GraphUnitProviderState state = prov.GetState(null);
            state.OnPage = onPage;
        }

        [HttpPost]
        public ActionResult ChangeInterval(SessionKeeper sKeeper, int pid, string dateFrom, int interval)
        {
            string[] res = new string[2];

            DateTime dFrom, dTo;
            ParseDates(sKeeper, pid, dateFrom, "", out dFrom, out dTo);
            dTo = dFrom.AddSeconds(GraphTimePeriodFormatter.FormatInterval((GraphTimePeriod)interval));
            res[0] = dFrom.ToString("dd.MM.yyyy HH:mm");
            res[1] = dTo.ToString("dd.MM.yyyy HH:mm");

            return Json(res);
        }

        [HttpPost]
        public ActionResult GetNextDate(SessionKeeper sKeeper, int pid, string dateFrom, string dateTo, int interval)
        {
            return Json(MoveDate(sKeeper, pid, 1, dateFrom, dateTo, interval));
        }
        [HttpPost]
        public ActionResult GetPrevDate(SessionKeeper sKeeper, int pid, string dateFrom, string dateTo, int interval)
        {
            return Json(MoveDate(sKeeper, pid, -1, dateFrom, dateTo, interval));
        }

        private void ParseDates(SessionKeeper sKeeper, int pid, string datFrom, string datTo, out DateTime dateFrom, out DateTime dateTo)
        {
            GraphUnitProvider prov = sKeeper.GetStrucProvider(pid).CurrentUnitProvider as GraphUnitProvider;
            GraphUnitProviderState state = prov.GetState(null);
            DateTime dFrom, dTo;
            if (!DateTime.TryParse(datFrom, out dFrom)) dFrom = state.GraphFrom;
            if (!DateTime.TryParse(datTo, out dTo)) dTo = state.GraphTo;
            dateFrom = dFrom;
            dateTo = dTo;
        }
        private string[] MoveDate(SessionKeeper sKeeper, int pid, int direction, string dateFrom, string dateTo, int interval)
        {
            DateTime dFrom, dTo;
            double shift;
            double step;
            string[] res = new string[2];

            ParseDates(sKeeper, pid, dateFrom, dateTo, out dFrom, out dTo);

            step = GraphTimePeriodFormatter.FormatInterval((GraphTimePeriod)interval);

            if (step == 0)
                step = Math.Abs(dTo.Subtract(dFrom).TotalSeconds);

            shift = (double)direction * step;
            dFrom = dFrom.AddSeconds(shift);
            if (step != 0)
                dTo = dFrom.AddSeconds(step);
            //if (chbxRequestData.Checked) RequestData();

            res[0] = dFrom.ToString("dd.MM.yyyy HH:mm");
            res[1] = dTo.ToString("dd.MM.yyyy HH:mm");

            return res;
        }
    }
}
