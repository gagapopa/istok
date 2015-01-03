using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Modules.modOpc
{
     public class OpcDataLoaderFactory : IDataLoaderFactory
    {
        public const String ConnectionTypeOPCHDA = "OPCHDA";
        public const String ConnectionTypeOPCDA_10 = "OPCDA_10";
        public const String ConnectionTypeOPCDA_20 = "OPCDA_20";
        public const String ConnectionTypeOPCDA_30 = "OPCDA_30";

        public static readonly ItemProperty HostProperty;
        public static readonly ItemProperty ServerProperty;
        public static readonly ItemProperty ConnectionTypeProperty;
        public static readonly ItemProperty TagCatalogProperty;
        public static readonly ItemProperty CollectionTypeProperty;

        static ModuleInfo info;

        static OpcDataLoaderFactory()
        {
            String category = "OPC";
            //AddProperty("Host", "Имя компьютера", "", category, "");
            HostProperty = new ItemProperty()
            {
                Name = "Host",
                DisplayName = "Имя компьютера",
                Description = "",
                Category = category,
                ValueType = typeof(String)
            };
            //AddProperty("Server", "OPC сервера", "", category, "");
            ServerProperty = new ItemProperty()
            {
                Name = "Server",
                DisplayName = "OPC сервера",
                Description = "",
                Category = category,
                ValueType = typeof(String)
            };
            //AddProperty("ConnectionType", "Тип сбора данных", "(HDA или DA)", category, "OPCDA_20");
            ConnectionTypeProperty = new ItemProperty()
            {
                Name = "ConnectionType",
                DisplayName = "Тип сбора данных",
                Description = "(HDA или DA)",
                Category = category,
                ValueType = typeof(String),
                HasStandardValues = true,
                StandardValuesAreExtinct = true,
                StandardValues = new String[] 
                {  
                    ConnectionTypeOPCHDA, 
                    ConnectionTypeOPCDA_10,               
                    ConnectionTypeOPCDA_20,
                    ConnectionTypeOPCDA_30,
                },
                DefaultValue = ConnectionTypeOPCDA_20
            };
            //AddProperty("TagCatalog", "Путь к параметрам на сервере", "(только для DA)", category, "");
            TagCatalogProperty = new ItemProperty()
            {
                Name = "TagCatalog",
                DisplayName = "Путь к параметрам на сервере",
                Description = "(только для DA)",
                Category = category,
                ValueType = typeof(String)
            };
            //CollectionType	syn|asyn|sub	- способ получения данных (синхронно|асинхронно|по подписке)
            CollectionTypeProperty = new ItemProperty()
            {
                Name = "CollectionType",
                DisplayName = "Cпособ получения данных",
                Description = "syn|asyn|sub  - (синхронно|асинхронно|по подписке)",
                Category = category,
                ValueType = typeof(String),
                HasStandardValues = true,
                StandardValuesAreExtinct = true,
                StandardValues = new String[] 
                {  
                    "syn", 
                    "asyn",
                    "sub",
                },
            };

            CommonProperty.ParameterCodeProperty.Category = category;
            CommonProperty.ParameterCodeProperty.DisplayName="OPC Tag";

            info = new ModuleInfo(
                null,
                "Модуль сбора OPC",
                new ItemProperty[] { 
                    HostProperty,
                    ServerProperty,
                    ConnectionTypeProperty,
                    TagCatalogProperty,
                    CollectionTypeProperty
                },
                new ItemProperty[] 
                { 
                    CommonProperty.ParameterCodeProperty
                });
        }

        #region IDataLoaderFactory Members

        public ModuleInfo Info
        {
            get { return info; }
        }

        public IEnumerable<DataLoadMethod> GetSupportedLoadMethods()
        {
            return new DataLoadMethod[] 
            {
                DataLoadMethod.Current,
                DataLoadMethod.Subscribe,
                DataLoadMethod.Archive,
            };
        }

        public IDataLoader CreateLoader(ChannelInfo channelInfo)
        {
            String connectionType = channelInfo[ConnectionTypeProperty];

            switch (connectionType)
            {
                case ConnectionTypeOPCHDA:
                    return new OpcHdaDataLoader();
                case ConnectionTypeOPCDA_10:
                case ConnectionTypeOPCDA_20:
                case ConnectionTypeOPCDA_30:
                    return new OpcDaDataLoader();
                default:
                    throw new ArgumentException(String.Format("Некорректное значение свойства {1}", channelInfo, ConnectionTypeProperty.DisplayName));
            }
        }

        #endregion
    }
}
