using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Calc;

namespace COTES.ISTOK.ClientCore.UnitProviders
{
    /// <summary>
    /// Редактор формул
    /// </summary>
    public class FormulaUnitProvider : UnitProvider
    {
        RevisedStorage<String> baseFormula;

        RevisedStorage<String> formulaStorage = new RevisedStorage<String>();

        String currentFormula;

        public virtual String Formula
        {
            get
            {
                RevisionInfo revision = GetRealRevision(CurrentRevision);
                currentFormula = formulaStorage.Get(revision);
                return currentFormula;
            }
            set
            {
                if (!String.Equals(currentFormula, value))
                {
                    RevisionInfo revision = GetRealRevision(CurrentRevision);
                    currentFormula = value;
                    formulaStorage.Set(revision, value);
                }
            }
        }

        protected override void OnCurrentRevisionChanged()
        {
            base.OnCurrentRevisionChanged();
            OnFormulaChanged();
            Functions = strucProvider.GetFunctions(CurrentRevision);
        }

        /// <summary>
        /// Аргументы условных параметров
        /// </summary>
        public CalcArgumentInfo[] Arguments { get; protected set; }

        public KeyValuePair<int, CalcArgumentInfo[]>[] ArgumentsKey { get; protected set; }

        /// <summary>
        /// Список функций доступных при расчете
        /// </summary>
        public FunctionInfo[] Functions { get; protected set; }

        /// <summary>
        /// Список констант, доступных при расчете
        /// </summary>
        public ConstsInfo[] Consts { get; protected set; }

        /// <summary>
        /// Событие, возникающие когда меняются аргументы
        /// </summary>
        public event Action<CalcArgumentInfo[]> ArgumentsRetrieved;

        /// <summary>
        /// Событие, возникающие когда меняются константы
        /// </summary>
        public event Action<ConstsInfo[]> ConstsRetrieved;

        public event EventHandler FormulaChanged;

        /// <summary>
        /// Вызвать событие ArgumentsRetrieved
        /// </summary>
        protected void OnArgumentsRetrieved()
        {
            if (ArgumentsRetrieved != null)
                ArgumentsRetrieved(Arguments);
        }

        /// <summary>
        /// Вызвать событие ConstsRetrieved
        /// </summary>
        protected void OnConstsRetrieved()
        {
            if (ConstsRetrieved != null)
                ConstsRetrieved(Consts);
        }

