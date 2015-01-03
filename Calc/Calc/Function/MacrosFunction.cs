using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Функция-макрос.
    /// </summary>
    /// <remarks>Пока что макросы не поддерживают временные переменные.</remarks>
    class MacrosFunction : Function
    {
        /// <summary>
        /// Тело макроса
        /// </summary>
        Instruction[] code;

        public MacrosFunction(String name, CalcArgumentInfo[] pars, String groupName, String comment, Instruction[] code)
            : base(name, pars, groupName, comment)
        {
            this.code = code;
        }

        public override void Subroutine(ICalcContext context)
        {
        }


        public CodePart CallCode(CompileContext context, Location locat, Tuple<Parameter, Address>[] args)
        {
            //bool newVariable;
            CodePart codePart = new CodePart();

            for (int i = 0; i < code.Length; i++)
            {
                Address a = code[i].A, b = code[i].B, c = code[i].C;

                if (a != null && a.Type == Address.AddressType.Symbol && a.SymbolName.Equals(CalcContext.ReturnVariableName))
                    a = codePart.Result = context.GetTempVariable(codePart.Result);//, out newVariable);

                for (int j = 0; j < args.Length; j++)
                {
                    UseParameter(ref a, args[j].Item1.Name, args[j].Item2);
                    UseParameter(ref b, args[j].Item1.Name, args[j].Item2);
                    UseParameter(ref c, args[j].Item1.Name, args[j].Item2);
                }
                codePart.Code.Add(new Instruction(locat, code[i].Operation, a, b, c));
            }
            return codePart;
        }

        /// <summary>
        /// Заместить упоменание входного аргумента макроса на значение
        /// </summary>
        /// <param name="adr">Проверяемый адрес</param>
        /// <param name="parameterName">Имя вхоного аргумента</param>
        /// <param name="parameterValue">Значение входного аргумента</param>
        private void UseParameter(ref Address adr, String parameterName, Address parameterValue)
        {
            if (adr != null && adr.Type == Address.AddressType.Symbol && String.Equals(adr.SymbolName, parameterName))
                adr = parameterValue;
        }
    }
}
