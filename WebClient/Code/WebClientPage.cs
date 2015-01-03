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
using System.Web.Configuration;

namespace WebClient
{
    public class WebClientPage : System.Web.UI.Page
    {
        private WebRemoteDataService data_service = null;
        private ParametersPageManager parameters_manager = null;

        public WebRemoteDataService DataService
        {
            get
            {
                if (data_service == null)
                    data_service = this.GetDataService();

                return data_service;
            }
        }

        public ParametersPageManager Parameters
        {
            get
            {
                if (parameters_manager == null)
                    parameters_manager = 
                        ParametersPageManager.FromQueryStrings(Request.QueryString);
                return parameters_manager;
            }
        }
    }
}
