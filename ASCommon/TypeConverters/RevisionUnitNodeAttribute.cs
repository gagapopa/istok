using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC
{
    [global::System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class RevisionUnitNodeAttribute : Attribute
    {
        readonly string propertyName;

        public RevisionUnitNodeAttribute(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public string PropertyName
        {
            get { return propertyName; }
        }

        public bool IsBinary { get; set; }
    }
}
