using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.ViewModel;

namespace Redwood.Framework.Controls
{
    public class RedwoodEventArgs
    {

        public ViewModelBase Root { get; set; }
        
        public string CommandName { get; set; }

        public object Target { get; set; }

        public object[] Parameters { get; set; }
        
    }
}