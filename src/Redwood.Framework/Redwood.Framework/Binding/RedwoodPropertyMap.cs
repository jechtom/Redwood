using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Redwood.Framework.Binding
{
    internal class RedwoodPropertyMap
    {
        private static readonly RedwoodPropertyMap @default = new RedwoodPropertyMap();

        public static RedwoodPropertyMap Default
        {
            get
            {
                return @default;
            }
        }

        private Hashtable nameKeyTable = new Hashtable();
        private int maximumId = 0;
        
        private class NameKey
        {
            private string name;

            private Type ownerType;

            private int hashCode;

            public NameKey(string name, Type ownerType)
            {
                this.name = name;
                this.ownerType = ownerType;
                this.hashCode = (this.name.GetHashCode() ^ this.ownerType.GetHashCode());
            }

            public override int GetHashCode()
            {
                return this.hashCode;
            }

            public override bool Equals(object obj)
            {
                if(obj == null || obj is NameKey)
                    return false;
                    
                return this.Equals((NameKey)obj);
            }

            public bool Equals(NameKey key)
            {
                return 
                    this.name.Equals(key.name) 
                    && this.ownerType == key.ownerType;
            }
        }

        public void RegisterProperty(RedwoodProperty property)
        {
            var key = new NameKey(property.Name, property.OwnerType);

            lock (nameKeyTable)
            {
                if (nameKeyTable.ContainsKey(key))
                    throw new InvalidOperationException(string.Format("Redwood property {0} already exists on {1}.", property.Name, property.OwnerType.FullName));

                nameKeyTable.Add(key, property);
            }
        }

        public int AssignNewId()
        {
            return Interlocked.Increment(ref maximumId);
        }

        public RedwoodProperty GetPropertyByNameForType(string propertyName, Type ownerType)
        {
            // TODO make faster
            return this.nameKeyTable.Values.Cast<RedwoodProperty>()
                .Where(p => p.Name == propertyName && p.OwnerType.IsAssignableFrom(ownerType))
                .FirstOrDefault();
        }
    }
}
