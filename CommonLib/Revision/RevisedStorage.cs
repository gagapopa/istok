using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK
{
    /// <summary>
    /// Представление версионифицированного значения
    /// </summary>
    /// <typeparam name="T">Тип значения</typeparam>
    [Serializable]
    public class RevisedStorage<T> : IEnumerable<RevisionInfo>, ICloneable
    {
        Dictionary<RevisionInfo, T> storage;

        public RevisedStorage()
        {
            storage = new Dictionary<RevisionInfo, T>();
        }

        /// <summary>
        /// Получить значение за указанную ревизию
        /// </summary>
        /// <param name="revision">Ревизия</param>
        /// <returns></returns>
        public T Get(RevisionInfo revision)
        {
            T value;

            if (!storage.TryGetValue(revision, out value))
            {                
                revision = GetRevisionByRevision(revision);
                if (!storage.TryGetValue(revision, out value))
                    value = default(T);
            }

            return value;
        }

        /// <summary>
        /// Получить значение за указанное время
        /// </summary>
        /// <param name="time">Время</param>
        /// <returns></returns>
        public T Get(DateTime time)
        {
            RevisionInfo revision = GetRevisionByTime(time);

            return Get(revision);
        }

        /// <summary>
        /// Должен возвращать значение последней имеющейся ревизии
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            RevisionInfo revision = GetLastRevision();

            return Get(revision);
        }

        /// <summary>
        /// Установить значение параметра для ревизии
        /// </summary>
        /// <param name="revision">Ревизия</param>
        /// <param name="value">Значение</param>
        public void Set(RevisionInfo revision, T value)
        {
            lock (storage)
            {
                ValidateSpecialRevision(revision);

                storage[revision] = value;
            }
        }

        private static void ValidateSpecialRevision(RevisionInfo revision)
        {
            if (revision.Equals(RevisionInfo.Current)
                || revision.Equals(RevisionInfo.Head))
                throw new ArgumentException(String.Format("Ревизия {0} должна заменятся на актуальную ревизию", revision));
        }

        /// <summary>
        /// Должен устанавливать значение последней имеющейся ревизии или Default
        /// </summary>
        /// <param name="value"></param>
        public void Set(T value)
        {
            lock (storage)
            {
                RevisionInfo revision = GetLastRevision();

                Set(revision, value); 
            }
        }

        private RevisionInfo GetLastRevision()
        {
            RevisionInfo revision = (from r in storage.Keys orderby r.Time descending select r).FirstOrDefault();

            return revision == null ? RevisionInfo.Default : revision;
        }

        private RevisionInfo GetRevisionByTime(DateTime time)
        {
            RevisionInfo revision = (from r in storage.Keys where r.Time <= time orderby r.Time descending select r).FirstOrDefault();

            return revision == null ? RevisionInfo.Default : revision;
        }

        private RevisionInfo GetRevisionByRevision(RevisionInfo revision)
        {
            DateTime time = revision.Time;

            ValidateSpecialRevision(revision);

            return GetRevisionByTime(time);
        }

        #region IEnumerable<RevisionInfo> Members

        IEnumerator<RevisionInfo> IEnumerable<RevisionInfo>.GetEnumerator()
        {
            foreach (var revision in storage.Keys)
            {
                yield return revision;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<RevisionInfo>)this).GetEnumerator();
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            var clone = new RevisedStorage<T>();
            T value;
            ICloneable cloneable;

            foreach (var revision in storage.Keys)
            {
                value = storage[revision];

                if ((cloneable = value as ICloneable) != null)
                    value = (T)cloneable.Clone();

                clone.storage[revision] = value;
            }

            return clone;
        }

        #endregion

        public bool Equals(RevisedStorage<T> str, Func<T, T, bool> equalFunc)
        {
            if (!storage.Count.Equals(str.storage.Count))
                return false;

            foreach (var revision in storage.Keys)
            {
                if (!str.storage.ContainsKey(revision)
                    || !equalFunc(storage[revision], str.storage[revision]))
                {
                    return false;
                }
            }
            return true;
        
        }

        public override bool Equals(object obj)
        {
            RevisedStorage<T> storage = obj as RevisedStorage<T>;

            if (storage != null)
                return Equals(storage, (a, b) => Object.Equals(a, b));

            return base.Equals(obj);
        }
    }
}
