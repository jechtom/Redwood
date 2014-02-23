using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redwood.Framework.Binding.Parsing.Expressions;

namespace Redwood.Framework.Controls
{
    public class KnockoutBindingHelper
    {
        private static readonly string[] knockoutBindingVariables = { "$root", "$parent" };

        public static bool IsKnockoutBinding(BindingMarkupExpression expr)
        {
            if(expr == null)
                return false;

            return true;
        }

        public static BindingMarkupExpression GetBindingExpressionOrNull(RedwoodProperty property, RedwoodBindable obj)
        {
            var value = obj.GetRawValue(property);
            if (value is BindingMarkupExpression)
            {
                return (BindingMarkupExpression)value;
            }

            return null;
        }


        public static bool IsKnockoutCommand(CommandMarkupExpression expr)
        {
            if (expr == null)
                return false;

            return true;
        }

        public static CommandMarkupExpression GetCommandExpressionOrNull(RedwoodProperty property, RedwoodBindable obj)
        {
            var value = obj.GetRawValue(property);
            if (value is CommandMarkupExpression)
            {
                return (CommandMarkupExpression)value;
            }

            return null;
        }

        /// <summary>
        /// Translates to knockout property.
        /// </summary>
        public static string TranslateToKnockoutProperty(RedwoodControl target, RedwoodProperty property, BindingMarkupExpression binding)
        {
            var path = binding.Path;

            // TODO: support for other than two-way modes

            var sb = new StringBuilder();
            var result = TranslateToKnockoutProperty(path, sb, allowConstants: false);
            if (result != null)
            {
                ThrowBindingContainsUnsupportedExpression();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Translates to knockout property and returns the rest of the expression if it cannot be processed.
        /// </summary>
        private static Binding.Parsing.Expressions.BindingExpression TranslateToKnockoutProperty(Binding.Parsing.Expressions.BindingExpression path, StringBuilder sb, bool allowConstants = false)
        {
            while (path != null)
            {
                if (path is BindingGetPropertyExpression)
                {
                    // get property
                    var propertyName = ((BindingGetPropertyExpression)path).PropertyName;
                    var next = ((BindingGetPropertyExpression)path).NextExpression;

                    sb.Append(propertyName);
                    if (next != null)
                    {
                        if (!knockoutBindingVariables.Contains(propertyName))
                        {
                            sb.Append(".");
                        }
                        else
                        {
                            sb.Append("().");
                        }
                    }

                    path = next;
                }
                else if (allowConstants && path is BindingConstantExpression)
                {
                    // constant
                    var isQuoted = ((BindingConstantExpression)path).IsQuoted;
                    var value = ((BindingConstantExpression)path).Value;

                    // TODO: convert value to javascript representation

                    if (isQuoted)
                    {
                        sb.Append("'");
                    }
                    sb.Append(value);
                    if (isQuoted)
                    {
                        sb.Append("'");
                    }

                    path = null;
                }
                else
                {
                    break;
                }
            }
            return path;
        }

        private static void ThrowBindingContainsUnsupportedExpression()
        {
            // TODO: more detailed error message
            throw new NotSupportedException("The binding contains unsupported elements!");
        }

        /// <summary>
        /// Translates the binding expression to the knockout command name.
        /// </summary>
        public static string TranslateToKnockoutCommand(RedwoodControl target, RedwoodProperty property, CommandMarkupExpression binding)
        {
            var path = binding.Path;

            var sb = new StringBuilder();
            TranslateToKnockoutCommand(target.DataContextPath, path, sb);
            return sb.ToString();
        }

        /// <summary>
        /// Translates to knockout command.
        /// </summary>
        private static void TranslateToKnockoutCommand(string currentDataContextPath, Binding.Parsing.Expressions.BindingExpression path, StringBuilder sb)
        {
            // generated output: Redwood.PostBack('current data context path', 'function name', [ arg1, arg2, arg3])

            sb.AppendFormat("function() {{ Redwood.PostBack('");
            sb.Append(currentDataContextPath);
            sb.Append("', '");

            // property accessors before function call
            while (path is BindingGetPropertyExpression)
            {
                var propertyName = ((BindingGetPropertyExpression)path).PropertyName;
                sb.Append(propertyName);
                sb.Append(".");

                path = ((BindingGetPropertyExpression)path).NextExpression;
            }

            if (path is BindingCallMethodExpression)
            {
                // method call
                var methodName = ((BindingCallMethodExpression)path).MethodName;
                sb.Append(methodName);
                sb.Append("', [");

                var arguments = ((BindingCallMethodExpression)path).Arguments;
                var isFirst = true;
                foreach (var argument in arguments)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }

                    var result = TranslateToKnockoutProperty(argument, sb, allowConstants: true);
                    if (result != null)
                    {
                        // TODO: function call parameter must be property get chain or constant
                    }
                }
                sb.Append("]) }");
            }
            else
            {
                // TODO: command must contain at least one function call       
            }
        }
    }
}
