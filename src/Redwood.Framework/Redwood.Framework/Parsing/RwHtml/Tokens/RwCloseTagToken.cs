using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Generation;

namespace Redwood.Framework.Parsing.RwHtml.Tokens
{
    public class RwCloseTagToken : RwHtmlToken
    {
        public bool IsSelfClosing { get; private set; }

        public RwCloseTagToken(bool isSelfClosing)
        {
            IsSelfClosing = isSelfClosing;
        }
    }
}