using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Controls
{
    public class Button : RenderableControl
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

        static RedwoodProperty TextProperty = RedwoodProperty.Register<string, Button>("Text");

        public object OnClick
        {
            get
            {
                return (object)GetValue(OnClickProperty);
            }
            set
            {
                SetValue(OnClickProperty, value);
            }
        }

        static RedwoodProperty OnClickProperty = RedwoodProperty.Register<object, Button>("OnClick");


        public override void Render(Generation.IHtmlWriter writer)
        {
            writer.RenderBeginTag("input");
            
            var expr = KnockoutBindingHelper.GetExpressionOrNull(TextProperty, this);
            if (KnockoutBindingHelper.IsKnockoutBinding(expr))
            {
                writer.AddBindingAttribute("value", KnockoutBindingHelper.TranslateToKnockoutProperty(expr.Path));
            }
            else
            {
                writer.AddAttribute("value", Text);
            }

            expr = KnockoutBindingHelper.GetExpressionOrNull(OnClickProperty, this);
            if (KnockoutBindingHelper.IsKnockoutBinding(expr))
            {
                writer.AddBindingAttribute("click", KnockoutBindingHelper.TranslateToKnockoutCommand(expr.Path));
            }
            
            writer.AddAttribute("type", "button");
            writer.RenderEndTag();
        }

    }
}
