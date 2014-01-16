using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Binding
{
    public class PropertyBasicAccessor : IPropertyAccessor
    {
        public PropertyBasicAccessor(PropertyInfo propInfo)
        {
            if (propInfo == null)
                throw new ArgumentNullException("propInfo");

            PropertyInfo = propInfo;
        }

        public PropertyInfo PropertyInfo
        {
            get;
            private set;
        }

        public void SetValue(object instance, object value)
        {
            PropertyInfo.SetValue(instance, value);
        }
    }
}
