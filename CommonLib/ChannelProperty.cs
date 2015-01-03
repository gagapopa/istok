using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK
{
    ///// <summary>
    ///// Описание дополнительного свойства узла
    ///// </summary>
    //[Serializable]
    //public class ChannelProperty
    //{
    //    /// <summary>
    //    /// Имя свойства
    //    /// </summary>
    //    public String Name { get; protected set; }

    //    /// <summary>
    //    /// Отобрааемое имя свойства
    //    /// </summary>
    //    public String DisplayName { get; protected set; }

    //    /// <summary>
    //    /// Описание свойства
    //    /// </summary>
    //    public String Description { get; protected set; }

    //    /// <summary>
    //    /// Категория свойства
    //    /// </summary>
    //    public String Category { get; protected set; }

    //    /// <summary>
    //    /// Тип значения свойства
    //    /// </summary>
    //    public Type ValueType { get; protected set; }

    //    /// <summary>
    //    /// Текущие значение свойства
    //    /// </summary>
    //    public Object PropertyValue { get; protected set; }

    //    public ChannelProperty( 
    //                            String name,
    //                            String displayName,
    //                            String description,
    //                            String category)
    //    {
    //        Name = name;
    //        DisplayName = displayName;
    //        Description = description;
    //        Category = category;
    //        ValueType = typeof(Object);
    //    }

    //    public ChannelProperty( 
    //                            String name,
    //                            String displayName,
    //                            String description,
    //                            String category,
    //                            Object value)
    //        : this(name, displayName, description, category)
    //    {
    //        PropertyValue = value;
    //        if (value != null)
    //            ValueType = value.GetType();
    //    }

    //    public ChannelProperty(
    //                            String name,
    //                            String displayName,
    //                            String description,
    //                            String category,
    //                            Object value,
    //                            Type valueType)
    //        : this(name, displayName, description, category)
    //    {
    //        PropertyValue = value;
    //        ValueType = valueType;
    //    }

    //    public ChannelProperty(
    //                            String name,
    //                            String displayName,
    //                            String description,
    //                            String category,
    //                            Type valueType)
    //        : this(name, displayName, description, category)
    //    {
    //        ValueType = valueType;
    //    }
    //}
}
