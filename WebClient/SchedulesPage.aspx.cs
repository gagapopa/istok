using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace WebClient
{
    public partial class SchedulesPage : WebClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var schedules = DataService.GetParmetrUnloadSchedules();

                TableRow row;
                foreach (var it in schedules)
                {
                    row = new TableRow();
                    row.Cells.Add(new TableCell(){ Text = it.Name });
                    row.Cells.Add(new TableCell(){ Text = it.Rule.ToString() });
                    SchedulesTable.Rows.Add(row);
                }
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
    }
}
