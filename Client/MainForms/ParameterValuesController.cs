using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;
using System.Threading.Tasks;

namespace COTES.ISTOK.Client
{
    /// <summary>
    /// Класс для хранения старой и новой версии некоего объекта
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class ModifiedPair<T>
    {
        public T Original { get; protected set; }

        public T Current { get; set; }

        public bool Modified { get { return Original == null || Current == null || !Current.Equals(Original); } }

        public ModifiedPair(T original, T current)
        {
            this.Original = original;
            this.Current = current;
        }
    }

    /// <summary>
    /// Реализация контроллера просмотра значений параметра, для значений запрашиваемых с сервера
    /// </summary>
    class ParameterValuesController : IValuesController
    {
        StructureProvider strucProvider;
        ParameterNode parameter;

        DateTime currentStartTime;
        DateTime currentEndTime;

        List<ModifiedPair<ParamValueItem>> valueList = new List<ModifiedPair<ParamValueItem>>();

        public ArgumentsValues Arguments { get; protected set; }

        public ParameterValuesController(StructureProvider strucProvider, ParameterNode parameter, ArgumentsValues args, DateTime startTime)
        {
            //this.interval = Interval.Zero;
            this.strucProvider = strucProvider;
            this.parameter = parameter;
            this.currentStartTime = startTime;
            this.Arguments = args;
            this.Interval = strucProvider.GetParameterInterval(parameter.Idnum);
        }

        public ParameterValuesController(StructureProvider strucProvider, ParameterNode parameter, DateTime startTime)
            : this(strucProvider, parameter, null, startTime)
        { }

        /// <summary>
        /// Форма запускающая вотчеры и показывающая всякие прогрессбары
        /// </summary>
        public BaseAsyncWorkForm AsyncForm { get; set; }

        #region IValuesController Members

        public string Title
        {
            get { return String.Format("{0} - [{1}]", parameter.Text, parameter.Code); }
        }

        //public DateTime StartTime
        //{
        //    get { return rds.GetParameterStartTime(parameter); }
        //}

        protected Interval interval;

        public Interval Interval
        {
            get { return interval; }
            protected set { interval = value; }
        }

        //protected async Task<DateTime> GetStartTime()
        //{
        //    StartTime = await strucProvider.GetParameterStartTime(parameter.Idnum);
        //    return StartTime;
        //}

        protected Task<Interval> GetInterval()
        {
            //Interval = await strucProvider.GetParameterInterval(parameter.Idnum);
            //return Interval;
            return Task.Factory.StartNew(() => strucProvider.GetParameterInterval(parameter.Idnum));
        }

        public bool VariableConsts
        {
            get { return Parameter.Typ == (int)UnitTypeId.ManualParameter && Interval == Interval.Zero; }
        }

        public ParameterNode Parameter
        {
            get { return parameter; }
        }

        static readonly DateTime minDateTimeValue = new DateTime(2000, 01, 01);

        public DateTime MinDateTimeValue { get { return minDateTimeValue; } }

        public DateTime CurrentStartTime
        {
            get
            {
                if (currentStartTime < minDateTimeValue)
                    return DateTime.Now;
                return currentStartTime;
            }
        }

        public DateTime CurrentEndTime
        {
            get { return currentEndTime; }
        }

        public void RetrieveValue(DateTime startTime, DateTime endTime)
        {
            bool locked = Locked;
            if (locked)
                ReleaseValues();
            currentStartTime = startTime;
            currentEndTime = endTime;
            valueList.Clear();
            AsyncOperationWatcher<UAsyncResult> watcher = strucProvider.BeginGetValues(parameter.Idnum, Arguments, startTime, endTime);
            watcher.AddValueRecivedHandler((x) =>
                {
                    if (x != null && x.Packages != null)
                        foreach (var item in x.Packages)
                            ValueRecived(item);
                });
            watcher.AddFinishHandler(OnValuesRetrieved);
            AsyncForm.RunWatcher(watcher);
            if (locked || (Lockeable && LockAlways))
                LockValues();
        }

        void ValueRecived(Package p)
        {
            p.Values.RemoveAll(v => v.Time < CurrentStartTime || v.Time >= CurrentEndTime);
            valueList.AddRange(p.Values.ConvertAll<ModifiedPair<ParamValueItem>>(v =>
                new ModifiedPair<ParamValueItem>(v, v.Clone() as ParamValueItem)));
        }

        public List<ParamValueItem> Values
        {
            get { return valueList.FindAll(p => p.Current != null).ConvertAll<ParamValueItem>(p => p.Current); }
        }

        public bool Modified(ParamValueItem value)
        {
            ModifiedPair<ParamValueItem> valueItemPair = valueList.Find(p => p.Current == value);

            return valueItemPair != null && valueItemPair.Modified;
        }

        public bool HasChanges
        {
            get
            {
                return valueList.Find(p => p.Modified) != null;
            }
        }

        public void ClearValues()
        {
            valueList.RemoveAll(p => p.Original == null);
            valueList.ForEach(p => p.Current = p.Original.Clone() as ParamValueItem);
            OnValuesRetrieved();
        }

        public bool Corrected(ParamValueItem value)
        {
            ModifiedPair<ParamValueItem> valueItemPair = valueList.Find(p => p.Current == value);

            return parameter.Typ == (int)UnitTypeId.TEP && valueItemPair != null && valueItemPair.Current is CorrectedParamValueItem;
        }

