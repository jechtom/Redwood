using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Redwood.Framework.Controls
{
    public class KnockoutBindingHelper
    {
        public static bool IsKnockoutBinding(BindingBase expr)
        {
            if(expr == null)
                return false;

            return true;
        }

        public static BindingBase GetExpressionOrNull(RedwoodProperty property, RedwoodBindable obj)
        {
            var value = obj.GetRawValue(property);
            if (value is BindingBase)
            {
                return (BindingBase)value;
            }

            return null;
        }


        /// <summary>
        /// Translates to knockout property.
        /// </summary>
        public static string TranslateToKnockoutProperty(string propertyPath)
        {
            return propertyPath.Replace(".", ".()");
        }

        /// <summary>
        /// Translates the binding expression to the knockout command name.
        /// </summary>
        public static string TranslateToKnockoutCommand(string commandPath)
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
