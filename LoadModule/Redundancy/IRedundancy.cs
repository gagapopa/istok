using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;

namespace COTES.ISTOK.Block.Redundancy
{
    /// <summary>
    /// Интерфейс дублирования
    /// </summary>
    public interface IRedundancy : ITestConnection<Object>
    {
        /// <summary>
        /// Уникальное имя для дублируемой группы
        /// </summary>
        string UID { get; }

        /// <summary>
        /// Возвращает массив участников дублирования
        /// </summary>
        /// <returns>Массив участников</returns>
        ServerInfo[] GetServers();
        /// <summary>
        /// Присоединяет сервер к списку участников дублирования
        /// </summary>
        /// <param name="serverInfo">Информация о подключаемом сервере</param>
        /// <param name="direction">Направление передачи информации</param>
        /// <returns>Массив участников</returns>
        ServerInfo[] AttachServer(ServerInfo serverInfo, TransferDirection direction);
        /// <summary>
        /// Добавлеяет команду в список выполнения
        /// </summary>
        /// <param name="commands">Команда</param>
        void AddCommands(RedundancyCommand[] commands);
        /// <summary>
        /// Запрашивает состояние участника дублирования
        /// </summary>
        /// <returns></returns>
        ServerState GetState();
        /// <summary>
        /// Запрашивает информацию об участнике дублирования
        /// </summary>
        /// <returns></returns>
        ServerInfo GetInfo();
        ///// <summary>
        ///// Запрашивает информацию о репликации
        ///// </summary>
        ///// <returns></returns>
        //PublicationInfo GetReplicationPublication();

        /// <summary>
        /// Запрашивает строку подключения к БД сервера
        /// </summary>
        /// <returns></returns>
        string GetConnectionString();

        /// <summary>
        /// Запрашивает информацию о публикации БД
        /// </summary>
        /// <returns></returns>
        PublicationInfo GetPublicationInfo();
    }
}
