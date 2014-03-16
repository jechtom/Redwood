using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwood.Framework.Binding
{
    [Flags]
    public enum RedwoodPropertyFlags : uint
    {
        None = 0u,
        IsInherited = 1u,
        IsAttached = 2u,
        IsHtmlAttribute = 4u,
        ReadOnly = 8u,
        IsInheritanceSource = 16u
    }
}
