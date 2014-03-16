using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Binding.Parsing.Expressions
{
    public class BindingCallMethodExpression : BindingPathExpression
    {
        public string MethodName { get; set; }

        public List<BindingPathExpression> Arguments { get; private set; }

        public BindingCallMethodExpression()
        {
            Arguments = new List<BindingPathExpression>();
        }

    }
}