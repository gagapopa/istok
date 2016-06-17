using System;
using System.Collections.Generic;

namespace COTES.ISTOK.Modules.Registrator
{
    public class SqlDataLoaderFactory : IDataLoaderFactory
    {
        static readonly ModuleInfo info;

        const String BaseCategory = "Регистратор";

        public static readonly ItemProperty HostProperty;
        public static readonly ItemProperty DatabaseProperty;
        public static readonly ItemProperty UsernameProperty;
        public static readonly ItemProperty PasswordProperty;
        public static readonly ItemProperty QueryTagNamesProperty;

        static SqlDataLoaderFactory()
        {
            //AddProperty("Host", "имя компьютера", "", baseCategory, "");
            HostProperty = new ItemProperty()
            {
                Name = "Host",
                DisplayName = "имя компьютера",
                Category = BaseCategory,
                ValueType = typeof(String)
            };
            //AddProperty("Database", "название базы данных", "", baseCategory, "");
            DatabaseProperty = new ItemProperty()
            {
                Name = "Database",
                DisplayName = "название базы данных",
                Category = BaseCategory,
                ValueType = typeof(String)
            };
            //AddProperty("Username", "имя пользователя", "", baseCategory, "");
            UsernameProperty = new ItemProperty()
            {
                Name = "Username",
                DisplayName = "имя пользователя",
                Category = BaseCategory,
                ValueType = typeof(String)
            };
            //AddProperty("Password", "пароль", "", baseCategory, "");
            PasswordProperty = new ItemProperty()
            {
                Name = "Password",
                DisplayName = "пароль",
                Category = BaseCategory,
                ValueType = typeof(String)
            };

            //AddProperty("QueryTagNames", "SQL-запрос на получение списка параметров из базы", "", baseCategory, "");
            QueryTagNamesProperty = new ItemProperty()
            {
                Name = "QueryTagNames",
                DisplayName = "SQL-запрос на получение списка параметров из базы",
                Category = BaseCategory,
                ValueType = typeof(String)
            };

            info = new ModuleInfo(
                null,
                BaseCategory,
                new ItemProperty[] 
                { 
                    HostProperty,
                    DatabaseProperty,
                    UsernameProperty,
                    PasswordProperty,                    
                    QueryTagNamesProperty
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
            return new Sql2DataLoader();
        }

        #endregion
    }
}
