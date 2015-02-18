using COTES.ISTOK.ASC;
using COTES.ISTOK.DiagnosticsInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace COTES.ISTOK.ClientCore
{
    public class DiagnosticsDataService : AsyncGlobalWorker
    {
        internal DiagnosticsDataService(Session session)
            : base(session)
        {
            //
        }

        public Diagnostics GetDiagnosticsObject()
        {
            string opid = "GetDiagnosticsObject" + Guid.NewGuid().ToString();
            try
            {
            	var rm = AllocQManager(opid);
            	var res = rm.GetDiagnosticsObject(session.Uid);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<Diagnostics>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
    }
}
