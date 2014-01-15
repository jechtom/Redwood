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
        private string[] names;
        
        private string prefix;

        public NameWithPrefix(string prefix, string[] names)
        {
            if(names == null || names.Length == 0)
                throw new ArgumentException("Parameter is null contains no elements.", "names");

            if (names.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Parameter contains null or white space value.", "names");

            this.names = names;
            this.prefix = prefix;
        }

        /// <summary>
        /// Gets local name(s).
        /// </summary>
        public string[] Names
        {
            get
            {
                return names;
            }
        }

        /// <summary>
        /// Gets prefix (namespace) or null if not provided.
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

        /// <summary>
        /// Gets if this value has more names.
        /// </summary>
        public bool HasMoreNames
        {
            get
            {
                return names.Length > 1;
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
                return new NameWithPrefix(null, ParseNames(text));
            }

            return new NameWithPrefix(
                    text.Substring(0, index),
                    ParseNames(text.Substring(index + 1))
                );
        }

        private static string[] ParseNames(string text)
        {
            var result = text.Split(new char[] { '.' },  StringSplitOptions.None);
            return result;
        }

        public override string ToString()
        {
            return Prefix + ":" + string.Join(".", Names ?? new [] { "N/A" });
        }

        public override bool Equals(object obj)
        {
            if (!(obj is NameWithPrefix))
                return false;

            var obj2 = (NameWithPrefix)obj;

            if (obj2.prefix != prefix)
                return false;

            if (obj2.names.Length != names.Length)
                return false;

            for (int i = 0; i < names.Length; i++)
            {
                if (obj2.names[i] != names[i])
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = names[0].GetHashCode();
            for (int i = 1; i < names.Length; i++)
            {
                result ^= names[i].GetHashCode();
            }
            if (HasPrefix)
                result ^= prefix.GetHashCode();

            return result;
        }

        public bool BeginsWith(NameWithPrefix beginning)
        {
            // diff. prefixes?
            if (HasPrefix != beginning.HasPrefix || !string.Equals(prefix, beginning.prefix, StringComparison.OrdinalIgnoreCase))
                return false;

            // beginning longer than current value?
            if (beginning.names.Length > names.Length)
                return false;

            for (int i = 0; i < beginning.names.Length; i++)
            {
                if (!string.Equals(names[i], beginning.names[i], StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }

        public string SingleName()
        {
            if (this.names.Length > 1)
                throw new InvalidOperationException("Single name value was expected on name: " + this.ToString());

            return names[0];
        }
    }
}
