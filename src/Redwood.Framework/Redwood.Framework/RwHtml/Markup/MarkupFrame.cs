using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    public class MarkupFrame
    {
        private MarkupFrame(MarkupFrame parent, MarkupFrameType type, MarkupNode node)
        {
            if (parent == null)
            {
                Depth = 0;
            }
            else
            {
                ParentFrame = parent;
                Depth = parent.Depth + 1;
            }

            FrameType = type;
            Node = node;
        }

        public static MarkupFrame CreateRoot(MarkupNode beginObjectNode)
        {
            beginObjectNode.AssertType(MarkupNodeType.BeginObject);
            return new MarkupFrame(null, MarkupFrameType.Object, beginObjectNode);
        }

        public MarkupFrame CreateNestedBeginObject(MarkupNode beginObjectNode)
        {
            beginObjectNode.AssertType(MarkupNodeType.BeginObject);

            if (!IsMemberFrame)
                throw new InvalidOperationException("Object frame is valid only in member frame.");

            return new MarkupFrame(this, MarkupFrameType.Object, beginObjectNode);
        }

        public MarkupFrame CreateNestedBeginMember(MarkupNode beginMemberNode)
        {
            beginMemberNode.AssertType(MarkupNodeType.BeginMember);

            if (!IsObjectFrame)
                throw new InvalidOperationException("Member frame is valid only in object frame.");

            return new MarkupFrame(this, MarkupFrameType.Member, beginMemberNode);
        }

        public int Depth { get; private set; }

        public MarkupFrame ParentFrame { get; private set; }
        public MarkupNode Node { get; private set; }
        
        public bool IsMemberFrame { get { return FrameType == MarkupFrameType.Member; } }
        public bool IsObjectFrame { get { return FrameType == MarkupFrameType.Object; } }

        public MarkupFrameType FrameType
        {
            get;
            private set;
        }
    }
}
