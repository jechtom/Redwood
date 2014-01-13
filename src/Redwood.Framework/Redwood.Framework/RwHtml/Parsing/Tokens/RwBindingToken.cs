using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.RwHtml.Parsing.Tokens
{
    public class RwBindingToken : RwHtmlToken
    {

        public string Expression { get; set; }

        public RwBindingToken(string expression)
        {
            Expression = expression;
        }
    }
}