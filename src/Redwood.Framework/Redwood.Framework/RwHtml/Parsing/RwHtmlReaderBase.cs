using Redwood.Framework.RwHtml.Parsing.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Parsing
{
    public abstract class RwHtmlReaderBase
    {
        Stack<RwOpenTagBeginToken> tagsStack;
        bool isInsideOpenedTag;

        private void Init()
        {
            tagsStack = new Stack<RwOpenTagBeginToken>();
            isInsideOpenedTag = false;
        }

        public virtual void Read(IEnumerable<RwHtmlToken> tokenSource)
        {
            Init();

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
        }

        protected virtual void ReadRwOpenTagBeginToken(RwOpenTagBeginToken token)
        {
            if (isInsideOpenedTag)
                throw new RwHtmlParsingException("Open tag token inside another tag is not allowed. ", token.SpanPosition);

            isInsideOpenedTag = true;

            tagsStack.Push(token);

            NewElementBegin();
        }

        protected virtual void ReadRwAttributeToken(RwAttributeToken token)
        {
            if (!isInsideOpenedTag)
                throw new RwHtmlParsingException("Attribute token is not allowed outside of opening element.", token.SpanPosition);

            OnNewAttribute();
        }

        protected virtual void ReadRwOpenTagEndToken(RwOpenTagEndToken token)
        {
            if (!isInsideOpenedTag)
                throw new RwHtmlParsingException("Open tag end token is not allowed without open tag begin token.", token.SpanPosition);

            isInsideOpenedTag = false;

            if (token.IsSelfClosing)
                tagsStack.Pop();

            OnOpenTagEnd();
        }

        protected virtual void ReadRwCloseTagToken(RwCloseTagToken token)
        {
            throw new NotImplementedException();
        }

        protected virtual void ReadRwLiteralToken(RwLiteralToken token)
        {
            if(isInsideOpenedTag)
                throw new RwHtmlParsingException("Literal token is not allowed inside opening element. ", token.SpanPosition);

            OnLiteralToken();
        }

        protected abstract void NewElementBegin();
        protected abstract void OnNewAttribute();
        protected abstract void OnOpenTagEnd();
        protected abstract void OnLiteralToken();
    }
}
