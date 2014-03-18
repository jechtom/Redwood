using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redwood.Framework.Binding;
using Redwood.Framework.Binding.Parsing;
using Redwood.Framework.Binding.Parsing.Expressions;

namespace Redwood.Framework.Tests.Binding
{
    [TestClass]
    public class BindingEvaluateVisitorTests
    {

        [TestMethod]
        public void BindingEvaluateVisitor_IndexerOnArray()
        {
            var target = CreateTestObject();
            var parser = new BindingParser();
            var eval = new BindingEvaluateVisitor();

            // property.arrayIndex
            var binding = (BindingMarkup)parser.ParseExpression("Child.ArrayOfIntegers[1]");
            Assert.AreEqual(2, eval.Visit(binding.Path, target));
        }

        [TestMethod]
        public void BindingEvaluateVisitor_IndexerOnIList()
        {
            var target = CreateTestObject();
            var parser = new BindingParser();
            var eval = new BindingEvaluateVisitor();

            // property.arrayIndex on List
            var binding = (BindingMarkup)parser.ParseExpression("Child.ListOfIntegers[0]");
            Assert.AreEqual(1, eval.Visit(binding.Path, target));
        }

        [TestMethod]
        public void BindingEvaluateVisitor_IndexerOnIEnumerable()
        {
            var target = CreateTestObject();
            var parser = new BindingParser();
            var eval = new BindingEvaluateVisitor();

            // property.arrayIndex on IEnumerable
            var binding = (BindingMarkup)parser.ParseExpression("Child.EnumerableOfValues[1].Value");
            Assert.AreEqual("b", eval.Visit(binding.Path, target));
        }

        [TestMethod]
        public void BindingEvaluateVisitor_IndexerOnly()
        {
            var target = CreateTestObject();
            var parser = new BindingParser();
            var eval = new BindingEvaluateVisitor();

            // arrayIndex on array
            var binding = (BindingMarkup)parser.ParseExpression("[1]");
            Assert.AreEqual(2, eval.Visit(binding.Path, target.Child.ArrayOfIntegers));
        }

        [TestMethod]
        public void BindingEvaluateVisitor_PropertyGet()
        {
            var target = CreateTestObject();
            var parser = new BindingParser();
            var eval = new BindingEvaluateVisitor();

            // arrayIndex on array
            var binding = (BindingMarkup)parser.ParseExpression("Child");
            Assert.AreEqual(target.Child, eval.Visit(binding.Path, target));
        }

        [TestMethod]
        public void BindingEvaluateVisitor_PropertyGetChain()
        {
            var target = CreateTestObject();
            var parser = new BindingParser();
            var eval = new BindingEvaluateVisitor();

            // arrayIndex on array
            var binding = (BindingMarkup)parser.ParseExpression("Child.ArrayOfIntegers");
            Assert.AreEqual(target.Child.ArrayOfIntegers, eval.Visit(binding.Path, target));
        }

        [TestMethod]
        public void BindingEvaluateVisitor_Empty()
        {
            var target = CreateTestObject();
            var parser = new BindingParser();
            var eval = new BindingEvaluateVisitor();

            // arrayIndex on array
            var binding = (BindingMarkup)parser.ParseExpression("");
            Assert.AreEqual(target, eval.Visit(binding.Path, target));
        }
        

        private static TestExpression1 CreateTestObject()
        {
            return new TestExpression1()
            {
                Child = new TestExpression2()
                {
                    ArrayOfIntegers = new[] { 1, 2, 3, 4, 5 },
                    ListOfIntegers = new List<int> { 1, 2, 3, 4, 5 },
                    EnumerableOfValues = new[] { new TestExpression1() { Value = "a" }, new TestExpression1() { Value = "b" } }
                }
            };
        }

        public class TestExpression1
        {
            public TestExpression2 Child { get; set; }

            public string Value { get; set; }

            public int Key { get; set; }
        }

        public class TestExpression2
        {
            public int[] ArrayOfIntegers { get; set; }
            
            public List<int> ListOfIntegers { get; set; }

            public IEnumerable<TestExpression1> EnumerableOfValues { get; set; }
        }



    }
}