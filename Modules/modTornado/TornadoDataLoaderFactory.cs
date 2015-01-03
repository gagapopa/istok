using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Modules.modTornado
{
    public class TornadoDataLoaderFactory : IDataLoaderFactory
    {
        static readonly ModuleInfo info;

        const String PropertyCategory = "Торнадо";
        public static readonly ItemProperty HostProperty;
        public static readonly ItemProperty DatabaseProperty;
        public static readonly ItemProperty DatabaseValuesProperty;
        public static readonly ItemProperty UsernameProperty;
        public static readonly ItemProperty PasswordProperty;
        public static readonly ItemProperty TagValuesTableProperty;
        public static readonly ItemProperty AddTimeHourProperty;

        static TornadoDataLoaderFactory()
        {
            //AddProperty("Host", "Имя компьютера", "", baseCategory, "");
            HostProperty = new ItemProperty()
            {
                Name = "Host",
                DisplayName = "Имя компьютера",
                Category = PropertyCategory,
                ValueType = typeof(String)
            };
            //AddProperty("Database", "БД конфигуратора", "", baseCategory, "");
            DatabaseProperty = new ItemProperty()
            {
                Name = "Database",
                DisplayName = "БД конфигуратора",
                Category = PropertyCategory,
                ValueType = typeof(String)
            };
            //AddProperty("DatabaseValues", "БД значений", "", baseCategory, "");
            DatabaseValuesProperty = new ItemProperty()
            {
                Name = "DatabaseValues",
                DisplayName = "БД значений",
                Category = PropertyCategory,
                ValueType = typeof(String)
            };
            //AddProperty("Username", "Имя пользователя", "", baseCategory, "");
            UsernameProperty = new ItemProperty()
            {
                Name = "Username",
                DisplayName = "Имя пользователя",
                Category = PropertyCategory,
                ValueType = typeof(String)
            };
            //AddProperty("Password", "Пароль", "", baseCategory, "");
            PasswordProperty = new ItemProperty()
            {
                Name = "Password",
                DisplayName = "Пароль",
                Category = PropertyCategory,
                ValueType = typeof(String)
            };
            //AddProperty("TagValuesTable", "Имя таблицы значений", "Имя таблицы значений", baseCategory, "");
            TagValuesTableProperty = new ItemProperty()
            {
                Name = "TagValuesTable",
                DisplayName = "Имя таблицы значений",
                Description = "Имя таблицы значений",
                Category = PropertyCategory,
                ValueType = typeof(String)
            };
            //AddProperty("AddTimeHour", "Корректировка времени", "Корректировка времени в часах. Применяется как поправка при агрегации", baseCategory, "");
            AddTimeHourProperty = new ItemProperty()
            {
                Name = "AddTimeHour",
                DisplayName = "Корректировка времени",
                Description = "Корректировка времени в часах. Применяется как поправка при агрегации",
                Category = PropertyCategory,
                ValueType = typeof(String)
            };

            info = new ModuleInfo(
                null,
                PropertyCategory,
                new ItemProperty[]
                {
                    HostProperty,
                    DatabaseProperty,
                    DatabaseValuesProperty,
                    UsernameProperty,
                    PasswordProperty,
                    TagValuesTableProperty,
                    AddTimeHourProperty,
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
            return new TornadoDataLoader();
        }

        #endregion
    }
}