        protected void OnFormulaChanged()
        {
            if (FormulaChanged != null)
                FormulaChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Проверить формулу
        /// </summary>
        /// <param name="callback">Метод для получения сообщений</param>
        /// <param name="checkedCallback">Метод, вызываемый по завершению проверки</param>
        public virtual Message[] CheckFormula()//Action<Message[]> callback, Action checkedCallback)
        {
            RevisionInfo revision = GetRealRevision(CurrentRevision);
            //callback(RDS.CalcDataService.CheckFormula( revision, Formula, ArgumentsKey));
            //checkedCallback();
            return RDS.CalcDataService.CheckFormula(revision, Formula, ArgumentsKey);
        }

        public FormulaUnitProvider(StructureProvider strucProvider, UnitNode parameter)
            : base(strucProvider, parameter)
        {
            UpdateUnitNode(parameter);
            LoadWorkInfo();
            //if (parameter != null)
            //{
            //    ParameterGateNode parameterGateNode = RDS.NodeDataService.GetUnitNode( parameter.ParentId).Result as ParameterGateNode;
            //    if (parameterGateNode != null)
            //    {
            //        ArgumentsKey = parameterGateNode.GetArguments(RevisionInfo.Default, id => RDS.NodeDataService.GetUnitNode( id).Result);
            //        if (ArgumentsKey == null)
            //            ArgumentsKey = new KeyValuePair<int, CalcArgumentInfo[]>[0];
            //        List<CalcArgumentInfo> argsList = new List<CalcArgumentInfo>();
            //        foreach (var item in ArgumentsKey)
            //            argsList.AddRange(item.Value);

            //        Arguments = argsList.ToArray();
            //    }
            //    else
            //    {
            //        ArgumentsKey = new KeyValuePair<int, CalcArgumentInfo[]>[0];
            //        Arguments = new CalcArgumentInfo[0];
            //    }
            //}
        }

        public FormulaUnitProvider(StructureProvider strucProvider)
            : this(strucProvider, null)
        {

        }

        public override void LoadWorkInfo()
        {
            base.LoadWorkInfo();
            if (unitNode != null)
            {
                ParameterGateNode parameterGateNode = null;
                if (unitNode.ParentId != 0)
                    parameterGateNode = RDS.NodeDataService.GetUnitNode(unitNode.ParentId) as ParameterGateNode;
                if (parameterGateNode != null)
                {
                    ArgumentsKey = parameterGateNode.GetArguments(
                        RevisionInfo.Default,
                        id => RDS.NodeDataService.GetUnitNode(id));

                    if (ArgumentsKey == null)
                        ArgumentsKey = new KeyValuePair<int, CalcArgumentInfo[]>[0];
                    List<CalcArgumentInfo> argsList = new List<CalcArgumentInfo>();
                    foreach (var item in ArgumentsKey)
                        argsList.AddRange(item.Value);

                    Arguments = argsList.ToArray();
                }
                else
                {
                    ArgumentsKey = new KeyValuePair<int, CalcArgumentInfo[]>[0];
                    Arguments = new CalcArgumentInfo[0];
                }
            }
        }

        private bool providerStarted = false;
        public void StartProvider()
        {
            if (providerStarted)
                return;
            // загрузить список функций
            Functions = strucProvider.GetFunctions(CurrentRevision);

            // загрузить список констант
            Consts = RDS.CalcDataService.GetConsts();
            OnConstsRetrieved();
        }

        public override bool HasChanges
        {
            get
            {
                return base.HasChanges || !baseFormula.Equals(formulaStorage); //!String.Equals(baseFormula, Formula);
            }
        }

        public override IEnumerable<ActionArgs> Save()
        {
            CalcParameterNode calcParameter;

            //if ((calcParameter = NewUnitNode as CalcParameterNode) != null
            //    && !String.Equals(baseFormula, Formula))
            //    calcParameter.Formula = Formula;

            if ((calcParameter = NewUnitNode as CalcParameterNode) != null
                && !baseFormula.Equals(formulaStorage))
                calcParameter.SetFormulaStorage(formulaStorage);

            var list = base.Save();
            ClearFormula();
            return list;
        }

        private void UpdateUnitNode(Object x)
        {
            CalcParameterNode parameter = x as CalcParameterNode;
            if (parameter != null)
            //Formula = baseFormula = parameter.Formula;
            {
                baseFormula = parameter.GetFormulaStorage();
                formulaStorage = baseFormula.Clone() as RevisedStorage<String>;
            }
        }

        //protected override BaseUnitControl GetUnitControl(bool multitab)
        //{
        //    if (Editable && !multitab)
        //    {
        //        FormulaEditControl editControl = new FormulaEditControl(this);
        //        editControl.Text = "Редактор формул";
        //        return editControl;
        //    }
        //    return null;
        //}

        /// <summary>
        /// Получить имя объекта по его ИД
        /// </summary>
        /// <param name="objectID">ИД объекта</param>
        /// <returns></returns>
        /// <remarks>Данный метод применяется для отображений сообщений полученных в ходе проверки формул,
        /// где для обозначения объекта используются <see cref="COTES.ISTOK.Calc.MessageAB.NodeID"/> и 
        /// <see cref="COTES.ISTOK.Calc.MessageAB.ObjectID"/></remarks>
        public virtual String GetObjectName(String objectID)
        {
            return String.Empty;
        }

        public override void ClearUnsavedData()
        {
            ClearFormula();
            base.ClearUnsavedData();
        }

        private void ClearFormula()
        {
            CalcParameterNode calcParameterNode;
            if ((calcParameterNode = UnitNode as CalcParameterNode) != null)
            {
                //Formula = baseFormula = calcParameterNode.Formula;
                baseFormula = calcParameterNode.GetFormulaStorage();

                formulaStorage = baseFormula.Clone() as RevisedStorage<String>;

                OnFormulaChanged();
            }
        }
    }
}
