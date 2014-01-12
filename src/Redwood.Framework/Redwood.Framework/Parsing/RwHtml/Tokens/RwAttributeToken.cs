using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Parsing.RwHtml.Tokens
{
    public class RwAttributeToken : RwHtmlToken
    {

        public string Name { get; set; }

        public RwHtmlToken Value { get; set; }

    }
}