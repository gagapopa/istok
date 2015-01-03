using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.Configuration;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace WebClient
{
    public class DownloadButtonBuilder
    {
        private static readonly string download_page;
        private static readonly string script_template;
        private static readonly string image_url;
        private static readonly string file_marker = @"file"; 
        private const string button_caption = @"ref";
        private const int size = 15;

        private ParametersPageManager params_buider = 
            new ParametersPageManager();
        
        public string Link { get; set; }
        public Guid File { get; set; }
        public string Text { get; set; }

        static DownloadButtonBuilder() 
        {
            download_page =
                Configuration.Get(Setting.FileDownloadPageUrl);
            script_template = 
                Configuration.Get(Setting.OpenUrlInNewWindowTemplateScript);
            image_url =
                Configuration.Get(Setting.ReferenceImageUrl);
            file_marker =
                Configuration.Get(Setting.FileMarker);
        }

        public DownloadButtonBuilder()
        {   }

        public static implicit operator ImageButton(DownloadButtonBuilder builder)
        {
            ImageButton result = new ImageButton();
            result.ImageUrl = image_url;
            result.AlternateText = button_caption;
            result.ToolTip = builder.Link;
            result.Height = result.Width = size;

            builder.params_buider.Clear();

            builder.params_buider.Add(file_marker,
                                      HttpContext.Current.Server.UrlEncode(builder.File.ToString()));

            result.OnClientClick = String.Format(script_template, 
                                                 download_page + builder.params_buider.ToString());

            return result;
        }
        public static implicit operator HyperLink(DownloadButtonBuilder builder)
        {
            HyperLink result = new HyperLink();

            result.ToolTip = "Файл отчета";
            result.NavigateUrl = builder.Link;
            result.Text = builder.Text;

            builder.params_buider.Clear();

            builder.params_buider.Add(file_marker,
                                      HttpContext.Current.Server.UrlEncode(builder.Link));

            //result.OnClientClick = String.Format(script_template,
            //                                     download_page + builder.params_buider.ToString());
            
            return result;
        }
    }
}
