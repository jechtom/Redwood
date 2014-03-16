using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Binding.Parsing.Expressions
{
    public abstract class BindingPathExpression
    {

        public object Evaluate(object context)
        {
            var visitor = new EvaluateBindingVisitor();
            return visitor.Visit(this, context);
        }

    }
}
