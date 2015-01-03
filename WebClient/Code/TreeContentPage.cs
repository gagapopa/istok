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
    public class TreeContentPage : WebClientPage
    {
        protected void SetSpecificTab(bool visible)
        {
            try
            {
                var master = this.Master as TreeTemplate;
                master.SpecificTabVisible = visible;
            }
            catch
            {   }
        }
    }
}
