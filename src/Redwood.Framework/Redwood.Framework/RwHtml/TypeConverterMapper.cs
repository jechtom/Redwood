using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Redwood.Framework.RwHtml
{
    public class TypeConverterMapper
    {


        public RwHtmlMarkupConverter GetConverterForType(Type outputType)
        {
            // string
            if (outputType == typeof(string))
            {
                return new BuiltinConverters.StringRwHtmlMarkupConverter();
            }

            // nullable types
            if (outputType.IsGenericType && outputType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var innerType = outputType.GetGenericArguments()[0];
                return new BuiltinConverters.NullableRwHtmlMarkupConverter(GetConverterForType(innerType));
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
                return new BuiltinConverters.EnumRwHtmlMarkupConverter(outputType);
            }

            // other types
            var converter = TypeDescriptor.GetConverter(outputType);
            if (converter.CanConvertFrom(typeof(string)))
            {
                return new BuiltinConverters.TypeConverterRwHtmlMarkupConverter(converter);
            }

            throw new InvalidOperationException(string.Format("Cannot find any converter for type {0}. You have to specify a custom converter by decorating the class with the [RwHtmlMarkupConverter(typeof(CustomConverterType))] attribute.", outputType));
        }
    }


    public class BuiltinConverters
    {

        public class StringRwHtmlMarkupConverter : RwHtmlMarkupConverter
        {
            protected override bool TryConvertFromStringCore(string value, out object result)
            {
                result = value;
                return true;
            }
        }

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

}
