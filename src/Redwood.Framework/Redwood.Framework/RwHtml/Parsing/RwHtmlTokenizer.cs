using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Redwood.Framework.Parsing;
using Redwood.Framework.RwHtml.Parsing.Tokens;

namespace Redwood.Framework.RwHtml.Parsing
{
    public class RwHtmlTokenizer : BaseTokenizer<RwHtmlAtom, RwHtmlToken>
    {
        // Samples:

        //<a>
        //Is:
        // - OpenTagBegin "<a"
        // - OpenTagEnd ">"

        //<a atr1="x" atr2="y" />
        //Is:
        // - OpenTagBegin "<a"
        // - Attribute " atr1="x""
        // - Attribute " atr2="x""
        // - OpenTagEnd " />" {IsSelfClosing=true}

        //<a />
        //Is:
        // - OpenTagBegin "<a"
        // - OpenTagEnd " />" {IsSelfClosing=true}

        //<a></a>
        //Is:
        // - OpenTagBegin "<a"
        // - OpenTagEnd ">"
        // - CloseTag "</a>"


        /// <summary>
        /// Determines whether the specified atom represents a new line.
        /// </summary>
        protected override bool IsNewLine(RwHtmlAtom atom)
        {
            return atom == RwHtmlAtom.NewLine;
        }

        /// <summary>
        /// Reads the token.
        /// </summary>
        protected override RwHtmlAtom ReadAtomCore()
        {
            var token = RwHtmlAtom.Text;
            var currentChar = Peek();

            // new line
            if (currentChar == '\r')
            {
                // if next token is \n, consider it as one line
                Read();
                if (!IsAtEnd && Peek() == '\n')
                {
                    Read();
                }
                return RwHtmlAtom.NewLine;
            }
            
            if (currentChar == '\n')
            {
                // new line
                token = RwHtmlAtom.NewLine;
            }
            else if (Char.IsWhiteSpace(currentChar))
            {
                // white space
                token = RwHtmlAtom.WhiteSpace;
            }

            // other chars
            switch (currentChar)
            {
                case '<': 
                    token = RwHtmlAtom.OpenAngle;
                    break;

                case '>': 
                    token = RwHtmlAtom.CloseAngle;
                    break;

                case '-': 
                    token = RwHtmlAtom.Dash;
                    break;

                case '\'': 
                    token = RwHtmlAtom.SingleQuote;
                    break;

                case '"':
                    token = RwHtmlAtom.DoubleQuote;
                    break;

                case '{':
                    token = RwHtmlAtom.OpenCurlyBrace;
                    break;

                case '}':
                    token = RwHtmlAtom.CloseCurlyBrace;
                    break;

                case '/':
                    token = RwHtmlAtom.Solidus;
                    break;

                case '=':
                    token = RwHtmlAtom.Equal;
                    break;
            }

            Read();
            return token;
        }
        
        /// <summary>
        /// Parses the whole document.
        /// </summary>
        protected override void ParseDocument()
        {
            if (IsAtEnd) return;

            SpanPosition? lastTextPosition = null;
            MoveNext();
            while (!IsAtEnd)
            {
                // if we have found < or {, return the current text token if we have any
                if (CurrentAtom == RwHtmlAtom.OpenAngle || CurrentAtom == RwHtmlAtom.OpenCurlyBrace)
                {
                    if (lastTextPosition != null)
                    {
                        // return the current text content 
                        ReturnToken(new RwValueToken(GetTextSinceLastToken(), false), DistanceFromLastToken);
                        lastTextPosition = null;
                    }

                    if (CurrentAtom == RwHtmlAtom.OpenAngle)
                    {
                        // element
                        ReadElement();
                    }
                    else if (CurrentAtom == RwHtmlAtom.OpenCurlyBrace)
                    {
                        // binding
                        ReturnToken(ReadBinding(), DistanceFromLastToken);
                    }
                }
                else
                {
                    // text content, only remember the position since there will be multiple text tokens
                    MoveNext();
                    if (lastTextPosition == null)
                    {
                        lastTextPosition = CurrentAtomPosition;
                    }
                }
            }

            if (lastTextPosition != null)
            {
                // return the remaining text content on the end of the buffer
                var remainingText = GetTextSinceLastToken();
                ReturnToken(new RwValueToken(remainingText, false), remainingText.Length);
            }
        }
        
