using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml
{
    public class TypeConverterMapper
    {

        

        public bool TryConvertFromString(string value, Type outputType, out object result)
        {
            // string
            if (outputType == typeof(string))
            {
                result = value;
                return true;
            }

            // nullable types
            if (outputType.IsGenericTypeDefinition && outputType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (string.IsNullOrEmpty(value))
                {
                    result = null;
                    return true;
                }
                else
                {
                    // continue with inner type
                    outputType = outputType.GetGenericArguments()[0];
                }
            }

            // custom converter (we can have multiple custom converters, the first one wins)
            var customAttributes = (RwHtmlMarkupConverterAttribute[])outputType.GetCustomAttributes(typeof (RwHtmlMarkupConverterAttribute));
            for (int i = 0; i < customAttributes.Length; i++)
            {
                var customConverter = customAttributes[i].GetConverter();
                if (customConverter.TryConvertFromString(value, out result))
                {
                    return true;
                }
            }

            // enums
            if (outputType.IsEnum)
            {
                return EnumHelpers.TryParse(outputType, value, true, out result);
            }

            // other types
            var converter = TypeDescriptor.GetConverter(outputType);
            if (converter.CanConvertFrom(typeof (string)))
            {
                if (converter.IsValid(value))
                {
                    result = converter.ConvertFromString(value);
                    return true;
                }
            }

            result = null;
            return false;
        }

    }
}
