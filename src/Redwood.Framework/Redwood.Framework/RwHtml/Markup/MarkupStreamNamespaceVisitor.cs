using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    /// <summary>
    /// Reads namespace declarations and maps it to the markup nodes.
    /// </summary>
    public class MarkupStreamNamespaceVisitor : MarkupStreamVisitor<MarkupFrame>
    {
        RwHtmlNamespaceScope namespaceScope;
        Markup.NamespaceDeclaration[] defaultNamespaces;
        Queue<MarkupNode> attachedPropertiesToResolve;

        public MarkupStreamNamespaceVisitor(Markup.NamespaceDeclaration[] defaultNamespaces)
        {
            if (defaultNamespaces == null)
                throw new ArgumentNullException("defaultNamespaces");

            this.defaultNamespaces = defaultNamespaces;
        }

        protected override void Init()
        {
            attachedPropertiesToResolve = new Queue<MarkupNode>();
            namespaceScope = new RwHtmlNamespaceScope();
            foreach (var item in defaultNamespaces) // register default namespaces
                namespaceScope.AddNamespace(item.Prefix, item.RwHtmlNamespace);

            base.Init();
        }

        protected override void AfterProcessing()
        {
            namespaceScope.EnsureScopeLevelIsZero();
            base.AfterProcessing();
        }

        protected override MarkupNode VisitNamespaceDeclarationNode(MarkupNode node)
        {
            // assign namespace declaration
            namespaceScope.AddNamespace(node.Namespace.Prefix, node.Namespace.RwHtmlNamespace);

            return node;
        }

        protected override void OnFramePushing(MarkupFrame markupFrame)
        {
            if (markupFrame.Node.CanContainNamespaceDeclaration)
            {
                // new element = no more namespace declarations for current markup frame
                ApplyNamespaceByPrefixesAndReleaseBuffer();

                namespaceScope.PushScope();

                // don't return this node until all namespace declarations are read and applied
                StartBufferingOutput();
                attachedPropertiesToResolve.Enqueue(markupFrame.Node);
            }

            base.OnFramePushing(markupFrame);
        }

        protected override void OnFramePopped(MarkupFrame markupFrame)
        {
            if (markupFrame.Node.CanContainNamespaceDeclaration)
            {
                namespaceScope.PopScope();

                // element ends = no more namespace declarations
                ApplyNamespaceByPrefixesAndReleaseBuffer();
            }
            
            base.OnFramePopped(markupFrame);
        }

        protected override MarkupNode VisitBeginMemberNode(MarkupNode node)
        {
            // remember all inline attached properties to resolve their namespace too
            if (node.Member.IsInlineDefinition && node.Member.IsAttachedProperty)
                attachedPropertiesToResolve.Enqueue(node);

            return base.VisitBeginMemberNode(node);
        }

        private void ApplyNamespaceByPrefixesAndReleaseBuffer()
        {
            // resolve rwhtml namespace for object types and attached properties
            while (attachedPropertiesToResolve.Count > 0)
            {
                var node = attachedPropertiesToResolve.Dequeue();
                switch (node.NodeType)
                {
                    case MarkupNodeType.BeginObject:
                        node.Type.RwHtmlNamespace = namespaceScope.GetNamespaceByPrefix(node.Type.Name.Prefix);
                        break;
                    case MarkupNodeType.BeginMember:
                        // apply namespace only to attached properties, regular members refers to parent object and don't have namespace definition
                        if (node.Member.IsAttachedProperty)
                            node.Member.RwHtmlNamespace = namespaceScope.GetNamespaceByPrefix(node.Member.Name.Prefix);
                        break;
                }
            }

            // namespaces resolved, release buffered output
            ReleaseBuffer();
        }
    }
}
