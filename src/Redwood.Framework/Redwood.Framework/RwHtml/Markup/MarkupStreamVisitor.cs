using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    /// <summary>
    /// Provides methods for visiting markup stream and defining markup frame context.
    /// </summary>
    public abstract class MarkupStreamVisitor<TFrame> : MarkupStreamVisitorBase where TFrame : MarkupFrame, new()
    {
        Stack<TFrame> frameStack;
        TFrame current;

        protected override void Init()
        {
            base.Init();

            frameStack = new Stack<TFrame>();
            current = null;
        }


        protected virtual TFrame CreateNewFrame()
        {
            return new TFrame();
        }

        protected virtual void InitFrameNestedObject(TFrame frame, MarkupNode beginObjectMember)
        {
            beginObjectMember.AssertType(MarkupNodeType.BeginObject);
            frame.Init(CurrentFrame, MarkupFrameType.Object, beginObjectMember);
        }

        protected virtual void InitFrameRoot(TFrame frame, MarkupNode node)
        {
            node.AssertType(MarkupNodeType.BeginObject);
            frame.Init(null, MarkupFrameType.Object, node);
        }

        protected virtual void InitNestedMember(TFrame frame, MarkupNode beginMemberNode)
        {
            beginMemberNode.AssertType(MarkupNodeType.BeginMember);
            frame.Init(CurrentFrame, MarkupFrameType.Member, beginMemberNode);
        }

        protected override MarkupNode VisitBeginObjectNode(MarkupNode node)
        {
            // root object
            var frame = CreateNewFrame();
            if(frameStack.Count == 0)
            {
                InitFrameRoot(frame, node);
            }
            else // nested object as member value
            {
                InitFrameNestedObject(frame, node);
            }
            PushFrame(frame);

            return node;
        }

        protected override MarkupNode VisitBeginMemberNode(MarkupNode node)
        {
            var frame = CreateNewFrame();
            InitNestedMember(frame, node);
            PushFrame(frame);
            return node;
        }

        protected override MarkupNode VisitEndMemberNode(MarkupNode node)
        {
            // verify frame in stack
            if (current == null || !current.IsMemberFrame)
                throw new InvalidOperationException("Member ending node unmatched with frame stack.");

            // pop frame for member end
            PopFrame();
            
            return node;
        }

        protected override MarkupNode VisitEndObjectNode(MarkupNode node)
        {
            // verify frame in stack
            if (current == null || !current.IsObjectFrame)
                throw new InvalidOperationException("Object ending node unmatched with frame stack.");

            // pop frame for object end
            PopFrame();

            return node;
        }
   
        private void PushFrame(TFrame frame)
        {
            // push frame - ensure correct depth
            int expectedDepth = 0;
            if (frameStack.Count > 0)
                expectedDepth = frameStack.Peek().Depth + 1;

            if (expectedDepth != frame.Depth)
                throw new InvalidOperationException(string.Format("Cannot push frame with level {0}. Expected level is {1}.", frame.Depth, expectedDepth));

            // before pushing event
            OnFramePushing(frame);

            // push and set as current
            frameStack.Push(frame);
            current = frame;
        }

        private void PopFrame()
        {
            // pop and set next as current
            var poppedFrame = frameStack.Pop();
            current = frameStack.Count > 0 ? frameStack.Peek() : null;

            // after popped event
            OnFramePopped(poppedFrame);
        }

        protected virtual void OnFramePopped(TFrame markupFrame)
        {
        }

        protected virtual void OnFramePushing(TFrame markupFrame)
        {
        }

        protected TFrame CurrentFrame
        {
            get
            {
                return current;
            }
        }
    }
}
