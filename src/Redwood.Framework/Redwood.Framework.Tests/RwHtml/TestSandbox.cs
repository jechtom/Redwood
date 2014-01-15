using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redwood.Framework.RwHtml;
using Redwood.Framework.RwHtml.Markup;
using System.Diagnostics;

namespace Redwood.Framework.Tests.RwHtml
{
    [TestClass]
    public class TestSandbox
    {
        [TestMethod]
        public void SandBox1()
        {
            string rwhtml = @"<html><rw:TextBox Text=""Hello!""  Mode=""Password"" /><rw:ContentControl /><aaa xmlns:aaa=""clr-namespace:Redwood.Framework.Controls;assembly=Redwood.Framework""><aaa:ContentControl></aaa:ContentControl></aaa></html>";

            var tokenizer = new Redwood.Framework.RwHtml.Parsing.RwHtmlTokenizer();
            var parser = new Redwood.Framework.RwHtml.Parsing.RwHtmlTokenToMarkupParser();
            var m = new Redwood.Framework.RwHtml.Markup.MarkupSorter();

            Debug.WriteLine("----------------");
            parser.Read(tokenizer.Parse(rwhtml)).DebugOutput();
            Debug.WriteLine("----------------");
            m.Sort(parser.Read(tokenizer.Parse(rwhtml))).DebugOutput();
            Debug.WriteLine("----------------");
        }

        [TestMethod]
        public void BuildControlTreeFromRwhtml1()
        {
            string rwhtml = @"<html><rw:TextBox Text=""Hello!""  Mode=""Password"" /><rw:ContentControl /><aaa xmlns:aaa=""clr-namespace:Redwood.Framework.Controls;assembly=Redwood.Framework""><aaa:ContentControl></aaa:ContentControl></aaa></html>";
            var builder = new ControlTreeBuilder();
            builder.Build(rwhtml);
        }
    }
}
