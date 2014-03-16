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
        public SpanPosition AddTo(SpanPosition other)
        {
            var result = new SpanPosition();
            if (LineNumber > 1)
            {
                result.PositionOnLine += other.PositionOnLine;
            }
            else
            {
                result.PositionOnLine = PositionOnLine;
            }
            result.LineNumber = other.LineNumber + LineNumber - 1;
            result.AbsolutePosition = other.AbsolutePosition + AbsolutePosition;
            result.Length = Length;

            return result;
        }
    }
}