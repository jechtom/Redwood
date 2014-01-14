using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwood.Framework.RwHtml.Markup
{
    public class MarkupElement
    {
        public MarkupElement(NameWithPrefix name)
        {
            this.Name = name;
            this.Attributes = new MarkupAttributes();
        }

        public NameWithPrefix Name { get; private set; }

        public MarkupAttributes Attributes { get; private set; }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append("<");
            result.Append(Name);
            foreach (var item in Attributes)
            {
                result.Append(" ");
                result.Append(item.Key);
                result.Append("=\"");
                result.Append(item.Value);
                result.Append("\"");
            }
            result.Append(">");
            return result.ToString();
        }
    }
}
