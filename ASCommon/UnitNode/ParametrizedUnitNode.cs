using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Runtime.Serialization;
using COTES.ISTOK.ASC.TypeConverters;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    [KnownType(typeof(GraphNode))]
    [KnownType(typeof(HistogramNode))]
    [DataContract]
    public abstract class ParametrizedUnitNode : UnitNode
    {
        [DataMember]
        protected List<ChildParamNode> parameters = new List<ChildParamNode>();

        public ParametrizedUnitNode() : base() { }
        public ParametrizedUnitNode(DataRow row) : base(row) { }

        public abstract ChildParamNode CreateNewChildParamNode();
        
        public override ChildParamNode AddChildParam(DataRow row)
        {
            ChildParamNode child = CreateNewChildParamNode();
            child.LoadData(row);
            child.SortIndex = GetNewSortIndex();
            parameters.Add(child);
            SortParameters();
            return child;
        }
        public override ChildParamNode AddChildParam(ParameterNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            ChildParamNode child = CreateNewChildParamNode();
            child.SortIndex = GetNewSortIndex();
            child.SetParameter(node);
            parameters.Add(child);
            SortParameters();
            return child;
        }
        public virtual ChildParamNode AddChildParam(ChildParamNode child)
        {
            if (child == null) throw new ArgumentNullException("child");
            child.SortIndex = GetNewSortIndex();
            parameters.Add(child);
            SortParameters();
            return child;
        }

        private void SortParameters()
        {
            var lst = from elem in parameters
                      orderby elem.SortIndex
                      select elem;
            Parameters = lst.ToArray();
        }

        protected int GetNewSortIndex()
        {
            if (parameters.Count == 0) return 0;
            int max = int.MinValue;
            foreach (var item in parameters)
                if (item.SortIndex > max) max = item.SortIndex;
            return max + 1;
        }
        
#if DEBUG
        [Browsable(true),
        CategoryOrder(CategoryGroup.Debug)]
#else
        [Browsable(false)]
#endif
        public override ChildParamNode[] Parameters
        {
            get
            {
                return parameters.ToArray();
            }
            set
            {
                if (value == null) parameters = new List<ChildParamNode>();
                else
                {
                    if (parameters == null)
                        parameters = new List<ChildParamNode>();
                    else
                        parameters.Clear();
                    parameters.AddRange(value);
                }
            }
        }

        public virtual void RemoveChildParam(ChildParamNode param)
        {
            foreach (var item in parameters)
            {
                if (item.Idnum == param.Idnum)
                {
                    parameters.Remove(item);
                    break;
                }
            }
        }

        public override object Clone()
        {
            ParametrizedUnitNode res = base.Clone() as ParametrizedUnitNode;
            List<ChildParamNode> lstParams = new List<ChildParamNode>();
            foreach (var item in parameters)
                lstParams.Add(item.Clone());
            res.Parameters = lstParams.ToArray();
            return res;
        }

        public override void AcceptChanges()
        {
            base.AcceptChanges();
            SortParameters();
        }
    }
}
