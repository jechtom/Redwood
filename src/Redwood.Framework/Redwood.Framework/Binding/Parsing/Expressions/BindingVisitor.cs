using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Binding.Parsing.Expressions
{
    public abstract class BindingVisitor<TAccumulator>
    {

        public TAccumulator Visit(BindingExpression expression, TAccumulator accumulator)
        {
            if (expression is BindingConstantExpression)
            {
                return VisitConstant((BindingConstantExpression)expression, accumulator);
            }
            else if (expression is BindingGetPropertyExpression)
            {
                return VisitGetProperty((BindingGetPropertyExpression)expression, accumulator);
            }
            else if (expression is BindingCallMethodExpression)
            {
                return VisitCallMethod((BindingCallMethodExpression)expression, accumulator);
            }
            else if (expression is BindingArrayGetByIndexExpression)
            {
                return VisitArrayGetByIndex((BindingArrayGetByIndexExpression)expression, accumulator);
            }
            else if (expression is BindingArrayGetByKeyExpression)
            {
                return VisitArrayGetByKey((BindingArrayGetByKeyExpression)expression, accumulator);
            }
            else
            {
                throw new NotSupportedException(string.Format("Binding expression of type {0} is not supported!", expression.GetType()));
            }
        }

        protected abstract TAccumulator VisitConstant(BindingConstantExpression expression, TAccumulator accumulator);

        protected abstract TAccumulator VisitGetProperty(BindingGetPropertyExpression expression, TAccumulator accumulator);

        protected abstract TAccumulator VisitCallMethod(BindingCallMethodExpression expression, TAccumulator accumulator);

        protected abstract TAccumulator VisitArrayGetByIndex(BindingArrayGetByIndexExpression expression, TAccumulator accumulator);

        protected abstract TAccumulator VisitArrayGetByKey(BindingArrayGetByKeyExpression expression, TAccumulator accumulator);
    }
}