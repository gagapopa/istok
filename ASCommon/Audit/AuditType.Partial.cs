using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace COTES.ISTOK.ASC.Audit
{
    [Serializable]
    [DataContract]
    public abstract class AuditItem
    {
        public abstract IEnumerable<String> AuditProperties { get; }

        public abstract String GetHead(String propertyName);

        public abstract String GetChange(String propertyName);

        protected abstract bool IsUpdate { get; }

        protected string GetChange(String oldValue, String newValue)
        {
            if (IsUpdate
                && !String.Equals(oldValue, newValue))
            {
                return String.Format("'{0}' --> '{1}'", oldValue, newValue);
            }
            return oldValue ?? newValue;
        }
    }

    partial class AuditType : AuditItem
    {
        public Guid ExtGuid
        {
            get { return new Guid(ExtGuidBinary); }
            set { ExtGuidBinary = value.ToByteArray(); }
        }

        private bool IsAdded { get { return TypeNameOld == null; } }

        private bool IsRemoved { get { return TypeNameNew == null; } }

        protected override bool IsUpdate { get { return !(IsAdded || IsRemoved); } }

        const String typeNameProperty = "Name";
        const String typePropsProperty = "TypeProps";
        const String typeImageProperty = "TypeImage";
        const String typeChildFilterProperty = "TypeChildFilter";

        public override IEnumerable<String> AuditProperties
        {
            get
            {
                yield return typeNameProperty;
                yield return typePropsProperty;
                yield return typeImageProperty;
                yield return typeChildFilterProperty;
            }
        }

        public override string GetHead(string propertyName)
        {
            switch (propertyName)
            {
                case typeNameProperty:
                    return "Название";
                case typePropsProperty:
                    return "Дополнительные свойства";
                case typeImageProperty:
                    return "Иконка типа";
                case typeChildFilterProperty:
                    return "Дочерний фильтр";
                default:
                    return propertyName;
            }
        }

        public override string GetChange(string propertyName)
        {
            switch (propertyName)
            {
                case typeNameProperty:
                    return GetChange(TypeNameOld, TypeNameNew);
                case typePropsProperty:
                    return GetChange(TypePropsOld, TypePropsNew);
                case typeImageProperty:
                    if (IsUpdate)
                    {
                        return "Изменилась";
                    }
                    return String.Empty;
                case typeChildFilterProperty:
                    return GetChange(TypeChildFilterOld, TypeChildFilterNew);
                default:
                    return null;
            }
        }


        public override string ToString()
        {
            const String addedFormat = "Добавлен тип (ID: {0}, GUID: {{1}}) '{2}'";
            const String removeFormat = "Удалён тип (ID: {0}, GUID: {{1}}) '{2}'";
            const String updateFormat = "Изменён тип (ID: {0}, GUID: {{1}}) '{2}'";

            String format;

            if (IsAdded)
            {
                format = addedFormat;
            }
            else if (IsRemoved)
            {
                format = removeFormat;
            }
            else
            {
                format = updateFormat;
            }

            return String.Format(format, TypeID, ExtGuid, TypeNameOld ?? TypeNameNew);
        }
    }
}
