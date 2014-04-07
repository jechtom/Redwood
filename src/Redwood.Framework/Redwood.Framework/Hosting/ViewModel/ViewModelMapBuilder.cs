using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Redwood.Framework.ViewModel;

namespace Redwood.Framework.Hosting.ViewModel
{
    /// <summary>
    /// Builds a map that describes the view model structure and can update the map with existing view model instance. 
    /// Is is also responsible for embedding the control state properties. (to be implemented)
    /// The usage of this class is this: 
    /// 1) In the Init phase, the map from the view model is created.
    /// 2) Then the page control tree is traversed, the control state properties are added to the map, and mapping to control properties is set up.
    /// 3) If the request is a postback, the value from client is deserialized and the map is updated. The values are set to control dependency properties.
    /// 4) When the request is ending, the page control tree is traversed and map is updated.
    /// 5) Then all required properties from view model map are serialized and sent to the client.
    /// 6) Also the properties which will be posted back, are collected, and a client function is generated to ensure that these properties will be posted back.
    /// </summary>
    public class ViewModelMapBuilder
    {
        private static readonly HashSet<Type> primitiveTypes = new HashSet<Type>(new[]
        {
            typeof(bool), typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong),
            typeof(float), typeof(double), typeof(decimal), typeof(DateTime), typeof(TimeSpan), typeof(Guid), typeof(char),
            typeof(bool?), typeof(byte?), typeof(sbyte?), typeof(short?), typeof(ushort?), typeof(int?), typeof(uint?), typeof(long?), typeof(ulong?),
            typeof(float?), typeof(double?), typeof(decimal?), typeof(DateTime?), typeof(TimeSpan?), typeof(Guid?), typeof(char?),
            typeof(string)
        });

        /// <summary>
        /// Applies the specified view model contents to the specified view model map.
        /// </summary>
        public virtual void CreateOrUpdateMap(object viewModel, PropertyInfo parentProperty, ref ViewModelMapNode map)
        {
            if (IsPrimitiveValue(viewModel))
            {
                // view model is primitive type value
                CreateOrUpdateConstantMap(viewModel, ref map);
            }
            else if (IsCollectionOrArray(viewModel))
            {
                // view model is a collection or an array
                CreateOrUpdateArrayMap(viewModel, parentProperty, ref map);
            }
            else
            {
                // view model is an object with properties
                CreateOrUpdatePropertyMap(viewModel, parentProperty, ref map);
            }
        }

        /// <summary>
        /// Creates the or update constant map.
        /// </summary>
        protected virtual void CreateOrUpdateConstantMap(object viewModel, ref ViewModelMapNode map)
        {
            if (map == null)
            {
                map = new ViewModelMapPrimitiveNode() { Value = viewModel };
            }
            else if (map is ViewModelMapPrimitiveNode)
            {
                ((ViewModelMapPrimitiveNode)map).Value = viewModel;
            }
            else
            {
                throw new Exception(string.Format("The viewModel is primitive value but but {0} was expected.", map.GetType()));
            }
        }

        /// <summary>
        /// Creates the or update array map.
        /// </summary>
        protected virtual void CreateOrUpdateArrayMap(object viewModel, PropertyInfo parentProperty, ref ViewModelMapNode map)
        {
            if (map == null)
            {
                map = new ViewModelMapArrayNode();
            }
            if (map is ViewModelMapArrayNode)
            {
                var itemValues = ((IEnumerable)viewModel).OfType<object>().ToList();
                var typedMap = (ViewModelMapArrayNode)map;

                // if the collection can be compared and updated by some property key, do it, otherwise replace the collection contents;
                // the synchronization can only occur when there is a key map or when the collection is empty
                var keyName = ResolveCollectionKeyName(parentProperty);
                if (!string.IsNullOrEmpty(keyName) && typedMap.Items.Count == typedMap.KeyMap.Count)
                {
                    SynchronizeCollections(typedMap, itemValues, keyName);
                }
                else
                {
                    // replace collection contents
                    typedMap.Items.Clear();
                    typedMap.Items.AddRange(itemValues.Select(i => CreateMap(i, null)));
                    typedMap.KeyMap.Clear();
                }
            }
            else
            {
                throw new Exception(string.Format("The viewModel is a collection or array but the {0} was expected.", map.GetType()));
            }
        }

