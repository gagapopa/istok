using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using COTES.ISTOK.ASC.TypeConverters;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    public class SchemaNode : ParametrizedUnitNode
    {
        const String pictureAttributeName = "picture";
        const String updateIntervalAttributeName = "UpdateInterval";
        const String relevanceAttributeName = "relevance";
        const String backColorAttributeName = "backcolor";

        protected byte[] imageBuffer = null;
        [Browsable(false)]
        public byte[] ImageBuffer
        {
            get
            {
                imageBuffer = GetBinaries(pictureAttributeName);
                return imageBuffer;
            }
            set
            {
                imageBuffer = value;
                SetBinaries(pictureAttributeName, imageBuffer);
            }
        }

        [CategoryOrder(CategoryGroup.Appearance), DisplayName("Обновление"), Description("Интервал обновления мнемосхемы (в секундах).")]
        public int UpdateInterval
        {
            get
            {
                int res = 1;

                if (int.TryParse(GetAttribute(updateIntervalAttributeName), out res))
                    return res;

                return 1;
            }
            set
            {
                SetAttribute(updateIntervalAttributeName, value.ToString());
            }
        }

        [CategoryOrder(CategoryGroup.Appearance), DisplayName("Актуальность"), Description("Время, спустя которое значение перестает считаться актуальным (в секундах).")]
        public int Relevance
        {
            get
            {
                int res;

                if (int.TryParse(GetAttribute(relevanceAttributeName), out res))
                    return res;

                return 1;
            }
            set
            {
                SetAttribute(relevanceAttributeName, value.ToString());
            }
        }

        [CategoryOrder(CategoryGroup.Appearance), DisplayName("Цвет фона"), Description("Цвет фона мнемосхемы.")]
        public Color BackColor
        {
            get
            {
                int res;
                if (int.TryParse(GetAttribute(backColorAttributeName), out res))
                    return Color.FromArgb(res);

                return Color.Empty;
            }
            set
            {
                SetAttribute(backColorAttributeName, value.ToArgb().ToString());
            }
        }

        public SchemaNode() : base() { }
        public SchemaNode(DataRow row)
            : base(row)
        {
            if (Typ != (int)UnitTypeId.Schema) throw new Exception("Неверный тип узла");
        }

        public override object Clone()
        {
            SchemaNode res = base.Clone() as SchemaNode;
            byte[] qq = ImageBuffer;
            if (qq != null)
            {
                res.imageBuffer = new byte[qq.Length];
                qq.CopyTo(res.imageBuffer, 0);
            }
            return res;
        }

        public override ChildParamNode CreateNewChildParamNode()
        {
            return new SchemaParamNode();
        }
    }
}
