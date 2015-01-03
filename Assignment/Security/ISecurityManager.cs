using System;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Assignment
{
    interface ISecurityManager
    {
        #region Механизм Alive для определения пользователей отключенных по таймауту
        /// <summary>
        /// Время после последней активности пользователя, перед его отключением по таймауту.
        /// </summary>
        TimeSpan UserTimeout { get; }

        /// <summary>
        /// Обновить последние время активности пользователя
        /// </summary>
        /// <param name="userGUID">ИД сессии пользователя.</param>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        void Alive(Guid userGUID);
        
        /// <summary>
        /// Проверить активнал ли сессия пользователя
        /// </summary>
        /// <param name="userGUID">ИД сессии пользователя.</param>
        /// <returns></returns>
        bool CheckAliveUser(Guid userGUID); 
        #endregion

        /// <summary>
        /// ИД Сессии для внутренних операций
        /// </summary>
        Guid InternalSession { get; }

        /// <summary>
        /// Проверить принадлежат ли две сессии одному и тому же пользователю
        /// </summary>
        /// <param name="userGUID">ИД первой сессии</param>
        /// <param name="anotherGUID">ИД второй сессии</param>
        /// <returns></returns>
        bool AreSameUser(Guid userGUID, Guid anotherGUID);

        /// <summary>
        /// Запросить информацию пользователя
        /// </summary>
        /// <param name="userGUID">ИД сессии пользователя.</param>
        /// <returns></returns>
        UserNode GetUserInfo(Guid userGUID);

        /// <summary>
        /// расчитать хэш для пароля
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        String GetHashPassword(String pass);

        #region Подключение/отключение пользователя
        /// <summary>
        /// Зарегистрировать сессию пользователя
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <returns>
        /// Идентификатор сессии пользователя,
        /// Guid.Empty - если сессия не зарегистрированна
        /// </returns>
        Guid Register(string userName, string password);

        /// <summary>
        /// Удалить информацию о сессии пользователя
        /// </summary>
        /// <param name="sessionGuid">ИД сессии пользователя.</param>
        void Unregister(Guid sessionGuid);

        /// <summary>
        /// Закрыть все сессии пользователя
        /// </summary>
        /// <param name="userNodeID">Идентификатор пользователя</param>
        void UnregisterAll(int userNodeID); 
        #endregion

        #region События на подключение/отключение пользователя
        /// <summary>
        /// Событие, возникаемое при подключении нового пользователя
        /// </summary>
        event EventHandler<SessionIDEventArgs> SessionRegistered;

        /// <summary>
        /// Событие при отключении пользователя
        /// </summary>
        event EventHandler<SessionIDEventArgs> SessionUnregistered; 
        #endregion

        #region Проверка доступа
        /// <summary>
        /// Проверить подключен ли пользователь к системе.
        /// </summary>
        /// <param name="userGUID">ИД сессии пользователя.</param>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        void ValidateAccess(Guid userGUID);

        /// <summary>
        /// Проверить обладает ли указанная сессия админскими правами
        /// </summary>
        /// <param name="userGUID">ИД сессии пользователя.</param>
        /// <returns></returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна.</exception>
        bool CheckAdminAccess(Guid userGUID);

        /// <summary>
        /// Проверить права доступа пользователя. 
        /// Является ли пользователь администратором.
        /// </summary>
        /// <param name="userGUID">ИД сессии пользователя.</param>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна.</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает запрашиваемыми правами доступа.</exception>
        void ValidateAdminAccess(Guid userGUID);

        /// <summary>
        /// Проверить доступ пользователя
        /// </summary>
        /// <param name="userGUID">ИД сессии пользователя.</param>
        /// <param name="unitNode">Проверяемый узел</param>
        /// <param name="privileges">апрашиваемые права доступа</param>
        /// <returns>
        /// Если текущий пользователь обладает указанными правами, true.
        /// В противном случае - false.
        /// </returns>
        /// <exception cref="UserNotConnectedException">Указанная сессия не действительна.</exception>
        bool CheckAccess(Guid userGUID, UnitNode unitNode, Privileges privileges);

        /// <summary>
        /// Проверить права доступа пользователя. 
        /// Обладает ли пользователь правами доступа к определенному узлу.
        /// </summary>
        /// <param name="userGUID">ИД сессии пользователя.</param>
        /// <param name="unitNode">Проверяемый узел</param>
        /// <param name="privileges">апрашиваемые права доступа</param>
        /// <exception cref="UserNotConnectedException">Указанная сессия не действительна.</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает запрашиваемыми правами доступа.</exception>
        void ValidateAccess(Guid userGUID, UnitNode unitNode, Privileges privileges);

        /// <summary>
        /// Проверить права доступа пользователя. 
        /// Обладает ли пользователь правами доступа к определенному узлу.
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="unitNode">Проверяемый узел</param>
        /// <param name="privileges">Запрашиваемые права доступа</param>
        /// <returns>
        /// Если текущий пользователь обладает указанными правами, true.
        /// В противном случае метод вернет false, 
        /// а также добавит соответствующие сообщение в состояние асинхронной операции.
        /// </returns>
        /// <exception cref="UserNotConnectedException">Указанная сессия не действительна.</exception>
        bool CheckAccess(OperationState state, UnitNode unitNode, Privileges privileges); 
        #endregion
    }
}
