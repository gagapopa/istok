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

namespace WebClient
{
    public partial class ErrorInform : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Configuration.Get(Setting.ExceptionMarker)] != null)
            {
                Exception exp = Session[Configuration.Get(Setting.ExceptionMarker)] as Exception;
                ExceptionDetails.Text = exp.ToString();
                ExceptionMessage.Text = exp.Message;
            }
        }
    }
}
