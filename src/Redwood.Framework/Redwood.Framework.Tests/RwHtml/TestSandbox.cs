using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redwood.Framework.RwHtml;

namespace Redwood.Framework.Tests.RwHtml
{
    [TestClass]
    public class TestSandbox
    {
        [TestMethod]
        public void BuildControlTreeFromRwhtml1()
        {
            string rwhtml = @"<html><rw:TextBox Text=""Hello!""  Mode=""Password"" /><rw:ContentControl /><aaa xmlns:aaa=""clr-namespace:Redwood.Framework.Controls;assembly=Redwood.Framework""><aaa:ContentControl></aaa:ContentControl></aaa></html>";
            var builder = new ControlTreeBuilder();
            builder.Build(rwhtml);
        }
    }
}
