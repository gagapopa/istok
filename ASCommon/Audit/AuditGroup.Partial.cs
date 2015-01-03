using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC.Audit
{
    partial class AuditGroup : AuditItem
    {
        const String groupNameProperty = "Name";
        const String groupDescriptionProperty = "GroupDescription";


        public override IEnumerable<string> AuditProperties
        {
            get
            {
                yield return groupNameProperty;
                yield return groupDescriptionProperty;
            }
        }

        public override string GetHead(string propertyName)
        {
            switch (propertyName)
            {
                case groupNameProperty: return "Имя";
                case groupDescriptionProperty: return "Описание";
                default:
                    return propertyName;
            }
        }

        public override string GetChange(string propertyName)
        {
            switch (propertyName)
            {
                case groupNameProperty:
                    return GetChange(GroupNameOld, GroupNameNew);
                case groupDescriptionProperty: 
                    return GetChange(GroupDescriptionOld, GroupDescriptionNew);
                default:
                    return null;
            }
        }

        private bool IsAdded { get { return GroupNameOld == null; } }

        private bool IsRemoved { get { return GroupNameNew == null; } }

        protected override bool IsUpdate { get { return !(IsAdded || IsRemoved); } }

        public override string ToString()
        {
            const String addedFormat = "Добавлена группа";
            const String removedFormat = "Удалена группа";
            const String updateFormat = "Изменена группа";

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
