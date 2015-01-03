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

namespace WebClient
{
    /// <summary>
    /// Перечисление настроек из веб конфига.
    /// </summary>
    public enum Setting
    {
        FileMarker,
        ExceptionMarker,
        IdObjectMarker,
        RequestUrlMarker,
        DefaultPageUrl,
        LoginErrorMessage,
        ServerLoadInformPageUrl,
        ObjectNotSelectedMessage,
        TreeContentMarker,
        GlobalServerUrl,
        TempIconFolderUrl,
        TempImageFolderUrl,
        TempFileFolderUrl,
        ErrorPageUrl,
        FileDownloadPageUrl,
        OpenUrlInNewWindowTemplateScript,
        ReferenceImageUrl,
        DataServiceMarker,
        AdminPageUrl,
        TempGraphicFolder,
        ReportViewUrl,
        LoginPageUrl
    }

    /// <summary>
    /// Класс обертка для более удобной работы с настройками веб конфига.
    /// </summary>
    static public class Configuration
    {
        /// <summary>
        /// Читает одну настройку из веб конфига.
        /// </summary>
        /// <param name="position">
        ///     Маркер настройки.
        /// </param>
        /// <returns>
        ///     Настройка.
        /// </returns>
        public static string Get(Setting position)
        {
            return WebConfigurationManager.AppSettings[position.ToString()];
        }
    }
}
