﻿using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Parsing.RwHtml.Tokens;

namespace Redwood.Framework.Parsing.RwHtml
{
    public class RwHtmlTokenizer : BaseTokenizer<RwHtmlAtom, RwHtmlToken>
    {

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

                case '!': 
                    token = RwHtmlAtom.Bang;
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
                        ReturnToken(new RwLiteralToken() { Text = GetTextSinceLastToken() }, DistanceFromLastToken);
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
                    if (lastTextPosition == null)
                    {
                        lastTextPosition = CurrentAtomPosition;
                    }
                    MoveNext();
                }
            }

            if (lastTextPosition != null)
            {
                // return the remaining text content on the end of the buffer
                ReturnToken(new RwLiteralToken() { Text = GetTextSinceLastToken() }, DistanceFromLastToken);
            }
        }
        
        /// <summary>
        /// Reads the element.
        /// </summary>
        private void ReadElement()
        {
            SkipWhiteSpaceOrNewLine();
            MoveNext();
            if (CurrentAtom == RwHtmlAtom.Text)
            {
                // element opening tag
                var nameStart = DistanceFromLastToken;
                ReadText();
                var tagName = GetTextSinceLastToken().Trim().Substring(nameStart);
                ReturnToken(new RwOpenTagToken(tagName, TagType.StandardTag), DistanceFromLastToken);

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
                        ReturnToken(new RwCloseTagToken(tagName, true), DistanceFromLastToken);
                    }
                    else
                    {
                        // TODO: solidus without close angle
                    }
                }
                else if (CurrentAtom == RwHtmlAtom.CloseAngle)
                {
                    MoveNext();
                    ReturnToken(new RwOpenTagToken(tagName, TagType.BeginTagCloseAngle), DistanceFromLastToken);
                }
                else
                {
                    // TODO: close angle expected
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
                    ReturnToken(new RwCloseTagToken(tagName, false), DistanceFromLastToken);
                }
                else
                {
                    // TODO: invalid char after tag name in the closing tag
                }
            }
            else
            {
                // TODO: invalid char after open angle
            }
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
                        var position = GetSpanPosition();
                        var bindingStart = DistanceFromLastToken;
                        attributeValue = ReadBinding();
                        position.Length = DistanceFromLastToken - bindingStart;
                        attributeValue.SpanPosition = position;
                    }
                    else
                    {
                        // value
                        var valueStart = DistanceFromLastToken;
                        var position = GetSpanPosition();
                        SkipWhile(t => t != quote);

                        attributeValue = new RwLiteralToken() { Text = GetTextSinceLastToken().Substring(valueStart) };
                        position.Length = ((RwLiteralToken)attributeValue).Text.Length;
                        attributeValue.SpanPosition = position;
                    }
                    MoveNext();
                }
                else
                {
                    // value without quotes
                    var position = GetSpanPosition();
                    var valueStart = DistanceFromLastToken;
                    ReadText();
                    attributeValue = new RwLiteralToken() { Text = GetTextSinceLastToken().Substring(valueStart) };
                    position.Length = ((RwLiteralToken)attributeValue).Text.Length;
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
        private RwBindingToken ReadBinding()
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
            return new RwBindingToken(expression);
        }


        /// <summary>
        /// Reads the text (e.g. element name).
        /// </summary>
        private void ReadText()
        {
            SkipWhile(a => a == RwHtmlAtom.Text);
        }

        /// <summary>
        /// Skips the white space or new line.
        /// </summary>
        private void SkipWhiteSpaceOrNewLine()
        {
            SkipWhile(a => a == RwHtmlAtom.WhiteSpace || a == RwHtmlAtom.NewLine);
        }
    }
}
