using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Controls
{
    /// <summary>
    /// Represents a literal control.
    /// </summary>
    public class Literal : RedwoodControl
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Literal"/> class.
        /// </summary>
        public Literal(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Renders the control to the HTML.
        /// </summary>
        public override void Render(Generation.IHtmlWriter writer)
        {
            if (!HasClientSideBinding("Text"))
            {
                writer.WriteText(Text, false);
            }
            else
            {
                writer.RenderBeginTag("span");
                writer.AddBindingAttribute("text", Bindings["Text"].ClientPath);
                writer.RenderEndTag();
            }
        }
    }
}