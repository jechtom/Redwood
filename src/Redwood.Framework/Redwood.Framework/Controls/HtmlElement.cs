using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redwood.Framework.Generation;

namespace Redwood.Framework.Controls
{
    public class HtmlElement : ContentControl, IHtmlAttributesStorageProvider
    {
        private HtmlAttributesStorage htmlAttributesStorage;

        public string Element
        {
            get { return (string)GetValue(ElementProperty); }
            set { SetValue(ElementProperty, value); }
        }

        public static readonly RedwoodProperty ElementProperty = RedwoodProperty.Register<string, HtmlElement>("Element");

        protected override void RenderControl(IHtmlWriter writer)
        {
            writer.RenderBeginTag(Element);
            if (htmlAttributesStorage != null)
                htmlAttributesStorage.Render(writer);
            base.RenderControl(writer);
            writer.RenderEndTag(true);
        }

        public override string ToString()
        {
            return "<" + Element + ">";
        }

        public IHtmlAttributesStorage ProvideStorage()
        {
            if (htmlAttributesStorage == null)
                htmlAttributesStorage = new HtmlAttributesStorage();
            return htmlAttributesStorage;
        }
    }
}
