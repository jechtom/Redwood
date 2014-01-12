using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Controls
{
    public class Page : ContainerControl
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly RedwoodProperty TitleProperty = RedwoodProperty.Register<string, Page>("Title");

        public override void Render(Generation.IHtmlWriter writer)
        {
            writer.RenderBeginTag("html");

            // head
            writer.RenderBeginTag("head");
            RenderHead(writer);
            writer.RenderEndTag();

            // body
            writer.RenderBeginTag("body");
            base.Render(writer);
            writer.RenderEndTag();

            writer.RenderEndTag();
        }

        private void RenderHead(Generation.IHtmlWriter writer)
        {
            writer.RenderBeginTag("title");
            writer.WriteText(Title, true);
            writer.RenderEndTag();
        }
    }
}
