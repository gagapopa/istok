using System;
using System.Collections.Generic;
using System.Text;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Код сообщения
    /// </summary>
    public enum MessageCode
    {
        /// <summary>
        /// Двойное объявление переменной
        /// </summary>
        DoubleDeclaration = 0x0B000001,
        /// <summary>
        /// Присваивание не к переменной
        /// </summary>
        InvalidAssigmentOperation = 0x0B000011,
        /// <summary>
        /// Не верная операция
        /// </summary>
        InvalidOperation = 0x0B000021,
        /// <summary>
        /// Параметр не определен
        /// </summary>
        UdefinedParameter = 0x0B000031,
        /// <summary>
        /// Ошибка в функции
        /// </summary>
        ErrorInFunctionCall = 0x0B000041,
        /// <summary>
        /// Функция не определена
        /// </summary>
        UndefinedFunction = 0x0B000051,
        /// <summary>
        /// Не правильный аргумент функции
        /// </summary>
        IncorectFunctionArgument = 0x0B000061,
        /// <summary>
        /// Слишком мало параметров
        /// </summary>
        TooFewArguments = 0x0B000071,
        /// <summary>
        /// Слишком много параметров
        /// </summary>
        TooManyArguments = 0x0B000081,
        /// <summary>
        /// Параметр не определен за требуемое время
        /// </summary>
        ParameterNotAssignedFor = 0x0B000091,
        /// <summary>
        /// Рекурсивное вхождение в параметр
        /// </summary>
        RecursiveParameter = 0x0B0000A1,
        /// <summary>
        /// Использование переменной с неопределенным значением
        /// </summary>
        UseOfUnAssignmentVariable = 0x0B0000A8,

        /// <summary>
        /// Бесконецный цикл
        /// </summary>
        CycleCounterOverflow = 0x0B0000B1,
        /// <summary>
        /// оператор break без цикла
        /// </summary>
        BreakWithoutCycle = 0x0B0000C1,
        /// <summary>
        /// оператор continue без цикла
        /// </summary>
        ContinueWithoutCycle = 0x0B0000D1,

        /// <summary>
        /// Не известная ошибка
        /// </summary>
        UndefinedError = 0x0C0001F2,

        /// <summary>
        /// Не закрытые ковычки
        /// </summary>
        Unquoted = 0x0A000001,
        /// <summary>
        /// Не закрытые ковычки $...$
        /// </summary>
        UnquotedParameter = 0x0A000011,
        /// <summary>
        /// Не известный символ
        /// </summary>
        UndefinedToken = 0x0A000021,
        /// <summary>
        /// Синтаксическая ошибка
        /// </summary>
        SyntaxError = 0x0A000031,

        /// <summary>
        /// Получено не допустимое число
        /// </summary>
        NotAvailableNumber = 0x070000A1
    }

    [Serializable]
    public class Location
    {
        public int sLin { get; set; }
        public int sCol { get; set; }
        public int eLin { get; set; }
        public int eCol { get; set; }

        public Location(int sLin, int sCol, int eLin, int eCol)
        {
            this.sLin = sLin;
            this.sCol = sCol;
            this.eLin = eLin;
            this.eCol = eCol;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CalcPosition
    {
        /// <summary>
        /// 
        /// </summary>
        public enum IntimeIdentification
        {
            /// <summary>
            /// 
            /// </summary>
            Runtime,

            /// <summary>
            /// 
            /// </summary>
            Compiletime
        };

        /// <summary>
        /// 
        /// </summary>
        public enum NodePart
        {
            /// <summary>
            /// 
            /// </summary>
            Formula,

            /// <summary>
            /// 
            /// </summary>
            Expression,

            /// <summary>
            /// 
            /// </summary>
            DefinitionDomain,

            /// <summary>
            /// 
            /// </summary>
            ArgumentExpression
        };

        /// <summary>
        /// 
        /// </summary>
        public int NodeID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IntimeIdentification Intime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public NodePart CurrentPart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// Для аргументов, имя текущего аргумента
        /// </summary>
        public String AdditionNote { get; set; }

        public void Merge(CalcPosition calcPosition)
        {
            NodeID = calcPosition.NodeID;
            Intime = calcPosition.Intime;
            CurrentPart = calcPosition.CurrentPart;
            if (Location == null)
                Location = calcPosition.Location;
            AdditionNote = calcPosition.AdditionNote;
        }
    }

    /// <summary>
    /// Сообщение во время компиляции/интерпритации
    /// </summary>
    [Serializable]
    [System.Runtime.Serialization.DataContract]
    public class CalcMessage : Message
    {
        [System.Runtime.Serialization.DataMember]
        public CalcPosition Position { get; set; }

        ///// <summary>
        ///// Положение в коде, вызвавшее сообщение
        ///// </summary>
        //public Location Location { get; set; }

        /// <summary>
        /// ИД узла с которым связанна ошибка
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public CalcPosition SubjectPosition { get; set; }

        [System.Runtime.Serialization.DataMember]
        public String ObjectExpression { get; set; }

        /// <summary>
        /// Время с которым связанна ошибка
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public DateTime ObjectTime { get; set; }

        public CalcMessage(MessageCategory categ, CalcPosition calcPosition, CalcPosition subjectPosition, String messFormat, params Object[] pars)
            : base(categ, messFormat, pars)
        {
            this.Position = calcPosition;
            this.SubjectPosition = subjectPosition;

            if (this.Position == null)
                this.Position = new CalcPosition();
            if (this.SubjectPosition == null)
                this.SubjectPosition = new CalcPosition();
            //Location = locat;
            //NodeID = nodeId;
            //ExpressionCode = expressionCode;
            //ObjectID = objectId;
        }

        //public CalcMessage(MessageCategory categ, Location locat, String messFormat, params Object[] pars)
        //    : this(categ,  locat, messFormat, pars) { }

        public CalcMessage(MessageCategory categ, Location location, String messFormat, params Object[] pars)
            : this(categ, new CalcPosition() { Location = location }, null, messFormat, pars) { }

        public CalcMessage(MessageCategory categ, CalcPosition calcPosition, String messFormat, params Object[] pars)
            : this(categ, calcPosition, null, messFormat, pars) { }

        public CalcMessage(MessageCategory categ, String messFormat, params Object[] pars)
            : this(categ, null, null, messFormat, pars) { }

        public CalcMessage(CalcMessage mess)
            : this(mess.Category, String.Empty, new Object[0])
        { Text = mess.Text; }

        public override string ToString()
        {
            StringBuilder mess = new StringBuilder();
            mess.AppendFormat("{0} - ", Category);
            //if (NodeID >= 0)
            //    mess.AppendFormat("{0}{1}", NodeID, Location != null ? ":" : " ");
            if (Position != null && Position.Location != null)
                mess.AppendFormat("({0}, {1}) ", Position.Location.sLin, Position.Location.sCol);
            mess.Append(Text);
            return mess.ToString();
        }
    }
}
