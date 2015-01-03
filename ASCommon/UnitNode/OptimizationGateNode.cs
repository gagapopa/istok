using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data;
using System.Text;
using COTES.ISTOK.ASC.TypeConverters;
using COTES.ISTOK.Calc;

namespace COTES.ISTOK.ASC
{
    /// <summary>
    /// Шаблон оптимизации, содержащий условные параметры
    /// </summary>
    [Serializable]
    public class OptimizationGateNode : ParameterGateNode
    {
        private const String argsAttributeName = "optimiz_args";
        private const String maximalizeAttributeName = "optimiz_max";
        private const String expressionAttributeName = "optimiz_expr";
        private const String defanitionDomainAttributeName = "optimiz_dd";
        private const String calcAllAttributeName = "calc_all";

        public OptimizationGateNode(DataRow row)
            : base(row)
        { }

        public OptimizationGateNode()
            : base()
        { }

#if DEBUG
        /// <summary>
        /// Строка изменяемых переменных, как она хранится в БД
        /// </summary>
        [CategoryOrder(CategoryGroup.Debug),
        DisplayName("Изменяемые переменные"),
        Description("Строка изменяемых переменных, как она хранится в БД")]
        public String ArgValuesPropertyString
        {
            get
            {
                return GetAttribute(argsAttributeName);
            }
        }
#endif

        /// <summary>
        /// Значения параметров, перебираемые при оптимизации
        /// </summary>
#if DEBUG
        [CategoryOrder(CategoryGroup.Debug),
        DisplayName("Изменяемые переменные"),
        Description("DEBUGONLY Именна переменных для оптимизации и их значения")]
#else
        [Browsable(false)]
#endif
        public OptimizationArgument[] ArgsValues
        {
            get
            {
                return GetArgsValues(RevisionInfo.Default).ToArray();
            }
        }

        public IEnumerable<OptimizationArgument> GetArgsValues(RevisionInfo revision)
        {
            return GetArgsValuesStorage().Get(revision).AsReadOnly();
        }

        public RevisedStorage<List<OptimizationArgument>> GetArgsValuesStorage()
        {
            RevisedStorage<String> ret;
            RevisedStorage<List<OptimizationArgument>> storage = new RevisedStorage<List<OptimizationArgument>>();

            Attributes.TryGetValue(argsAttributeName, out ret);

            foreach (var revision in ret)
            {
                String argsString = ret.Get(revision);
                List<OptimizationArgument> argsValues = new List<OptimizationArgument>();
                OptimizationArgument optimizationArgument;
                int pos = 0;
                while ((optimizationArgument = OptimizationArgument.FromString(argsString, ref pos)) != null)
                    argsValues.Add(optimizationArgument);

                storage.Set(revision, argsValues);
            }

            return storage;
        }

        public void SetArgsValuesStorage(RevisedStorage<List<OptimizationArgument>> storage)
        {
            RevisedStorage<String> stringStorage = new RevisedStorage<String>(); ;
            List<OptimizationArgument> argsValues;
            StringBuilder builder = new StringBuilder();

            foreach (var revision in storage)
            {
                builder.Length = 0;
                argsValues = storage.Get(revision);

                foreach (var item in argsValues)
                {
                    item.ToString(builder);
                }
                stringStorage.Set(revision, builder.ToString());
            }
            Attributes[argsAttributeName] = stringStorage;
        }


        public override KeyValuePair<int, CalcArgumentInfo[]>[] GetArguments(RevisionInfo revision, Func<int, UnitNode> getNodeFunction)
        {
            List<CalcArgumentInfo> argsList = new List<CalcArgumentInfo>();
            List<KeyValuePair<int, CalcArgumentInfo[]>> arguments = new List<KeyValuePair<int, CalcArgumentInfo[]>>();
            KeyValuePair<int, CalcArgumentInfo[]>[] baseArguments = base.GetArguments(revision, getNodeFunction);

            if (baseArguments != null) arguments.AddRange(baseArguments);

            foreach (var item in GetArgsValues(revision))
            {
                argsList.Add(new CalcArgumentInfo(item.Name, String.Empty, ParameterAccessor.In));
            }
            arguments.Add(new KeyValuePair<int, CalcArgumentInfo[]>(this.Idnum, argsList.ToArray()));
            return arguments.ToArray();
        }

