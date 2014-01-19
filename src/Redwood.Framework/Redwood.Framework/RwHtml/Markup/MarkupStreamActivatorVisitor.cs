using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    /// <summary>
    /// Reads objects and members and activates CLR objects.
    /// </summary>
    public class MarkupStreamActivatorVisitor : MarkupStreamVisitor
    {
        ControlTypeActivator typeActivator;
        Stack<object> objectStack;
        object lastBuiltValue;

        public object Result { get; private set; }

        public MarkupStreamActivatorVisitor()
        {
            typeActivator = ControlTypeActivator.Default;
        }

        protected override void Init()
        {
            objectStack = new Stack<object>();
            lastBuiltValue = null;
            base.Init();
        }

        protected override void AfterProcessing()
        {
            base.AfterProcessing();

            if (objectStack.Count > 0)
                throw new InvalidOperationException("Object stack is not empty after processing.");

            Result = lastBuiltValue;
        }

        protected override void OnFramePushing(MarkupFrame markupFrame)
        {
            switch (markupFrame.FrameType)
            {
                case MarkupFrameType.Object:
                    OnBeginObjectFrame(markupFrame);
                    break;
                case MarkupFrameType.Member:
                    OnBeginMemberFrame(markupFrame);
                    break;
            }

            base.OnFramePushing(markupFrame);
        }

        protected override void OnFramePopped(MarkupFrame markupFrame)
        {
            switch (markupFrame.FrameType)
            {
                case MarkupFrameType.Object:
                    OnEndObjectFrame(markupFrame);
                    break;
                case MarkupFrameType.Member:
                    OnEndMemberFrame(markupFrame);
                    break;
            }

            base.OnFramePopped(markupFrame);
        }

        private void OnEndMemberFrame(MarkupFrame markupFrame)
        {
            var propAccessor = markupFrame.Node.Member.PropertyAccessor;
            if (propAccessor == null)
                throw new InvalidOperationException("Property accessor has not been resolved.");

            var targetObj = objectStack.Peek();
            propAccessor.SetValue(targetObj, lastBuiltValue);
        }

        private void OnBeginMemberFrame(MarkupFrame markupFrame)
        {
            //   
        }

        private void OnBeginObjectFrame(MarkupFrame markupFrame)
        {
            var node = markupFrame.Node;
            var clrType = node.Type.ClrType;

            if (clrType == null)
                throw new InvalidOperationException("CLR type has not been resolved.");
            
            var instance = typeActivator.Activate(clrType);
            objectStack.Push(instance);
        }

        private void OnEndObjectFrame(MarkupFrame markupFrame)
        {
            lastBuiltValue = objectStack.Pop();
        }

        protected override MarkupNode VisitValueNode(MarkupNode node)
        {
            string stringValue = node.Value.Value;
            var propAccessor = CurrentFrame.Node.Member.PropertyAccessor;
            if (propAccessor == null)
                throw new InvalidOperationException("Property accessor has not been resolved.");

            lastBuiltValue = Binding.DefaultModelBinder.ConvertValue(stringValue, propAccessor.Type);

            return base.VisitValueNode(node);
        }
    }
}
