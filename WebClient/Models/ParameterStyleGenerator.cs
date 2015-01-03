using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.WebPages.Html;

namespace COTES.ISTOK.WebClient.Models
{
    public static class ParameterStyleGenerator
    {
        public static string GetStyle(ChildParamNode param)
        {
            StringBuilder sb = new StringBuilder();
            
            int w, h, l, t;
            string st;
            foreach (var item in param.Attributes.Keys)
            {
                switch (item.ToLower())
                {
                    case "width":
                        break;
                }
                //int.TryParse(item.Attributes["width"], out w);
                //int.TryParse(item.Attributes["height"], out h);
                //int.TryParse(item.Attributes["left"], out l);
                //int.TryParse(item.Attributes["top"], out t);
                //var bgcolor = string.Format("{0}", System.Drawing.Color.Green.Name);
                //st = string.Format("width:{0}px;height:{1}px;left:{2}px;top:{3}px;background-color:{4};position:absolute;overflow:hidden;font-size:x-small;", w, h, l, t,bgcolor);
            }

            sb.Append("<div id=\"rev\">");
            sb.Append("Рабочая ревизия: ");
            
            sb.Append("</div>");
            return sb.ToString();
        }

        public static object GetParams(ChildParamNode[] parameters, ParamValueItemWithID[] values)
        {
            string attr;
            double minW, maxW, minA, maxA;
            double v;
            
            if (parameters == null || values == null) return null;

            var pars = new List<ChildParamNode>(parameters);
            var vals = new List<ParamValueItemWithID>(values);

            for (int i = 0; i < vals.Count; i++)
            {
                if (double.IsNaN(vals[i].Value))
                {
                    vals.RemoveAt(i);
                    pars.RemoveAt(i);
                    i--;
                }
            }

            ColoredParamValue[] arrRes = new ColoredParamValue[vals.Count];

            for (int i = 0; i < arrRes.Length; i++)
            {
                arrRes[i] = new ColoredParamValue(vals[i]);
                minW = double.NaN;
                minA = double.NaN;
                maxW = double.NaN;
                maxA = double.NaN;

                foreach (var item in pars)
                {
                    if (item.ParameterId == arrRes[i].ParameterID)
                    {
                        attr = "NominalColor";
                        if (item.Attributes.ContainsKey(attr))
                            arrRes[i].Color = FormatColor(item.Attributes[attr]);                                

                        attr = "minAlertValue";
                        if (item.Attributes.ContainsKey(attr))
                            if (double.TryParse(item.Attributes[attr], out v))
                                minA = v;
                        attr = "maxAlertValue";
                        if (item.Attributes.ContainsKey(attr))
                            if (double.TryParse(item.Attributes[attr], out v))
                                maxA = v;
                        attr = "minWarningValue";
                        if (item.Attributes.ContainsKey(attr))
                            if (double.TryParse(item.Attributes[attr], out v))
                                minW = v;
                        attr = "maxWarningValue";
                        if (item.Attributes.ContainsKey(attr))
                            if (double.TryParse(item.Attributes[attr], out v))
                                maxW = v;

                        attr = "maxWarningColor";
                        if (!double.IsNaN(maxW) && arrRes[i].Value >= maxW)
                            arrRes[i].Color = item.Attributes.ContainsKey(attr) ? FormatColor(item.Attributes[attr]) : FormatColor(0);
                        attr = "maxAlertColor";
                        if (!double.IsNaN(maxA) && arrRes[i].Value >= maxA)
                            arrRes[i].Color = item.Attributes.ContainsKey(attr) ? FormatColor(item.Attributes[attr]) : FormatColor(0);

                        attr = "minWarningColor";
                        if (!double.IsNaN(minW) && arrRes[i].Value <= minW)
                            arrRes[i].Color = item.Attributes.ContainsKey(attr) ? FormatColor(item.Attributes[attr]) : FormatColor(0);
                        attr = "minAlertColor";
                        if (!double.IsNaN(minA) && arrRes[i].Value <= minA)
                            arrRes[i].Color = item.Attributes.ContainsKey(attr) ? FormatColor(item.Attributes[attr]) : FormatColor(0);

                        break;
                    }
                }
            }

            return arrRes;
        }

        public static string FormatColor(string argb)
        {
            int v;
            int.TryParse(argb, out v);
            return FormatColor(v);
        }
        public static string FormatColor(int argb)
        {
            Color c = Color.FromArgb(argb);
            return string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);
        }
        public static string FormatColor(Color color)
        {
            return FormatColor(color.ToArgb());
        }
    }

    public class ColoredParamValue : ParamValueItemWithID
    {
        public string Color { get; set; }
        public string FormattedTime
        {
            get { return Time.ToString("dd.MM.yyyy HH:mm:ss"); }
        }

        public ColoredParamValue()
        {
            Color = "#FFFFFF";
        }
        public ColoredParamValue(ParamValueItemWithID param)
            : this()
        {
            this.Arguments = param.Arguments;
            this.ChangeTime = param.ChangeTime;
            this.ChannelID = param.ChannelID;
            this.ParameterID = param.ParameterID;
            this.Quality = param.Quality;
            this.Time = param.Time;
            this.Value = param.Value;
        }
    }
}