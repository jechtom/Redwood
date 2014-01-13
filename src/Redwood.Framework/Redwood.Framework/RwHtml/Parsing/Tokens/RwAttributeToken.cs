using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.RwHtml.Parsing.Tokens
{
    public class RwAttributeToken : RwHtmlToken
    {

        public string Name { get; set; }

        public RwHtmlToken Value { get; set; }


        public RwAttributeToken(string attributeName, RwHtmlToken attributeValue)
        {
            Name = attributeName;
            Value = attributeValue;
        }
    }
}