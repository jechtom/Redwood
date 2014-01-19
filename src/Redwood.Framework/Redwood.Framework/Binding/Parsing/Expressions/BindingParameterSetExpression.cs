using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Binding.Parsing.Expressions
{
    internal class BindingParameterSetExpression : BindingExpression
    {

        public string ParameterName { get; set; }

        public BindingExpression Value { get; set; }

    }
}