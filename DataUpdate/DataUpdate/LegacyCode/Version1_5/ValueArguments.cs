using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOKDataUpdate.Version1_5
{
    /// <summary>
    /// Значения аргументов условных параметров
    /// </summary>
    [Serializable]
    public class ValueArguments : IComparable<ValueArguments>, ICloneable
    {
        private List<LevelArguments> Values;

        /// <summary>
        /// Значения аргументов условных параметров для одного уровня вложенной оптимизации
        /// </summary>
        [Serializable]
        public class LevelArguments
        {
            /// <summary>
            /// ИД узла оптимизации данного уровня вложенной оптимизации
            /// </summary>
            public int OptimizationNodeID { get; set; }

            /// <summary>
            /// Значения оптимизации имя аргумента - значение
            /// </summary>
            private KeyValuePair<String, Object>[] arguments;

            public LevelArguments(int id, KeyValuePair<String, Object>[] args)
            {
                this.OptimizationNodeID = id;
                this.arguments = new KeyValuePair<String, Object>[args.Length];
                args.CopyTo(this.arguments, 0);
            }

            public LevelArguments(LevelArguments level)
                : this(level.OptimizationNodeID, level.arguments)
            { }

            /// <summary>
            /// Получить значение конкретного аргумента
            /// </summary>
            /// <param name="index">Порядковый номер аргумента на текущем уровне</param>
            /// <returns>Пара имя - значение аргумента</returns>
            public KeyValuePair<String, Object> this[int index]
            {
                get
                {
                    //if (index < 0 || index > arguments.Length)
                    //    return null;
                    return arguments[index];
                }
                internal set {
                    arguments[index] = value;
                }
            }

            /// <summary>
            /// Количество аргументов на текущем уровне
            /// </summary>
            public int Length { get { return arguments.Length; } }

            public override int GetHashCode()
            {
                int ret = 0;
                ret += OptimizationNodeID.GetHashCode();
                for (int i = 0; i < arguments.Length; i++)
                {
                    ret += arguments[i].Key.GetHashCode();
                    ret += arguments[i].Value.GetHashCode();
                }
                return ret;
            }

            public override bool Equals(object obj)
            {
                bool ret;
                LevelArguments b = obj as LevelArguments;

                if (b == null)
                    return false;

                ret = b.OptimizationNodeID.Equals(OptimizationNodeID);
                for (int i = 0; ret && i < b.arguments.Length; i++)
                    ret = String.Equals(b.arguments[i].Key, arguments[i].Key)
                        && Object.Equals(b.arguments[i].Value, arguments[i].Value);

                return ret;
            }
        }

        public ValueArguments()
        {
            //Values = new List<KeyValuePair<int, Object[]>>();
            Values = new List<LevelArguments>();
        }

        public ValueArguments(ValueArguments args)
            : this()
        {
            if (args != null)
                foreach (LevelArguments level in args.Values)
                    Values.Add(new LevelArguments(level));
        }

        /// <summary>
        /// Получить значения аргументов базовой оптимизации для вложенной оптимизации
        /// </summary>
        /// <returns></returns>
        public ValueArguments GetBaseArgument()
        {
            if (Values.Count < 2)
                return null;

            ValueArguments baseArgument = new ValueArguments();

            for (int i = 0; i < Values.Count - 1; i++)
            {
                baseArgument.Values.Add(Values[i]);
            }
            return baseArgument;
        }

        /// <summary>
        /// Получить аргумент из нижнего уровня вложенной оптимизации
        /// </summary>
        /// <param name="index">Номер запрашиваемого аргумента на уровне</param>
        /// <returns></returns>
        public KeyValuePair<String, Object> GetLastArgument(int index)
        {
            if (Values.Count < 1)
                return new KeyValuePair<string, object>();
            return Values[Values.Count - 1][index];
        }

        /// <summary>
        /// Добавить нижний уровень аргументов вложенной оптимизации
        /// </summary>
        /// <param name="nodeId">ИД Узла оптимизации</param>
        /// <param name="argsValues">Аргументы уровня</param>
        public void Add(int nodeId, KeyValuePair<String, Object>[] argsValues)
        {
            Values.Add(new LevelArguments(nodeId, argsValues));
        }

        /// <summary>
        /// Добавить верхний уровень аргументов вложенной оптимизации
        /// </summary>
        /// <param name="nodeId">ИД Узла оптимизации</param>
        /// <param name="argsValues">Аргументы уровня</param>
        public void AddTop(int nodeId, KeyValuePair<String, Object>[] argsValues)
        {
            Values.Insert(0, new LevelArguments(nodeId, argsValues));
        }

        /// <summary>
        /// Проверить включают ли текущие аргументы данный
        /// </summary>
        /// <param name="arguments">Проверяемые значения аргументов</param>
        /// <returns></returns>
        public bool Include(ValueArguments valueArgument)
        {
            bool ret;
            if (valueArgument == null) return true;
            ret = valueArgument.Values.Count <= Values.Count;
            for (int i = 0; ret && i < valueArgument.Values.Count; i++)
                ret = Values[i].Equals(valueArgument.Values[i]);

            return ret;
        }

        public bool Include(int optimizationNodeID)
        {
            for (int i = 0; i < Values.Count; i++)
                if (Values[i].OptimizationNodeID == optimizationNodeID)
                    return true;

            return false;
        }

        /// <summary>
        /// Проверить включают ли один аргумент другой
        /// </summary>
        /// <param name="args">Аргумент, включающий в себя другой</param>
        /// <param name="valueArguments">Базовый аргумент, включенный в первый</param>
        /// <returns></returns>
        public static bool Include(ValueArguments args, ValueArguments valueArguments)
        {
            if ((args == null || args.Length == 0) && (valueArguments == null || valueArguments.Length == 0))
                return true;

            return args != null && args.Include(valueArguments);
        }

        /// <summary>
        /// Количество уровней вложенной оптимизации в данном значении аргументов
        /// </summary>
        public int Length { get { return Values.Count; } }

        /// <summary>
        /// Получить аргументы конкретнного уровеня вложенной оптмизации
        /// </summary>
        /// <param name="index">Порядковый номер уровня, начиная с верхнего</param>
        /// <returns></returns>
        public LevelArguments this[int index] { get { return Values[index]; } }

        public Object this[String name]
        {
            get {
                for (int i = 0; i < Length; i++)
                    for (int j = 0; j < this[i].Length; j++)
                        if (String.Equals( this[i][j].Key, name))
                            return this[i][j].Value;

                throw new ArgumentException();
            }
            set
            {
                for (int i = 0; i < Length; i++)
                    for (int j = 0; j < this[i].Length; j++)
                        if (String.Equals(this[i][j].Key, name))
                        {
                            this[i][j] = new KeyValuePair<String, Object>(name, value);
                            return;
                        }

                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Значения аргументов нижнего уровня вложенной оптимизации
        /// </summary>
        public LevelArguments LastLevel
        {
            get
            {
                if (Values == null || Values.Count < 1)
                    return null;
                return Values[Values.Count - 1];
            }
        }

        public override int GetHashCode()
        {
            int ret = 0;

            if (Values != null)
                for (int i = 0; i < Values.Count; i++)
                    ret += Values[i].GetHashCode();

            return ret;
        }

        public override bool Equals(object obj)
        {
            ValueArguments key;

            //if (obj == null)
            //    return Length == 0;

            if ((key = obj as ValueArguments) != null
                && key.Values.Count == Values.Count)
            {
                bool ret = true;
                for (int i = 0; ret && i < Values.Count; i++)
                    ret = Values[i].Equals(key.Values[i]);

                return ret;
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (Values != null)
            {
                int length;

                length = builder.Length;
                for (int i = 0; i < Values.Count; i++)
                {
                    for (int j = 0; j < Values[i].Length; j++)
                    {
                        if (builder.Length > length) builder.Append(", ");
                        if (Values[i][j].Value is double)
                        {
                            String format;
                            double val = (double)Values[i][j].Value;
                            if (Math.Abs(val - (int)val) < 1e-4)
                                format = "{0}";
                            else
                                format = "{0:0.000}";
                            builder.AppendFormat(format, val);
                        }
                        else builder.Append(Values[i][j].Value);
                    }
                }
            }
            return builder.ToString();
        }

        #region IComparable<ValueArguments> Members

        public int CompareTo(ValueArguments other)
        {
            ValueArguments val = other as ValueArguments;
            if (val != null)
            {
                if (Length > val.Length) return 1;
                if (Length < val.Length) return -1;

                for (int i = 0; i < Length; i++)
                {
                    if (this[i].Length > val[i].Length) return 1;
                    if (this[i].Length < val[i].Length) return -1;

                    for (int j = 0; j < this[i].Length; j++)
                    {
                        double t, o;
                        t = Convert.ToDouble(this[i][j].Value);
                        o = Convert.ToDouble(val[i][j].Value);
                        if (t > o) return 1;
                        if (t < o) return -1;
                    }
                }
                return 0;
            }
            throw new ArgumentException();
        }

        #endregion

        public ValueArguments Degrade(int optimizationNodeID)
        {
            for (int i = 0; i < Values.Count; i++)
            {
                if (Values[i].OptimizationNodeID == optimizationNodeID)
                {
                    ValueArguments args = new ValueArguments();
                    for (int j = 0; j <= i; j++)
                        args.Values.Add(this[j]);

                    return args;
                }
            }
            return null;
        }

        #region ICloneable Members

        public object Clone()
        {
            return new ValueArguments(this);
        }

        #endregion
    }
}
