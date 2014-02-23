using Redwood.Framework.Binding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redwood.Framework.Generation;

namespace Redwood.Framework.Controls
{
    /// <summary>
    /// Represents container for single <see cref="RedwoodControl"/> or <see cref="IHtmlContent"/> or any encoded text content.
    /// </summary>
    [ContentProperty("Content")]
    public class ContentControl : RenderableControl
    {
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set {  SetValue(ContentProperty, value); }
        }
        public static readonly RedwoodProperty ContentProperty = RedwoodProperty.Register<object, ContentControl>("Content");



        protected override void RenderControl(IHtmlWriter writer)
        {
            var content = Content;

            if (content is IEnumerable)
            {
                // list
                foreach (var item in (IEnumerable)content)
                {
                    RenderObject(writer, item);
                }
            }
            else
            {
                // single object
                RenderObject(writer, content);
            }
        }

        private static void RenderObject(Generation.IHtmlWriter writer, object content)
        {
            if (content == null)
                return;

            // render nested control
            if (content is IRenderable)
            {
                ((IRenderable)content).Render(writer);
                return;
            }

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
