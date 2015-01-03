using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Интерфейс для получения информации о владельце узла
    /// </summary>
    public interface IOwnerRetrieval
    {
        /// <summary>
        /// Получить идентификатор текущего пользователя
        /// </summary>
        /// <returns>Идентификатор пользователя * -1</returns>
        int GetCurrentUser();

        /// <summary>
        /// Получить идентификаторы групп
        /// </summary>
        /// <returns>Ид групп</returns>
        IEnumerable<int> GetGroups();

        /// <summary>
        /// Получить идентификаторы всех пользователей системы
        /// </summary>
        /// <returns>Ид пользователей * -1</returns>
        IEnumerable<int> GetUsers();

        /// <summary>
        /// Получить локалезованное имя владельца
        /// </summary>
        /// <param name="ownerID">Идентфикатор владельца</param>
        /// <param name="culture">Культура</param>
        /// <returns>Отображаемая строка</returns>
        String GetOwnerLocalization(int ownerID, System.Globalization.CultureInfo culture);
    }
}
