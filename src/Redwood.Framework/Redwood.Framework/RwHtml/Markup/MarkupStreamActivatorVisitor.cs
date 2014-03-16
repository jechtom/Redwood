using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redwood.Framework.Parsing;

namespace Redwood.Framework.RwHtml.Markup
{
    /// <summary>
    /// Reads objects and members and activates CLR objects.
    /// </summary>
    public class MarkupStreamActivatorVisitor : MarkupStreamVisitor<MarkupStreamActivatorFrame>
    {   
        ControlTypeActivator typeActivator;
        Converters.TypeConverterMapper converterMapper;
        MarkupStreamActivatorFrame lastFrame;
        Binding.Parsing.BindingParser bindingParser;
        
        public object ProcessToResult(IEnumerable<MarkupNode> input)
        {
            foreach (var item in Process(input))
            {
            }

            return this.Result;
        }

        public object Result { get; private set; }

        public MarkupStreamActivatorVisitor()
        {
            typeActivator = ControlTypeActivator.Default;
            converterMapper = Converters.TypeConverterMapper.Default;
            bindingParser = new Binding.Parsing.BindingParser();
        }

        protected override void Init()
        {
            base.Init();
            lastFrame = null;
            Result = null;
        }

        protected override void AfterProcessing()
        {
            base.AfterProcessing();
            Result = lastFrame.Value;
        }

        protected override void OnFramePushing(MarkupStreamActivatorFrame markupFrame)
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

        protected override void OnFramePopped(MarkupStreamActivatorFrame markupFrame)
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
            lastFrame = markupFrame; // set last frame
        }

        private void OnEndObjectFrame(MarkupStreamActivatorFrame markupFrame)
        {
        }

        private void OnEndMemberFrame(MarkupStreamActivatorFrame markupFrame)
        {
            var propAccessor = markupFrame.Node.Member.PropertyAccessor;
            if (propAccessor == null)
                throw new InvalidOperationException("Property accessor has not been resolved.");

            // member is ending - set member value
            var value = markupFrame.Value;
            var parent = ((MarkupStreamActivatorFrame)markupFrame.ParentFrame).Value;
            propAccessor.SetValue(parent, value);
        }

        private void OnBeginMemberFrame(MarkupStreamActivatorFrame frame)
        {
            //
        }

        private void OnBeginObjectFrame(MarkupStreamActivatorFrame frame)
        {
            var node = frame.Node;
            var clrType = node.Type.ClrType;

            if (clrType == null)
                throw new InvalidOperationException("CLR type has not been resolved.");
            
            var instance = typeActivator.Activate(clrType);
            frame.Value = instance; // set current object instance

            if (frame.Depth > 0)
            {
                var parentFrame = ((MarkupStreamActivatorFrame)frame.ParentFrame);
                AddValueToMember(parentFrame, instance); // add instance to parent member definition
            }
        }

        protected override MarkupNode VisitValueNode(MarkupNode node)
        {
            // current frame have to be Member
            if (CurrentFrame.FrameType != MarkupFrameType.Member)
                throw new Parsing.RwHtmlParsingException("Member frame expected for value node.", node.CurrentPosition);

            // get property accessor for this member
            string stringValue = node.Value.Value;
            var propAccessor = CurrentFrame.Node.Member.PropertyAccessor;
            if (propAccessor == null)
                throw new InvalidOperationException("Property accessor has not been resolved.");

            // expression?
            object resultValue;
            if (node.Value.IsExpression)
            {
                // parse expression and evaluate
                MarkupExpression expr;
                try
                {
                    expr = bindingParser.ParseExpression(node.Value.Value);
                }
                catch (ParserException ex)
                {
                    ex.Position = ex.Position.AddTo(node.CurrentPosition);
                    throw;
                }

                var exprContext = new MarkupExpressionEvaluationContext()
                {
                    TargetProperty = propAccessor
                };
                resultValue = expr.EvaluateMarkupExpression(exprContext);
            }
            else
            {
                // convert value
                var converter = converterMapper.GetConverterForType(propAccessor.Type);
                if (!converter.TryConvertFromString(stringValue, out resultValue))
                {
                    throw new InvalidOperationException("Can't convert value to " + propAccessor.Type.FullName);
                }
            }

            // convert to raw html (don't encode)
            if (propAccessor.Type == typeof (object) && resultValue is string)
            {
                resultValue = Controls.HtmlContent.Create((string)resultValue);
            }

            // set value
            AddValueToMember(CurrentFrame, resultValue);

            return base.VisitValueNode(node);
        }

        private void AddValueToMember(MarkupStreamActivatorFrame memberFrame, object value)
        {
            if (memberFrame.IsCollection)
            {
                // another value
                ((IList)memberFrame.Value).Add(value);
            }
            else if (memberFrame.Value != null)
            {
                // second value - create list
                var firstValue = memberFrame.Value;
                var list = new List<object>();
                list.Add(firstValue);
                list.Add(value);
                memberFrame.Value = list;
                memberFrame.IsCollection = true;
            }
            else
            {
                // first value
                memberFrame.Value = value;
            }
        }
    }
}
