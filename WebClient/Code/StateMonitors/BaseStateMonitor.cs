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
    public abstract class BaseStateMonitor<T> 
        where T : ParameterDescriptor
    {
        public int ID { get; protected set; }
        public string Name { get; protected set; }
        public int UpdateTransactionID { get; protected set; }

        protected Dictionary<int, List<T>> parametrs =
            new Dictionary<int, List<T>>();

        public BaseStateMonitor(int id, 
                                string name, 
                                int transaction_id)
        {
            ID = id;
            Name = name;
            UpdateTransactionID = transaction_id;
        }

        public void AddParameter(T parameter)
        {
            if (!parametrs.ContainsKey(parameter.ParametrID))
                parametrs.Add(parameter.ParametrID,
                    new List<T>());

            parametrs[parameter.ParametrID].Add(parameter);
        }

        public IEnumerable<T> this[int id]
        {
            get
            {
                return parametrs[id];
            }
        }
    }
}
