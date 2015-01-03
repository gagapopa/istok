using System;
using System.Collections.Generic;
using COTES.ISTOK.ASC.Audit;

namespace COTES.ISTOK.Assignment.Audit
{
    interface IAuditServer : IDisposable
    {
        //IEnumerable<AuditEntry> ReadAuditEntries(DateTime startTime, DateTime endTime);

        //IEnumerable<AuditEntry> ReadAuditEntries(int indexStart, int indexEnd);

        IEnumerable<AuditEntry> ReadAuditEntries(AuditRequestContainer request);
        
        void WriteAuditEntry(AuditEntry auditEntry);
    }
}
