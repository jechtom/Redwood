using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redwood.Framework.Controls;
using Redwood.Framework.Hosting;

namespace Redwood.Framework.Tests.Hosting
{
    [TestClass]
    public class CommandResolveTests
    {

        [TestMethod]
        public void ResolveCommand_Simple()
        {
            var serializer = new DefaultViewModelSerializer();
            var command = serializer.ResolveCommand(CreateTestInstance(), "Test", "$root", new object[] { });
            command();
            Assert.AreEqual(1, InvokedMethod);
        }

        [TestMethod]
        public void ResolveCommand_Array()
        {
            var serializer = new DefaultViewModelSerializer();
            var command = serializer.ResolveCommand(CreateTestInstance(), "Test", "$root.Items[1]", new object[] { });
            command();
            Assert.AreEqual(3, InvokedMethod);
        }

        [TestMethod]
        public void ResolveCommand_List()
        {
            var serializer = new DefaultViewModelSerializer();
            var command = serializer.ResolveCommand(CreateTestInstance(), "Test", "$root.Items2[0].Child", new object[] { });
            command();
            Assert.AreEqual(5, InvokedMethod);
        }


        private static TestViewModel CreateTestInstance()
        {
            return new TestViewModel(1)
            {
                Items = new []{ new TestViewModel(2), new TestViewModel(3) },
                Items2 = new List<TestViewModel>() { new TestViewModel(4) { Child = new TestViewModel(5) } }
            };
        }


        private static int InvokedMethod = 0;

        public class TestViewModel
        {
            private int uniqueId;

            public TestViewModel(int uniqueId)
            {
                this.uniqueId = uniqueId;
            }

            public TestViewModel[] Items { get; set; }

            public List<TestViewModel> Items2 { get; set; }

            public TestViewModel Child { get; set; }

            public void Test(RedwoodEventArgs args)
            {
                InvokedMethod = uniqueId;
            }
        }
    }
}
