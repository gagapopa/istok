using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOKDataUpdate
{
    [AttributeUsage(
        AttributeTargets.Class, 
        Inherited = false, 
        AllowMultiple = false)]
    sealed class ISTOKDBUpdaterAttribute : Attribute
    {
        readonly CurrentService.ServiceType serverType;

        public ISTOKDBUpdaterAttribute(CurrentService.ServiceType serverType)
        {
            this.serverType = serverType;
        }

        public CurrentService.ServiceType ServerType
        {
            get { return serverType; }
        }

        //public int DBVersionFrom { get; set; }

        //public int DBVersionTo { get; set; }

        //public IEnumerable<Guid> RequiredBefore { get; set; }
    }
}
