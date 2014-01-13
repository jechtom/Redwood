using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.RwHtml.Parsing.Tokens
{
    public abstract class RwHtmlToken : IToken
    {

        public SpanPosition SpanPosition { get; set; } 

    }
}