        public event EventHandler ValuesRetrieved;

        protected void OnValuesRetrieved()
        {
            if (ValuesRetrieved != null)
                ValuesRetrieved(this, EventArgs.Empty);
        }

        public bool LockAlways { get { return true; } }

        public bool Lockeable
        {
            get
            {
                return (Parameter.Typ == (int)UnitTypeId.ManualParameter || Parameter.Typ == (int)UnitTypeId.TEP)
                    && strucProvider.CheckAccess(Parameter, Privileges.Execute);
            }
        }

        private bool locked;
        public void LockValues()
        {
            strucProvider.LockValues(parameter, CurrentStartTime, CurrentEndTime);
            locked = true;
            OnLockChanged();
        }

        public void ReleaseValues()
        {
            strucProvider.ReleaseValues(parameter, CurrentStartTime, CurrentEndTime);
            locked = false;
            OnLockChanged();
        }

        public bool Locked
        {
            get { return locked; }
        }

        public List<DateTime> LockedTimes
        {
            get { return new List<DateTime>(); }
        }

        public event EventHandler LockChanged;

        private void OnLockChanged()
        {
            if (LockChanged != null)
                LockChanged(this, EventArgs.Empty);
        }

        public bool Editable
        {
            get
            {
                return parameter.Typ == (int)UnitTypeId.ManualParameter
                    && strucProvider.CheckAccess(parameter, Privileges.Execute);
            }
        }

        public bool TimeEditable
        {
            get { return parameter.Typ == (int)UnitTypeId.ManualParameter && Interval == Interval.Zero; }
        }

        public void SetValue(ParamValueItem value, ParamValueItem originalValue)
        {
            if (originalValue == null)
            {
                valueList.Add(new ModifiedPair<ParamValueItem>(null, value));
            }
            else
            {
                ModifiedPair<ParamValueItem> valueItemPair = valueList.Find(p => p.Current == originalValue);
                if (valueItemPair != null)
                    valueItemPair.Current = value;
            }
        }

        public void DeleteValue(ParamValueItem value)
        {
            ModifiedPair<ParamValueItem> valueItemPair = valueList.Find(p => p.Current == value);
            if (valueItemPair != null)
            {
                valueItemPair.Current = null;
            }
        }

        public bool Correctable
        {
            get
            {
                return HasOriginalValue && strucProvider.CheckAccess(parameter, Privileges.Execute);
            }
        }

        public bool HasOriginalValue
        {
            get { return parameter.Typ == (int)UnitTypeId.TEP; }
        }

        public ParamValueItem Correct(ParamValueItem value)
        {
            ModifiedPair<ParamValueItem> valueItemPair = valueList.Find(p => p.Current == value);

            if (valueItemPair != null)
            {
                valueItemPair.Current = new CorrectedParamValueItem(value);
                return valueItemPair.Current;
            }
            return null;
        }

        public ParamValueItem Decorrect(ParamValueItem value)
        {
            ModifiedPair<ParamValueItem> valueItemPair = valueList.Find(p => p.Current == value);

            if (valueItemPair != null)
            {
                ParamValueItem valueItem = valueItemPair.Original.Clone() as ParamValueItem;
                if (valueItem is CorrectedParamValueItem)
                    (valueItem as CorrectedParamValueItem).Value = double.NaN;

                valueItemPair.Current = valueItem;
                return valueItemPair.Current;
            }
            return null;
        }

        public void Save()
        {
            //AsyncOperationWatcher watcher = null;
            // сначала берем отредактированные значения
            List<ParamValueItem> editValues = valueList.FindAll(
                    p => p.Current != null && p.Original != null && p.Modified
                ).ConvertAll<ParamValueItem>(
                    p => p.Current.Time != p.Original.Time ? 
                    //new CorrectedParamValueItem(p.Original.Time, p.Current.Time, p.Current.Quality, p.Current.Value) : 
                    new CorrectedParamValueItem(p.Original, p.Current) : 
                    p.Current
                );
            // затем добавленные
            editValues.AddRange(valueList.FindAll(p => p.Current != null && p.Original == null).ConvertAll<ParamValueItem>(p => p.Current));

            List<DateTime> deleteList = valueList.FindAll(p => p.Current == null).ConvertAll<DateTime>(p => p.Original.Time);
            if (deleteList.Count > 0)
                strucProvider.DeleteValues(parameter.Idnum, deleteList.ToArray());

            //if (watcher == null)
            //    watcher = StartSaveWatcher(editValues);
            //else watcher.AddFinishHandler(() => AsyncForm.RunWatcher(StartSaveWatcher(editValues)));

            //AsyncForm.RunWatcher(watcher);
            StartSaveWatcher(editValues);
        }

        private AsyncOperationWatcher StartSaveWatcher(List<ParamValueItem> editValues)
        {
            //AsyncOperationWatcher watcher = null;

            Package package;
            //if (Arguments != null)
            //    package = new CPackage(parameter.Idnum, Arguments, editValues);
            //else
                package = new Package(parameter.Idnum, editValues);

            //watcher = 
            strucProvider.SaveValues(new Package[] { package });
            //watcher.AddFinishHandler(RetrieveValue);
            //return watcher;
            RetrieveValue(CurrentStartTime, CurrentEndTime);
            return null;
        }

        //private void RetrieveValue()
        //{
        //    RetrieveValue(CurrentStartTime, CurrentEndTime);
        //}

        #endregion
    }
}
