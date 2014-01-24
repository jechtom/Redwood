using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;

namespace Redwood.Framework.Parsing
{
    public abstract class BaseTokenizer<TAtom, TToken> where TToken : IToken
    {
        private ITextReader reader;

        private List<TToken> tokens = new List<TToken>();
        private StringBuilder textSinceLastToken = new StringBuilder();
        private SpanPosition lastTokenSpanPosition;
        private bool isAtEnd = false;



        protected SpanPosition CurrentAtomPosition { get; private set; }
        
        protected TAtom CurrentAtom { get; private set; }

        protected int CurrentLineNumber { get; private set; }

        protected int CurrentPosition
        {
            get { return reader.Position; }
        }

        protected int PositionOnLine { get; private set; }



        /// <summary>
        /// Gets a value indicating whether we are at end of the input.
        /// </summary>
        protected bool IsAtEnd
        {
            get { return isAtEnd; }
        }

        /// <summary>
        /// Gets the current character.
        /// </summary>
        protected char Peek()
        {
            if (IsAtEnd) return '\0';
            return (char)reader.Peek();
        }

        /// <summary>
        /// Reads the next character.
        /// </summary>
        protected char Read()
        {
            if (reader.IsAtEnd)
            {
                isAtEnd = true;
                return '\0';
            }

            var value = (char)reader.Read();
            textSinceLastToken.Append(value);
            return value;
        }

        /// <summary>
        /// Determines whether the specified atom represents a new line.
        /// </summary>
        protected abstract bool IsNewLine(TAtom atom);

        /// <summary>
        /// Reads the token.
        /// </summary>
        protected abstract TAtom ReadAtomCore();

        /// <summary>
        /// Returns the token to the output.
        /// </summary>
        protected void ReturnToken(TToken token, int tokenLength, SpanPosition? nextTokenSpanPosition = null)
        {
            lastTokenSpanPosition.Length = tokenLength;
            token.SpanPosition = lastTokenSpanPosition;
            
            tokens.Add(token);

            textSinceLastToken.Remove(0, tokenLength);
            lastTokenSpanPosition = nextTokenSpanPosition ?? CurrentAtomPosition;
        }

        /// <summary>
        /// Moves to the next atom.
        /// </summary>
        protected void MoveNext()
        {
            CurrentAtomPosition = GetSpanPosition();
            if (IsAtEnd)
            {
                return;
            }

            CurrentAtom = ReadAtomCore();
            if (IsNewLine(CurrentAtom))
            {
                CurrentLineNumber++;
                PositionOnLine = 0;
            }
        }

        /// <summary>
        /// Gets the span position.
        /// </summary>
        protected SpanPosition GetSpanPosition()
        {
            return new SpanPosition() { AbsolutePosition = reader.Position, LineNumber = CurrentLineNumber, PositionOnLine = PositionOnLine };
        }

        /// <summary>
        /// Parses the specified text.
        /// </summary>
        public IEnumerable<TToken> Parse(string text)
        {
            return Parse(new StringTextReader(text));
        }

        /// <summary>
        /// Parses the document.
        /// </summary>
        public IEnumerable<TToken> Parse(ITextReader reader)
        {
            this.reader = reader;
            this.reader.Position = 0;
            lastTokenSpanPosition = new SpanPosition();
            textSinceLastToken.Clear();
            isAtEnd = false;

            CurrentLineNumber = 0;
            PositionOnLine = 0;

            tokens = new List<TToken>();
            ParseDocument();
            return tokens;
        }

        /// <summary>
        /// Parses the document.
        /// </summary>
        protected abstract void ParseDocument();


        /// <summary>
        /// Skips the while.
        /// </summary>
        protected void SkipWhile(Func<TAtom, bool> predicate)
        {
            while (!IsAtEnd && predicate(CurrentAtom))
            {
                MoveNext();
            }
        }

        /// <summary>
        /// Gets the text since last token.
        /// </summary>
        protected string GetTextSinceLastToken()
        {
            return textSinceLastToken.ToString(0, DistanceFromLastToken);
        }

        /// <summary>
        /// Gets the distance from last token.
        /// </summary>
        protected int DistanceFromLastToken
        {
            get
            {
                return CurrentAtomPosition.AbsolutePosition - (lastTokenSpanPosition.AbsolutePosition + lastTokenSpanPosition.Length);
            }
        }
    }
}
