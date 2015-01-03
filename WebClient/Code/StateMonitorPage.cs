using System;
using System.Data;
using System.Configuration;
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
    public class StateMonitorPage : TreeContentPage
    {
        public string FormatTooltip(string parameter_name,
                                    DateTime update_rime)
        {
            const string tooltip_format = @"{0} [{1}]";
            const string full_datetime_format = @"F";

            return String.Format(tooltip_format,
                                 parameter_name,
                                 update_rime.ToString(full_datetime_format));
        }
    }
}
