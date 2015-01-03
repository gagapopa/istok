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
    public class MonitorTableParameterDescriptor : ParameterDescriptor
    {
        public CellDescriptor Cell { get; private set; }

        public MonitorTableParameterDescriptor(int id,
                                               int parameter_id,
                                               string name,
                                               CellDescriptor cell)
            :base(id, parameter_id, name)
        {
            Cell = cell;
        }
    }
}
