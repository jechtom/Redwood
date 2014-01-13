using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Generation;

namespace Redwood.Framework.RwHtml.Parsing.Tokens
{
    public class RwCloseTagToken : RwHtmlToken
    {

        public string TagName { get; set; }

        public bool IsSelfClosing { get; private set; }

        public RwCloseTagToken(string tagName, bool isSelfClosing)
        {
            TagName = tagName;
            IsSelfClosing = isSelfClosing;
        }
    }
}