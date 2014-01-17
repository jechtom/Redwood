using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml
{
    public class ControlTypeActivator
    {
        static readonly ControlTypeActivator defaultActivator = new ControlTypeActivator();

        public static ControlTypeActivator Default
        {
            get
            {
                return defaultActivator;
            }
        }

        ConcurrentDictionary<Type, ConstructorInfo> defaultConstructorCache;

        public ControlTypeActivator()
        {
            defaultConstructorCache = new ConcurrentDictionary<Type, ConstructorInfo>();
        }

        public object Activate(Type clrType)
        {
            var constructor = defaultConstructorCache.GetOrAdd(clrType, GetConstructorForType);
            if (constructor == null)
                throw new InvalidOperationException("No default constructor found for type: " + clrType.FullName);

            var result = constructor.Invoke(null);
            return result;
        }

        protected virtual ConstructorInfo GetConstructorForType(Type type)
        {
            var constructor = type.GetConstructor(System.Type.EmptyTypes);
            return constructor;
        }
    }
}
