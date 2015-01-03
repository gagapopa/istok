using COTES.ISTOK.ASC;
using COTES.ISTOK.ClientCore;
using COTES.ISTOK.ClientCore.UnitProviders;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Text;
using System.Web;

namespace COTES.ISTOK.WebClient.Models
{
    public class SessionKeeper
    {
        private int pageid = 1;

        //public UnitTypeId[] TypeFilter { get; set; }
        //public FilterParams FilterParams { get; set; }

        public RevisionInfo[] Revisions { get; private set; }
        //public RevisionInfo CurrentRevision { get; set; }

        //public UnitNode CurrentNode { get; set; }
        //public UnitProvider CurrentUnitProvider { get; set; }

        HttpSessionStateBase httpSession;

        public Session Session { get; private set; }

        //public StructureProvider StrucProvider { get { return GetStrucProvider(0); } }

        private Dictionary<int, StructureProvider> dicStrucProvider;

        public SessionKeeper(HttpSessionStateBase session)
        {
            this.httpSession = session;
            //TypeFilter = new UnitTypeId[] { };

            dicStrucProvider = new Dictionary<int, StructureProvider>();
            this.Session = new Session();
            //UpdateRevisions();
        }

        public StructureProvider GetStrucProvider(int id)
        {
            if (dicStrucProvider.ContainsKey(id))
                return dicStrucProvider[id];
            StructureProvider sp = Session.GetStructureProvider();
            UpdateRevisions(sp);
            dicStrucProvider[id] = sp;
            return sp;
        }

        public void UpdateTypes(string imgPath)
        {
            string typename;
            bool first = true;
            bool needSave;
            StringBuilder sb = new StringBuilder();
            
            sb.Append("\"types\":{\"types\":{");
            foreach (var type in Session.Types)
            {
                MemoryStream ms = new MemoryStream(type.Icon);
                Image img = Image.FromStream(ms);

                typename = "type" + type.Idnum.ToString();
                string path = Path.Combine(imgPath, typename + ".png");

                var file = new FileInfo(path);
                needSave = true;
                if (file.Exists)
                {
                    using (var ms2 = new MemoryStream(File.ReadAllBytes(path)))
                    {
                        if (ms.GetHashCode() == ms2.GetHashCode())
                            needSave = false;
                    }
                }

                if (needSave) img.Save(path, ImageFormat.Png);
                
                if (!first) sb.Append(",");
                else first = false;
                sb.Append("\"" + typename + "\":{");
                sb.Append("\"icon\":{");
                sb.Append("\"image\":\"Content/Images/" + typename + ".png\"");
                sb.Append("}}");
            }
            sb.Append("}}");
            httpSession["JTTypes"] = sb.ToString();
        }

        public void UpdateRevisions(StructureProvider strucProvider)
        {
            Revisions = Session.Revisions;
            if (strucProvider != null)
            {
                if (strucProvider.CurrentRevision != null && Revisions != null)
                {
                    foreach (var item in Revisions)
                    {
                        if (strucProvider.CurrentRevision.Equals(item))
                        {
                            strucProvider.CurrentRevision = item;
                            break;
                        }
                    }
                }
                if (strucProvider.CurrentRevision == null && Revisions != null && Revisions.Length > 0)
                    strucProvider.CurrentRevision = RevisionInfo.Current;
            }
        }

        public int GenerateId()
        {
            return pageid++;
        }
    }
}