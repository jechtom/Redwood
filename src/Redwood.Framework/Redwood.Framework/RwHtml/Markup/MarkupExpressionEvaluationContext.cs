using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwood.Framework.RwHtml.Markup
{
    public class MarkupExpressionEvaluationContext
    {
        public object TargetInstane { get; set; }

        public IPropertyAccessor TargetProperty { get; set; }
    }
}
