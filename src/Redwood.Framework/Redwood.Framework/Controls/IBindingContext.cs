using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redwood.Framework.ViewModel;

namespace Redwood.Framework.Controls
{
    public class BindingContext
    {

        public BindingContext Parent { get; set; }

        public object ViewModel { get; set; }

    }
}
