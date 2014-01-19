using Redwood.Framework.Binding;
using Redwood.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    /// <summary>
    /// Reads HTML elements and maps it to CLR controls.
    /// </summary>
    public class MarkupStreamHtmlElementVisitor : MarkupStreamVisitor
    {
        const string HtmlElementNamespace = Redwood.Framework.Controls.RedwoodControl.DefaultSchemaNamespaceName;
        readonly static Type HtmlElementType = typeof(HtmlElement);
        readonly static NameWithPrefix HtmlElementTypeName = new NameWithPrefix(HtmlElementType.Name);
        readonly static NameWithPrefix HtmlElementElementPropName = new NameWithPrefix(HtmlElement.ElementProperty.Name);
        
        public MarkupStreamHtmlElementVisitor()
        {
        }

        protected override MarkupNode VisitBeginObjectNode(MarkupNode node)
        {
            if(!IsHtmlBeginObjectNode(node))
                return base.VisitBeginObjectNode(node);

            string htmlElementName = node.Type.Name.ToString();

            // replace html element with control
            node.Type = new MarkupType()
            {
                Name = HtmlElementTypeName,
                RwHtmlNamespace = HtmlElementNamespace,
                ClrType = HtmlElementType
            };

            // set property "Element"
            YieldReturnAfterCurrentNode(new MarkupNode()
            {
                Level = node.Level + 1,
                NodeType = MarkupNodeType.BeginMember,
                Member = new MarkupMember()
                {
                    Name = HtmlElementElementPropName
                }
            });

            // set value
            YieldReturnAfterCurrentNode(new MarkupNode()
            {
                Level = node.Level + 2,
                NodeType = MarkupNodeType.Value,
                Value = new MarkupValue(htmlElementName, false)
            });

            // end of property "Element"
            YieldReturnAfterCurrentNode(new MarkupNode()
            {
                Level = node.Level + 1,
                NodeType = MarkupNodeType.EndMember
            });

            return base.VisitBeginObjectNode(node);
        }

        private bool IsHtmlBeginObjectNode(MarkupNode node)
        {
            // namespace not resolved
            return node.Type.RwHtmlNamespace == null;
        }
    }
}
