using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Redwood.Framework.RwHtml.Converters
{
    public static class EnumHelpers
    {
        static MethodInfo tryParseMethod;

        static EnumHelpers()
        {
            tryParseMethod = typeof(Enum).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == "TryParse" && m.GetParameters().Length == 3);
        }

        public static bool TryParse(Type enumType, string value, bool ignoreCase, out object enumValue)
        {
            var genericTryParseMethod = tryParseMethod.MakeGenericMethod(enumType);
            var args = new[] { value, ignoreCase, Activator.CreateInstance(enumType) };
            
            var success = (bool)genericTryParseMethod.Invoke(null, args);
            enumValue = args[2];
            return success;
        }
    }
}