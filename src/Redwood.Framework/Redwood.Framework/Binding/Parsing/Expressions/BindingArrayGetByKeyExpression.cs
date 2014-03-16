using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwood.Framework.Binding.Parsing.Expressions
{
    public class BindingArrayGetByKeyExpression : BindingPathExpression
    {
        public string KeyPropertyName { get; set; }

        public string KeyValue { get; set; }

        public bool IsPlaceholder { get; set; }
    }
}
