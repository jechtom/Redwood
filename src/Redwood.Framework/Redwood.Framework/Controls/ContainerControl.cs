using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Controls
{
    public class ContainerControl : RenderableControl
    {
        List<RedwoodControl> controls;
        IReadOnlyList<RedwoodControl> controlsReadOnly;

        public ContainerControl()
        {
            controls = new List<RedwoodControl>();
            controlsReadOnly = controls.AsReadOnly();
        }

        /// <summary>
        /// Gets the controls.
        /// </summary>
        public IReadOnlyList<RedwoodControl> Controls
        {
            get
            {
                return controlsReadOnly;
            }
        }

        public void AddControl(RedwoodControl control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            controls.Add(control);
            OnChildAdded(control);
        }

        private void OnChildAdded(RedwoodControl control)
        {
            control.OnAddedToParent(this);
        }

        public override void Render(Generation.IHtmlWriter writer)
        {
            RenderChildren(writer);
        }

        /// <summary>
        /// Renders the children.
        /// </summary>
        protected void RenderChildren(Generation.IHtmlWriter writer)
        {
            foreach (var control in Controls.OfType<RenderableControl>())
            {
                control.Render(writer);
            }
        }
    }
}
