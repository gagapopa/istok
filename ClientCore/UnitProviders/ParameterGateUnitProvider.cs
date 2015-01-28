using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore.Utils;

namespace COTES.ISTOK.ClientCore.UnitProviders
{
    /// <summary>
    /// Унит провайдер для ручного ввода и расчета
    /// </summary>
    public class ParameterGateUnitProvider : UnitProvider
    {
        private const int DefaultPrevValuesColumnCount = 2;
        private const int MaxPrevValuesColumnCount = 42;
        private static int LastPrevValuesColumnCount = -1;
        public const bool DefaultUseMimeTex = true;

        ///// <summary>
        ///// Время по умолчанию, при открытии ручного ввода или расчета
        ///// </summary>
        //public static DateTime LastDateTime { get; set; }

        #region События
        /// <summary>
        /// Список отображаемых параметров изменился
        /// </summary>
        public event EventHandler ParameterListChanged;

        /// <summary>
        /// Значения изменились
        /// </summary>
        public event EventHandler ParameterValuesChanged;

        /// <summary>
        /// Изображения кодов параметров изменились
        /// </summary>
        public event EventHandler ParameterCodeImageChanged;

        /// <summary>
        /// Количество отображаемых колонок с предыдущими значениями изменилось
        /// </summary>
        public event EventHandler PrevValueColumnCountChanged;

        /// <summary>
        /// Изменилос свойство UseMimeTex
        /// </summary>
        public event EventHandler UseMimeTexChanged;
        #endregion

        private bool useMimeTex;

