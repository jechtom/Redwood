using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.RwHtml.Parsing.Tokens
{
    public class RwOpenTagEndToken : RwHtmlToken
    {
        public bool IsSelfClosing { get; private set; }

        public RwOpenTagEndToken(string tagName, bool isSelfClosing)
        {
            IsSelfClosing = isSelfClosing;
        }
    }
}