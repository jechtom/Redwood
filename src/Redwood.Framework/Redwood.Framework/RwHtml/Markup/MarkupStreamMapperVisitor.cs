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
        PropertyMapper propertyMapper;

        public MarkupStreamMapperVisitor()
        {
            typeMapper = ControlTypeMapper.Default;
            propertyMapper = PropertyMapper.Default;
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
            node.Type.ClrType = ResolveTypeForObjectNode(node);
        }

        private Type ResolveTypeForObjectNode(MarkupNode node)
        {
            string name = node.Type.Name.SingleName();
            string rwhtmlNamespace = node.Type.RwHtmlNamespace;
            
            Type result = typeMapper.GetType(rwhtmlNamespace, name);
            
            if (result == null)
                throw new InvalidOperationException(string.Format("Type \"{0}\" not found in rwhtml namespace \"{1}\".", name, rwhtmlNamespace)); // not found

            return result;
        }

        private Type ResolveTypeForAttachedProperty(MarkupNode memberNode, out string attachedPropertyName)
        {
            if(memberNode.Member.Name.Names.Length != 2) // <rw:TextBox OwnerName.PropertyName - only 2 names - name of type and name of property.
                throw new Parsing.RwHtmlParsingException("Attached properties is allowed only with 2 names.", memberNode.CurrentPosition);
            
            string name = memberNode.Member.Name.Names[0]; // first name is name of type (rw:ObjectName.PropertyName)
            attachedPropertyName = memberNode.Member.Name.Names[1];
            string rwhtmlNamespace = memberNode.Member.RwHtmlNamespace;

            Type result = typeMapper.GetType(rwhtmlNamespace, name);

            if (result == null)
                throw new InvalidOperationException(string.Format("Type \"{0}\" not found in rwhtml namespace \"{1}\".", name, rwhtmlNamespace)); // not found

            return result;
        }

        private void OnBeginMemberFrame(MarkupFrame markupFrame)
        {
            var node = markupFrame.Node;
            var parentNode = markupFrame.ParentFrame.Node;
            parentNode.AssertType(MarkupNodeType.BeginObject);

            
            if (node.Member.IsAttachedProperty)
            {
                // resolve CLR type based on attached property and resolve attached property reference
                string attachedPropertyName;
                node.Member.AttachedPropertyOwnerType = ResolveTypeForAttachedProperty(node, out attachedPropertyName);
                node.Member.PropertyAccessor = propertyMapper.GetPropertyOrThrowError(node.Member.AttachedPropertyOwnerType, attachedPropertyName, false);
            }
            else
            {
                // find property accessor
                var clrType = parentNode.Type.ClrType;
                string name = node.Member.Name.SingleName();
                node.Member.PropertyAccessor = propertyMapper.GetPropertyOrThrowError(clrType, name, false);
            }
        }
    }
}
