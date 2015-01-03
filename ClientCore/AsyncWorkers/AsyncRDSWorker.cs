using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace COTES.ISTOK.ClientCore
{
    public class AsyncRDSWorker : AsyncWorker
    {
        internal RemoteDataService RDS { get { return Session.RemoteDataService; } }

        public Session Session { get; protected set; }

        internal AsyncRDSWorker(Session session)
        {
            Session = session;
        }

        #region AsyncOperations
        public override void WaitAsyncOperation(ulong operationId)
        {
            RDS.WaitAsyncOperation(operationId);
        }
        public override UAsyncResult GetOperationResult(ulong operationId)
        {
            return RDS.GetOperationResult(operationId);
        }
        public override UAsyncResult GetOperationMessages(ulong operationId)
        {
            return RDS.GetOperationMessages(operationId);
        }
        public override OperationInfo GetOperationState(ulong operationId)
        {
            return RDS.GetOperationState(operationId);
        }
        public override void EndAsyncOperation(ulong operationId)
        {
            RDS.EndAsyncOperation(operationId);
        }
        public override bool AbortAsyncOperation(ulong operationId)
        {
            return RDS.AbortAsyncOperation(operationId);
        }
        #endregion

        #region IAsyncOperationManager members
        //protected override OperationState GetOperationRealState(Guid userGuid, ulong operationId)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}
