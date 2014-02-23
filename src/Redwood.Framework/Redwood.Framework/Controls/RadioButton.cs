using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Binding;
using Redwood.Framework.Generation;

namespace Redwood.Framework.Controls
{
    public class RadioButton : RenderableControl
    {
        public bool IsChecked
        {
            get { return Value == SelectedValue; }
        }

        public string SelectedValue
        {
            get { return (string)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }
        public static RedwoodProperty SelectedValueProperty = RedwoodProperty.Register<string, CheckBox>("SelectedValue", new RedwoodPropertyMetadata(""));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static RedwoodProperty TextProperty = RedwoodProperty.Register<string, CheckBox>("Text", new RedwoodPropertyMetadata(""));

        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }
        public static RedwoodProperty GroupNameProperty = RedwoodProperty.Register<string, CheckBox>("GroupName", new RedwoodPropertyMetadata(""));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static RedwoodProperty ValueProperty = RedwoodProperty.Register<string, CheckBox>("Value", new RedwoodPropertyMetadata(""));



        protected override void RenderControl(IHtmlWriter writer)
        {
            var textExpression = KnockoutBindingHelper.GetBindingExpressionOrNull(TextProperty, this);
            var needsLabel = KnockoutBindingHelper.IsKnockoutBinding(textExpression) || !string.IsNullOrEmpty(Text);


            // render the checkbox
            writer.RenderBeginTag("input");
            writer.AddAttribute("type", "radio");
            
            var valueExpression = KnockoutBindingHelper.GetBindingExpressionOrNull(ValueProperty, this);
            if (KnockoutBindingHelper.IsKnockoutBinding(valueExpression))
            {
                writer.AddBindingAttribute("value", KnockoutBindingHelper.TranslateToKnockoutProperty(this, ValueProperty, valueExpression));
            }
            else
            {
                writer.AddAttribute("value", Value);
            }

            writer.AddAttribute("name", GroupName);

            var selectedValueExpression = KnockoutBindingHelper.GetBindingExpressionOrNull(SelectedValueProperty, this);
            if (KnockoutBindingHelper.IsKnockoutBinding(selectedValueExpression))
            {
                writer.AddBindingAttribute("checked", KnockoutBindingHelper.TranslateToKnockoutProperty(this, SelectedValueProperty, selectedValueExpression));
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