using System;
using System.Linq;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Интерпретатор расчетного модуля
    /// </summary>
    class Interpreter
    {
        /// <summary>
        /// Основной интерпритирующий метод
        /// </summary>
        /// <param name="context">Контекст расчёта</param>
        public void Exec(ICalcContext calcContext)
        {
            Instruction ci;
            SymbolValue ax, bx;
            IValueRefference v;

            try
            {
                while (calcContext.NextNode())
                {
                    ICallContext context;

                    while ((context = PrepareContext(calcContext)) != null
                        && context.NextInstruction(calcContext))
                    {
                        ci = context.CurrentInstruction;

                        switch (ci.Operation)
                        {
                            case Instruction.OperationCode.Call:
                                Function func;
                                if ((func = calcContext.SymbolTable.GetFunction(context.StartTime, ci.A.SymbolName) as Function) != null)
                                {
                                    func.Subroutine(calcContext);
                                }
                                else calcContext.AddMessage(new CalcMessage(MessageCategory.CriticalError, "Функция '{0}' не найдена", ci.A.SymbolName));
                                break;
                            case Instruction.OperationCode.Return:
                                context.Return(calcContext);
                                break;
                            case Instruction.OperationCode.Addition:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);
                                bx = GetValue(calcContext, ci.C);

                                if (!context.Fail)
                                    v.Value = ax.Addition(calcContext, bx);
                                break;
                            case Instruction.OperationCode.Subtraction:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);
                                bx = GetValue(calcContext, ci.C);

                                if (!context.Fail)
                                    v.Value = ax.Subtraction(calcContext, bx);
                                break;
                            case Instruction.OperationCode.Multiplication:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);
                                bx = GetValue(calcContext, ci.C);

                                if (!context.Fail)
                                    v.Value = ax.Multiplication(calcContext, bx);
                                break;
                            case Instruction.OperationCode.Division:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);
                                bx = GetValue(calcContext, ci.C);

                                if (!context.Fail)
                                    v.Value = ax.Division(calcContext, bx);
                                break;
                            case Instruction.OperationCode.Modulo:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);
                                bx = GetValue(calcContext, ci.C);

                                if (!context.Fail)
                                    v.Value = ax.Modulo(calcContext, bx);
                                break;
                            case Instruction.OperationCode.UnaryPlus:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);

                                if (!context.Fail)
                                    v.Value = ax.UnaryPlus(calcContext);
                                break;
                            case Instruction.OperationCode.UnaryMinus:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);

                                if (!context.Fail)
                                    v.Value = ax.UnaryMinus(calcContext);
                                break;
                            case Instruction.OperationCode.IncrementPrefix:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);

                                if (!context.Fail)
                                    v.Value = ax.IncrementPrefix(calcContext);
                                break;
                            case Instruction.OperationCode.IncrementSuffix:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);

                                v.Value = ax.IncrementSuffix(calcContext);
                                break;
                            case Instruction.OperationCode.DecrementPrefix:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);

                                if (!context.Fail)
                                    v.Value = ax.DecrementPrefix(calcContext);
                                break;
                            case Instruction.OperationCode.DecrementSuffix:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);

                                v.Value = ax.DecrementSuffix(calcContext);
                                break;
                            case Instruction.OperationCode.Move:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);

                                if (!context.Fail)
                                    v.Value = ax.CopyValue(calcContext);
                                break;
                            case Instruction.OperationCode.Equal:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);
                                bx = GetValue(calcContext, ci.C);

                                if (!context.Fail)
                                    v.Value = ax.Equal(calcContext, bx);
                                break;
                            case Instruction.OperationCode.NotEqual:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);
                                bx = GetValue(calcContext, ci.C);

                                if (!context.Fail)
                                    v.Value = ax.NotEqual(calcContext, bx);
                                break;
                            case Instruction.OperationCode.Greater:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);
                                bx = GetValue(calcContext, ci.C);

                                if (!context.Fail)
                                    v.Value = ax.Greater(calcContext, bx);
                                break;
                            case Instruction.OperationCode.Less:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);
                                bx = GetValue(calcContext, ci.C);

                                if (!context.Fail)
                                    v.Value = ax.Less(calcContext, bx);
                                break;
                            case Instruction.OperationCode.GreaterOrEqual:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);
                                bx = GetValue(calcContext, ci.C);

                                if (!context.Fail)
                                    v.Value = ax.GreaterOrEqual(calcContext, bx);
                                break;
                            case Instruction.OperationCode.LessOrEqual:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);
                                bx = GetValue(calcContext, ci.C);

                                if (!context.Fail)
                                    v.Value = ax.LessOrEqual(calcContext, bx);
                                break;
                            case Instruction.OperationCode.LogicalNot:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);

                                if (!context.Fail)
                                    v.Value = ax.IsZero(calcContext) ? SymbolValue.TrueValue : SymbolValue.FalseValue;
                                break;
                            case Instruction.OperationCode.ArrayDecl:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);

                                v.Value = new ArrayValue(calcContext, Convert.ToInt32(ax.GetValue()));
                                break;
                            case Instruction.OperationCode.ArrayLength:
                                v = GetVariable(calcContext, ci.A);
                                ax = GetValue(calcContext, ci.B);

                                if (!context.Fail)
                                    v.Value = new DoubleValue(ax.ArrayLength(calcContext));
                                break;
                            case Instruction.OperationCode.Jump:
                                if (ci.A.Type == Address.AddressType.Label)
                                    context.JumpShift(calcContext, ci.A.ReferenceIndex - 1);
                                break;
                            case Instruction.OperationCode.JumpZ:
                                if (ci.B.Type == Address.AddressType.Label)
                                {
                                    ax = GetValue(calcContext, ci.A);
                                    if (ax.IsZero(calcContext))
                                        context.JumpShift(calcContext, ci.B.ReferenceIndex - 1);
                                }
                                break;
                            case Instruction.OperationCode.JumpNZ:
                                if (ci.B.Type == Address.AddressType.Label)
                                {
                                    ax = GetValue(calcContext, ci.A);
                                    if (!ax.IsZero(calcContext))
                                        context.JumpShift(calcContext, ci.B.ReferenceIndex - 1);
                                }
                                break;
                            case Instruction.OperationCode.NOP:
                                break;
                            case Instruction.OperationCode.LoopEnter:
                                context.LoopEnter();
                                break;
                            case Instruction.OperationCode.LoopPass:
                                if (context.LoopPass() > calcContext.MaxLoopCount)
                                    calcContext.AddMessage(new CalcMessage(MessageCategory.Error, "Цикл был выполнен более максимольного допустимого количества раз ({0}) и был прерван.", calcContext.MaxLoopCount));
                                break;
                            case Instruction.OperationCode.LoopLeave:
                                context.LoopLeave();
                                break;
                            case Instruction.OperationCode.EnterLevel:
                                calcContext.SymbolTable.PushSymbolScope();
                                break;
                            case Instruction.OperationCode.LeaveLevel:
                                calcContext.SymbolTable.PopSymbolScope();
                                break;
                            case Instruction.OperationCode.VarDecl:
                                if (ci.A.Type == Address.AddressType.Symbol)
                                {
                                    if (ci.B != null)
                                        ax = GetValue(calcContext, ci.B);
                                    else
                                        ax = SymbolValue.Nothing;
                                    calcContext.SymbolTable.DeclareSymbol(new Variable(ci.A.SymbolName, ax));
                                } break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                CalcMessage msg = new CalcMessage(MessageCategory.CriticalError, "Сбой во время расчёта {0}{1}", exc.Message, calcContext.ContextStateReport());
                calcContext.AddMessage(msg);
                throw;
            }
        }

        private ICallContext PrepareContext(ICalcContext calcContext)
        {
            ICallContext context = null;
            ICallContext callContext = calcContext.SymbolTable.CallContext;

            while (callContext != null && (callContext.Fail || callContext.NeedPrepare))
            {
                bool fail = callContext.Fail;
                bool needPrepare = callContext.NeedPrepare;

                if (callContext.Fail)
                {
                    callContext.Return(calcContext);
                }

                callContext.Prepare(calcContext);

                context = callContext;
                callContext = calcContext.SymbolTable.CallContext;

                if (context == callContext && fail == callContext.Fail && needPrepare == callContext.NeedPrepare)
                {
                    // возможно, здесь луше использовать ексепшен
                    calcContext.AddMessage(new CalcMessage(MessageCategory.CriticalError, "Зацикливоние при подготовке контекстов расчёта к работе"));
                }

            }

            return callContext;
        }

        /// <summary>
        /// Поучить переменную по адрессу
        /// Если указанная переменная не объявленна, объявляет переменную
        /// </summary>
        /// <param name="context">Текущий контекст расчета</param>
        /// <param name="address">Адрес для переменной</param>
        /// <param name="addressType">Тип адреса</param>
        /// <returns>Если адрес не является переменной, вернет null</returns>
        private IValueRefference GetVariable(ICalcContext context, Address address)
        {
            return GetVariable(context, address, true);
        }

        private IValueRefference GetVariable(ICalcContext context, Address address, bool declare)
        {
            IValueRefference ret = null;
            if (address.Type == Address.AddressType.ArrayElement)
            {
                ret = GetArrayHolder(context, address);
            }
            else if (address.Type == Address.AddressType.Symbol)
            {
                if ((ret = context.SymbolTable.GetSymbol(address.SymbolName, address.SkipTopFrame) as Variable) == null)
                {
                    if (declare)
                        ret = context.SymbolTable.DeclareSymbol(new Variable(address.SymbolName), false, true) as Variable;
                    else
                        context.AddMessage(new CalcMessage(MessageCategory.Error, "Использование не инициированной переменной {0}", address.SymbolName));
                }
            }

            return ret;
        }

        private IValueRefference GetArrayHolder(ICalcContext context, Address address)
        {
            SymbolValue arrayVallue = GetValue(context, address.ArrayAddress) ;
            ArrayValue array = arrayVallue as ArrayValue;
            SymbolValue index = GetValue(context, address.ArrayIndex);

            if (array == null)
                context.AddMessage(new CalcMessage(MessageCategory.Error, "Использование к {0} как к массиву", arrayVallue == null ? address.ArrayAddress.ToString() : arrayVallue.ToString()));
            else if (!(index.GetValue() is double))
                context.AddMessage(new CalcMessage(MessageCategory.Error, "Не верный тип для индекса массива", index));
            else
                return new ArrayElementRefference(context, array, (int)(double)index.GetValue());

            return null;
        }

        /// <summary>
        /// Получить значение по адресу
        /// </summary>
        /// <param name="context">Текущий контекст расчета</param>
        /// <param name="address">Адрес</param>
        /// <param name="addressType">Тип адреса</param>
        /// <returns>Значение хранимаое по адресу</returns>
        private SymbolValue GetValue(ICalcContext context, Address address)
        {
            Variable t;
            SymbolValue v = SymbolValue.Nothing;

            if (address.Type == Address.AddressType.ArrayElement)
            {
                IValueRefference holder = GetArrayHolder(context, address);
                if (holder != null)
                    v = holder.Value;
            }
            else if (address.Type == Address.AddressType.Symbol)
            {
                if ((t = context.SymbolTable.GetSymbol(address.SymbolName, address.SkipTopFrame) as Variable) != null)
                    v = t.Value;
                else 
                    context.AddMessage(new CalcMessage(MessageCategory.Error, "Использование не инициированной переменной {0}", address.SymbolName));
            }
            else if (address.Type == Address.AddressType.Value)
                v = address.Value;
            else if (address.Type == Address.AddressType.Parameter)
                v = SymbolValue.CreateValue(address.SymbolName);
            
            return v;
        }
    }
}
