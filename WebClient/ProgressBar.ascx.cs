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
    public partial class ProgressBar : System.Web.UI.UserControl
    {
        private const double min_progress = 0.0;
        private const double max_progress = 100.0;
        private const int precision = 2;
        private const string percent_format = @"{0} %";
        private double progress = min_progress;
        private double width_per_percent = 3.0;

        protected void Page_Load(object sender, EventArgs e)
        { }

        public double WidthPerProcess
        {
            set
            {
                if (value <= 0.0) return;
                width_per_percent = value;

                UpdateProgress();
            }
            get
            {
                return width_per_percent;
            }
        }

        public double Progress
        {
            set
            {
                if (value < min_progress ||
                    value > max_progress)
                    return;

                progress = value;

                UpdateProgress();
            }
            get
            {
                return progress;
            }
        }

        public string CssProgress
        {
            set
            {
                ProgressIndicator.CssClass = value;
            }
            get
            {
                return ProgressIndicator.CssClass;
            }
        }

        public string CssText
        {
            set
            {
                PercentIndicator.CssClass = value;
            }
            get
            {
                return PercentIndicator.CssClass;
            }
        }

        private void UpdateProgress()
        {
            ProgressIndicator.Width = (int)(width_per_percent * progress);
            PercentIndicator.Text = 
                String.Format(percent_format, 
                              Math.Round(progress, precision));
        }
    }
}