using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC.Audit
{
    partial class AuditProp : AuditItem
    {
        const String unitNodeNameProperty = "Name";
        const String propertyNameProperty = "PropName";
        const String revisionProperty = "Revision";
        const String valueProperty = "Value";

        public override IEnumerable<string> AuditProperties
        {
            get {
                yield return  unitNodeNameProperty;
                yield return  propertyNameProperty ;
                yield return  revisionProperty ;
                yield return valueProperty; 
            }
        }

        public override string GetHead(string propertyName)
        {
            switch (propertyName)
            {
                case unitNodeNameProperty:
                    return "Имя";
                case propertyNameProperty:
                    return "Имя свойства";
                case revisionProperty:
                    return "Ревизия";
                case valueProperty: 
                    return "Значение";
                default:
                    return propertyName;
            }
        }

        public override string GetChange(string propertyName)
        {
            switch (propertyName)
            {
                case unitNodeNameProperty:
                    return UnitNodeFullPath;
                case propertyNameProperty:
                    return PropName;
                case revisionProperty:
                    if (RevisionID != null)
                    {
                        var revision = new RevisionInfo()
                        {
                            ID = (int)RevisionID,
                            Time = (DateTime)RevisionTime,
                            Brief = RevisionBrief
                        };
                        return revision.ToString();
                    }
                    return null;
                case valueProperty:
                    return GetChange(ValueOld, ValueNew);
                default:
                    return null;
            }
        }

        private bool IsAdded { get { return ValueOld == null; } }

        private bool IsRemoved { get { return ValueNew == null; } }

        protected override bool IsUpdate { get { return !(IsAdded || IsRemoved); } }

        public override string ToString()
        {
            const String addedFormat = "Добавлено свойство";
            const String removedFormat = "Удалено свойство";
            const String updateFormat = "Изменено свойство";

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
