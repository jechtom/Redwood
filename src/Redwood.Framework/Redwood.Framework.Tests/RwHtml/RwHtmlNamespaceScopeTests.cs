using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redwood.Framework.RwHtml;

namespace Redwood.Framework.Tests.RwHtml
{
    [TestClass]
    public class RwHtmlNamespaceScopeTests
    {
        [TestMethod]
        public void StoreSinglePrefix()
        {
            var scope = new RwHtmlNamespaceScope();
            
            scope.PushScope(); // level == 1
            scope.AddNamespace("a", "aa");
            Assert.AreEqual("aa", scope.GetNamespaceByPrefix("a")); // found
            Assert.IsNull(scope.GetNamespaceByPrefix("b")); // not found
            scope.PopScope(); // level == 0
            Assert.IsNull(scope.GetNamespaceByPrefix("a")); // out of scope, not found
            
            scope.EnsureScopeLevelIsZero(); // ensure level == 0
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PushPopCountConsistencyLevel1()
        {
            var scope = new RwHtmlNamespaceScope();

            scope.PushScope(); // level == 1
            scope.EnsureScopeLevelIsZero(); // level == 1 - error
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PushPopCountConsistencyLevel2()
        {
            var scope = new RwHtmlNamespaceScope();

            scope.PushScope(); // level == 1
            scope.PushScope(); // level == 2
            scope.PopScope(); // level == 1
            scope.EnsureScopeLevelIsZero(); // level == 1 - error
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PushPopCountConsistencyLevel3()
        {
            var scope = new RwHtmlNamespaceScope();

            scope.PushScope(); // level == 1
            scope.PopScope(); // level == 0
            scope.PopScope(); // level == -1 - error
        }

        [TestMethod]
        public void OverridePrefix()
        {
            var scope = new RwHtmlNamespaceScope();

            scope.PushScope(); // level == 1
            scope.AddNamespace("a", "aa");

            scope.PushScope(); // level == 2
            scope.AddNamespace("a", "bb");

            Assert.AreEqual("bb", scope.GetNamespaceByPrefix("a"));

            scope.PopScope(); // level == 1
            Assert.AreEqual("aa", scope.GetNamespaceByPrefix("a"));

            scope.PopScope(); // level == 0
            Assert.IsNull(scope.GetNamespaceByPrefix("a"));

            scope.EnsureScopeLevelIsZero();
        }
    }
}
