using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Controls
{
    public class HtmlElement : ContentControl, ICustomHtmlAttributes
    {
        public string Element
        {
            get
            {
                return (string)GetValue(ElementProperty);
            }
            set
            {
                SetValue(ElementProperty, value);
            }
        }

        public static readonly RedwoodProperty ElementProperty = RedwoodProperty.Register<string, HtmlElement>("Element");

        public override void Render(Generation.IHtmlWriter writer)
        {
            writer.RenderBeginTag(Element);
            // TODO write HTML element properties
            base.Render(writer);
            writer.RenderEndTag();
        }

        public void SetAttributeValue(string name, string value)
        {
            // TODO do something
        }
    }
}
