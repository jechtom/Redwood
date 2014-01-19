using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.RwHtml.Converters
{
    public class EnumRwHtmlMarkupConverter : RwHtmlMarkupConverter
    {
        private Type outputType;

        public EnumRwHtmlMarkupConverter(Type outputType)
        {
            this.outputType = outputType;
        }

        protected override bool TryConvertFromStringCore(string value, out object result)
        {
            return EnumHelpers.TryParse(outputType, value, true, out result);
        }
    }
}