using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Parsing.ViewModel
{
    public class TypeScriptViewModelTypeMapper : IViewModelTypeMapper
    {

        private static readonly Type[] numericTypes = new[]
        {
            typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong),
            typeof(float), typeof(double), typeof(decimal)
        };

        private Dictionary<Type, string> customTypes = new Dictionary<Type, string>(); 

        /// <summary>
        /// Maps the type.
        /// </summary>
        public string MapType(Type type)
        {
            // primitive types
            if (type == typeof(string) || type == typeof(char) || type == typeof(Guid))
            {
                return "string";
            }
            if (numericTypes.Contains(type))
            {
                return "number";
            }
            if (type == typeof(bool))
            {
                return "boolean";
            }
            if (type == typeof(object))
            {
                return "any";
            }

            // Array
            if (type.IsArray)
            {
                return MapType(type.GetElementType()) + "[]";
            }

            // IEnumerable
            var ienum = type.GetInterfaces().FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof (IEnumerable<>));
            if (ienum != null)
            {
                return MapType(ienum.GetGenericArguments()[0]) + "[]";
            }
            
            // custom types
            if (!customTypes.ContainsKey(type))
            {
                // make unique name
                var existingNames = customTypes.Values;
                var name = type.Name;
                var uniqueName = name;
                var counter = 1;
                while (existingNames.Contains(uniqueName))
                {
                    uniqueName = name + counter.ToString();
                    counter++;
                }
                customTypes[type] = uniqueName;
            }
            return customTypes[type];
        }
    }
}