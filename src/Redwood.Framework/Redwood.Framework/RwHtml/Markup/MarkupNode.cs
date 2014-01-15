using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    public class MarkupNode
    {
        public MarkupNodeType NodeType { get; set; }
        public SpanPosition CurrentPosition { get; set; }
        public RwHtmlNamespaceDeclaration Namespace { get; set; }
        public MarkupType Type { get; set; }
        public MarkupValue Value { get; set; }
        public MarkupMember Member { get; set; }
        public int Level { get; set; }

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
    }
}
