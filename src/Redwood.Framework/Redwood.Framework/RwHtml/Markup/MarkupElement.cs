using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwood.Framework.RwHtml.Markup
{
    public class MarkupElement
    {
        public MarkupElement(string name)
        {
            this.Name = name;
            this.Attributes = new MarkupAttributes();
        }

        public string Name { get; private set; }

        public MarkupAttributes Attributes { get; private set; }
    }
}
