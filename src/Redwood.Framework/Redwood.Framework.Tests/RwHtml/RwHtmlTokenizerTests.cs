using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redwood.Framework.Parsing;
using Redwood.Framework.RwHtml.Parsing;
using Redwood.Framework.RwHtml.Parsing.Tokens;

namespace Redwood.Framework.Tests.RwHtml
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

        private void ValidateSpanContent(string input, string expectedValue, SpanPosition span)
        {
            string actualValue = input.Substring(span.AbsolutePosition, span.Length);
            Assert.AreEqual(expectedValue, actualValue);
        }


        [TestMethod]
        public void RwHtmlTokenizer_ValidInput_PlainOpenCloseTagsAndText()
        {
            var input = "test <element> text </element> test";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();
            
            Assert.AreEqual(6, tokens.Count);

            Assert.IsInstanceOfType(tokens[0], typeof(RwValueToken));
            Assert.AreEqual("test ", ((RwValueToken)tokens[0]).Text);

            Assert.IsInstanceOfType(tokens[1], typeof(RwOpenTagBeginToken));
            Assert.AreEqual("element", ((RwOpenTagBeginToken)tokens[1]).TagName);
            ValidateSpanContent(input, "<element", tokens[1].SpanPosition);

            Assert.IsInstanceOfType(tokens[2], typeof(RwOpenTagEndToken));
            Assert.IsFalse(((RwOpenTagEndToken)tokens[2]).IsSelfClosing);
            ValidateSpanContent(input, ">", tokens[2].SpanPosition);
            
            Assert.IsInstanceOfType(tokens[3], typeof(RwValueToken));
            Assert.AreEqual(" text ", ((RwValueToken)tokens[3]).Text);

            Assert.IsInstanceOfType(tokens[4], typeof(RwCloseTagToken));
            Assert.AreEqual("element", ((RwCloseTagToken)tokens[4]).TagName);

            Assert.IsInstanceOfType(tokens[5], typeof(RwValueToken));
            Assert.AreEqual(" test", ((RwValueToken)tokens[5]).Text);

            ValidateTokenSequence(tokens, input.Length);
        }

        [TestMethod]
        public void RwHtmlTokenizer_ValidInput_AttributeWithLiteralValue_SingleQuotes()
        {
            var input = "<element attr='value' /> ";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();

            Assert.AreEqual(4, tokens.Count);

            Assert.IsInstanceOfType(tokens[0], typeof(RwOpenTagBeginToken));
            Assert.AreEqual("element", ((RwOpenTagBeginToken)tokens[0]).TagName);
            ValidateSpanContent(input, "<element", tokens[0].SpanPosition);

            Assert.IsInstanceOfType(tokens[1], typeof(RwAttributeToken));
            Assert.AreEqual("attr", ((RwAttributeToken)tokens[1]).Name);
            Assert.AreEqual("value", ((RwValueToken)((RwAttributeToken)tokens[1]).Value).Text);
            ValidateSpanContent(input, " attr='value'", tokens[1].SpanPosition);

            Assert.IsInstanceOfType(tokens[2], typeof(RwOpenTagEndToken));
            Assert.IsTrue(((RwOpenTagEndToken)tokens[2]).IsSelfClosing);
            ValidateSpanContent(input, " />", tokens[2].SpanPosition);

            Assert.IsInstanceOfType(tokens[3], typeof(RwValueToken));
            Assert.AreEqual(" ", ((RwValueToken)tokens[3]).Text);

            ValidateTokenSequence(tokens, input.Length);
        }

        [TestMethod]
        public void RwHtmlTokenizer_ValidInput_AttributeWithBindingValue_DoubleQuotesAndSpaces()
        {
            var input = "< element   attr = \"{value}\"   /> ";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();

            Assert.AreEqual(4, tokens.Count);

            Assert.IsInstanceOfType(tokens[0], typeof(RwOpenTagBeginToken));
            Assert.AreEqual("element", ((RwOpenTagBeginToken)tokens[0]).TagName);
            ValidateSpanContent(input, "< element", tokens[0].SpanPosition);

            Assert.IsInstanceOfType(tokens[1], typeof(RwAttributeToken));
            Assert.AreEqual("attr", ((RwAttributeToken)tokens[1]).Name);
            Assert.AreEqual("value", ((RwValueToken)((RwAttributeToken)tokens[1]).Value).Expression);
            ValidateSpanContent(input, "   attr = \"{value}\"", tokens[1].SpanPosition);

            Assert.IsInstanceOfType(tokens[2], typeof(RwOpenTagEndToken));
            Assert.IsTrue(((RwOpenTagEndToken)tokens[2]).IsSelfClosing);
            ValidateSpanContent(input, "   />", tokens[2].SpanPosition);

            Assert.IsInstanceOfType(tokens[3], typeof(RwValueToken));
            Assert.AreEqual(" ", ((RwValueToken)tokens[3]).Text);

            ValidateTokenSequence(tokens, input.Length);
        }

        [TestMethod]
        public void RwHtmlTokenizer_ValidInput_AttributeWithLiteralValue_NoQuotes()
        {
            var input = "<element attr=value_2 /> ";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();

            Assert.AreEqual(4, tokens.Count);

            Assert.IsInstanceOfType(tokens[0], typeof(RwOpenTagBeginToken));
            Assert.AreEqual("element", ((RwOpenTagBeginToken)tokens[0]).TagName);
            ValidateSpanContent(input, "<element", tokens[0].SpanPosition);

            Assert.IsInstanceOfType(tokens[1], typeof(RwAttributeToken));
            Assert.AreEqual("attr", ((RwAttributeToken)tokens[1]).Name);
            Assert.AreEqual("value_2", ((RwValueToken)((RwAttributeToken)tokens[1]).Value).Text);
            ValidateSpanContent(input, " attr=value_2", tokens[1].SpanPosition);

            Assert.IsInstanceOfType(tokens[2], typeof(RwOpenTagEndToken));
            Assert.IsTrue(((RwOpenTagEndToken)tokens[2]).IsSelfClosing);
            ValidateSpanContent(input, " />", tokens[2].SpanPosition);

            Assert.IsInstanceOfType(tokens[3], typeof(RwValueToken));
            Assert.AreEqual(" ", ((RwValueToken)tokens[3]).Text);

            ValidateTokenSequence(tokens, input.Length);
        }


        [TestMethod]
        public void RwHtmlTokenizer_ValidInput_EmptyLiteral()
        {
            var input = "";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();

            Assert.AreEqual(0, tokens.Count);
        }


        [TestMethod]
        public void RwHtmlTokenizer_ValidInput_ScriptContents()
        {
            var input = "test <script>if (xxx < 15) return;</script> test2";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();

            Assert.AreEqual(6, tokens.Count);

            Assert.IsInstanceOfType(tokens[0], typeof(RwValueToken));
            Assert.AreEqual("test ", ((RwValueToken)tokens[0]).Text);
            ValidateSpanContent(input, "test ", tokens[0].SpanPosition);

            Assert.IsInstanceOfType(tokens[1], typeof(RwOpenTagBeginToken));
            Assert.AreEqual("script", ((RwOpenTagBeginToken)tokens[1]).TagName);
            ValidateSpanContent(input, "<script", tokens[1].SpanPosition);

            Assert.IsInstanceOfType(tokens[2], typeof(RwOpenTagEndToken));
            Assert.IsFalse(((RwOpenTagEndToken)tokens[2]).IsSelfClosing);
            ValidateSpanContent(input, ">", tokens[2].SpanPosition);

            Assert.IsInstanceOfType(tokens[3], typeof(RwValueToken));
            Assert.AreEqual("if (xxx < 15) return;", ((RwValueToken)tokens[3]).Text);
            ValidateSpanContent(input, "if (xxx < 15) return;", tokens[3].SpanPosition);

            Assert.IsInstanceOfType(tokens[4], typeof(RwCloseTagToken));
            Assert.AreEqual("script", ((RwCloseTagToken)tokens[4]).TagName);
            ValidateSpanContent(input, "</script>", tokens[4].SpanPosition);

            Assert.IsInstanceOfType(tokens[5], typeof(RwValueToken));
            Assert.AreEqual(" test2", ((RwValueToken)tokens[5]).Text);
            ValidateSpanContent(input, " test2", tokens[5].SpanPosition);

            ValidateTokenSequence(tokens, input.Length);
        }

        [TestMethod]
        public void RwHtmlTokenizer_ValidInput_ScriptEndsAtEndOfFile()
        {
            var input = "test <script>if (xxx < 15) return;</script>";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();

            Assert.AreEqual(5, tokens.Count);

            Assert.IsInstanceOfType(tokens[0], typeof(RwValueToken));
            Assert.AreEqual("test ", ((RwValueToken)tokens[0]).Text);
            ValidateSpanContent(input, "test ", tokens[0].SpanPosition);

            Assert.IsInstanceOfType(tokens[1], typeof(RwOpenTagBeginToken));
            Assert.AreEqual("script", ((RwOpenTagBeginToken)tokens[1]).TagName);
            ValidateSpanContent(input, "<script", tokens[1].SpanPosition);

            Assert.IsInstanceOfType(tokens[2], typeof(RwOpenTagEndToken));
            Assert.IsFalse(((RwOpenTagEndToken)tokens[2]).IsSelfClosing);
            ValidateSpanContent(input, ">", tokens[2].SpanPosition);

            Assert.IsInstanceOfType(tokens[3], typeof(RwValueToken));
            Assert.AreEqual("if (xxx < 15) return;", ((RwValueToken)tokens[3]).Text);
            ValidateSpanContent(input, "if (xxx < 15) return;", tokens[3].SpanPosition);

            Assert.IsInstanceOfType(tokens[4], typeof(RwCloseTagToken));
            Assert.AreEqual("script", ((RwCloseTagToken)tokens[4]).TagName);
            ValidateSpanContent(input, "</script>", tokens[4].SpanPosition);

            ValidateTokenSequence(tokens, input.Length);
        }

        [TestMethod]
        public void RwHtmlTokenizer_ValidInput_BindingInsideScript()
        {
            var input = "test <script>if (xxx < {{Binding MaxValue}}) { return; }</script>";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();

            Assert.AreEqual(7, tokens.Count);

            Assert.IsInstanceOfType(tokens[0], typeof(RwValueToken));
            Assert.AreEqual("test ", ((RwValueToken)tokens[0]).Text);
            ValidateSpanContent(input, "test ", tokens[0].SpanPosition);

            Assert.IsInstanceOfType(tokens[1], typeof(RwOpenTagBeginToken));
            Assert.AreEqual("script", ((RwOpenTagBeginToken)tokens[1]).TagName);
            ValidateSpanContent(input, "<script", tokens[1].SpanPosition);

            Assert.IsInstanceOfType(tokens[2], typeof(RwOpenTagEndToken));
            Assert.IsFalse(((RwOpenTagEndToken)tokens[2]).IsSelfClosing);
            ValidateSpanContent(input, ">", tokens[2].SpanPosition);

            Assert.IsInstanceOfType(tokens[3], typeof(RwValueToken));
            Assert.IsFalse(((RwValueToken)tokens[3]).IsExpression);
            Assert.AreEqual("if (xxx < ", ((RwValueToken)tokens[3]).Text);
            ValidateSpanContent(input, "if (xxx < ", tokens[3].SpanPosition);

            Assert.IsInstanceOfType(tokens[4], typeof(RwValueToken));
            Assert.IsTrue(((RwValueToken)tokens[4]).IsExpression);
            Assert.AreEqual("Binding MaxValue", ((RwValueToken)tokens[4]).Text);
            ValidateSpanContent(input, "{{Binding MaxValue}}", tokens[4].SpanPosition);
            
            Assert.IsInstanceOfType(tokens[5], typeof(RwValueToken));
            Assert.IsFalse(((RwValueToken)tokens[5]).IsExpression);
            Assert.AreEqual(") { return; }", ((RwValueToken)tokens[5]).Text);
            ValidateSpanContent(input, ") { return; }", tokens[5].SpanPosition);

            Assert.IsInstanceOfType(tokens[6], typeof(RwCloseTagToken));
            Assert.AreEqual("script", ((RwCloseTagToken)tokens[6]).TagName);
            ValidateSpanContent(input, "</script>", tokens[6].SpanPosition);

            ValidateTokenSequence(tokens, input.Length);
        }

        [TestMethod]
        public void RwHtmlTokenizer_ValidInput_SelfClosedControl()
        {
            var input = "<html><c:Control Text=\"{Text, HtmlEncode=true}\" /></html>";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();

            Assert.AreEqual(6, tokens.Count);

            Assert.IsInstanceOfType(tokens[0], typeof(RwOpenTagBeginToken));
            Assert.AreEqual("html", ((RwOpenTagBeginToken)tokens[0]).TagName);
            ValidateSpanContent(input, "<html", tokens[0].SpanPosition);

            Assert.IsInstanceOfType(tokens[1], typeof(RwOpenTagEndToken));
            Assert.IsFalse(((RwOpenTagEndToken)tokens[1]).IsSelfClosing);
            ValidateSpanContent(input, ">", tokens[1].SpanPosition);

            Assert.IsInstanceOfType(tokens[2], typeof(RwOpenTagBeginToken));
            Assert.AreEqual("c:Control", ((RwOpenTagBeginToken)tokens[2]).TagName);
            ValidateSpanContent(input, "<c:Control", tokens[2].SpanPosition);

            Assert.IsInstanceOfType(tokens[3], typeof(RwAttributeToken));
            Assert.AreEqual("Text", ((RwAttributeToken)tokens[3]).Name);
            Assert.IsInstanceOfType(((RwAttributeToken)tokens[3]).Value, typeof(RwValueToken));
            Assert.AreEqual("Text, HtmlEncode=true", ((RwValueToken)(((RwAttributeToken)tokens[3]).Value)).Expression);
            ValidateSpanContent(input, "{Text, HtmlEncode=true}", ((RwAttributeToken)tokens[3]).Value.SpanPosition);
            ValidateSpanContent(input, " Text=\"{Text, HtmlEncode=true}\"", tokens[3].SpanPosition);
            
            Assert.IsInstanceOfType(tokens[4], typeof(RwOpenTagEndToken));
            Assert.IsTrue(((RwOpenTagEndToken)tokens[4]).IsSelfClosing);
            ValidateSpanContent(input, " />", tokens[4].SpanPosition);

            Assert.IsInstanceOfType(tokens[5], typeof(RwCloseTagToken));
            Assert.AreEqual("html", ((RwCloseTagToken)tokens[5]).TagName);
            ValidateSpanContent(input, "</html>", tokens[5].SpanPosition);

            ValidateTokenSequence(tokens, input.Length);
        }

        [TestMethod]
        public void RwHtmlTokenizer_ValidInput_ControlWithContent()
        {
            var input = "<html><c:Control Text=\"{Text, HtmlEncode=true}\">test</c:Control></html> te";
            var tokens = new RwHtmlTokenizer().Parse(input).ToList();

            Assert.AreEqual(9, tokens.Count);
            Assert.IsInstanceOfType(tokens[0], typeof(RwOpenTagBeginToken));
            Assert.AreEqual("html", ((RwOpenTagBeginToken)tokens[0]).TagName);
            ValidateSpanContent(input, "<html", tokens[0].SpanPosition);

            Assert.IsInstanceOfType(tokens[1], typeof(RwOpenTagEndToken));
            Assert.IsFalse(((RwOpenTagEndToken)tokens[1]).IsSelfClosing);
            ValidateSpanContent(input, ">", tokens[1].SpanPosition);

            Assert.IsInstanceOfType(tokens[2], typeof(RwOpenTagBeginToken));
            Assert.AreEqual("c:Control", ((RwOpenTagBeginToken)tokens[2]).TagName);
            ValidateSpanContent(input, "<c:Control", tokens[2].SpanPosition);

            Assert.IsInstanceOfType(tokens[3], typeof(RwAttributeToken));
            Assert.AreEqual("Text", ((RwAttributeToken)tokens[3]).Name);
            Assert.IsInstanceOfType(((RwAttributeToken)tokens[3]).Value, typeof(RwValueToken));
            Assert.AreEqual("Text, HtmlEncode=true", ((RwValueToken)(((RwAttributeToken)tokens[3]).Value)).Expression);
            ValidateSpanContent(input, "{Text, HtmlEncode=true}", ((RwAttributeToken)tokens[3]).Value.SpanPosition);
            ValidateSpanContent(input, " Text=\"{Text, HtmlEncode=true}\"", tokens[3].SpanPosition);

            Assert.IsInstanceOfType(tokens[4], typeof(RwOpenTagEndToken));
            Assert.IsFalse(((RwOpenTagEndToken)tokens[4]).IsSelfClosing);
            ValidateSpanContent(input, ">", tokens[4].SpanPosition);

            Assert.IsInstanceOfType(tokens[5], typeof(RwValueToken));
            Assert.AreEqual("test", ((RwValueToken)tokens[5]).Text);
            ValidateSpanContent(input, "test", tokens[5].SpanPosition);

            Assert.IsInstanceOfType(tokens[6], typeof(RwCloseTagToken));
            Assert.AreEqual("c:Control", ((RwCloseTagToken)tokens[6]).TagName);
            ValidateSpanContent(input, "</c:Control>", tokens[6].SpanPosition);

            Assert.IsInstanceOfType(tokens[7], typeof(RwCloseTagToken));
            Assert.AreEqual("html", ((RwCloseTagToken)tokens[7]).TagName);
            ValidateSpanContent(input, "</html>", tokens[7].SpanPosition);

            Assert.IsInstanceOfType(tokens[8], typeof(RwValueToken));
            Assert.AreEqual(" te", ((RwValueToken)tokens[8]).Text);
            ValidateSpanContent(input, " te", tokens[8].SpanPosition);

            ValidateTokenSequence(tokens, input.Length);
        }

        // TODO: invalid inputs


        [TestMethod]
        public void RwHtmlTokenizer_ValidInput_Sample()
        {
            var fileName = Path.Combine(TestContext.TestDeploymentDir, "..\\..\\..\\Redwood.Samples.Basic\\TaskList.rwhtml");
            var input = File.ReadAllText(fileName, Encoding.UTF8);

            var tokens = new RwHtmlTokenizer().Parse(input).ToList();
            ValidateTokenSequence(tokens, input.Length);
        }

        [TestMethod]
        public void RwHtmlTokenizer_InvalidInput_AdditionalCharsInBindingAttribute()
        {
            try
            {
                var input = "<rw:Button Text=\"{Binding Title} aa\" />";
                var tokens = new RwHtmlTokenizer().Parse(input).ToList();
                
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ParserException));
                Assert.AreEqual(32, ((ParserException)ex).Position.PositionOnLine);
            }
        }


    }
}
