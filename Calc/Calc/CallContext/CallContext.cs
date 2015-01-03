using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Базовая реализация контекста выполнения
    /// </summary>
    abstract class CallContext : ICallContext
    {
        /// <summary>
        /// Выполняемый код
        /// </summary>
        protected Instruction[] body;

        /// <summary>
        /// Индекс текущей выполняемой операции
        /// </summary>
        protected int CurrentIndex { get; set; }

        protected CallContext(DateTime startTime, DateTime endTime)
        {
            this.CalcStartTime = startTime;
            this.CalcEndTime = endTime;
            NeedPrepare = true;
            loopCount = new Stack<int>();
        }

        protected CallContext()
            : this(DateTime.MinValue, DateTime.MinValue)
        {

        }

        public virtual bool TheSame(ICallContext callContext) { return false; }

        #region ICallContext Members

        public Instruction CurrentInstruction
        {
            get
            {
                if (body == null || CurrentIndex < 0 || CurrentIndex > body.Length)
                    return null;

                return body[CurrentIndex];
            }
        }

        public Location CurrentLocation
        {
            get
            {
                if (CurrentInstruction == null)
                    return null;

                return CurrentInstruction.Location;
            }
        }

        public CalcPosition CurrentNode { get; protected set; }

        public virtual String GetStatusString()
        {
#if DEBUG
            return "Расчёт неизвестно чего";
#else
            return "@#$";
#endif
        }

        public DateTime CalcStartTime { get; protected set; }

        public DateTime CalcEndTime { get; protected set; }

        public DateTime StartTime
        {
            get { return GetStartTime(0); }
        }

        public DateTime EndTime
        {
            get
            {
                // TODO костыль что бы пройти тест, требуется срочно убрать это
                if (StartTime == DateTime.MinValue) 
                    return DateTime.MinValue;
                return GetInterval(0, 0).GetNextTime(StartTime);
            }
        }

        public abstract DateTime GetStartTime(int tau);

        public abstract Interval GetInterval(int tau, int tautill);

        /// <summary>
        /// Стек счётчиков циклов
        /// </summary>
        Stack<int> loopCount;

        public void LoopEnter()
        {
            loopCount.Push(0);
        }

        public void LoopLeave()
        {
            loopCount.Pop();
        }

        public int LoopPass()
        {
            int count = loopCount.Pop();
            loopCount.Push(++count);
            return count;
        }

        public bool NeedPrepare
        {
            get;
            protected set;
        }

        public bool Fail { get; set; }

        //public bool IsReturned { get; set; }

        public bool Blocked { get; set; }

        public virtual void Prepare(ICalcContext calcContext)
        {
            CurrentIndex = -1;
            Fail = body == null;
            NeedPrepare = false;
            Blocked = false;

            SetReturnVariableToNothing(calcContext);
        }

        private static void SetReturnVariableToNothing(ICalcContext calcContext)
        {
            // сбросить значение переменной @ret
            Variable retVar = calcContext.SymbolTable.GetSymbol(CalcContext.ReturnVariableName) as Variable;
            if (retVar == null)
                calcContext.SymbolTable.DeclareSymbol(retVar = new Variable(CalcContext.ReturnVariableName));
            retVar.Value = SymbolValue.Nothing;
        }

        public virtual void Return(ICalcContext calcContext)
        {
            NeedPrepare = true;
        }

        public bool NextInstruction(ICalcContext calcContext)
        {
            return Jump(calcContext, CurrentIndex + 1);
        }

        public bool Jump(ICalcContext calcContext, int index)
        {
            if (body == null || Fail)
                return false;

            CurrentIndex = index;

            return 0 <= CurrentIndex && CurrentIndex < body.Length;
        }

        public bool JumpShift(ICalcContext calcContext, int delta)
        {
            return Jump(calcContext, CurrentIndex + delta);
        }

        public string ContextReport()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[");
            if (body != null)
            {
                for (int i = 0; i < body.Length; i++)
                {
                    builder.AppendFormat("\n{0,3}:{2}{1}", i, body[i], i == CurrentIndex ? ">" : " ");
                }
            }
            builder.Append("\n]");
            return builder.ToString();
        }
        #endregion
    }
}
