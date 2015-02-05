using System;
using System.ComponentModel;
using System.Data;
using COTES.ISTOK.ASC.TypeConverters;
using System.Drawing;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    public class ManualParameterNode : ParameterNode
    {
        public ManualParameterNode()
            : base() { }
        public ManualParameterNode(int id)
            : base(id) { }
        public ManualParameterNode(DataRow row)
            : base(row)
        { }
        const String manualConnectingParamNode = "connectNode_id";
        
        [DisplayName("ID Связанного параметра")]
        [Description("Параметр автоматического расчета, связанный с данным ручным параметром")]
        [CategoryOrder(CategoryGroup.Values)]
        public int ValueConnectingParamNode {       	
    		get
            {
                int s;
				return int.TryParse(GetAttribute(manualConnectingParamNode), out s) ? s : 0;

            }
            set
            {
                SetAttribute(manualConnectingParamNode, value.ToString());
            }
        }
    }
}
