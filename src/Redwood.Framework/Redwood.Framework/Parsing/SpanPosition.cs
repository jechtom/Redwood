using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Parsing
{
    public struct SpanPosition
    {
        public int LineNumber { get; set; }

        public int AbsolutePosition { get; set; }

        public int PositionOnLine { get; set; }

        public int Length { get; set; }
    }
}