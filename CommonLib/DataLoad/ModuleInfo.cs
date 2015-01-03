using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace COTES.ISTOK
{
    /// <summary>
    /// Информация о модуле сбора
    /// </summary>
    [Serializable]
    [DataContract]
    public class ModuleInfo
    {
        public ModuleInfo(
            String name,
            String friendlyName,
            IEnumerable<ItemProperty> properties,
            IEnumerable<ItemProperty> parameterProperties)
        {
            Name = name;
            FriendlyName = friendlyName;
            ChannelProperties = (properties ?? new List<ItemProperty>()).ToArray();
            ParameterProperties = (parameterProperties ?? new List<ItemProperty>()).ToArray();
        }

        /// <summary>
        /// Уникальное имя модуля
        /// </summary>
        [DataMember]
        public String Name { get; set; }

        /// <summary>
        /// Отображаемое имя модуля
        /// </summary>
        [DataMember]
        public String FriendlyName { get; set; }

        /// <summary>
        /// Свойства необходимые для настройки канала сбора
        /// </summary>
        [DataMember]
        public ItemProperty[] ChannelProperties { get; set; }

        /// <summary>
        /// Свойства необходимые для собираемых параметров
        /// </summary>
        [DataMember]
        public ItemProperty[] ParameterProperties { get; set; }

        public override string ToString()
        {
            return Name;
            //return String.Format("{0} ({1})", Name, FriendlyName);
        }
    }
}