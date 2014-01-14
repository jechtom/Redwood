using Redwood.Framework.RwHtml.Parsing.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Parsing
{
    /// <summary>
    /// Provides parsing of rwhtml tokens into markup.
    /// </summary>
    public class RwHtmlTokenParser
    {
        Stack<RwOpenTagBeginToken> tagsStack;
        Markup.IRwHtmlMarkupBuilder markupBuilder;
        Markup.MarkupElement currentElement;

        bool isInsideOpenedTag;

        private void Init(Markup.IRwHtmlMarkupBuilder markupBuilder)
        {
            tagsStack = new Stack<RwOpenTagBeginToken>();
            isInsideOpenedTag = false;
            this.markupBuilder = markupBuilder;
        }

        public virtual void Read(IEnumerable<RwHtmlToken> tokenSource, Markup.IRwHtmlMarkupBuilder markupBuilder)
        {
            if (tokenSource == null)
                throw new ArgumentNullException("tokenSource");

            if (markupBuilder == null)
                throw new ArgumentNullException("markupBuilder");

            Init(markupBuilder);

            // read
            foreach (var token in tokenSource)
            {
                if (token is RwOpenTagBeginToken)
                {
                    ReadRwOpenTagBeginToken((RwOpenTagBeginToken)token);
                } else if (token is RwOpenTagEndToken)
                {
                    ReadRwOpenTagEndToken((RwOpenTagEndToken)token);
                }
                else if (token is RwAttributeToken)
                {
                    ReadRwAttributeToken((RwAttributeToken)token);
                }
                else if (token is RwCloseTagToken)
                {
                    ReadRwCloseTagToken((RwCloseTagToken)token);
                }
                else if (token is RwLiteralToken)
                {
                    ReadRwLiteralToken((RwLiteralToken)token);
                }
                else
                {
                    throw new RwHtmlParsingException("Unknown token type: " + token.GetType().FullName, token.SpanPosition);
                }
            }

            EnsureStackIsEmpty();
        }

        protected virtual void EnsureStackIsEmpty()
        {
            if (tagsStack.Count == 0)
                return; // ok

            var item = tagsStack.Peek();

            throw new RwHtmlParsingException("Closing element is missing for: " + item.TagName, item.SpanPosition);
        }

        protected virtual void ReadRwOpenTagBeginToken(RwOpenTagBeginToken token)
        {
            if (isInsideOpenedTag)
                throw new RwHtmlParsingException("Open tag token inside another tag is not allowed. ", token.SpanPosition);

            isInsideOpenedTag = true;
            
            tagsStack.Push(token);

            OnOpenTagBegin(token);
        }

        protected virtual void ReadRwAttributeToken(RwAttributeToken token)
        {
            if (!isInsideOpenedTag)
                throw new RwHtmlParsingException("Attribute token is not allowed outside of opening element.", token.SpanPosition);

            if (token.Value == null)
            {
                throw new NullReferenceException("Attribute value is null.");
            }
            if (token.Value is RwLiteralToken)
            {
                OnNewAttributeValue(token, (RwLiteralToken)token.Value);
            }
            else if(token.Value is RwBindingToken)
            {
                OnNewAttributeBinding(token, (RwBindingToken)token.Value);
            }
            else
            {
                throw new RwHtmlParsingException("Unknown attribute value type.", token.Value.SpanPosition);
            }
        }

        protected virtual void ReadRwOpenTagEndToken(RwOpenTagEndToken token)
        {
            if (!isInsideOpenedTag)
                throw new RwHtmlParsingException("Open tag end token is not allowed without open tag begin token.", token.SpanPosition);

            isInsideOpenedTag = false;

            OnOpenTagEnd();

            if (token.IsSelfClosing)
            {
                tagsStack.Pop();
                OnTagEnd();
            }
        }

        protected virtual void ReadRwCloseTagToken(RwCloseTagToken token)
        {
            if (isInsideOpenedTag)
                throw new RwHtmlParsingException("Close tag token inside another tag is not allowed. ", token.SpanPosition);

            if(tagsStack.Count == 0)
                throw new RwHtmlParsingException(
                    string.Format("Unexpected closing tag \"{0}\".", token.TagName),
                    token.SpanPosition);

            var tag = tagsStack.Peek();
            if (!string.Equals(tag.TagName, token.TagName, StringComparison.OrdinalIgnoreCase))
                throw new RwHtmlParsingException(
                    string.Format("Unmatched closing tag \"{0}\" (expected tag name is \"{1}\").", token.TagName, tag.TagName),
                    token.SpanPosition);

            tagsStack.Pop();
            OnTagEnd();
        }

        protected virtual void ReadRwLiteralToken(RwLiteralToken token)
        {
            if(isInsideOpenedTag)
                throw new RwHtmlParsingException("Literal token is not allowed inside opening element. ", token.SpanPosition);

            OnLiteralToken(token);
        }

        protected virtual void OnOpenTagBegin(RwOpenTagBeginToken token)
        {
            var name = NameWithPrefix.Parse(token.TagName);
            currentElement = new Markup.MarkupElement(name);
        }

        protected virtual void OnOpenTagEnd()
        {
            markupBuilder.PushElement(currentElement);
            currentElement = null;
        }

        protected virtual void OnTagEnd()
        {
            markupBuilder.PopElement();
        }

        protected virtual void OnNewAttributeValue(RwAttributeToken token, RwLiteralToken value)
        {
            var name = NameWithPrefix.Parse(token.Name);
            currentElement.Attributes.Add(name, new Markup.MarkupValue(value.Text, false));
        }

        protected virtual void OnNewAttributeBinding(RwAttributeToken token, RwBindingToken value)
        {
            var name = NameWithPrefix.Parse(token.Name);
            currentElement.Attributes.Add(name, new Markup.MarkupValue(value.Expression, true));
        }

        protected virtual void OnLiteralToken(RwLiteralToken literal)
        {
            markupBuilder.WriteValue(new Markup.MarkupValue(literal.Text, false));
        }
    }
}
