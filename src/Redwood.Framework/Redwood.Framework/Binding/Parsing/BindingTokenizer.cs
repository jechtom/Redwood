using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Binding.Parsing.Tokens;
using Redwood.Framework.Parsing;

namespace Redwood.Framework.Binding.Parsing
{
    public class BindingTokenizer : BaseTokenizer<BindingAtom, BindingToken>
    {
        protected override bool IsNewLine(BindingAtom atom)
        {
            return atom == BindingAtom.NewLine;
        }

        protected override BindingAtom ReadAtomCore()
        {
            var token = BindingAtom.Text;
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
                return BindingAtom.NewLine;
            }

            if (currentChar == '\n')
            {
                // new line
                token = BindingAtom.NewLine;
            }
            else if (Char.IsWhiteSpace(currentChar))
            {
                // white space
                token = BindingAtom.WhiteSpace;
            }

            // other chars
            switch (currentChar)
            {
                case '(':
                    token = BindingAtom.OpenBrace;
                    break;

                case ')':
                    token = BindingAtom.CloseBrace;
                    break;

                case '.':
                    token = BindingAtom.Dot;
                    break;

                case ',':
                    token = BindingAtom.Comma;
                    break;

                case '=':
                    token = BindingAtom.Equal;
                    break;

                case '"':
                    token = BindingAtom.DoubleQuotes;
                    break;
            }

            Read();
            return token;
        }

        /// <summary>
        /// Parses the document.
        /// </summary>
        protected override void ParseDocument()
        {
            SkipWhiteSpaceOrNewLine();
            if (CurrentAtom != BindingAtom.Text)
            {
                // TODO: invalid character, identifier expected
            }
            var bindingType = ReadText();
            SkipWhiteSpaceOrNewLine();
            ReturnToken(new BindingTypeToken() { BindingTypeName = bindingType }, DistanceFromLastToken);

            // read comma-separated expressions
            while (!IsAtEnd)
            {
                ReadExpression();
            }
        }

        /// <summary>
        /// Reads the expression.
        /// </summary>
        private void ReadExpression()
        {
            if (CurrentAtom != BindingAtom.Text)
            {
                // TODO: expression must start with text
            }
            var value = ReadText();
            SkipWhiteSpaceOrNewLine();
            ReturnToken(new BindingTextToken() { Text = value }, DistanceFromLastToken);

            if (CurrentAtom == BindingAtom.Dot)
            {
                // identifier.identifier...
                MoveNext();
                SkipWhiteSpaceOrNewLine();
                ReturnToken(new BindingDotToken(), DistanceFromLastToken);

                ReadExpression();
            }
            else if (CurrentAtom == BindingAtom.Equal)
            {
                // identifier = text/string
                MoveNext();
                SkipWhiteSpaceOrNewLine();
                ReturnToken(new BindingEqualsToken(), DistanceFromLastToken);

                if (CurrentAtom == BindingAtom.Text)
                {
                    var text = ReadText();
                    SkipWhiteSpaceOrNewLine();
                    ReturnToken(new BindingTextToken() { Text = text }, DistanceFromLastToken);
                }
                else if (CurrentAtom == BindingAtom.DoubleQuotes)
                {
                    MoveNext();
                    var start = DistanceFromLastToken;
                    SkipWhile(t => t != BindingAtom.DoubleQuotes);
                    if (CurrentAtom != BindingAtom.DoubleQuotes)
                    {
                        // TODO: double quotes expected
                    }
                    var text = GetTextSinceLastToken().Substring(start);
                    MoveNext();
                    SkipWhiteSpaceOrNewLine();
                    ReturnToken(new BindingQuotedTextToken() { Text = text }, DistanceFromLastToken);
                }
                else
                {
                    // TODO: text or double quotes expected
                }
            }
            else if (CurrentAtom == BindingAtom.OpenBrace)
            {
                // identifier(expr, expr...)
                MoveNext();
                SkipWhiteSpaceOrNewLine();
                ReturnToken(new BindingOpenBraceToken(), DistanceFromLastToken);

                var isFirst = true;
                do
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else 
                    {
                        if (CurrentAtom != BindingAtom.Comma)
                        {
                            // TODO: comma or close brace expected
                        }

                        MoveNext();
                        SkipWhiteSpaceOrNewLine();
                        ReturnToken(new BindingCommaToken(), DistanceFromLastToken);
                    }

                    ReadExpression();
                }
                while (!IsAtEnd && CurrentAtom != BindingAtom.CloseBrace);

                if (CurrentAtom != BindingAtom.CloseBrace)
                {
                    // TODO: close brace expected
                }
                MoveNext();
                SkipWhiteSpaceOrNewLine();
                ReturnToken(new BindingCloseBraceToken(), DistanceFromLastToken);
            }
        }




        /// <summary>
        /// Reads the text (e.g. identifier name).
        /// </summary>
        private string ReadText()
        {
            var textStart = DistanceFromLastToken;
            SkipWhile(a => a == BindingAtom.Text);
            return GetTextSinceLastToken().Substring(textStart);
        }

        /// <summary>
        /// Skips the white space or new line.
        /// </summary>
        private void SkipWhiteSpaceOrNewLine()
        {
            SkipWhile(a => a == BindingAtom.WhiteSpace || a == BindingAtom.NewLine);
        }
    }

    public enum BindingAtom
    {
        Text,
        WhiteSpace,
        NewLine,
        Dot,
        Comma,
        OpenBrace,
        CloseBrace,
        Equal,
        DoubleQuotes
    }
}
