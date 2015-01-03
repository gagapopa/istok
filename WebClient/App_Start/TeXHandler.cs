using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web;
using System.Web.Routing;
using System.Linq;
using System.IO;
using System.Text;

namespace COTES.ISTOK.WebClient
{
    public class TeXHandler : IHttpHandler
    {
        protected RequestContext RequestContext { get; set; }

        public TeXHandler() { }

        public TeXHandler(RequestContext requestContext)
        {
            RequestContext = requestContext;
        }

        /// <summary>
        /// You will need to configure this handler in the Web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var str = context.Request.Path;
            var finfo = new FileInfo(System.Web.Hosting.HostingEnvironment.MapPath("/Content" + str));
            var exists = false;
            if (!finfo.Exists)
            {
                try
                {
                    str = finfo.Name.Substring(0, finfo.Name.Length - finfo.Extension.Length);
                    //var x = Convert.ToBase64String(Encoding.Unicode.GetBytes(str));
                    //var code = Encoding.Unicode.GetString(Convert.FromBase64String(x));
                    var code = Encoding.Unicode.GetString(Convert.FromBase64String(str));
                    var tex = new MimeTexWrapper();
                    tex.EquationToGif(code, finfo.FullName);
                    //img.Save(context.Response.OutputStream, ImageFormat.Png);
                    //img.Save(finfo.FullName, ImageFormat.Png);
                    finfo.Refresh();
                }
                catch (ArgumentException) { }
            }
            else
                exists = true;
            context.Response.ContentType = "image/gif";
            if (exists || finfo.Exists) context.Response.WriteFile(finfo.FullName);
        }

        #endregion
    }
}
