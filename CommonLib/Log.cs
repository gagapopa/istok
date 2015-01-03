using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace COTES.ISTOK
{
    /// <summary>
    /// Категория сообщения
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(MessageCategoryTypeConverter))]
    public enum MessageCategory
    {
        /// <summary>
        /// Отладочное сообщение
        /// </summary>
        TraceInfo,
        /// <summary>
        /// Простое информационное сообщение
        /// </summary>
        Message,
        /// <summary>
        /// Предупреждение
        /// </summary>
        Warning,
        /// <summary>
        /// Ошибка
        /// </summary>
        Error,
        /// <summary>
        /// Критическая ошибка
        /// </summary>
        CriticalError
    };

    public class MessageCategoryTypeConverter : EnumConverter
    {
        public MessageCategoryTypeConverter()
            : base(typeof(MessageCategory))
        { }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType.Equals(typeof(String)))
                switch ((MessageCategory)value)
                {
                    case MessageCategory.TraceInfo:
                        return "Отладочное сообщение";
                    case MessageCategory.Message:
                        return "Сообщение";
                    case MessageCategory.Warning:
                        return "Предупреждение";
                    case MessageCategory.Error:
                        return "Ошибка";
                    case MessageCategory.CriticalError:
                        return "Критическая ошибка";
                }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if(value is String)
                switch (value.ToString())
                {
                    case "Отладочное сообщение":
                        return MessageCategory.TraceInfo;
                    case "Сообщение":
                        return MessageCategory.Message;
                    case "Предупреждение":
                        return MessageCategory.Warning;
                    case "Ошибка":
                        return MessageCategory.Error;
                    case "Критическая ошибка":
                        return MessageCategory.CriticalError;
                }
            return base.ConvertFrom(context, culture, value);
        }
    }

    /// <summary>
    /// Класс для хранения сообщения
    /// </summary>
    [Serializable]
    [System.Runtime.Serialization.DataContract]
    [System.Runtime.Serialization.KnownType(typeof(COTES.ISTOK.Calc.CalcMessage))]
    //[System.Runtime.Serialization.KnownType(typeof(MessageByException))]
    public class Message
    {
        /// <summary>
        /// Создать новое сообщение
        /// </summary>
        /// <param name="time">Время возникновения сообщения</param>
        /// <param name="category">Категория сообщения</param>
        /// <param name="message">Текст сообщения</param>
        public Message(DateTime time, MessageCategory category, String message)
        {
            Time = time;
            Category = category;
            Text = message;
        }

        /// <summary>
        /// Создать новое сообщение
        /// </summary>
        /// <param name="category">Категория сообщения</param>
        /// <param name="messageFormat">Форматируемый текст сообщения</param>
        /// <param name="pars">Параметры для форматируемого сообщения</param>
        public Message(MessageCategory category, String messageFormat, params Object[] pars) :
            this(DateTime.Now, category, String.Format(messageFormat, pars))
        { }

        /// <summary>
        /// Время записи сообщения
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public DateTime Time { get; protected set; }

        /// <summary>
        /// Категория сообщения
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public MessageCategory Category { get; protected set; }

        /// <summary>
        /// Текст сообщения 
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public virtual String Text { get; set; }

        public virtual void AppendText(string appendedText)
        {
            Text = appendedText + Text;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    [Serializable]
    [System.Runtime.Serialization.DataContract]
    public class MessageByException : Message
    {
        public Exception Exception { get; protected set; }

        public MessageByException(Exception exc)
            : base(MessageCategory.Error, String.Empty)
        {
            Exception = exc;
            Text = exc.Message;
        }
    }
}
