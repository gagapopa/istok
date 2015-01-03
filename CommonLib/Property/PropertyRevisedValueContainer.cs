using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK
{
    /// <summary>
    /// Контэйнер для хранений свойств-значений с ревизиями
    /// </summary>
    /// <typeparam name="T">
    /// Тип данных значений свойств
    /// </typeparam>
    [Serializable]
    class PropertyRevisedValueContainer<T> : IRevisedPropertyable<T>
    {
        Dictionary<ItemProperty, RevisedStorage<T>> valueDictionary = new Dictionary<ItemProperty, RevisedStorage<T>>();

        public bool Contains(ItemProperty property)
        {
            return valueDictionary.ContainsKey(property);
        }

        public bool Contains(String propertyName)
        {
            return Contains(new ItemProperty() { Name = propertyName });
        }

        public IEnumerable<ItemProperty> Properties
        {
            get { return valueDictionary.Keys.ToArray(); }
        }

        public T GetPropertyValue(RevisionInfo revision, ItemProperty property)
        {
            RevisedStorage<T> storage;
            if (valueDictionary.TryGetValue(property, out storage))
            {
                return storage.Get(revision);
            }
            return default(T);
        }

        public T GetPropertyValue(RevisionInfo revision, string propertyName)
        {
            return GetPropertyValue(revision, new ItemProperty() { Name = propertyName });
        }

        public void SetPropertyValue(RevisionInfo revision, ItemProperty property, T value)
        {
            RevisedStorage<T> storage;
            if (!valueDictionary.TryGetValue(property, out storage))
            {
                valueDictionary[property] = storage = new RevisedStorage<T>();
            }
            storage.Set(revision, value);
        }

        public void SetPropertyValue(RevisionInfo revision, string propertyName, T value)
        {
            SetPropertyValue(revision, new ItemProperty() { Name = propertyName }, value);
        }

        public RevisedStorage<T> GetPropertyStorage(ItemProperty property)
        {
            RevisedStorage<T> storage;
            if (valueDictionary.TryGetValue(property, out storage))
            {
                return storage;
            }
            return null;
        }

        //public void SetPropertyStorage(ItemProperty property, RevisedStorage<T> storage)
        //{
        //    throw new NotImplementedException();
        //}

        public T this[ItemProperty property]
        {
            get
            {
                return GetPropertyValue(property);
            }
            set
            {
                SetPropertyValue(property, value); 
            }
        }

        public T GetPropertyValue(ItemProperty property)
        {
            RevisedStorage<T> storage;
            if (valueDictionary.TryGetValue(property, out storage))
            {
                return storage.Get();
            }
            return default(T);
        }

        public T GetPropertyValue(string propertyName)
        {
            return GetPropertyValue(new ItemProperty() { Name = propertyName });
        }

        public void SetPropertyValue(ItemProperty property, T value)
        {
            RevisedStorage<T> storage;
            if (!valueDictionary.TryGetValue(property, out storage))
            {
                valueDictionary[property] = storage = new RevisedStorage<T>();
            }
            storage.Set(value);
        }

        public void SetPropertyValue(string propertyName, T value)
        {
            SetPropertyValue(new ItemProperty() { Name = propertyName }, value);
        }
    }
}
