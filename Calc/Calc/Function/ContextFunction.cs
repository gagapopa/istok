using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Функция предоставляющая во время расчета информацию из текущего контекста
    /// </summary>
    class ContextFunction : Function
    {
        public ContextFunction(String name, String groupName, String comment)
            : base(name, null, groupName, comment)
        { }

        public override void Subroutine(ICalcContext context)
        {
            Variable retVar = context.SymbolTable.GetSymbol(CalcContext.ReturnVariableName) as Variable;
            if (retVar != null)
            {
                switch (Name)
                {
                    case "GetInterval":
                        //retVar.Value = SymbolValue.CreateValue(Interval.GetIntervalInSeconds(context.SymbolTable.CallContext.StartTime, context.SymbolTable.CallContext.EndTime));
                        retVar.Value = SymbolValue.CreateValue((context.SymbolTable.CallContext.EndTime - context.SymbolTable.CallContext.StartTime).TotalSeconds);
                        break;
                    case "GetStartYear":
                        retVar.Value = SymbolValue.CreateValue(context.SymbolTable.CallContext.StartTime.Year);
                        break;
                    case "GetStartMonth":
                        retVar.Value = SymbolValue.CreateValue(context.SymbolTable.CallContext.StartTime.Month);
                        break;
                    case "GetStartDay":
                        retVar.Value = SymbolValue.CreateValue(context.SymbolTable.CallContext.StartTime.Day);
                        break;
                    case "GetEndYear":
                        retVar.Value = SymbolValue.CreateValue(context.SymbolTable.CallContext.EndTime.Year);
                        break;
                    case "GetEndMonth":
                        retVar.Value = SymbolValue.CreateValue(context.SymbolTable.CallContext.EndTime.Month);
                        break;
                    case "GetEndDay":
                        retVar.Value = SymbolValue.CreateValue(context.SymbolTable.CallContext.EndTime.Day);
                        break;
                    case "GetDaysInMonth":
                        DateTime time = context.SymbolTable.CallContext.StartTime;
                        retVar.Value = SymbolValue.CreateValue(time.AddMonths(1).Subtract(time).TotalDays);
                        break;
                    default:
                        retVar.Value = SymbolValue.Nothing;
                        break;
                }
            }
        }
    }
}
