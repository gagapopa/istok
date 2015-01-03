using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Функция агрегации параметра
    /// </summary>
    class ParameterFunction : Function
    {
        public const String ParameterDefaultArgumentName = CalcContext.TempVariablePrefix + "node";
        public const String ParameterArgumentName = ParameterDefaultArgumentName + "{0}";
        public const String TauArgumentName = CalcContext.TempVariablePrefix + "tau";
        public const String TauTillArgumentName = CalcContext.TempVariablePrefix + "tautill";
        public const String ParameterArgumentNameFormat = CalcContext.TempVariablePrefix + "{0}" + CalcContext.TempVariablePrefix + "{1}";

        protected ICalcSupplier calcSupplier;

        /// <summary>
        /// Метод агригации
        /// </summary>
        public CalcAggregation Aggregation { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Имя функции</param>
        /// <param name="agreg">Метод агригации</param>
        /// <param name="groupName">Группа функции (для группировки функций в редакторе формул)</param>
        /// <param name="comment">Примечания к функции</param>
        public ParameterFunction(ICalcSupplier calcSupplier, String name, CalcAggregation agreg, String groupName, String comment)
            : base(name, GetArgumetns(agreg), groupName, comment)
        {
            if (agreg == CalcAggregation.Nothing)
                throw new InvalidOperationException("Невозможно расчитать агрегацию Nothing!");
            this.calcSupplier = calcSupplier;
            Aggregation = agreg;
        }

        private static CalcArgumentInfo[] GetArgumetns(CalcAggregation agreg)
        {
            int parameterCount = GetParameterCount(agreg);

            var args = new CalcArgumentInfo[parameterCount + 2];

            for (int i = 0; i < parameterCount; i++)
            {
                String name = GetParameterName(parameterCount, i);
                args[i] = new CalcArgumentInfo(name, null, ParameterAccessor.In);
            }
            args[parameterCount] = new CalcArgumentInfo(TauArgumentName, String.Empty, "0", ParameterAccessor.In);
            args[parameterCount + 1] = new CalcArgumentInfo(TauTillArgumentName, String.Empty, TauArgumentName, ParameterAccessor.In);

            return args;
        }

        private static String GetParameterName(int parameterCount, int i)
        {
            if (parameterCount < 2)
                return ParameterDefaultArgumentName;

            return String.Format(ParameterArgumentName, i + 1);
        }


        public String GetParameterName(int nodeNum, String argumentName)
        {
            return String.Format(ParameterArgumentNameFormat, nodeNum + 1, argumentName);
        }


        public int ParameterCount
        {
            get
            {
                return GetParameterCount(Aggregation);
            }
        }

        private static int GetParameterCount(CalcAggregation agreg)
        {
            int parameterCount;

            switch (agreg)
            {
                case CalcAggregation.Weighted:
                    parameterCount = 2;
                    break;
                default:
                    parameterCount = 1;
                    break;
            }
            return parameterCount;
        }

        protected String[] GetNode(ICalcContext context)
        {
            int parameterCount = GetParameterCount(Aggregation);

            String[] ret = new String[parameterCount];
            Variable par;

            for (int i = 0; i < parameterCount; i++)
            {
                par = context.SymbolTable.GetSymbol(GetParameterName(parameterCount, i)) as Variable;

                ret[i] = par.Value.GetValue().ToString();
            }
            return ret;
        }

        protected void GetTimeInterval(ICalcContext context, out DateTime startTime, out Interval interval)//, out DateTime endTime)
        {
            int tau, tautill;
            Variable par;

            par = context.SymbolTable.GetSymbol(TauArgumentName) as Variable;
            tau = Convert.ToInt32(par.Value.GetValue());
            par = context.SymbolTable.GetSymbol(TauTillArgumentName) as Variable;
            if (String.Equals(par.Value.GetValue(), TauArgumentName))
                tautill = tau;
            else
                tautill = Convert.ToInt32(par.Value.GetValue());

            startTime = context.SymbolTable.CallContext.GetStartTime(tau);
            interval = context.SymbolTable.CallContext.GetInterval(tau, tautill);
        }

        public override void Subroutine(ICalcContext context)
        {
            DateTime startTime;
            Interval interval;

            // обработка переданных аргументов
            GetTimeInterval(context, out startTime, out interval);

            var revisions = calcSupplier.GetRevisions(startTime, interval.GetNextTime(startTime));

            List<CalcNodeKey> calNodeKeyList = new List<CalcNodeKey>();
            List<CalcNodeKey> calculatedNodeKeyList = new List<CalcNodeKey>();

            String[] parameterCodes = GetNode(context);

            // выставляется в true, если хотябы один параметр в хотябы одной ревизии обязателен к расчёту
            bool forceCalc = false;
            bool isCalculable = false;

            for (int i = 0; i < parameterCodes.Length; i++)
            {
                ICalcNode calcNode = null;
                IOptimizationInfo optimizationInfo = null;
                ArgumentsValues args = null;

                String parameterCode = parameterCodes[i];

                foreach (var revision in revisions)
                {
                    IParameterInfo parameterInfo = calcSupplier.GetParameterNode(context, revision, parameterCode);
                    ICalcNode revisionCalcnode = parameterInfo.CalcNode;
                    IOptimizationInfo revisionOptimizationInfo = parameterInfo.Optimization;

                    if (calcNode == null)
                        calcNode = revisionCalcnode;
                    else if (!calcNode.Equals(revisionCalcnode))
                        context.AddMessage(new CalcMessage(MessageCategory.Error, "Параметр ${0}$ за запрашиваемый период сменил код", parameterCode));

                    if (optimizationInfo == null)
                        optimizationInfo = revisionOptimizationInfo;
                    else if (!optimizationInfo.Equals(revisionOptimizationInfo))
                        context.AddMessage(new CalcMessage(MessageCategory.Error, "У параметра ${0}$ за запрашиваемый период изменились настройки оптимизации", parameterCode));

                    if (parameterInfo.Calculable)
                    {
                        forceCalc |= context.ForceCalc(parameterInfo);
                        isCalculable = true;
                    }
                }
                if (optimizationInfo != null)
                {
                    bool fail;
                    args = GetArgumentsValues(context, parameterCode, optimizationInfo, i, out fail);
                    if (fail)
                        return;
                }
                calNodeKeyList.Add(new CalcNodeKey(calcNode, args));
                //if (isCalculable)
                //{
                    calculatedNodeKeyList.Add(new CalcNodeKey(calcNode, args));
                //}
            }

            // запрос значений из кэша
            SymbolValue value = context.ValuesKeeper.GetValue(Aggregation, startTime, interval, calNodeKeyList.ToArray());

            // запрос значение из БД
            // 1. если среди параметров нет расчитываемых
            // 2. если параметры расчитываемы и алгоритм агрегации не Exist || Count
            // 3. если параметры расчитываемы и не установлен флаг RecalcAll
            if (value == null
                && (!isCalculable//calculatedNodeKeyList.Count == 0
                || (Aggregation != CalcAggregation.Exist 
                && Aggregation != CalcAggregation.Count
                && !forceCalc)))
            {
                // здесь должен быть пропус для расчитываемых параметров, когда ForceCalc
                value = GetValue(context, startTime, interval, calNodeKeyList.ToArray());
            }
            // запуск расчёта
            if (value == null
                && isCalculable)// calculatedNodeKeyList.Count > 0)
            {
                ICallContext callContext = context.SymbolTable.CallContext;
                // проверка на Calculable
                if (context.Call(startTime, interval.GetNextTime(startTime), calculatedNodeKeyList.ToArray()))
                {
                    callContext.JumpShift(context, -1);
                    return;
                }
            }

            // если значения нет, сообщить об ошибке
            if (value == null || value == SymbolValue.Nothing)
            {
                CalcMessage mess;
                StringBuilder argBuilder = new StringBuilder();

                context.AddMessage(mess = new CalcMessage(MessageCategory.Error, "Агрегация {0}({1}) не может быть расчитана за '{2}'", Name, GetParams(calNodeKeyList, startTime), startTime));
                value = SymbolValue.Nothing;
            }
            Variable var;
            if ((var = context.SymbolTable.GetSymbol(CalcContext.ReturnVariableName) as Variable) != null) var.Value = value;
        }

        private string GetParams(IEnumerable<CalcNodeKey> calcNodes, DateTime startTime)
        {
            StringBuilder builder = new StringBuilder();

            var revision = calcSupplier.GetRevision(startTime);

            foreach (var calcNode in calcNodes)
            {
                if (builder.Length > 0)
                    builder.Append(",");
                builder.AppendFormat("${0}$", calcNode.Node.GetParameter(revision).Code);
                if (calcNode.Arguments != null)
                    builder.AppendFormat("({0})", calcNode.Arguments);
            }

            return builder.ToString();
        }

        protected ArgumentsValues GetArgumentsValues(ICalcContext calcContext,String parameterCode, IOptimizationInfo optimizationInfo, int nodeNum, out bool fail)
        {
            ArgumentsValues arguments;
            Stack<IOptimizationInfo> optimizatinStack = new Stack<IOptimizationInfo>();
            IOptimizationInfo info = optimizationInfo;

            while (info!=null)
            {
                optimizatinStack.Push(info);
                info = info.Optimization;
            }

            arguments = new ArgumentsValues();
            fail = false;
            bool imp = false;

            while (optimizatinStack.Count > 0)
            {
                int count = 0;

                info = optimizatinStack.Pop();

                foreach (var argument in info.Arguments)
                {
                    Variable var = calcContext.SymbolTable.GetSymbol(GetParameterName(nodeNum, argument.Name)) as Variable;

                    if (var == null
                        || var.Value == SymbolValue.Nothing
                        || var.Value == SymbolValue.BlockedValue                        )
                    {
                        break;
                    }
                    arguments[argument.Name] = (double)var.Value;
                    ++count;
                    if (imp)
                        break;
                }

                imp |= info.Calculable && count == 0;

                // Если аргументы переданы не полностью или не переданны совсем для нерасчитываемой оптимизации,
                // или неявная передача аргументов не по порядку с конца
                // сообщить об ошибке и выйти
                if ((imp && count > 0) 
                    || (count < info.Arguments.Length && (!info.Calculable || count > 0)))
                {
                    calcContext.AddMessage(new CalcMessage(MessageCategory.Error, "Параметр ${0}$ должен вызываться с аргументами ({1})", parameterCode, GetArgsString(optimizationInfo)));
                    fail = true;
                    break;
                }
                else fail = false;
            }

            if (arguments.Count == 0)
                return null;

            return arguments;
        }

        private String GetArgsString(IOptimizationInfo optimizationInfo)
        {
            StringBuilder builder = new StringBuilder();
            if (optimizationInfo.Optimization != null)
                builder.Append(GetArgsString(optimizationInfo.Optimization));

            foreach (var argument in optimizationInfo.Arguments)
            {
                if (builder.Length > 0)
                    builder.Append(", ");
                builder.Append(argument.Name);
            }

            return builder.ToString();
        }

        private bool IsCalculable(IOptimizationInfo optimizationInfo)
        {
            bool ret = optimizationInfo != null;
            IOptimizationInfo info = optimizationInfo;

            while (ret && info != null)
            {
                ret = info.Calculable;
                info = info.Optimization;
            }
            return ret;
        }

        /// <summary>
        /// Запросить значение парамтера
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameterInfo">Информация о параметре</param>
        /// <param name="argsKey"></param>
        /// <param name="startTime">Начальное время</param>
        /// <param name="endTime">Конечное время</param>
        /// <returns>Значение параметра</returns>
        private SymbolValue GetValue(ICalcContext context, DateTime startTime, Interval interval/*DateTime endTime*/, CalcNodeKey[] nodes)
        {
            SymbolValue val = null;
            bool serverNotAccessible;
            List<Message> messages;
            ParamValueItem receiveItem;

            receiveItem = calcSupplier.GetParameterNodeValue(
                                        context,
                                        from n in nodes select Tuple.Create(n.Node, n.Arguments),
                                        Aggregation,
                                        startTime,
                                        interval,
                                        out  messages,
                                        out serverNotAccessible);

            if (serverNotAccessible)
            {
                context.SymbolTable.CallContext.Blocked = true;
                val = SymbolValue.BlockedValue;
            }
            else if (receiveItem != null && !double.IsNaN(receiveItem.Value))
                val = new DoubleValue(receiveItem.Value);

            if (messages != null)
                context.AddMessage(messages);

            if (val != null)
                context.ValuesKeeper.AddValue(Aggregation, startTime, interval, val, nodes);

            return val;
        }
    }
}
