using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Modules.modAtec
{
    public class AtecDataLoaderFactory : IDataLoaderFactory
    {
        static ModuleInfo info;

        const String category = "ПО Отчет";

        public static readonly ItemProperty HostProperty;
        public static readonly ItemProperty DatabaseProperty;
        public static readonly ItemProperty UsernameProperty;
        public static readonly ItemProperty PasswordProperty;
        public static readonly ItemProperty HostTypeProperty;

        static AtecDataLoaderFactory()
        {
            //AddProperty("Host", "имя компьютера", "", category, "");
            HostProperty = new ItemProperty()
            {
                Name = "Host",
                DisplayName = "имя компьютера",
                Description="",
                Category=category,
                ValueType=typeof(String)
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
            return new AtecDataLoader();
        }

        #endregion
    }
}
