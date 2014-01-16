using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Binding
{
    public interface IPropertyAccessor
    {
        void SetValue(object instance, object value);
    }
}
