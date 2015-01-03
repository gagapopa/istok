using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Библиотечная функция, подгружаемая из библиотек написанных на c#
    /// </summary>
    [Serializable]
    class LibraryFunction : Function
    {
        RemoteFunctionManager functionManager;

        public LibraryFunction(RemoteFunctionManager manager, String name, CalcArgumentInfo[] pars, String groupName, String comment)
            : base(name, pars, groupName, comment)
        { functionManager = manager; }

        public override void Subroutine(ICalcContext context)
        {
            Variable retVariable;
            Object[] args;

            args = LoadArguments(context);
            if (args != null && (retVariable = context.SymbolTable.GetSupSymbol(CalcContext.ReturnVariableName) as Variable) != null)
            {
                CalcMessage mess;
                retVariable.Value = functionManager.CallFunction(Name, args, out mess);
                if (mess != null)
                    context.AddMessage(new CalcMessage(mess));
            }
        }
    }
}
