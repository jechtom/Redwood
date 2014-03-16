using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Redwood.Framework.Controls;

namespace Redwood.Framework.Hosting
{
    public interface IPageBuilder
    {

        Page BuildPage(RedwoodRequestContext context, MarkupFile markupFile);

    }
}