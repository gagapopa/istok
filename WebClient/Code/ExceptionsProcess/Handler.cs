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
using System.Web.Configuration;
using COTES.ISTOK.ASC;
using System.Net.Sockets;

namespace WebClient
{
    /// <summary>
    /// Класс обработчиков ошибок - визитер.
    /// Представляет собой синглетон.
    /// </summary>
    public class Handler : IExceptionHandler
    {
        #region Реализация синглетона
        private static Handler instance = new Handler();

        /// <summary>
        /// Обеспечивает многопоточную типобезопасность.
        /// </summary>
        static Handler()
        { }

        private Handler()
        { }

        public static Handler Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        #region Реализация интерфейса обработчика - визитера
        public void Process(BaseWebClientException exp)
        {
            this.ProcessUndefined(exp);
        }

        public void Process(NotConnectionException exp)
        {
            FormsAuthentication.SignOut();
            HttpContext.Current.Session.Abandon();
            FormsAuthentication.RedirectToLoginPage();
        }

        public void Process(PageParameterException exp)
        {
            try
            {
                HttpContext.Current.Response.Redirect(Configuration.Get(Setting.DefaultPageUrl),
                                                      false);
            }
            catch { }
        }

        public void Process(ServerException exp)
        {
            if (exp.InnerException != null &&
                (exp.GetType().Equals(typeof(UserNotConnectedException)) ||
                 exp.GetType().Equals(typeof(SocketException)) ||
                 exp.GetType().Equals(typeof(UnauthorizedAccessException)) ) )
            {
                FormsAuthentication.SignOut();
                HttpContext.Current.Session.Abandon();
                FormsAuthentication.RedirectToLoginPage();
            }
            else
            {
                ProcessUndefined(exp);
            }
        }

        public void ProcessUndefined(Exception exp)
        {
            try
            {
                HttpContext.Current.Session[Configuration.Get(Setting.ExceptionMarker)] = exp;
                HttpContext.Current.Response.Redirect(Configuration.Get(Setting.ErrorPageUrl),
                                                      false);
            }
            catch { }
        }
        #endregion
    }
}
