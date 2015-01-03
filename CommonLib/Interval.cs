using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace COTES.ISTOK
{
    /// <summary>
    /// Интервал
    /// </summary>
    [Serializable]
    [DataContract]
    [KnownType(typeof(SecondsInterval))]
    [KnownType(typeof(MonthsInterval))]
    [KnownType(typeof(CompositeInterval))]
    public abstract class Interval : IComparable<Interval>
    {
        #region static stuff
        public static readonly Interval Zero = new SecondsInterval(0);

        public static readonly Interval Second = new SecondsInterval(1);

        public static readonly Interval Minute = new SecondsInterval(60);

        public static readonly Interval Hour = new SecondsInterval(3600);

        public static readonly Interval Day = new SecondsInterval(86400);

        public static readonly Interval Month = new MonthsInterval(1);

        public static readonly Interval Quarter = new MonthsInterval(3);

        public static readonly Interval Year = new MonthsInterval(12);

        public static readonly Interval Infinity = new SecondsInterval(double.PositiveInfinity);

        public static Interval GetInterval(DateTime startTime, DateTime endTime)
        {
            if (startTime.Day == endTime.Day
                && startTime.Hour == endTime.Hour
                && startTime.Minute == endTime.Minute
                && startTime.Second == endTime.Second
                && startTime.Millisecond == endTime.Millisecond)
            {
                return new MonthsInterval((endTime.Year - startTime.Year) * 12 + (endTime.Month - startTime.Month));
            }

            return new SecondsInterval((endTime - startTime).TotalSeconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Строка может представлять два формата:
        /// - целое число задаёт количество секунд в интервале или количество месяцев, если число отрицательное. 
        /// Данный формат представлен для совместимости со старой версией интервалов.
        /// - строка вида 
        /// <br />\[TIMEZONE\]?SHIFT?=CYCLE(-INTERVAL)*;
        /// <br />TIMEZONE = local|UTC[+-][0-9][0-9]
        /// <br />SHIFT = INT;
        /// <br />CYCLE = INT;
        /// <br />INTERVAL = INT;
        /// <br />INT = ([0-9]+[YMdhms])+
        /// <br />где:
        /// INT - обычный интервал, выражающий количество секунд (s), 
        /// минут (m), часов (h), дней (d), месяцев (M) или лет (Y).
        /// SHIFTT - смещение для первого интревала относительно локального времени (l) или UTC (u).
        /// CYCLE - основной интервал.
        /// INTERVAL - вложенный интервал внутри цикла (для неровных повторяющихся интервалов.
        /// Примеры
        /// "=1d" - сутки по локальному времени
        /// "[UTC]=1d" - сутки по UTC
        /// "[UTC+04]=1d" - сутки по москве
        /// "8h=12h" - вахта с 08:00-20:00 и 20:00-08:00
        /// "[UTC+04]=1d-5h-12h-7h" - хитрые интервалы для русного ввода суток/вахт
        /// "[UTC+04]=1d-l8h-12h" - хитрые интервалы для произвольного часового пояса
        /// "=1M-10d-10d" - десятидневки: с 1-ого по 10-ое, с 11-ого по 20-ое и с 21-го до конца месяца
        /// "=1M" - месяц
        /// "[UTC+04]=1M" - месяц по москве
        /// "[UTC+04]2h=1d" - сутки с 2 до 2 по Москве
        /// </remarks>
        /// <param name="intervalString"></param>
        /// <returns></returns>
        public static Interval FromString(String intervalString)
        {
            const String intervalRqegex = @"(?<by_int>^[+-]?[0-9]+$)|^(\[(?<timezone>local|UTC([+-][0-9][0-9])?)\])?(?<shift>([0-9]+[YMdhms])*)=(?<cycle>([0-9]+[YMdhms])+)(?<interval>-([0-9]+[YMdhms])+)*$";

            Regex regex = new Regex(intervalRqegex);

            var match = regex.Match(intervalString);

            if (match.Success)
            {
                if (match.Groups["by_int"].Success)
                {
                    int period = int.Parse(match.Groups["by_int"].Value);
                    if (period < 0)
                    {
                        return new MonthsInterval(-period);
                    }
                    else
                    {
                        return new SecondsInterval(period);
                    }
                }
                else
                {
                    TimeZoneInfo timezone;
                    // get interval timezone
                    if (match.Groups["timezone"].Success)
                    {
                        if (match.Groups["timezone"].Value == "local")
                        {
                            //timezone = TimeZoneInfo.Local;
                            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
                            timezone = (from tx in TimeZoneInfo.GetSystemTimeZones() where tx.BaseUtcOffset == offset select tx).First();
                        }
                        else if (match.Groups["timezone"].Value == "UTC")
                        {
                            timezone = TimeZoneInfo.Utc;
                        }
                        else
                        {
                            int utcOffset = int.Parse(match.Groups["timezone"].Value.Substring("UTC".Length));
                            TimeSpan offset = TimeSpan.FromHours(utcOffset);
                            // TODO:  tx.SupportsDaylightSavingTime  удалить и сделать сереализацию правил перехода зима-лето. И проследить как это обрабатывается на глобале							                         
                            timezone = TimeZoneInfo.GetSystemTimeZones().First(tx => tx.BaseUtcOffset == offset && tx.SupportsDaylightSavingTime == false);
                        }
                    }
                    else
                    {
                        timezone = TimeZoneInfo.Utc;
                    }

                    Interval shiftInterval = null;

                    // get shift
                    if (match.Groups["shift"].Success
                        && !String.IsNullOrEmpty(match.Groups["shift"].Value))
                    {
                        shiftInterval = GetSubInterval(timezone, match.Groups["shift"].Value);
                    }
                    // get cycleInterval
                    var cycleInterval = GetSubInterval(timezone, match.Groups["cycle"].Value);
                    // get subintervals
                    List<Interval> intervalList = new List<Interval>();
                    foreach (Capture item in match.Groups["interval"].Captures)
                    {
                        var interval = GetSubInterval(timezone, item.Value.Substring(1));
                        intervalList.Add(interval);
                    }

                    if (intervalList.Count == 0
                        && shiftInterval == null)
                    {
                        return cycleInterval;
                    }
                    return new CompositeInterval(timezone, shiftInterval, cycleInterval, intervalList.ToArray());
                }
            }
            return Interval.Zero;
        }

        private static Interval GetSubInterval(TimeZoneInfo timeZone, string p)
        {
            Regex regex = new Regex("[0-9]+[YMdhms]");

            var interval = Interval.Zero;
            foreach (Match item in regex.Matches(p))
            {
                String stringValue = item.Value;
                int value = int.Parse(stringValue.Substring(0, stringValue.Length - 1));
                switch (stringValue.Last())
                {
                    case 'Y':
                        interval += new MonthsInterval(timeZone, value * 12);
                        break;
                    case 'M':
                        interval += new MonthsInterval(timeZone, value);
                        break;
                    case 'd':
                        interval += new SecondsInterval(timeZone, value * 86400);
                        break;
                    case 'h':
                        interval += new SecondsInterval(timeZone, value * 3600);
                        break;
                    case 'm':
                        interval += new SecondsInterval(timeZone, value * 60);
                        break;
                    case 's':
                        interval += new SecondsInterval(timeZone, value);
                        break;
                    default:
                        break;
                }
            }
            return interval;
        }
        #endregion

        #region Methods For child classes

        /// <summary>
        /// Вывести информацию о часовом поясе.
        /// Должен использоватся в методах ToString()
        /// </summary>
        /// <returns></returns>
        protected String TimeZoneToString()
        {
            if (TimeZone.Equals(TimeZoneInfo.Local))
            {
                return "[local]";
            }
            else if (TimeZone.Equals(TimeZoneInfo.Utc))
            {
                return String.Empty;
            }
            else
            {
                return String.Format("[UTC{0:+00;-00}]", TimeZone.BaseUtcOffset.Hours);
            }
        }

        /// <summary>
        /// Преобразовывает время в часовой пояс указанный в свойстве TimeZone
        /// </summary>
        /// <remarks>
        /// В зависимости от значения свойства DateTime.Kind у исходного времени,
        /// за исходный часовой пояс принимает UTC или текущий системный пояс.
        /// </remarks>
        /// <param name="lastTime">Исходное время</param>
        /// <returns>Время в часовом поясе TimeZone</returns>
        protected DateTime ToTargetTimeZone(DateTime lastTime)
        {
            DateTime time;
            if (lastTime.Kind == DateTimeKind.Utc)
            {
                time = TimeZoneInfo.ConvertTime(lastTime, TimeZoneInfo.Utc, TimeZone);
            }
            else
            {
                time = TimeZoneInfo.ConvertTime(lastTime, TimeZoneInfo.Local, TimeZone);
            }
            return time;
        }

        /// <summary>
        /// Преобразовывает время из часового пояса указанного в свойстве TimeZone
        /// </summary>
        /// <remarks>
        /// В зависимости от значения свойства DateTime.Kind у исходного времени,
        /// за исходный часовой пояс принимает UTC или текущий системный пояс.
        /// </remarks>
        /// <param name="lastTime">Исходное время</param>
        /// <param name="retTime">Время в часовом поясе TimeZone</param>
        /// <returns>Время в исходном часовом поясе</returns>
        protected DateTime FromTargetTimeZone(DateTime lastTime, DateTime retTime)
        {
            if (lastTime.Kind == DateTimeKind.Utc)
            {
                retTime = TimeZoneInfo.ConvertTime(retTime, TimeZone, TimeZoneInfo.Utc);
            }
            else
            {
                retTime = TimeZoneInfo.ConvertTime(retTime, TimeZone, TimeZoneInfo.Local);
            }
            return retTime;
        }
        #endregion

        #region Interval interface

        /// <summary>
        /// Часовой пояс, используемый для расчёта интервалов.
        /// Играет роль для интервалов больших часовых
        /// </summary>
        [DataMember]
        public TimeZoneInfo TimeZone { get; private set; }

        public Interval(TimeZoneInfo timezone)
        {
            this.TimeZone = timezone;
        }


        /// <summary>
        /// Проверяет пересикаются ли указанные интервалы
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="interval1"></param>
        /// <param name="time2"></param>
        /// <param name="interval2"></param>
        /// <returns></returns>
        public static bool Cross(DateTime time1, Interval interval1, DateTime time2, Interval interval2)
        {
            DateTime period1Start = time1;
            DateTime period1End = interval1.GetNextTime(period1Start);

            DateTime period2Start = time2;
            DateTime period2End = interval2.GetNextTime(period2Start);

            return (period1Start <= period2Start && period2Start <= period1End)
                || (period1Start <= period2End && period2End <= period1End)
                || (period2Start <= period1Start && period1End <= period2End);
        }

        public abstract DateTime GetTime(DateTime time, int times);

        public DateTime GetNextTime(DateTime time)
        {
            var ret = GetTime(time, 1);
            ret = NearestEarlierTime(ret);
            return ret;
        }

        public DateTime GetPrevTime(DateTime time)
        {
            var ret = GetTime(time, -1);
            ret = NearestEarlierTime(ret);
            return ret;
        }

        public abstract DateTime NearestEarlierTime(DateTime lastTime);

        public abstract DateTime NearestLaterTime(DateTime lastTime);

        public abstract int GetQueryValues(DateTime beginTime, DateTime endTime);

        #endregion

        public static bool operator ==(Interval a, Interval b)
        {
            if (Object.ReferenceEquals(a, null)
                && Object.ReferenceEquals(b, null))
                return true;
            if (!Object.ReferenceEquals(a, null)
                && !Object.ReferenceEquals(b, null))
                return ((IComparable<Interval>)a).CompareTo(b) == 0;
            return false;
        }

        public static bool operator !=(Interval a, Interval b)
        {
            if (Object.ReferenceEquals(a, null)
                && Object.ReferenceEquals(b, null))
                return false;
            if (!Object.ReferenceEquals(a, null)
                && !Object.ReferenceEquals(b, null))
                return ((IComparable<Interval>)a).CompareTo(b) != 0;
            return true;
        }

        public static bool operator >(Interval a, Interval b)
        {
            return ((IComparable<Interval>)a).CompareTo(b) > 0;
        }

        public static bool operator <(Interval a, Interval b)
        {
            return ((IComparable<Interval>)a).CompareTo(b) < 0;
        }

        public static bool operator >=(Interval a, Interval b)
        {
            return ((IComparable<Interval>)a).CompareTo(b) >= 0;
        }

        public static bool operator <=(Interval a, Interval b)
        {
            return ((IComparable<Interval>)a).CompareTo(b) <= 0;
        }

        public static Interval operator +(Interval a, Interval b)
        {
            if (a == Interval.Zero)
            {
                return b;
            }
            if (b == Interval.Zero)
            {
                return a;
            }
            if (a is SecondsInterval && b is SecondsInterval)
            {
                return new SecondsInterval(
                    a.TimeZone,
                    (a as SecondsInterval).Seconds + (b as SecondsInterval).Seconds);
            }
            if (a is MonthsInterval && b is MonthsInterval)
            {
                return new MonthsInterval(
                    a.TimeZone,
                    (a as MonthsInterval).Months + (b as MonthsInterval).Months);
            }

            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            Interval other = obj as Interval;
            if (other != null)
            {
                return ((IComparable<Interval>)this).CompareTo(other) == 0;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return TimeZone.GetHashCode();
        }

        #region IComparable<Interval> Members

        int IComparable<Interval>.CompareTo(Interval other)
        {
            Interval thisInterval = this;
            Interval otherInterval = other;

            while (thisInterval is CompositeInterval)
            {
                if ((thisInterval as CompositeInterval).SubIntervalsLength > 0)
                {
                    thisInterval = (thisInterval as CompositeInterval).SubIntervals[0];
                }
                else
                {
                    thisInterval = (thisInterval as CompositeInterval).Cycle;
                }
            }
            while (otherInterval is CompositeInterval)
            {
                if ((otherInterval as CompositeInterval).SubIntervalsLength > 0)
                {
                    otherInterval = (otherInterval as CompositeInterval).SubIntervals[0];
                }
                else
                {
                    otherInterval = (otherInterval as CompositeInterval).Cycle;
                }
            }

            if (thisInterval is SecondsInterval
                && otherInterval is SecondsInterval)
            {
                return (thisInterval as SecondsInterval).Seconds.CompareTo((otherInterval as SecondsInterval).Seconds);
            }
            if (thisInterval is MonthsInterval
                && otherInterval is MonthsInterval)
            {
                return (thisInterval as MonthsInterval).Months.CompareTo((otherInterval as MonthsInterval).Months);
            }
            if (thisInterval is SecondsInterval
                && otherInterval is MonthsInterval)
            {
                return -1;
            }
            if (thisInterval is MonthsInterval
                && otherInterval is SecondsInterval)
            {
                return 1;
            }

            throw new NotImplementedException();
        }

        #endregion

        public virtual Interval Multiply(int times)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Интервал на базе секунд
    /// </summary>
    [Serializable]
    [DataContract]
    public class SecondsInterval : Interval
    {
        [DataMember]
        public double Seconds { get; private set; }

        public SecondsInterval(double seconds)
            : this(TimeZoneInfo.Utc, seconds)
        {

        }

        public SecondsInterval(TimeZoneInfo timezone, double seconds)
            : base(timezone)
        {
            this.Seconds = seconds;
        }

        public override DateTime GetTime(DateTime time, int times)
        {
            if (Seconds == 0)
                return time;

            return time.AddSeconds(Seconds * times);
        }

        public override DateTime NearestEarlierTime(DateTime lastTime)
        {
            if (Seconds == 0)
                return lastTime;

            DateTime StartTime = DateTime.MinValue;
            DateTime time = ToTargetTimeZone(lastTime);

            if (time.Kind == DateTimeKind.Utc)
            {
                StartTime = new DateTime(DateTime.MinValue.Ticks, DateTimeKind.Utc);
            }

            long times = (long)((time - StartTime).TotalSeconds / Seconds);
            DateTime retTime = StartTime.AddSeconds(times * Seconds);

            return FromTargetTimeZone(lastTime, retTime);
        }

        public override DateTime NearestLaterTime(DateTime lastTime)
        {
            if (Seconds == 0)
                return lastTime;

            DateTime StartTime = DateTime.MinValue;
            DateTime time = ToTargetTimeZone(lastTime);

            long times = (long)((time - StartTime).TotalSeconds / Seconds) ;

            if ((time - StartTime).TotalSeconds % Seconds > 0)
                ++times;

            DateTime retTime = StartTime.AddSeconds(times * Seconds);

            return FromTargetTimeZone(lastTime, retTime);
        }

        public override int GetQueryValues(DateTime beginTime, DateTime endTime)
        {
            if (Seconds == 0)
                return 0;

            DateTime time = NearestEarlierTime(beginTime);

            int times = (int)((endTime - time).TotalSeconds / Seconds);

            if ((endTime - time).TotalSeconds % Seconds != 0)
            {
                ++times;
            }
            return times;
        }

        public override Interval Multiply(int times)
        {
            if (times <= 0)
            {
                throw new ArgumentException();
            }
            return new SecondsInterval(TimeZone, Seconds * times);
        }

        public override int GetHashCode()
        {
            return Seconds.GetHashCode();
        }

        public override string ToString()
        {
            const int secondsInDay = 86400;
            const int secondsInHour = 3600;
            const int secondsInMinute = 60;

            if (Seconds == 0)
            {
                return "0";
            }

            String res = TimeZoneToString() + "=";

            int count = (int)Seconds;

            int daysCount = (int)(count / secondsInDay);
            count = count % secondsInDay;

            int hourCount = (int)(count / secondsInHour);
            count = count % secondsInHour;

            int minuteCount = (int)(count / secondsInMinute);

            int secondCount = count % secondsInMinute;

            if (daysCount > 0)
            {
                res += String.Format("{0}d", daysCount);
            }
            if (hourCount > 0)
            {
                res += String.Format("{0}h", hourCount);

            }
            if (minuteCount > 0)
            {
                res += String.Format("{0}m", minuteCount);

            }
            if (secondCount > 0)
            {
                res += String.Format("{0}s", secondCount);

            }
            return res;
        }
    }

    /// <summary>
    /// Интервал на базе месяцев
    /// </summary>
    [Serializable]
    [DataContract]
    public class MonthsInterval : Interval
    {
        [DataMember]
        public int Months { get; private set; }

        public MonthsInterval(int months)
            : this(TimeZoneInfo.Utc, months)
        {

        }

        public MonthsInterval(TimeZoneInfo timezone, int months)
            : base(timezone)
        {
            //this.StartTime = startTime;
            this.Months = months;
        }

        public override DateTime GetTime(DateTime time, int times)
        {
            return time.AddMonths(times * Months);
        }

        public override DateTime NearestEarlierTime(/*DateTime startTime,*/ DateTime lastTime)
        {
            DateTime StartTime = DateTime.MinValue;
            //DateTime time = TimeZoneInfo.ConvertTime(lastTime, TimeZoneInfo.Local, TimeZone);
            DateTime time = ToTargetTimeZone(lastTime);

            int lastMonth = time.Year * 12 + time.Month;
            int startMonth = StartTime.Year * 12 + StartTime.Month;

            int times = (int)((lastMonth - startMonth) / Months);
            DateTime retTime = StartTime.AddMonths(times * Months);

            //return TimeZoneInfo.ConvertTime(retTime, TimeZone, TimeZoneInfo.Local);
            return FromTargetTimeZone(lastTime, retTime);
        }

        public override DateTime NearestLaterTime(/*DateTime startTime,*/ DateTime lastTime)
        {
            DateTime StartTime = DateTime.MinValue;
            //DateTime time = TimeZoneInfo.ConvertTime(lastTime, TimeZoneInfo.Local, TimeZone);
            DateTime time = ToTargetTimeZone(lastTime);

            int lastMonth = time.Year * 12 + time.Month;
            int startMonth = StartTime.Year * 12 + StartTime.Month;

            int times = (int)((lastMonth - startMonth) / Months);

            if ((lastMonth - startMonth) % Months > 0 || time.Day > 1 || time.TimeOfDay > TimeSpan.Zero)
                ++times;

            DateTime retTime = StartTime.AddMonths(times * Months);

            //return TimeZoneInfo.ConvertTime(retTime, TimeZone, TimeZoneInfo.Local);
            return FromTargetTimeZone(lastTime, retTime);
        }

        public override int GetQueryValues(/*DateTime startTime,*/ DateTime beginTime, DateTime endTime)
        {
            DateTime time = NearestEarlierTime(/*startTime,*/ beginTime);

            int startMonth = time.Year * 12 + time.Month;
            int lastMonth = endTime.Year * 12 + endTime.Month;

            int times = (int)((lastMonth - startMonth) / Months);

            if ((lastMonth - startMonth) % Months != 0)
            {
                ++times;
            }
            return times;
        }

        public override Interval Multiply(int times)
        {
            if (times <= 0)
            {
                throw new ArgumentException();
            }
            return new MonthsInterval(TimeZone, Months * times);
        }

        public override int GetHashCode()
        {
            return Months.GetHashCode();
        }

        public override string ToString()
        {
            const int monthInYear = 12;

            String res = TimeZoneToString() + "=";

            int yearCount = (int)(Months / monthInYear);

            int monthCount = Months % monthInYear;

            if (yearCount > 0)
            {
                res += String.Format("{0}Y", yearCount);
            }
            if (monthCount > 0)
            {
                res += String.Format("{0}M", monthCount);

            }
            return res;
        }
    }

    /// <summary>
    /// Составной интервал со смещением и подинтервалами 
    /// для представления сложных, не ровных интервалов
    /// </summary>
    [Serializable]
    [DataContract]
    public class CompositeInterval : Interval
    {
        [DataMember]
        public int Multiplex { get; private set; }

        /// <summary>
        /// Смещение интервала.
        /// </summary>
        /// <remarks>
        /// Учитывается в методе NearestEarlierTime добавлением смещения в результату метода для свойства Cycle
        /// </remarks>
        [DataMember]
        public Interval Shift { get; private set; }

        /// <summary>
        /// Основной интервал.
        /// Если SubIntervals не пустой - интервал цикла, включающего SubIntervalsLength интервалов.
        /// Иначе просто интервал, который будет сммещатся на Shift.
        /// </summary>
        [DataMember]
        public Interval Cycle { get; private set; }

        /// <summary>
        /// Подинтервалы, которые представляют неровные интервалы.
        /// Сумма всех интервалов должна быть меньше или равна Cycle.
        /// </summary>
        [DataMember]
        public Interval[] SubIntervals { get; private set; }

        /// <summary>
        /// Количество подинтервалов в цикле.
        /// <br />Если SubIntervals пустое, равно 0.
        /// <br />Если сумма SubIntervals меньше Cycle, то равно SubIntervals.Length + 1.
        /// Последний подинтервал выравнивает время с Cycle.
        /// <br />Если сумма SubIntervals равна Cycle, то равно SubIntervals.Length.
        /// </summary>
        [DataMember]
        public int SubIntervalsLength { get; private set; }

        public CompositeInterval(TimeZoneInfo timezone, Interval shift, Interval cycle, params Interval[] subIntervals)
            : base(timezone)
        {
            this.Shift = shift;
            this.Cycle = cycle;
            this.Multiplex = 1;
            SubIntervals = new List<Interval>(subIntervals ?? new Interval[] { }).ToArray();

            SubIntervalsLength = SubIntervals.Length;
            if (SubIntervals.Length > 0)
            {
                var interval = Interval.Zero;
                foreach (var item in SubIntervals)
                {
                    interval += item;
                }
                if (interval < Cycle)
                {
                    ++SubIntervalsLength;
                }
            }
        }

        private CompositeInterval(TimeZoneInfo timeZone, Interval shift, Interval cycle, Interval[] subIntervals, int times)
            : this(timeZone, shift, cycle, subIntervals)
        {
            this.Multiplex = times;
        }

        public override DateTime GetTime(DateTime time, int times)
        {
            times *= Multiplex;

            // get cycle start for startTime
            DateTime cycleTime = Cycle.NearestEarlierTime(time);

            // correct on shift
            if (Shift != null)
            {
                //cycleTime = Shift.GetNextTime(cycleTime);
                cycleTime = Shift.GetTime(cycleTime, 1);
                if (cycleTime > time)
                {
                    //cycleTime = Cycle.GetPrevTime(cycleTime);
                    cycleTime = Shift.GetTime(cycleTime, -1);
                }
            }

            // increase times for subCycles to get nearestTime for time
            DateTime lastTime = cycleTime;

            for (int i = 0; i < SubIntervals.Length; i++)
            {
                //DateTime tmp = SubIntervals[i].GetNextTime(lastTime);
                DateTime tmp = SubIntervals[i].GetTime(lastTime, 1);
                if (time >= tmp)
                {
                    lastTime = tmp;
                    ++times;
                }
                else
                {
                    break;
                }
            }

            // move time in whoe cycles
            int wholeCycles;//= (int)(times / SubIntervalsLength);
            int intervalsInsideCycle;// = times % SubIntervalsLength;

            if (SubIntervalsLength == 0)
            {
                wholeCycles = times;
                intervalsInsideCycle = 0;
            }
            else
            {
                wholeCycles = (int)(times / SubIntervalsLength);
                intervalsInsideCycle = times % SubIntervalsLength;
            }

            // intervalsInsideCycle always >= 0
            if (intervalsInsideCycle < 0)
            {
                --wholeCycles;
                intervalsInsideCycle += SubIntervalsLength;
            }

            DateTime retTime = Cycle.GetTime(cycleTime, wholeCycles);

            for (int i = 0; i < intervalsInsideCycle; i++)
            {
                //retTime = SubIntervals[i].GetNextTime(retTime);
                retTime = SubIntervals[i].GetTime(retTime, 1);
            }
            return retTime;
        }

        public override DateTime NearestEarlierTime(DateTime time)
        {
            // get cycle start for startTime
            DateTime cycleTime = Cycle.NearestEarlierTime(time);

            // correct on shift
            if (Shift != null)
            {
                //cycleTime = Shift.GetNextTime(cycleTime);
                cycleTime = Shift.GetTime(cycleTime, 1);
                if (cycleTime > time)
                {
                    //cycleTime = Cycle.GetPrevTime(cycleTime);
                    cycleTime = Cycle.GetTime(cycleTime, -1);
                }
            }

            // increase times for subCycles to get nearestTime for time
            DateTime lastTime = cycleTime;

            for (int i = 0; i < SubIntervals.Length; i++)
            {
                DateTime tmp = SubIntervals[i].GetTime(lastTime, 1);
                if (time >= tmp)
                {
                    lastTime = tmp;
                }
                else
                {
                    break;
                }
            }
            return lastTime;
        }

        public override DateTime NearestLaterTime(DateTime time)
        {
            // get cycle start for startTime
            DateTime cycleTime = Cycle.NearestEarlierTime(time);

            // increase times for subCycles to get nearestTime for time
            DateTime lastTime = cycleTime;

            for (int i = 0; time > lastTime && i < SubIntervalsLength; i++)
            {
                lastTime = SubIntervals[i].GetNextTime(lastTime);
            }
            return lastTime;
        }

        public override int GetQueryValues(DateTime beginTime, DateTime endTime)
        {
            int subTimes = 0;
            int times = Cycle.GetQueryValues(beginTime, endTime);

            DateTime lastTime = Cycle.NearestEarlierTime(beginTime);

            if (Shift != null)
            {
                lastTime = Shift.GetNextTime(lastTime);
                if (lastTime >= beginTime)
                    --times;
            }

            for (int i = 0; beginTime > lastTime && i < SubIntervals.Length; i++)
            {
                --subTimes;
                lastTime = SubIntervals[i].GetNextTime(lastTime);
            }

            lastTime = Cycle.NearestEarlierTime(endTime);

            if (Shift != null)
            {
                lastTime = Shift.GetNextTime(lastTime);
                if (lastTime > endTime)
                    ++times;
            }

            if (lastTime < endTime)
            {
                --times;

                for (int i = 0; endTime > lastTime && i < SubIntervals.Length; i++)
                {
                    lastTime = SubIntervals[i].GetNextTime(lastTime);
                    ++subTimes;
                }
            }

            if (SubIntervalsLength == 0)
                return times;

            return times * SubIntervalsLength + subTimes;
        }

        public override Interval Multiply(int times)
        {
            return new CompositeInterval(TimeZone, Shift, Cycle, SubIntervals, times);
        }

        public override int GetHashCode()
        {
            int hash = Cycle.GetHashCode();

            if (Shift != null)
            {
                hash += Shift.GetHashCode();
            }

            foreach (var item in SubIntervals)
            {
                hash += item.GetHashCode();
            }

            return hash;
        }

        public override string ToString()
        {
            String sub;
            String res = TimeZoneToString();

            if (Shift != null)
            {
                sub = Shift.ToString();
                res += sub.Substring(sub.IndexOf('=') + 1);
            }
            sub = Cycle.ToString();
            res += sub.Substring(sub.IndexOf('='));

            foreach (var item in SubIntervals)
            {
                sub = item.ToString();
                res += "-" + sub.Substring(sub.IndexOf('=') + 1);
            }
            return res;
        }
    }
}
