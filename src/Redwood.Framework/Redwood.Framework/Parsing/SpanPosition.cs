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

        /// <summary>
        /// Adds this position to the specified one.
        /// </summary>
        public void AddTo(SpanPosition other)
        {
            if (LineNumber > 1)
            {
                PositionOnLine += other.PositionOnLine;
            }
            LineNumber = other.LineNumber + LineNumber - 1;
            AbsolutePosition = other.AbsolutePosition + AbsolutePosition;
        }
    }
}