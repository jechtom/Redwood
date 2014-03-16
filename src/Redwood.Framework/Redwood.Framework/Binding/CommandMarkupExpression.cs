using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Redwood.Framework.Binding
{
    [DefaultProperty("Path")]
    public class CommandMarkupExpression : RwHtml.Markup.MarkupExpression
    {

        /// <summary>
        /// Gets or sets command function path. Can be null.
        /// </summary>
        public Parsing.Expressions.BindingPathExpression Path { get; set; }


        public object Evaluate(RedwoodBindable redwoodBindable)
        {
            // TODO: implementat
            return null;
        }

        public override object EvaluateMarkupExpression(RwHtml.Markup.MarkupExpressionEvaluationContext context)
        {
            // TODO: implementat
            return null;
        }
    }
}