using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Binding;
using Redwood.Framework.Generation;

namespace Redwood.Framework.Controls
{
    public class CheckBox : RenderableControl
    {
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
        public static RedwoodProperty IsCheckedProperty = RedwoodProperty.Register<bool, CheckBox>("IsChecked", new RedwoodPropertyMetadata(false));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static RedwoodProperty TextProperty = RedwoodProperty.Register<string, CheckBox>("Text", new RedwoodPropertyMetadata(""));




        protected override void RenderControl(IHtmlWriter writer)
        {
            var textExpression = KnockoutBindingHelper.GetBindingExpressionOrNull(TextProperty, this);
            var needsLabel = KnockoutBindingHelper.IsKnockoutBinding(textExpression) || !string.IsNullOrEmpty(Text);
            

            // render the checkbox
            writer.RenderBeginTag("input");
            writer.AddAttribute("type", "checkbox");
            var isCheckedExpression = KnockoutBindingHelper.GetBindingExpressionOrNull(IsCheckedProperty, this);
            if (KnockoutBindingHelper.IsKnockoutBinding(isCheckedExpression))
            {
                writer.AddBindingAttribute("checked", KnockoutBindingHelper.TranslateToKnockoutProperty(this, IsCheckedProperty, isCheckedExpression));
            }
            else
            {
                writer.AddAttribute("checked", IsChecked ? "checked" : "");
            }
            if (needsLabel)
            {
                EnsureHasID();
                writer.AddAttribute("id", ID);
            }
            writer.RenderEndTag();


            // render the label
            if (needsLabel)
            {
                writer.RenderBeginTag("label");
                writer.AddAttribute("for", ID);
                if (KnockoutBindingHelper.IsKnockoutBinding(textExpression))
                {
                    writer.AddBindingAttribute("", KnockoutBindingHelper.TranslateToKnockoutProperty(this, TextProperty, textExpression));
                }
                else
                {
                    writer.WriteText(Text, true);
                }
                writer.RenderEndTag();
            }
        }
    }
}
