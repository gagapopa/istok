using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace COTES.ISTOK.ClientCore
{
    public class OnlineDataService : AsyncGlobalWorker
    {
        IGlobalQueryManager qMan = null;

        internal OnlineDataService(Session session)
            : base(session)
        {
            //
        }

        public int RegisterClient(ParamValueItemWithID[] parameters)
        {
            string opid = "RegisterClient" + Guid.NewGuid().ToString();
            try
            {
                if (qMan == null) qMan = AllocQManager(opid);
                var x = qMan.Test(null);
                var res = qMan.RegisterClient(session.Uid, parameters);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<int>(ex);
            }
            finally
            {
                qMan = null;
                FreeQManager(opid);
            }
        }
        public void UnRegisterClient(int transactionId)
        {
            string opid = "UnRegisterClient" + Guid.NewGuid().ToString();
            try
            {
                if (qMan == null) qMan = AllocQManager(opid);
                var res = qMan.UnRegisterClient(session.Uid, transactionId);
                session.CommitDataChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod<Object>(ex);
            }
            finally
            {
                qMan = null;
                FreeQManager(opid);
            }
        }
        public ParamValueItemWithID[] GetValuesFromBank(int transactionId)
        {
            string opid = "GetValuesFromBank" + Guid.NewGuid().ToString();
            try
            {
                if (qMan == null)
                    //throw new Exception("Global connection failed");
                    qMan = AllocQManager(opid);
                var res = qMan.GetValuesFromBank(session.Uid, transactionId);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ParamValueItemWithID[]>(ex);
            }
            finally
            {
                qMan = null;
                FreeQManager(opid);
            }
        }
    }
}
