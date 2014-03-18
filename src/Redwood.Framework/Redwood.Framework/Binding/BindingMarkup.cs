using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Redwood.Framework.Binding.Parsing.Expressions;

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
        /// Initializes a new instance of the <see cref="BindingMarkup"/> class.
        /// </summary>
        public BindingMarkup()
        {
            Path = new BindingGetPropertyExpression();
        }

        public override object EvaluateMarkupExpression(RwHtml.Markup.MarkupExpressionEvaluationContext context)
        {
            return new BindingExpression(this);
        }

        public override string MarkupExtensionName
        {
            get { return "Binding"; }
        }
    }
}
