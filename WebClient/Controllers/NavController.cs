using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.WebClient.Models;
using COTES.ISTOK.WebClient.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace COTES.ISTOK.WebClient.Controllers
{
    public class NavController : Controller
    {
        //IGlobalQueryManager client = null;

        //
        // GET: /Nav/

        public NavController()
        {
            //portalHost = new ServerClient();
        }

        public ActionResult Tree(SessionKeeper sKeeper, int pid)
        {
            List<TreeItem> treeLinks = new List<TreeItem>();
            ViewData[Properties.Resources.strPageId] = pid;
            try
            {
                var pars = GetParameters(sKeeper, pid, 0);
                foreach (var item in pars)
                {
                    treeLinks.Add(item);
                }
            }
            catch(UserNotConnectedException)
            {
                return Redirect("/");
            }

            return View(treeLinks);
        }

        public ActionResult Menu(SessionKeeper sKeeper, int pid)
        {
            List<NavLink> menuLinks = new List<NavLink>();
            if (sKeeper.Session.User != null)
            {
                StructureProvider prov = sKeeper.GetStrucProvider(pid);
                menuLinks.Add(new NavLink()
                {
                    Text = "Все элементы",
                    IsSelected = prov.FilterParams == FilterParams.All,
                    RouteValues = new RouteValueDictionary(new
                    {
                        controller = "Home",
                        action = "Index"
                    })
                });
                menuLinks.Add(new NavLink()
                {
                    Text = "Параметры",
                    IsSelected = prov.FilterParams == FilterParams.TepParameters || prov.FilterParams == FilterParams.ManualParameters,
                    RouteValues = new RouteValueDictionary(new
                    {
                        controller = "Home",
                        action = "Index",
                        filter = (int)FilterParams.TepParameters
                    })
                });
                menuLinks.Add(new NavLink()
                {
                    Text = "Графики",
                    IsSelected = prov.FilterParams == FilterParams.Graphs,
                    RouteValues = new RouteValueDictionary(new
                    {
                        controller = "Home",
                        action = "Index",
                        filter = (int)FilterParams.Graphs
                    })
                });
                menuLinks.Add(new NavLink()
                {
                    Text = "Норм. графики",
                    IsSelected = prov.FilterParams == FilterParams.NormFunctions,
                    RouteValues = new RouteValueDictionary(new
                    {
                        controller = "Home",
                        action = "Index",
                        filter = (int)FilterParams.NormFunctions
                    })
                });
                menuLinks.Add(new NavLink()
                {
                    Text = "Мнемосхемы",
                    IsSelected = prov.FilterParams == FilterParams.Schemas,
                    RouteValues = new RouteValueDictionary(new
                    {
                        controller = "Home",
                        action = "Index",
                        filter = (int)FilterParams.Schemas
                    })
                });
                menuLinks.Add(new NavLink()
                {
                    Text = "Отчеты",
                    IsSelected = prov.FilterParams == FilterParams.Reports,
                    RouteValues = new RouteValueDictionary(new
                    {
                        controller = "Home",
                        action = "Index",
                        filter = (int)FilterParams.Reports
                    })
                });
            }

            return View(menuLinks);
        }

        [HttpPost]
        public string GetParams(SessionKeeper sKeeper, int pid, int id)
        {
            string res = "";

            try
            {
                var nodes = GetParameters(sKeeper, pid, id);

                var sb = new StringBuilder();
                sb.Append("[");
                foreach (var link in nodes)
                {
                    if (nodes.First() != link) { sb.Append(","); }
                    sb.Append("{");
                    if (!link.IsLeaf) { sb.Append("\"state\":\"closed\","); }
                    sb.Append("\"data\":\"" + link.Text + "\",");
                    sb.Append("\"attr\":{\"id\": \"li" + link.id.ToString() + "\", \"rel\":\"type" + link.type + "\", \"title\":\"" + link.Text + "\"}");
                    sb.Append("}");
                }
                sb.Append("]");
                //@Html.Raw(sb.ToString());
                res = sb.ToString();
                res = res.Replace("\\", "\\\\");
                res = res.Replace("&quot;", "\\\"");
                //using (StringWriter sw = new StringWriter())
                //{
                //    ViewEngineResult vr = ViewEngines.Engines.FindPartialView(this.ControllerContext, "GetParams");
                //    ViewContext viewContext = new ViewContext(this.ControllerContext, vr.View, this.ViewData, this.TempData, sw);
                //    viewContext.ViewData.Model = nodes;
                //    vr.View.Render(viewContext, sw);
                //    res = sw.GetStringBuilder().ToString();
                //    res = res.Replace("\\", "\\\\");
                //    res = res.Replace("&quot;", "\\\"");
                //}
            }
            catch
            {
                //
            }

            return res;
        }

        [HttpPost]
        public void SetActiveTab(SessionKeeper sKeeper, int pid, int tab)
        {
            try
            {
                StructureProvider prov = sKeeper.GetStrucProvider(pid);
                prov.Data[Resources.strActiveTab] = tab;
            }
            catch (UserNotConnectedException)
            {
                //
            }
        }

        [HttpPost]
        public void SetParameter(SessionKeeper sKeeper, int pid, int id)
        {
            try
            {
                StructureProvider prov = sKeeper.GetStrucProvider(pid);
                prov.CurrentNode = prov.GetUnitNode(id);
                prov.CurrentUnitProvider = prov.GetUnitProvider(prov.CurrentNode);
            }
            catch(UserNotConnectedException)
            {
                //
            }
        }

        [HttpPost]
        public ActionResult GetParameterView(SessionKeeper sKeeper, int pid)
        {
            try
            {
                StructureProvider prov = sKeeper.GetStrucProvider(pid);
                if (prov.CurrentUnitProvider == null) return null;
                ViewData[Properties.Resources.strPageId] = pid;
                if (prov.Data.ContainsKey(Resources.strActiveTab))
                    ViewData[Resources.strActiveTab] = prov.Data[Resources.strActiveTab];
                return View(prov.CurrentUnitProvider);
            }
            catch (UserNotConnectedException)
            {
                return Redirect("/");
            }
            return null;
        }

        #region Private Methods
        private IEnumerable<TreeItem> GetParameters(SessionKeeper sKeeper, int pid, int parentId)
        {
            List<TreeItem> res = new List<TreeItem>();
            TreeItem ti;
            var tmp = sKeeper.GetStrucProvider(pid).GetUnitNodesFiltered(parentId);

            foreach (var item in tmp)
            {
                ti = new TreeItem()
                {
                    Text = string.IsNullOrEmpty(item.DocIndex) ? item.Text : item.DocIndex + " " + item.Text,
                    id = item.Idnum,
                    IsLeaf = item.NodesIds.Length == 0,
                    type = (int)item.Typ
                };
                res.Add(ti);
            }
            
            return res;
        }

        private UnitNode GetParameter(SessionKeeper sKeeper, int pid, int paramId)
        {
            return sKeeper.GetStrucProvider(pid).GetUnitNode(paramId);
        }
        #endregion
    }

    public class NavLink
    {
        public string Text { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
        public object Route { get; set; }
        public bool IsSelected { get; set; }
    }

    public class TreeItem : NavLink
    {
        public List<TreeItem> items;
        public int id;
        public int type;
        public bool IsLeaf { get; set; }
        public bool isPlaceholder;

        public TreeItem()
        {
            items = new List<TreeItem>();
        }
    }
}
