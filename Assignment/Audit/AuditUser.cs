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
    
    public partial class AuditUser
    {
    	[System.Runtime.Serialization.DataMember]
        public int ID { get; private set; }
    	[System.Runtime.Serialization.DataMember]
        private int EntryID { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public int UserID { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public string UserLoginOld { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public string UserLoginNew { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public string UserGroupsOld { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public string UserGroupsNew { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public string UserFullNameOld { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public string UserFullNameNew { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public string UserPositionOld { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public string UserPositionNew { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public string UserRoleOld { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public string UserRoleNew { get; set; }
    	[System.Runtime.Serialization.DataMember]
        public bool UserPasswordChanged { get; set; }
    
    	[System.Runtime.Serialization.DataMember]
        public virtual AuditEntry AuditEntry { get; set; }
    }
}
