using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redwood.Framework.RwHtml.Parsing;
using Redwood.Framework.RwHtml.Parsing.Tokens;

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
        public void ValidInput_AttributeWithLiteralValue_SingleQuotes()
        {
            var input = "<element attr='value' /> ";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();

            Assert.AreEqual(4, tokens.Count);

            Assert.IsInstanceOfType(tokens[0], typeof(RwOpenTagToken));
            Assert.AreEqual("element", ((RwOpenTagToken)tokens[0]).TagName);

            Assert.IsInstanceOfType(tokens[1], typeof(RwAttributeToken));
            Assert.AreEqual("attr", ((RwAttributeToken)tokens[1]).Name);
            Assert.AreEqual("value", ((RwLiteralToken)((RwAttributeToken)tokens[1]).Value).Text);
            Assert.AreEqual(15, ((RwAttributeToken)tokens[1]).Value.SpanPosition.AbsolutePosition);
            Assert.AreEqual(5, ((RwAttributeToken)tokens[1]).Value.SpanPosition.Length);

            Assert.IsInstanceOfType(tokens[2], typeof(RwCloseTagToken));
            Assert.AreEqual("element", ((RwCloseTagToken)tokens[2]).TagName);
            Assert.IsTrue(((RwCloseTagToken)tokens[2]).IsSelfClosing);

            Assert.IsInstanceOfType(tokens[3], typeof(RwLiteralToken));
            Assert.AreEqual(" ", ((RwLiteralToken)tokens[3]).Text);

            ValidateTokenSequence(tokens, input.Length);
        }

        [TestMethod]
        public void ValidInput_AttributeWithBindingValue_DoubleQuotesAndSpaces()
        {
            var input = "< element   attr = \"{value}\"   /> ";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();

            Assert.AreEqual(4, tokens.Count);

            Assert.IsInstanceOfType(tokens[0], typeof(RwOpenTagToken));
            Assert.AreEqual("element", ((RwOpenTagToken)tokens[0]).TagName);

            Assert.IsInstanceOfType(tokens[1], typeof(RwAttributeToken));
            Assert.AreEqual("attr", ((RwAttributeToken)tokens[1]).Name);
            Assert.AreEqual("value", ((RwBindingToken)((RwAttributeToken)tokens[1]).Value).Expression);
            Assert.AreEqual(20, ((RwAttributeToken)tokens[1]).Value.SpanPosition.AbsolutePosition);
            Assert.AreEqual(7, ((RwAttributeToken)tokens[1]).Value.SpanPosition.Length);

            Assert.IsInstanceOfType(tokens[2], typeof(RwCloseTagToken));
            Assert.AreEqual("element", ((RwCloseTagToken)tokens[2]).TagName);
            Assert.IsTrue(((RwCloseTagToken)tokens[2]).IsSelfClosing);

            Assert.IsInstanceOfType(tokens[3], typeof(RwLiteralToken));
            Assert.AreEqual(" ", ((RwLiteralToken)tokens[3]).Text);

            ValidateTokenSequence(tokens, input.Length);
        }

        [TestMethod]
        public void ValidInput_AttributeWithLiteralValue_NoQuotes()
        {
            var input = "<element attr=value_2 /> ";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();

            Assert.AreEqual(4, tokens.Count);

            Assert.IsInstanceOfType(tokens[0], typeof(RwOpenTagToken));
            Assert.AreEqual("element", ((RwOpenTagToken)tokens[0]).TagName);

            Assert.IsInstanceOfType(tokens[1], typeof(RwAttributeToken));
            Assert.AreEqual("attr", ((RwAttributeToken)tokens[1]).Name);
            Assert.AreEqual("value_2", ((RwLiteralToken)((RwAttributeToken)tokens[1]).Value).Text);
            Assert.AreEqual(14, ((RwAttributeToken)tokens[1]).Value.SpanPosition.AbsolutePosition);
            Assert.AreEqual(7, ((RwAttributeToken)tokens[1]).Value.SpanPosition.Length);

            Assert.IsInstanceOfType(tokens[2], typeof(RwCloseTagToken));
            Assert.AreEqual("element", ((RwCloseTagToken)tokens[2]).TagName);
            Assert.IsTrue(((RwCloseTagToken)tokens[2]).IsSelfClosing);

            Assert.IsInstanceOfType(tokens[3], typeof(RwLiteralToken));
            Assert.AreEqual(" ", ((RwLiteralToken)tokens[3]).Text);

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
            var input = "<html><c:Control Text=\"{Text, HtmlEncode=true}\">test</c:Control></html> te";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();

            Assert.AreEqual(9, tokens.Count);
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

            Assert.IsInstanceOfType(tokens[4], typeof(RwOpenTagToken));
            Assert.AreEqual("c:Control", ((RwOpenTagToken)tokens[4]).TagName);
            Assert.AreEqual(TagType.BeginTagCloseAngle, ((RwOpenTagToken)tokens[4]).TagType);

            Assert.IsInstanceOfType(tokens[5], typeof(RwLiteralToken));
            Assert.AreEqual("test", ((RwLiteralToken)tokens[5]).Text);

            Assert.IsInstanceOfType(tokens[6], typeof(RwCloseTagToken));
            Assert.AreEqual("c:Control", ((RwCloseTagToken)tokens[6]).TagName);
            Assert.IsFalse(((RwCloseTagToken)tokens[6]).IsSelfClosing);

            Assert.IsInstanceOfType(tokens[7], typeof(RwCloseTagToken));
            Assert.AreEqual("html", ((RwCloseTagToken)tokens[7]).TagName);

            Assert.IsInstanceOfType(tokens[8], typeof(RwLiteralToken));
            Assert.AreEqual(" te", ((RwLiteralToken)tokens[8]).Text);

            ValidateTokenSequence(tokens, input.Length);
        }

        // TODO: invalid inputs


        [TestMethod]
        public void ValidInput_Sample()
        {
            var fileName = Path.Combine(TestContext.TestDeploymentDir, "..\\..\\..\\Redwood.Samples.Basic\\index.rwhtml");
            var input = File.ReadAllText(fileName, Encoding.UTF8);

            var tokens = new RwHtmlTokenizer().Parse(input).ToList();
            ValidateTokenSequence(tokens, input.Length);
        }

    }
}
