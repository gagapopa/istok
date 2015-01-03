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
    public class DivBuilder
    {
        const string begin_tag = @"<div {0}>";
        const string end_tag = @"</div>";
        const string style = "style=\"position:absolute; top:{0}px; left:{1}px; border-style:none;\"";

        public int Top { get; set; }
        public int Left { get; set; }

        public DivBuilder()
        { }

        public LiteralControl BeginTag
        { get { return new LiteralControl(BuildFullBeginTag()); } }

        private string BuildFullBeginTag()
        {
            return String.Format(begin_tag,
                                 String.Format(style,
                                               Top,
                                               Left));
        }

        public LiteralControl EndTag
        { get { return new LiteralControl(end_tag); } }

        public static implicit operator LiteralControl(DivBuilder div)
        {
            return new LiteralControl(div.ToString());
        }

        public override string ToString()
        {
            return BuildFullBeginTag() + end_tag;
        }
    }
}
