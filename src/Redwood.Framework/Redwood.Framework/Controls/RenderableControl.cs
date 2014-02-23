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
            // set data context path if there is a binding on data context
            var dataContextBinding = GetRawValue(DataContextProperty, true) as BindingMarkupExpression;
            if (dataContextBinding != null)
            {
                var path = GetRawValue(DataContextPathProperty);
                if (path == null)
                {
                    DataContextPath = DataContextPathBuilder.AppendPropertyPath(DataContextPath, dataContextBinding.Path);
                }
            }

            // write
            RenderControl(writer);
        }

        /// <summary>
        /// Renders the specified writer.
        /// </summary>
        protected abstract void RenderControl(IHtmlWriter writer);
    }
}
