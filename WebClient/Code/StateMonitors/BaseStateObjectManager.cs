using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace WebClient
{
    public abstract class BaseStateObjectManager<T, S> : IEnumerable<T> 
        where T : BaseStateMonitor<S>
        where S : ParameterDescriptor
    {
        private Dictionary<int, T> state_objects =
            new Dictionary<int, T>();

        public BaseStateObjectManager()
        {   }

        public void Add(T deskriptor)
        {
            state_objects[deskriptor.ID] = deskriptor;
        }

        public bool Exist(int id)
        {
            return state_objects.ContainsKey(id);
        }

        public void Remove(int id)
        {
            if (Exist(id))
                state_objects.Remove(id);
        }

        public void Clear()
        {
            state_objects.Clear();
        }

        public T this[int id]
        {
            get
            {
                if (Exist(id))
                    return state_objects[id];
                else return null;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return state_objects.Values.GetEnumerator();
        }
    }
}
