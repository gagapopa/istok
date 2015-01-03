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
    public partial class AdminPage : WebClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!DataService.GetUser().IsAdmin)
                {
                    FormsAuthentication.SignOut();
                    FormsAuthentication.RedirectToLoginPage();
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

        protected void LoadIconsBtn_Click(object sender, EventArgs e)
        {
            try
            {
                DataService.UpdateIconsForTreeView();
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

        protected void ClearTempGraphImageBtn_Click(object sender, EventArgs e)
        {
            try
            {
                DataService.ClearTempZedGraphImages();
                DataService.DeleteUnregisterResource();
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
