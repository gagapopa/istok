using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC.Audit
{
    partial class AuditUnit : AuditItem
    {
        const String fullPathProperty = "Name";
        const String typeNameProperty = "TypeName";

        public override IEnumerable<string> AuditProperties
        {
            get
            {
                yield return fullPathProperty;
                yield return typeNameProperty;
            }
        }

        public override string GetHead(string propertyName)
        {
            switch (propertyName)
            {
                case fullPathProperty:
                    return "Имя";
                case typeNameProperty:
                    return "Тип";
                default:
                    return propertyName;
            }
        }

        public override string GetChange(string propertyName)
        {
            switch (propertyName)
            {
                case fullPathProperty:
                    return GetChange(FullPathOld,FullPathNew);
                case typeNameProperty:
                    return GetChange(TypeNameOld, TypeNameNew);
                default:
                    return null;
            }
        }

        private bool IsAdded { get { return FullPathOld == null; } }

        private bool IsRemoved { get { return FullPathNew == null; } }

        protected override bool IsUpdate { get { return !(IsAdded || IsRemoved); } }

        public override string ToString()
        {
            const String addedFormat = "Добавлен узел";
            const String removedFormat = "Удалён узел";
            const String updateFormat = "Изменён узел";

            if (IsAdded)
            {
                return addedFormat;
            }
            else if (IsRemoved)
            {
                return removedFormat;
            }
            else
            {
                return updateFormat;
            }
        }
    }
}
