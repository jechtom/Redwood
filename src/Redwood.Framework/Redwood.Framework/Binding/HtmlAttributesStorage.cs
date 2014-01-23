using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Binding
{
    public class HtmlAttributesStorage : IHtmlAttributesStorage
    {
        Dictionary<string, string> values;

        public HtmlAttributesStorage()
        {
            values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public void SetAttributeValue(string name, string value)
        {
            values[name] = value;
        }

        public void Render(Generation.IHtmlWriter writer)
        {
            foreach (var item in values)
            {
                writer.AddAttribute(item.Key, item.Value);
            }
        }
    }
}
