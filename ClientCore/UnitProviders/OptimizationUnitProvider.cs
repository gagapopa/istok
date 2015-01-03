using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC;
using COTES.ISTOK.Calc;

namespace COTES.ISTOK.ClientCore.UnitProviders
{
    /// <summary>
    /// Редактор оптимизационного расчета
    /// </summary>
    public class OptimizationUnitProvider : FormulaUnitProvider
    {
        /// <summary>
        /// Тип выражения оптимизационного расчета
        /// </summary>
        public enum ExpressionType
        {
            Nothing,
            /// <summary>
            /// Оптимизируемое выражение
            /// </summary>
            Expression,
            /// <summary>
            /// Выражение, определяющие область определения
            /// </summary>
            DefinationDomain,
            /// <summary>
            /// Выражение, определяющие возможные значения аргументов
            /// </summary>
            ArgumentExpression
        }

        RevisedStorage<String> expression, definationDomainExpression;
        Dictionary<String, OptimizationArgument> arguments;

        protected override void OnCurrentRevisionChanged()
        {
            base.OnCurrentRevisionChanged();
            RefreshArguments();
            //OnFormulaChanged();
        }

        public String Expression
        {
            get
            {
                RevisionInfo revision = GetRealRevision(CurrentRevision);
                return expression.Get(revision);
            }
            set
            {
                RevisionInfo revision = GetRealRevision(CurrentRevision);
                expression.Set(revision, value);
            }
        }

        public String DefinationDomainExpression
        {
            get
            {
                RevisionInfo revision = GetRealRevision(CurrentRevision);
                return definationDomainExpression.Get(revision);
            }
            set
            {
                RevisionInfo revision = GetRealRevision(CurrentRevision);
                definationDomainExpression.Set(revision, value);
            }
        }

        RevisedStorage<List<OptimizationArgument>> argsValues;

        /// <summary>
        /// Аргументы оптимизационного расчета
        /// </summary>
        public List<OptimizationArgument> ArgsValues
        {
            get
            {
                RevisionInfo revision = GetRealRevision(CurrentRevision);
                return argsValues.Get(revision);
            }
            protected set
            {
                RevisionInfo revision = GetRealRevision(CurrentRevision);
                argsValues.Set(revision, value);
            }
        }

        public OptimizationUnitProvider(StructureProvider strucProvider, OptimizationGateNode node)
            : base(strucProvider, node)
        {
            ClearUnsavedData();
            RefreshArguments();
        }

        public override void LoadWorkInfo()
        {
            base.LoadWorkInfo();
            RefreshArguments();
        }

        /// <summary>
        /// Обновить информацию о аргументах согласно ArgsValues
        /// </summary>
        private void RefreshArguments()
        {
            if (arguments == null || argsValues == null)
                return;
            List<CalcArgumentInfo> args = new List<CalcArgumentInfo>();

            arguments.Clear();
            foreach (var item in ArgsValues)
            {
                arguments.Add(item.Name, item);
                //args.Add(new CalcArgumentInfo(item.Name, String.Empty, ParameterAccessor.In));
            }
            RevisionInfo revision = GetRealRevision(CurrentRevision);

            ArgumentsKey = (unitNode as OptimizationGateNode).GetArguments(revision, id => RDS.NodeDataService.GetUnitNode(id));
            OnArgumentsRetrieved();
        }

        ExpressionType selectedExpression;
        /// <summary>
        /// Тип текущего редактируемого выражения
        /// </summary>
        public ExpressionType SelectedExpression
        {
            get { return selectedExpression; }
            set
            {
                selectedExpression = value;
                OnSelectedExpressionChanged();
            }
        }

        String selectedArgumentName;
        /// <summary>
        /// Имя текущего редактируемого аргумента
        /// </summary>
        public String SelectedArgumentName
        {
            get { return selectedArgumentName; }
            set
            {
                selectedArgumentName = value;
                OnSelectedExpressionChanged();
            }
        }

        /// <summary>
        /// Событие порождаемое когда меняется текущая редактируемая формула
        /// </summary>
        public event EventHandler SelectedExpressionChanged;

