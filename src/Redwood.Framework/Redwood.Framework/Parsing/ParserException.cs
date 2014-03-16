using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Parsing
{
    public class ParserException : ApplicationException
    {

        public SpanPosition Position { get; set; }

        public string FileName { get; set; }


        public ParserException(string message) : base(message)
        {
        }

        public ParserException(string message, Exception innerException) : base(message, innerException)
        {
        }


    }
}
