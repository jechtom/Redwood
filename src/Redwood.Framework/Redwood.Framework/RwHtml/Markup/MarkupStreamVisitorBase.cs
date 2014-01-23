using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    /// <summary>
    /// Provides base methods for visiting markup stream and buffering output.
    /// </summary>
    public abstract class MarkupStreamVisitorBase
    {
        Queue<MarkupNode> buffer;
        Queue<MarkupNode> yeildReturnBuffer;
        bool useBuffer, releaseBuffer;
        
        public IEnumerable<MarkupNode> Process(IEnumerable<MarkupNode> input)
        {
            Init();

            foreach (var node in input)
            {
                var visitedNode = VisitNode(node);

                // release buffered nodes from previous iterations if required
                if(releaseBuffer)
                {
                    releaseBuffer = false;
                    while (buffer.Count > 0)
                        yield return buffer.Dequeue();
                }

                // use buffer or return node directly
                if (useBuffer)
                {
                    // add current item to the buffer
                    if(visitedNode != null)
                        buffer.Enqueue(visitedNode);

                    // buffer all other items added to return
                    if (yeildReturnBuffer != null)
                        while (yeildReturnBuffer.Count > 0)
                            buffer.Enqueue(yeildReturnBuffer.Dequeue());
                }
                else
                {
                    // return current item 
                    if(visitedNode != null)
                        yield return visitedNode;

                    // return all other items added to return
                    if (yeildReturnBuffer != null)
                        while (yeildReturnBuffer.Count > 0)
                            yield return yeildReturnBuffer.Dequeue();
                }
            }

            // release rest of buffer
            while (buffer.Count > 0)
                yield return buffer.Dequeue();

            AfterProcessing();
        }

        
        /// <summary>
        /// Adds another node to be returned to consumer after current item.
        /// </summary>
        /// <param name="node"></param>
        protected void YieldReturnAfterCurrentNode(MarkupNode node)
        {
            if (yeildReturnBuffer == null)
                yeildReturnBuffer = new Queue<MarkupNode>();
            
            yeildReturnBuffer.Enqueue(node);
        }

        /// <summary>
        /// All nodes in stream (including current node) will be buffered and not released to output node stream.
        /// </summary>
        protected void StartBufferingOutput()
        {
            useBuffer = true;
        }

        /// <summary>
        /// All items in buffer will be released and buffering will stop.
        /// </summary>
        protected void ReleaseBuffer()
        {
            useBuffer = false;
            releaseBuffer = true;
        }

        protected virtual void Init()
        {
            buffer = new Queue<MarkupNode>();
            yeildReturnBuffer = null;
            useBuffer = false;
            releaseBuffer = false;
        }

        protected virtual MarkupNode VisitNode(MarkupNode node)
        {
            switch (node.NodeType)
            {
                case MarkupNodeType.Value:
                    return VisitValueNode(node);
                case MarkupNodeType.BeginObject:
                    return VisitBeginObjectNode(node);
                case MarkupNodeType.EndObject:
                    return VisitEndObjectNode(node);
                case MarkupNodeType.BeginMember:
                    return VisitBeginMemberNode(node);
                case MarkupNodeType.EndMember:
                    return VisitEndMemberNode(node);
                case MarkupNodeType.NamespaceDeclaration:
                    return VisitNamespaceDeclarationNode(node);
                case MarkupNodeType.EndOfDocument:
                    return VisitEndOfDocumentNode(node);
                default:
                    return VisitDefault(node);
            }
        }

        protected virtual MarkupNode VisitNamespaceDeclarationNode(MarkupNode node)
        {
            return node;
        }

        protected virtual MarkupNode VisitEndOfDocumentNode(MarkupNode node)
        {
            return node;
        }

        protected virtual MarkupNode VisitValueNode(MarkupNode node)
        {
            return node;
        }

        protected virtual MarkupNode VisitBeginObjectNode(MarkupNode node)
        {
            return node;
        }

        protected virtual MarkupNode VisitBeginMemberNode(MarkupNode node)
        {
            return node;
        }

        protected virtual MarkupNode VisitEndMemberNode(MarkupNode node)
        {
            return node;
        }

        protected virtual MarkupNode VisitEndObjectNode(MarkupNode node)
        {
            return node;
        }

        protected virtual MarkupNode VisitDefault(MarkupNode node)
        {
            return node;
        }

        protected virtual void AfterProcessing()
        {
        }
    }
}
