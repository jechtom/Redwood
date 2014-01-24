using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redwood.Framework.Binding;
using Redwood.Framework.Binding.Parsing;
using Redwood.Framework.Binding.Parsing.Expressions;

namespace Redwood.Framework.Tests.Binding
{
    [TestClass]
    public class BindingParserTests
    {

        [TestMethod]
        public void BindingParser_NoParameters()
        {
            var parser = new BindingParser();
            var binding = (BindingMarkupExpression)parser.ParseExpression("Binding");
            Assert.IsNull(binding.Path);
        }

        [TestMethod]
        public void BindingParser_BindingWithDefaultProperty()
        {
            var parser = new BindingParser();
            var binding = (BindingMarkupExpression)parser.ParseExpression("Binding Test.Test2, Mode=TwoWay");

            Assert.AreEqual(BindingFlags.TwoWay, binding.Mode);

            dynamic path = binding.Path;

            Assert.IsInstanceOfType(path, typeof(BindingGetPropertyExpression));
            Assert.AreEqual("Test", path.PropertyName);

            Assert.IsInstanceOfType(path.NextExpression, typeof(BindingGetPropertyExpression));
            Assert.AreEqual("Test2", path.NextExpression.PropertyName);
        }

        [TestMethod]
        public void BindingParser_BindingWithSpecifiedPropertyName()
        {
            var parser = new BindingParser();
            var binding = (BindingMarkupExpression)parser.ParseExpression("Binding Path=Test.Test2");

            dynamic path = binding.Path;

            Assert.IsInstanceOfType(path, typeof(BindingGetPropertyExpression));
            Assert.AreEqual("Test", path.PropertyName);

            Assert.IsInstanceOfType(path.NextExpression, typeof(BindingGetPropertyExpression));
            Assert.AreEqual("Test2", path.NextExpression.PropertyName);
        }
    }
}
