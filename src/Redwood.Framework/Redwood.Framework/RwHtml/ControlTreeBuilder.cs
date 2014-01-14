using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var tokenParser = new Parsing.RwHtmlTokenParser();
            var markupBuilder = new Markup.RwHtmlMarkupBuilder();

            var input = new Parsing.StringTextReader(rwhtml);
            var tokenSource = tokenizer.Parse(input);
            tokenParser.Read(tokenSource, markupBuilder);
        }
    }
}
