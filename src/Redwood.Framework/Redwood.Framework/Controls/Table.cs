using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Binding.Parsing.Expressions;
using Redwood.Framework.Generation;

namespace Redwood.Framework.Controls
{
    public class Table : TemplatedItemsControl
    {
        /// <summary>
        /// Renders the specified writer.
        /// </summary>
        protected override void RenderControl(IHtmlWriter writer)
        {
            writer.RenderBeginTag("table");

            if (ItemTemplate != null)
            {
                var itemsSourceExpression = KnockoutBindingHelper.GetBindingExpressionOrNull(ItemsSourceProperty, this);
                if (KnockoutBindingHelper.IsKnockoutBinding(itemsSourceExpression))
                {
                    // knockout template
                    writer.RenderBeginTag("tbody");
                    writer.AddBindingAttribute("foreach", KnockoutBindingHelper.TranslateToKnockoutProperty(this, ItemsSourceProperty, itemsSourceExpression));

                    if (string.IsNullOrEmpty(KeyPropertyName))
                    {
                        ItemTemplate.DataContext = CreateClientTemplateInstanceDataContextBinding();
                    }
                    else
                    {
                        ItemTemplate.DataContext = CreateClientTemplateInstanceDataContextBinding(KeyPropertyName);
                    }
                    ItemTemplate.Render(writer);

                    writer.RenderEndTag();
                }
                else if (ItemsSource != null)
                {
                    writer.RenderBeginTag("tbody");
                    var index = 0;
                    foreach (var item in ItemsSource)
                    {
                        // render on server side
                        writer.RenderBeginTag("tr");

                        if (string.IsNullOrEmpty(KeyPropertyName) || item == null)
                        {
                            ItemTemplate.DataContext = CreateServerTemplateInstanceDataContextBinding(index);
                        }
                        else
                        {
                            var keyValue = GetKeyValue(item);
                            ItemTemplate.DataContext = CreateServerTemplateInstanceDataContextBinding(KeyPropertyName, keyValue);
                        }
                        ItemTemplate.Render(writer);

                        writer.RenderEndTag();

                        index++;
                    }
                    writer.RenderEndTag();
                }
            }

            writer.RenderEndTag();
        }
    }
}