        /// <summary>
        /// Synchronizes the collections using specified key property.
        /// </summary>
        private void SynchronizeCollections(ViewModelMapArrayNode targetArray, IEnumerable<object> sourceItems, string keyName)
        {
            var sourceKeyMap = sourceItems.ToDictionary(i => i.GetType().GetProperty(keyName).GetValue(i).ToString());
            var targetKeyMap = targetArray.KeyMap;

            // detect changes
            var newItems = sourceKeyMap.Keys.Except(targetKeyMap.Keys).ToList();
            var updatedItems = targetKeyMap.Keys.Intersect(sourceKeyMap.Keys).ToList();
            var removedItems = targetKeyMap.Keys.Except(sourceKeyMap.Keys).ToList();

            // add items
            foreach (var newItem in newItems)
            {
                var item = CreateMap(sourceKeyMap[newItem], null);
                targetArray.Items.Add(item);
                targetKeyMap[newItem] = item;
            }

            // update items
            foreach (var updatedItem in updatedItems)
            {
                var sourceItem = sourceKeyMap[updatedItem];
                var targetItem = targetKeyMap[updatedItem];
                CreateOrUpdateMap(sourceItem, null, ref targetItem);
            }

            // remove items
            foreach (var removedItem in removedItems)
            {
                var item = targetKeyMap[removedItem];
                targetArray.Items.Remove(item);
                targetKeyMap.Remove(removedItem);
            }
        }

        /// <summary>
        /// Creates the or update property map.
        /// </summary>
        protected virtual void CreateOrUpdatePropertyMap(object viewModel, PropertyInfo parentProperty, ref ViewModelMapNode map)
        {
            if (map == null)
            {
                map = new ViewModelMapObjectNode();
            }
            if (map is ViewModelMapObjectNode)
            {
                // get property values
                var propertyMaps = viewModel.GetType().GetProperties().Where(p => p.CanRead).Select(p => new
                {
                    Property = p, 
                    Name = p.Name, 
                    Value = p.GetValue(viewModel)
                }).ToList();

                // add or update properties in the object
                var typedMap = (ViewModelMapObjectNode)map;
                foreach (var propertyMap in propertyMaps)
                {
                    if (typedMap.Properties.ContainsKey(propertyMap.Name))
                    {
                        var currentValue = typedMap.Properties[propertyMap.Name];
                        CreateOrUpdateMap(propertyMap.Value, propertyMap.Property, ref currentValue);
                        typedMap.Properties[propertyMap.Name] = currentValue;
                    }
                    else
                    {
                        typedMap.Properties[propertyMap.Name] = CreateMap(propertyMap.Value, propertyMap.Property);
                    }
                }
            }
            else
            {
                throw new Exception(string.Format("The viewModel is an object but the {0} was expected.", map.GetType()));
            }
        }

        /// <summary>
        /// Creates the view model map from the specified view model.
        /// </summary>
        public ViewModelMapNode CreateMap(object viewModel, PropertyInfo parentProperty = null)
        {
            ViewModelMapNode map = null;
            CreateOrUpdateMap(viewModel, parentProperty, ref map);
            return map;
        }

        /// <summary>
        /// Determines whether the object is a collection or an array.
        /// </summary>
        protected virtual bool IsCollectionOrArray(object viewModel)
        {
            return viewModel is IEnumerable;
        }

        /// <summary>
        /// Determines whether the object is of a primitive type.
        /// </summary>
        protected virtual bool IsPrimitiveValue(object viewModel)
        {
            return (viewModel == null) || primitiveTypes.Contains(viewModel.GetType());
        }


        /// <summary>
        /// Looks on the property of type collection and tries to extract the primary key property that uniquely identifies the item.
        /// It searches for the KeyProperty attribute on ICollection&lt;T&gt; property, and if not found, looks for the Key attribute on a property in the T class.
        /// </summary>
        protected virtual string ResolveCollectionKeyName(PropertyInfo property)
        {
            if (property == null)
            {
                return null;
            }

            // look for the KeyProperty attribute on the collection
            var attr = property.GetCustomAttribute<KeyPropertyAttribute>();
            if (attr != null)
            {
                return attr.PropertyName;
            }

            // try to find the property with Key attribute in the collection items
            var collectionType = new[] { property.PropertyType }.Concat(property.PropertyType.GetInterfaces())
                .FirstOrDefault(i => i.IsInterface && i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IEnumerable<>));
            if (collectionType != null)
            {
                // find properties with Key attribute
                var itemType = collectionType.GetGenericArguments().First();
                var keyProperties = itemType.GetProperties().Where(p => p.GetCustomAttribute<KeyAttribute>() != null).ToList();

                if (keyProperties.Count == 0)
                {
                    return null;
                }
                else if (keyProperties.Count == 1)
                {
                    return keyProperties.First().Name;
                }
                else
                {
                    throw new Exception(string.Format("The class {0} has multiple properties decorated with the [Key] attribute which is not supported in binding. Please add the [KeyProperty(\"Id\")] attribute on the property {1}.{2} and specify one property that can be used as entity primary key, or null (not recommended).", itemType, property.DeclaringType, property.Name));
                }
            }
            return null;
        }
    }
}