        /// <summary>
        /// Искать максимум или минимум
        /// </summary>
        [CategoryOrder(CategoryGroup.Calc),
        DisplayName("Поиск максимума"),
        Description("Да - если требуется максимизировать критерий и Нет - вслучае минимизации")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool Maximalize
        {
            get
            {
                bool res;

                if (bool.TryParse(GetAttribute(maximalizeAttributeName), out res))
                    return res;

                return false;
            }
            set
            {
                SetAttribute(maximalizeAttributeName, value.ToString());
            }
        }

        /// <summary>
        /// Оптимизируемый критерий
        /// </summary>
        [CategoryOrder(CategoryGroup.Calc),
        DisplayName("Оптимизируемое выражение"),
        Description("Выражение чей минимум будет ")]
        [RevisionUnitNode(expressionAttributeName)]
        public String Expression
        {
            get { return GetAttribute(expressionAttributeName); }
            set { SetAttribute(expressionAttributeName, value); }
        }

        public RevisedStorage<String> GetExpressionStorage()
        {
            RevisedStorage<String> ret;

            Attributes.TryGetValue(expressionAttributeName, out ret);

            if (ret == null)
                ret = new RevisedStorage<String>();

            return ret;
        }

        public void SetExpressionStorage(RevisedStorage<String> storage)
        {
            Attributes[expressionAttributeName] = storage;
        }

        /// <summary>
        /// Область определения для входных аргументов
        /// </summary>
        [CategoryOrder(CategoryGroup.Calc),
        DisplayName("Область определения"),
        Description("Выражение, возвращает истину если все входные параметры удовлетворяют области определения")]
        [RevisionUnitNode(defanitionDomainAttributeName)]
        public String DefinationDomain
        {
            get { return GetAttribute(defanitionDomainAttributeName); }
            set { SetAttribute(defanitionDomainAttributeName, value); }
        }

        public RevisedStorage<String> GetDefinationDomainStorage()
        {
            RevisedStorage<String> ret;

            Attributes.TryGetValue(defanitionDomainAttributeName, out ret);

            if (ret == null)
                ret = new RevisedStorage<string>();

            return ret;
        }

        public void SetDefinationDomainStorage(RevisedStorage<String> storage)
        {
            Attributes[defanitionDomainAttributeName] = storage;
        }

        /// <summary>
        /// Расчитывать значения всех вложенных параметров для всех аргументов
        /// </summary>
        [CategoryOrder(CategoryGroup.Calc),
        DisplayName("Расчитывать все"),
        Description("Расчитывать значения всех вложенных параметров для всех аргументов")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool CalcAllChildParameters
        {
            get
            {
                bool res;
                if (bool.TryParse(GetAttribute(calcAllAttributeName), out res))
                    return res;

                return false;
            }
            set
            {
                SetAttribute(calcAllAttributeName, value.ToString());
            }
        }

        public override bool Equals(object obj)
        {
            bool ret = base.Equals(obj);
            OptimizationGateNode x;
            if (ret && (x = obj as OptimizationGateNode) != null)
            {
                ret = (String.IsNullOrEmpty(Expression) && String.IsNullOrEmpty(x.Expression))
                    || String.Equals(Expression, x.Expression);
                if (ret)
                {
                    ret = (String.IsNullOrEmpty(DefinationDomain) && String.IsNullOrEmpty(x.DefinationDomain))
                        || String.Equals(DefinationDomain, x.DefinationDomain);
                    if (ret)
                    {
                        RevisedStorage<String> args, xargs;

                        if (Attributes.TryGetValue(argsAttributeName, out args) 
                            && x.Attributes.TryGetValue(argsAttributeName, out xargs))
                            ret = args.Equals(xargs);
                    }
                }
                return ret;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// Аргумент для оптимизационных расчетов
    /// </summary>
    [Serializable]
    public class OptimizationArgument : IOptimizationArgument
    {
        const String argumentExpressionStartToken = "@[";
        const String argumentExpressionEndToken = "@]";
        const String intervalStartToken = "@{";
        const String intervalEndToken = "@}";
        const char intervalSeparatorToken = ';';
        const String manualToken = "@.";
        const String columnNumToken = "@|";
        const String argumentEscToken = "@";
        const String argumentEscaptedToken = "@@";

        /// <summary>
        /// Имя аргумента
        /// </summary>
        [DisplayName("Наименование аргумента"),
        Description("Название условного аргумента, используемое в оптимизационных расчетах")]
        public String Name { get; set; }

        /// <summary>
        /// Выражение для расчета возможных значений аргумента
        /// </summary>
        [DisplayName("Значения переменной"),
        Description("Выражение возвращающие массив со значениями, которые необходимо будет проверить во время оптимизационного расчета")]
        public String Expression { get; set; }

        public OptimizationArgumentMode Mode { get; set; }

        public double IntervalFrom { get; set; }

        public double IntervalTo { get; set; }

        public double IntervalStep { get; set; }

        public OptimizationArgument()
        { IntervalFrom = IntervalTo = IntervalStep = double.NaN; }

        public OptimizationArgument(OptimizationArgument argument)
            : this()
        {
            CopyFrom(argument);
        }

        internal void ToString(StringBuilder builder)
        {
            int startPos = builder.Length;
            TokenSerialize(Mode, builder, startPos);
            for (OptimizationArgumentMode mode = OptimizationArgumentMode.Default; mode <= OptimizationArgumentMode.Expression; mode++)
            {
                if (mode != Mode)
                    TokenSerialize(mode, builder, startPos);
            }
            builder.Insert(startPos, Name.Replace(argumentEscToken, argumentEscaptedToken));
        }

        private void TokenSerialize(OptimizationArgumentMode mode, StringBuilder builder, int startPos)
        {
            switch (mode)
            {
                case OptimizationArgumentMode.Interval:
                    if (!double.IsNaN(IntervalFrom) && !double.IsNaN(IntervalTo) && !double.IsNaN(IntervalStep))
                    {
                        builder.Append(intervalStartToken);
                        builder.Append(IntervalFrom.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                        builder.Append(intervalSeparatorToken);
                        builder.Append(IntervalTo.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                        builder.Append(intervalSeparatorToken);
                        builder.Append(IntervalStep.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                        builder.Append(intervalEndToken);
                    }
                    break;
                case OptimizationArgumentMode.Expression:
                    if (!String.IsNullOrEmpty(Expression))
                    {
                        builder.Append(argumentExpressionStartToken);
                        builder.Append(Expression);
                        builder.Append(argumentExpressionEndToken);
                    }
                    break;
                case OptimizationArgumentMode.ColumnNum:
                    if (builder.Length == startPos)
                        builder.Append(columnNumToken);
                    break;
                case OptimizationArgumentMode.Manual:
                default:
                    if (builder.Length == startPos)
                        builder.Append(manualToken);
                    break;
            }
        }

        internal static OptimizationArgument FromString(String str, ref int pos)
        {
            String token;
            double intervalStart, intervalEnd, intervalStep;
            OptimizationArgument optimizationArgs = null;
            int startPos = pos;

            pos = str.IndexOf(argumentEscToken, startPos);
            if (pos < 0)
                pos = str.Length;

            if (pos > startPos)
            {
                optimizationArgs = new OptimizationArgument();
                optimizationArgs.Name = str.Substring(startPos, pos - startPos);

                bool cont = true;
                while (cont && str.Length > pos + 1)
                {
                    token = str.Substring(pos, 2);

                    switch (token)
                    {
                        case argumentExpressionStartToken:
                            pos += token.Length;
                            startPos = pos;
                            while (pos >= startPos && pos < str.Length)
                            {
                                pos = str.IndexOf(argumentEscToken, pos);
                                if (!String.Equals(str.Substring(pos, argumentExpressionEndToken.Length), argumentExpressionEndToken))
                                    pos += argumentEscaptedToken.Length;
                                else break;
                            }

                            if (pos > startPos)
                                optimizationArgs.Expression = str.Substring(startPos, pos - startPos);
                            pos += argumentExpressionEndToken.Length;
                            if (optimizationArgs.Mode == OptimizationArgumentMode.Default)
                                optimizationArgs.Mode = OptimizationArgumentMode.Expression;
                            break;
                        case intervalStartToken:
                            pos += token.Length;
                            startPos = pos;
                            pos = str.IndexOf(intervalEndToken, startPos);
                            if (pos > startPos)
                            {
                                String intervalString = str.Substring(startPos, pos - startPos);
                                String[] intervalArray = intervalString.Split(intervalSeparatorToken);
                                if (intervalArray.Length >= 3
                                    && double.TryParse(intervalArray[0], out intervalStart)
                                    && double.TryParse(intervalArray[1], out intervalEnd)
                                    && double.TryParse(intervalArray[2], out intervalStep))
                                {
                                    optimizationArgs.IntervalFrom = intervalStart;
                                    optimizationArgs.IntervalTo = intervalEnd;
                                    optimizationArgs.IntervalStep = intervalStep;
                                    if (optimizationArgs.Mode == OptimizationArgumentMode.Default)
                                        optimizationArgs.Mode = OptimizationArgumentMode.Interval;
                                }
                            }
                            pos += intervalEndToken.Length;
                            break;
                        case columnNumToken:
                            pos += token.Length;
                            if (optimizationArgs.Mode == OptimizationArgumentMode.Default)
                                optimizationArgs.Mode = OptimizationArgumentMode.ColumnNum;
                            break;
                        case manualToken:
                            pos += token.Length;
                            if (optimizationArgs.Mode == OptimizationArgumentMode.Default)
                                optimizationArgs.Mode = OptimizationArgumentMode.Manual;
                            break;
                        default:
                            cont = false;
                            break;
                    }
                }
            }
            return optimizationArgs;
        }

        public OptimizationArgument(String name, String expression)
            : this()
        {
            this.Name = name;
            this.Expression = expression;
        }

        public void CopyFrom(OptimizationArgument argument)
        {
            this.Name = argument.Name;
            this.Expression = argument.Expression;
            this.Mode = argument.Mode;
            this.IntervalFrom = argument.IntervalFrom;
            this.IntervalTo = argument.IntervalTo;
            this.IntervalStep = argument.IntervalStep;
        }

        public override string ToString()
        {
            //return "Аргумент условного параметра";
            switch (Mode)
            {
                case OptimizationArgumentMode.Manual:
                    return String.Format("{0} (ручной ввод)", Name);
                case OptimizationArgumentMode.Interval:
                    return String.Format("{0} ({1} - {2})/{3}", Name, IntervalFrom, IntervalTo, IntervalStep);
                case OptimizationArgumentMode.Expression:
                    return String.Format("{0} (выражение)", Name);
                case OptimizationArgumentMode.Default:
                default:
                    return Name;
            }
        }

        public override bool Equals(object obj)
        {
            OptimizationArgument ptr = obj as OptimizationArgument;
            if (ptr == null) return false;

            if ((!String.IsNullOrEmpty(this.Name) || !String.IsNullOrEmpty(ptr.Name))
                && !String.Equals(this.Name, ptr.Name))
                return false;
            if ((!String.IsNullOrEmpty(this.Expression) || !String.IsNullOrEmpty(ptr.Expression))
                && !String.Equals(this.Expression, ptr.Expression))
                return false;

            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

