using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Generation
{
    public class AttributeList<T> : IEnumerable<KeyValuePair<string, T>>
    {

        private Dictionary<string, T> data = new Dictionary<string, T>();


        /// <summary>
        /// Determines whether the collection contains the specified key.
        /// </summary>
        public bool ContainsKey(string key)
        {
            return data.ContainsKey(key);
        }

        /// <summary>
        /// Adds the specified key and value to the collection.
        /// </summary>
        public void Add(string key, T value)
        {
            data.Add(key, value);
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        public bool Remove(string key)
        {
            return data.Remove(key);
        }

        /// <summary>
        /// Gets or sets the <see cref="System.String"/> with the specified key.
        /// </summary>
        public T this[string key]
        {
            get { return data[key]; }
            set { data[key] = value; }
        }

        /// <summary>
        /// Tries to the get value.
        /// </summary>
        public bool TryGetValue(string key, out T value)
        {
            return data.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the value or default.
        /// </summary>
        public T GetValueOrDefault(string key, T defaultValue = default(T))
        {
            return ContainsKey(key) ? this[key] : defaultValue;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return data.Count;
            }
        }
    }
}