        /// <summary>
        /// Используется ли картинки кодов параметров
        /// </summary>
        public bool UseMimeTex
        {
            get { return useMimeTex; }
            set
            {
                if (value != useMimeTex)
                {
                    useMimeTex = value;
                    StartMimeTexGenerator();
                    if (UseMimeTexChanged != null)
                        UseMimeTexChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Использовать ли MimeTexGenerator для создания картинок кода.
        /// В некорых случаях (тонкий клиент), без него лучше обойтись
        /// </summary>
        public bool UseMimeTexGenerator { get; set; }

        private void StartMimeTexGenerator()
        {
            if (UseMimeTex && UseMimeTexGenerator && ((ParameterGateNode)unitNode).ManualParameters != null)
                foreach (ParameterNode paramNode in ((ParameterGateNode)unitNode).ManualParameters)
                    MimeTexGenerator.Singlton.GetImage(paramNode, CodeImageChanged);
        }

        public ParameterGateUnitProvider(StructureProvider strucProvider, UnitNode unitNode)
            : base(strucProvider, unitNode)
        {
            //LastDateTime = DateTime.Now;
            //queryTime = Interval.NearestEarlierTime(LastDateTime);

            //try { useMimeTex = ClientSettings.Instance.UseMimeTex; }
            //catch { useMimeTex = DefaultUseMimeTex; }
            UseMimeTexGenerator = true;
            useMimeTex = DefaultUseMimeTex;

            //RemoteDataService.UnitNodeChanged += new EventHandler<UnitNodeEventArgs>(RemoteDataService_UnitNodeChanged);

            prevValueColumnCount = -1;
        }

        protected override void DisposeProvider()
        {
            //RemoteDataService.UnitNodeChanged -= new EventHandler<UnitNodeEventArgs>(RemoteDataService_UnitNodeChanged);
            base.DisposeProvider();
        }

        public void StartProvider()
        {
            if (QueryTime == DateTime.MinValue)
            {
                QueryTime = Interval.NearestEarlierTime(strucProvider.LastDateTime);
            }

            if (prevValueColumnCount < 0)
            {
                if (LastPrevValuesColumnCount >= 0)
                    PrevValueColumnCount = LastPrevValuesColumnCount;
                else
                    PrevValueColumnCount = DefaultPrevValuesColumnCount;
            }

            ParameterGateNode parameterGateNode = unitNode as ParameterGateNode;

            if (parameterGateNode != null)
            {
                if (parameterGateNode.ManualParameters == null)
                {
                    int[] filterType = null;
                    if (parameterGateNode.Typ == (int)UnitTypeId.ManualGate)
                        filterType = new int[] { (int)UnitTypeId.ManualParameter };
                    else if (parameterGateNode.Typ == (int)UnitTypeId.TEPTemplate)
                        filterType = new int[] { (int)UnitTypeId.TEP };

                    parameterGateNode.ManualParameters = new List<ParameterNode>();
                    QueryUnitNodes(parameterGateNode.Idnum, filterType);
                }
                else
                {
                    ParamReceive(parameterGateNode.ManualParameters.Cast<UnitNode>().ToArray());
                    RetrieveValues(QueryTime);
                }
            }
        }

        //void RemoteDataService_UnitNodeChanged(object sender, UnitNodeEventArgs e)
        //{
        //    if (e.UnitNode.ParentId == UnitNode.ParentId)
        //    {
        //        ParameterGateNode parameterGateNode = unitNode as ParameterGateNode;

        //        if (parameterGateNode != null && parameterGateNode.ManualParameters != null)
        //        {
        //            //ReceivePr
        //        }
        //        //OnNewUnitNodeChanged
        //        //parameterGateNode.ManualParameters = null;
        //    }
        //}

        #region Работа со значениями
        Dictionary<int, ParamValueItem> valueParamDictionary = new Dictionary<int, ParamValueItem>();
        Dictionary<int, ParamValueItem[]> valueParamOriginalDictionary = new Dictionary<int, ParamValueItem[]>();

        public ParamValueItem GetParameterValue(ParameterNode parameterNode, uint tau)
        {
            ParamValueItem ret = null;
            ParamValueItem[] values;

            if ((tau > 0 || !valueParamDictionary.TryGetValue(parameterNode.Idnum, out ret))
                && valueParamOriginalDictionary.TryGetValue(parameterNode.Idnum, out values))
            {
                if (values != null && values.Length > tau)
                    ret = values[tau];
                if (tau == 0 && ret != null)
                    ret = ret.Clone() as ParamValueItem;
            }
            return ret;
        }

        public void SetParameterValue(ParameterNode parameterNode, ParamValueItem value)
        {
            valueParamDictionary[parameterNode.Idnum] = value;
        }

        /// <summary>
        /// Вызывается при получении значения
        /// </summary>
        /// <param name="x"></param>
        //FIXME: Вызывается при получении значеий
        private void ValueReceive(Package pack)
        {
            ParamValueItem[] paramValues;

            if (pack != null)
            {
                foreach (ParamValueItem receiveItem in pack.Values)
                {
                    if (!valueParamOriginalDictionary.TryGetValue(pack.Id, out paramValues))
                        valueParamOriginalDictionary[pack.Id] = paramValues = new ParamValueItem[PrevValueColumnCount + 1];

                    int i = Interval.GetQueryValues(/*StartTime,*/ receiveItem.Time, QueryTime);// -1;
                    if (Interval == Interval.Zero)
                        i = 0;
                    if (0 <= i && i < paramValues.Length)
                        paramValues[i] = receiveItem;
                }
            }
        }

        public void RetrieveValues(DateTime dateTime)
        {
            ClearUnsavedValues();
            ReleaseValues();
            QueryTime = dateTime;
            strucProvider.LastDateTime = QueryTime;
            valueParamDictionary.Clear();
            RetrieveValues();
        }

        public bool Received { get; private set; }

        private void RetrieveValues()
        {
            DateTime beginTime, endTime;
            Received = false;
            //очистить valueParamDictionary
            List<int> removeList = new List<int>();
            foreach (var paramId in valueParamDictionary.Keys)
                if (!Modified(paramId))
                    removeList.Add(paramId);

            foreach (var paramId in removeList)
                valueParamDictionary.Remove(paramId);

            valueParamOriginalDictionary.Clear();

            if (Interval != Interval.Zero)
            {
                beginTime = Interval.GetTime(QueryTime, -PrevValueColumnCount);
                endTime = Interval.GetNextTime(QueryTime);
            }
            else endTime = beginTime = DateTime.Now;

            if (((ParameterGateNode)unitNode).ManualParameters != null)
            {
                List<int> lstIds = ((ParameterGateNode)unitNode).ManualParameters.ConvertAll<int>(x => x.Idnum);
                //FIXME: Здесь добавлена праверка на наличие связанных параметров
                ((ParameterGateNode)unitNode).ManualParameters.ForEach(param => {
					if (param  as ManualParameterNode != null && ((ManualParameterNode)param).ValueConnectingParamNode > 0)
						lstIds.Add(((ManualParameterNode)param).ValueConnectingParamNode);
                                                                       });
                BeginGetValues(lstIds.ToArray(), beginTime, endTime, Interval, CalcAggregation.Nothing, false);
            }
        }

        private void DisplayValue()
        {
            Received = true;
            if (ParameterValuesChanged != null)
                ParameterValuesChanged(this, EventArgs.Empty);
        }

        public void SaveValues()
        {
            Package pack;
            List<Package> packages = new List<Package>();

            if ((UnitNode as ParameterGateNode).ManualParameters != null)
            {
                foreach (ParameterNode parameterNode in (UnitNode as ParameterGateNode).ManualParameters)
                {
                    if (Modified(parameterNode))
                    {
                        ParamValueItem receiveItem = valueParamDictionary[parameterNode.Idnum];
                        pack = new Package();
                        pack.Id = parameterNode.Idnum;
                        pack.Add(receiveItem);
                        packages.Add(pack);
                    }
                }
                BeginSaveValues(packages.ToArray());
            }
        }

        #endregion

        Dictionary<int, System.Drawing.Image> parameterCodeImageDictionary = new Dictionary<int, System.Drawing.Image>();

        private void CodeImageChanged(ParameterNode param, System.Drawing.Image codeImage)
        {
            parameterCodeImageDictionary[param.Idnum] = codeImage;
            if (codeImage.Height > RowHeight) RowHeight = codeImage.Height;
            if (codeImage.Width > CodeColumnWidth) CodeColumnWidth = codeImage.Width;
            if (ParameterCodeImageChanged != null)
                ParameterCodeImageChanged(this, EventArgs.Empty);
        }

        public System.Drawing.Image GetParameterCodeImage(ParameterNode parameterNode)
        {
            System.Drawing.Image codeImage;

            if (!parameterCodeImageDictionary.TryGetValue(parameterNode.Idnum, out codeImage)
                || codeImage == null)
                foreach (var item in StructureProvider.Session.Types)
                {
                    if (item.Idnum == (int)parameterNode.Typ)
                    {
                        codeImage = Image.FromStream(new MemoryStream(item.Icon));
                        break;
                    }
                }

            return codeImage;
        }

        public int RowHeight { get; protected set; }

        public int CodeColumnWidth { get; protected set; }

        /// <summary>
        /// Вызывается при получении параметра
        /// </summary>
        /// <param name="x"></param>
        private void ParamReceive(Object x)
        {
            UnitNode[] unitNodes = x as UnitNode[];
            ParameterNode paramNode;

            if (unitNodes != null)
            {
                foreach (UnitNode node in unitNodes)
                {
                    if ((paramNode = node as ParameterNode) != null)
                    {
                        if (!((ParameterGateNode)unitNode).ManualParameters.Contains(paramNode))
                            ((ParameterGateNode)unitNode).ManualParameters.Add(paramNode);
                    }
                }
                StartMimeTexGenerator();
                if (ParameterListChanged != null)
                    ParameterListChanged(this, EventArgs.Empty);
            }
        }

        public void CalcForm_CalcFinished(Object sender, EventArgs e)
        {
            RetrieveValues();
        }

        /// <summary>
        /// Интервал для листания
        /// </summary>
        public Interval Interval { get { return ((ParameterGateNode)unitNode).Interval; } }

        private DateTime queryTime;

        public event EventHandler QueryTimeChanged;

        /// <summary>
        /// Текущее отображаемое время
        /// </summary>
        public DateTime QueryTime
        {
            get { return queryTime; }
            protected set
            {
                DateTime oldTime = queryTime;
                queryTime = Interval.NearestEarlierTime(value);
                if (QueryTimeChanged != null && !oldTime.Equals(queryTime))
                    QueryTimeChanged(this, EventArgs.Empty);
            }
        }

        private int prevValueColumnCount;
        /// <summary>
        /// Количество колонок отображающих предыдущие значения
        /// </summary>
        public int PrevValueColumnCount
        {
            get { return prevValueColumnCount; }
            set
            {
                if (value > MaxPrevValuesColumnCount)
                    prevValueColumnCount = MaxPrevValuesColumnCount;
                else
                    prevValueColumnCount = value;

                LastPrevValuesColumnCount = prevValueColumnCount;
                if (PrevValueColumnCountChanged != null)
                    PrevValueColumnCountChanged(this, EventArgs.Empty);
                RetrieveValues();
            }
        }

        public DateTime GetNextTime()
        {
            return Interval.GetNextTime(QueryTime);
        }

        public DateTime GetPrevTime()
        {
            return Interval.GetPrevTime(QueryTime);
        }

        public override bool HasChanges
        {
            get
            {
                if (base.HasChanges) return true;
                if (unitNode.Typ == (int)UnitTypeId.ManualGate && (UnitNode as ParameterGateNode).ManualParameters != null)
                {
                    ParameterNode[] parameterList = (UnitNode as ParameterGateNode).ManualParameters.ToArray();
                    foreach (ParameterNode parameterNode in parameterList)
                        if (Modified(parameterNode))
                            return true;
                }
                return false;
            }
        }

        public override IEnumerable<ActionArgs> Save()
        {
            SaveValues();
            return base.Save();
        }

        /// <summary>
        /// Сравнить изменилось ли значение в строке
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Modified(ParameterNode paramNode)
        { return Modified(paramNode.Idnum); }

        public bool Modified(int parameterId)
        {
            try
            {
                ParamValueItem receiveItem = null;
                ParamValueItem[] paramValues;

                if (valueParamDictionary.TryGetValue(parameterId, out receiveItem)
                    && valueParamOriginalDictionary.TryGetValue(parameterId, out paramValues))
                {
                    return !receiveItem.Equals(paramValues[0]);
                }
                return receiveItem != null
                    && receiveItem.Quality != Quality.Bad;
            }
            catch
            {
                return true;
            }
        }

        public ParameterNode GetParameterNode(int id)
        {
            return ((ParameterGateNode)unitNode).ManualParameters.Find(x => x.Idnum == id);
        }

        public ParameterNode[] GetParameterNodes()
        {
            if (((ParameterGateNode)unitNode).ManualParameters == null)
                return new ParameterNode[0];
            return ((ParameterGateNode)unitNode).ManualParameters.ToArray();
        }

        public override void ClearUnsavedData()
        {
            ClearUnsavedValues();
            base.ClearUnsavedData();
        }

        public virtual void ClearUnsavedValues()
        {
            ClearValues();
            OnValuesEditModeChanged();
            ReleaseValues();
        }

        public void ReleaseValues()
        {
            DateTime startTime, endTime;
            if (Interval == Interval.Zero)
            {
                startTime = DateTime.MinValue;
                endTime = DateTime.MaxValue;
            }
            else
            {
                startTime = QueryTime;
                endTime = Interval.GetNextTime(startTime);
            }
            RDS.ValuesDataService.ReleaseValues(UnitNode, startTime, endTime);
            ValuesEditMode = false;
        }

        private void ClearValues()
        {
            valueParamDictionary.Clear();

            if (correctingParameters != null)
            {
                DateTime startTime = QueryTime, endTime = Interval.GetNextTime(startTime);

                foreach (var item in correctingParameters)
                    RDS.ValuesDataService.ReleaseValues(item, startTime, endTime);
                correctingParameters.Clear();
            }
        }

        private UnitNode[] lockedParameters;

        private List<ParameterNode> correctingParameters;

        /// <summary>
        /// Скорректировать значение параметра.<br />
        /// Попытаться взять параметр на изменение значений
        /// </summary>
        /// <param name="param"></param>
        public void Correct(ParameterNode param)
        {
            RDS.ValuesDataService.LockValues(param, QueryTime, Interval.GetNextTime(QueryTime));
            if (correctingParameters == null)
                correctingParameters = new List<ParameterNode>();
            correctingParameters.Add(param);
            ParamValueItem receiveItem = GetParameterValue(param, 0);
            if (!(receiveItem is CorrectedParamValueItem))
                SetParameterValue(param, new CorrectedParamValueItem(receiveItem));
            ValuesEditMode = true;
        }

        public void LockValues()
        {
            try
            {
                DateTime startTime, endTime;
                if (Interval == Interval.Zero)
                {
                    startTime = DateTime.MinValue;
                    endTime = DateTime.MaxValue;
                }
                else
                {
                    startTime = QueryTime;
                    endTime = Interval.GetNextTime(startTime);
                }
                RDS.ValuesDataService.LockValues(UnitNode, startTime, endTime);
                lockedParameters = null;
                ValuesEditMode = true;
            }
            catch (LockException exc)
            {
                lockedParameters = (from c in exc.Causes select c.Node).ToArray();

                ValuesEditMode = !lockedParameters.Contains(UnitNode);

                throw;
            }
        }

        private bool valuesEditMode;
        public bool ValuesEditMode
        {
            get { return valuesEditMode; }
            protected set
            {
                valuesEditMode = value;
                OnValuesEditModeChanged();
            }
        }

        public event EventHandler ValuesEditModeChanged;

        protected void OnValuesEditModeChanged()
        {
            if (ValuesEditModeChanged != null)
                ValuesEditModeChanged(this, EventArgs.Empty);
        }

        public bool IsLocked(ParameterNode parameter)
        {
            return lockedParameters != null && lockedParameters.Contains(parameter/*.Idnum*/);
        }

        public bool IsCorrecting(ParameterNode parameter)
        {
            return correctingParameters != null && correctingParameters.Find(p => p.Idnum == parameter.Idnum) != null;
        }

        public bool IsCorrected(ParameterNode parameter, uint tau)
        {
            return (tau > 0 || !IsCorrecting(parameter))
                && GetParameterValue(parameter, tau) is CorrectedParamValueItem;
        }

        public void Deccorect(ParameterNode parameter)
        {
            CorrectedParamValueItem correctedValue;
            if (IsCorrecting(parameter))
            {
                correctedValue = GetParameterValue(parameter, 0) as CorrectedParamValueItem;
                if (correctedValue != null)
                {
                    correctedValue.Value = double.NaN;
                    SetParameterValue(parameter, correctedValue);
                }
                correctingParameters.Remove(parameter);
                if (!Modified(parameter))
                    RDS.ValuesDataService.ReleaseValues(parameter, QueryTime, Interval.GetNextTime(QueryTime));
            }
            else
            {
                correctedValue = GetParameterValue(parameter, 0) as CorrectedParamValueItem;
                if (correctedValue != null)
                {
                    RDS.ValuesDataService.LockValues(parameter, QueryTime, Interval.GetNextTime(QueryTime));
                    correctedValue.Value = double.NaN;
                    SetParameterValue(parameter, correctedValue);
                }
            }
        }

        #region DataRequest
        protected bool syncRequest = true;
        protected void BeginGetValues(int[] ids, DateTime beginTime,
            DateTime endTime, Interval interval, CalcAggregation aggregation, bool useBlockValues)
        {
            IEnumerable<Package> res = RDS.ValuesDataService.GetValues(ids, beginTime, endTime, interval, aggregation, useBlockValues);
            foreach (var item in res)
                ValueReceive(item);
            DisplayValue();
        }
        protected void BeginSaveValues(Package[] packages)
        {
            RDS.ValuesDataService.SaveValues(packages);
            ClearValues();
            RetrieveValues();
        }
        protected void QueryUnitNodes(int parent, int[] filterType)
        {
            IEnumerable<UnitNode> res = RDS.NodeDataService.GetUnitNodesFiltered(parent, filterType);
            ParamReceive(res.ToArray());
            RetrieveValues(QueryTime);
        }
        #endregion
    }
}
