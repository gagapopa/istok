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
    public partial class UpdateIntervalSlider : System.Web.UI.UserControl
    {
        public int Period { get; private set; }
        public event EventHandler OnPeriodChanged;

        protected void Page_Load(object sender, EventArgs e)
        {
           //
        }

        protected void UpdatePeriodTextBound_TextChanged(object sender, EventArgs e)
        {
            const int default_period = 1; // sec
            const int ms_in_second = 1000;

            int period;
            if (Int32.TryParse(UpdatePeriodTextBound.Text, out period))
                Period = period * ms_in_second;
            else
                Period = default_period;

            if (OnPeriodChanged != null)
                OnPeriodChanged(sender, e);
        }
    }
}