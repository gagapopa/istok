using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using COTES.ISTOK;

namespace COTES.ISTOK.ASC
{
    public interface IScheduleRetrivial
    {
        /// <summary>
        /// Возвращает список все расписаний.
        /// </summary>
        /// <returns>Массив всех расписаний</returns>
        Schedule[] GetSchedules();
        /// <summary>
        /// Возвращает расписание по иду
        /// </summary>
        /// <param name="id">Айдишка расписания</param>
        /// <returns>Расписание</returns>
        Schedule GetSchedule(int id);
        /// <summary>
        /// Возвращает расписание по названию
        /// </summary>
        /// <param name="name">Название расписания</param>
        /// <returns>Расписание</returns>
        Schedule GetSchedule(string name);
    }
}
