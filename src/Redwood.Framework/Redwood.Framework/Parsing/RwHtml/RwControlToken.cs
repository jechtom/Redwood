using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Controls;
using Redwood.Framework.Generation;

namespace Redwood.Framework.Parsing.RwHtml
{
    public class RwControlToken : RwHtmlToken
    {
        public string TagPrefix { get; private set; }

        public string TagName { get; private set; }

        public AttributeList<string> Attributes { get; private set; }

        
        public RwControlToken(string tagPrefix, string tagName)
        {
            TagPrefix = tagPrefix;
            TagName = tagName;
            Attributes = new AttributeList<string>();
        }
    }
}