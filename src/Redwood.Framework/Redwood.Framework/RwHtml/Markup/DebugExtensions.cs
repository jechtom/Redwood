using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    public static class DebugExtensions
    {
        public static void DebugOutput(this IEnumerable<MarkupNode> nodes)
        {
            foreach (var item in nodes)
            {
                var msg = item.ToDebugString();
                Debug.WriteLine(msg);
            }
        }

        public static string ToDebugString(this MarkupNode node)
        {
            StringBuilder msg = new StringBuilder(new string('|', node.Level));

            msg.Append("+ " + node.NodeType + " ");

            if (node.Value != null)
                msg.Append("- value: " + node.Value);

            if (node.Member != null)
                msg.Append("- member: " + node.Member.ToString());

            if (node.Type != null)
                msg.Append("- type: " + node.Type.ToString());

            if (node.Namespace != null)
                msg.Append("- namespace: " + node.Namespace.Prefix + ":" + node.Namespace.RwHtmlNamespace);

            return msg.ToString();
        }
    }
}
