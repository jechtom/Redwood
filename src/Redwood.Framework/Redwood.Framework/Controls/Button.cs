using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Generation;

namespace Redwood.Framework.Controls
{
    public class Button : RenderableControl
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static RedwoodProperty TextProperty = RedwoodProperty.Register<string, Button>("Text");


        public object OnClick
        {
            get { return GetValue(OnClickProperty); }
            set { SetValue(OnClickProperty, value); }
        }
        public static RedwoodProperty OnClickProperty = RedwoodProperty.Register<object, Button>("OnClick");




        protected override void RenderControl(IHtmlWriter writer)
        {
            writer.RenderBeginTag("input");
            
            var expr = KnockoutBindingHelper.GetBindingExpressionOrNull(TextProperty, this);
            if (KnockoutBindingHelper.IsKnockoutBinding(expr))
            {
                writer.AddBindingAttribute("value", KnockoutBindingHelper.TranslateToKnockoutProperty(this, TextProperty, expr));
            }
            else
            {
                writer.AddAttribute("value", Text);
            }

            var expr2 = KnockoutBindingHelper.GetCommandExpressionOrNull(OnClickProperty, this);
            if (KnockoutBindingHelper.IsKnockoutCommand(expr2))
            {
                writer.AddBindingAttribute("click", KnockoutBindingHelper.TranslateToKnockoutCommand(this, OnClickProperty, expr2));
            }
            
            writer.AddAttribute("type", "button");
            writer.RenderEndTag();
        }

    }
}
