using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Parsing
{
    public interface ITextReader
    {

        bool IsAtEnd { get; }

        int Read();

        int Peek();

        int Length { get; }

        int Position { get; set; }

    }
}