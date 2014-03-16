using Redwood.Framework.Binding;
using Redwood.Framework.Generation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Controls
{
    public abstract class RenderableControl : RedwoodControl, IRenderable
    {

        /// <summary>
        /// Renders the specified writer.
        /// </summary>
        public void Render(IHtmlWriter writer)
        {
            RenderControl(writer);
        }

        /// <summary>
        /// Renders the specified writer.
        /// </summary>
        protected abstract void RenderControl(IHtmlWriter writer);
    }
}
