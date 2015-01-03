using COTES.ISTOK.WebClient.Models;
using COTES.ISTOK.WebClient.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace COTES.ISTOK.WebClient.HtmlHelpers
{
    public static class LoginHelper
    {
        public static HtmlString GetLoginStrip(this HtmlHelper html)
        {
            SessionKeeper sKeeper = html.ViewContext.HttpContext.Session[Resources.strSessionKeeperKey] as SessionKeeper;
            StringBuilder sb = new StringBuilder();
            
            sb.Append("<div id=\"loginstrip\">");
            if (sKeeper != null)
            {
                sb.Append(string.Format("<span class=\"lslogin\">{0}</span>", sKeeper.Session.User.Text));
                sb.Append("&nbsp;");
                sb.Append("<a class=\"lslogout\" href=\"/Home/Logout\">[Выход]</a>");
            }
            sb.Append("</div>");
            
            return new HtmlString(sb.ToString());
        }
    }
}