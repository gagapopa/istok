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
    [System.Runtime.Serialization.DataContract(IsReference = true)]
    
    public partial class AuditCalcStart
    {
        public AuditCalcStart()
        {
            this.AuditCalcNodes = new HashSet<AuditCalcNode>();
        }
    
    	[System.Runtime.Serialization.DataMember]
        public int ID { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public int EntryID { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public System.DateTime CalcStart { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public System.DateTime CalcEnd { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public bool CalcRecalc { get; set; }
    
    	[System.Runtime.Serialization.DataMember]
        public virtual HashSet<AuditCalcNode> AuditCalcNodes { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public virtual AuditEntry AuditEntry { get; set; }
    }
}
