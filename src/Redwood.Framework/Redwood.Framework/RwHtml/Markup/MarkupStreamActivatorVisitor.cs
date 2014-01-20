using System;
using System.Collections;
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
        Stack<ValueContext> valuesStack;
        Stack<object> objectsStack;
        Converters.TypeConverterMapper converterMapper;
        
        public object Result { get; private set; }

        public MarkupStreamActivatorVisitor()
        {
            typeActivator = ControlTypeActivator.Default;
            converterMapper = Converters.TypeConverterMapper.Default;
        }

        protected override void Init()
        {
            valuesStack = new Stack<ValueContext>();
            valuesStack.Push(new ValueContext()); // result object
            objectsStack = new Stack<object>();
            base.Init();
        }

        protected override void AfterProcessing()
        {
            base.AfterProcessing();

            var result = valuesStack.Pop(); // pop root object

            if (valuesStack.Count > 0)
                throw new InvalidOperationException("Context stack is not empty after processing.");

            Result = result.Value;
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

        private void OnEndObjectFrame(MarkupFrame markupFrame)
        {
            // pop activated object
            objectsStack.Pop();
        }

        private void OnEndMemberFrame(MarkupFrame markupFrame)
        {
            var propAccessor = markupFrame.Node.Member.PropertyAccessor;
            if (propAccessor == null)
                throw new InvalidOperationException("Property accessor has not been resolved.");

            var value = valuesStack.Pop();
            var parent = objectsStack.Peek();
            propAccessor.SetValue(parent, value.Value);
        }

        private void OnBeginMemberFrame(MarkupFrame markupFrame)
        {
            valuesStack.Push(new ValueContext());
        }

        private void OnBeginObjectFrame(MarkupFrame markupFrame)
        {
            var node = markupFrame.Node;
            var clrType = node.Type.ClrType;

            if (clrType == null)
                throw new InvalidOperationException("CLR type has not been resolved.");
            
            var instance = typeActivator.Activate(clrType);
            objectsStack.Push(instance);
            BuildValue(instance);
        }

        protected override MarkupNode VisitValueNode(MarkupNode node)
        {
            string stringValue = node.Value.Value;
            var propAccessor = CurrentFrame.Node.Member.PropertyAccessor;
            if (propAccessor == null)
                throw new InvalidOperationException("Property accessor has not been resolved.");

            var converter = converterMapper.GetConverterForType(propAccessor.Type);
            object resultValue;
            if(!converter.TryConvertFromString(stringValue, out resultValue))
                throw new InvalidOperationException("Can't convert value to " + propAccessor.Type.FullName);

            BuildValue(resultValue);
            
            return base.VisitValueNode(node);
        }

        private void BuildValue(object value)
        {
            var context = valuesStack.Peek();

            context.AddValue(value);
        }

        
        private class ValueContext
        {
            public bool IsCollection { get; set; }
            public object Value { get; set; }

            public object LastValue { get; set; }

            public void AddValue(object value)
            {
                if (IsCollection)
                {
                    // another value
                    ((IList)Value).Add(value);
                }
                else if (Value != null)
                {
                    // second value - create list
                    var firstValue = Value;
                    var list = new List<object>();
                    list.Add(firstValue);
                    list.Add(value);
                    Value = list;
                }
                else
                {
                    // first value
                    Value = value;
                }

                // set last value
                LastValue = value;
            }
        }
    }
}
