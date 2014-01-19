using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Parsing
{
    public class StringTextReader : ITextReader
    {
        private readonly string input;
        private int position;

        public StringTextReader(string input)
        {
            this.input = input;
        }

        public bool IsAtEnd
        {
            get { return Position >= input.Length; }
        }

        public int Read()
        {
            if (IsAtEnd)
            {
                return -1;
            }
            return input[Position++];
        }

        public int Peek()
        {
            if (IsAtEnd)
            {
                return -1;
            }
            return input[Position];
        }

        public int Length
        {
            get { return input.Length; }
        }

        public int Position
        {
            get { return position; }
            set
            {
                if (value < 0 || value > input.Length)
                {
                    throw new IndexOutOfRangeException();
                }
                position = value;
            }
        }
    }
}