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

                case '[':
                    token = BindingAtom.ArrayOpenBrace;
                    break;

                case ']':
                    token = BindingAtom.ArrayCloseBrace;
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
                ThrowParserError("The expression must start with identifier.");
            }
            var bindingType = ReadText();
            SkipWhiteSpaceOrNewLine();
            ReturnToken(new BindingTypeToken() { BindingTypeName = bindingType }, DistanceFromLastToken);

            // read comma-separated expressions
            var isFirst = true;
            while (!IsAtEnd)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    if (CurrentAtom != BindingAtom.Comma)
                    {
                        ThrowParserError("The parameters of markup extension must be separated by comma.");
                    }

                    MoveNext();
                    SkipWhiteSpaceOrNewLine();
                    ReturnToken(new BindingCommaToken(), DistanceFromLastToken);
                }

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
                ThrowParserError("The expression must start with identifier.");
            }
            var value = ReadText();
            SkipWhiteSpaceOrNewLine();
            ReturnToken(new BindingTextToken() { Text = value }, DistanceFromLastToken);

            var hasIndexer = false;
            if (CurrentAtom == BindingAtom.ArrayOpenBrace)
            {
                // identifier[index] or identifier[property=value]
                MoveNext();
                SkipWhiteSpaceOrNewLine();
                ReturnToken(new BindingOpenIndexerToken(), DistanceFromLastToken);

                var first = ReadText();
                SkipWhiteSpaceOrNewLine();
                ReturnToken(new BindingTextToken() { Text = first }, DistanceFromLastToken);

                if (CurrentAtom == BindingAtom.Equal)
                {
                    MoveNext();
                    SkipWhiteSpaceOrNewLine();
                    ReturnToken(new BindingEqualsToken(), DistanceFromLastToken);

                    ReturnTextTokenOrQuotedTextToken();
                }

                if (CurrentAtom != BindingAtom.ArrayCloseBrace)
                {
                    ThrowParserError("The array indexer was not closed.");
                }
                MoveNext();
                SkipWhiteSpaceOrNewLine();
                ReturnToken(new BindingCloseIndexerToken(), DistanceFromLastToken);
                
                hasIndexer = true;
            }

            if (CurrentAtom == BindingAtom.Dot)
            {
                // identifier.identifier...
                MoveNext();
                SkipWhiteSpaceOrNewLine();
                ReturnToken(new BindingDotToken(), DistanceFromLastToken);

                ReadExpression();
            }
            else if (CurrentAtom == BindingAtom.Equal && !hasIndexer)
            {
                // identifier = string or expression
                MoveNext();
                SkipWhiteSpaceOrNewLine();
                ReturnToken(new BindingEqualsToken(), DistanceFromLastToken);

                if (CurrentAtom == BindingAtom.DoubleQuotes)
                {
                    ReturnTextTokenOrQuotedTextToken();
                }
                else
                {
                    ReadExpression();
                }
            }
            else if (CurrentAtom == BindingAtom.OpenBrace && !hasIndexer)
            {
                // identifier(expr, expr...)
                MoveNext();
                SkipWhiteSpaceOrNewLine();
                ReturnToken(new BindingOpenBraceToken(), DistanceFromLastToken);

                if (CurrentAtom != BindingAtom.CloseBrace)
                {
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
                                ThrowParserError("The parameters in the function call must be separated with comma.");
                            }

                            MoveNext();
                            SkipWhiteSpaceOrNewLine();
                            ReturnToken(new BindingCommaToken(), DistanceFromLastToken);
                        }

                        ReadExpression();
                    } while (!IsAtEnd && CurrentAtom != BindingAtom.CloseBrace);

                    if (CurrentAtom != BindingAtom.CloseBrace)
                    {
                        ThrowParserError("The parameter list was not closed.");
                    }
                }
                
                MoveNext();
                SkipWhiteSpaceOrNewLine();
                ReturnToken(new BindingCloseBraceToken(), DistanceFromLastToken);
            }
        }

        /// <summary>
        /// Reads a text token or a quoted text token.
        /// </summary>
        private void ReturnTextTokenOrQuotedTextToken()
        {
            if (CurrentAtom == BindingAtom.DoubleQuotes)
            {
                MoveNext();
                var start = DistanceFromLastToken;
                SkipWhile(t => t != BindingAtom.DoubleQuotes);
                if (CurrentAtom != BindingAtom.DoubleQuotes)
                {
                    ThrowParserError("The string literal was not closed.");
                }
                var text = GetTextSinceLastToken().Substring(start);
                MoveNext();
                SkipWhiteSpaceOrNewLine();
                ReturnToken(new BindingQuotedTextToken() { Text = text }, DistanceFromLastToken);
            }
            else
            {
                var text = ReadText();
                SkipWhiteSpaceOrNewLine();
                ReturnToken(new BindingTextToken() { Text = text }, DistanceFromLastToken);
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

        /// <summary>
        /// Throws the parser error.
        /// </summary>
        private void ThrowParserError(string message)
        {
            throw new ParserException(message) { Position = CurrentAtomPosition };
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
        DoubleQuotes,
        ArrayOpenBrace,
        ArrayCloseBrace
    }
}
