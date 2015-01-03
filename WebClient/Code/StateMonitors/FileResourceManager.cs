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
using System.Threading;
using System.Collections;

namespace WebClient
{
    public class FileResourceManager : IEnumerable<FileResource>
    {
        private object lock_item = new object();
        private Dictionary<Guid, FileResource> resources_by_id = 
            new Dictionary<Guid, FileResource>();
        private Dictionary<Guid, List<FileResource>> resources_by_user =
            new Dictionary<Guid, List<FileResource>>();

        public FileResourceManager()
        { }

        public void AddNewResource(Guid user, FileResource resource)
        {
            if (!resources_by_id.ContainsKey(resource.ID))
                resources_by_id[resource.ID] = resource;

            if (!resources_by_user.ContainsKey(user))
                resources_by_user[user] = new List<FileResource>();

            resources_by_user[user].Add(resources_by_id[resource.ID]);
            ++resources_by_id[resource.ID].ClientCount;
        }

        public bool ExistResource(Guid resource_id)
        {
            return resources_by_user.ContainsKey(resource_id);
        }

        public FileResource this[Guid resource_id]
        {
            get
            {
                return resources_by_id.ContainsKey(resource_id) ?
                    resources_by_id[resource_id] : null;
            }
        }

        public IEnumerable<FileResource> ClearForUser(Guid user)
        {
            List<FileResource> result = 
                new List<FileResource>();

            if (!resources_by_user.ContainsKey(user))
                return result;

            foreach (var it in resources_by_user[user])
                if (--it.ClientCount == 0)
                {
                    resources_by_id.Remove(it.ID);
                    result.Add(it);
                }

            resources_by_user.Remove(user);

            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<FileResource> GetEnumerator()
        {
            return resources_by_id.Values.GetEnumerator();
        }
    }
}
