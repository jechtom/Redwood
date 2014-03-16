using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Binding.Parsing.Expressions
{
    public class BindingGetPropertyExpression : BindingPathExpression
    {
        
        public string PropertyName { get; set; }

        public BindingPathExpression NextExpression { get; set; }

        public BindingPathExpression Indexer { get; set; }
    }
}