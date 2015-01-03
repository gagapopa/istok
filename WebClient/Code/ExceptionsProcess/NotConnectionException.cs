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
    /// Класс исключений ошибки соединения.
    /// Генерируется в случае если пользователь не подключен,
    /// нет входа, нет соединения с сервером.
    /// </summary>
    public class NotConnectionException : BaseWebClientException
    {
        public NotConnectionException(string message)
            : base(message)
        { }

        public NotConnectionException(Exception inner)
            :base(inner.Message, inner)
        { }

        public NotConnectionException(string message, Exception inner)
            :base(message, inner)
        { }

        public override void Accept(IExceptionHandler handler)
        {
            handler.Process(this);
        }
    }
}
