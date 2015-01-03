using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Состояние оптимизационного расчета
    /// </summary>
    public class OptimizationState : CalcState
    {
        /// <summary>
        /// Информация о оптимизации
        /// </summary>
        IOptimizationInfo optimizationInfo;

        ICalcStateStorage calcCache;

        public override ICalcNodeInfo NodeInfo
        {
            get
            {
                return optimizationInfo;
            }
        }

        public OptimizationState(ICalcStateStorage calcCache, IOptimizationInfo optimizationInfo, RevisionInfo revision)
            : base(revision)
        {
            this.calcCache = calcCache;
            this.optimizationInfo = optimizationInfo;

            argumentsBodies = new Dictionary<String, Tuple<int, Instruction[]>>();
        }

        /// <summary>
        /// Хэш оптимизируемого выражения
        /// </summary>
        private int expressionHash;

        /// <summary>
        /// 3-х адресный код оптимизируемого выражения
        /// </summary>
        //public Instruction[] Body { get; protected set; }

        public Instruction[] ExpressionBody { get; protected set; }

        public Instruction[] DDBody { get; protected set; }

        Dictionary<String,Tuple<int, Instruction[]>> argumentsBodies;

        public Instruction[] GetArgumentBody(String argumentName)
        {
            Tuple<int, Instruction[]> body;

            argumentsBodies.TryGetValue(argumentName, out body);

            if (body != null)
                return body.Item2;
            return null;
        }

        /// <summary>
        /// Хэш выражения области определения
        /// </summary>
        private int ddHash;

        /// <summary>
        /// Входные аргументы для оптимизационного расчета
        /// </summary>
        public CalcArgumentInfo[] Arguments
        {
            get
            {
                if (ArgumentsKey != null)
                {
                    List<CalcArgumentInfo> argumentList = new List<CalcArgumentInfo>();
                    foreach (var item in ArgumentsKey)
                        argumentList.AddRange(item.Value);

                    return argumentList.ToArray();
                }
                return null;
            }
        }

        public CalcArgumentInfo[] OptimizationArguments
        {
            get
            {
                if (ArgumentsKey != null)
                {
                    List<CalcArgumentInfo> argumentList = new List<CalcArgumentInfo>();
                    foreach (var item in ArgumentsKey)
                        if (item.Key.Equals(NodeInfo))
                            argumentList.AddRange(item.Value);

                    return argumentList.ToArray();
                }
                return null;
            }
        }

        private KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>[] argumentsKey;
        public KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>[] ArgumentsKey
        {
            get
            {
                if (argumentsKey == null)
                {
                    IOptimizationInfo info = optimizationInfo;
                    Stack<KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>> argsStack =
                        new Stack<KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>>();
                    List<CalcArgumentInfo> infoArgs = new List<CalcArgumentInfo>();

                    while (info != null)
                    {
                        infoArgs.Clear();
                        for (int i = 0; i < info.Arguments.Length; i++)
                            infoArgs.Add(new CalcArgumentInfo(info.Arguments[i].Name, String.Empty, ParameterAccessor.In));

                        argsStack.Push(new KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>(info, infoArgs.ToArray()));
                        info = info.Optimization;
                    }
                    argumentsKey = new KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>[argsStack.Count];
                    for (int i = 0; i < argumentsKey.Length; i++)
                        argumentsKey[i] = argsStack.Pop();
                }
                return argumentsKey;
            }
        }

        public override void Compile(ICompiler compiler, ICalcContext context)
        {
            IOptimizationInfo optimizationInfo=NodeInfo as IOptimizationInfo;

            // компиляция выражения
            if (!String.IsNullOrEmpty(optimizationInfo.Expression) 
                && !expressionHash.Equals(optimizationInfo.Expression.GetHashCode()))
            {
                ExpressionBody = compiler.Compile(context, Revision, optimizationInfo.Expression, ArgumentsKey);
                expressionHash = optimizationInfo.Expression.GetHashCode(); 
            }

            // компиляция области определения
            if (!String.IsNullOrEmpty(optimizationInfo.DefinationDomain)
                && !ddHash.Equals(optimizationInfo.DefinationDomain.GetHashCode()))
            {
                DDBody = compiler.Compile(context, Revision, optimizationInfo.DefinationDomain, ArgumentsKey);
                ddHash = optimizationInfo.DefinationDomain.GetHashCode(); 
            }

            int hash;
            Instruction[] body;

            // компиляция аргументов
            foreach (var item in optimizationInfo.Arguments)
            {
                if (item.Mode == OptimizationArgumentMode.Expression)
                {
                    Tuple<int, Instruction[]> argumentCode;

                    if (argumentsBodies.TryGetValue(item.Name, out argumentCode))
                    {
                        hash = argumentCode.Item1;
                        body = argumentCode.Item2;
                    }
                    else hash = 0;
                    
                    if (!hash.Equals(item.Expression.GetHashCode()))
                    {
                        body = compiler.Compile(context, Revision, item.Expression, null);
                        hash = item.Expression.GetHashCode();

                        argumentsBodies[item.Name] = Tuple.Create(hash, body); 
                    }
                }
            }
        }
    }
}
