using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.ClientCore.UnitProviders;
using COTES.ISTOK.WebClient.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace COTES.ISTOK.WebClient.Controllers.UnitControllers
{
    public class SchemaUnitController : Controller
    {
        [HttpPost]
        public Task<ActionResult> GetContent(SessionKeeper sKeeper, int pid)
        {
            return Task.Factory.StartNew<ActionResult>(() =>
            {
                SchemaUnitProvider prov = sKeeper.GetStrucProvider(pid).CurrentUnitProvider as SchemaUnitProvider;
                prov.RegisterParameters();
                ViewData[Properties.Resources.strPageId] = pid;
                return View(prov);
            });
        }

        public ActionResult GetSchema(SessionKeeper sKeeper, int pid)
        {
            try
            {
                SchemaNode snode = sKeeper.GetStrucProvider(pid).CurrentUnitProvider.UnitNode as SchemaNode;

                //var ms = new MemoryStream(snode.ImageBuffer);

                return new FileContentResult(snode.ImageBuffer, "image/png");
            }
            catch (Exception)
            {
            }
            return null;
        }

        [HttpPost]
        public ActionResult GetGraph(SessionKeeper sKeeper, int pid, int parameterId)
        {
            StructureProvider prov = sKeeper.GetStrucProvider(pid);
            var node = prov.GetUnitNode(parameterId) as ParameterNode;
            if (node != null)
            {
                GraphNode gnode = null;
                if (prov.Data.ContainsKey("gnode"))
                    gnode = prov.Data["gnode"] as GraphNode;
                if (gnode == null)
                    gnode = new GraphNode();
                gnode.Text = node.Text;
                gnode.UpdateInterval = ((SchemaNode)prov.CurrentNode).UpdateInterval;
                bool found = false;
                foreach (var item in gnode.Parameters)
                {
                    if (item.ParameterId == node.Idnum)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) gnode.AddChildParam(node);
                prov.Data["gnode"] = gnode;
                var gprov = prov.GetUnitProvider(gnode) as GraphUnitProvider;
                                
                if (gprov != null)
                {
                    GraphUnitProviderState state = gprov.GetState(null);
                    state.GraphPeriod = GraphTimePeriod.User;
                    state.GraphTo = DateTime.Now;
                    state.GraphFrom = state.GraphTo.AddMinutes(-5);//так в толстом вроде было

                    prov.Data["sunode"] = prov.CurrentNode;
                    prov.Data["suprov"] = prov.CurrentUnitProvider;
                    prov.CurrentUnitProvider = gprov;
                    prov.CurrentNode = gnode;

                    return RedirectToAction("GetContent", "GraphUnit", new { pid = pid });
                }
            }
            
            return null;
        }

        [HttpPost]
        public void CloseGraph(SessionKeeper sKeeper, int pid)
        {
            StructureProvider prov = sKeeper.GetStrucProvider(pid);
            prov.CurrentNode = prov.Data["sunode"] as SchemaNode;
            prov.CurrentUnitProvider = prov.Data["suprov"] as SchemaUnitProvider;
        }

        [HttpPost]
        public ActionResult GetValues(SessionKeeper sKeeper, int pid)
        {
            StructureProvider prov = sKeeper.GetStrucProvider(pid);
            if (prov.CurrentUnitProvider == null || !(prov.CurrentUnitProvider is SchemaUnitProvider))
            {
                //походу зомби-ajax лезет до того, как страница загрузится
                return null;
            }
            //открыт график, юнитпровайдер подменен
            if (!(prov.CurrentUnitProvider is SchemaUnitProvider)) return null;
            
            SchemaUnitProvider schprov = prov.CurrentUnitProvider as SchemaUnitProvider;

            var x = Json(ParameterStyleGenerator.GetParams(((SchemaNode)schprov.UnitNode).Parameters, schprov.ParamValues));
            return x;
        }
    }
}
