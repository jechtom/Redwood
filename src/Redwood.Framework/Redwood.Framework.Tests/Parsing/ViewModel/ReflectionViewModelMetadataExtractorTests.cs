using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redwood.Framework.Parsing.ViewModel;
using Redwood.Framework.ViewModel;

namespace Redwood.Framework.Tests.Parsing.ViewModel
{
    [TestClass]
    public class ReflectionViewModelMetadataExtractorTests
    {

        [TestMethod]
        public void DependentTypesTest()
        {
            var extractor = new ReflectionViewModelMetadataExtractor();
            var types = extractor.GetDependentTypes(typeof (TestViewModel)).ToList();
            Assert.AreEqual(3, types.Count);
            Assert.IsTrue(types.Contains(typeof(A)));
            Assert.IsTrue(types.Contains(typeof(B)));
        }

        [TestMethod]
        public void GetPropertiesTest()
        {
            var extractor = new ReflectionViewModelMetadataExtractor();
            var props = extractor.GetProperties(typeof (TestViewModel)).OrderBy(p => p.PropertyName).ToList();
            Assert.AreEqual(3, props.Count);
            Assert.AreEqual("1 * 2", props[2].ClientImplementation);
            Assert.IsTrue(props[2].IsReadOnly);
        }

        [TestMethod]
        public void GetCommandsTest()
        {
            var extractor = new ReflectionViewModelMetadataExtractor();
            var commands = extractor.GetCommands(typeof(TestViewModel)).OrderBy(p => p.CommandName).ToList();
            Assert.AreEqual(2, commands.Count);
            Assert.AreEqual("JSMethod", commands[1].ClientFunctionName);
        }


        public class TestViewModel
        {

            public string Prop1 { get; set; }

            public List<A> Items { get; set; }

            [ClientImplementation("1 * 2")]
            public int RunningTotal
            {
                get { return 1 * 2; }
            }

            public void Command1()
            {
                
            }

            [ClientImplementation("JSMethod")]
            public void Command2()
            {

            }
        }

        public class A
        {

            public TestViewModel Parent { get; set; }

            public B B { get; set; }

        }

        public class B
        {
            public string Prop2 { get; set; }
        }

    }
}
