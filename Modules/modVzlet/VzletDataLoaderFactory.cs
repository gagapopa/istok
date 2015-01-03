using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Modules.modVzlet
{
    public class VzletDataLoaderFactory : IDataLoaderFactory
    {
        static ModuleInfo info;

        const String category = "Взлет";

        public static readonly ItemProperty HostProperty;
        public static readonly ItemProperty DatabaseProperty;
        public static readonly ItemProperty UsernameProperty;
        public static readonly ItemProperty PasswordProperty;
        public static readonly ItemProperty HostTypeProperty;

        public static readonly ItemProperty QueryTypeProperty;
        public static readonly ItemProperty QueryFunctionProperty;

        static VzletDataLoaderFactory()
        {
            //AddProperty("Host", "имя компьютера", "", category, "");
            HostProperty = new ItemProperty()
            {
                Name = "Host",
                DisplayName = "Имя компьютера",
                Description = "",
                Category = category,
                ValueType = typeof(String)
            };
            //AddProperty("Database", "Имя базы данных", "", category, "");
            DatabaseProperty = new ItemProperty()
            {
                Name = "Database",
                DisplayName = "Имя базы данных",
                Description = "",
                Category = category,
                ValueType = typeof(String)
            };
            //AddProperty("Username", "Имя пользователя", "", category, "");
            UsernameProperty = new ItemProperty()
            {
                Name = "Username",
                DisplayName = "Имя пользователя",
                Description = "",
                Category = category,
                ValueType = typeof(String)
            };
            //AddProperty("Password", "Пароль", "", category, "");
            PasswordProperty = new ItemProperty()
            {
                Name = "Password",
                DisplayName = "Пароль",
                Description = "",
                Category = category,
                ValueType = typeof(String)
            };
            //AddProperty("HostType", "Тип сервера", "MSSQL или MSJet", category, "");
            HostTypeProperty = new ItemProperty()
            {
                Name = "HostType",
                DisplayName = "Тип сервера",
                Description = "MSSQL или MSJet",
                Category = category,
                ValueType = typeof(String),
                HasStandardValues = true,
                StandardValuesAreExtinct = true,
                StandardValues = new String[] { "MSSQL", "MSJet" }
            };

            QueryTypeProperty = new ItemProperty()
            {
                Name = "QueryType",
                DisplayName = "Тип запроса",
                Description = "Получение данных через SQL-запрос или вызовом функции",
                Category = category,
                ValueType = typeof(String),
                HasStandardValues = true,
                StandardValuesAreExtinct = true,
                StandardValues = new String[] { "Запрос", "Функция" }
            };
            QueryFunctionProperty = new ItemProperty()
            {
                Name = "QueryFunction",
                DisplayName = "Название функции",
                Description = "Название функции получения данных (при выборе \"Тип запроса\"=\"Функция\").",
                Category = category,
                ValueType = typeof(String)
            };

            info = new ModuleInfo(
                null,
                category,
                new ItemProperty[] 
                { 
                    HostProperty,
                    DatabaseProperty,
                    UsernameProperty,
                    PasswordProperty,
                    HostTypeProperty,
                    QueryTypeProperty,
                    QueryFunctionProperty
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
            yield return DataLoadMethod.Archive;
        }

        public IDataLoader CreateLoader(ChannelInfo channelInfo)
        {
            return new VzletDataLoader();
        }

        #endregion
    }
}
