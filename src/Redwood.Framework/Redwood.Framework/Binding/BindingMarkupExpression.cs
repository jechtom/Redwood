using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Redwood.Framework.Binding
{
    [DefaultProperty("Path")]
    public class BindingMarkupExpression : RwHtml.Markup.MarkupExpression
    {

        /// <summary>
        /// Gets or sets binding property path. Can be null.
        /// </summary>
        public Parsing.Expressions.BindingExpression Path { get; set; }

        /// <summary>
        /// Gets or sets the binding mode.
        /// </summary>
        public BindingFlags Mode { get; set; }

        /// <summary>
        /// Gets or sets whether the binding is evaluated on the server side.
        /// </summary>
        public bool EvaluateOnServer { get; set; }


        public object Evaluate(RedwoodBindable redwoodBindable)
        {
            throw new NotImplementedException();
        }

        public override object EvaluateMarkupExpression(RwHtml.Markup.MarkupExpressionEvaluationContext context)
        {
            throw new InvalidOperationException();
        }
    }

    [Flags]
    public enum BindingFlags : uint
    {
        OneTime = 0u,
        OneWay = 1u,
        OneWayToSource = 2u,
        TwoWay = OneWay | OneWayToSource
    }
}
