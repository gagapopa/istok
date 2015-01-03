using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Функция для использования во время расчетов
    /// </summary>
    [Serializable]
    public abstract class Function : Symbol
    {
        public virtual RevisionInfo Revision { get { return RevisionInfo.Default; } }

        /// <summary>
        /// Параметры функции
        /// </summary>
        public CalcArgumentInfo[] Parameters { get; protected set; }

        /// <summary>
        /// Группа функции (для группировки функций в редакторе формул)
        /// </summary>
        public String GroupName { get; set; }

        /// <summary>
        /// Примечания к функции
        /// </summary>
        public String Comment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Имя функции</param>
        /// <param name="pars">Параметры функции</param>
        /// <param name="groupName">Группа функции (для группировки функций в редакторе формул)</param>
        /// <param name="comment">Примечания к функции</param>
        public Function(String name, CalcArgumentInfo[] pars, String groupName, String comment)
            : base(name)
        {
            if (pars == null) Parameters = new CalcArgumentInfo[0];
            else Parameters = pars;
            Comment = comment;
            GroupName = groupName;
        }

        /// <summary>
        /// Получить значения переданные функции
        /// </summary>
        /// <param name="context">Контекст расчета</param>
        /// <returns>Значения аргументов</returns>
        protected Object[] LoadArguments(ICalcContext context)
        {
            Object[] args;
            Variable var;
            args = new Object[Parameters.Length];
            for (int i = 0; i < Parameters.Length; i++)
            {
                var = context.SymbolTable.GetSupSymbol(Parameters[i].Name) as Variable;
                if (var == null)
                {
                    if (!String.IsNullOrEmpty(Parameters[i].DefaultValue))
                        args[i] = double.Parse(Parameters[i].DefaultValue);
                    else
                    {
                        context.AddMessage(new CalcMessage(MessageCategory.Error, "Функции {1} не передан обязательный аргумент {0}", Parameters[i].Name, FunctionString()));
                        return null;
                    }
                }
                else if (var.Value != null && var.Value != SymbolValue.Nothing && var.Value != SymbolValue.BlockedValue)
                    args[i] = var.Value.GetValue();
                else
                {
                    context.AddMessage(new CalcMessage(MessageCategory.Error, "Значение аргумента {0} для функции {1} не допустимого типа", var == null ? Parameters[i].Name : var.ToString(), FunctionString()));
                    return null;
                }
            }
            return args;
        }

        /// <summary>
        /// Вывести прототип функции со значениями аргументов
        /// </summary>
        /// <param name="args">Значения аргументов функции</param>
        /// <returns></returns>
        protected String FunctionString(object[] args)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0}(", Name);
            if (Parameters != null)
                for (int i = 0; i < Parameters.Length; i++)
                {
                    if (i > 0) builder.Append(", ");
                    builder.AppendFormat("{0}={1}", Parameters[i].Name, args != null && args.Length > i ? args[i] : Parameters[i].DefaultValue);
                }
            builder.Append(")");
            return builder.ToString();
        }

        /// <summary>
        /// Вывести прототип функции
        /// </summary>
        /// <returns></returns>
        protected String FunctionString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0}(", Name);
            if (Parameters != null)
                for (int i = 0; i < Parameters.Length; i++)
                {
                    if (i > 0) builder.Append(", ");
                    builder.AppendFormat(Parameters[i].Name);
                    if (!String.IsNullOrEmpty(Parameters[i].DefaultValue)) builder.AppendFormat(" = {0}", Parameters[i].DefaultValue);
                }
            builder.Append(")");
            return builder.ToString();
        }

        /// <summary>
        /// Вызвать функцию
        /// </summary>
        /// <param name="context">Текущий контекст расчета</param>
        /// <returns>Возвращает новый контекст, если для вычисления требуется выполнения кода,
        /// в противном случае null</returns>
        public abstract void Subroutine(ICalcContext context);

        public override string ToString()
        {
            return FunctionString();
        }
    }
}
