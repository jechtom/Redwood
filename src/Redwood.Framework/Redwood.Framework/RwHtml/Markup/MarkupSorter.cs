using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    /// <summary>
    /// Sorts markup nodes to correct logical order.
    /// </summary>
    /// <remarks>
    /// Namespace declarations are moved before StartObject s StartMember nodes.
    /// </remarks>
    public class MarkupSorter
    {
        Stack<MarkupNode> nodeStack;
        Queue<MarkupNode> buffer;

        public MarkupSorter()
        {
        }

        private void Init()
        {
            nodeStack = new Stack<MarkupNode>();
            buffer = new Queue<MarkupNode>();
        }

        public IEnumerable<MarkupNode> Sort(IEnumerable<MarkupNode> source)
        {
            Init();

            bool holdQueue = false;

            foreach (var item in source)
            {
                // update stack
                while (nodeStack.Count > 0 && nodeStack.Peek().Level >= item.Level)
                    nodeStack.Pop();
                
                if(item.CanContainNestedNodes)
                    nodeStack.Push(item);


                throw new NotImplementedException("Not finished.");

                if (item.NodeType == MarkupNodeType.NamespaceDeclaration)
                    yield return item; // direct output
                else
                    buffer.Enqueue(item); // buffered output
            }

            // dequeue all other items from buffer if any
            while (buffer.Count > 0)
            {
                yield return buffer.Dequeue();
            }
        }
    }
}
