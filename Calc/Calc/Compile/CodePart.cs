using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Информация о результате компиляции всех дочерних веток узла дерева разбора.
    /// </summary>
    class BranchesInfo
    {
        /// <summary>
        /// Результат компиляции для каждой ветки
        /// </summary>
        public CodePart[] Parts { get; set; }

        /// <summary>
        /// Если во всех дочерних ветках значения (константы), должно выставлять в true.
        /// </summary>
        public bool IsAllValues { get; set; }
    }

    /// <summary>
    /// Результат компиляции всей формулы или какой то её части
    /// </summary>
    class CodePart
    {
        public bool Fail { get; set; }
        
        /// <summary>
        /// Скомпилируемый код
        /// </summary>
        public List<Instruction> Code { get; set; }

        /// <summary>
        /// Адрес с результатом скомпилированной части
        /// </summary>
        public Address Result { get; set; }

        /// <summary>
        /// Если результатом скомпилированной части является условный параметр,
        /// здесь должна хранится информация о данном параметре
        /// </summary>
        public NodeState Parameter { get; set; }

        /// <summary>
        /// Для условных параметров, здесь должны быть адреса со значениями аргументов параметра
        /// </summary>
        public Address[] ParameterArguments { get; set; }

        public CodePart()
        {
            Code = new List<Instruction>();
        }

        /// <summary>
        /// Объеденить данную часть кода с переданной.
        /// Код переданной части будет скопирован в конец,
        /// а результат объединенной части будет равен результату второй части.
        /// </summary>
        /// <param name="codePart"></param>
        public void Add(CodePart codePart)
        {
            if (codePart != null)
            {
                Fail |= codePart.Fail;
                Code.AddRange(codePart.Code);
                Result = codePart.Result;
                Parameter = codePart.Parameter;
                ParameterArguments = codePart.ParameterArguments;
            }
        }
    }
}
