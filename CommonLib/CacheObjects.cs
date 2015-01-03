using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace COTES.ISTOK
{
    /// <summary>
    /// Класс кеша с удалением.
    /// Хранит н(по умолчанию 100) объектов в кеше, 
    /// в случае превышения количества объекто
    /// удаляет объект с наименьшим числом 
    /// обращений.
    /// </summary>
    /// <typeparam name="K">
    ///     Тип ключа.
    /// </typeparam>
    /// <typeparam name="V">
    ///     Тип объекта.
    /// </typeparam>
    public class CacheObjects <K, V> : IEnumerable<V>
    {
        /// <summary>
        /// По умолчанию храниться 100 объектов.
        /// </summary>
        private const int default_object_limit = 100;
        /// <summary>
        /// Максимальное количество кешируемых объектов.
        /// </summary>
        private uint objects_limit = default_object_limit;
        /// <summary>
        /// Собственно сам кеш в виде словаря.
        /// </summary>
        private Dictionary<K, RatiedObject<V>> cache = 
            new Dictionary<K, RatiedObject<V>>();

        //public delegate K KeyGenerator(T value);

        //private

        /// <summary>
        /// Класс обертка для хранимых объектов.
        /// Добавляет своей функциональностью
        /// счетчик обращений.
        /// </summary>
        /// <typeparam name="S">
        ///     Всегда S=V.
        /// </typeparam>
        private class RatiedObject<S>
        {
            internal int RequestCount { set; get; }
            internal S Object { set; get; }
        }

        /// <summary>
        /// Максимальное количество хранимых
        /// объектов.
        /// При изменении происходит нормализация.
        /// </summary>
        public uint ObjectsLimit 
        { 
            set
            {
                objects_limit = value;

                while (cache.Count > objects_limit)
                    RemoveWithMinRating();
            }
            get
            { return objects_limit; }
        }

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public CacheObjects()
        { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="max_object_count">
        ///     Максимальное количество 
        ///     кешируемых объектов.
        /// </param>
        public CacheObjects(uint max_object_count)
        { objects_limit = max_object_count; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(K key)
        { return cache.ContainsKey(key); }

        public void Add(K key, V value)
        {
            const int init_reauest_count = 1;
            const int new_object = 1;

            if (cache.Count + new_object > objects_limit)
                RemoveWithMinRating();

            cache.Add(key, new RatiedObject<V>()
                               {
                                   RequestCount = init_reauest_count,
                                   Object = value
                               });
        }

        public void Remove(K key)
        { cache.Remove(key); }

        public void Clear()
        { cache.Clear(); }

        public V this[K key]
        {
            set { cache[key].Object = value; }
            get 
            {
                var current = cache[key];
                ++current.RequestCount;
                return current.Object; 
            }
        }

        public IEnumerator<V> GetEnumerator()
        {
            return (from it in cache 
                        select it.Value.Object).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void RemoveWithMinRating()
        {
            int min_request = int.MaxValue;
            K key = default(K);

            foreach (var it in cache)
                if (it.Value.RequestCount < min_request)
                {
                    key = it.Key;
                    min_request = it.Value.RequestCount;
                }

            cache.Remove(key);
        }
    }
}
