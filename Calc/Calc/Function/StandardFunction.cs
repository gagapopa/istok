using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Стандартные функции
    /// </summary>
    class StandardFunction : Function
    {
        /// <summary>
        /// Вызываемая функция
        /// </summary>
        protected MethodInfo functionBody;

        /// <summary>
        /// Вызываемый объект (null если функция статическая)
        /// </summary>
        private Object targetObject;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Имя функции</param>
        /// <param name="target">Вызываемый объект</param>
        /// <param name="func">Вызываемая фугкция</param>
        /// <param name="groupName">Группа функции (для группировки функций в редакторе формул)</param>
        /// <param name="comment">Примечания к функции</param>
        public StandardFunction(String name, Object target, MethodInfo method, String groupName, String comment)
            : base(name, null, groupName, comment)
        {
            functionBody = method;
            ParameterInfo[] pars = functionBody.GetParameters();
            Parameters = new CalcArgumentInfo[pars.Length];
            for (int i = 0; i < Parameters.Length; i++)
            {
                String defVal;
                Object val = pars[i].DefaultValue;
                if (val == null) defVal = null;
                else defVal = val.ToString();
                Parameters[i] = new CalcArgumentInfo(pars[i].Name, defVal, ParameterAccessor.In);
            }

            targetObject = target;
        }

        /// <summary>
        /// Создание функции, вызывающий статический метод
        /// </summary>
        /// <param name="name">Имя функции</param>
        /// <param name="func">Вызываемая фугкция</param>
        /// <param name="groupName">Группа функции (для группировки функций в редакторе формул)</param>
        /// <param name="comment">Примечания к функции</param>
        public StandardFunction(String name, MethodInfo func, String groupName, String comment) : this(name, null, func, groupName, comment) { }

        public override void Subroutine(ICalcContext context)
        {
            InvokeMethod(context, targetObject);
        }

        protected void InvokeMethod(ICalcContext context, Object target)
        {
            Variable var;
            Object ret;
            Object[] args = new Object[Parameters.Length];
            for (int i = 0; i < Parameters.Length; i++)
            {
                Variable par = context.SymbolTable.GetSupSymbol(Parameters[i].Name) as Variable;
                if (par == null) context.AddMessage(new CalcMessage(MessageCategory.CriticalError, "Сбой сервера расчета"));
                else args[i] = par.Value.GetValue();
            }
            ret = functionBody.Invoke(target, args);
            if ((var = context.SymbolTable.GetSymbol(CalcContext.ReturnVariableName) as Variable) != null) var.Value = SymbolValue.CreateValue(ret);
        }
    }
}
