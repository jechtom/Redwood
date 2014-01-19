using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Parsing;

namespace Redwood.Framework.RwHtml.Parsing.Tokens
{
    public abstract class RwHtmlToken : IToken
    {

        public SpanPosition SpanPosition { get; set; } 

    }
}