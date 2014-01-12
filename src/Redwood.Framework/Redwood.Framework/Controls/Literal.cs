using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Redwood.Framework.Controls
{
    /// <summary>
    /// Represents a literal control.
    /// </summary>
    public class Literal : RenderableControl
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
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

        public static readonly RedwoodProperty TextProperty = RedwoodProperty.Register<string, Literal>("Text");


        /// <summary>
        /// Initializes a new instance of the <see cref="Literal"/> class.
        /// </summary>
        public Literal() : this("")
        {
        }

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
            var expr = KnockoutBindingHelper.GetExpressionOrNull(TextProperty, this);
            if (!KnockoutBindingHelper.IsKnockoutBinding(expr))
            {
                writer.WriteText(Text, false);
            }
            else
            {
                writer.RenderBeginTag("span");
                throw new NotImplementedException();
                //writer.AddBindingAttribute("text", KnockoutBindingHelper.TranslateToKnockoutProperty(expr.Path));
                writer.RenderEndTag();
            }
        }
    }
}