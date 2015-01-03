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
    public static class RevisionSelectorHelper
    {
        public static HtmlString GetSelector(this HtmlHelper html, int pid)
        {
            SessionKeeper sKeeper = html.ViewContext.HttpContext.Session[Resources.strSessionKeeperKey] as SessionKeeper;
            StringBuilder sb = new StringBuilder();
            sb.Append("<div id=\"rev\">");
            sb.Append("Рабочая ревизия: ");
            if (sKeeper == null)
                sb.Append("(null)");
            else
            {
                sb.Append("<select>");
                foreach (var item in sKeeper.Revisions)
                {
                    sb.Append(string.Format("<option value=\"{0}\" {2}>{1}</option>",
                        item.ID.ToString(),
                        html.Encode(item.ToString()),
                        item.Equals(sKeeper.GetStrucProvider(pid).CurrentRevision) ? "selected" : ""));
                }
                sb.Append("</select>");
            }
            sb.Append("</div>");
            return new HtmlString(sb.ToString());
        }
    }
}