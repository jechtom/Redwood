using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    /// <summary>
    /// Base class for describing expressions in rwhtml markup.
    /// </summary>
    public abstract class MarkupExpression
    {
        public abstract object EvaluateMarkupExpression(MarkupExpressionEvaluationContext context);

        public abstract string MarkupExtensionName { get; }
    }
}
