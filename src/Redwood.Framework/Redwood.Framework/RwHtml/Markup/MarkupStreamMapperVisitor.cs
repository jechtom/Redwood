using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    /// <summary>
    /// Reads objects and members and validates it against CLR objects.
    /// </summary>
    public class MarkupStreamMapperVisitor : MarkupStreamVisitor
    {
        ControlTypeMapper typeMapper;

        public MarkupStreamMapperVisitor()
        {
            typeMapper = ControlTypeMapper.Default;
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

        private void OnBeginObjectFrame(MarkupFrame markupFrame)
        {
            var node = markupFrame.Node;
            node.Type.ClrType = typeMapper.GetType(node.Type.RwHtmlNamespace, node.Type.Name.SingleName());
            node.Type.ClrConstructor = node.Type.ClrType.GetConstructor(System.Type.EmptyTypes); // parameter-less constructor
        }

        private void OnBeginMemberFrame(MarkupFrame markupFrame)
        {
            var node = markupFrame.Node;
            var parentNode = markupFrame.ParentFrame.Node;
            parentNode.AssertType(MarkupNodeType.BeginObject);

            // find property accessor
            node.Member.PropertyAccessor = FindProperty(node, parentNode);
        }

        private Binding.IPropertyAccessor FindProperty(MarkupNode memberNode, MarkupNode objectNode)
        {
            var clrType = objectNode.Type.ClrType;

            // try find redwood property
            var redwoodPropInfo = Binding.RedwoodProperty.GetByName(memberNode.Member.Name.SingleName(), clrType);
            if (redwoodPropInfo != null)
            {
                return new Binding.RedwoodPropertyAccessor(redwoodPropInfo);
            }

            // try find CLR property
            if (!memberNode.Member.IsAttachedProperty)
            {
                var propName = memberNode.Member.Name.SingleName();
                var propInfo = clrType.GetProperty(propName);
                if (propInfo != null)
                    return new Binding.PropertyBasicAccessor(propInfo);
            }

            throw new InvalidOperationException(string.Format("Property \"{0}\" not found on \"{1}\".", propName, clrType.FullName));
        }
    }
}
