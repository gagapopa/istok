using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.WebClient.Models;
using COTES.ISTOK.WebClient.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace COTES.ISTOK.WebClient.Controllers
{
    public class HomeController : Controller
    {
        //IGlobalQueryManager client = null;

        public HomeController()
        {
            
        }

        public ActionResult Error(Exception ex)
        {
            return View(ex);
        }

        public ActionResult Index(SessionKeeper sKeeper, int filter = 0)
        {
            try
            {
                int id = sKeeper.GenerateId();
                ViewData[Resources.strPageId] = id;
                if (sKeeper.Session.Uid == Guid.Empty)
                    return Redirect("/Home/Login");
                StructureProvider strucProvider = sKeeper.GetStrucProvider(id);
                FilterParams f = (FilterParams)filter;
                strucProvider.FilterParams = f;
            }
            catch (CommunicationObjectFaultedException)
            {
                Session.Clear();
                return Redirect("/Home/Error");
            }

            return View();
        }

        [HttpPost]
        public void SetRevision(SessionKeeper sKeeper, int pid, int revId)
        {
            try
            {
                foreach (var item in sKeeper.Session.Revisions)
                {
                    if (item.ID == revId)
                    {
                        sKeeper.GetStrucProvider(pid).CurrentRevision = item;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                //
            }
        }

        //public ActionResult Filter(SessionKeeper sKeeper, int filter)
        //{
        //    FilterParams f = (FilterParams)filter;
        //    int pid = sKeeper.GenerateId();
        //    StructureProvider strucProvider = sKeeper.GetStrucProvider(pid);
        //    //sKeeper.CurrentNode = null;
        //    //sKeeper.CurrentUnitProvider = null;
        //    strucProvider.FilterParams = f;
        //    return Redirect("/");
        //}

        [HttpPost]
        public int CheckLogin(SessionKeeper sKeeper, string user, string pass)
        {
            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
            {
                try
                {
                    sKeeper.Session.Connect(user, pass);
                    PropertyFormatter.rds = new COTES.ISTOK.WebClient.Models.RemoteDataService(sKeeper);
                    sKeeper.UpdateTypes(Server.MapPath("~/Content/Images"));
                    sKeeper.UpdateRevisions(null);
                    return 0;
                }
                catch (UserNotConnectedException)
                {
                    return 1;
                }
                catch (Exception)
                {
                    return 2;
                    //return new RedirectResult("/Home/Error");
                }
            }

            return 1;
        }

        public ActionResult Login(SessionKeeper sKeeper)
        {
            return View();
        }
        public RedirectResult Logout(SessionKeeper sKeeper)
        {
            RedirectResult res = new RedirectResult("/");

            sKeeper.Session.Disconnect();
            Session.Clear();
            //sKeeper = new SessionKeeper(Session);
            sKeeper = null;
            return res;
        }
    }
}
