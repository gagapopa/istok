using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ASC.Audit;

namespace COTES.ISTOK.ClientCore
{
    public class AuditDataService : AsyncGlobalWorker
    {
        internal AuditDataService(Session session)
            : base(session)
        {
            //
        }

        public AuditEntry[] GetAudit(AuditRequestContainer request)
        {
            string opid = "GetAudit" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetAudit(session.Uid, request);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<AuditEntry[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
    }
}
