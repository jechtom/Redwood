using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Controls
{
    public class TextBox : RedwoodControl
    {

        [Bindable(true, BindingDirection.TwoWay)]
        public string Text { get; set; }

        public TextMode Mode { get; set; }


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
            if (!HasClientSideBinding("Text"))
            {
                writer.WriteText(Text, true);
            }
            else
            {
                writer.AddBindingAttribute("value", TranslateToKnockoutProperty(Bindings["Text"].Path));
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
