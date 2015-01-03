using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore.UnitProviders;
using COTES.ISTOK.ClientCore.Utils;
using COTES.ISTOK.WebClient.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZedGraph;
using ZedGraph.Web;

namespace COTES.ISTOK.WebClient.Controllers.UnitControllers
{
    public class NormFuncUnitController : Controller
    {
        [HttpPost]
        public ActionResult GetContent(SessionKeeper sKeeper, int pid)
        {
            ViewData[Properties.Resources.strPageId] = pid;
            return View(sKeeper.GetStrucProvider(pid).CurrentUnitProvider);
        }

        public ActionResult GetGraph(SessionKeeper sKeeper, int pid)
        {
            NormFuncUnitProvider prov = sKeeper.GetStrucProvider(pid).CurrentUnitProvider as NormFuncUnitProvider;

            var web = new ZedGraphWeb();
            web.Height = 450;
            web.Width = 800;
            web.RenderGraph += (w, g, masterPane) =>
            {
                GraphPane myPane = masterPane[0];
                prov.FillPane(myPane);
            };
            var ms = new MemoryStream();
            web.CreateGraph(ms, ImageFormat.Png);
            return new FileContentResult(ms.ToArray(), "image/png");
        }

        [HttpPost]
        public string Calc(SessionKeeper sKeeper, int pid, string[] props, string[] values)
        {
            MultiDimensionalTable mdt = (sKeeper.GetStrucProvider(pid).CurrentNode as NormFuncNode).GetMDTable(sKeeper.GetStrucProvider(pid).CurrentRevision);
            double[] coords = new double[mdt.DimensionInfo.Length];
            double tmp;
            int m;

            if (props == null || values == null) return "NaN";

            m = Math.Min(props.Length, values.Length);
            for (int i = 0; i < mdt.DimensionInfo.Length; i++)
            {
                for (int p = 0; p < m; p++)
                {
                    if (props[p].ToString() == mdt.DimensionInfo[i].Name)
                    {
                        double.TryParse(values[p].ToString(), out tmp);
                        coords[i] = tmp;
                        break;
                    }
                }                
            }

            coords = coords.Reverse().ToArray();
            return mdt.GetValue(coords).ToString(DoubleControlSettings.DoubleFormat(3));
        }
    }
}
