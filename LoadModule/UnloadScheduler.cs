using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

using COTES.ISTOK;
using System.Collections;

namespace COTES.ISTOK.Block
{
    /// <summary>
    /// Класс следящий за расписанием выгрузки параметров.
    /// </summary>
    class UnloadScheduler
    {
        /// <summary>
        /// таймеры для выгрузки
        /// </summary>
        private LinkedList<Timer> timers = new LinkedList<Timer>();
        
        /// <summary>
        /// делегат для эвента, смотри нижe
        /// </summary>
        /// <param name="id">
        /// ид расписания
        /// </param>
        public delegate void UnloadDelegate(int id);
        /// <summary>
        /// событие информируеющие подписчика, что
        /// необходимо выгрузить на глобал параметры
        /// которые принадлежат расписанию с идом таким то
        /// </summary>
        public event UnloadDelegate UnloadEvent;

        public string BlockUID
        { set; get; }

        ///// <summary>
        ///// Перечисление расписаний
        ///// </summary>
        //volatile Dictionary<int, UnloadParamsSchedule> dicSchedules = new Dictionary<int, UnloadParamsSchedule>();

        /// <summary>
        /// конструктор
        /// </summary>
        public UnloadScheduler()
        {
            //ReloadScheduleFromDB();
        }

        /// <summary>
        /// перегрузить расписания из бд
        /// </summary>
        public void ReloadScheduleFromDB()
        {
            //LinkedList<UnloadParamsSchedule> lst =
            //    new LinkedList<UnloadParamsSchedule>();
            //try
            //{
            //    var table = NSI.dalManager.GetSchedules();

            //    foreach (DataRow it in table.Rows)
            //        lst.AddFirst(new UnloadParamsSchedule()
            //        {
            //            ID = it.Field<int>(@"id"),
            //            Name = it.Field<string>(@"name"),
            //            Period = TimeSpan.FromTicks(it.Field<long>(@"period"))
            //        });

            //    AcceptNewSchedules(lst);
            //}
            //catch { }
            }

        ///// <summary>
        ///// перестройка под новые расписания,
        ///// возможно еще будет переработано
        ///// </summary>
        ///// <param name="schedules">
        ///// </param>
        //public void AcceptNewSchedules(IEnumerable<UnloadParamsSchedule> schedules)
        //{
        //    Clear();
        //    foreach (var it in schedules)
        //    {
        //        dicSchedules[it.ID] = it;
        //        timers.AddFirst(new Timer(this.TimerCallBack,
        //                                  it.ID,
        //                                  TimeSpan.FromTicks(0),
        //                                  it.Period));
        //    }
        //}

        public void Clear()
        {
            foreach (var it in timers)
                it.Dispose();

            timers.Clear();
            //dicSchedules.Clear();
        }

        /// <summary>
        /// колбек таймеров, переадресуется в евент
        /// </summary>
        /// <param name="id">
        ///     идентификатор расписания
        /// </param>
        private void TimerCallBack(object id)
        {
            if (UnloadEvent != null)
                UnloadEvent((int)id);

            //костыль, очень страшный костыль
            // выпиливается вместе с NSI
            IGlobal global = NSI.conInspector.GlobalServer;

            if (global == null) return;

            try
            {
                //global.DataRequest(BlockUID, (int)id);
            }
            catch { }
        }
    }
}
