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
using COTES.ISTOK.ASC;
using COTES.ISTOK;
using System.IO;

namespace WebClient
{
    public partial class ExcelReport : TreeContentPage
    {
        ExcelReportNode node = null;

        DateTime dateFrom = DateTime.Today, dateTo = DateTime.Today;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string sid = Parameters[@"ID"];//
                int id = 0;

                //ParametersPageManager params_buider = new ParametersPageManager();
                //params_buider.Add(Configuration.Get(Setting.FileMarker), HttpContext.Current.Server.UrlEncode("report.xlsx"));
                //string tmp = string.Format(Configuration.Get(Setting.OpenUrlInNewWindowTemplateScript),
                //                                 Configuration.Get(Setting.FileDownloadPageUrl) + params_buider.ToString());
                //btnMake.OnClientClick = tmp;

                if (!DateTime.TryParse(FromDate.Text, out dateFrom))
                {
                    dateFrom = DateTime.Today;
                    FromDate.Text = dateFrom.ToString("dd.MM.yyyy HH:mm:ss");
                }
                if (!DateTime.TryParse(ToDate.Text, out dateTo))
                {
                    dateTo = DateTime.Today;
                    ToDate.Text = dateTo.ToString("dd.MM.yyyy HH:mm:ss");
                }

                if (!string.IsNullOrEmpty(sid))
                {
                    int.TryParse(sid, out id);
                    node = this.GetDataService().GetUnitNode(id) as ExcelReportNode;
                }
                if (node != null && node.Typ == UnitTypeId.ExcelReport)
                    lblReport.Text = node.Text;
                else
                    lblReport.Text = "";
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

        protected void btnMake_Click(object sender, EventArgs e)
        {
            try
            {
                if (node != null /*&& node.ExcelReportBody != null*/)
                {
                    string fname = "report.xlsx";
                    byte[] res = this.GetDataService().GetFilledExcelReport(node.Idnum, dateFrom, dateTo);

                    //fname = Path.GetTempFileName();
                    using (FileStream fs = new FileStream(fname, FileMode.Create))
                    {
                        BinaryWriter bw = new BinaryWriter(fs);
                        if (res != null) bw.Write(res);
                        bw.Close();
                    }

                    Guid fr = DataService.AddFileResource(fname);
                    ParametersPageManager params_buider = new ParametersPageManager();
                    params_buider.Add(Configuration.Get(Setting.FileMarker), HttpContext.Current.Server.UrlEncode(fr.ToString()));
                    
                    hl.Text = "Загрузить";
                    hl.Target = "_blank";
                    hl.NavigateUrl = Configuration.Get(Setting.FileDownloadPageUrl) + params_buider.ToString();
                    hl.Visible = true;
                    //Response.Clear();
                    //Response.ContentType = "application/octet-stream";
                    //Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(fname));
                    //Response.Flush();
                    //Response.BinaryWrite(res);
                    //Response.End();
                    //Response.End();
                }
            }
            catch (BaseWebClientException exp)
            {
                exp.Accept(Handler.Instance);
            }
            catch (Exception exp)
            {
                string msg = exp.Message;
                Handler.Instance.ProcessUndefined(exp);
            }
        }
    }
}
