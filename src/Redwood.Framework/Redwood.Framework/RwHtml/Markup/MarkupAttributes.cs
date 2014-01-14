using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwood.Framework.RwHtml.Markup
{
    /// <summary>
    /// Case insensitive attributes dictionary.
    /// </summary>
    public class MarkupAttributes : IEnumerable<KeyValuePair<string, MarkupValue>>
    {
        private Dictionary<string, MarkupValue> values;

        private void EnsureDictionaryExists()
        {
            if (values == null)
                values = new Dictionary<string, MarkupValue>(StringComparer.OrdinalIgnoreCase);
        }

        public bool IsEmpty
        {
            get
            {
                return values == null || values.Count == 0;
            }
        }

        public void Add(string key, MarkupValue value)
        {
            EnsureDictionaryExists();
            Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            if (values == null)
                return false;

            return values.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            if (values == null)
                return false;

            return values.Remove(key);
        }

        public bool TryGetValue(string key, out MarkupValue value)
        {
            if (values == null)
            {
                value = null;
                return false;
            }

            return TryGetValue(key, out value);
        }

        public MarkupValue this[string key]
        {
            get
            {
                if (values == null)
                    return null;

                return values[key];
            }
            set
            {
                EnsureDictionaryExists();
                values[key] = value;
            }
        }

        public void Clear()
        {
            if(values != null)
                values.Clear();
        }

        public int Count
        {
            get
            {
                if (IsEmpty)
                    return 0;
                return values.Count;
            }
        }

        public IEnumerator<KeyValuePair<string, MarkupValue>> GetEnumerator()
        {
            if (values == null)
                yield break;

            foreach (var item in values)
            {
                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
