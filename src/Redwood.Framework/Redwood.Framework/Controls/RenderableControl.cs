using Redwood.Framework.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Controls
{
    public abstract class RenderableControl : RedwoodControl, IRenderable
    {
        /// <summary>
        /// Renders the specified writer.
        /// </summary>
        public abstract void Render(IHtmlWriter writer);
    }
}
