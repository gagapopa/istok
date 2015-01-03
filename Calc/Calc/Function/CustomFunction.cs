using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Класс для расчета пользовательских функций
    /// </summary>
    class CustomFunction : Function
    {
        CustomFunctionInfo functionInfo;

        public CustomFunction(CustomFunctionInfo functionInfo)
            : base(functionInfo.Name, functionInfo.Arguments, functionInfo.GroupName, functionInfo.Comment)
        {
            this.functionInfo = functionInfo;
        }

        public override void Subroutine(ICalcContext context)
        {
            throw new NotImplementedException();
        }
    }
}
