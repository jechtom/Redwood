using Redwood.Framework.Binding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Controls
{
    public abstract class ItemsControl : RenderableControl
    {
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly RedwoodProperty ItemsSourceProperty = RedwoodProperty.Register<IEnumerable, ItemsControl>("ItemsSource");

        public string KeyPropertyName
        {
            get { return (string)GetValue(KeyPropertyNameProperty); }
            set { SetValue(KeyPropertyNameProperty, value); }
        }
        public static RedwoodProperty KeyPropertyNameProperty = RedwoodProperty.Register<string, TemplatedItemsControl>("KeyPropertyName");

        protected object GetKeyValue(object item)
        {
            var itemType = item.GetType();
            var keyProperty = itemType.GetProperty(KeyPropertyName);
            if (keyProperty == null)
            {
                throw new Exception(string.Format("The item of type {0} does not have a property {1}!", itemType, KeyPropertyName)); // TODO: more precise error message
            }
            return keyProperty.GetValue(item);
        }
    }
}
