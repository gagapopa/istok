using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    public class ParameterFilter
    {
        private Dictionary<string, string> dicProperties;
        private bool usePropertiesAND;
        private bool useMustExist;
        private string f_name;
        private string f_code;

        private bool useReg = false;
        private bool useWhole = false;

        public ParameterFilter()
        {
            dicProperties = new Dictionary<string, string>();
            usePropertiesAND = false;
            useMustExist = false;
            f_name = f_code = "";
        }
        public ParameterFilter(string name, string code)
            : this()
        {
            f_name = name;
            f_code = code;
        }

        /// <summary>
        /// Имя параметра
        /// </summary>
        public string Name
        {
            get { return f_name; }
            set { f_name = value; }
        }
        /// <summary>
        /// Код параметра
        /// </summary>
        public string Code
        {
            get { return f_code; }
            set { f_code = value; }
        }
        /// <summary>
        /// Искать полное совпадение имени
        /// </summary>
        public bool WholeWord
        {
            get { return useWhole; }
            set { useWhole = value; }
        }
        /// <summary>
        /// Учитывать регистр
        /// </summary>
        public bool UseReg
        {
            get { return useReg; }
            set { useReg = value; }
        }
        /// <summary>
        /// Обязательное существование свойств
        /// </summary>
        public bool UseMustExist
        {
            get { return useMustExist; }
            set { useMustExist = value; }
        }

        /// <summary>
        /// Добавление свойства для сравнения
        /// </summary>
        /// <param name="name">Имя свойства</param>
        /// <param name="value">Значение свойства</param>
        public void AddProperty(string name, string value)
        {
            dicProperties.Add(name, value);
        }
        /// <summary>
        /// Удаление всех свойств
        /// </summary>
        public void ClearProperties()
        {
            dicProperties.Clear();
        }

        /// <summary>
        /// Проверить, подходит ли параметр по фильтру
        /// </summary>
        /// <param name="node">Проверяемый параметр</param>
        /// <returns>true, если подходит, иначе - false</returns>
        public bool Check(UnitNode node, RevisionInfo revision)
        {
            RevisionInfo rev;
            bool rlo;
            bool? res = null;
            bool result = false;

            if (node == null) return false;

            if (!string.IsNullOrEmpty(f_name))
            {
                rlo = CheckStr(node.Text, f_name);
                res = res == null ? rlo : res & rlo;
            }
            if (!string.IsNullOrEmpty(f_code))
            {
                rlo = CheckStr(node.Code, f_code);
                res = res == null ? rlo : res & rlo;
            }

            if (res != null)
                result = (bool)res;
            else
            {
                if (dicProperties.Keys.Count == 0)
                    result = false;
                else
                    result = true;
            }

            if (!result) return result;

            if (dicProperties.Keys.Count > 0)
                result = false;
            if (revision == null)
                rev = RevisionInfo.Head;
            else
                rev = revision;
            foreach (string key in dicProperties.Keys)
            {
                if (node.Attributes.ContainsKey(key))
                {
                    rlo = (node.Attributes[key].Get(rev) == dicProperties[key]);
                    if (usePropertiesAND)
                        result = result & rlo;
                    else
                        result = result | rlo;
                }
                else
                {
                    if (useMustExist)
                        return false;
                    else
                        result = true;
                }

                if (!result && usePropertiesAND) break;
            }

            return result;
        }

        private bool CheckStr(string str_base, string str_add)
        {
            string str1, str2;
            bool rlo = false;

            if (!useReg)
            {
                str1 = str_base.ToLower();
                str2 = str_add.ToLower();
            }
            else
            {
                str1 = str_base;
                str2 = str_add;
            }
            if (useWhole)
                rlo = str1 == str2;
            else
                rlo = str1.Contains(str2);

            return rlo;
        }
    }
}
