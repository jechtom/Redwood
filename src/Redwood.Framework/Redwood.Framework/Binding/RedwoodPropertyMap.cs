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

        private ReaderWriterLockSlim propertiesLock;
        private Dictionary<NameKey, RedwoodProperty> propertiesDict;
        private int maximumId = 0;

        public RedwoodPropertyMap()
        {
            propertiesLock = new ReaderWriterLockSlim();
            propertiesDict = new Dictionary<NameKey, RedwoodProperty>();
        }
        
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

            propertiesLock.EnterWriteLock();
            try
            {
                if (propertiesDict.ContainsKey(key))
                    throw new InvalidOperationException(string.Format("Redwood property {0} already exists on {1}.", property.Name, property.OwnerType.FullName));

                propertiesDict.Add(key, property);
            }
            finally
            {
                propertiesLock.ExitWriteLock();
            }
        }

        public int AssignNewId()
        {
            return Interlocked.Increment(ref maximumId);
        }

        public RedwoodProperty GetPropertyByNameForType(string propertyName, Type targetType)
        {
            propertiesLock.EnterReadLock();
            try
            {
                return this.propertiesDict.Values
                    .Where(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase) && p.IsApplicableOn(targetType))
                    .SingleOrDefault();
            }
            finally
            {
                propertiesLock.ExitReadLock();
            }
        }
    }
}
