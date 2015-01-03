using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using COTES.ISTOK.ASC.TypeConverters;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    [TypeConverter(typeof(NormFuncNodeTypeConverter))]
    public class NormFuncNode : UnitNode
    {
        const String codeAttributeName = "formula_cod";
        const String unitAttributeName = "resultUnit";

#if DEBUG
        [DisplayName("Buffer"),
        CategoryOrder(CategoryGroup.Debug)]
        public byte[] Buffer
        {
            get
            {
                return GetBinaries(normfuncPropertyName);
            }
        }
#endif

        [Description("Уникальный символьный код, используемый в формулах.")]
        [DisplayName("Код функции")]
        [CategoryOrder(CategoryGroup.Calc)]
        [Browsable(true)]
        public override string Code
        {
            get { return GetAttribute(codeAttributeName); }
            set { SetAttribute(codeAttributeName, value); }
        }

        [Description("Единица измерения возвращаемого значения.")]
        [DisplayName("Единица измерения")]
        [CategoryOrder(CategoryGroup.General)]
        [Browsable(true)]
        public string ResultUnit
        {
            get { return GetAttribute(unitAttributeName); }
            set { SetAttribute(unitAttributeName, value); }
        }

        const String normfuncPropertyName = "normfunc";

        public MultiDimensionalTable GetMDTable(RevisionInfo revision)
        {
            MultiDimensionalTable mdt;
            byte[] normFuncBuffer;
            normFuncBuffer = GetBinaries(normfuncPropertyName, revision);
            mdt = new MultiDimensionalTable();
            if (normFuncBuffer != null)
                mdt.FromBytes(normFuncBuffer);

            return mdt;
        }

        public void SetMDTable(RevisionInfo revision, MultiDimensionalTable mdt)
        {
            byte[] normFuncBuffer = null;

            if (mdt == null)
                normFuncBuffer = new byte[0];
            else
                normFuncBuffer = mdt.ToBytes();
            SetBinaries(normfuncPropertyName, revision, normFuncBuffer);
        }

        public NormFuncNode() : base() { }
        public NormFuncNode(DataRow row)
            : base(row)
        {
            if (Typ != (int)UnitTypeId.NormFunc) throw new Exception("Неверный тип узла");
        }
    }
}
