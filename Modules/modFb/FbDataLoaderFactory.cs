using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Modules.modFb
{
    public class FbDataLoaderFactory : IDataLoaderFactory
    {
        static ModuleInfo info;

        const String category = "Firebird";

        public static readonly ItemProperty HostProperty;
        public static readonly ItemProperty DatabaseProperty;
        public static readonly ItemProperty UsernameProperty;
        public static readonly ItemProperty PasswordProperty;
        public static readonly ItemProperty TagValuesTableProperty;
        public static readonly ItemProperty TagValuesFieldPNameProperty;
        public static readonly ItemProperty TagValuesFieldTNameProperty;
        public static readonly ItemProperty TagValuesFieldRNameProperty;
        public static readonly ItemProperty TagValuesFieldXNameProperty;
        public static readonly ItemProperty TagValuesFieldTimeNameProperty;

        public static readonly ItemProperty TagNamesFieldIdProperty;
        public static readonly ItemProperty TagNamesFieldNameProperty;
        public static readonly ItemProperty TagNamesTableProperty;
        public static readonly ItemProperty TagValuesFieldIDProperty;

        static FbDataLoaderFactory()
        {
            //AddProperty("Host", "Имя сервера", "", category, "");
            HostProperty = new ItemProperty()
            {
                Name = "Host",
                DisplayName = "Имя сервера",
                Description = "",
                Category = category,
                ValueType = typeof(String)
            };
            //AddProperty("Database", "Название базы данных", "", category, "");
            DatabaseProperty = new ItemProperty()
            {
                Name = "Database",
                DisplayName = "Название базы данных",
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
            //AddProperty("TagValuesTable", "Имя таблицы со значениями", "", category, "");
            TagValuesTableProperty = new ItemProperty()
            {
                Name = "TagValuesTable",
                DisplayName = "Имя таблицы со значениями",
                Description = "",
                Category = category,
                ValueType = typeof(String)
            };
            //AddProperty("TagValuesFieldPName", "Имя поля со значением давления", "", category, "");
            TagValuesFieldPNameProperty = new ItemProperty()
            {
                Name = "TagValuesFieldPName",
                DisplayName = "Имя поля со значением давления",
                Description = "",
                Category = category,
                ValueType = typeof(String)
            };
            //AddProperty("TagValuesFieldTName", "Имя поля со значением температуры", "", category, "");
            TagValuesFieldTNameProperty = new ItemProperty()
            {
                Name = "TagValuesFieldTName",
                DisplayName = "Имя поля со значением температуры",
                Description = "",
                Category = category,
                ValueType = typeof(String)
            };
            //AddProperty("TagValuesFieldRName", "Имя поля со значением расхода", "", category, "");
            TagValuesFieldRNameProperty = new ItemProperty()
            {
                Name = "TagValuesFieldRName",
                DisplayName = "Имя поля со значением расхода",
                Description = "",
                Category = category,
                ValueType = typeof(String)
            };
            //AddProperty("TagValuesFieldXName", "Имя поля со значением времени работы", "", category, "");
            TagValuesFieldXNameProperty = new ItemProperty()
            {
                Name = "TagValuesFieldXName",
                DisplayName = "Имя поля со значением времени работы",
                Description = "",
                Category = category,
                ValueType = typeof(String)
            };
            //AddProperty("TagValuesFieldTimeName", "Имя поля с датой записи в таблице значений", "", category, "");
            TagValuesFieldTimeNameProperty = new ItemProperty()
            {
                Name = "TagValuesFieldTimeName",
                DisplayName = "Имя поля с датой записи в таблице значений",
                Description = "",
                Category = category,
                ValueType = typeof(String)
            };
            //AddProperty("TagNamesFieldId", "ИД параметра", "", category, "");
            TagNamesFieldIdProperty = new ItemProperty()
            {
                Name = "TagNamesFieldId",
                DisplayName = "ИД параметра",
                Description = "",
                Category = category,
                ValueType = typeof(String)
            };
            //AddProperty("TagNamesFieldName", "Имя поля с названием параметра в таблице параметров", "", category, "");
            TagNamesFieldNameProperty = new ItemProperty()
            {
                Name = "TagNamesFieldName",
                DisplayName = "Имя поля с названием параметра в таблице параметров",
                Description = "",
                Category = category,
                ValueType = typeof(String)
            };
            //AddProperty("TagNamesTable", "Имя таблицы с названиями параметров", "", category, "");
            TagNamesTableProperty = new ItemProperty()
            {
                Name = "TagNamesTable",
                DisplayName = "Имя таблицы с названиями параметров",
                Description = "",
                Category = category,
                ValueType = typeof(String)
            };
            //AddProperty("TagValuesFieldID", "Имя поле с кодом параметра", "", category, "");
            TagValuesFieldIDProperty = new ItemProperty()
            {
                Name = "TagValuesFieldID",
                DisplayName = "Имя поле с кодом параметра",
                Description = "",
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
                    TagValuesTableProperty,
                    TagValuesFieldPNameProperty,
                    TagValuesFieldTNameProperty,
                    TagValuesFieldRNameProperty,
                    TagValuesFieldXNameProperty,
                    TagValuesFieldTimeNameProperty,

                    TagNamesFieldIdProperty,
                    TagNamesFieldNameProperty,
                    TagNamesTableProperty,
                    TagValuesFieldIDProperty,
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
            yield return DataLoadMethod.ArchiveByParameter;
        }

        public IDataLoader CreateLoader(ChannelInfo channelInfo)
        {
            return new FbDataLoader();
        }

        #endregion
    }
}
