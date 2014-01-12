using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public static readonly RedwoodProperty ModeProperty = RedwoodProperty.Register<TextMode, TextBox>("Mode", defaultValue: TextMode.Text);

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
            BindingBase expr = KnockoutBindingHelper.GetExpressionOrNull(TextProperty, this);
            if (!KnockoutBindingHelper.IsKnockoutBinding(expr))
            {
                writer.WriteText(Text, true);
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
