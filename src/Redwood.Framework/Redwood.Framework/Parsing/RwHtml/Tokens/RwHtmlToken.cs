using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Parsing.RwHtml.Tokens
{
    public abstract class RwHtmlToken : IToken
    {

        public SpanPosition SpanPosition { get; set; } 

    }
}