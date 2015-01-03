using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.ASC.Audit;
using NLog;

namespace COTES.ISTOK.Assignment.Audit
{
    class AuditServer : IAuditServer
    {
        Logger log = LogManager.GetCurrentClassLogger();

        AuditEntities context;

        public AuditServer()
        {
            context = GetContext();
        }

        public void WriteAuditEntry(AuditEntry auditEntry)
        {
            if (!auditEntry.IsEmpty)
            {
                context.AuditEntries.Add(auditEntry);

                int count = context.SaveChanges();

                log.Trace("Сохранение аудита {0}.", auditEntry); 
            }
        }

        readonly DateTime minimalDateTime = new DateTime(2000, 01, 01);

        //public IEnumerable<AuditEntry> ReadAuditEntries(DateTime startTime, DateTime endTime)
        //{
        //    //context.Configuration.ProxyCreationEnabled = false;
        //    if (startTime < minimalDateTime)
        //        startTime = minimalDateTime;
        //    if (endTime < minimalDateTime)
        //        endTime = minimalDateTime;

        //    var ret = from e in context.AuditEntries
        //              where startTime <= e.Time && e.Time <= endTime
        //              select e;

        //    foreach (var item in ret)
        //    {
        //        //yield return new AuditEntry(item);
        //        yield return DeProxier.GetDeProxier<AuditEntry>().DeProxy(item);
        //    }
        //}

        //public IEnumerable<AuditEntry> ReadAuditEntries(int indexStart, int indexEnd)
        //{
        //    var ret = (from e in context.AuditEntries
        //               orderby e.ID ascending
        //               select e).Skip(indexStart).Take(indexEnd - indexStart);

        //    foreach (var item in ret)
        //    {
        //        //yield return new AuditEntry(item);
        //        yield return DeProxier.GetDeProxier<AuditEntry>().DeProxy(item);
        //    }
        //}

