﻿using Redwood.Framework.Binding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Controls
{
    public class Table : ItemsControl
    {
        /// <summary>
        /// Renders the specified writer.
        /// </summary>
        public override void Render(Generation.IHtmlWriter writer)
        {
            writer.RenderBeginTag("table");

            BindingExpression expr = KnockoutBindingHelper.GetExpressionOrNull(ItemsSourceProperty, this);
            if (KnockoutBindingHelper.IsKnockoutBinding(expr))
            {
                writer.RenderBeginTag("tbody");
                writer.AddBindingAttribute("foreach", KnockoutBindingHelper.TranslateToKnockoutProperty(expr.Path));
                ItemTemplate.DataContext = null;
                ItemTemplate.Render(writer);
                writer.RenderEndTag();
            }
            else
            {
                writer.RenderBeginTag("tbody");
                foreach (var item in ItemsSource)
                {
                    writer.RenderBeginTag("tr");
                    ItemTemplate.DataContext = item;
                    ItemTemplate.Render(writer);
                    writer.RenderEndTag();
                }
                writer.RenderEndTag();    
            }
            
            writer.RenderEndTag();
        }



    }
}
