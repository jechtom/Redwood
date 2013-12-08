using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Redwood.Framework.ViewModel;

namespace Redwood.Framework.Parsing.ViewModel
{
    /// <summary>
    /// Extracts information about the view model.
    /// </summary>
    public class ReflectionViewModelMetadataExtractor : IViewModelMetadataExtractor
    {

        private static readonly Type[] builtinTypes = new[]
        {
            typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong),
            typeof(float), typeof(double), typeof(decimal), 
            typeof(Guid), typeof(string), typeof(char), typeof(bool), typeof(object)
        };

        /// <summary>
        /// Gets the dependent types.
        /// </summary>
        public IEnumerable<Type> GetDependentTypes(Type type)
        {
            var dependentTypes = new HashSet<Type> { type };
            GetDependentTypesCore(type, dependentTypes);
            return dependentTypes;
        }

        /// <summary>
        /// Gets the dependent types into the specified hashset.
        /// </summary>
        public void GetDependentTypesCore(Type type, HashSet<Type> dependentTypes)
        {
            foreach (var prop in GetProperties(type))
            {
                // get the property type
                var propType = prop.PropertyType;

                // if it is an array or IEnumerable, extract the inner type
                while (propType.IsArray)
                {
                    propType = propType.GetElementType();
                }
                Type ienum;
                do
                {
                    ienum = propType.GetInterfaces().FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                    if (ienum != null)
                    {
                        propType = ienum.GetGenericArguments()[0];
                    }
                }
                while (ienum != null);

                // if it is not the builtin type, add it to the list
                if (!builtinTypes.Contains(propType) && !dependentTypes.Contains(propType))
                {
                    dependentTypes.Add(propType);
                    GetDependentTypesCore(propType, dependentTypes);
                }
            }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        public IEnumerable<ViewModelProperty> GetProperties(Type type)
        {
            foreach (var prop in type.GetProperties())
            {
                var attr = prop.GetCustomAttribute<ClientImplementationAttribute>();
                yield return new ViewModelProperty()
                {
                    PropertyName = prop.Name,
                    PropertyType = prop.PropertyType,
                    IsReadOnly = !prop.CanWrite, 
                    ClientImplementation = attr != null ? attr.Expression : string.Empty
                };
            }
        }

        /// <summary>
        /// Gets the commands.
        /// </summary>
        public IEnumerable<ViewModelCommand> GetCommands(Type type)
        {
            foreach (var command in type.GetMethods().Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object)))
            {
                var attr = command.GetCustomAttribute<ClientImplementationAttribute>();
                yield return new ViewModelCommand()
                {
                    CommandName = command.Name,
                    ParameterTypes = command.GetParameters().Select(p => new ViewModelCommandParameter() { Name = p.Name, Type = p.ParameterType }).ToArray(),
                    ClientFunctionName = attr != null ? attr.Expression : string.Empty 
                };
            }
        }
    }
}