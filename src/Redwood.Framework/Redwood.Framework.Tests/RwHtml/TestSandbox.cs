using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redwood.Framework.RwHtml;
using Redwood.Framework.RwHtml.Markup;
using System.Diagnostics;
using System.Linq;
using Redwood.Framework.Controls;

namespace Redwood.Framework.Tests.RwHtml
{
    [TestClass]
    public class TestSandbox
    {
        //[TestMethod]
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
        public void SandBox2()
        {
            string[] rwhtmls = new string[] { 
@"<rw:ContentControl>
    <rw:TextBox Text=""Hello Redwood!"" />
</rw:ContentControl>",

@"<rw:ContentControl>
    <rw:TextBox>
        <rw:TextBox.Text>Hello Redwood!</rw:TextBox.Text>
    </rw:TextBox>
</rw:ContentControl>",

@"<rw:ContentControl>
    <rw:ContentControl.Content>
        <rw:TextBox Text=""Hello Redwood!"" />
    </rw:ContentControl.Content>
</rw:ContentControl>" };

            var serializer = new RwHtmlSerializer();
            foreach (var rwhtml in rwhtmls)
            {
                var result = (ContentControl)serializer.LoadFromString(rwhtml);

                Assert.IsInstanceOfType(result.Content, typeof(TextBox));
                Assert.AreEqual("Hello Redwood!", ((TextBox)result.Content).Text);
            }
        }

        [TestMethod]
        public void SandBox3()
        {
            string rwhtml =
@"<rw:ContentControl>
    <div class=""form"">
        <ul>
            <li>
                <rw:TextBox Text=""Hello Redwood!"" />
            </li>
        </ul>
    </div>
</rw:ContentControl>";
            rwhtml = new System.Net.WebClient().DownloadString("http://www.dotnetportal.cz/");

            rwhtml = "<rw:ContentControl>" + rwhtml + "</rw:ContentControl>";

            var serializer = new RwHtmlSerializer();
            var result = (ContentControl)serializer.LoadFromString(rwhtml);
            var writer = new Generation.HtmlWriter();
            result.Render(writer);
            var str = writer.ToString();
        }

        [TestMethod]
        public void SandBox4()
        {
            string rwhtml = 
            @"<html>
                <head>
                    <title>{Title}</title>
                </head>
                <body>
                    <rw:ContentControl />
                </body>
            </html>";
            var serializer = new RwHtmlSerializer();
            var result = (HtmlElement)serializer.LoadFromString(rwhtml);
            
        }

        [TestMethod]
        public void BuildControlTreeFromRwhtml1()
        {
            string rwhtml = @"<html><rw:TextBox Text=""Hello!""  Mode=""Password"" /><rw:ContentControl /><aaa xmlns:aaa=""clr-namespace:Redwood.Framework.Controls;assembly=Redwood.Framework""><aaa:ContentControl></aaa:ContentControl></aaa></html>";
            var serializer = new RwHtmlSerializer();
            var result = serializer.LoadFromString(rwhtml);
        }
    }
}