        /// <summary>
        /// Вызвать событие SelectedExpressionChanged
        /// </summary>
        protected void OnSelectedExpressionChanged()
        {
            if (SelectedExpressionChanged != null)
                SelectedExpressionChanged(this, EventArgs.Empty);
        }

        public override string Formula
        {
            get
            {
                switch (SelectedExpression)
                {
                    case ExpressionType.Expression:
                        return Expression;
                    case ExpressionType.DefinationDomain:
                        return DefinationDomainExpression;
                    case ExpressionType.ArgumentExpression:
                        if (!String.IsNullOrEmpty(SelectedArgumentName) && arguments.ContainsKey(SelectedArgumentName))
                            return arguments[SelectedArgumentName].Expression;
                        return null;
                    default:
                        return String.Empty;
                }
            }
            set
            {
                switch (SelectedExpression)
                {
                    case ExpressionType.Expression:
                        Expression = value;
                        break;
                    case ExpressionType.DefinationDomain:
                        DefinationDomainExpression = value;
                        break;
                    case ExpressionType.ArgumentExpression:
                        if (!String.IsNullOrEmpty(SelectedArgumentName) && arguments.ContainsKey(SelectedArgumentName))
                            arguments[SelectedArgumentName].Expression = value;
                        break;
                    default:
                        break;
                }
            }
        }

        public override Message[] CheckFormula()//Action<Message[]> callback, Action checkedCallback)
        {
            Object lockObject = new Object();
            int checkLeft = 0;

            lock (lockObject) checkLeft = 2;

            //AsyncOperationWatcher.FinishNotificationDelegate finishDelegate =
            //    () => { lock (lockObject) if (--checkLeft == 0)checkedCallback(); };

            AsyncOperationWatcher watcher;

            RevisionInfo revision = GetRealRevision(CurrentRevision);

            // проверка формулы для каждого аргумента
            foreach (var item in ArgsValues)
            {
                lock (lockObject) ++checkLeft;
                int index = ArgsValues.IndexOf(item);

                throw new NotImplementedException("CheckFormula");
                //watcher = RemoteDataService.BeginCheckFormula(revision, item.Expression, null);
                //watcher.AddMessageReceivedHandler(x =>
                //{
                //    callback(x);
                //});
                //watcher.AddFinishHandler(finishDelegate);
                //UniForm.RunWatcher(watcher, false);
            }

            throw new NotImplementedException("CheckFormula");
            // проверка оптимизируемого выражения
            //watcher = RemoteDataService.BeginCheckFormula(revision, Expression, ArgumentsKey);
            //watcher.AddMessageReceivedHandler(x =>
            //{
            //    callback(x);
            //});
            //watcher.AddFinishHandler(finishDelegate);
            //UniForm.RunWatcher(watcher, false);

            // проверка выражения области определения
            //watcher = RemoteDataService.BeginCheckFormula(revision, DefinationDomainExpression, ArgumentsKey);
            //watcher.AddMessageReceivedHandler(x =>
            //{
            //    callback(x);
            //});
            //watcher.AddFinishHandler(finishDelegate);
            //UniForm.RunWatcher(watcher, false);
        }

        /// <summary>
        /// Добавить новый оптимизационный аргумент
        /// </summary>
        /// <param name="argumentName">Имя аргумента</param>
        public void AddArgument(OptimizationArgument argumentName)
        {
            ArgsValues.Add(argumentName);
            RefreshArguments();
        }

        /// <summary>
        /// Удалить оптимизационный аргумент
        /// </summary>
        /// <param name="argumentName">Имя аргумента</param>
        public void RemoveArgument(OptimizationArgument argumentName)
        {
            ArgsValues.RemoveAll(x => String.Equals(x.Name, argumentName.Name));
            RefreshArguments();
        }

        /// <summary>
        /// Переименовать оптимизационный аргумент
        /// </summary>
        /// <param name="oldArgumentName">Старое имя аргумента</param>
        /// <param name="newArgumentName">Новое имя аргумента</param>
        public void RenameArgument(String oldArgumentName, OptimizationArgument newArgumentName)
        {
            OptimizationArgument argument;
            argument = ArgsValues.Find(x => String.Equals(x.Name, oldArgumentName));
            if (arguments != null)
                argument.CopyFrom(newArgumentName);
            RefreshArguments();
        }

