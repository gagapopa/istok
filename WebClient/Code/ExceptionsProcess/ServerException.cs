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
    /// <summary>
    /// Класс исключений ошибок на стороне сервера.
    /// </summary>
    public class ServerException : BaseWebClientException
    {
        public ServerException(string message)
            : base(message)
        { }

        public ServerException(Exception inner)
            :base(inner.Message, inner)
        { }

        public ServerException(string message, Exception inner)
            :base(message, inner)
        { }

        public override void Accept(IExceptionHandler handler)
        {
            handler.Process(this);
        }
    }
}
