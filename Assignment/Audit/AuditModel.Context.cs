﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace COTES.ISTOK.Assignment.Audit
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using COTES.ISTOK.ASC.Audit;
    
    public partial class AuditEntities : DbContext
    {
        public AuditEntities()
            : base("name=AuditEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<AuditEntry> AuditEntries { get; set; }
        public DbSet<AuditLob> AuditLobs { get; set; }
        public DbSet<AuditProp> AuditProps { get; set; }
        public DbSet<AuditUnit> AuditUnits { get; set; }
        public DbSet<AuditCalcNode> AuditCalcNodes { get; set; }
        public DbSet<AuditCalcStart> AuditCalcStarts { get; set; }
        public DbSet<AuditValue> AuditValues { get; set; }
        public DbSet<AuditGroup> AuditGroups { get; set; }
        public DbSet<AuditType> AuditTypes { get; set; }
        public DbSet<AuditUser> AuditUsers { get; set; }
    }
}
