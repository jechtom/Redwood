using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwood.Framework.Controls
{
    public interface IBindingContext
    {

        IBindingContext Parent { get; }

        object ViewModel { get; set; }

    }
}
