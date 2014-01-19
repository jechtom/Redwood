using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Parsing
{
    public interface IToken
    {

        SpanPosition SpanPosition { get; set; } 

    }
}
