using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.RwHtml.Converters
{
    public class StringRwHtmlMarkupConverter : RwHtmlMarkupConverter
    {
        protected override bool TryConvertFromStringCore(string value, out object result)
        {
            result = value;
            return true;
        }
    }
}