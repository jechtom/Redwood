using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Controls
{
    public class HtmlElement : ContainerControl
    {
        public string ElementName
        {
            get
            {
                return (string)GetValue(ElementNameProperty);
            }
            set
            {
                SetValue(ElementNameProperty, value);
            }
        }

        public static readonly RedwoodProperty ElementNameProperty = RedwoodProperty.Register<string, HtmlElement>("ElementName");

        public override void Render(Generation.IHtmlWriter writer)
        {
            writer.RenderBeginTag(ElementName);
            // TODO write HTML element properties
            base.Render(writer);
            writer.RenderEndTag();
        }
    }
}
