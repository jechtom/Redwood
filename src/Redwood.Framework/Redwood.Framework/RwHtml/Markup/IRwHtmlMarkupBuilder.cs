using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    public interface IRwHtmlMarkupBuilder
    {
        void PushElement(MarkupElement element);
        void WriteValue(MarkupValue value);
        void PopElement();
    }
}
