using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Controls
{
    /// <summary>
    /// Represents container for single <see cref="RedwoodControl"/> or <see cref="IHtmlContent"/> or any encoded text content.
    /// </summary>
    public class ContentControl : RenderableControl
    {
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set {  SetValue(ContentProperty, value); }
        }

        public static readonly RedwoodProperty ContentProperty = RedwoodProperty.Register<object, ContentControl>("Content");

        public override void Render(Generation.IHtmlWriter writer)
        {
            var content = Content;
            if (content == null)
                return;

            // render nested control
            if (content is IRenderable)
                ((IRenderable)content).Render(writer);

            // render text value
            string contentText;
            bool encode;
            if (content is IHtmlContent)
            {
                contentText = ((IHtmlContent)content).GetRawString();
                encode = false;
            }
            else
            {
                contentText = content.ToString();
                encode = true;
            }

            writer.WriteText(contentText, encode);
        }
    }
}
