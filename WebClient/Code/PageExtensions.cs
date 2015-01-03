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
using System.Web.SessionState;
using System.Web.Configuration;
using System.Xml.Linq;
using System.Net;

namespace WebClient
{
    ///
    public static class PageExtensions
    {
        public static WebRemoteDataService GetDataService(this Page p)
        {
            return Global.GetDataService(p.Session);
        }

        public static WebRemoteDataService GetDataService(this MasterPage p)
        {
            return Global.GetDataService(p.Session);
        }
    }
}
