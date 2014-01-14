using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwood.Framework.RwHtml
{
    public interface IPropertyAccessor
    {
        Type PropertyType { get; }
        string Name { get; }
    }
}
