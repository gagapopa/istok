using System;
using System.Collections.Generic;
using System.Data;

namespace COTES.ISTOK.Extension
{
    /// <summary>
    /// Информации о расширении
    /// </summary>
    [Serializable]
    public class ExtensionInfo
    {
        /// <summary>
        /// ИД расширения, различное при разных запусках системы
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Описание расширения
        /// </summary>
        public String Caption { get; private set; }

        public ExtensionInfo(int id, String caption)
        {
            this.ID = id;
            this.Caption = caption;
        }
    }
}