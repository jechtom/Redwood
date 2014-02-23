using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Generation;

namespace Redwood.Framework.Controls
{
    public class TextBox : RenderableControl
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly RedwoodProperty TextProperty = RedwoodProperty.Register<string, TextBox>("Text");

        public TextMode Mode
        {
            get { return (TextMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }
        public static readonly RedwoodProperty ModeProperty = RedwoodProperty.Register<TextMode, TextBox>("Mode", TextMode.Text);



        protected override void RenderControl(IHtmlWriter writer)
        {
            // begin
            if (Mode == TextMode.MultiLine)
            {
                writer.RenderBeginTag("textarea");
            }
            else
            {
                writer.RenderBeginTag("input");
                writer.AddAttribute("type", Mode.ToString().ToLower());
            }

            // content
            var expr = KnockoutBindingHelper.GetBindingExpressionOrNull(TextProperty, this);
            if (!KnockoutBindingHelper.IsKnockoutBinding(expr))
            {
                if (Mode == TextMode.MultiLine)
                {
                    writer.WriteText(Text, true);
                }
                else
                {
                    writer.AddAttribute("value", Text);
                }
            }
            else
            {
                writer.AddBindingAttribute("value", KnockoutBindingHelper.TranslateToKnockoutProperty(this, TextProperty, expr));
            }

            // end
            writer.RenderEndTag();
        }
    }

    public enum TextMode
    {
        Text,
        MultiLine,
        Password
    }
}