        public IEnumerable<AuditEntry> ReadAuditEntries(AuditRequestContainer request) 
        {
            IEnumerable<AuditEntry> entries = null;

            IEnumerable<AuditType> types = null;
            IEnumerable<AuditUser> users = null;
            IEnumerable<AuditGroup> groups = null;
            
            IEnumerable<AuditUnit> units = null;
            IEnumerable<AuditProp> props = null;
            IEnumerable<AuditLob> lobs = null;


            // apply filter by UnitNode
            if (request.NodeFilter != null)
            {
                units = from u in units ?? context.AuditUnits
                        where u.UnitNodeID == request.NodeFilter.Idnum
                        select u;
                props = from p in props ?? context.AuditProps
                        where p.UnitNodeID == request.NodeFilter.Idnum
                        select p;
                lobs = from p in lobs ?? context.AuditLobs
                       where p.UnitNodeID == request.NodeFilter.Idnum
                       select p;
            }

            // apply filter by text
            if (!String.IsNullOrEmpty(request.FilterText))
            {
                types = from t in types ?? context.AuditTypes
                        where (t.TypeNameOld != null && t.TypeNameOld.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (t.TypeNameNew != null && t.TypeNameNew.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (t.TypePropsOld != null && t.TypePropsOld.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (t.TypePropsNew != null && t.TypePropsNew.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        select t;
                users = from u in users ?? context.AuditUsers
                        where (u.UserLoginOld != null && u.UserLoginOld.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (u.UserLoginNew != null && u.UserLoginNew.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (u.UserFullNameOld != null && u.UserFullNameOld.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (u.UserFullNameNew != null && u.UserFullNameNew.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (u.UserPositionOld != null && u.UserPositionOld.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (u.UserPositionNew != null && u.UserPositionNew.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (u.UserGroupsOld != null && u.UserGroupsOld.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (u.UserGroupsNew != null && u.UserGroupsNew.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (u.UserRoleOld != null && u.UserRoleOld.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (u.UserRoleNew != null && u.UserRoleNew.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        select u;
                groups = from g in groups ?? context.AuditGroups
                         where (g.GroupNameOld != null && g.GroupNameOld.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                         || (g.GroupNameNew != null && g.GroupNameNew.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                         || (g.GroupDescriptionOld != null && g.GroupDescriptionOld.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                         || (g.GroupDescriptionNew != null && g.GroupDescriptionNew.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                         select g;
                units = from u in units ?? context.AuditUnits
                        where (u.FullPathOld != null && u.FullPathOld.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (u.FullPathNew != null && u.FullPathNew.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (u.TypeNameOld != null && u.TypeNameOld.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (u.TypeNameNew != null && u.TypeNameNew.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        select u;
                props = from p in props ?? context.AuditProps
                        where (p.PropName != null && p.PropName.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (p.RevisionBrief != null && p.RevisionBrief.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (p.UnitNodeFullPath != null && p.UnitNodeFullPath.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (p.ValueOld != null && p.ValueOld.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        || (p.ValueNew != null && p.ValueNew.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        select p;
                lobs = from p in lobs ?? context.AuditLobs
                       where (p.PropName != null && p.PropName.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                       || (p.RevisionBrief != null && p.RevisionBrief.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                       || (p.UnitNodeFullPath != null && p.UnitNodeFullPath.IndexOf(request.FilterText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                       select p;
            }

            // union entity collection
            if (types != null)
            {
                var tmp = from t in types select t.AuditEntry;
                entries = entries == null ? tmp : entries.Union(tmp);
            }
            if (users != null)
            {
                var tmp = from u in users select u.AuditEntry;
                entries = entries == null ? tmp : entries.Union(tmp);
            }
            if (groups != null)
            {
                var tmp = from u in groups select u.AuditEntry;
                entries = entries == null ? tmp : entries.Union(tmp);
            }
            if (units != null)
            {
                var tmp = from u in units select u.AuditEntry;
                entries = entries == null ? tmp : entries.Union(tmp);
            }
            if (props != null)
            {
                var tmp = entries.Union(from p in props select p.AuditEntry);
                entries = entries == null ? tmp : entries.Union(tmp);
            }
            if (lobs != null)
            {
                var tmp = entries.Union(from p in lobs select p.AuditEntry);
                entries = entries == null ? tmp : entries.Union(tmp);
        }

            if (entries==null)
            {
                entries = context.AuditEntries;
            }
            // apply filter by categories
            if (request.CategoryFilter != AuditCategory.AllCategories)
        {
                entries = from e in entries
                      where
                      ((request.CategoryFilter & AuditCategory.TypesChange) == AuditCategory.TypesChange && e.AuditTypes.Count > 0)
                      || ((request.CategoryFilter & AuditCategory.GroupsChange) == AuditCategory.GroupsChange && e.AuditGroups.Count > 0)
                      || ((request.CategoryFilter & AuditCategory.UsersChange) == AuditCategory.UsersChange && e.AuditUsers.Count > 0)
                      || ((request.CategoryFilter & AuditCategory.UnitsChange) == AuditCategory.UnitsChange && (e.AuditUnits.Count + e.AuditProps.Count + e.AuditLobs.Count) > 0)
                      || ((request.CategoryFilter & AuditCategory.CalcStarts) == AuditCategory.CalcStarts && e.AuditCalcStarts.Count > 0)
                      || ((request.CategoryFilter & AuditCategory.ValuesChange) == AuditCategory.ValuesChange && e.AuditValues.Count > 0)
                      select e;
            }

            // set query limits
            if (request.StartTime > DateTime.MinValue)
            {
                entries = from e in entries
                          where request.StartTime <= e.Time && e.Time <= request.EndTime
                          orderby e.Time descending
                          select e;
            }
            else
            {
                entries = (from e in entries
                           orderby e.Time descending
                           select e).Skip(request.StartIndex)
                           .Take(request.EndIndex - request.StartIndex);
        }

            var deproxier = new DeProxier();

            // deproxy results before return
            foreach (var item in entries)
            {
                yield return deproxier.GetDeProxier<AuditEntry>().DeProxy(item);
            }
        }

        class DeProxier
        {
            Dictionary<Type, DeProxier> deProxierDictionary = new Dictionary<Type, DeProxier>();

            protected DeProxier Base { get; set; }

            public DeProxier GetDeProxier(Type type)
            {
                DeProxier deProxier;
                if (!deProxierDictionary.TryGetValue(type, out deProxier))
                {
                    deProxier= Activator.CreateInstance(typeof(DeProxier<>).MakeGenericType(type)) as DeProxier;

                    deProxier.Base = this;
                    deProxierDictionary[type] = deProxier;
                }
                return deProxier;
            }

            public DeProxier<T> GetDeProxier<T>()
                where T : new()
            {
                return GetDeProxier(typeof(T)) as DeProxier<T>;
            }

            public virtual Object DeProxy(Object obj)
            {
                return null;
            }
        }

        class DeProxier<T> : DeProxier
            where T : new()
        {
            Dictionary<T, T> deproxyCache = new Dictionary<T, T>();


            public override object DeProxy(object obj)
            {
                return DeProxy((T)obj);
            }

            public T DeProxy(T obj)
            {
                if (System.Data.Objects.ObjectContext.GetObjectType(obj.GetType()) == obj.GetType())
                {
                    return obj;
                }

                if (deproxyCache.ContainsKey(obj))
                {
                    return deproxyCache[obj];
                }

                var newObj = new T();
                deproxyCache[obj] = newObj;

                var props = typeof(T).GetProperties();

                foreach (var item in props)
                {
                    if (item.CanRead)
                    {
                        var value = item.GetValue(obj, null);

                        if (value != null)
                        {
                            if (item.PropertyType.IsGenericType
                                && item.PropertyType.GetGenericTypeDefinition().FullName == "System.Collections.Generic.HashSet`1")
                            {
                                var targetCollection = item.GetValue(newObj, null);

                                var deProxier = Base.GetDeProxier(item.PropertyType.GetGenericArguments().First());

                                foreach (var subItem in value as IEnumerable)
                                {
                                    var collectionItem = deProxier.DeProxy(subItem);

                                    item.PropertyType.InvokeMember("Add", System.Reflection.BindingFlags.InvokeMethod, null, targetCollection, new Object[] { collectionItem });
                                }
                            }
                            else if (item.CanWrite)
                            {
                                if (System.Data.Objects.ObjectContext.GetObjectType(value.GetType()) != value.GetType())
                                {
                                    var deProxier = Base.GetDeProxier(System.Data.Objects.ObjectContext.GetObjectType(value.GetType()));

                                    item.SetValue(newObj, deProxier.DeProxy(value), null);
                                }
                                else
                                {
                                    item.SetValue(newObj, value, null);
                                }
                            }
                        }
                    }
                }

                return newObj;
            }
        }

        private AuditEntities GetContext()
        {
            System.Data.SqlClient.SqlConnectionStringBuilder scsb = new System.Data.SqlClient.SqlConnectionStringBuilder();

            scsb.DataSource = GlobalSettings.Instance.DataBase.Host;
            scsb.InitialCatalog = GlobalSettings.Instance.DataBase.Name.Replace("station", "audit");
            scsb.UserID = GlobalSettings.Instance.DataBase.User;
            scsb.Password = CommonData.SecureStringToString(
                             CommonData.DecryptText(
                                 CommonData.Base64ToString(
                                     GlobalSettings.Instance.DataBase.Password)));
            scsb.MultipleActiveResultSets = true;
            // при создании новой базы возможно стоит указывать путь к файлу:
            // scsb.AttachDBFilename

            System.Data.EntityClient.EntityConnectionStringBuilder ecb = new System.Data.EntityClient.EntityConnectionStringBuilder();
            ecb.Metadata = "res://*/Audit.AuditModel.csdl|res://*/Audit.AuditModel.ssdl|res://*/Audit.AuditModel.msl";
            ecb.Provider = "System.Data.SqlClient";
            ecb.ProviderConnectionString = scsb.ConnectionString;

            var context = new AuditEntities(ecb.ConnectionString);
            //context.Configuration.ProxyCreationEnabled = false;
            //context.Configuration.LazyLoadingEnabled = false;

            if (context.Database.CreateIfNotExists())
            {
                log.Info("Созданна база данных для аудита {0}", context.Database.Connection.Database);
            }

            return context;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (context != null)
            {
                context.Dispose();
            }
        }

        #endregion
    }

    partial class AuditEntities
    {
        public AuditEntities(String connectionString)
            : base(connectionString)
        {

        }
    }

    //partial class AuditEntry
    //{
    //    public AuditEntry(COTES.ISTOK.ASC.UserNode userNode)
    //        : this()
    //    {
    //        Time = DateTime.Now;
    //        UserLogin = userNode.Text;
    //        UserFullName = userNode.UserFullName;
    //        UserPosition = userNode.Position;
    //        UserRole = userNode.Roles;
    //    }

    //    public override string ToString()
    //    {
    //        StringBuilder stringbuilder = new StringBuilder();

    //        stringbuilder.AppendFormat("{{{0} [{1}] ", Time, UserLogin);
    //        int length = stringbuilder.Length;

    //        foreach (var tuple in new Tuple<String, int>[] 
    //        {
    //            Tuple.Create("types", AuditTypes.Count),
    //            Tuple.Create("users", AuditUsers.Count),
    //            Tuple.Create("groups", AuditGroups.Count),
    //            Tuple.Create("units", AuditUnits.Count),
    //            Tuple.Create("props", AuditProps.Count),
    //            Tuple.Create("lobs", AuditLobs.Count),
    //            Tuple.Create("values", AuditValues.Count),
    //            Tuple.Create("calcs", AuditCalcStarts.Count),
    //        })
    //        {
    //            if (tuple.Item2 > 0)
    //            {
    //                if (stringbuilder.Length > length)
    //                {
    //                    stringbuilder.Append(", ");
    //                }
    //                stringbuilder.AppendFormat("{0}: {1}", tuple.Item1, tuple.Item2);
    //            } 
    //        }
    //        stringbuilder.Append("}");

    //        return stringbuilder.ToString();
    //    }

    //    public bool IsEmpty
    //    {
    //        get
    //        {
    //            return AuditUnits.Count == 0
    //                && AuditProps.Count == 0
    //                && AuditLobs.Count == 0
    //                && AuditValues.Count == 0
    //                && AuditCalcStarts.Count == 0
    //                && AuditTypes.Count == 0
    //                && AuditUsers.Count == 0
    //                && AuditGroups.Count == 0;
    //        }
    //    }
    //}

    //partial class AuditType
    //{
    //    public Guid ExtGuid
    //    {
    //        get { return new Guid(ExtGuidBinary); }
    //        set { ExtGuidBinary = value.ToByteArray(); }
    //    }
    //}
}
