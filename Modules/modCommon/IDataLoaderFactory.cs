using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Modules
{
    /// <summary>
    /// Фабрика для IDataLoader. 
    /// Так же хранит метаинформацию о модуле сбора
    /// </summary>
    public interface IDataLoaderFactory
    {
        /// <summary>
        /// Информация о модуле сбора, 
        /// о требуемых настройках канала и параметра
        /// </summary>
        ModuleInfo Info { get; }

        /// <summary>
        /// Получить поддерживаемые методы сбора данных
        /// </summary>
        /// <returns></returns>
        IEnumerable<DataLoadMethod> GetSupportedLoadMethods();

        /// <summary>
        /// Создать модуль сбора
        /// </summary>
        /// <returns></returns>
        IDataLoader CreateLoader(ChannelInfo channelInfo);
    }
}
