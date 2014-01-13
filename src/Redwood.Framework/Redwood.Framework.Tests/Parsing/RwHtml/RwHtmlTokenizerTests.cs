using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redwood.Framework.Parsing.RwHtml;
using Redwood.Framework.Parsing.RwHtml.Tokens;

namespace Redwood.Framework.Tests.Parsing.RwHtml
{
    [TestClass]
    public class RwHtmlTokenizerTests
    {

        public TestContext TestContext { get; set; }

        private void ValidateTokenSequence(List<RwHtmlToken> tokens, int inputLength)
        {
            var pos = 0;
            for (var i = 0; i < tokens.Count; i++)
            {
                Assert.AreEqual(pos, tokens[i].SpanPosition.AbsolutePosition);
                pos += tokens[i].SpanPosition.Length;
            }
            Assert.AreEqual(pos, inputLength);
        }




        [TestMethod]
        public void ValidInput_PlainOpenCloseTagsAndText()
        {
            var input = "test <element> text </element> test";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();
            
            Assert.AreEqual(6, tokens.Count);

            Assert.IsInstanceOfType(tokens[0], typeof(RwLiteralToken));
            Assert.AreEqual("test ", ((RwLiteralToken)tokens[0]).Text);

            Assert.IsInstanceOfType(tokens[1], typeof(RwOpenTagToken));
            Assert.AreEqual("element", ((RwOpenTagToken)tokens[1]).TagName);
            Assert.AreEqual(5, tokens[1].SpanPosition.AbsolutePosition);
            Assert.AreEqual(8, tokens[1].SpanPosition.Length);

            Assert.IsInstanceOfType(tokens[2], typeof(RwOpenTagToken));
            Assert.AreEqual("element", ((RwOpenTagToken)tokens[2]).TagName);
            Assert.AreEqual(13, tokens[2].SpanPosition.AbsolutePosition);
            Assert.AreEqual(1, tokens[2].SpanPosition.Length);

            Assert.IsInstanceOfType(tokens[3], typeof(RwLiteralToken));
            Assert.AreEqual(" text ", ((RwLiteralToken)tokens[3]).Text);

            Assert.IsInstanceOfType(tokens[4], typeof(RwCloseTagToken));
            Assert.AreEqual("element", ((RwCloseTagToken)tokens[4]).TagName);

            Assert.IsInstanceOfType(tokens[5], typeof(RwLiteralToken));
            Assert.AreEqual(" test", ((RwLiteralToken)tokens[5]).Text);

            ValidateTokenSequence(tokens, input.Length);
        }


        [TestMethod]
        public void ValidInput_EmptyLiteral()
        {
            var input = "";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();

            Assert.AreEqual(0, tokens.Count);
        }

        [TestMethod]
        public void ValidInput_SelfClosedControl()
        {
            var input = "<html><c:Control Text=\"{Text, HtmlEncode=true}\" /></html>";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();

            Assert.AreEqual(6, tokens.Count);

            Assert.IsInstanceOfType(tokens[0], typeof(RwOpenTagToken));
            Assert.AreEqual("html", ((RwOpenTagToken)tokens[0]).TagName);

            Assert.IsInstanceOfType(tokens[1], typeof(RwOpenTagToken));
            Assert.AreEqual("html", ((RwOpenTagToken)tokens[1]).TagName);
            Assert.AreEqual(TagType.BeginTagCloseAngle, ((RwOpenTagToken)tokens[1]).TagType);

            Assert.IsInstanceOfType(tokens[2], typeof(RwOpenTagToken));
            Assert.AreEqual("c:Control", ((RwOpenTagToken)tokens[2]).TagName);

            Assert.IsInstanceOfType(tokens[3], typeof(RwAttributeToken));
            Assert.AreEqual("Text", ((RwAttributeToken)tokens[3]).Name);
            Assert.IsInstanceOfType(((RwAttributeToken)tokens[3]).Value, typeof(RwBindingToken));
            Assert.AreEqual("Text, HtmlEncode=true", ((RwBindingToken)(((RwAttributeToken)tokens[3]).Value)).Expression);

            Assert.IsInstanceOfType(tokens[4], typeof(RwCloseTagToken));
            Assert.AreEqual("c:Control", ((RwCloseTagToken)tokens[4]).TagName);
            Assert.IsTrue(((RwCloseTagToken)tokens[4]).IsSelfClosing);

            Assert.IsInstanceOfType(tokens[5], typeof(RwCloseTagToken));
            Assert.AreEqual("html", ((RwCloseTagToken)tokens[5]).TagName);

            ValidateTokenSequence(tokens, input.Length);
        }

        [TestMethod]
        public void ValidInput_ControlWithContent()
        {
            var input = "<html><c:Control Text=\"{{Text, HtmlEncode=true}}\">test</c:Control></html>";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();

            Assert.AreEqual(5, tokens.Count);
            Assert.IsInstanceOfType(tokens[0], typeof(RwLiteralToken));
            Assert.IsInstanceOfType(tokens[1], typeof(RwOpenTagToken));
            Assert.IsInstanceOfType(tokens[2], typeof(RwLiteralToken));
            Assert.IsInstanceOfType(tokens[3], typeof(RwCloseTagToken));
            Assert.IsInstanceOfType(tokens[4], typeof(RwLiteralToken));
        }

        // TODO: invalid inputs


        [TestMethod]
        public void ValidInput_Sample()
        {
            var fileName = Path.Combine(TestContext.TestDeploymentDir, "..\\..\\..\\Redwood.Samples.Basic\\index.rwhtml");
        }

    }
}
