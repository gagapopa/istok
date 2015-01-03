using System;
using System.Collections.Generic;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Текущие состояние расчитываемого параметра
    /// </summary>
    public class NodeState : CalcState
    {
        /// <summary>
        /// Параметр
        /// </summary>
        IParameterInfo parameterInfo;

        public override ICalcNodeInfo NodeInfo
        {
            get
            {
                return parameterInfo;
            }
        }

        /// <summary>
        /// Скомпилированный код для расчета параметра
        /// </summary>
        public Instruction[] Body { get; set; }

        private int formulaHash;

        /// <summary>
        /// Скомпилировать текущий узел
        /// </summary>
        /// <param name="compiler">Компилятор</param>
        /// <param name="context">Текущий контекст</param>
        public override void Compile(ICompiler compiler, ICalcContext context)
        {
            if (NodeInfo.Calculable)
            {
                KeyValuePair<IOptimizationInfo, CalcArgumentInfo[]>[] arguments = null;

                if ((!Failed && Body == null)
                    || !formulaHash.Equals(parameterInfo.Formula.GetHashCode()))
                {
                    if (parameterInfo.Optimization != null)
                    {
                        OptimizationState optimizationState = context.GetOptimization(parameterInfo.Optimization, Revision);
                        arguments = optimizationState.ArgumentsKey;
                    }

                    Body = compiler.Compile(context, Revision, parameterInfo.Formula, arguments);
                    //if (Failed)
                    //    Body = null;
                    Failed = Body == null;
                    formulaHash = Body != null ? parameterInfo.Formula.GetHashCode() : 0;
                } 
            }
        }

        /// <summary>
        /// true, если запрос значений из блочного или расчет параметра не возможен
        /// </summary>
        public bool Blocked { get; set; }

        public NodeState(IParameterInfo nodeInfo, RevisionInfo revision)
            : base(revision)
        {
            parameterInfo = nodeInfo;
        }
    }
}
