using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebClient
{
    /// <summary>
    /// Интерфейс обработчика исключений - визитера.
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Обрабатывает базовую ошибку, 
        /// в случае невозможности идентифицировать тип.
        /// </summary>
        /// <param name="exp">
        ///     Объект исключения.
        /// </param>
        void Process(BaseWebClientException exp);
        
        /// <summary>
        /// Обрабатывает ошибку отсутсвия подключения.
        /// </summary>
        /// <param name="exp">
        ///     Объект исключения.
        /// </param>
        void Process(NotConnectionException exp);

        /// <summary>
        /// Обрабатывает ошибку параметров страницы.
        /// </summary>
        /// <param name="exp">
        ///     Объект исключения.
        /// </param>
        void Process(PageParameterException exp);
        
        /// <summary>
        /// Обрабатывает ошибки на строне сервера. 
        /// </summary>
        /// <param name="exp">
        ///     Объект исключения.
        /// </param>
        void Process(ServerException exp);
        
        /// <summary>
        /// Обрабатывает не определенные ошибки.
        /// </summary>
        /// <param name="exp">
        ///     Объект исключения.
        /// </param>
        void ProcessUndefined(Exception exp);
    }
}
