using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Controls
{
    public class Button : RedwoodControl
    {


        [Bindable(true, BindingDirection.TwoWay)]
        public string Text { get; set; }
        


        public override void Render(Generation.IHtmlWriter writer)
        {
            writer.RenderBeginTag("input");

            if (HasClientSideBinding("Text"))
            {
                writer.AddBindingAttribute("value", TranslateToKnockoutProperty(Bindings["Text"].Path));
            }
            else
            {
                writer.AddAttribute("value", Text);
            }

            if (HasClientSideBinding("OnClick"))
            {
                writer.AddBindingAttribute("click", TranslateToKnockoutCommand(Bindings["OnClick"].Path));
            }

            writer.AddAttribute("type", "button");
            writer.RenderEndTag();
        }

    }
}
