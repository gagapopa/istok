using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections;

namespace WebClient
{
    public class MnemoschemesManager : 
        BaseStateObjectManager<MnemoschemesDescriptor, MnemoschemeParameterDescriptor>//IEnumerable<MnemoschemesDeskriptor>
    {
        /*private Dictionary<int, MnemoschemesDeskriptor> mnemoschemes =
            new Dictionary<int, MnemoschemesDeskriptor>();

        public MnemoschemesManager()
        {   }

        public void Add(MnemoschemesDeskriptor deskriptor)
        {
            mnemoschemes[deskriptor.ID] = deskriptor;
        }

        public bool Exist(int mnemoscheme_id)
        {
            return mnemoschemes.ContainsKey(mnemoscheme_id);
        }

        public void Remove(int mnemoscheme_id)
        {
            if (Exist(mnemoscheme_id))
                mnemoschemes.Remove(mnemoscheme_id);
        }

        public void Clear()
        {
            mnemoschemes.Clear();
        }

        public MnemoschemesDeskriptor this[int mnemoscheme_id]
        {
            get
            {
                if (Exist(mnemoscheme_id))
                    return mnemoschemes[mnemoscheme_id];
                else return null;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<MnemoschemesDeskriptor> GetEnumerator()
        {
            return mnemoschemes.Values.GetEnumerator();
        }*/
    }
}
