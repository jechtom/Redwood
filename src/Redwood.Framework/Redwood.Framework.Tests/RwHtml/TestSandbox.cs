using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redwood.Framework.RwHtml;
using Redwood.Framework.RwHtml.Markup;
using System.Diagnostics;
using System.Linq;

namespace Redwood.Framework.Tests.RwHtml
{
    [TestClass]
    public class TestSandbox
    {
        [TestMethod]
        public void SandBox1()
        {
            string rwhtml = @"<html><rw:TextBox Text=""Hello!""  Mode=""Password"" /><rw:ContentControl /><aaa xmlns:aaa=""clr-namespace:Redwood.Framework.Controls;assembly=Redwood.Framework""><aaa.Prop1>Něco</aaa.Prop1><aaa:ContentControl></aaa:ContentControl></aaa></html>";


            rwhtml = @"
<rw:Container Layout.Source=""~/Views/Layout.rwhtml"" xmlns:my=""clr-namespace:MyAssembly;assembly=MyAssembly"">
  <my:CustomLayout.Something>SomethingElse</my:CustomLayout.Something>
  <div class=""inside-container"">
    <my:CustomTableControl Title=""My Table"">
      Hello Redwood!
      <my:CustomTableControl.RowTemplate>
        <div>{RawBinding Content}</div>
      </my:CustomTableControl.RowTemplate>
    </my:CustomTableControl>
  </div>
</rw:Container>
";

            //rwhtml = new System.Net.WebClient().DownloadString("http://www.dotnetportal.cz/");

            var serializer = new RwHtmlSerializer();
            serializer.LoadFromString(rwhtml);
            
            //Debug.WriteLine("----------------");


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
