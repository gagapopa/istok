using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Collections;

namespace WebClient
{
    public class MonitorEnumerator<T> : IEnumerator<T>, IDisposable
    {
        private const int index_brfore_first = -1;

        private List<T> source = new List<T>();
        private int current_index = index_brfore_first;

        public MonitorEnumerator()
        { }

        public void AddContent(IEnumerable<T> content)
        {
            source.AddRange(content);
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public bool MoveNext()
        {
            return (++current_index) < source.Count;
        }

        public void Reset()
        {
            current_index = index_brfore_first;
        }

        public T Current
        {
            get
            {
                return current_index < source.Count ?
                           source[current_index] : default(T);
            }
        }

        void IDisposable.Dispose()
        {
            source.Clear();
        }
    }
}
