using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace COTES.ISTOK
{
    /// <summary>
    /// Класс для записи правила расписания
    /// </summary>
    [Serializable]
    public class ScheduleReg
    {
        BackUpPeriod period;
        DayOfWeek dayOfWeek;
        int day;
        DateTime time;

        /// <summary>
        /// Создать новый экземпляр правила
        /// </summary>
        /// <param name="period"></param>
        /// <param name="dayOfWeek"></param>
        /// <param name="day"></param>
        /// <param name="time"></param>
        public ScheduleReg(BackUpPeriod period, DayOfWeek dayOfWeek, int day, DateTime time)
        {
            this.period = period;
            this.dayOfWeek = dayOfWeek;
            this.day = day;
            this.time = time;
            InitRepeats();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reg"></param>
        public ScheduleReg(String reg)
        {
            String[] split = reg.Split(' ', '\t', '\n');

            if (split.Length > 6)
            {
                period = (BackUpPeriod)int.Parse(split[0]);
                dayOfWeek = (DayOfWeek)int.Parse(split[1]);
                day = int.Parse(split[2]);
                time = DateTime.Parse(split[3]);
                int b = int.Parse(split[4]);
                Repeat = b != 0;
                repeatEach = uint.Parse(split[5]);
                repeatFor = uint.Parse(split[6]);
            }
        }

        public override string ToString()
        {
            String format = "{0} {1} {2} {3} {4} {5} {6}";
            return String.Format(format, (int)period, (int)dayOfWeek, day, time.ToString("HH:mm")/*time.ToShortTimeString()*/, Repeat ? 1 : 0, RepeatEach, RepeatFor);
        }

        /// <summary>
        /// Период (День, неделя, месяц)
        /// </summary>
        public BackUpPeriod Period
        {
            get { return period; }
        }

        /// <summary>
        /// День недели
        /// </summary>
        public DayOfWeek DayOfWeek
        {
            get { return dayOfWeek; }
        }

        /// <summary>
        /// День в месяце
        /// </summary>
        public int Day
        {
            get
            {
                if (day < 1 || day > 31) day = 1;
                return day;
            }
        }

        /// <summary>
        /// Время в течение дня
        /// </summary>
        public DateTime Time
        {
            get { return time; }
        }

        #region Повторения
        private void InitRepeats()
        {
            repeatEach = 60;
            repeatFor = 60;
        }
        /// <summary>
        /// Хранит количество секунд, через которое требуется повторить задачу
        /// </summary>
        protected uint repeatEach;
        /// <summary>
        /// Хранит количество секунд, в течение которого задача будет повторяться
        /// </summary>
        protected uint repeatFor;

        /// <summary>
        /// Флаг повторения задачи
        /// </summary>
        public bool Repeat { get; set; }

        /// <summary>
        /// Указывает количество секунд, через которое требуется повторить задачу
        /// </summary>
        public uint RepeatEach
        {
            get { return repeatEach; }
            set { repeatEach = value; }
        }

        /// <summary>
        /// Строковое представление интервала повторения задачи
        /// </summary>
        public string RepeatEachString
        {
            get
            {
                return FormatRepeatValue(repeatEach,
                    RepeatInterval.Days | RepeatInterval.Hours | RepeatInterval.Minutes);
            }
            set
            {
                repeatEach = FormatRepeatValue(value,
                    RepeatInterval.Days | RepeatInterval.Hours | RepeatInterval.Minutes);
            }
        }

        /// <summary>
        /// Указывает количество секунд, в течение которого задача будет повторяться
        /// </summary>
        public uint RepeatFor
        {
            get { return repeatFor; }
            set { repeatFor = value; }
        }

        /// <summary>
        /// Строковое представление интервала, в течение которого задача будет повторяться
        /// </summary>
        public string RepeatForString
        {
            get
            {
                return FormatRepeatValue(repeatFor,
                    RepeatInterval.Days | RepeatInterval.Hours | RepeatInterval.Minutes);
            }
            set
            {
                repeatFor = FormatRepeatValue(value,
                    RepeatInterval.Days | RepeatInterval.Hours | RepeatInterval.Minutes);
            }
        }

        private string FormatRepeatValue(uint value, RepeatInterval interval)
        {
            double val = (double)value;
            double tmp;

            if ((interval & RepeatInterval.Days) == RepeatInterval.Days)
            {
                tmp = Math.Truncate(val / 86400);
                if (tmp > 0) return string.Format("{0} д.", tmp.ToString("F0"));
            }
            if ((interval & RepeatInterval.Hours) == RepeatInterval.Hours)
            {
                tmp = Math.Truncate(val / 3600);
                if (tmp > 0) return string.Format("{0} ч.", tmp.ToString("F0"));
            }
            if ((interval & RepeatInterval.Minutes) == RepeatInterval.Minutes)
            {
                tmp = Math.Truncate(val / 60);
                if (tmp > 0) return string.Format("{0} мин.", tmp.ToString("F0"));
            }
            if ((interval & RepeatInterval.Seconds) == RepeatInterval.Seconds)
            {
                return string.Format("{0} сек.", val);
            }
            throw new NotSupportedException();
        }
        private uint FormatRepeatValue(string value, RepeatInterval interval)
        {
            List<string> lst = new List<string>();
            StringBuilder sb = new StringBuilder();
            
            if ((interval & RepeatInterval.Days) == RepeatInterval.Days) { if (sb.Length > 0) sb.Append("|"); sb.Append("д."); }
            if ((interval & RepeatInterval.Hours) == RepeatInterval.Hours) { if (sb.Length > 0) sb.Append("|"); sb.Append("ч."); }
            if ((interval & RepeatInterval.Minutes) == RepeatInterval.Minutes) { if (sb.Length > 0) sb.Append("|"); sb.Append("мин."); }
            if ((interval & RepeatInterval.Seconds) == RepeatInterval.Seconds) { if (sb.Length > 0) sb.Append("|"); sb.Append("сек."); }
            if (sb.Length == 0) throw new NotSupportedException();
            sb.Append(")");
            sb.Insert(0, @"(\d+) (");
            Regex regex = new Regex(sb.ToString());
            Match match = regex.Match(value);
            if (match.Success && match.Groups.Count == 3)
            {
                uint tmp = 0;
                if (!uint.TryParse(match.Groups[1].Value, out tmp)) throw new FormatException();
                if (tmp == 0) throw new FormatException();
                switch (match.Groups[2].Value)
                {
                    case "д.": return tmp * 86400;
                    case "ч.": return tmp * 3600;
                    case "мин.": return tmp * 60;
                    case "сек.": return tmp;
                }
            }
            throw new FormatException();
        }

        [Flags]
        enum RepeatInterval
        {
            Seconds = 1,
            Minutes = 2,
            Hours = 4,
            Days = 8
        }
        #endregion

        /// <summary>
        /// Проверить удовлетворяет ли время данному правилу
        /// </summary>
        /// <param name="dateTime">Время</param>
        /// <param name="lastCall">Время последнего вызова</param>
        /// <returns></returns>
        public bool CheckTime(DateTime dateTime, DateTime lastCall)
        {
            bool ret = false;
            int aday = 0;

            switch (period)
            {
                case BackUpPeriod.Month:
                    if (Repeat)
                    {
                        aday = (int)Math.Truncate((double)repeatFor / 86400);
                        if (dateTime.Day > day)
                        {
                            if (dateTime.Day > day + aday) return false;
                            aday = dateTime.Day - day;
                        }
                        else
                            if (dateTime.Day < day)
                            {
                                int year = dateTime.Year;
                                int month = dateTime.Month;
                                if (month == 1) { month = 12; year--; } else month--;
                                int tmp = day + aday - DateTime.DaysInMonth(year, month);
                                if (tmp < 1 || dateTime.Day > tmp) return false;
                                aday = DateTime.DaysInMonth(year, month) - day + dateTime.Day;
                            }
                            else
                                aday = 0;
                    }
                    else
                        if (dateTime.Day != day) return false;
                    goto case BackUpPeriod.Day;
                    break;
                case BackUpPeriod.Week:
                    if (Repeat)
                    {
                        aday = (int)Math.Truncate((double)repeatFor / 86400);
                        if (dateTime.DayOfWeek > dayOfWeek)
                        {
                            if (dateTime.DayOfWeek > dayOfWeek + aday) return false;
                            aday = dateTime.DayOfWeek - dayOfWeek;
                        }
                        else
                            if (dateTime.DayOfWeek < dayOfWeek)
                            {
                                aday = (int)Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Max() - (int)dayOfWeek + (int)dateTime.DayOfWeek;
                            }
                            else
                                aday = 0;
                    }
                    else
                        if (dateTime.DayOfWeek != dayOfWeek) return false;
                    goto case BackUpPeriod.Day;
                    break;
                case BackUpPeriod.Day:
                    uint ctm = (uint)Math.Truncate(dateTime.TimeOfDay.TotalMinutes);
                    uint stm;
                    
                    //if (ctm == stm) return true;
                    if (Repeat)
                    {
                        //if (aday == 0 && ctm == stm) return true;
                        DateTime date = dateTime.Date;
                        if (period == BackUpPeriod.Month || period == BackUpPeriod.Week) date = new DateTime(date.Year, date.Month, Day);
                        date = date.AddMinutes(time.TimeOfDay.TotalMinutes);
                        DateTime stime = date;
                        int tmpday = stime.AddDays(aday).Day;
                        while (date.Day < tmpday) date = date.AddSeconds(repeatEach);//TODO: test weeks!
                        if (date.Day > tmpday) return false;
                        stm = (uint)Math.Truncate(date.TimeOfDay.TotalMinutes);
                        if (ctm == stm) return true;
                        if (ctm < stm && dateTime.Subtract(lastCall).TotalSeconds > repeatFor) return false;
                        if (dateTime.Subtract(lastCall).TotalSeconds < repeatEach) return false;
                        long d;
                        if (ctm < stm) d = (long)ctm + 12 * 60 - stm; //+12 часов
                        else d = (long)ctm - stm;
                        float tmp = (float)d * 60 / repeatEach;
                        if (tmp == Math.Truncate(tmp)) return true;
                    }
                    else
                    {
                        stm = (uint)Math.Truncate(time.TimeOfDay.TotalMinutes);
                        if (ctm == stm) return true;
                    }
                    //ret = Math.Truncate(dateTime.TimeOfDay.TotalMinutes).Equals(Math.Truncate(time.TimeOfDay.TotalMinutes));
                    return false;
            }
            return ret;
        }

        /// <summary>
        /// Проверить входит ли время удовлетворящее данному правилу в интервал времен
        /// </summary>
        /// <param name="beginTime">Начальное время интервала</param>
        /// <param name="endTime">Конечное время интервала</param>
        /// <returns></returns>
        public bool CheckTimes(DateTime beginTime, DateTime endTime)
        {
            bool ret = false;
            switch (period)
            {
                case BackUpPeriod.Month:
                    if ((endTime.Year > beginTime.Year || (endTime.Month - beginTime.Month) > 1)
                        || (beginTime.Day < day && day < endTime.Day)
                        || (day > beginTime.Day && beginTime.Month < endTime.Month)
                        || (day < endTime.Day && beginTime.Month < endTime.Month)
                        || (beginTime.Day == day && Math.Truncate(beginTime.TimeOfDay.TotalMinutes) <= Math.Truncate(time.TimeOfDay.TotalMinutes))
                        || (endTime.Day == day && Math.Truncate(endTime.TimeOfDay.TotalMinutes) >= Math.Truncate(time.TimeOfDay.TotalMinutes))) ret = true;
                    break;
                case BackUpPeriod.Week:
                    if ((endTime - beginTime).TotalDays > 7
                        || (beginTime.DayOfWeek < dayOfWeek && dayOfWeek < endTime.DayOfWeek)
                        || (beginTime.DayOfWeek == dayOfWeek && Math.Truncate(beginTime.TimeOfDay.TotalMinutes) <= Math.Truncate(time.TimeOfDay.TotalMinutes))
                        || (endTime.DayOfWeek == dayOfWeek && Math.Truncate(endTime.TimeOfDay.TotalMinutes) >= Math.Truncate(time.TimeOfDay.TotalMinutes))) ret = true;
                    break;
                case BackUpPeriod.Day:
                    if ((endTime - beginTime).TotalDays > 1
                        || ((Math.Truncate(beginTime.TimeOfDay.TotalMinutes) <= Math.Truncate(time.TimeOfDay.TotalMinutes))
                        && (Math.Truncate(endTime.TimeOfDay.TotalMinutes) >= Math.Truncate(time.TimeOfDay.TotalMinutes)))) ret = true;
                    break;
            }
            return ret;
        }
    }
}
