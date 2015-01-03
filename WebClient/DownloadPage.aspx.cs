using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;

namespace WebClient
{
    public partial class DownloadPage : WebClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /*try
            {
                string file_id = Parameters[Configuration.Get(Setting.FileMarker)];

                FileResource file =
                    DataService.GetResource(new Guid(file_id));

                Response.Clear();

                Response.ContentType = @"application/octet-stream";

                //Response.AddHeader("Content-Type",
                //                    "application/octet-stream");
                //Response.AddHeader("Content-Length:",
                //                      (new FileInfo(Server.MapPath(file.Link))).Length.ToString());
                Response.AddHeader("Content-Disposition",
                                      String.Format("Attachment; FileName=\"{0}\"",
                                                    Path.GetFileName(Server.MapPath(file.Link))));

                Response.Flush();

                Response.TransmitFile(Server.MapPath(file.Link));

                Response.Flush();

                Response.Close();

            }
            catch (BaseWebClientException exp)
            {
                exp.Accept(Handler.Instance);
            }
            catch (Exception exp)
            {
                Handler.Instance.ProcessUndefined(exp);
            }*/
        }

        protected void Download_Click(object sender, EventArgs e)
        {
            try
            {
                string file_id = Parameters[Configuration.Get(Setting.FileMarker)];

                FileResource file =
                    DataService.GetResource(new Guid(file_id));

                Response.Clear();

                Response.AddHeader("Content-Disposition",
                                      String.Format("attachment; filename=\"{0}\"",
                                                    Path.GetFileName(file.Link)));

                Response.Flush();

                Response.TransmitFile(file.Link);

                Response.Flush();

                Response.Close();
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
