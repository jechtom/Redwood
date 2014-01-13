using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Parsing.RwHtml
{
    public enum RwHtmlAtom
    {
        Text,
        WhiteSpace,
        NewLine,
        OpenAngle,          // <
        CloseAngle,         // >
        SingleQuote,        // '
        DoubleQuote,        // "
        Bang,               // !
        Solidus,            // /
        Dash,               // -
        OpenCurlyBrace,     // {
        CloseCurlyBrace,    // }
        Equal               // =
    }
}