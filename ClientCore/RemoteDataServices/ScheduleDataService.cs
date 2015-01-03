using COTES.ISTOK.ASC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace COTES.ISTOK.ClientCore
{
    public class ScheduleDataService : AsyncGlobalWorker
    {
        internal ScheduleDataService(Session session)
            : base(session)
        {
            //
        }

        public Schedule[] GetParamsSchedules()
        {
            string opid = "GetParamsSchedules" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetParamsSchedules(session.Uid);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<Schedule[]>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public Schedule GetParamsSchedule(int id)
        {
            string opid = "GetParamsSchedule" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetParamsSchedule(session.Uid, id);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<Schedule>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }
        public Schedule GetParamsSchedule(string name)
        {
            string opid = "GetParamsSchedule" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).GetParamsScheduleByName(session.Uid, name);
                session.CommitDataChanges(res.Changes);
                return res.Result;
            }
            catch (FaultException ex)
            {
                return ExceptionMethod<Schedule>(ex);
            }
            finally
            {
                FreeQManager(opid);
            }
        }

        public void AddParamsSchedule(Schedule schedule)
        {
            string opid = "AddParamsSchedule" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).AddParamsSchedule(session.Uid, schedule);
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

        public void UpdateParamsSchedule(Schedule schedule)
        {
            string opid = "UpdateParamsSchedule" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).UpdateParamsSchedule(session.Uid, schedule);
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

        public void DeleteSchedule(int id)
        {
            string opid = "DeleteSchedule" + Guid.NewGuid().ToString();
            try
            {
                var res = AllocQManager(opid).DeleteParamsSchedule(session.Uid, id);
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
