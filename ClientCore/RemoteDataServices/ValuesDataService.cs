using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace COTES.ISTOK.ClientCore
{
    public class ValuesDataService : AsyncGlobalWorker
    {
        internal ValuesDataService(Session session)
            : base(session)
        {
            //
        }

        public void LockValues(UnitNode unitNode, DateTime startTime, DateTime endTime)
        {
            string opid = "LockValues" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).LockValues(session.Uid, unitNode.Idnum, startTime, endTime);
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
        public void ReleaseValues(UnitNode unitNode, DateTime startTime, DateTime endTime)
        {
            string opid = "ReleaseValues" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).ReleaseValues(session.Uid, unitNode.Idnum, startTime, endTime);
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

        public void DeleteValues(int parameterId, DateTime[] deletingDateTimes)
        {
            string opid = "DeleteValues" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).DeleteValues(session.Uid, parameterId, deletingDateTimes);
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
        public void DeleteValuesOptimization(int optimizationNodeId, ArgumentsValues[] valueArguments, DateTime time)
        {
            string opid = "DeleteValuesOptimization" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).DeleteValuesOptimization(session.Uid, optimizationNodeId, valueArguments, time);
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

        public void DeleteLoadValues(int unitNodeId, DateTime dateTime)
        {
            string opid = "DeleteLoadValues" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).DeleteLoadValues(session.Uid, unitNodeId, dateTime);
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

        public ulong BeginGetValues(int[] ids,
            DateTime beginTime, DateTime endTime, Interval interval,
            CalcAggregation aggregation, bool useBlockValues)
        {
            string opid = "BeginGetValues" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).BeginGetValues4(session.Uid, ids, beginTime, endTime, interval, aggregation, useBlockValues);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ulong>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public IEnumerable<Package> GetValues(int[] ids,
            DateTime beginTime, DateTime endTime, Interval interval,
            CalcAggregation aggregation, bool useBlockValues)
        {
            ulong op = BeginGetValues(ids, beginTime, endTime, interval, aggregation, useBlockValues);
            return GetOperationMultipleResult<Package>(op);
        }
        public ulong BeginGetValues(int[] ids,
            DateTime beginTime, DateTime endTime, bool useBlockValues)
        {
            string opid = "BeginGetValues" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).BeginGetValues(session.Uid, ids, beginTime, endTime, useBlockValues);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ulong>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public IEnumerable<Package> GetValues(int[] ids,
            DateTime beginTime, DateTime endTime, bool useBlockValues)
        {
            ulong op = BeginGetValues(ids, beginTime, endTime, useBlockValues);
            return GetOperationMultipleResult<Package>(op);
        }
        public ulong BeginGetValues(int parameterId,
            DateTime beginTime, DateTime endTime, Interval interval,
            CalcAggregation aggregation, bool useBlockValues)
        {
            string opid = "BeginGetValues" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).BeginGetValues3(session.Uid, parameterId, beginTime, endTime, interval, aggregation, useBlockValues);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ulong>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public IEnumerable<Package> GetValues(int parameterId,
            DateTime beginTime, DateTime endTime, Interval interval,
            CalcAggregation aggregation, bool useBlockValues)
        {
            ulong op = BeginGetValues(parameterId, beginTime, endTime, interval, aggregation, useBlockValues);
            return GetOperationMultipleResult<Package>(op);
        }

        public ulong BeginGetValues(int parameterId, ArgumentsValues arguments, DateTime startTime, DateTime endTime)
        {
            return BeginGetValues(parameterId, arguments, startTime, endTime, Interval.Zero, CalcAggregation.Nothing);
        }
        public ulong BeginGetValues(int parameterId, ArgumentsValues arguments,
            DateTime startTime, DateTime endTime,
            Interval interval,
            CalcAggregation aggregation)
        {
            string opid = "BeginGetValues" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).BeginGetValues5(session.Uid, parameterId, arguments, startTime, endTime, interval, aggregation);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ulong>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public void SaveValues(Package[] packages)
        {
            string opid = "SaveValues" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).SaveValues(session.Uid, packages);
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
    }
}
