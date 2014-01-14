using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.RwHtml.Parsing.Tokens
{
    public class RwOpenTagBeginToken : RwHtmlToken
    {

        public string TagName { get; private set; }

        public TagType TagType { get; private set; }


        public RwOpenTagBeginToken(string tagName, TagType tagType)
        {
            TagName = tagName;
            TagType = tagType;
        }
    }

    public enum TagType
    {
        StandardTag,
        XmlProcessingInstruction,
        DoctypeDeclaration
    }
}