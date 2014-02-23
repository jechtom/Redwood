using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Binding.Parsing.Expressions
{
    public class BindingCallMethodExpression : BindingExpression
    {
        public string MethodName { get; set; }

        public List<BindingExpression> Arguments { get; private set; }

        public BindingCallMethodExpression()
        {
            Arguments = new List<BindingExpression>();
        }

    }
}