using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using COTES.ISTOK.ASC.TypeConverters;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Класс настройки диагностики
    /// </summary>
    public class AdminUISettings : ICloneable
    {
        private int timeoutNorm;
        //private int timeoutFull;
        //private int timeoutSelect;
        //private int fullPeriod;
        private int updateInterval;

        public AdminUISettings()
        {
            timeoutNorm = 2000;
            UpdateData = true;
            UpdateTabPage = true;
            //timeoutFull = 5000;
            //timeoutSelect = 1000;
            //fullPeriod = 5;
            updateInterval = 5000;
        }

        [DisplayName("Время проверки соединения")]
        [Description("Максимальное время проверки соединения (мс).")]
        [Category("Таймаут")]
        [DefaultValue(2000)]
        public int TimeoutNorm
        {
            get { return timeoutNorm; }
            set { timeoutNorm = value; }
        }
        [DisplayName("Обновлять текущий узел")]
        [Description("Указывает, будут ли автоматически отображатся обновленные данные выделенного узла.")]
        [Category("Обновление")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool UpdateTabPage { get; set; }
        [DisplayName("Обновлять данные")]
        [Description("Указывает, будут ли автоматически обновляться данные узла при проверке связи.")]
        [Category("Обновление")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool UpdateData { get; set; }
        //[DisplayName("Проверка связи")]
        //[Description("Допустимая задержка при периодической проверке связи (мс).")]
        //[Category("Таймаут")]
        //[DefaultValue(5000)]
        //public int TimeoutFull
        //{
        //    get { return timeoutFull; }
        //    set { timeoutFull = value; }
        //}
        //[DisplayName("Выбор узла")]
        //[Description("Допустимая задержка при выборе элемента в дереве (мс).")]
        //[Category("Таймаут")]
        //[DefaultValue(1000)]
        //public int TimeoutSelect
        //{
        //    get { return timeoutSelect; }
        //    set { timeoutSelect = value; }
        //}
        //[DisplayName("Полная проверка соединения")]
        //[Description("Периодичность полной проверки узлов. Каждая n-я проверка будет полной.")]
        //[Category("Интервал")]
        //[DefaultValue(5)]
        //public int FullPeriod
        //{
        //    get { return fullPeriod; }
        //    set { fullPeriod = value; }
        //}
        [DisplayName("Интервал проверки соединения")]
        [Description("Период времени между запусками проверки доступности узлов (мс).")]
        [Category("Интервал")]
        [DefaultValue(5000)]
        public int UpdateInterval
        {
            get { return updateInterval; }
            set { updateInterval = value; }
        }

        #region ICloneable Members

        public object Clone()
        {
            AdminUISettings n = new AdminUISettings();

            //n.FullPeriod = this.FullPeriod;
            //n.TimeoutFull = this.TimeoutFull;
            n.TimeoutNorm = this.TimeoutNorm;
            //n.TimeoutSelect = this.TimeoutSelect;
            n.UpdateInterval = this.UpdateInterval;
            n.UpdateData = this.UpdateData;
            n.UpdateTabPage = this.UpdateTabPage;

            return n;
        }

        #endregion
    }
}
