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
                StringBuilder msg = new StringBuilder(new string('|', item.Level));

                msg.Append("+ " + item.NodeType + " ");

                if (item.Value != null)
                    msg.Append("- value: " + item.Value);

                if (item.Member != null)
                    msg.Append("- member: " + item.Member.ToString());

                if (item.Type != null)
                    msg.Append("- type: " + item.Type.ToString());

                if (item.Namespace != null)
                    msg.Append("- namespace: " + item.Namespace.Prefix + ":" + item.Namespace.RwHtmlNamespace);

                Debug.WriteLine(msg.ToString());
            }
        }
    }
}
