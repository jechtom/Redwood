using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    public class MarkupType
    {
        public string RwHtmlNamespace { get; set; }
        public NameWithPrefix Name { get; set; }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