        /// <summary>
        /// Переместить аргумент
        /// </summary>
        /// <param name="argumentName">Имя аргумента</param>
        /// <param name="shift">Смещение</param>
        public void MoveArgument(string argumentName, int shift)
        {
            OptimizationArgument argument;
            argument = ArgsValues.Find(x => String.Equals(x.Name, argumentName));

            int index = ArgsValues.IndexOf(argument), newindex = index + shift;

            ArgsValues.Remove(argument);
            ArgsValues.Insert(newindex, argument);

            RefreshArguments();
        }

        public const String ExpressionObjectID = "-1";
        public const String DefinationDomainObjectID = "-2";

        public override String GetObjectName(String objectID)
        {
            switch (objectID)
            {
                case ExpressionObjectID:
                    return "Оптимизируемое выражение";
                case DefinationDomainObjectID:
                    return "Область определения";
                default:
                    int id;
                    if (int.TryParse(objectID, out id))
                    {
                        if (0 <= id && id < Arguments.Length)
                            return Arguments[id].Name;
                    }
                    break;
            }
            return base.GetObjectName(objectID);
        }

        public override IEnumerable<ActionArgs> Save()
        {
            OptimizationGateNode optimizationNode;

            if ((optimizationNode = NewUnitNode as OptimizationGateNode) != null)
            {
                optimizationNode.SetExpressionStorage(expression);
                optimizationNode.SetDefinationDomainStorage(definationDomainExpression);
                optimizationNode.SetArgsValuesStorage(argsValues);
            }

            return base.Save();
        }

        private Message[] messages;

        public Message[] CalcMessages
        {
            get { return messages; }
            set
            {
                messages = value;
                if (CalcMessagesChanged != null)
                    CalcMessagesChanged(this, EventArgs.Empty);
            }
        }

        private bool hideMessages = true;

        public bool HideMessages
        {
            get { return hideMessages; }
            set
            {
                hideMessages = value;
                if (HideMessagesChanged != null)
                    HideMessagesChanged(this, EventArgs.Empty);
            }
        }

        private ArgumentsValues[] paremtArguments;

        public ArgumentsValues[] ParentArguments
        {
            get { return paremtArguments; }
            set
            {
                paremtArguments = value;
                if (ParentArgumentsChanged != null)
                    ParentArgumentsChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler CurrentTimeChanged;

        public event EventHandler CalcMessagesChanged;

        public event EventHandler HideMessagesChanged;

        public event EventHandler ParentArgumentsChanged;

        private void MessageRetrieved(Message[] messages)
        {
            if (messages != null)
            {
                List<Message> calcMessages = new List<Message>();
                if (CalcMessages != null) calcMessages.AddRange(CalcMessages);
                calcMessages.AddRange(messages);
                CalcMessages = calcMessages.ToArray();
            }
        }

        public override bool HasChanges
        {
            get
            {
                OptimizationGateNode optimizationNode = unitNode as OptimizationGateNode;

                if (optimizationNode != null)
                {
                    bool hasChanged = !expression.Equals(optimizationNode.GetExpressionStorage())
                        || !definationDomainExpression.Equals(optimizationNode.GetDefinationDomainStorage())
                        || !argsValues.Equals(optimizationNode.GetArgsValuesStorage(), (a, b) =>
                        {
                            bool ret = a.Count == b.Count;
                            for (int i = 0; ret && i < a.Count; i++)
                            {
                                ret = a[i].Equals(b[i]);
                            }
                            return ret;
                        });

                    return hasChanged;
                }
                return false;
            }
        }

        public override void ClearUnsavedData()
        {
            OptimizationGateNode optimizationNode;
            if ((optimizationNode = unitNode as OptimizationGateNode) != null)
            {
                expression = optimizationNode.GetExpressionStorage().Clone() as RevisedStorage<String>;
                definationDomainExpression = optimizationNode.GetDefinationDomainStorage().Clone() as RevisedStorage<String>;
                argsValues = optimizationNode.GetArgsValuesStorage().Clone() as RevisedStorage<List<OptimizationArgument>>;

                arguments = new Dictionary<String, OptimizationArgument>();
                RefreshArguments();
            }
            base.ClearUnsavedData();
        }
    }
}
