using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK
{
    /// <summary>
    /// Интерфейс для работы со свойствами с ревизиями
    /// </summary>
    /// <typeparam name="T">
    /// Тип данных значений свойств
    /// </typeparam>
    public interface IRevisedPropertyable<T> : IPropertyable<T>
    {
        /// <summary>
        /// Получить значения свойства
        /// </summary>
        /// <param name="revision">Ревизия</param>
        /// <param name="property">Свойство</param>
        /// <returns>Значение свойства</returns>
        T GetPropertyValue(RevisionInfo revision, ItemProperty property);

        /// <summary>
        /// Получить значения свойства
        /// </summary>
        /// <param name="revision">Ревизия</param>
        /// <param name="propertyName">Имя свойства</param>
        /// <returns>Значение свойства</returns>
        T GetPropertyValue(RevisionInfo revision, String propertyName);

        /// <summary>
        /// Установить значение свойства
        /// </summary>
        /// <param name="revision">Ревизия</param>
        /// <param name="property">Свойство</param>
        /// <param name="value">Новое значение свойства</param>
        void SetPropertyValue(RevisionInfo revision, ItemProperty property, T value);

        /// <summary>
        /// Установить значение свойства
        /// </summary>
        /// <param name="revision">Ревизия</param>
        /// <param name="propertyName">Имя свойства</param>
        /// <param name="value">Новое значение свойства</param>
        void SetPropertyValue(RevisionInfo revision, String propertyName, T value);

        /// <summary>
        /// Получить хранилище ревизий для свойства
        /// </summary>
        /// <param name="property">Свойство</param>
        /// <returns></returns>
        RevisedStorage<T> GetPropertyStorage(ItemProperty property);
    }
}