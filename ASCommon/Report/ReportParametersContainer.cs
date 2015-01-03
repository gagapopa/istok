using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
//using COTES.ISTOK.ASC;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Контейнер, для настройки параметров отчёта
    /// </summary>
    [TypeConverter(typeof(ReportPropertiesTypeConverter))]
    public class ReportParametersContainer 
    {
        public ReportParametersContainer()
        {
            ParameterDictionary = new Dictionary<string, ReportParameter>();
        }

        /// <summary>
        /// Словарь, основное хранилище параметров отчёта
        /// </summary>
        Dictionary<String, ReportParameter> ParameterDictionary;

        /// <summary>
        /// Получить список параметров отчёта
        /// </summary>
        public List<ReportParameter> Parameters
        {
            get
            {
                return new List<ReportParameter>(ParameterDictionary.Values);
            }
        }

        /// <summary>
        /// Указывает, что сордержимое отчёта только для чтения
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Обновить содержимое контейнера.
        /// Параметры не в передаваемом списке удаляются. 
        /// Новые параметры добавляются.
        /// Значения оставщихся параметров остаются без изменений.
        /// </summary>
        /// <param name="changedProperties"></param>
        public void SetProperties(IEnumerable<ReportParameter> changedProperties)
        {
            List<ReportParameter> source = new List<ReportParameter>(changedProperties);

            List<String> nameList = source.ConvertAll(r => r.Name);

            foreach (var item in ParameterDictionary.Keys.ToArray())
            {
                if (!nameList.Contains(item))
                    ParameterDictionary.Remove(item);
            }
            foreach (var item in source)
            {
                if (!ParameterDictionary.ContainsKey(item.Name))
                    ParameterDictionary[item.Name] = item;
            }
        }

        /// <summary>
        /// Очистить списко парамтеров
        /// </summary>
        public void Clear()
        {
            ParameterDictionary.Clear();
        }

        #region IReportParameterContainer Members

        public ReportParameter GetParameter(string parameterName)
        {
            return Parameters.Find(p => String.Equals(p.Name, parameterName));
        }

        #endregion
    }

    /// <summary>
    /// TypeConverter для корректного отображения содержимого ReportParametersContainer
    /// </summary>
    public class ReportPropertiesTypeConverter : ExpandableObjectConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            ReportParametersContainer container = value as ReportParametersContainer;

            if (container != null)
            {
                PropertyDescriptor[] props = container.Parameters.ConvertAll(p => new ReportSourcePropertyDescriptor(container, p)).ToArray();

                return new PropertyDescriptorCollection(props);
            }

            return base.GetProperties(context, value, attributes);
        }

        /// <summary>
        /// Дескриптор для параметра отчёта
        /// </summary>
        class ReportSourcePropertyDescriptor : SimplePropertyDescriptor
        {
            ReportParametersContainer container;
            ReportParameter reportParameter;

            public ReportSourcePropertyDescriptor(ReportParametersContainer container, ReportParameter reportParameter)
                : base(reportParameter.GetType(), reportParameter.Name, Type.GetType(reportParameter.PropertyType))
            {
                this.container = container;
                this.reportParameter = reportParameter;
            }

            protected override Attribute[] AttributeArray
            {
                get
                {
                    List<Attribute> attributesList = new List<Attribute>(base.AttributeArray);
                    if (reportParameter.TypeConverter != null)
                    {
                        attributesList.Add(new TypeConverterAttribute(reportParameter.TypeConverter));
                    }
                    return attributesList.ToArray();
                }
                set
                {
                    base.AttributeArray = value;
                }
            }

            public override string DisplayName
            {
                get
                {
                    return reportParameter.DisplayName;
                }
            }

            public override string Description
            {
                get
                {
                    return reportParameter.Description;
                }
            }

            public override string Category
            {
                get
                {
                    return reportParameter.Category;
                }
            }

            public override object GetValue(object component)
            {
                return reportParameter.GetValue();
            }

            public override void SetValue(object component, object value)
            {
                reportParameter.SetValue(value);
                if (reportParameter.RequiredString && Converter != null)
                {
                    reportParameter.StringValue = Converter.ConvertToString(value);
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    return container.ReadOnly;
                }
            }

            //TypeConverter converter;

            //public override TypeConverter Converter
            //{
            //    get
            //    {
            //        return converter = base.Converter;
            //    }
            //}
        }
    }
}
