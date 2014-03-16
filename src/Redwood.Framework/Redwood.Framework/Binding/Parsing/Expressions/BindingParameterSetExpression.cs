using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Binding.Parsing.Expressions
{
    internal class BindingParameterSetExpression : BindingPathExpression
    {

        public string ParameterName { get; set; }

        public BindingPathExpression Value { get; set; }

    }
}