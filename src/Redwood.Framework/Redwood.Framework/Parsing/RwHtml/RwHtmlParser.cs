using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Redwood.Framework.Controls;

namespace Redwood.Framework.Parsing.RwHtml
{
    public class RwHtmlParser
    {

        /// <summary>
        /// Parses the page.
        /// </summary>
        public Page ParsePage(string html)
        {
            // read the page
            var tokenizer = new RwHtmlTokenizer();
            var tokens = tokenizer.GetTokens(html).ToList();

            // build control structure
            var controls = new Stack<RedwoodControl>();
            var page = new Page();
            controls.Push(page);
            var index = BuildControls(controls, tokens, 0);
            if (index != tokens.Count)
            {
                throw new Exception("The closing tag does not have matching opening tag!");
            }
            return page;
        }

        /// <summary>
        /// Builds the controls.
        /// </summary>
        private int BuildControls(Stack<RedwoodControl> controls, List<RwHtmlToken> tokens, int startPosition)
        {
            var currentControl = controls.Peek();
            var beginHierarchyLevel = controls.Count;

            for (var i = startPosition; i < tokens.Count; i++)
            {
                var currentToken = tokens[i];

                // skip empty tokens
                if (currentToken is RwLiteralToken && string.IsNullOrWhiteSpace(((RwLiteralToken)currentToken).Text))
                {
                    continue;
                }

                // literal
                if (currentToken is RwLiteralToken)
                {
                    currentControl.Controls.Add(new Literal(((RwLiteralToken)currentToken).Text));
                }

                // control start
                if (currentToken is RwControlToken)
                {
                    if (!IsPropertyName(((RwControlToken)currentToken).TagPrefix, ((RwControlToken)currentToken).TagName))
                    {
                        // create control
                        var type = ResolveControlType(((RwControlToken)currentToken).TagPrefix, ((RwControlToken)currentToken).TagName);
                        var control = CreateControl(type, (RwControlToken)currentToken);
                        currentControl.Controls.Add(control);
                        controls.Push(control);

                        // build its internal structure
                        i = BuildControls(controls, tokens, i + 1);
                    }
                    else
                    {
                        // it must be property name of the last control
                        var property = ResolveProperty(((RwControlToken)currentToken).TagPrefix, ((RwControlToken)currentToken).TagName);
                        if (property.PropertyType == typeof (RedwoodTemplate))
                        {
                            // parse the template
                            var propValue = new RedwoodTemplate();
                            var propStack = new Stack<RedwoodControl>();
                            propStack.Push(propValue);
                            i = BuildControls(propStack, tokens, i + 1);

                            // set the template to the control
                            property.SetValue(currentControl, propValue);
                        }
                        else
                        {
                            throw new Exception("Inner property must be of type RedwoodTemplate!");
                        }
                    }
                }

                // control end
                if (currentToken is RwControlClosingToken)
                {
                    if (controls.Count == beginHierarchyLevel)
                    {
                        // end reading, we cannot go outside the scope we have started
                        controls.Pop();
                        return i;
                    }
                    else
                    {
                        // remove current control
                        var type = ResolveControlType(((RwControlClosingToken)currentToken).TagPrefix, ((RwControlClosingToken)currentToken).TagName);
                        if (currentControl.GetType() != type)
                        {
                            throw new Exception("The closing tag does not match with the opening tag!");
                        }
                        controls.Pop();
                    }
                }
            }

            if (controls.Count != beginHierarchyLevel)
            {
                throw new Exception("Some tags are not closed!");
            }

            return tokens.Count;
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
        private RedwoodControl CreateControl(Type type, RwControlToken controlToken)
        {
            var control = (RedwoodControl)Activator.CreateInstance(type);
            
            // set attributes
            foreach (var attribute in controlToken.Attributes)
            {
                if (attribute.Value.StartsWith("{{") && attribute.Value.EndsWith("}}"))
                {
                    // parse binding
                    control.Bindings.Add(attribute.Key, ParseBinding(attribute.Value));
                }
                else
                {
                    // set attribute value
                    var prop = type.GetProperty(attribute.Key);
                    var value = Binding.ConvertValue(attribute.Value, prop.PropertyType);
                    prop.SetValue(control, value);
                }
            }

            return control;
        }

        /// <summary>
        /// Parses the binding.
        /// </summary>
        public static Binding ParseBinding(string bindingValue)
        {
            bindingValue = bindingValue.Substring(2, bindingValue.Length - 4);

            var parts = bindingValue.Split(',').Select(p => p.Trim()).ToArray();
            var binding = new Binding(parts[0]);

            for (var i = 1; i < parts.Length; i++)
            {
                var sides = parts[i].Split('=').Select(p => p.Trim()).ToArray();
                if (string.IsNullOrEmpty(sides[0]))
                {
                    throw new Exception("Binding attribute name must not be empty!");
                }

                binding.Attributes.Add(sides[0], sides[1]);
            }

            return binding;
        }
    }
}
