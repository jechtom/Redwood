using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Generation;

namespace Redwood.Framework.RwHtml.Parsing.Tokens
{
    public class RwValueToken : RwHtmlToken
    {
        public RwValueToken(string text, bool isExpression)
        {
            Text = text;
            IsExpression = isExpression;
        }

        public string Text { get; set; }
        public bool IsExpression { get; set; }

        public string Expression
        {
            get
            {
                if (!IsExpression)
                    throw new InvalidOperationException("This property is not available for instances with IsExpression = false.");
                return Text;
            }
        }
    }
}