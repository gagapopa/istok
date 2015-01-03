using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Доступ к параметру
    /// </summary>
    [Flags]
    public enum ParameterAccessor { In = 1, Out = 2, InOut = In | Out }

    /// <summary>
    /// Параметр, передающийся в качестве аргумента функции
    /// </summary>
    [Serializable]
    public class Parameter : Variable
    {
        /// <summary>
        /// Доступ к параметру
        /// </summary>
        public ParameterAccessor ParameterAccessor { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <param name="value">Значение по умолчанию</param>
        /// <param name="accessor">Доступ</param>
        public Parameter(String name, SymbolValue value, ParameterAccessor accessor)
            : base(name, value)
        {
            ParameterAccessor = accessor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Имя парамтетра</param>
        /// <param name="accessor">Доступ</param>
        public Parameter(String name, ParameterAccessor accessor) : this(name, null, accessor) { }

        /// <summary>
        /// Конструктор копий
        /// </summary>
        /// <param name="x"></param>
        public Parameter(Parameter x) : this(x.Name, x.Value, x.ParameterAccessor) { }

        public Parameter(CalcArgumentInfo info)
            : this(info.Name, SymbolValue.ValueFromString(info.DefaultValue), info.ParameterAccessor)
        {

        }

        public override object Clone()
        {
            return new Parameter(this);
        }
    }
}
