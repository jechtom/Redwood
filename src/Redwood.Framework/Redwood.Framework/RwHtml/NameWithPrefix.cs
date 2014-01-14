using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml
{
    public struct NameWithPrefix
    {
        private string name;
        
        private string prefix;

        public NameWithPrefix(string prefix, string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Parameter is null or whitespace.", "name");

            this.name = name;
            this.prefix = prefix;
        }

        /// <summary>
        /// Gets local name.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Gets prefix (namespace).
        /// </summary>
        public string Prefix
        {
            get
            {
                return prefix;
            }
        }

        /// <summary>
        /// Gets if prefix is null.
        /// </summary>
        public bool HasPrefix
        {
            get
            {
                return Prefix != null;
            }
        }

        public static NameWithPrefix Parse(string text)
        {
            if(text == null)
            {
                throw new ArgumentNullException("text");
            }

            // format: "prefix:name"
            var index = text.IndexOf(':');

            if(index == 0 || index == text.Length) // ":" as first or last character
            {
                throw new FormatException(string.Format("Invalid format of name with prefix: \"{0}\"", text));
            }

            if(index == -1)
            {
                return new NameWithPrefix(null, text);
            }

            return new NameWithPrefix(
                    text.Substring(0, index),
                    text.Substring(index + 1)
                );
        }

        public override string ToString()
        {
            return Prefix + ":" + Name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is NameWithPrefix))
                return false;

            var obj2 = (NameWithPrefix)obj;

            return obj2.prefix == prefix && obj2.name == name;
        }

        public override int GetHashCode()
        {
            var result = name.GetHashCode();
            if (HasPrefix)
                result ^= prefix.GetHashCode();

            return result;
        }
    }
}
