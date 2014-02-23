using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
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

                    var path = DataContextPathBuilder.AppendPropertyPath(DataContextPath, itemsSourceExpression.Path);
                    path = DataContextPathBuilder.AppendCollectionIndexPlaceholder(path);
                    ItemTemplate.DataContextPath = path;
                    ItemTemplate.DataContext = null;
                    ItemTemplate.Render(writer);

                    writer.RenderEndTag();
                }
                else
                {
                    writer.RenderBeginTag("tbody");
                    var index = 0;
                    foreach (var item in ItemsSource)
                    {
                        // render on server side
                        writer.RenderBeginTag("tr");

                        var path = DataContextPathBuilder.AppendPropertyPath(DataContextPath, itemsSourceExpression.Path);
                        path = DataContextPathBuilder.AppendCollectionIndex(path, index);
                        ItemTemplate.DataContextPath = path;
                        ItemTemplate.DataContext = item;
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
