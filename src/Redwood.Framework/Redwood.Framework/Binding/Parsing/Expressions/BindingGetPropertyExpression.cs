using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Binding.Parsing.Expressions
{
    public class BindingGetPropertyExpression : BindingExpression
    {
        
        public string PropertyName { get; set; }

        public BindingExpression NextExpression { get; set; }

    }
}