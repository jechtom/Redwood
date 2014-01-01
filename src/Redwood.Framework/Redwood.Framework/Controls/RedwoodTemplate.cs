using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Generation;

namespace Redwood.Framework.Controls
{
    public class RedwoodTemplate : RedwoodControl
    {
        

        public override void Render(IHtmlWriter writer)
        {
            RenderChildren(writer);    
        }
    }
}