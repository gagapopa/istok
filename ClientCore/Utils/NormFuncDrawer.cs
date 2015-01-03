using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using ZedGraph;

namespace COTES.ISTOK.ClientCore.Utils
{
    public class NormFuncDrawer
    {
        MultiDimensionalTable mdtable;
        double[] coordinates;

        NormFuncGraphParam gr_param;
        Color[] norm_colors = { Color.Blue, Color.Orange, Color.Green,
            Color.Cyan, Color.Red, Color.Brown,Color.Gold,Color.Black };

        private CommonData.Approx approx;
        private double cnt_x = 1, cnt_y = 1;

        public NormFuncDrawer(NormFuncGraphParam param)
        {
            gr_param = param;
            mdtable = gr_param.mdtable;
            coordinates = gr_param.coordinates;
            approx = gr_param.approx;

            cnt_x = gr_param.delta_x;
            cnt_y = gr_param.delta_y;
        }

        public static void Clear(GraphPane pane)
        {
            pane.CurveList.Clear();
            pane.AxisChange();
            //graph.Refresh();
        }

        public void Draw(GraphPane pane)
        {
            int j;
            Color color;
            Random rand = new Random();
            LineItem mySerP;

            if (pane == null) return;

            Clear(pane);

            try
            {
                //pane.Title.Text = gr_param.strCaption;
                pane.Title.FontSpec.Size = 12;
                //pane.XAxis.Title.Text = gr_param.strAxeX;
                //pane.YAxis.Title.Text = gr_param.strAxeY;
                
                //pane.IsShowPointValues = true;
                
                //// Градиентная заливка графика
                //pane.Chart.Fill = new Fill(Color.Red, Color.White, 400F);
                //// Градиентная заливка коймы графика
                //pane.Fill = new Fill(Color.White, Color.Gray, 400F);
                
                // Включаем отображение сетки напротив крупных рисок по оси X
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


                if (mdtable.DimensionInfo.Length == 0) return;

                double[] res_coords = new double[mdtable.DimensionInfo.Length];
                double[] coords = new double[mdtable.DimensionInfo.Length - 1];
                double[] coords_y;
                double res;

                Array.Copy(coordinates, coords, coordinates.Length);
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
                    coords_x = mdtable.GetDimension(CommonData.Approx.Linear, coords);
                    coords_x = FillApprox(coords_x, cnt_x);

                    PointPairList list = new PointPairList();

                    foreach (var item2 in coords_x)
                    {
                        res_coords[0] = item2;
                        if (!double.IsNaN(item)) res_coords[1] = item;
                        res = mdtable.GetValue(res_coords);

                        list.Add(item2, res);
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
                    mySerP.Symbol.Fill.Type = FillType.Solid;
                    mySerP.Symbol.Size = 5;
                    mySerP.Line.Width = 2;
                }

                pane.AxisChange();

                //pane.Refresh();
                //pane.Size = new Size(pane.Width, pane.Height);
                //pane.PerformAutoScale();

                return;
            }
            catch (FormatException)
            {
                throw new FormatException("Некорректный формат числа");
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

    public class NormFuncGraphParam
    {
        public string tablename = String.Empty;
        //public string funcid;

        public double delta_x = .0, delta_y = .0;

        public CommonData.Approx approx = CommonData.Approx.None;

        public DataSet dataset = null;
        public double[] coordinates = null;
        public MultiDimensionalTable mdtable = null;

        public string strCaption = String.Empty;
        public string strAxeX = String.Empty;
        public string strAxeY = String.Empty;
        public string strAxeZ = String.Empty;
    }
}
