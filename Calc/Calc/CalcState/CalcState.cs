using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Базовый класс для состояния расчета
    /// </summary>
    public abstract class CalcState : ICalcState
    {
        protected CalcState(RevisionInfo revision)
        {
            this.Revision = revision;
        }

        /// <summary>
        /// Скомпилировать расчитываемые формулы, если нужно
        /// </summary>
        /// <param name="compiler">Компилятор</param>
        /// <param name="context">Контекст расчета</param>
        public abstract void Compile(ICompiler compiler, ICalcContext context);

        public bool Failed { get; set; }

        /// <summary>
        /// Параметр
        /// </summary>
        public abstract ICalcNodeInfo NodeInfo { get; }

        public virtual CalcPosition CalcPosition
        {
            get
            {
                return new CalcPosition()
                {
                    NodeID = NodeInfo.CalcNode.NodeID,
                    Intime = CalcPosition.IntimeIdentification.Compiletime,
                    //CurrentPart = CalcPosition.NodePart.Formula
                };
            }
        }


        public RevisionInfo Revision { get; private set; }
    }
}
