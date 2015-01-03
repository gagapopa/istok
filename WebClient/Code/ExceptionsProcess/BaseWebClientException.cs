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
    /// Абстрактный базовый класс исключений для веб клиента.
    /// Вводит функционал паттерна визитер для обработки
    /// исключений.
    /// </summary>
    public abstract class BaseWebClientException : Exception
    {
        #region Конструкторы
        public BaseWebClientException(string message)
            : base(message)
        { }

        public BaseWebClientException(Exception inner)
            :base(inner.Message, inner)
        { }

        public BaseWebClientException(string message, Exception inner)
            :base(message, inner)
        { }
        #endregion
        
        /// <summary>
        /// Точка входа визитера.
        /// Перегружается наследниками с целью вызова
        /// метода обработки визитера.
        /// </summary>
        /// <param name="handler">
        ///     Объект визитер.
        /// </param>
        public abstract void Accept(IExceptionHandler handler);
    }
}
