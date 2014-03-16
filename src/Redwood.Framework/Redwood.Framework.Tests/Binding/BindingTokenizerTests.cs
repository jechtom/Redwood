using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Binding.Parsing;
using Redwood.Framework.Binding.Parsing.Tokens;

namespace Redwood.Framework.Tests.Binding
{
    [TestClass]
    public class BindingTokenizerTests
    {

        [TestMethod]
        public void BindingTokenizer_Empty()
        {
            var tokenizer = new BindingTokenizer();
            var results = tokenizer.Parse("Binding").ToList();

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Binding", ((BindingTypeToken)results[0]).BindingTypeName);
            Assert.AreEqual(0, results[0].SpanPosition.AbsolutePosition);
            Assert.AreEqual(7, results[0].SpanPosition.Length);
        }

        [TestMethod]
        public void BindingTokenizer_IndexerOnly()
        {
            var tokenizer = new BindingTokenizer();
            var results = tokenizer.Parse("Binding [13]").ToList();

            Assert.AreEqual(5, results.Count);
            Assert.AreEqual("Binding", ((BindingTypeToken)results[0]).BindingTypeName);
            Assert.AreEqual(0, results[0].SpanPosition.AbsolutePosition);
            Assert.AreEqual(8, results[0].SpanPosition.Length);

            Assert.AreEqual("", ((BindingTextToken)results[1]).Text);

            Assert.IsInstanceOfType(results[2], typeof(BindingOpenIndexerToken));

            Assert.AreEqual("13", ((BindingTextToken)results[3]).Text);

            Assert.IsInstanceOfType(results[4], typeof(BindingCloseIndexerToken));
        }

        [TestMethod]
        public void BindingTokenizer_SingleIdentifier()
        {
            var tokenizer = new BindingTokenizer();
            var results = tokenizer.Parse("Binding Test").ToList();

            Assert.AreEqual(2, results.Count);
            Assert.IsInstanceOfType(results[1], typeof(BindingTextToken));
            Assert.AreEqual("Test", ((BindingTextToken)results[1]).Text);
        }

        [TestMethod]
        public void BindingTokenizer_IdentifierWithDots()
        {
            var tokenizer = new BindingTokenizer();
            var results = tokenizer.Parse("Binding Test.Test2.Test3").ToList();

            Assert.AreEqual(6, results.Count);

            Assert.IsInstanceOfType(results[1], typeof(BindingTextToken));
            Assert.AreEqual("Test", ((BindingTextToken)results[1]).Text);

            Assert.IsInstanceOfType(results[2], typeof(BindingDotToken));

            Assert.IsInstanceOfType(results[3], typeof(BindingTextToken));
            Assert.AreEqual("Test2", ((BindingTextToken)results[3]).Text);

            Assert.IsInstanceOfType(results[4], typeof(BindingDotToken));

            Assert.IsInstanceOfType(results[5], typeof(BindingTextToken));
            Assert.AreEqual("Test3", ((BindingTextToken)results[5]).Text);
        }
        
        [TestMethod]
        public void BindingTokenizer_IdentifierWithDotsAndIndexers()
        {
            var tokenizer = new BindingTokenizer();
            var results = tokenizer.Parse("Binding Test[1].Test2.Test3[Key=\"hello\"]").ToList();

            Assert.AreEqual(14, results.Count);

            Assert.IsInstanceOfType(results[1], typeof(BindingTextToken));
            Assert.AreEqual("Test", ((BindingTextToken)results[1]).Text);

            Assert.IsInstanceOfType(results[2], typeof(BindingOpenIndexerToken));

            Assert.IsInstanceOfType(results[3], typeof(BindingTextToken));
            Assert.AreEqual("1", ((BindingTextToken)results[3]).Text);

            Assert.IsInstanceOfType(results[4], typeof(BindingCloseIndexerToken));
            
            Assert.IsInstanceOfType(results[5], typeof(BindingDotToken));

            Assert.IsInstanceOfType(results[6], typeof(BindingTextToken));
            Assert.AreEqual("Test2", ((BindingTextToken)results[6]).Text);
            
            Assert.IsInstanceOfType(results[7], typeof(BindingDotToken));

            Assert.IsInstanceOfType(results[8], typeof(BindingTextToken));
            Assert.AreEqual("Test3", ((BindingTextToken)results[8]).Text);

            Assert.IsInstanceOfType(results[9], typeof(BindingOpenIndexerToken));

            Assert.IsInstanceOfType(results[10], typeof(BindingTextToken));
            Assert.AreEqual("Key", ((BindingTextToken)results[10]).Text);

            Assert.IsInstanceOfType(results[11], typeof(BindingEqualsToken));

            Assert.IsInstanceOfType(results[12], typeof(BindingQuotedTextToken));
            Assert.AreEqual("hello", ((BindingTextToken)results[12]).Text);

            Assert.IsInstanceOfType(results[13], typeof(BindingCloseIndexerToken));
        }

