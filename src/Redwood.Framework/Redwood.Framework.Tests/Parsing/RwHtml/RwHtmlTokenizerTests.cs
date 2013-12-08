using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redwood.Framework.Parsing.RwHtml;

namespace Redwood.Framework.Tests.Parsing.RwHtml
{
    [TestClass]
    public class RwHtmlTokenizerTests
    {

        [TestMethod]
        public void ValidInput_SingleLiteral()
        {
            var input = "test <element> text";
            var tokens = new RwHtmlTokenizer().GetTokens(input).ToList();
            
            Assert.AreEqual(1, tokens.Count);
            Assert.IsInstanceOfType(tokens[0], typeof(RwLiteralToken));
            Assert.AreEqual(input, ((RwLiteralToken)tokens[0]).Text);
        }

        [TestMethod]
        public void ValidInput_EmptyLiteral()
        {
            var input = "";
            var tokens = new RwHtmlTokenizer().GetTokens(input).ToList();

            Assert.AreEqual(0, tokens.Count);
        }

        [TestMethod]
        public void ValidInput_SelfClosedControl()
        {
            var input = "<html><c:Control Text=\"{{Text, HtmlEncode=true}}\" /></html>";
            var tokens = new RwHtmlTokenizer().GetTokens(input).ToList();

            Assert.AreEqual(4, tokens.Count);
            Assert.IsInstanceOfType(tokens[0], typeof(RwLiteralToken));
            Assert.IsInstanceOfType(tokens[1], typeof(RwControlToken));
            Assert.IsInstanceOfType(tokens[2], typeof(RwControlClosingToken));
            Assert.IsInstanceOfType(tokens[3], typeof(RwLiteralToken));

            Assert.AreEqual("c", ((RwControlToken)tokens[1]).TagPrefix);
            Assert.AreEqual("Control", ((RwControlToken)tokens[1]).TagName);

            Assert.AreEqual(1, ((RwControlToken)tokens[1]).Attributes.Count);
            Assert.AreEqual("{{Text, HtmlEncode=true}}", ((RwControlToken)tokens[1]).Attributes["Text"]);
        }

        [TestMethod]
        public void ValidInput_ControlWithContent()
        {
            var input = "<html><c:Control Text=\"{{Text, HtmlEncode=true}}\">test</c:Control></html>";
            var tokens = new RwHtmlTokenizer().GetTokens(input).ToList();

            Assert.AreEqual(5, tokens.Count);
            Assert.IsInstanceOfType(tokens[0], typeof(RwLiteralToken));
            Assert.IsInstanceOfType(tokens[1], typeof(RwControlToken));
            Assert.IsInstanceOfType(tokens[2], typeof(RwLiteralToken));
            Assert.IsInstanceOfType(tokens[3], typeof(RwControlClosingToken));
            Assert.IsInstanceOfType(tokens[4], typeof(RwLiteralToken));

            Assert.AreEqual("c", ((RwControlToken)tokens[1]).TagPrefix);
            Assert.AreEqual("Control", ((RwControlToken)tokens[1]).TagName);

            Assert.AreEqual(1, ((RwControlToken)tokens[1]).Attributes.Count);
            Assert.AreEqual("{{Text, HtmlEncode=true}}", ((RwControlToken)tokens[1]).Attributes["Text"]);
        }

        // TODO: invalid input tests


    }
}
