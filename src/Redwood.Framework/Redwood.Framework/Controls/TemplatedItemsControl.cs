using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Binding;
using Redwood.Framework.Binding.Parsing.Expressions;

namespace Redwood.Framework.Controls
{
    public abstract class TemplatedItemsControl : ItemsControl
    {

        public RedwoodTemplate ItemTemplate
        {
            get { return (RedwoodTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        public static readonly RedwoodProperty ItemTemplateProperty = RedwoodProperty.Register<RedwoodTemplate, ItemsControl>("ItemTemplate", new RedwoodPropertyMetadata(null, RedwoodPropertyFlags.IsInheritanceSource));




        public static BindingGetPropertyExpression CreateClientTemplateInstanceDataContextBinding()
        {
            return new BindingGetPropertyExpression()
            {
                PropertyName = "",
                Indexer = new BindingArrayGetByIndexExpression() { IsPlaceholder = true }
            };
        }

        public static BindingGetPropertyExpression CreateClientTemplateInstanceDataContextBinding(string keyPropertyName)
        {
            return new BindingGetPropertyExpression()
            {
                PropertyName = "",
                Indexer = new BindingArrayGetByKeyExpression() { IsPlaceholder = true, KeyPropertyName = keyPropertyName }
            };
        }

        public static BindingGetPropertyExpression CreateServerTemplateInstanceDataContextBinding(int index)
        {
            return new BindingGetPropertyExpression()
            {
                PropertyName = "",
                Indexer = new BindingArrayGetByIndexExpression() { Index = index }
            };
        }

        public static BindingGetPropertyExpression CreateServerTemplateInstanceDataContextBinding(string keyPropertyName, object keyValue)
        {
            var keyValueString = keyValue == null ? string.Empty : keyValue.ToString();

            return new BindingGetPropertyExpression()
            {
                PropertyName = "",
                Indexer = new BindingArrayGetByKeyExpression() { KeyPropertyName = keyPropertyName, KeyValue = keyValueString }
            };
        }
    }
}