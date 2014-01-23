using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    public class MarkupFrame
    {
        bool isInited = false;

        public void Init(MarkupFrame parent, MarkupFrameType type, MarkupNode node)
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

            isInited = true;
        }

        protected void EnsureInited()
        {
            if (!isInited)
                throw new InvalidOperationException("Invalid operation. MarkupFrame is not yet inited.");
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
