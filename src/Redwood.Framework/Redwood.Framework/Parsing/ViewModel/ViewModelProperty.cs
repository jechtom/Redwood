using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Parsing.ViewModel
{
    public class ViewModelProperty
    {

        public string PropertyName { get; set; }

        public Type PropertyType { get; set; }

        public bool IsReadOnly { get; set; }

        public string ClientImplementation { get; set; }

    }
}