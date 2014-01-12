using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Controls
{
    public class HtmlContent : IHtmlContent
    {
        string value;

        public HtmlContent(string value)
        {
            this.value = value;
        }

        public static HtmlContent Create(string value)
        {
            return new HtmlContent(value);
        }

        public readonly static HtmlContent Empty = HtmlContent.Create(string.Empty);

        public string GetRawString()
        {
            return value;
        }
    }
}
