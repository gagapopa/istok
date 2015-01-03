using COTES.ISTOK.ClientCore.UnitProviders;
using COTES.ISTOK.WebClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace COTES.ISTOK.WebClient.Controllers.UnitControllers
{
    public class ParameterGateController : UnitController
    {
        [HttpPost]
        public Task<ActionResult> GetContent(SessionKeeper sKeeper, int pid)
        {
            return Task.Factory.StartNew<ActionResult>(() =>
            {
                ParameterGateUnitProvider prov = sKeeper.GetStrucProvider(pid).CurrentUnitProvider as ParameterGateUnitProvider;
                if (prov != null)
                {
                    prov.UseMimeTexGenerator = false;
                    prov.StartProvider();
                }
                ViewData[Properties.Resources.strPageId] = pid;
                return View(prov);
            });
        }

        [HttpPost]
        public void NextDate(SessionKeeper sKeeper, int pid)
        {
            ParameterGateUnitProvider prov = sKeeper.GetStrucProvider(pid).CurrentUnitProvider as ParameterGateUnitProvider;
            prov.RetrieveValues(prov.GetNextTime());
            //return GetContent(sKeeper);
        }
        [HttpPost]
        public void PrevDate(SessionKeeper sKeeper, int pid)
        {
            ParameterGateUnitProvider prov = sKeeper.GetStrucProvider(pid).CurrentUnitProvider as ParameterGateUnitProvider;
            prov.RetrieveValues(prov.GetPrevTime());
            //return GetContent(sKeeper); //Redirect("/ParameterGate/GetContent");
        }

        [HttpPost]
        public void SetDate(SessionKeeper sKeeper, int pid, string date)
        {
            ParameterGateUnitProvider prov = sKeeper.GetStrucProvider(pid).CurrentUnitProvider as ParameterGateUnitProvider;
            DateTime time;
            if (DateTime.TryParse(date, out time))
            {
                prov.RetrieveValues(time);
            }
            //return RenderContentView(sKeeper);
        }

        private string RenderContentView(SessionKeeper sKeeper, int pid)
        {
            ParameterGateUnitProvider prov = sKeeper.GetStrucProvider(pid).CurrentUnitProvider as ParameterGateUnitProvider;
            StringWriter sw;
            using (sw = new StringWriter())
            {
                ViewEngineResult vr = ViewEngines.Engines.FindPartialView(this.ControllerContext, "GetContent");
                ViewContext viewContext = new ViewContext(this.ControllerContext, vr.View, this.ViewData, this.TempData, sw);
                viewContext.ViewData.Model = prov;
                vr.View.Render(viewContext, sw);
            }
            return sw.ToString();
        }
    }
}
