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
using System.Web.Configuration;

namespace WebClient
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                Response.Redirect(Configuration.Get(Setting.DefaultPageUrl), false);
        }

        protected void PerfomLogin_Click(object sender, EventArgs e)
        {
            try
            {
                WebRemoteDataService data_service = new WebRemoteDataService();
                if (data_service.LoginUser(UserName.Text, Password.Text))
                {
                    Global.SetDataService(Session, data_service);
                    FormsAuthentication.RedirectFromLoginPage(UserName.Text, false);
                }
            }
            catch
            { }
            finally
            {
                Status.Text = Configuration.Get(Setting.LoginErrorMessage);
            }
        }
    }
}
