using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redwood.Framework.Parsing;

namespace Redwood.Framework.RwHtml
{
    public class ControlTreeBuilder
    {
        public ControlTreeBuilder()
        {
            
        }

        public void Build(string rwhtml)
        {
            var tokenizer = new Parsing.RwHtmlTokenizer();
            //var tokenParser = new Parsing.RwHtmlMarkupParser();
            //var markupBuilder = new Markup.RwHtmlMarkupBuilder();

            var input = new StringTextReader(rwhtml);
            var tokenSource = tokenizer.Parse(input);
            //tokenParser.Read(tokenSource, markupBuilder);
        }
    }
}
