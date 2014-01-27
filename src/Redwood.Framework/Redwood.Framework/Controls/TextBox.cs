using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Controls
{
    public class TextBox : RenderableControl
    {
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public static readonly RedwoodProperty TextProperty = RedwoodProperty.Register<string, TextBox>("Text");

        public TextMode Mode
        {
            get
            {
                return (TextMode)GetValue(ModeProperty);
            }
            set
            {
                SetValue(ModeProperty, value);
            }
        }

        public static readonly RedwoodProperty ModeProperty = RedwoodProperty.Register<TextMode, TextBox>("Mode", TextMode.Text);

        public override void Render(Generation.IHtmlWriter writer)
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
            BindingMarkupExpression expr = KnockoutBindingHelper.GetExpressionOrNull(TextProperty, this);
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
                throw new NotImplementedException();
                //writer.AddBindingAttribute("value", KnockoutBindingHelper.TranslateToKnockoutProperty(expr.Path));
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
