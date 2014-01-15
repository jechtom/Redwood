using Redwood.Framework.RwHtml.Parsing.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Parsing
{
    /// <summary>
    /// Provides basic parsing of rwhtml tokens.
    /// </summary>
    public abstract class RwHtmlTokenParserBase<T>
    {
        Stack<RwOpenTagBeginToken> tagsStack;
        Queue<T> valuesQueue;

        bool isInsideOpenedTag;

        protected virtual void Init()
        {
            tagsStack = new Stack<RwOpenTagBeginToken>();
            valuesQueue = new Queue<T>();
            isInsideOpenedTag = false;
        }

        public virtual IEnumerable<T> Read(IEnumerable<RwHtmlToken> tokenSource)
        {
            if (tokenSource == null)
                throw new ArgumentNullException("tokenSource");

            Init();

            // read
            foreach (var token in tokenSource)
            {
                ReadToken(token);

                // return values
                while(valuesQueue.Count > 0)
                {
                    yield return valuesQueue.Dequeue();
                }
            }

            // make sure all pushed tokens has been poped out
            EnsureStackIsEmpty();

            // end of document
            OnEndOfDocument();

            // return remaining values
            while (valuesQueue.Count > 0)
            {
                yield return valuesQueue.Dequeue();
            }
        }

        protected virtual void PushValue(T value)
        {
            valuesQueue.Enqueue(value);
        }

        protected virtual void ReadToken(RwHtmlToken token)
        {
            if (token is RwOpenTagBeginToken)
            {
                ReadRwOpenTagBeginToken((RwOpenTagBeginToken)token);
            }
            else if (token is RwOpenTagEndToken)
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
            else if (token is RwValueToken)
            {
                ReadRwLiteralToken((RwValueToken)token);
            }
            else
            {
                throw new RwHtmlParsingException("Unknown token type: " + token.GetType().FullName, token.SpanPosition);
            }
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
            if (token.Value is RwValueToken)
            {
                OnNewAttributeValue(token, (RwValueToken)token.Value);
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

        protected virtual void ReadRwLiteralToken(RwValueToken token)
        {
            if(isInsideOpenedTag)
                throw new RwHtmlParsingException("Literal token is not allowed inside opening element. ", token.SpanPosition);

            OnLiteralToken(token);
        }

        protected abstract void OnOpenTagBegin(RwOpenTagBeginToken token);
        
        protected abstract void OnOpenTagEnd();

        protected abstract void OnTagEnd();

        protected abstract void OnNewAttributeValue(RwAttributeToken token, RwValueToken value);

        protected abstract void OnLiteralToken(RwValueToken literal);

        protected abstract void OnEndOfDocument();
    }
}
