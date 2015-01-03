using COTES.ISTOK.ASC;
using COTES.ISTOK.Calc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace COTES.ISTOK.ClientCore
{
    public class CalcDataService : AsyncGlobalWorker
    {
        internal CalcDataService(Session session)
            : base(session)
        {
            //
        }

        public ConstsInfo[] GetConsts()
        {
            string opid = "GetConsts" + Guid.NewGuid().ToString();
            try
            {
                var res= AllocQManager(opid).GetConsts(session.Uid);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<ConstsInfo[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public CustomFunctionInfo[] GetCustomFunctions()
        {
            string opid = "GetCustomFunctions" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetCustomFunctions(session.Uid);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<CustomFunctionInfo[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public void SaveConsts(ConstsInfo[] constsInfo)
        {
            string opid = "SaveConsts" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).SaveConsts(session.Uid, constsInfo);
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

        public void RemoveConsts(ConstsInfo[] constsInfo)
        {
            string opid = "RemoveConsts" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).RemoveConsts(session.Uid, constsInfo);
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

        public void SaveCustomFunctions(CustomFunctionInfo[] customFunctionInfo)
        {
            string opid = "SaveCustomFunctions" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).SaveCustomFunctions(session.Uid, customFunctionInfo);
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

        public bool GetRoundRobinAutoStart()
        {
            string opid = "GetRoundRobinAutoStart" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetRoundRobinAutoStart(session.Uid);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<bool>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public int GetLastRoundRobinMessagesCount()
        {
            string opid = "GetLastRoundRobinMessagesCount" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetLastRoundRobinMessagesCount(session.Uid);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<int>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public Message[] GetLastRoundRobinMessages(int start, int count)
        {
            string opid = "GetLastRoundRobinMessages" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetLastRoundRobinMessages(session.Uid, start, count);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<Message[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public void SetRoundRobinAutoStart(bool p)
        {
            string opid = "SetRoundRobinAutoStart" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).SetRoundRobinAutoStart(session.Uid, p);
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

        public RoundRobinInfo GetRoundRobinInfo()
        {
            string opid = "GetRoundRobinInfo" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetRoundRobinInfo(session.Uid);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<RoundRobinInfo>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public bool IsRoundRobinStarted()
        {
            string opid = "IsRoundRobinStarted" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).IsRoundRobinStarted(session.Uid);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<bool>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public void StartRoundRobinCalc()
        {
            string opid = "StartRoundRobinCalc" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).StartRoundRobinCalc(session.Uid);
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

        public void StopRoundRobinCalc()
        {
            string opid = "StopRoundRobinCalc" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).StopRoundRobinCalc(session.Uid);
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

        public FunctionInfo[] GetCalcFunction(RevisionInfo revision)
        {
            string opid = "GetCalcFunction" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetCalcFunction(session.Uid, revision);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<FunctionInfo[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public Message[] CheckFormula(RevisionInfo revision, string formula, KeyValuePair<int, CalcArgumentInfo[]>[] argumentsKey)
        {
            string opid = "CheckFormula" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).CheckFormula(session.Uid, revision, formula, argumentsKey);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<Message[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public ulong BeginCalc(UnitNode[] unitNode, DateTime timeStart, DateTime timeEnd, bool recalcAll)
        {
            string opid = "BeginCalc" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).BeginCalc(session.Uid, (from n in unitNode select n.Idnum).ToArray(), timeStart, timeEnd, recalcAll);
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
    }
}
