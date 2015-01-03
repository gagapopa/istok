using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Обертка для вызова внешних функций
    /// </summary>
    class ExternalFunction : Function
    {
        IExternalFunctionInfo functionInfo;

        public override RevisionInfo Revision
        {
            get
            {
                return functionInfo.Revision;
            }
        }

        public ExternalFunction(IExternalFunctionInfo functionInfo)
            : base(functionInfo.Name, functionInfo.Arguments, functionInfo.GroupName, functionInfo.Description)
        {
            this.functionInfo = functionInfo;
        }

        public override void Subroutine(ICalcContext context)
        {
            Variable retVariable;
            Object[] args = LoadArguments(context);
            Object ret;

            if (args != null && (retVariable = context.SymbolTable.GetSupSymbol(CalcContext.ReturnVariableName) as Variable) != null)
            {
                try
                {
                    ret = functionInfo.Call(args);
                }
                catch (Exception exc)
                {
                    String mess = exc.Message;
#if DEBUG
                    mess += String.Format("({0})", exc.StackTrace);
#endif
                    context.AddMessage(new CalcMessage(MessageCategory.Error, "При выполнении функции '{0}' произошла ошибка '{1}'", FunctionString(args), mess));
                    ret = SymbolValue.Nothing;
                }
                if (ret is double) retVariable.Value = new DoubleValue(Convert.ToDouble(ret));
                else retVariable.Value = SymbolValue.Nothing;
            }
        }
    }
}
