using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Redwood.Framework.Binding
{
    [DefaultProperty("Path")]
    public class BindingMarkup : RwHtml.Markup.MarkupExpression
    {

        /// <summary>
        /// Gets or sets binding property path. Can be null.
        /// </summary>
        public Parsing.Expressions.BindingPathExpression Path { get; set; }

        /// <summary>
        /// Gets or sets the binding mode.
        /// </summary>
        public BindingMode Mode { get; set; }

        /// <summary>
        /// Gets or sets whether the binding is evaluated on the server side.
        /// </summary>
        public bool EvaluateOnServer { get; set; }

        public override object EvaluateMarkupExpression(RwHtml.Markup.MarkupExpressionEvaluationContext context)
        {
            return new BindingExpression(this);
        }
    }
}
