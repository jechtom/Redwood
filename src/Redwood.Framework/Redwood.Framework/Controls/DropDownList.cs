using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Binding;
using Redwood.Framework.Generation;

namespace Redwood.Framework.Controls
{
    public class DropDownList : ItemsControl
    {
        public BindingMarkupExtension DisplayMemberBinding
        {
            get { return (BindingMarkupExtension)GetValue(DisplayMemberBindingProperty); }
            set { SetValue(DisplayMemberBindingProperty, value); }
        }
        public static RedwoodProperty DisplayMemberBindingProperty = RedwoodProperty.Register<BindingMarkupExtension, DropDownList>("DisplayMemberBinding", new RedwoodPropertyMetadata(null));


        public BindingMarkupExtension ValueMemberBinding
        {
            get { return (BindingMarkupExtension)GetValue(ValueMemberBindingProperty); }
            set { SetValue(ValueMemberBindingProperty, value); }
        }
        public static RedwoodProperty ValueMemberBindingProperty = RedwoodProperty.Register<BindingMarkupExtension, DropDownList>("ValueMemberBinding", new RedwoodPropertyMetadata(null));


        public object SelectedValue
        {
            get { return (object)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }
        public static RedwoodProperty SelectedValueProperty = RedwoodProperty.Register<object, DropDownList>("SelectedValue", new RedwoodPropertyMetadata(null));


        // TODO: this property does not support client binding
        public string NullItemText
        {
            get { return (string)GetValue(NullItemTextProperty); }
            set { SetValue(NullItemTextProperty, value); }
        }
        public static RedwoodProperty NullItemTextProperty = RedwoodProperty.Register<string, DropDownList>("NullItemText", new RedwoodPropertyMetadata(""));


        // TODO: this property does not support client binding
        public bool DisplayNullItem
        {
            get { return (bool)GetValue(DisplayNullItemProperty); }
            set { SetValue(DisplayNullItemProperty, value); }
        }
        public static RedwoodProperty DisplayNullItemProperty = RedwoodProperty.Register<bool, DropDownList>("DisplayNullItem", new RedwoodPropertyMetadata(false));


        public object EvaluateValueMember(object item)
        {
            if (ValueMemberBinding != null)
            {
                item = ValueMemberBinding.Path.Evaluate(item);
            }
            return item == null ? null : item.ToString();
        }

        public object EvaluateDisplayMember(object item)
        {
            if (DisplayMemberBinding != null)
            {
                item = DisplayMemberBinding.Path.Evaluate(item);
            }
            return item == null ? null : item.ToString();
        }


        /// <summary>
        /// Renders the control to the writer.
        /// </summary>
        protected override void RenderControl(IHtmlWriter writer)
        {
            writer.RenderBeginTag("select");

            var itemsSourceExpression = KnockoutBindingHelper.GetBindingExpressionOrNull(ItemsSourceProperty, this);
            if (KnockoutBindingHelper.IsKnockoutBinding(itemsSourceExpression))
            {
                // render items on client
                writer.AddBindingAttribute("options", KnockoutBindingHelper.TranslateToKnockoutProperty(this, ItemsSourceProperty, itemsSourceExpression));
                if (DisplayNullItem)
                {
                    writer.AddBindingAttribute("optionsCaption", NullItemText);
                }
                if (DisplayMemberBinding != null)
                {
                    writer.AddBindingAttribute("optionsText", KnockoutBindingHelper.TranslateToKnockoutProperty(this, DisplayMemberBindingProperty, DisplayMemberBinding));
                }
                if (ValueMemberBinding != null)
                {
                    writer.AddBindingAttribute("optionsValue", KnockoutBindingHelper.TranslateToKnockoutProperty(this, ValueMemberBindingProperty, ValueMemberBinding));
                }
            }
            else if (ItemsSource != null)
            {
                // render items on server
                if (DisplayNullItem)
                {
                    writer.RenderBeginTag("option");
                    writer.AddAttribute("value", "");
                    writer.WriteText(NullItemText, true);
                    writer.RenderEndTag();
                }

                // render on server
                foreach (var item in ItemsSource)
                {
                    var itemValue = EvaluateValueMember(item);
                    var itemText = (EvaluateDisplayMember(item) ?? "").ToString();
                    var isSelected = (itemValue == null && SelectedValue == null) || itemValue.Equals(SelectedValue);

                    writer.RenderBeginTag("option");
                    writer.AddAttribute("value", (itemValue ?? "").ToString());
                    writer.WriteText(itemText, true);
                    if (isSelected)
                    {
                        writer.AddAttribute("selected", "selected");
                    }
                    writer.RenderEndTag();
                }
            }
        }

        
    }
}
