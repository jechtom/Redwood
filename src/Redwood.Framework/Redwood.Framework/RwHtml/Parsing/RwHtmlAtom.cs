using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.RwHtml.Parsing
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
        Solidus,            // /
        Dash,               // -
        OpenCurlyBrace,     // {
        CloseCurlyBrace,    // }
        Equal,              // =
        ExclamationMark,    // !
        QuestionMark        // ?
    }
}