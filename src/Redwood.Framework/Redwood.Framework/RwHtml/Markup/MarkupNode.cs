using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Parsing;

namespace Redwood.Framework.RwHtml.Markup
{
    public class MarkupNode
    {
        public MarkupNodeType NodeType { get; set; }
        public SpanPosition CurrentPosition { get; set; }
        public NamespaceDeclaration Namespace { get; set; }
        public MarkupType Type { get; set; }
        public MarkupValue Value { get; set; }
        public MarkupMember Member { get; set; }
        public int Level { get; set; }

        /// <summary>
        /// Gets if this node can contain nested nodes.
        /// </summary>
        public bool CanContainNestedNodes
        {
            get
            {
                switch (NodeType)
                {
                    case MarkupNodeType.BeginObject:
                    case MarkupNodeType.BeginMember:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Gets if this node can contain namespace declarations.
        /// </summary>
        public bool CanContainNamespaceDeclaration
        {
            get
            {
                // begin object is always new element in RwHtml ("<rw:TextBox>..."
                if (NodeType == MarkupNodeType.BeginObject)
                    return true;

                // non-inline property are elements in RwHtml ("<rw:TextBox><rw:TextBox.Content>..." etc.)
                if (NodeType == MarkupNodeType.BeginMember && !Member.IsInlineDefinition)
                    return true;

                return false;
            }
        }

        public void AssertType(MarkupNodeType markupNodeType)
        {
            if (NodeType != markupNodeType)
                throw new InvalidOperationException(string.Format("Invalid markup node type. Expected: {0}, current: {1}", markupNodeType, NodeType));
        }

    }
}
