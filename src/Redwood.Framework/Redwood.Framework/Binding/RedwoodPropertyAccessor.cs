using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Binding
{
    public class RedwoodPropertyAccessor : IPropertyAccessor
    {
        public RedwoodPropertyAccessor(RedwoodProperty propInfo)
        {
            if (propInfo == null)
                throw new ArgumentNullException("propInfo");

            PropertyInfo = propInfo;
        }

        public RedwoodProperty PropertyInfo
        {
            get;
            private set;
        }

        public void SetValue(object instance, object value)
        {
            var instanceBindable = instance as RedwoodBindable;
            if (instanceBindable == null)
                throw new InvalidOperationException("Instance is not RedwoodBindable. RedwoodProperty can be set only on RedwoodBindable instances.");

            instanceBindable.SetValue(PropertyInfo, value);
        }
    }
}
