using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redwood.Framework.Generation
{
    public interface IHtmlWriter
    {

        /// <summary>
        /// Writes the text to the output.
        /// </summary>
        void WriteText(string text, bool htmlEncode);

        /// <summary>
        /// Renders the begin tag.
        /// </summary>
        void RenderBeginTag(string tagName);

        /// <summary>
        /// Renders the end tag.
        /// </summary>
        void RenderEndTag();

        /// <summary>
        /// Adds the attribute.
        /// </summary>
        void AddAttribute(string name, string value);

        /// <summary>
        /// Adds the inline CSS attribute.
        /// </summary>
        void AddStyleAttribute(string name, string value);

        /// <summary>
        /// Adds the Knockout JS binding attribute.
        /// </summary>
        void AddBindingAttribute(string name, string value);

    }
}
