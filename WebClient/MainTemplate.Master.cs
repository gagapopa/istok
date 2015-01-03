using System;
using System.Collections;
using System.Collections.Generic;
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
using COTES.ISTOK.ASC;
using System.Web.Configuration;

namespace WebClient
{
    public partial class MainTemplate : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                UserLabel.Text = CutName(Page.User.Identity.Name);

                AdminLink.Enabled = AdminLink.Visible = this.GetDataService().GetUser().IsAdmin;

                if (!this.GetDataService().ServerIsLoad())
                {
                    ParametersPageManager builder = new ParametersPageManager();
                    builder.Add(Configuration.Get(Setting.RequestUrlMarker), 
                                this.Request.AppRelativeCurrentExecutionFilePath);
                    Response.Redirect(Configuration.Get(Setting.ServerLoadInformPageUrl) +
                                      builder.ToString(),
                                      false);
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

        protected void LogoutButton_Click(object sender, EventArgs e)
        {
            try
            {
                FormsAuthentication.SignOut();
                Response.Redirect(Configuration.Get(Setting.LoginPageUrl), false);
                this.GetDataService().LogoutUser();
                Session.Abandon();
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

        private string CutName(string name)
        {
            const int max_lenght = 23;
            const string filler = @"...";

            if (name.Length > max_lenght)
                return name.Remove(max_lenght - filler.Length) + 
                       filler;

            return name;
        }

        protected void AdminLink_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Response.Redirect(Configuration.Get(Setting.AdminPageUrl), false);
            }
            catch { }
        }
    }
}
