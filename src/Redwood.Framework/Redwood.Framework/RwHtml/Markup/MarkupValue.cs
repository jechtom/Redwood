﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwood.Framework.RwHtml.Markup
{
    public class MarkupValue
    {
        public MarkupValue(string value, bool isExpression)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            this.IsExpression = IsExpression;
            this.Value = value;
        }

        public bool IsExpression { get; private set; }
        public string Value { get; private set; }
    }
}
