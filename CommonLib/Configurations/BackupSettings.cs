using System;

namespace COTES.ISTOK
{
    public enum BackUpPeriod
    {
        Day,
        Week,
        Month
    }
    public enum BackUpType
    {
        Differential,
        Full
    }

    public static class BackUpTypeFormatter
    {
        public static string Format(BackUpType type)
        {
            switch (type)
            {
                case BackUpType.Differential:
                    return "Дифференциальный";
                case BackUpType.Full:
                    return "Полный";
            }

            return "хз";
        }

        public static BackUpType Format(string type)
        {
            switch (type.ToLower())
            {
                case "дифференциальный":
                    return BackUpType.Differential;
                case "полный":
                    return BackUpType.Full;
            }

            return BackUpType.Full;
        }
    }

    public static class BackUpPeriodFormatter
    {
        public static string Format(BackUpPeriod period)
        {
            switch (period)
            {
                case BackUpPeriod.Day:
                    return "Ежедневно";
                case BackUpPeriod.Week:
                    return "Неделя";
                case BackUpPeriod.Month:
                    return "Месяц";
            }

            return "хз";
        }

        public static BackUpPeriod Format(string period)
        {
            switch (period.ToLower())
            {
                case "ежедневно":
                    return BackUpPeriod.Day;
                case "неделя":
                    return BackUpPeriod.Week;
                case "месяц":
                    return BackUpPeriod.Month;
            }

            return BackUpPeriod.Month;
        }
    }

    public static class DayOfWeekFormatter
    {
        public static DayOfWeek Format(string dow)
        {
            switch (dow.ToLower())
            {
                case "понедельник":
                    return DayOfWeek.Monday;
                case "вторник":
                    return DayOfWeek.Tuesday;
                case "среда":
                    return DayOfWeek.Wednesday;
                case "четверг":
                    return DayOfWeek.Thursday;
                case "пятница":
                    return DayOfWeek.Friday;
                case "суббота":
                    return DayOfWeek.Saturday;
                case "воскресенье":
                    return DayOfWeek.Sunday;
            }

            return DayOfWeek.Sunday;
        }

        public static string Format(DayOfWeek dow)
        {
            switch (dow)
            {
                case DayOfWeek.Monday:
                    return "Понедельник";
                case DayOfWeek.Tuesday:
                    return "Вторник";
                case DayOfWeek.Wednesday:
                    return "Среда";
                case DayOfWeek.Thursday:
                    return "Четверг";
                case DayOfWeek.Friday:
                    return "Пятница";
                case DayOfWeek.Saturday:
                    return "Суббота";
                case DayOfWeek.Sunday:
                    return "Воскресенье";
            }

            return "хз";
        }
    }

    /// <summary>
    /// Класс с настройками резервирования
    /// </summary>
    [Serializable]
    public class BackUpSettings
    {
        private string name;
        private string description;
        private string filename;
        private string database;

        private bool inUse;

        private DateTime lastBackup;

        private BackUpType type;
        private DayOfWeek dow;
        private BackUpPeriod period;
        private DateTime time;
        private int ttl;
        private int day;

        public BackUpSettings()
        {
            name = "";
            description = "";
            filename = "";
            database = "";
            inUse = false;
            dow = DayOfWeek.Sunday;
            period = BackUpPeriod.Month;
            Time = DateTime.MinValue;
            Day = 1;
            TTL = 5;
            lastBackup = DateTime.MinValue;
            type = BackUpType.Full;
        }

        public BackUpSettings(BackUpType bu_type)
            : this()
        {
            type = bu_type;
        }

        /// <summary>
        /// Имя базы данных
        /// </summary>
        public string DataBase
        {
            get { return database; }
            set { database = value; }
        }

        /// <summary>
        /// Тип резервирования
        /// </summary>
        public BackUpType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Время последнего резервирования
        /// </summary>
        public DateTime LastBackup
        {
            get { return lastBackup; }
            set { lastBackup = value; }
        }

        /// <summary>
        /// Используется ли этот резерв
        /// </summary>
        public bool InUse
        {
            get { return inUse; }
            set { inUse = value; }
        }

        /// <summary>
        /// День месяца, в который стартует бэкап
        /// </summary>
        public int Day
        {
            get { return day; }
            set
            {
                if (value < 1 || value > 31)
                    throw new ArgumentOutOfRangeException("Day must be between 1 and 31");
                day = value;
            }
        }

        /// <summary>
        /// Количество дней, которое резерв считается актуальным
        /// </summary>
        public int TTL
        {
            get { return ttl; }
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException("TTL must be between 0 and 100");
                ttl = value;
            }
        }

        /// <summary>
        /// Время запуска процесса резервирования
        /// </summary>
        public DateTime Time
        {
            get { return time; }
            set
            {
                time = DateTime.MinValue;
                time = new DateTime(time.Year,
                    time.Month,
                    time.Day,
                    value.Hour,
                    value.Minute,
                    value.Second);
            }
        }

        /// <summary>
        /// Название резерва
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Описание резерва
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Имя файла резерва
        /// </summary>
        public string FileName
        {
            get { return filename; }
            set { filename = value; }
        }

        /// <summary>
        /// День недели запуска резервирования
        /// </summary>
        public DayOfWeek DayOfWeek
        {
            get { return dow; }
            set { dow = value; }
        }

        /// <summary>
        /// Период запуска процесса резервирования
        /// </summary>
        public BackUpPeriod Period
        {
            get { return period; }
            set { period = value; }
        }

        public string GetBackupQuery()
        {
            /*
             * WARNING: This code contains the possibility for sql-injection!!!!!
             */
            string str;

            str = "BACKUP DATABASE [" + DataBase + "]";
            str += " TO DISK=N'" + FileName + "'";
            str += " WITH";
            if (type == BackUpType.Differential)
                str += " DIFFERENTIAL,";
            str += " DESCRIPTION=N'" + Description + "'";
            str += ", RETAINDAYS = " + ttl.ToString();
            str += ", NOFORMAT, NOINIT";
            str += ", NAME=N'" + Name + "'";
            str += ", SKIP, NOREWIND, NOUNLOAD,  STATS = 10";

            return str;
        }
    }
}