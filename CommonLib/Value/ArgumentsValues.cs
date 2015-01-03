using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK
{
    /// <summary>
    /// Значения аргументов
    /// </summary>
    [Serializable]
    public class ArgumentsValues : IEnumerable<String>
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly ArgumentsValues Empty = new ArgumentsValues();

        /// <summary>
        /// Признак плохого значения.
        /// </summary>
        /// <remarks>
        /// Используется в расчёте как признак отсутствия аргументов 
        /// там где они должны быть (из-за неудачного расчёта оптимизации) 
        /// и при сохранении подобных значений в БД
        /// </remarks>
        public static readonly ArgumentsValues BadArguments = new ArgumentsValues();

        /// <summary>
        /// Хранилище значений аргументов
        /// </summary>
        Dictionary<String, double> arguments;

        public ArgumentsValues()
        {
            arguments = new Dictionary<String, double>();
        }

        public ArgumentsValues(ArgumentsValues args)
            : this()
        {
            if (args != null)
                foreach (var name in args)
                {
                    this[name] = args[name];
                }
        }

        public ArgumentsValues(Object obj)
            : this()
        {
            if (obj != null)
            {
                Type t = obj.GetType();
                var props = t.GetProperties();

                foreach (var item in props)
                {
                    if (item.CanRead)
                    {
                        Object objectValue = item.GetValue(obj, null);
                        //double symbolValue = double.CreateValue(objectValue);
                        double value = (double)Convert.ChangeType(objectValue, typeof(double));

                        arguments[item.Name] = value;// symbolValue;
                    }
                }
            }
        }

        /// <summary>
        /// Количество аргументов
        /// </summary>
        public int Count
        {
            get
            {
                return arguments.Count;
            }
        }

        /// <summary>
        /// Получить или установить значение аргумента по имени
        /// </summary>
        /// <param name="name">Имя аргумента</param>
        /// <returns></returns>
        public double this[String name]
        {
            get { return arguments[name]; }
            set { arguments[name] = value; }
        }

        public override bool Equals(object obj)
        {
            ArgumentsValues args = obj as ArgumentsValues;

            if (args != null)
            {
                if (arguments.Count != args.arguments.Count)
                    return false;

                foreach (var name in arguments.Keys)
                {
                    if (!args.arguments.ContainsKey(name)
                        || !Object.Equals(arguments[name], args.arguments[name]))
                        return false;
                }
                return true;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = arguments.Count;

            foreach (var name in arguments.Keys)
            {
                hash += name.GetHashCode();
                hash += arguments[name].GetHashCode();
            }

            return hash;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            //builder.Append("(");
            int builderLength = builder.Length;

            foreach (var name in this)
            {
                if (builder.Length > builderLength)
                    builder.Append(", ");
                builder.AppendFormat("{0}={1}", name, this[name]);
            }

            //builder.Append(")");

            return builder.ToString();
        }

        #region IEnumerable<String> Members

        public IEnumerator<String> GetEnumerator()
        {
            foreach (var item in arguments.Keys)
            {
                yield return item;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Проверяет соответствуют ли данные аргументы требуемым.
        /// </summary>
        /// <seealso cref="m:Include"/>
        /// <param name="args">Имена требуемых аргументов</param>
        /// <returns>
        /// Если присутсвуют все требуемые аргументы и нету лишних, возвращает true.
        /// <br />
        /// В противном случае возвращает false.
        /// </returns>
        public bool CorrespondTo(string[] args)
        {
            bool ret;

            ret = (args == null && Count == 0) || (args != null && args.Length == Count);

            for (int i = 0;ret&& i < args.Length; i++)
            {
                ret = arguments.ContainsKey(args[i]);
            }
            return ret;
        }

        /// <summary>
        /// Удалить из аргументов определенные параметры.
        /// </summary>
        /// <param name="nodeArgs">Удаляемые имена атрибутов</param>
        /// <returns></returns>
        public ArgumentsValues Exclude(IEnumerable<String> nodeArgs)
        {
            ArgumentsValues values = new ArgumentsValues(this);

            foreach (var item in nodeArgs)
            {
                if (values.arguments.ContainsKey(item))
                    values.arguments.Remove(item);
            }

            return values;
        }

        /// <summary>
        /// Проверяет включают ли аргументы в себя требуемые параметры.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>
        /// Если присутствуют все требуемые аргументы с требуемыми значениями, возвращает true.
        /// <br />
        /// В противном случае - false.
        /// </returns>
        public bool Include(ArgumentsValues args)
        {
            double values;

            foreach (var name in args)
            {
                if (!arguments.TryGetValue(name, out values)
                    || !doubleEquals(values, args[name]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Проверяет включают ли аргументы в себя требуемые параметры.
        /// </summary>
        /// <seealso cref="m:CorrespondTo"/>
        /// <param name="args">Имена требуемых параметров</param>
        /// <returns>
        /// Если присутствуют все требуемые аргументы, возвращает true.
        /// <br />
        /// В противном случае - false.
        /// </returns>
        public bool Include(string[] args)
        {
            foreach (var name in args)
            {
                if (!arguments.ContainsKey(name))
                {
                    return false;
                }
            }
            return true;
        }

        private bool doubleEquals(double a, double b)
        {
            return (double.IsNaN(a) && double.IsNaN(b))
                || double.Equals(a, b);
        }
    }
}
