using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Redwood.Framework.Controls;
using Redwood.Framework.RwHtml.Parsing;

namespace Redwood.Framework.Hosting
{
    public class DefaultPageBuilder : IPageBuilder
    {
        public Page BuildPage(IOwinContext context, string markup)
        {
            throw new NotImplementedException();
        }
    }
}