using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Modules
{
    /// <summary>
    /// Часто используемые свойства
    /// </summary>
    public static class CommonProperty
    {
        public static String ChannelMessagePrefix(ChannelInfo channel)
        {
            //return String.Format("Канал {0}. ", channel);
            return String.Format("[{0} '{1}'] ", channel.Id, channel.Name);
        }

        public static readonly ItemProperty ChannelLoadMethodProperty = new ItemProperty()
        {
            Name = "LoadMethod",
            DisplayName = "Метод опроса",
            Description = "Метод опроса",
            Category = "Сбор",
            ValueType = typeof(DataLoadMethod),
            HasStandardValues = true,
            StandardValuesAreExtinct = true,
            StandardValues = new String[] 
            { 
                TypeDescriptor.GetConverter(typeof(DataLoadMethod)).ConvertToInvariantString(DataLoadMethod.Current),
                TypeDescriptor.GetConverter(typeof(DataLoadMethod)).ConvertToInvariantString(DataLoadMethod.Subscribe),
                TypeDescriptor.GetConverter(typeof(DataLoadMethod)).ConvertToInvariantString(DataLoadMethod.Archive),
                TypeDescriptor.GetConverter(typeof(DataLoadMethod)).ConvertToInvariantString(DataLoadMethod.ArchiveByParameter),
            }
        };

        public static readonly ItemProperty ParameterCodeProperty = new ItemProperty()
        {
            Name = Consts.ParameterCode,
            DisplayName = "Код параметра",
            Description = "Уникальный код параметра",
            Category = "Сбор"
        };

        #region Общие свойства каналов
        public const String commonItemPropertyCategory = "Общие";
        public static readonly ItemProperty CaptureIntervalNormalProperty = new ItemProperty()
        {
            Name = "CaptureIntervalNormal",
            DisplayName = "Период нормального запроса",
            Description = "Период нормального запроса (в секундах).",
            Category = commonItemPropertyCategory,
            ValueType = typeof(int),
            DefaultValue = 60.ToString()
        };

        public static readonly ItemProperty CaptureIntervalLargeProperty = new ItemProperty()
        {
            Name = "CaptureIntervalLarge",
            DisplayName = "Период увеличенного запроса",
            Description = "Период увеличенного запроса (в секундах)",
            Category = commonItemPropertyCategory,
            ValueType = typeof(int),
            DefaultValue = 600.ToString()
        };

        public static readonly ItemProperty BufferLimitProperty = new ItemProperty()
        {
            Name = "BufferLimit",
            DisplayName = "Размер буфера",
            Description = "Максимальное количество значений параметров, по превышению которого прекращается сбор.",
            Category = commonItemPropertyCategory,
            ValueType = typeof(int),
            DefaultValue = 500.ToString()
        };

        public static readonly ItemProperty PauseProperty = new ItemProperty()
        {
            Name = "Pause",
            DisplayName = "Задержка между запросами",
            Description = "Задержка в секундах между запросами.",
            Category = commonItemPropertyCategory,
            ValueType = typeof(float),
            DefaultValue = 1.ToString()
        };

        public static readonly ItemProperty TimeLagProperty = new ItemProperty()
        {
            Name = "TimeLag",
            DisplayName = "Задержка от текущего времени",
            Description = "Задержка от текущего времени (сек)",
            Category = commonItemPropertyCategory,
            ValueType = typeof(int),
            DefaultValue = 0.ToString()
        };
        #endregion

        #region Общие свойства параметров

        #endregion

    }
}
