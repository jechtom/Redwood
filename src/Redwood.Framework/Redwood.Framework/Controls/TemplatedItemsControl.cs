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




        protected BindingExpression CreateClientTemplateInstanceDataContextBinding()
        {
            return new BindingExpression(
                BindingMode.OneTime,
                new BindingGetPropertyExpression()
                {
                    PropertyName = "",
                    Indexer = new BindingArrayGetByIndexExpression() { IsPlaceholder = true }
                },
                ItemsControl.ItemsSourceProperty,
                this);
        }

        protected BindingExpression CreateClientTemplateInstanceDataContextBinding(string keyPropertyName)
        {
            return new BindingExpression(
                BindingMode.OneTime,
                new BindingGetPropertyExpression()
                {
                    PropertyName = "",
                    Indexer = new BindingArrayGetByKeyExpression() { IsPlaceholder = true, KeyPropertyName = keyPropertyName }
                },
                ItemsControl.ItemsSourceProperty,
                this);
        }

        protected BindingExpression CreateServerTemplateInstanceDataContextBinding(int index)
        {
            return new BindingExpression(
                BindingMode.OneTime,
                new BindingGetPropertyExpression()
                {
                    PropertyName = "",
                    Indexer = new BindingArrayGetByIndexExpression() { Index = index }
                },
                ItemsControl.ItemsSourceProperty,
                this);
        }

        protected BindingExpression CreateServerTemplateInstanceDataContextBinding(string keyPropertyName, object keyValue)
        {
            var keyValueString = keyValue == null ? string.Empty : keyValue.ToString();

            return new BindingExpression(
                BindingMode.OneTime,
                new BindingGetPropertyExpression()
                {
                    PropertyName = "",
                    Indexer = new BindingArrayGetByKeyExpression() { KeyPropertyName = keyPropertyName, KeyValue = keyValueString }
                },
                ItemsControl.ItemsSourceProperty,
                this);
        }
    }
}