using System;
using System.Data;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Extension;

namespace COTES.ISTOK.ClientCore.UnitProviders
{
    /// <summary>
    /// Аргументы для событий порождаемые IGraphUnitProvider 
    /// и хранящие в себе информацию о текущей вкладе
    /// </summary>
    public class GraphUnitProviderEventArgs : EventArgs
    {
        /// <summary>
        /// Вкладка, с которой связано событие
        /// </summary>
        public ExtensionDataInfo TableInfo { get; protected set; }

        public GraphUnitProviderEventArgs(ExtensionDataInfo tableInfo)
        {
            this.TableInfo = tableInfo;
        }
    }
}