using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Binding
{
    public abstract class ExpressionBase
    {
        public virtual object GetValue(RedwoodBindable target, RedwoodProperty property)
        {
            return RedwoodProperty.UnsetValue;
        }
    }
}
