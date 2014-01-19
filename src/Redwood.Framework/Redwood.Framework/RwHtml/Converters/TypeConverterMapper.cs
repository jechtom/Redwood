using System;
using System.ComponentModel;

namespace Redwood.Framework.RwHtml.Converters
{
    public class TypeConverterMapper
    {
        static readonly TypeConverterMapper defaultTypeConverterMapper = new TypeConverterMapper();

        public static TypeConverterMapper Default
        {
            get { return defaultTypeConverterMapper; }
        }





        public RwHtmlMarkupConverter GetConverterForType(Type outputType)
        {
            // string or object
            if (outputType == typeof(string) || outputType == typeof(object))
            {
                return new PassThroughRwHtmlMarkupConverter();
            }

            // nullable types
            if (outputType.IsGenericType && outputType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var innerType = outputType.GetGenericArguments()[0];
                return new NullableRwHtmlMarkupConverter(GetConverterForType(innerType));
            }

            // custom converter
            var customAttributes = (RwHtmlMarkupConverterAttribute[])outputType.GetCustomAttributes(typeof(RwHtmlMarkupConverterAttribute), true);
            if (customAttributes.Length > 0)
            {
                return customAttributes[0].GetConverter();
            }

            // enums
            if (outputType.IsEnum)
            {
                return new EnumRwHtmlMarkupConverter(outputType);
            }

            // other types
            var converter = TypeDescriptor.GetConverter(outputType);
            if (converter.CanConvertFrom(typeof(string)))
            {
                return new TypeConverterRwHtmlMarkupConverter(converter);
            }

            throw new InvalidOperationException(string.Format("Cannot find any converter for type {0}. You have to specify a custom converter by decorating the class with the [RwHtmlMarkupConverter(typeof(CustomConverterType))] attribute.", outputType));
        }
    }
}
