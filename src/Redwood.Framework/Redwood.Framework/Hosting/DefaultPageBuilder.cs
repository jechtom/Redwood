﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Redwood.Framework.Controls;
using Redwood.Framework.Parsing.RwHtml;

namespace Redwood.Framework.Hosting
{
    public class DefaultPageBuilder : IPageBuilder
    {
        public Page BuildPage(IOwinContext context, string markup)
        {
            var parser = new RwHtmlParser();
            return parser.ParsePage(markup);
        }
    }
}