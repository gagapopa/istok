using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Modules
{
    /// <summary>
    /// Объект, принимающий данные от модуля сбора
    /// </summary>
    public interface IDataListener
    {
        /// <summary>
        /// Принять собранные данные
        /// </summary>
        /// <param name="channelInfo">Канал</param>
        /// <param name="parameter">Параметр</param>
        /// <param name="values">Полученные данные</param>
        void NotifyValues(ChannelInfo channelInfo, ParameterItem parameter, IEnumerable<ParamValueItem> values);

        /// <summary>
        /// Принять сообщение об ошибке от модуля сбора
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке</param>
        void NotifyError(String errorMessage);
    }
}
