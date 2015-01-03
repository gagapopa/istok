using System;
using System.ComponentModel;
using System.Data;
using COTES.ISTOK.ASC.TypeConverters;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    public class CalcParameterNode : ParameterNode
    {
        const String formulaAttributeName = "formula_text";
        const String neededAttributeName = "needed";
        const String roundrobinAttributeName = "roundrobin_calc_enabled";

        [Description("Текст формулы")]
        [DisplayName("Формула")]
        [CategoryOrder(CategoryGroup.Calc)]
        [RevisionUnitNode(formulaAttributeName)]
        public string Formula
        {
            get { return GetAttribute(formulaAttributeName); }
            set { SetAttribute(formulaAttributeName, value); }
        }

        [Description("Коды параметров, разделенные точкой запятой, которые требуется расчитать до перед расчётом данного параметра.")]
        [DisplayName("Зависит от")]
        [CategoryOrder(CategoryGroup.Calc)]
        [RevisionUnitNode(neededAttributeName)]
        public string NeededParametersCodes
        {
            get { return GetAttribute(neededAttributeName); }
            set { SetAttribute(neededAttributeName, value); }
        }

        public RevisedStorage<string> GetFormulaStorage()
        {
            RevisedStorage<string> storage;
            if (!Attributes.TryGetValue(formulaAttributeName, out storage))
                Attributes[formulaAttributeName] = storage = new RevisedStorage<string>();

            return storage;
        }

        public void SetFormulaStorage(RevisedStorage<string> formulaStorage)
        {
            Attributes[formulaAttributeName] = formulaStorage;
        }

        [DisplayName("Участвует в циклическом расчете")]
        [Description("Указывает на то будет ли этот параметр расчитам при циклическом расчете.")]
        [CategoryOrder(CategoryGroup.Calc)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public bool RoundRobinCalc
        {
            get
            {
                bool res = false;

                if (bool.TryParse(GetAttribute(roundrobinAttributeName), out res))
                    return res;

                return false;
            }
            set
            {
                SetAttribute(roundrobinAttributeName, value.ToString());
            }
        }

        public CalcParameterNode()
            : base() { }
        public CalcParameterNode(int id)
            : base(id) { }
        public CalcParameterNode(DataRow row)
            : base(row)
        { }
    }
}
