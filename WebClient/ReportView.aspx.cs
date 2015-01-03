using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.IO;
using FastReport;
using COTES.ISTOK.ASC;

namespace WebClient
{
    public partial class ReportView : System.Web.UI.Page
    {
        ParametersPageManager parameters_manager = null;
        DateTime dateFrom = DateTime.MinValue, dateTo = DateTime.MinValue;
        WebRemoteDataService wrds = null;
        ReportNode node = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                parameters_manager = ParametersPageManager.FromQueryStrings(Request.QueryString);
                int id = 0;
                int.TryParse(parameters_manager["id"], out id);
                DateTime.TryParseExact(parameters_manager["f"], "ddMMyyyyHHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None, out dateFrom);
                DateTime.TryParseExact(parameters_manager["t"], "ddMMyyyyHHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTo);

                if (id != 0)
                {
                    wrds = Global.GetDataService(Session);
                    node = wrds.GetUnitNode(id) as ReportNode;
                }
            }
            catch (Exception ex)
            {
                //
            }
        }

        public void frw_StartReport(object sender, EventArgs e)
        {
            try
            {
                if (wrds != null && node != null && dateFrom != DateTime.MinValue && dateTo != DateTime.MinValue)
                {
                    byte[] res = wrds.GenerateReport(node, dateFrom, dateTo, false);
                    MemoryStream ms = new MemoryStream(res);
                    Report frx = frw.Report;
                    frx.LoadPrepared(ms);
                    frw.ReportDone = true;
                }
            }
            catch (Exception ex)
            {
                //
            }
        }
    }
}
