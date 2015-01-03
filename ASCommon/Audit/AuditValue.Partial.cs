using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC.Audit
{
    partial class AuditValue : AuditItem
    {
        const String valueParamProperty = "Name";
        const String valueTimeProperty = "Time";
        const String valueArgsProperty = "Args";
        const String valueProperty = "Value";

        public override IEnumerable<string> AuditProperties
        {
            get
            {
                yield return valueParamProperty;
                yield return valueTimeProperty;
                yield return valueArgsProperty;
                yield return valueProperty;
            }
        }

        public override string GetHead(string propertyName)
        {
            switch (propertyName)
            {
                case valueParamProperty: return "Параметр";
                case valueTimeProperty: return "Время";
                case valueArgsProperty: return "Аргументы";
                case valueProperty: return "Значение";
                default:
                    return propertyName;
            }
        }

        public override string GetChange(string propertyName)
        {
            switch (propertyName)
            {
                case valueParamProperty:
                    return UnitNodeFullPath;
                case valueTimeProperty:
                    return ValueTime.ToString();
                case valueArgsProperty:
                    return (ValueArgs == null ? null : ValueArgs.ToString());
                case valueProperty:
                    return GetChange(
                        (ValueOriginal == null ? null : ValueOriginal.ToString()),
                        (ValueNew == null ? null : ValueNew.ToString()));
                default:
                    return null;
            }
        }

        protected override bool IsUpdate
        {
            get { return ValueNew != null && ValueOriginal != null; }
        }

        public override string ToString()
        {
            if (ValueNew == null)
            {
                return "Удалено значение";
            }
            if (ValueOriginal != null)
            {
                return "Скорректированно значение";
            }
            return "Введено значение";
        }
    }
}
