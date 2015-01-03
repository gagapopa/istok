using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Modules.modMHT
{
    class MHTDataLoaderFactory : IDataLoaderFactory
    {
        static ModuleInfo info;

        const String baseCategory = "MHT";
        public static readonly ItemProperty HostProperty;
        public static readonly ItemProperty PathProperty;
        public static readonly ItemProperty UsernameProperty;
        public static readonly ItemProperty PasswordProperty;

        static MHTDataLoaderFactory()
        {
            ////AddProperty("Host", "Имя компьютера", "", baseCategory, "");
            HostProperty = new ItemProperty()
            {
                Name = "Host",
                DisplayName = "Имя компьютера",
                Description = "",
                Category = baseCategory,
                ValueType = typeof(String)
            };
            //AddProperty("Path", "Путь к файлам данных", "", baseCategory, "");
            PathProperty = new ItemProperty()
            {
                Name = "Path",
                DisplayName = "Путь к файлам данных",
                Description="",
                Category=baseCategory,
                ValueType=typeof(String)
            };
            //AddProperty("Username", "Имя пользователя", "", baseCategory, "");
            UsernameProperty = new ItemProperty()
            {
                Name = "Username",
                DisplayName = "Имя пользователя",
                Description = "",
                Category = baseCategory,
                ValueType = typeof(String)
            };
            //AddProperty("Password", "Пароль", "", baseCategory, "");
            PasswordProperty = new ItemProperty()
            {
                Name = "Password",
                DisplayName = "Пароль",
                Description = "",
                Category = baseCategory,
                ValueType = typeof(String)
            };
            //KeepConnected = true;
            ////AddProperty("TagValuesTable", "Имя таблицы значений", "Имя таблицы значений", baseCategory, "");

            info = new ModuleInfo(
                null,
                baseCategory,
                new ItemProperty[]
                {
                    HostProperty,
                    PathProperty,
                    UsernameProperty,
                    PasswordProperty,
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
            return new MHTDataLoader();
        }

        #endregion
    }
}
