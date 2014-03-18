using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Binding.Parsing.Expressions
{
    public class BindingEvaluateVisitor : BindingVisitor<object>
    {
        protected override object VisitConstant(BindingConstantExpression expression, object accumulator)
        {
            return expression.Value;
        }

        protected override object VisitGetProperty(BindingGetPropertyExpression expression, object accumulator)
        {
            if (accumulator == null) return null;

            var type = accumulator.GetType();
            object result;

            if (!string.IsNullOrEmpty(expression.PropertyName))
            {
                var prop = type.GetProperty(expression.PropertyName);
                if (prop == null)
                {
                    // TODO: error handling
                    throw new ArgumentException(string.Format("The object of type {0} does not have property called {1}!", type, expression.PropertyName));
                }
                result = prop.GetValue(accumulator);
            }
            else
            {
                result = accumulator;
            }

            if (expression.Indexer != null)
            {
                result = Visit(expression.Indexer, result);
            }

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


        protected override object VisitArrayGetByIndex(BindingArrayGetByIndexExpression expression, object accumulator)
        {
            if (accumulator == null) return null;

            if (accumulator.GetType().IsArray)
            {
                return ((dynamic)accumulator)[expression.Index];
            }
            else if (accumulator is IList)
            {
                return ((IList)accumulator)[expression.Index];
            }
            else if (accumulator is IEnumerable)
            {
                return ((IEnumerable)accumulator).OfType<object>().ElementAt(expression.Index);
            }
            else
            {
                throw new NotSupportedException(string.Format("Value of type {0} does not support array or collection indexer!", accumulator.GetType().ToString()));        // TODO: more precise error message
            }
        }

        protected override object VisitArrayGetByKey(BindingArrayGetByKeyExpression expression, object accumulator)
        {
            if (accumulator == null) return null;

            if (accumulator is IEnumerable)
            {
                return ((IEnumerable)accumulator).OfType<object>().FirstOrDefault(item =>
                {
                    var prop = item.GetType().GetProperty(expression.KeyPropertyName);
                    if (prop == null) return false;
                    var value = prop.GetValue(item);
                    return (value.ToString() == expression.KeyValue);
                });
            }
            else
            {
                throw new NotSupportedException(string.Format("Value of type {0} does not support lookup by key - it is not a collection!", accumulator.GetType().ToString()));        // TODO: more precise error message
            }
        }
    }
}
