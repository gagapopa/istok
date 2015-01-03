using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NLog;

namespace COTES.ISTOK.Assignment
{
    class Scheduler
    {
        Logger log = LogManager.GetCurrentClassLogger();

        Timer timer;
        public ScheduleManager ScheduleManager { get; set; }
        public IUnitManager UnitManager { get; set; }
        public BlockProxy BlockProxy { get; set; }
        public ISecurityManager Security { get; set; }

        const int timerPeriod = 1000;

        /// <summary>
        /// Минимальное время между повторениями одной и той же задачи (в минутах).
        /// </summary>
        public double MinRecallTime { get; set; }

        public Scheduler()
        {

#if DEBUG
            MinRecallTime = 0.5;
#else
            MinRecallTime = 10;
#endif
            timer = new Timer(new TimerCallback(ScheduleProcessing));
        }

        public void Start()
        {
            timer.Change(1000, timerPeriod);
        }
        public void Stop()
        {
            timer.Change(Timeout.Infinite, timerPeriod);
        }

        Dictionary<int, Schedule> manualScheduleDictionary = new Dictionary<int, Schedule>();

        public void RegisterSchedule(Schedule schedule, AsyncDelegate action)
        {
            manualScheduleDictionary[schedule.Id] = schedule;

            schedule.ActionList.Add(action);
        }

        public void UnregisterSchedule(Schedule schedule, AsyncDelegate action)
        {
            schedule.ActionList.Remove(action);
        }

        private void ScheduleProcessing(object state)
        {
            Schedule[] arrSchedules;
            DateTime now = DateTime.Now;

            try
            {
                arrSchedules = ScheduleManager.GetParameterSchedules(new OperationState(Security.InternalSession));
                foreach (var schedule in arrSchedules.Union(manualScheduleDictionary.Values))
                {
                    if (schedule.Rule == null) continue;
                    if (now.Subtract(schedule.LastCall).TotalMinutes < MinRecallTime) continue;
                    if (schedule.Rule.CheckTime(now, schedule.LastCall))
                    {
                        schedule.LastCall = now;
                        foreach (var action in schedule.ActionList)
                        {
                            //if (schedule.Action != null)
                            if (action != null)
                            {
                                GlobalQueryManager.globSvcManager.asyncOperation.BeginAsyncOperation(
                                    Security.InternalSession, true, action, schedule);
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                log.ErrorException("", ex);
            }
        }

        //        private void DataRequest(string uid, int schedule_id)
        //        {
        //            var parametrs = UnitManager.GetParametersForSchedule(new OperationState(), uid, schedule_id);

        //#if DEBUG
        //            Console.WriteLine("DataRequest");
        //#endif
        //        }
    }
}
