using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Parsing.RwHtml
{
    public class RwHtmlTokenizer
    {

        /// <summary>
        /// Gets the tokens.
        /// </summary>
        public IEnumerable<RwHtmlToken> GetTokens(string html)
        {
            var state = 0;
            var lastPosition = 0;
            RwControlToken controlToken = null;

            // go through the string
            for (var i = 0; i < html.Length; i++)
            {
                switch (state)
                {
                    case 0:
                        // in text
                        if (html[i] == '<')
                        {
                            if (i + 1 < html.Length && html[i + 1] == '/')
                            {
                                // closing tag
                                string tagPrefix, tagName;
                                if (StartsWithServerTagName(html, i + 2, out tagPrefix, out tagName))
                                {
                                    if (i > lastPosition)
                                    {
                                        yield return new RwLiteralToken()
                                        {
                                            Text = html.Substring(lastPosition, i - lastPosition)
                                        };
                                        lastPosition = i;
                                    }

                                    yield return new RwControlClosingToken(tagPrefix, tagName);
                                    lastPosition += tagPrefix.Length + tagName.Length + 4;
                                }
                            }
                            else
                            {
                                // opening tag
                                string tagPrefix, tagName;
                                if (StartsWithServerTagName(html, i + 1, out tagPrefix, out tagName))
                                {
                                    if (i > lastPosition)
                                    {
                                        yield return new RwLiteralToken()
                                        {
                                            Text = html.Substring(lastPosition, i - lastPosition)
                                        };
                                        lastPosition = i;
                                    }
                                    controlToken = new RwControlToken(tagPrefix, tagName);
                                    state = 1;
                                    i += tagPrefix.Length + tagName.Length + 1;
                                }
                            }
                        }
                        else if (html[i] == '{' && i + 1 < html.Length && html[i + 1] == '{')
                        {
                            // binding literal {{something}}
                            var binding = ReadBinding(html, i + 2);
                            if (binding != null)
                            {
                                if (i > lastPosition)
                                {
                                    yield return new RwLiteralToken()
                                    {
                                        Text = html.Substring(lastPosition, i - lastPosition)
                                    };
                                    lastPosition = i;
                                }

                                var bindingToken = new RwControlToken("rw", "Literal");
                                bindingToken.Attributes["Text"] = "{{" + binding + "}}";
                                yield return bindingToken;
                                yield return new RwControlClosingToken("rw", "Literal");
                                lastPosition += binding.Length + 4;
                            }
                        }
                        break;

                    case 1:
                        // begin or self closing tag
                        if (html[i] == '/' && i + 1 < html.Length && html[i + 1] == '>')
                        {
                            // tag is closed immediately
                            state = 0;
                            lastPosition = i + 2;
                            yield return controlToken;
                            yield return new RwControlClosingToken(controlToken.TagPrefix, controlToken.TagName);
                            controlToken = null;
                        }
                        else if (html[i] == '>')
                        {
                            // begin tag is closed
                            state = 0;
                            lastPosition = i + 1;
                            yield return controlToken;
                            controlToken = null;
                        }
                        else if (Char.IsWhiteSpace(html[i]))
                        {
                            // skip whitespace
                        }
                        else if (char.IsLetterOrDigit(html[i]))
                        {
                            // parse attribute name
                            var name = ReadAttributeName(html, i);
                            i += name.Length;

                            // skip whitespace
                            i = SkipWhiteSpace(html, i);
                            if (html[i] != '=')
                            {
                                throw new Exception("The equal char was expected!");
                            }
                            i++;

                            // parse attribute value
                            string value;
                            i = SkipWhiteSpace(html, i);
                            if (html[i] == '"')
                            {
                                i++;
                                value = ReadUntilChar(html, i, '"');
                                i += value.Length;
                            }
                            else if (html[i] == '\'')
                            {
                                i++;
                                value = ReadUntilChar(html, i, '\'');
                                i += value.Length;
                            }
                            else
                            {
                                throw new Exception("The apostrophe or double quote char was expected!");
                            }

                            controlToken.Attributes[name] = value;
                        }
                        else
                        {
                            throw new Exception("Invalid char in the element!");
                        }
                        break;
                }
            }

            if (lastPosition < html.Length)
            {
                yield return new RwLiteralToken()
                {
                    Text = html.Substring(lastPosition)
                };
            }
        }

        /// <summary>
        /// Reads the binding.
        /// </summary>
        private string ReadBinding(string html, int position)
        {
            var bindingEnd = html.IndexOf("}}", position);
            if (bindingEnd >= 0)
            {
                return html.Substring(position, bindingEnd - position);
            }
            return null;
        }

        /// <summary>
        /// Reads until a specified character is found.
        /// </summary>
        private string ReadUntilChar(string html, int position, char stopChar)
        {
            for (var i = position; i < html.Length; i++)
            {
                if (html[i] == stopChar)
                {
                    return html.Substring(position, i - position);
                }
            }
            throw new Exception("The attribute value was not closed!");
        }

        /// <summary>
        /// Skips the white space.
        /// </summary>
        private int SkipWhiteSpace(string html, int position)
        {
            for (var i = position; i < html.Length; i++)
            {
                if (!Char.IsWhiteSpace(html[i]))
                {
                    return i;
                }
            }
            throw new Exception("The tag was not closed!");
        }

        /// <summary>
        /// Reads the attribute name.
        /// </summary>
        private string ReadAttributeName(string html, int position)
        {
            int i;
            for (i = position; i < html.Length; i++)
            {
                if (!Char.IsLetterOrDigit(html[i]))
                {
                    break;
                }
            }
            if (i == position)
            {
                throw new Exception("Identifier expected!");
            }
            return html.Substring(position, i - position);
        }

        /// <summary>
        /// Determines whether the server tag (format tagPrefix:tagName) is on the specified position in the string.
        /// </summary>
        public static bool StartsWithServerTagName(string html, int position, out string tagPrefix, out string tagName)
        {
            tagPrefix = "";
            tagName = "";
            var isInTagPrefix = true;
            var numberOfDots = 0;

            for (var i = position; i < html.Length; i++)
            {
                if (html[i] == ':')
                {
                    if (tagPrefix.Length == 0)
                    {
                        return false;
                    }
                    isInTagPrefix = false;
                }
                else if (Char.IsWhiteSpace(html[i]) || html[i] == '>' || (html[i] == '/' && i + 1 < html.Length && html[i + 1] == '>'))
                {
                    if (tagName.Length > 0)
                    {
                        return true;
                    }
                    return false;
                }
                else if (!Char.IsLetterOrDigit(html[i]) && (isInTagPrefix || html[i] != '.' || numberOfDots > 0))
                {
                    return false;
                }
                else if (isInTagPrefix)
                {
                    tagPrefix += html[i];
                }
                else
                {
                    if (html[i] == '.')
                    {
                        numberOfDots++;
                    }
                    tagName += html[i];
                }
            }
            return false;
        }

    }
}
