//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace COTES.ISTOK.ASC.Audit
{
    using System;
    using System.Collections.Generic;
    
    [System.Serializable]
    [System.Runtime.Serialization.DataContract]
    
    public partial class AuditValue
    {
    	[System.Runtime.Serialization.DataMember]
        public int ID { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public int EntryID { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public int UnitNodeID { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public string UnitNodeFullPath { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public string ValueArgs { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public Nullable<decimal> ValueOriginal { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public Nullable<decimal> ValueNew { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public System.DateTime ValueTime { get; set; }
    
    	[System.Runtime.Serialization.DataMember]
        public virtual AuditEntry AuditEntry { get; set; }
    }
}
