using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public BindingContext DataContext { get; internal set; }

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

        /// <summary>
        /// Renders the children.
        /// </summary>
        protected void RenderChildren(Generation.IHtmlWriter writer)
        {
            foreach (var control in Controls)
            {
                control.Render(writer);
            }
        }

        /// <summary>
        /// Translates to knockout property.
        /// </summary>
        protected string TranslateToKnockoutProperty(string propertyPath)
        {
            return propertyPath.Replace(".", ".()");
        }

        /// <summary>
        /// Translates the binding expression to the knockout command name.
        /// </summary>
        protected string TranslateToKnockoutCommand(string commandPath)
        {
            var match = Regex.Match(commandPath, @"^([^\(\)]+)(\([^\(\)]+\))?$");
            
            var functionName = match.Groups[1].Value.Trim();
            string arguments;
            if (match.Groups[2].Captures.Count > 0)
            {
                arguments = match.Groups[2].Captures[0].Value.Substring(1, match.Groups[2].Captures[0].Length - 2);
            }
            else
            {
                arguments = "";
            }

            return string.Format("function() {{ {0}($element, [{1}]); }}", functionName, arguments);
        }
    }
}
