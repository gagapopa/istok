using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace COTES.ISTOK.ASC
{
    [Serializable]
    [DataContract]
    public class UTypeNode : ServerNode
    {
        //protected int tree_visible;
        //public int Tree_visible { get { return tree_visible; } set { if (tree_visible != value) { tree_visible = value;/* modified = true;*/ } } }
        [DataMember]
        protected byte[] icon = null;
        [DataMember]
        public byte[] Icon { get { return icon; } set { icon = value; } }
        [DataMember]
        protected string props;
        [DataMember]
        public string Props { get { return props; } set { if (props != value) { props = value;/* modified = true;*/ } } }

        [DataMember]
        public bool ChildFilterAll { get; set; }
        [DataMember]
        public List<int> ChildFilter { get; set; }

        [DataMember]
        public Guid ExtensionGUID { get; set; }

        [DataMember]
        public String Filter
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                if (ChildFilterAll)
                    builder.Append("all");
                else
                {
                    foreach (int typeId in ChildFilter)
                    {
                        if (builder.Length > 0) builder.Append(';');
                        builder.Append(typeId);
                    }
                }
                return builder.ToString();
            }
            set
            {
                ChildFilterAll = !String.IsNullOrEmpty(value) && value.ToLower().Equals("all");

                ChildFilter = new List<int>();
                if (!ChildFilterAll)
                {
                    if (!String.IsNullOrEmpty(value))
                    {
                        String[] childFilterTypes = value.Split(new char[] { ';' });
                        foreach (String childType in childFilterTypes)
                        {
                            if (!String.IsNullOrEmpty(childType.Trim()))
                            {
                                ChildFilter.Add(int.Parse(childType.Trim()));
                            }
                        }
                    }
                }
            }
        }

        public UTypeNode()
            : base()
        {
            props = "";
            ChildFilter = new List<int>();
        }

        public override object Clone()
        {
            UTypeNode res = new UTypeNode();
            res.idnum = Idnum;
            res.Text = Text;
            //res.Editor = Editor;
            res.ExtensionGUID = ExtensionGUID;
            //res.tree_visible = Tree_visible;
            res.props = Props.ToString();
            res.Filter = Filter;
            if (Icon != null)
            {
                res.icon = new byte[Icon.Length];
                Icon.CopyTo(res.icon, 0);
            }
            return res;
        }

        public ItemProperty[] GetProperties()
        {
            var lstRes = new List<ItemProperty>();
            ItemProperty ptr;

            foreach (var item in Props.Split(';'))
            {
                if (item != null && !string.IsNullOrEmpty(item.Trim()))
                {
                    ptr = new ItemProperty()
                    {
                        Name = item,
                        DisplayName = item,
                        Description = "",
                        Category = "",
                    };
                    lstRes.Add(ptr);
                }
            }
            return lstRes.ToArray();
        }

        //#region Description for logging
        //public override string DescriptionLog
        //{
        //    get { return "Тип"; }
        //}
        //#endregion
    }
}
