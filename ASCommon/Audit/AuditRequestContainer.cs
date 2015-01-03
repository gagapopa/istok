using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC.Audit
{
    public class AuditRequestContainer
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public UnitNode NodeFilter { get; set; }

        public AuditCategory CategoryFilter { get; set; }

        public String FilterText { get; set; }
    }

    [Flags]
    public enum AuditCategory
    {
        AllCategories = 0x00,
        TypesChange = 0x01,
        GroupsChange = 0x02,
        UsersChange = 0x04,
        UnitsChange = 0x08,
        CalcStarts = 0x10,
        ValuesChange = 0x20
    }

    public class AuditCategoryTypeConverter : System.ComponentModel.EnumConverter
    {
        public AuditCategoryTypeConverter()
            : base(typeof(AuditCategory))
        {

        }

        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType.Equals(typeof(String)))
            {
                switch ((AuditCategory)value)
                {
                    case AuditCategory.AllCategories:
                        return "Все";
                    case AuditCategory.TypesChange:
                        return "Аудит типов";
                    case AuditCategory.GroupsChange:
                        return "Аудит групп";
                    case AuditCategory.UsersChange:
                        return "Аудит пользователей";
                    case AuditCategory.UnitsChange:
                        return "Аудит структуры";
                    case AuditCategory.CalcStarts:
                        return "Аудит расчёта";
                    case AuditCategory.ValuesChange:
                        return "Аудит значений";
                    default:
                        break;
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is String)
            {
                switch (value.ToString())
                {
                    case "Все":
                        return AuditCategory.AllCategories;
                    case "Аудит типов":
                        return AuditCategory.TypesChange;
                    case "Аудит групп":
                        return AuditCategory.GroupsChange;
                    case "Аудит пользователей":
                        return AuditCategory.UsersChange;
                    case "Аудит структуры":
                        return AuditCategory.UnitsChange;
                    case "Аудит расчёта":
                        return AuditCategory.CalcStarts;
                    case "Аудит значений":
                        return AuditCategory.ValuesChange;
                    default:
                        break;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
