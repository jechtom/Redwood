using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Redwood.Framework.Binding.Parsing.Expressions;
using Redwood.Framework.Binding.Parsing.Tokens;
using Redwood.Framework.RwHtml;
using Redwood.Framework.RwHtml.Converters;
using Redwood.Framework.RwHtml.Markup;

namespace Redwood.Framework.Binding.Parsing
{
    public class BindingParser
    {

        public const string DefaultBindingType = "Binding";

        public Dictionary<string, Func<MarkupExpression>> MappingTable { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingParser"/> class.
        /// </summary>
        public BindingParser()
        {
            MappingTable = new Dictionary<string, Func<MarkupExpression>>(StringComparer.InvariantCultureIgnoreCase);
            MappingTable.Add("Binding", () => new BindingMarkupExtension());
            MappingTable.Add("Command", () => new CommandMarkupExpression());
        }


        /// <summary>
        /// Parses the expression.
        /// </summary>
        public MarkupExpression ParseExpression(string expression)
        {
            // gets the tokens
            var tokens = GetTokens(expression);

            // create binding object
            var binding = CreateBindingObject(tokens);

            // read comma-separated binding parameters
            var parameters = ReadParameters(tokens);

            // validate and resolve default parameter
            ValidateAndResolveDefaultParameter(binding, parameters);

            // set the values
            SetBindingProperties(binding, parameters);

            return binding;
        }

        /// <summary>
        /// Reads the binding parameters.
        /// </summary>
        private List<Expressions.BindingExpression> ReadParameters(List<BindingToken> tokens)
        {
            var index = 1;
            var parameters = new List<Expressions.BindingExpression>();
            while (index < tokens.Count)
            {
                if (parameters.Count > 0)
                {
                    if (!(tokens[index] is BindingCommaToken))
                    {
                        // TODO: comma expected, but the tokenizer should already handle this
                    }
                    index++;
                }

                var param = ReadExpression(tokens, ref index);
                parameters.Add(param);
            }
            return parameters;
        }

        /// <summary>
        /// Gets the binding default property.
        /// </summary>
        private string GetBindingDefaultProperty(MarkupExpression binding)
        {
            var attr = binding.GetType().GetCustomAttribute<DefaultPropertyAttribute>();
            if (attr == null) return string.Empty;
            return attr.Name;
        }

        /// <summary>
        /// Creates the binding object according to the specified binding type.
        /// </summary>
        private MarkupExpression CreateBindingObject(List<BindingToken> tokens)
        {
            if (tokens.Count == 0)
            {
                // TODO: unexpected end
            }
            var bindingType = ((BindingTypeToken)tokens[0]).BindingTypeName;
            if (!MappingTable.ContainsKey(bindingType))
            {
                // TODO: unknown binding type
            }
            return MappingTable[bindingType]();
        }

        /// <summary>
        /// Validates the parameters of the binding and resolves the default property expression.
        /// </summary>
        private void ValidateAndResolveDefaultParameter(MarkupExpression binding, List<Expressions.BindingExpression> parameters)
        {
            var defaultProperty = GetBindingDefaultProperty(binding);

            if (string.IsNullOrEmpty(defaultProperty))
            {
                if (!parameters.All(p => p is BindingParameterSetExpression))
                {
                    // TODO: no default property, all parameters must be BindingParameterSetExpression
                }
            }
            else
            {
                if (parameters.Count(p => !(p is BindingParameterSetExpression)) > 1)
                {
                    // TODO: at most one parameter can be other type than BindingParameterSetExpression
                }
                var defaultParameter = parameters.FirstOrDefault(p => !(p is BindingParameterSetExpression));
                if (defaultParameter != null)
                {
                    parameters.Remove(defaultParameter);
                    parameters.Add(new BindingParameterSetExpression() { ParameterName = defaultProperty, Value = defaultParameter });
                }
            }
        }

        /// <summary>
        /// Converts the expression to constant.
        /// </summary>
        private string ConvertExpressionToConstant(Expressions.BindingExpression expression)
        {
            if (expression is BindingConstantExpression)
            {
                return ((BindingConstantExpression)expression).Value;
            }

            if (!(expression is BindingGetPropertyExpression) || ((BindingGetPropertyExpression)expression).NextExpression != null)
            {
                // TODO: single word or quoted string is expected for the specified property type
            }

            return ((BindingGetPropertyExpression)expression).PropertyName;
        }

        /// <summary>
        /// Sets the binding properties.
        /// </summary>
        private void SetBindingProperties(MarkupExpression binding, List<Expressions.BindingExpression> parameters)
        {
            var type = binding.GetType();
            foreach (var parameter in parameters.OfType<BindingParameterSetExpression>())
            {
                // get property
                var prop = type.GetProperty(parameter.ParameterName);
                if (prop == null)
                {
                    // TODO: binding does not have property of that name
                }

                // set value
                if (prop.PropertyType == typeof (Expressions.BindingExpression))
                {
                    // value is expression
                    prop.SetValue(binding, parameter.Value);
                }
                else
                {
                    // value is constant
                    var converter = TypeConverterMapper.Default.GetConverterForType(prop.PropertyType);
                    object value;
                    if (!converter.TryConvertFromString(ConvertExpressionToConstant(parameter.Value), out value))
                    {
                        // TODO: value could not be converted to the desired type
                    }
                    prop.SetValue(binding, value);
                }
            }
        }


        /// <summary>
        /// Reads the expression.
        /// </summary>
        private Expressions.BindingExpression ReadExpression(List<BindingToken> tokens, ref int index)
        {
            if (!(tokens[index] is BindingTextToken))
            {
                // TODO: expression must start with identifier
            }
            var text = ((BindingTextToken)tokens[index]).Text;

            if (index + 1 < tokens.Count)
            {
                if (tokens[index + 1] is BindingDotToken)
                {
                    // identifier.identifier
                    index += 2;
                    return new BindingGetPropertyExpression() { PropertyName = text, NextExpression = ReadExpression(tokens, ref index) };
                }
                else if (tokens[index + 1] is BindingEqualsToken)
                {
                    // identifier = expression
                    index += 2;
                    return new BindingParameterSetExpression() { ParameterName = text, Value = ReadExpression(tokens, ref index) };
                }
                else if (tokens[index + 1] is BindingOpenBraceToken)
                {
                    // identifier(expr, expr2...)
                    index += 2;
                    var expr = new BindingCallMethodExpression() { MethodName = text };
                    while (!(tokens[index] is BindingCloseBraceToken))
                    {
                        if (expr.Arguments.Count > 0)
                        {
                            if (!(tokens[index] is BindingCommaToken))
                            {
                                // TODO: comma expected, but the tokenizer should already handle this
                            }
                            index++;
                        }

                        var param = ReadExpression(tokens, ref index);
                        expr.Arguments.Add(param);
                    }
                    index++;
                    return expr;
                }
            }

            if (index + 1 < tokens.Count && tokens[index + 1] is BindingOpenIndexerToken)
            {
                var first = ((BindingTextToken)tokens[index + 2]).Text;
                if (tokens[index + 3] is BindingCloseIndexerToken)
                {
                    // identifier[index]
                    index += 4;

                    int firstValue;
                    if (!int.TryParse(first, out firstValue))
                    {
                        // TODO: the index must be integer
                        throw new Exception();
                    }

                    return new BindingGetPropertyExpression() { PropertyName = text, Indexer = new BindingArrayGetByIndexExpression() { Index = firstValue} };
                }
                else
                {
                    // identifier[property=value]
                    var second = ((BindingTextToken)tokens[index + 4]).Text;

                    index += 6;
                    return new BindingGetPropertyExpression() { PropertyName = text, Indexer = new BindingArrayGetByKeyExpression() { KeyPropertyName = first, KeyValue = second } };
                }
            }

            index++;
            return new BindingGetPropertyExpression() { PropertyName = text };
        }

        /// <summary>
        /// Gets the tokens.
        /// </summary>
        private List<BindingToken> GetTokens(string expression)
        {
            // add binding type if it is not specified
            if (!Regex.IsMatch(expression, @"^[a-zA-Z]+\s") && !MappingTable.Keys.Contains(expression))
            {
                expression = DefaultBindingType + " " + expression;
            }

            // parse the token stream
            var tokenizer = new BindingTokenizer();
            var tokens = tokenizer.Parse(expression).ToList();
            
            // TODO: handle errors

            return tokens;
        }
    }
}
