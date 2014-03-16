using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwood.Framework.Binding.Parsing.Expressions
{
    public class BindingArrayGetByIndexExpression : BindingExpression
    {

        public int Index { get; set; }


        public bool IsPlaceholder { get; set; }
    }
}
