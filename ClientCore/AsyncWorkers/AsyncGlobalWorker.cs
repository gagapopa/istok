using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using COTES.ISTOK.DiagnosticsInfo;

namespace COTES.ISTOK.ClientCore
{
    [KnownType(typeof(UTypeNode))]
    public class AsyncGlobalWorker : AsyncWorker, ITestConnection<object>
    {
        //protected IGlobalQueryManager qManager;
        protected Session session;

        //internal AsyncGlobalWorker(IGlobalQueryManager qManager)
        //{
        //    this.qManager = qManager;
        //}
        internal AsyncGlobalWorker(Session session)
        {
            this.session = session;
        }

        int qManagerCount = 0;
        //protected IGlobalQueryManager AllocQManager()
        //{
        //    //Interlocked.Increment(ref qManagerCount); //qManagerCount++;
        //    return session.AllocQManager();
        //}
        //protected void FreeQManager<T>(T task)
        //{
        //    FreeQManager();
        //}
        //protected void FreeQManager()
        //{
        //    //var cnt = Interlocked.Decrement(ref qManagerCount);
        //    //if (cnt == 0)
        //    //    session.FreeQManager();
        //    //else
        //    //    if (cnt < 0)
        //    //        Interlocked.Exchange(ref qManagerCount, 0);
        //    session.FreeQManager();
        //}

        protected IGlobalQueryManager AllocQManager(string opid)
        {
            return session.AllocQManager(opid);
        }
         protected IDiagnostics AllocDiagManager(string opid)
        {
            return session.AllocDiagManager(opid);
        }
        protected void FreeQManager(string opid)
        {
            session.FreeQManager(opid);
        }

        #region AsyncOperations
        public override void WaitAsyncOperation(ulong operationId)
        {
            string opid = "WaitAsyncOperation" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).WaitAsyncOperation(session.Uid, operationId);
                session.CommitDataChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public override UAsyncResult GetOperationResult(ulong operationId)
        {
            string opid = "GetOperationResult" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetOperationResult(session.Uid, operationId);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<UAsyncResult>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public override UAsyncResult GetOperationMessages(ulong operationId)
        {
            string opid = "GetOperationMessages" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetOperationMessages(session.Uid, operationId);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<UAsyncResult>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public override OperationInfo GetOperationState(ulong operationId)
        {
            string opid = "GetOperationState" + Guid.NewGuid().ToString();
            try
            {
                var qman = AllocQManager(opid);
                var res = qman.GetOperationState(session.Uid, operationId);

                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<OperationInfo>(ex);
            }
#if DEBUG
            catch (Exception ex)
            {
                throw new Exception("Channel communication problem. WTF?", ex);
            }
#endif
            finally
            {
                FreeQManager(opid);
            }
        }
        public override void EndAsyncOperation(ulong operationId)
        {
            string opid = "EndAsyncOperation" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).EndAsyncOperation(session.Uid, operationId);
                session.CommitDataChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public override bool AbortAsyncOperation(ulong operationId)
        {
            string opid = "AbortAsyncOperation" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).AbortOperation(session.Uid, operationId);
                session.CommitDataChanges(res.Changes);
                return true;
            }
            catch (FaultException ex)
            {
                ExceptionMethod(ex);
                return false;
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        #endregion

        #region IAsyncOperationManager members
        //protected override OperationState GetOperationRealState( ulong operationId)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion

        #region ITestConnection<object> Members

        public bool Test(object arg)
        {
            return true;
        }

        #endregion
    }
}
