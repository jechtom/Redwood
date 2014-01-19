using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Binding.Parsing.Expressions
{
    public class BindingConstantExpression : BindingExpression
    {

        public string Value { get; set; }

        public bool IsQuoted { get; set; }

    }
}