        /// <summary>
        /// Reads the element.
        /// </summary>
        private void ReadElement()
        {
            MoveNext();
            SkipWhiteSpaceOrNewLine();
            if (CurrentAtom == RwHtmlAtom.Text)
            {
                // element opening tag
                var nameStart = DistanceFromLastToken;
                ReadText();
                var tagName = GetTextSinceLastToken().Trim().Substring(nameStart);
                ReturnToken(new RwOpenTagBeginToken(tagName, TagType.StandardTag), DistanceFromLastToken);

                // read attributes
                while (ReadAttribute())
                {
                }

                // self closing tag
                SkipWhiteSpaceOrNewLine();
                if (CurrentAtom == RwHtmlAtom.Solidus)
                {
                    MoveNext();
                    SkipWhiteSpaceOrNewLine();
                    if (CurrentAtom == RwHtmlAtom.CloseAngle)
                    {
                        MoveNext();

                        // tag ending
                        ReturnToken(new RwOpenTagEndToken(tagName, true), DistanceFromLastToken);
                    }
                    else
                    {
                        ThrowParserError("The tag was not closed.");
                    }
                }
                else if (CurrentAtom == RwHtmlAtom.CloseAngle)
                {
                    MoveNext();
                    ReturnToken(new RwOpenTagEndToken(tagName, false), DistanceFromLastToken);

                    if (string.Equals(tagName, "script", StringComparison.InvariantCultureIgnoreCase) ||
                        string.Equals(tagName, "style", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ReadLiteralUntilEndTag(tagName);
                    }
                }
                else
                {
                    ThrowParserError("The tag was not closed.");
                }
            }
            else if (CurrentAtom == RwHtmlAtom.Solidus)
            {
                // element closing tag
                MoveNext();
                SkipWhiteSpaceOrNewLine();
                
                var tagNameStart = DistanceFromLastToken;
                ReadText();
                var tagName = GetTextSinceLastToken().Trim().Substring(tagNameStart);
                
                SkipWhiteSpaceOrNewLine();
                if (CurrentAtom == RwHtmlAtom.CloseAngle)
                {
                    MoveNext();
                    ReturnToken(new RwCloseTagToken(tagName), DistanceFromLastToken);
                }
                else
                {
                    ThrowParserError("The tag was not closed.");
                }
            }
            else
            {
                ThrowParserError("The tag name contains invalid characters!");
            }
        }

        /// <summary>
        /// Reads the literal until end tag.
        /// </summary>
        private void ReadLiteralUntilEndTag(string tagName)
        {
            while (!IsAtEnd)
            {
                // look for the end tag
                if (CurrentAtom == RwHtmlAtom.OpenAngle)
                {
                    var endTagStart = DistanceFromLastToken;
                    var endTagStartPosition = CurrentAtomPosition;

                    MoveNext();
                    SkipWhiteSpaceOrNewLine();

                    if (CurrentAtom == RwHtmlAtom.Solidus)
                    {
                        MoveNext();
                        SkipWhiteSpaceOrNewLine();

                        var endTagNameStart = DistanceFromLastToken;
                        ReadText();
                        var endTagName = GetTextSinceLastToken().Substring(endTagNameStart);

                        if (string.Equals(tagName, endTagName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            SkipWhiteSpaceOrNewLine();

                            if (CurrentAtom == RwHtmlAtom.CloseAngle)
                            {
                                MoveNext();

                                // end tag found, return the literal and the tag
                                ReturnToken(new RwValueToken(GetTextSinceLastToken().Substring(0, endTagStart), false), endTagStart, endTagStartPosition);
                                ReturnToken(new RwCloseTagToken(endTagName), DistanceFromLastToken);
                                return;
                            }
                        }
                    }
                }

                // look for the bindings
                if (CurrentAtom == RwHtmlAtom.OpenCurlyBrace)
                {
                    var bindingStart = DistanceFromLastToken;
                    var bindingStartPosition = CurrentAtomPosition;

                    MoveNext();
                    if (CurrentAtom == RwHtmlAtom.OpenCurlyBrace)
                    {
                        var binding = ReadBinding();

                        if (CurrentAtom == RwHtmlAtom.CloseCurlyBrace)
                        {
                            MoveNext();

                            // binding found, return the literal and the binding
                            ReturnToken(new RwValueToken(GetTextSinceLastToken().Substring(0, bindingStart), false), bindingStart, bindingStartPosition);
                            ReturnToken(binding, DistanceFromLastToken);
                        }
                        else
                        {
                            ThrowParserError("The markup extension was not closed.");
                        }
                    }
                }

                MoveNext();
            }

            ThrowParserError(string.Format("The <{0}> tag was not closed!", tagName));
        }

        /// <summary>
        /// Reads the attribute.
        /// </summary>
        private bool ReadAttribute()
        {
            SkipWhiteSpaceOrNewLine();

            if (CurrentAtom != RwHtmlAtom.Text)
            {
                return false;
            }

            // read attribute name
            var nameStart = DistanceFromLastToken;
            ReadText();
            var attributeName = GetTextSinceLastToken().Substring(nameStart);
            SkipWhiteSpaceOrNewLine();
            RwHtmlToken attributeValue = null;

            if (CurrentAtom == RwHtmlAtom.Equal)
            {
                MoveNext();
                SkipWhiteSpaceOrNewLine();

                if (CurrentAtom == RwHtmlAtom.SingleQuote || CurrentAtom == RwHtmlAtom.DoubleQuote)
                {
                    // value in quotes
                    var quote = CurrentAtom;
                    MoveNext();
                    if (CurrentAtom == RwHtmlAtom.OpenCurlyBrace)
                    {
                        // binding
                        var position = CurrentAtomPosition;
                        var bindingStart = DistanceFromLastToken;
                        attributeValue = ReadBinding();
                        position.Length = DistanceFromLastToken - bindingStart;
                        attributeValue.SpanPosition = position;
                    }
                    else
                    {
                        // value
                        var valueStart = DistanceFromLastToken;
                        var position = CurrentAtomPosition;
                        SkipWhile(t => t != quote);

                        attributeValue = new RwValueToken(GetTextSinceLastToken().Substring(valueStart), false);
                        position.Length = ((RwValueToken)attributeValue).Text.Length;
                        attributeValue.SpanPosition = position;
                    }
                    MoveNext();
                }
                else
                {
                    // value without quotes
                    var position = CurrentAtomPosition;
                    var valueStart = DistanceFromLastToken;
                    ReadText();
                    attributeValue = new RwValueToken(GetTextSinceLastToken().Substring(valueStart), false);
                    position.Length = ((RwValueToken)attributeValue).Text.Length;
                    attributeValue.SpanPosition = position;
                }

                // return the attribute
                ReturnToken(new RwAttributeToken(attributeName, attributeValue), DistanceFromLastToken);
            }

            return true;
        }

        /// <summary>
        /// Reads the binding.
        /// </summary>
        private RwValueToken ReadBinding()
        {
            MoveNext();
            var exprStart = DistanceFromLastToken;
            var level = 1;
            SkipWhile(a =>
            {
                if (a == RwHtmlAtom.OpenCurlyBrace)
                {
                    level++;
                }
                if (a == RwHtmlAtom.CloseCurlyBrace)
                {
                    level--;
                }
                return level > 0 || a != RwHtmlAtom.CloseCurlyBrace;
            });

            var expression = GetTextSinceLastToken().Substring(exprStart);
            MoveNext();
            return new RwValueToken(expression, true);
        }


        /// <summary>
        /// Reads the text (e.g. element name).
        /// </summary>
        private void ReadText()
        {
            SkipWhile(a => a == RwHtmlAtom.Text || a == RwHtmlAtom.Dash);
        }

        /// <summary>
        /// Skips the white space or new line.
        /// </summary>
        private void SkipWhiteSpaceOrNewLine()
        {
            SkipWhile(a => a == RwHtmlAtom.WhiteSpace || a == RwHtmlAtom.NewLine);
        }

        /// <summary>
        /// Throws the parser error.
        /// </summary>
        private void ThrowParserError(string message)
        {
            throw new ParserException(message) { Position = CurrentAtomPosition };
        }
    }
}
