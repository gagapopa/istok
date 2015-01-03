using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using COTES.ISTOK.ASC;
using System.Data;
using ZedGraph;
using ZedGraph.Web;
using System.Drawing;

namespace WebClient
{
    public partial class NormFunc : TreeContentPage
    {
        NormFuncNode normFuncNode;
        MultiDimensionalTable mdtable;
        double[] coordinates;

        NormFuncGraphParam gr_param;
        Color[] norm_colors = { Color.Blue, Color.Orange, Color.Green,
            Color.Cyan, Color.Red, Color.Brown,Color.Gold,Color.Black };
        double cnt_x = 1, cnt_y = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            SetSpecificTab(true);
        }

        protected void Graph_RenderGraph(ZedGraphWeb webObject,
                                         System.Drawing.Graphics g,
                                         ZedGraph.MasterPane pane)
        {
            try
            {
                string sid = Parameters[Configuration.Get(Setting.IdObjectMarker)];
                int id = string.IsNullOrEmpty(sid) ? 0 : int.Parse(sid);

                UnitNode node =
                    DataService.GetUnitNode(id);

                normFuncNode = node as NormFuncNode;
                if (normFuncNode == null) return;
                
                //mdtable = normFuncNode.MDTable;
                mdtable = normFuncNode.GetMDTable(COTES.ISTOK.RevisionInfo.Default);
                gr_param = new NormFuncGraphParam();
                if (mdtable.DimensionInfo.Length > 1)
                    gr_param.strAxeZ = mdtable.DimensionInfo[1].Name;
                coordinates = new double[] { };// { mdtable.GetDimension()[0] };

                var graph_pane = pane[0];

                Draw(webObject, pane);
                GridViewData.ShowHeader = false;
                GridViewData.DataSource = mdtable.GetTable(coordinates);
                GridViewData.DataBind();
                //DateTime from_time, to_time;
                //if (DatesIsCorrect(out from_time, out to_time))
                //{
                //    DataTable graph_content;
                //    CurveList curves = null;
                //    if (node.GetType().Equals(typeof(GraphNode)))
                //        curves = CreateCurves(node as GraphNode,
                //                              from_time,
                //                              to_time,
                //                              out graph_content);
                //    else
                //        curves = CreateCurves(node as HistogramNode,
                //                              from_time,
                //                              to_time,
                //                              out graph_content);

                //    GridViewGraphData.DataSource = graph_content;
                //    GridViewGraphData.DataBind();

                //    foreach (var it in curves)
                //        graph_pane.CurveList.Add(it);
                //}

                //webObject.Title = node.Text;
                //pane.AxisChange(g);
            }
            catch (BaseWebClientException exp)
            {
                exp.Accept(Handler.Instance);
            }
            catch (Exception exp)
            {
                Handler.Instance.ProcessUndefined(exp);
            }
        }

        public static void Clear(ZedGraphWeb graph, GraphPane pane)
        {
            pane.CurveList.Clear();
            //graph.AxisChange();
            //graph.Refresh();
        }

