using System;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.Assignment
{
    interface IUserManager
    {
        #region Редактирование пользователей
        /// <summary>
        /// Добавить нового пользователя
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="addnode">Добавляемый пользователь</param>
        /// <returns>Добавленный пользователь</returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        UserNode AddUserNode(OperationState state, UserNode addnode);

        /// <summary>
        /// Изменить пользователя 
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="updnode">Редактируемый узел пользователя</param>
        /// <returns>Обновленный узел пользователя</returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        UserNode UpdateUserNode(OperationState state, UserNode updnode);

        /// <summary>
        /// Удалить пользователя
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="userNodeID">Идентификаторы удаляемого пользователя</param>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        void RemoveUserNode(OperationState state, int userNodeID);

        /// <summary>
        /// Добавить первого поьзователя.
        /// Добавление пользователя через данный метод возможно только 
        /// если метод GetUser посылает исключение COTES.ISTOK.ASC.NoOneUserException.
        /// </summary>
        /// <param name="userNode">Узел первого пользователя</param>
        /// <exception cref="UnauthorizedAccessException">Если в системе уже присутствует пользователь.</exception>
        void NewAdmin(UserNode userNode);
        #endregion

        #region Запрос информации о пользователях
        /// <summary>
        /// Получить узел пользователя по UID
        /// </summary>
        /// <param name="userID">Идентификатор пользователя</param>
        /// <returns>Узел пользователя</returns>
        UserNode GetUser(int userID);

        /// <summary>
        /// Запросить узел пользователя по его логину
        /// </summary>
        /// <param name="userName">Имя пользователя</param>
        /// <returns>
        /// Узел запрашиваемого пользователя, 
        /// null - если пользователя с таким именем не найдено
        /// </returns>
        /// <exception cref="COTES.ISTOK.ASC.NoOneUserException">В системе не существует ни одного пользователя</exception>
        UserNode GetUser(string userName);

        /// <summary>
        /// Запросить список пользователей, присутствующих в системе
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <returns></returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        UserNode[] GetUserNodes(OperationState state);
        
        /// <summary>
        /// апросить список пользователей, присутствующих в системе
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="ids">Массив ИДов</param>
        /// <returns></returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        UserNode[] GetUserNodes(OperationState state, int[] ids);
        #endregion

        #region Редактирование групп
        /// <summary>
        /// Добавить новую группу пользователей
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="addnode">Добавляемая группа</param>
        /// <returns>Добавленная группа</returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        GroupNode AddGroupNode(OperationState state, GroupNode addnode);

        /// <summary>
        /// Удалить группу пользователей
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="groupNodeID">Идентификатор удаляемой группы</param>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        void RemoveGroupNode(OperationState state, int groupNodeID);

        /// <summary>
        /// Изменить группу пользователя
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <param name="updnode">Редактируемая группа пользователей</param>
        /// <returns>Обновленная группа пользователей</returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        GroupNode UpdateGroupNode(OperationState state, GroupNode updnode);
        #endregion

        #region Получение информации о группах
        /// <summary>
        /// Получить узел группы пользователей по GID
        /// </summary>
        /// <param name="GID">Идентификатор группы</param>
        /// <returns>Узел группы</returns>
        GroupNode GetGroup(int GID);

        /// <summary>
        /// Запросить группу по названию
        /// </summary>
        /// <param name="groupName">Уникальное имя группы</param>
        /// <returns>Узел группы</returns>
        GroupNode GetGroup(string groupName);

        /// <summary>
        /// Запросить список групп пользователей
        /// </summary>
        /// <param name="state">Состояние асинхронной операции.</param>
        /// <returns></returns>
        /// <exception cref="COTES.ISTOK.ASC.UserNotConnectedException">Указанная сессия не действительна</exception>
        /// <exception cref="UnauthorizedAccessException">Пользователь не обладает правами для проведения операции</exception>
        GroupNode[] GetGroupNodes(OperationState state); 
        #endregion
    }
}
