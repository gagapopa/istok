using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Modules.modSql
{
    public class SqlDataLoaderFactory : IDataLoaderFactory
    {
        static readonly ModuleInfo info;

        const String BaseCategory = "Пирамида";

        public static readonly ItemProperty HostProperty;
        public static readonly ItemProperty DatabaseProperty;
        public static readonly ItemProperty UsernameProperty;
        public static readonly ItemProperty PasswordProperty;
        public static readonly ItemProperty QueryWhereProperty;
        public static readonly ItemProperty QueryTagNamesProperty;

        const String ParamTableCategory = BaseCategory + "Таблица параметров";
        public static readonly ItemProperty TagNamesTableProperty;
        public static readonly ItemProperty TagNamesFieldIdProperty;
        public static readonly ItemProperty TagNamesFieldNameProperty;

        const String ValueTableCategory = BaseCategory + "Таблица значений";
        public static readonly ItemProperty TagValuesTableProperty;
        public static readonly ItemProperty TagValuesFieldIdProperty;
        public static readonly ItemProperty TagValuesFieldNameProperty;
        public static readonly ItemProperty TagValuesFieldTagIdProperty;
        public static readonly ItemProperty TagValuesFieldTagNameProperty;
        public static readonly ItemProperty TagValuesFieldQualityNameProperty;
        public static readonly ItemProperty TagValuesFieldQualityIdProperty;
        public static readonly ItemProperty TagValuesFieldTimeNameProperty;
        public static readonly ItemProperty DateFormatProperty;

        const String QualityTableCategory = BaseCategory + "Таблица качества";
        public static readonly ItemProperty QualityTableProperty;
        public static readonly ItemProperty QualityFieldIdProperty;
        public static readonly ItemProperty QualityFieldNameProperty;

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

            //AddProperty("TagNamesTable", "Имя таблицы параметров", "Имя таблицы с названиями параметров (справочник)", category, "");
            TagNamesTableProperty = new ItemProperty()
            {
                Name = "TagNamesTable",
                DisplayName = "Имя таблицы параметров",
                Description = "Имя таблицы с названиями параметров (справочник)",
                Category = ParamTableCategory,
                ValueType = typeof(String)
            };
            //AddProperty("TagNamesFieldId", "Первичный ключ", "Имя поля с номером записи в таблице параметров", category, "");
            TagNamesFieldIdProperty = new ItemProperty()
            {
                Name = "TagNamesFieldId",
                DisplayName = "Первичный ключ",
                Description = "Имя поля с номером записи в таблице параметров",
                Category = ParamTableCategory,
                ValueType = typeof(String)
            };
            //AddProperty("TagNamesFieldName", "Имя", "имя поля с названием параметра в таблице параметров", category, "");
            TagNamesFieldNameProperty = new ItemProperty()
            {
                Name = "TagNamesFieldName",
                DisplayName = "Имя",
                Description = "имя поля с названием параметра в таблице параметров",
                Category = ParamTableCategory,
                ValueType = typeof(String)
            };

            //AddProperty("TagValuesTable", "Имя таблицы значений", "Имя таблицы значений", category, "");
            TagValuesTableProperty = new ItemProperty()
            {
                Name = "TagValuesTable",
                DisplayName = "Имя таблицы значений",
                Description = "Имя таблицы значений",
                Category = ValueTableCategory,
                ValueType = typeof(String)
            };
            //AddProperty("TagValuesFieldId", "Первичный ключ", "Имя поля с номером записи в таблице значений", category, "");
            TagValuesFieldIdProperty = new ItemProperty()
            {
                Name = "TagValuesFieldId",
                DisplayName = "Первичный ключ",
                Description = "Имя поля с номером записи в таблице значений",
                Category = ValueTableCategory,
                ValueType = typeof(String)
            };
            //AddProperty("TagValuesFieldName", "Значение", "Имя поля со значением параметра в таблице значений", category, "");
            TagValuesFieldNameProperty = new ItemProperty()
            {
                Name = "TagValuesFieldName",
                DisplayName = "Значение",
                Description = "Имя поля со значением параметра в таблице значений",
                Category = ValueTableCategory,
                ValueType = typeof(String)
            };
            //AddProperty("TagValuesFieldTagId", "ИД параметра", "Имя поля с номером параметра (из справочника) в таблице значений", category, "");
            TagValuesFieldTagIdProperty = new ItemProperty()
            {
                Name = "TagValuesFieldTagId",
                DisplayName = "ИД параметра",
                Description = "Имя поля с номером параметра (из справочника) в таблице значений",
                Category = ValueTableCategory,
                ValueType = typeof(String)
            };
            //AddProperty("TagValuesFieldTagName", "Код параметра", "Имя поля с кодовым обозначением параметра в таблице значений", category, "");
            TagValuesFieldTagNameProperty = new ItemProperty()
            {
                Name = "TagValuesFieldTagName",
                DisplayName = "Код параметра",
                Description = "Имя поля с кодовым обозначением параметра в таблице значений",
                Category = ValueTableCategory,
                ValueType = typeof(String)
            };
            //AddProperty("TagValuesFieldQualityName", "Имя качество", "Имя поля с названием (или кодом) качества в таблице значений", category, "");
            TagValuesFieldQualityNameProperty = new ItemProperty()
            {
                Name = "TagValuesFieldQualityName",
                DisplayName = "Имя качество",
                Description = "Имя поля с названием (или кодом) качества в таблице значений",
                Category = ValueTableCategory,
                ValueType = typeof(String)
            };
            //AddProperty("TagValuesFieldQualityId", "Ид качества", "Имя поля с номером качества (из справочника) в таблице значений", category, "");
            TagValuesFieldQualityIdProperty = new ItemProperty()
            {
                Name = "TagValuesFieldQualityId",
                DisplayName = "Ид качества",
                Description = "Имя поля с номером качества (из справочника) в таблице значений",
                Category = ValueTableCategory,
                ValueType = typeof(String)
            };
            //AddProperty("TagValuesFieldTimeName", "Время значения", "Имя поля с временем записи в таблице значений", category, "");
            TagValuesFieldTimeNameProperty = new ItemProperty()
            {
                Name = "TagValuesFieldTimeName",
                DisplayName = "Время значения",
                Description = "Имя поля с временем записи в таблице значений",
                Category = ValueTableCategory,
                ValueType = typeof(String)
            };
            DateFormatProperty = new ItemProperty()
            {
                Name = "DateFormat",
                DisplayName = "Формат даты",
                Description = "Формат хранения даты",
                Category = ValueTableCategory,
                ValueType = typeof(String),
                HasStandardValues = true,
                StandardValuesAreExtinct = true,
                StandardValues = new String[] { "MM.dd.yyyy HH:mm:ss", "dd.MM.yyyy HH:mm:ss" }
            };

            //AddProperty("QualityTable", "Имя таблицы качества", "Имя таблицы с качеством (справочник)", category, "");
            QualityTableProperty = new ItemProperty()
            {
                Name = "QualityTable",
                DisplayName = "Имя таблицы качества",
                Description = "Имя таблицы с качеством (справочник)",
                Category = QualityTableCategory,
                ValueType = typeof(String)
            };
            //AddProperty("QualityFieldId", "Первичный ключ", "Имя поля с номером записи в таблице качеств", category, "");
            QualityFieldIdProperty = new ItemProperty()
            {
                Name = "QualityFieldId",
                DisplayName = "Первичный ключ",
                Description = "Имя поля с номером записи в таблице качеств",
                Category = BaseCategory,
                ValueType = typeof(String)
            };
            //AddProperty("QualityFieldName", "Название качества", "Имя поля с названием качества в таблице качеств", category, "");
            QualityFieldNameProperty = new ItemProperty()
            {
                Name = "QualityFieldName",
                DisplayName = "Название качества",
                Description = "Имя поля с названием качества в таблице качеств",
                Category = QualityTableCategory,
                ValueType = typeof(String)
            };

            //AddProperty("QueryWhere", " доп. условие для запроса значений", "", baseCategory, "");
            QueryWhereProperty = new ItemProperty()
            {
                Name = "QueryWhere",
                DisplayName = " доп. условие для запроса значений",
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
                    QueryWhereProperty,
                    QueryTagNamesProperty,

                    TagNamesTableProperty,
                    TagNamesFieldIdProperty,
                    TagNamesFieldNameProperty,

                    TagValuesTableProperty,
                    TagValuesFieldIdProperty,
                    TagValuesFieldNameProperty,
                    TagValuesFieldTagIdProperty,
                    TagValuesFieldTagNameProperty,
                    TagValuesFieldQualityNameProperty,
                    TagValuesFieldQualityIdProperty,
                    TagValuesFieldTimeNameProperty,
                    DateFormatProperty,

                    QualityTableProperty,
                    QualityFieldIdProperty,
                    QualityFieldNameProperty,
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
            return new SqlDataLoader();
        }

        #endregion
    }
}