        public void Draw(ZedGraphWeb webObject, MasterPane mpane)
        {
            GraphPane pane = mpane.PaneList[0];
            int j;
            Color color;
            Random rand = new Random();
            LineItem mySerP;

            if (webObject == null)
                return;

            Clear(webObject, pane);

            try
            {
                pane.Title.Text = normFuncNode.Text;
                pane.Title.FontSpec.Size = 12;
                //webObject.IsShowPointValues = true;
                pane.XAxis.MajorGrid.IsVisible = true;
                pane.XAxis.MajorGrid.Color = System.Drawing.Color.FromArgb(227, 203, 157);    //Violet;
                // Задаем вид пунктирной линии для крупных рисок по оси X:
                pane.XAxis.MajorGrid.DashOn = 20;
                // пропуск
                pane.XAxis.MajorGrid.DashOff = 0;
                // Включаем отображение сетки напротив крупных рисок по оси Y
                pane.YAxis.MajorGrid.IsVisible = true;
                pane.YAxis.MajorGrid.Color = System.Drawing.Color.FromArgb(227, 203, 157);
                // Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
                pane.YAxis.MajorGrid.DashOn = 10;
                pane.YAxis.MajorGrid.DashOff = 0;
                pane.YAxis.MinorGrid.Color = System.Drawing.Color.FromArgb(208, 207, 176);
                // Включаем отображение сетки напротив мелких рисок по оси X
                pane.YAxis.MinorGrid.IsVisible = true;
                // Задаем вид пунктирной линии для крупных рисок по оси Y: 
                pane.YAxis.MinorGrid.DashOn = 10;
                // пропуск
                pane.YAxis.MinorGrid.DashOff = 0;
                pane.XAxis.MinorGrid.Color = System.Drawing.Color.FromArgb(208, 207, 176);
                // Включаем отображение сетки напротив мелких рисок по оси Y
                pane.XAxis.MinorGrid.IsVisible = true;
                // Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
                pane.XAxis.MinorGrid.DashOn = 10;
                pane.XAxis.MinorGrid.DashOff = 0;

                double[] res_coords = new double[mdtable.DimensionInfo.Length];
                double[] coords = new double[mdtable.DimensionInfo.Length - 1];
                double[] coords_y;
                double res;

                Array.Copy(coordinates, coords, coordinates.Length);
                if (mdtable.DimensionInfo.Length - coordinates.Length < 3)
                {
                    if (mdtable.DimensionInfo.Length > 1)
                        Array.Copy(coordinates, res_coords, mdtable.DimensionInfo.Length - 2);
                    Array.Reverse(res_coords);
                    if (mdtable.DimensionInfo.Length - coordinates.Length == 2)
                    {
                        coords_y = mdtable.GetDimension(coordinates);
                        coords_y = FillApprox(coords_y, cnt_y);
                    }
                    else
                    {
                        coords_y = new double[1];
                        if (coordinates.Length > 0)
                            coords_y[0] = coordinates[coordinates.Length - 1];
                        else
                            coords_y[0] = double.NaN;
                    }

                    pane.Legend.IsVisible = coords_y.Length > 1;

                    for (j = 0; j < coords_y.Length; j++)
                    {
                        double[] coords_x;
                        double item = coords_y[j];

                        if (!double.IsNaN(item)) coords[coords.Length - 1] = item;
                        coords_x = mdtable.GetDimension(COTES.ISTOK.CommonData.Approx.Linear, coords);
                        coords_x = FillApprox(coords_x, cnt_x);

                        PointPairList list = new PointPairList();

                        foreach (var item2 in coords_x)
                        {
                            res_coords[0] = item2;
                            if (!double.IsNaN(item)) res_coords[1] = item;
                            res = mdtable.GetValue(res_coords);

                            list.Add(item2, res, res.ToString());
                        }

                        if (j < norm_colors.Length)
                            color = norm_colors[j];
                        else
                        {
                            List<Color> lstColors = new List<Color>(norm_colors);
                            color = Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255));
                            lstColors.Add(color);
                            norm_colors = lstColors.ToArray();
                        }

                        mySerP = pane.AddCurve(gr_param.strAxeZ + " = " + item.ToString(), list, color, SymbolType.Circle);
                        mySerP.Link = new Link("", "", "");
                        mySerP.Symbol.IsVisible = chbxShowMarkers.Checked;
                        //mySerP.Symbol.Fill.Type = FillType.Solid;
                        //mySerP.Symbol.Size = 5;
                        mySerP.Line.Width = 2;
                    }
                }

                //webObject.AxisChange();

                //graph.Refresh();
                //graph.Size = new Size(graph.Width, graph.Height);
                //graph.PerformAutoScale();
            }
            catch (FormatException)
            {
                throw new FormatException("Некорректный формат числа");
            }
            catch (BaseWebClientException exp)
            {
                exp.Accept(Handler.Instance);
            }
            catch (Exception exp)
            {
                Handler.Instance.ProcessUndefined(exp);
            }
        }

        private double[] FillApprox(double[] coordinates, double coef)
        {
            List<double> lstResult = new List<double>();
            double[] coords = coordinates;
            double lastval;
            double delta;

            if (coords == null || coords.Length < 2) return coords;

            if (coef == 0f) throw new ArgumentException("coef cannot be zero.");

            lstResult.Add(coords[0]);
            for (int i = 1; i < coords.Length; i++)
            {
                lastval = lstResult[lstResult.Count - 1];
                delta = (coords[i] - coords[i - 1]) * coef;

                for (double j = lastval + delta; j <= coords[i]; j += delta)
                    lstResult.Add(j);
            }
            if (lstResult[lstResult.Count - 1] < coords[coords.Length - 1])
                lstResult.Add(coords[coords.Length - 1]);

            return lstResult.ToArray();
        }
    }

    class NormFuncGraphParam
    {
        public string tablename;
        public double[] coordinates;
        public string strCaption;
        public string strAxeX;
        public string strAxeY;
        public string strAxeZ;
    }
}
