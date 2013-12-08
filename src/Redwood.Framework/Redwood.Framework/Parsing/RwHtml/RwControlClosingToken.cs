using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Generation;

namespace Redwood.Framework.Parsing.RwHtml
{
    public class RwControlClosingToken : RwHtmlToken
    {

        public string TagPrefix { get; private set; }

        public string TagName { get; private set; }

        public RwControlClosingToken(string tagPrefix, string tagName)
        {
            TagPrefix = tagPrefix;
            TagName = tagName;
        }
    }
}