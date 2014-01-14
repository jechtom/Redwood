using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Parsing
{
    public class RwHtmlParsingException : Exception
    {
        public RwHtmlParsingException(string message, SpanPosition span)
            : base(message)
        {
            this.Span = span;
        }

        public SpanPosition Span { get; private set; }
    }
}
