using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace COTES.ISTOK.ClientCore
{
    public class IntervalDataService : AsyncGlobalWorker
    {
        internal IntervalDataService(Session session)
            : base(session)
        {
            //
        }

        public IntervalDescription[] GetStandardsIntervals()
        {
            string opid = "GetStandardsIntervals" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetStandardsIntervals(session.Uid);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<IntervalDescription[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public void RemoveStandardIntervals(IntervalDescription[] intervalsToRemove)
        {
            string opid = "RemoveStandardIntervals" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).RemoveStandardIntervals(session.Uid, intervalsToRemove);
                session.CommitDataChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod<Object>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public void SaveStandardIntervals(IntervalDescription[] modifiedIntervals)
        {
            string opid = "SaveStandardIntervals" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).SaveStandardIntervals(session.Uid, modifiedIntervals);
                session.CommitDataChanges(res.Changes);
            }
            catch (FaultException ex)
            {
                ExceptionMethod<Object>(ex);
            }
            catch (Exception ex2)
            {
            	Console.WriteLine(ex2.Message);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
    }
}
