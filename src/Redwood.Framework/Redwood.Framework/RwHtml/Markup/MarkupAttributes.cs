using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwood.Framework.RwHtml.Markup
{
    /// <summary>
    /// Case insensitive attributes dictionary.
    /// </summary>
    public class MarkupAttributes : IEnumerable<KeyValuePair<NameWithPrefix, MarkupValue>>
    {
        private Dictionary<NameWithPrefix, MarkupValue> values;

        private void EnsureDictionaryExists()
        {
            if (values == null)
                values = new Dictionary<NameWithPrefix, MarkupValue>();
        }

        public bool IsEmpty
        {
            get
            {
                return values == null || values.Count == 0;
            }
        }

        public void Add(NameWithPrefix key, MarkupValue value)
        {
            EnsureDictionaryExists();
            values.Add(key, value);
        }

        public bool ContainsKey(NameWithPrefix key)
        {
            if (values == null)
                return false;

            return values.ContainsKey(key);
        }

        public bool Remove(NameWithPrefix key)
        {
            if (values == null)
                return false;

            return values.Remove(key);
        }

        public bool TryGetValue(NameWithPrefix key, out MarkupValue value)
        {
            if (values == null)
            {
                value = null;
                return false;
            }

            return TryGetValue(key, out value);
        }

        public MarkupValue this[NameWithPrefix key]
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

        public IEnumerator<KeyValuePair<NameWithPrefix, MarkupValue>> GetEnumerator()
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
