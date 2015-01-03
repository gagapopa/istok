using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK
{
    /// <summary>
    /// Интерфейс для работы со свойствами
    /// </summary>
    /// <typeparam name="T">
    /// Тип данных значений свойств
    /// </typeparam>
    public interface IPropertyable<T>
    {
        /// <summary>
        /// Проверить наличие данного свойства
        /// </summary>
        /// <param name="property">Свойства</param>
        /// <returns>
        /// Если объект содержит значения для свойства, вернуть true.
        /// В противном случае false.</returns>
        bool Contains(ItemProperty property);

        /// <summary>
        /// Проверить наличие данного свойства
        /// </summary>
        /// <param name="property">Имя свойства</param>
        /// <returns>
        /// Если объект содержит значения для свойства, вернуть true.
        /// В противном случае false.</returns>
        bool Contains(String propertyName);

        /// <summary>
        /// Колекция всех имеющихся у объекта свойств
        /// </summary>
        IEnumerable<ItemProperty> Properties { get; }

        /// <summary>
        /// Получить или утсановить значение свойства по индексу
        /// </summary>
        /// <param name="property">Свойство</param>
        /// <returns>Значение свойства</returns>
        T this[ItemProperty property] { get; set; }

        /// <summary>
        /// Получить значения свойства
        /// </summary>
        /// <param name="property">Свойство</param>
        /// <returns>Значение свойства</returns>
        T GetPropertyValue(ItemProperty property);

        /// <summary>
        /// Получить значения свойства
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        /// <returns>Значение свойства</returns>
        T GetPropertyValue(String propertyName);

        /// <summary>
        /// Установить значение свойства
        /// </summary>
        /// <param name="property">Свойство</param>
        /// <param name="value">Новое значение свойства</param>
        void SetPropertyValue(ItemProperty property, T value);

        /// <summary>
        /// Установить значение свойства
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        /// <param name="value"></param>
        void SetPropertyValue(String propertyName, T value);
    }
}