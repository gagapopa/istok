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
    /// Класс исключений ошибок параметров страницы.
    /// Генерируется в случае ошибок разбора параметров страницы
    /// или отсутсвия требуемого параметра.
    /// </summary>
    public class PageParameterException : BaseWebClientException
    {
        public PageParameterException(string message)
            : base(message)
        { }

        public PageParameterException(Exception inner)
            :base(inner.Message, inner)
        { }

        public PageParameterException(string message, Exception inner)
            :base(message, inner)
        { }

        public override void Accept(IExceptionHandler handler)
        {
            handler.Process(this);
        }
    }
}
