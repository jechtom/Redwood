using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Binding.Parsing.Expressions
{
    public class EvaluateBindingVisitor : BindingVisitor<object>
    {
        protected override object VisitConstant(BindingConstantExpression expression, object accumulator)
        {
            return expression.Value;
        }

        protected override object VisitGetProperty(BindingGetPropertyExpression expression, object accumulator)
        {
            if (accumulator == null) return null;

            var type = accumulator.GetType();
            var prop = type.GetProperty(expression.PropertyName);
            if (prop == null)
            {
                // TODO: error handling
                throw new ArgumentException(string.Format("The object of type {0} does not have property called {1}!", type, expression.PropertyName));
            }

            var result = prop.GetValue(accumulator);

            if (expression.NextExpression != null)
            {
                return Visit(expression.NextExpression, result);
            }
            else
            {
                return result;
            }
        }

        protected override object VisitCallMethod(BindingCallMethodExpression expression, object accumulator)
        {
            if (accumulator == null) return null;

            var type = accumulator.GetType();
            var methods = type.GetMethods().Where(m => m.GetParameters().Length == expression.Arguments.Count).ToList();

            if (methods.Count == 0)
            {
                // TODO: error handling
                throw new Exception(string.Format("The object of type {0} does not have a method {1} that accepts {2} arguments!", type, expression.MethodName, expression.Arguments.Count));
            }
            else if (methods.Count > 1)
            {
                // TODO: error handling
                throw new Exception(string.Format("The object of type {0} has multiple methods {1} that accept {2} arguments! Only one of them is permitted.", type, expression.MethodName, expression.Arguments.Count));
            }

            return methods[0].Invoke(accumulator, expression.Arguments.Select(a => Visit(a, accumulator)).ToArray());
        }

    }
}
