using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Parsing;

namespace Redwood.Framework.Binding.Parsing.Tokens
{
    public abstract class BindingToken : IToken
    {
        public SpanPosition SpanPosition { get; set; }
    }
}