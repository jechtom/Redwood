using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Redwood.Framework.RwHtml.Converters
{
    public class TypeConverterRwHtmlMarkupConverter : RwHtmlMarkupConverter
    {
        private TypeConverter converter;

        public TypeConverterRwHtmlMarkupConverter(TypeConverter converter)
        {
            this.converter = converter;
        }

        protected override bool TryConvertFromStringCore(string value, out object result)
        {
            if (converter.IsValid(value))
            {
                result = converter.ConvertFromString(value);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }
}