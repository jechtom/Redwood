using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Redwood.Framework.Controls;
using Redwood.Framework.Binding;
using Redwood.Framework.Parsing.RwHtml.Tokens;

namespace Redwood.Framework.Parsing.RwHtml
{
    public class RwHtmlParser
    {
        /// <summary>
        /// Parses the page.
        /// </summary>
        public Page ParsePage(string html)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Builds the controls.
        /// </summary>
        private int BuildControls(Stack<RedwoodControl> controls, List<RwHtmlToken> tokens, int startPosition)
        {
            throw new NotImplementedException();
        }

        private void AddContentIfSupported(RedwoodControl currentControl, RedwoodControl control)
        {
            // TODO create default content property instead of relying on ContainerControl
            if (currentControl is ContainerControl)
            {
                ((ContainerControl)currentControl).AddControl(control);
            }
            else
            {
                throw new NotSupportedException("Parent control does not supports content.");
            }
        }

        /// <summary>
        /// Resolves the property.
        /// </summary>
        private PropertyInfo ResolveProperty(string tagPrefix, string tagName)
        {
            if (!IsPropertyName(tagPrefix, tagName))
            {
                throw new Exception("Invalid format of property name!");
            }
            var parts = tagName.Split('.');
            var type = ResolveControlType(tagPrefix, parts[0]);
            var prop = type.GetProperty(parts[1]);
            if (prop == null)
            {
                throw new Exception("The specified property does not exist!");
            }
            return prop;
        }

        /// <summary>
        /// Determines whether the specified tag prefix and name has correct format.
        /// </summary>
        private bool IsPropertyName(string tagPrefix, string tagName)
        {
            if (tagPrefix != "rw")
            {
                throw new Exception("Invalid tag prefix!");
            }
            var parts = tagName.Split('.');
            return (parts.Length == 2 && IsValidName(parts[0]) && IsValidName(parts[1]));
        }

        /// <summary>
        /// Determines whether the specified string is a valid control or property name.
        /// </summary>
        private static bool IsValidName(string tagName)
        {
            return Regex.IsMatch(tagName, @"^[a-zA-Z][a-zA-Z0-9]*$");
        }


        /// <summary>
        /// Resolves the control.
        /// </summary>
        private Type ResolveControlType(string tagPrefix, string tagName)
        {
            if (tagPrefix != "rw")
            {
                throw new Exception("Invalid tag prefix!");
            }
            if (!IsValidName(tagName))
            {
                throw new Exception("Invalid tag name!");
            }
            return Type.GetType("Redwood.Framework.Controls." + tagName + ", Redwood.Framework", true);
        }

        /// <summary>
        /// Creates the control.
        /// </summary>
        private RedwoodControl CreateControl(Type type, RwOpenTagToken controlToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses the binding.
        /// </summary>
        public static BindingBase ParseBinding(RedwoodProperty property, string bindingValue)
        {
            bindingValue = bindingValue.Substring(2, bindingValue.Length - 4);

            var parts = bindingValue.Split(',').Select(p => p.Trim()).ToArray();
            throw new NotImplementedException();
            BindingBase binding = null;//new BindingBase(parts[0], BindingMode.TwoWay);

            for (var i = 1; i < parts.Length; i++)
            {
                var sides = parts[i].Split('=').Select(p => p.Trim()).ToArray();
                if (string.IsNullOrEmpty(sides[0]))
                {
                    throw new Exception("Binding attribute name must not be empty!");
                }

                throw new NotImplementedException();
                //binding.Attributes.Add(sides[0], sides[1]);
            }

            return binding;
        }
    }
}
