using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotArchaius
{
    public class InMemoryDynamicPropertySupport : IDynamicPropertySupport, IDictionary<string, string>, IDictionary
    {
        protected readonly IDictionary<string, string> Dictionary = new Dictionary<string, string>();

        protected readonly List<IPropertyEventListener> PropertyEventListeners = new List<IPropertyEventListener>();

        public string GetString(string propertyName)
        {
            string value;
            if (Dictionary.TryGetValue(propertyName, out value))
            {
                return value;
            }

            return null;
        }

        public void AddPropertyEventListener(IPropertyEventListener propertyEventListener)
        {
            PropertyEventListeners.Add(propertyEventListener);
        }

        public int Count => Dictionary.Count;

        public bool IsEmpty => Dictionary.Count == 0;

        public ICollection<string> Keys => Dictionary.Keys;

        public ICollection<string> Values => Dictionary.Values;

        public string this[string key]
        {
            get { return Dictionary[key]; }
            set
            {
                PropertyEventListeners.ForEach(x => x.OnUpdatingProperty(this, key, value));
                Dictionary[key] = value;
                PropertyEventListeners.ForEach(x => x.OnPropertyUpdated(this, key, value));
            }
        }

        public virtual void Clear()
        {
            PropertyEventListeners.ForEach(x => x.OnClearingProperties(this));
            Dictionary.Clear();
            PropertyEventListeners.ForEach(x => x.OnPropertiesCleared(this));
        }

        public virtual bool TryGetValue(string key, out string value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return Dictionary.TryGetValue(key, out value);
        }

        public virtual string AddOrUpdate(string key, Func<string, string> newValueFactory, Func<string, string, string> updateValueFactory)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (newValueFactory == null) throw new ArgumentNullException(nameof(newValueFactory));
            if (updateValueFactory == null) throw new ArgumentNullException(nameof(updateValueFactory));

            if (Dictionary.ContainsKey(key))
            {
                var oldValue = Dictionary[key];
                var updatedValue = updateValueFactory(key, oldValue);
                this[key] = updatedValue;
                return updatedValue;
            }

            var newValue = newValueFactory(key);
            ((IDictionary<string, string>)this).Add(key, newValue);

            return newValue;
        }

        public virtual string AddOrUpdate(string key, string newValue, Func<string, string, string> updateValueFactory)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (updateValueFactory == null) throw new ArgumentNullException(nameof(updateValueFactory));

            return AddOrUpdate(key, s => newValue, updateValueFactory);
        }

        public virtual string GetOrAdd(string key, Func<string, string> valueFactory)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            if (Dictionary.ContainsKey(key))
            {
                return Dictionary[key];
            }

            var value = valueFactory(key);
            ((IDictionary<string, string>)this).Add(key, value);

            return value;
        }

        public virtual string GetOrAdd(string key, string value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return GetOrAdd(key, s => value);
        }

        public virtual bool TryAdd(string key, string value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            if (Dictionary.ContainsKey(key))
            {
                return false;
            }

            ((IDictionary<string, string>)this).Add(key, value);
            return true;
        }

        public virtual bool TryUpdate(string key, string newValue, string comparisonValue)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            if (Dictionary.ContainsKey(key) && Dictionary[key] == comparisonValue)
            {
                this[key] = newValue;
                return true;
            }

            return false;
        }

        public virtual bool TryRemove(string key, out string value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            value = null;

            if (!Dictionary.ContainsKey(key))
            {
                return false;
            }

            try
            {
                ((IDictionary<string, string>)this).Remove(key);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual bool ContainsKey(string key)
        {
            return Dictionary.ContainsKey(key);
        }

        public virtual IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        public virtual KeyValuePair<string, string>[] ToArray()
        {
            return Dictionary.ToArray();
        }

        void IDictionary<string, string>.Add(string key, string value)
        {
            PropertyEventListeners.ForEach(x => x.OnAddingProperty(this, key, value));
            Dictionary.Add(key, value);
            PropertyEventListeners.ForEach(x => x.OnPropertyAdded(this, key, value));
        }

        ICollection IDictionary.Values => (ICollection)Values;

        bool IDictionary.IsReadOnly => ((IDictionary)Dictionary).IsReadOnly;

        bool IDictionary.IsFixedSize => ((IDictionary)Dictionary).IsFixedSize;

        bool IDictionary<string, string>.Remove(string key)
        {
            var value = Dictionary[key];
            PropertyEventListeners.ForEach(x => x.OnRemovingProperty(this, key, value));

            if (Dictionary.Remove(key))
            {
                PropertyEventListeners.ForEach(x => x.OnPropertyRemoved(this, key));
                return true;
            }

            return false;
        }

        ICollection IDictionary.Keys => (ICollection)Keys;

        void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
        {
            PropertyEventListeners.ForEach(x => x.OnAddingProperty(this, item.Key, item.Value));
            Dictionary.Add(item);
            PropertyEventListeners.ForEach(x => x.OnPropertyAdded(this, item.Key, item.Value));
        }

        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)Dictionary).Contains(key);
        }

        void IDictionary.Add(object key, object value)
        {
            PropertyEventListeners.ForEach(x => x.OnAddingProperty(this, key.ToString(), value.ToString()));
            ((IDictionary)Dictionary).Add(key, value);
            PropertyEventListeners.ForEach(x => x.OnPropertyAdded(this, key.ToString(), value.ToString()));
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)Dictionary).GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            var value = ((IDictionary)Dictionary)[key];
            PropertyEventListeners.ForEach(x => x.OnRemovingProperty(this, key.ToString(), value));
            ((IDictionary)Dictionary).Remove(key);
            PropertyEventListeners.ForEach(x => x.OnPropertyRemoved(this, key.ToString()));
        }

        object IDictionary.this[object key]
        {
            get { return ((IDictionary)Dictionary)[key]; }
            set
            {
                PropertyEventListeners.ForEach(x => x.OnUpdatingProperty(this, key.ToString(), value.ToString()));
                ((IDictionary)Dictionary)[key] = value;
                PropertyEventListeners.ForEach(x => x.OnPropertyUpdated(this, key.ToString(), value.ToString()));
            }
        }

        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
        {
            return Dictionary.Contains(item);
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            Dictionary.CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)Dictionary).CopyTo(array, index);
        }

        object ICollection.SyncRoot => ((ICollection)Dictionary).SyncRoot;

        bool ICollection.IsSynchronized => ((ICollection)Dictionary).IsSynchronized;

        bool ICollection<KeyValuePair<string, string>>.IsReadOnly => Dictionary.IsReadOnly;

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            PropertyEventListeners.ForEach(x => x.OnRemovingProperty(this, item.Key, item.Value));

            if (Dictionary.Remove(item))
            {
                PropertyEventListeners.ForEach(x => x.OnPropertyRemoved(this, item.Key));
                return true;
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Dictionary).GetEnumerator();
        }
    }
}