using COTES.ISTOK.ClientCore.UnitProviders;
using COTES.ISTOK.WebClient.Models;
using COTES.ISTOK.WebClient.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace COTES.ISTOK.WebClient.Controllers.UnitControllers
{
    public class ReportUnitController : Controller
    {

        [HttpPost]
        public ActionResult GetContent(SessionKeeper sKeeper, int pid)
        {
            ViewData[Resources.strPageId] = pid;
            return View(sKeeper.GetStrucProvider(pid).CurrentUnitProvider);
        }

        //[HttpPost]
        public FileResult GenerateReport(SessionKeeper sKeeper, int pid, string dateFrom, string dateTo)
        {
            try
            {
                DateTime datFrom, datTo;
                var prov = sKeeper.GetStrucProvider(pid).CurrentUnitProvider as ExcelReportUnitProvider;

                //datFrom = datTo = DateTime.Now;
                DateTime.TryParse(dateFrom, out datFrom);
                DateTime.TryParse(dateTo, out datTo);
                prov.DatFrom = datFrom;
                prov.DatTo = datTo;
                var buf = prov.GenerateReport(false);

                return File(buf, System.Net.Mime.MediaTypeNames.Application.Octet, "report.xlsx");
            }
            catch
            {
                //лог
                return File(new byte[0], System.Net.Mime.MediaTypeNames.Application.Octet, "Error");
            }
        }
    }
}
