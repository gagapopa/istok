using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace COTES.ISTOK.ASC.TypeConverters
{
    public enum CategoryGroup
    {
        Debug,
        General,
        Calc,
        Load,
        Values,
        Appearance,
        Misc
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    sealed class CategoryOrderAttribute : Attribute
    {
        readonly CategoryGroup categoryGroup;

        public CategoryOrderAttribute(CategoryGroup categoryGroup)
        {
            this.categoryGroup = categoryGroup;
        }

        public CategoryGroup Group
        {
            get { return categoryGroup; }
        }
    }
}