        [TestMethod]
        public void BindingTokenizer_EqualWithText()
        {
            var tokenizer = new BindingTokenizer();
            var results = tokenizer.Parse("Binding Test=Text").ToList();

            Assert.AreEqual(4, results.Count);

            Assert.IsInstanceOfType(results[1], typeof(BindingTextToken));
            Assert.AreEqual("Test", ((BindingTextToken)results[1]).Text);

            Assert.IsInstanceOfType(results[2], typeof(BindingEqualsToken));

            Assert.IsInstanceOfType(results[3], typeof(BindingTextToken));
            Assert.AreEqual("Text", ((BindingTextToken)results[3]).Text);
        }

        [TestMethod]
        public void BindingTokenizer_EqualWithQuotedText()
        {
            var tokenizer = new BindingTokenizer();
            var results = tokenizer.Parse("Binding Test=\"Text dsaf, dsf( .adf \"").ToList();

            Assert.AreEqual(4, results.Count);

            Assert.IsInstanceOfType(results[1], typeof(BindingTextToken));
            Assert.AreEqual("Test", ((BindingTextToken)results[1]).Text);

            Assert.IsInstanceOfType(results[2], typeof(BindingEqualsToken));

            Assert.IsInstanceOfType(results[3], typeof(BindingQuotedTextToken));
            Assert.AreEqual("Text dsaf, dsf( .adf ", ((BindingQuotedTextToken)results[3]).Text);
        }


        [TestMethod]
        public void BindingTokenizer_ExpressionWithBraces()
        {
            var tokenizer = new BindingTokenizer();
            var results = tokenizer.Parse("Binding Model.Command(Id, Address.Street)").ToList();

            Assert.AreEqual(11, results.Count);

            Assert.IsInstanceOfType(results[1], typeof(BindingTextToken));
            Assert.AreEqual("Model", ((BindingTextToken)results[1]).Text);

            Assert.IsInstanceOfType(results[2], typeof(BindingDotToken));

            Assert.IsInstanceOfType(results[3], typeof(BindingTextToken));
            Assert.AreEqual("Command", ((BindingTextToken)results[3]).Text);

            Assert.IsInstanceOfType(results[4], typeof(BindingOpenBraceToken));

            Assert.IsInstanceOfType(results[5], typeof(BindingTextToken));
            Assert.AreEqual("Id", ((BindingTextToken)results[5]).Text);

            Assert.IsInstanceOfType(results[6], typeof(BindingCommaToken));

            Assert.IsInstanceOfType(results[7], typeof(BindingTextToken));
            Assert.AreEqual("Address", ((BindingTextToken)results[7]).Text);
            
            Assert.IsInstanceOfType(results[8], typeof(BindingDotToken));

            Assert.IsInstanceOfType(results[9], typeof(BindingTextToken));
            Assert.AreEqual("Street", ((BindingTextToken)results[9]).Text);

            Assert.IsInstanceOfType(results[10], typeof(BindingCloseBraceToken));
        }

        [TestMethod]
        public void BindingTokenizer_MultipleExpressions()
        {
            var tokenizer = new BindingTokenizer();
            var results = tokenizer.Parse("Binding Path=Model.Command(Id, Address.Street), Expr2=Expr3").ToList();

            Assert.AreEqual(17, results.Count);

            Assert.IsInstanceOfType(results[1], typeof(BindingTextToken));
            Assert.AreEqual("Path", ((BindingTextToken)results[1]).Text);

            Assert.IsInstanceOfType(results[2], typeof(BindingEqualsToken));

            Assert.IsInstanceOfType(results[3], typeof(BindingTextToken));
            Assert.AreEqual("Model", ((BindingTextToken)results[3]).Text);

            Assert.IsInstanceOfType(results[4], typeof(BindingDotToken));

            Assert.IsInstanceOfType(results[5], typeof(BindingTextToken));
            Assert.AreEqual("Command", ((BindingTextToken)results[5]).Text);

            Assert.IsInstanceOfType(results[6], typeof(BindingOpenBraceToken));

            Assert.IsInstanceOfType(results[7], typeof(BindingTextToken));
            Assert.AreEqual("Id", ((BindingTextToken)results[7]).Text);

            Assert.IsInstanceOfType(results[8], typeof(BindingCommaToken));

            Assert.IsInstanceOfType(results[9], typeof(BindingTextToken));
            Assert.AreEqual("Address", ((BindingTextToken)results[9]).Text);

            Assert.IsInstanceOfType(results[10], typeof(BindingDotToken));

            Assert.IsInstanceOfType(results[11], typeof(BindingTextToken));
            Assert.AreEqual("Street", ((BindingTextToken)results[11]).Text);

            Assert.IsInstanceOfType(results[12], typeof(BindingCloseBraceToken));

            Assert.IsInstanceOfType(results[13], typeof(BindingCommaToken));

            Assert.IsInstanceOfType(results[14], typeof(BindingTextToken));
            Assert.AreEqual("Expr2", ((BindingTextToken)results[14]).Text);

            Assert.IsInstanceOfType(results[15], typeof(BindingEqualsToken));

            Assert.IsInstanceOfType(results[16], typeof(BindingTextToken));
            Assert.AreEqual("Expr3", ((BindingTextToken)results[16]).Text);
        }
    }
}
