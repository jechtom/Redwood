using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Redwood.Framework.Generation
{

    public class HtmlWriter : IHtmlWriter
    {

        private StringBuilder builder = new StringBuilder();

        private Stack<OpenTag> openTags = new Stack<OpenTag>(); 

        /// <summary>
        /// Writes the text to the output.
        /// </summary>
        public void WriteText(string text, bool htmlEncode)
        {
            EnsureCurrentTagRendered();

            if (htmlEncode)
            {
                text = WebUtility.HtmlEncode(text);
            }
            builder.Append(text);
        }

        /// <summary>
        /// Ensures the current tag rendered.
        /// </summary>
        private void EnsureCurrentTagRendered()
        {
            OpenTag lastTag;
            if (openTags.Any() && !(lastTag = openTags.Peek()).IsBeginTagRendered)
            {
                lastTag.WriteBeginTag(builder);
                lastTag.IsBeginTagRendered = true;
            }
        }

        /// <summary>
        /// Renders the begin tag.
        /// </summary>
        public void RenderBeginTag(string tagName)
        {
            EnsureCurrentTagRendered();

            openTags.Push(new OpenTag(tagName));
        }

        /// <summary>
        /// Renders the end tag.
        /// </summary>
        public void RenderEndTag(bool forceFullEndTag = false)
        {
            var tag = GetCurrentOpenTag();
            if (!tag.IsBeginTagRendered && !forceFullEndTag)
            {
                // the begin tag was not rendered, so the tag has no content - we can close it immediately using <tag />
                tag.WriteEmptyTag(builder);
            }
            else
            {
                EnsureCurrentTagRendered();

                // the tag has already been open, render the closing tag
                tag.WriteEndTag(builder);
            }

            openTags.Pop();
        }

        /// <summary>
        /// Adds the attribute.
        /// </summary>
        public void AddAttribute(string name, string value)
        {
            GetCurrentOpenTag().Attributes.Add(name, value);
        }

        /// <summary>
        /// Adds the inline CSS attribute.
        /// </summary>
        public void AddStyleAttribute(string name, string value)
        {
            GetCurrentOpenTag().StyleAttributes.Add(name, value);
        }

        /// <summary>
        /// Adds the Knockout JS binding attribute.
        /// </summary>
        public void AddBindingAttribute(string name, string value)
        {
            GetCurrentOpenTag().BindingAttributes.Add(name, value);
        }

        /// <summary>
        /// Gets the current open tag.
        /// </summary>
        private OpenTag GetCurrentOpenTag()
        {
            if (openTags.Count == 0)
            {
                throw new InvalidOperationException("The RenderBeginTag function must be called prior the AddAttribute, AddStyleAttribute, AddBindingAttribute or RenderEndTag call!");
            }
            return openTags.Peek();
        }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return builder.ToString();
        }



        /// <summary>
        /// Represents a HTML tag in the <see cref="HtmlWriter" /> class.
        /// </summary>
        public class OpenTag
        {
            public string TagName { get; private set; }

            public HtmlAttributeList<string> Attributes { get; private set; }

            public HtmlAttributeList<string> StyleAttributes { get; private set; }

            public HtmlAttributeList<string> BindingAttributes { get; private set; }

            public bool IsBeginTagRendered { get; set; }

            public OpenTag(string tagName)
            {
                TagName = tagName;
                Attributes = new HtmlAttributeList<string>();
                StyleAttributes = new HtmlAttributeList<string>();
                BindingAttributes = new HtmlAttributeList<string>();
            }

            /// <summary>
            /// Writes the begin tag.
            /// </summary>
            public void WriteBeginTag(StringBuilder builder)
            {
                WriteBeginTagWithoutEndBrace(builder);
                builder.Append(">");
            }

            /// <summary>
            /// Writes the empty tag.
            /// </summary>
            public void WriteEmptyTag(StringBuilder builder)
            {
                WriteBeginTagWithoutEndBrace(builder);
                builder.Append(" />");
            }

            /// <summary>
            /// Writes the begin tag without end brace.
            /// </summary>
            private void WriteBeginTagWithoutEndBrace(StringBuilder builder)
            {
                builder.Append("<");
                builder.Append(TagName);

                foreach (var attribute in Attributes)
                {
                    builder.Append(" ");
                    builder.Append(attribute.Key);
                    builder.Append("=\"");
                    builder.Append(WebUtility.HtmlEncode(attribute.Value));
                    builder.Append("\"");
                }

                if (StyleAttributes.Count > 0)
                {
                    builder.Append(" style=\"");
                    var isFirst = true;
                    foreach (var styleAttribute in StyleAttributes)
                    {
                        if (!isFirst)
                        {
                            builder.Append("; ");
                        }
                        isFirst = false;

                        builder.Append(styleAttribute.Key);
                        builder.Append(": ");
                        builder.Append(WebUtility.HtmlEncode(styleAttribute.Value));
                    }
                    builder.Append("\"");
                }

                if (BindingAttributes.Count > 0)
                {
                    builder.Append(" data-bind=\"");
                    var isFirst = true;
                    foreach (var bindingAttribute in BindingAttributes)
                    {
                        if (!isFirst)
                        {
                            builder.Append(", ");
                        }
                        isFirst = false;

                        builder.Append(bindingAttribute.Key);
                        builder.Append(": ");
                        builder.Append(WebUtility.HtmlEncode(bindingAttribute.Value));
                    }
                    builder.Append("\"");
                }
            }

            /// <summary>
            /// Writes the end tag.
            /// </summary>
            public void WriteEndTag(StringBuilder builder)
            {
                builder.Append("</");
                builder.Append(TagName);
                builder.Append(">");
            }
        }
    }
}