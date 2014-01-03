using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Binding
{
    public class DefaultModelBinder
    {
        /// <summary>
        /// Converts the value.
        /// </summary>
        public static object ConvertValue(object value, Type type)
        {
            // handle null values
            if ((value == null) && (type.IsValueType))
                return Activator.CreateInstance(type);

            // handle nullable types
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if ((value is string) && ((string)value == string.Empty))
                {
                    // value is an empty string, return null
                    return null;
                }
                else
                {
                    // value is not null
                    var nullableConverter = new NullableConverter(type);
                    type = nullableConverter.UnderlyingType;
                }
            }

            // handle exceptions
            if ((value is string) && (type == typeof(Guid)))
                return new Guid((string)value);
            if (type == typeof(object)) return value;

            // convert
            return Convert.ChangeType(value, type);
        }
    }
}
