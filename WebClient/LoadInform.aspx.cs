using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace WebClient
{
    public partial class LoadInform : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                WebRemoteDataService data_service = this.GetDataService();
                RedirectLink.Visible = data_service.ServerIsLoad();
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

        protected void RefreshTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                WebRemoteDataService data_service = this.GetDataService();

                Progress.Progress =
                    data_service.GetServerLoadProgress();

                SatusString.Text =
                    data_service.GetServerLoadStatusString();

                if (data_service.ServerIsLoad())
                {
                    this.RefreshTimer.Enabled = false;
                    
                    ParametersPageManager parameters = ParametersPageManager.FromQueryStrings(Request.QueryString);
                    string request_url = parameters.Exist(Configuration.Get(Setting.RequestUrlMarker)) ? 
                        parameters[Configuration.Get(Setting.RequestUrlMarker)] : null;
                    request_url = request_url != null && request_url != String.Empty && 
                        request_url != Configuration.Get(Setting.ServerLoadInformPageUrl) ?
                                               request_url : Configuration.Get(Setting.DefaultPageUrl);
                    
                    Page.Response.Redirect(request_url, false);
                    
                    RedirectLink.NavigateUrl = request_url;
                    RedirectLink.Visible = true;
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
