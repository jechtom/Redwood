using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redwood.Framework.Generation;

namespace Redwood.Framework.Controls
{
    public abstract class RedwoodControl
    {

        /// <summary>
        /// Gets the collection of bindings.
        /// </summary>
        public AttributeList<Binding> Bindings { get; private set; }

        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        public IBindingContext DataContext { get; internal set; }

        /// <summary>
        /// Gets the controls.
        /// </summary>
        public List<RedwoodControl> Controls { get; private set; } 

        /// <summary>
        /// Renders the specified writer.
        /// </summary>
        public abstract void Render(IHtmlWriter writer);


        /// <summary>
        /// Initializes a new instance of the <see cref="RedwoodControl"/> class.
        /// </summary>
        public RedwoodControl()
        {
            Bindings = new AttributeList<Binding>();
            Controls = new List<RedwoodControl>();
        }


        /// <summary>
        /// Loads the state from data context.
        /// </summary>
        internal void LoadStateFromDataContext()
        {
            foreach (var binding in Bindings)
            {
                // get binding value and set it to the property
                var value = binding.Value.Evaluate(DataContext.ViewModel);
                GetType().GetProperty(binding.Key).SetValue(this, value, null);
            }
        }

        /// <summary>
        /// Determines whether the control has client side binding on the specified property name.
        /// </summary>
        protected bool HasClientSideBinding(string propertyName)
        {
            return Bindings.ContainsKey(propertyName) && !Bindings[propertyName].RenderOnServer;
        }
    }
}
