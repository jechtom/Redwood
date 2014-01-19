using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.RwHtml.Converters
{
    public class NullableRwHtmlMarkupConverter : RwHtmlMarkupConverter
    {
        public NullableRwHtmlMarkupConverter(RwHtmlMarkupConverter nextConverter)
        {
            NextConverter = nextConverter;
        }

        public RwHtmlMarkupConverter NextConverter { get; private set; }


        protected override bool TryConvertFromStringCore(string value, out object result)
        {
            if (string.IsNullOrEmpty(value))
            {
                result = null;
                return true;
            }
            else
            {
                return NextConverter.TryConvertFromString(value, out result);
            }
        }
    }
}