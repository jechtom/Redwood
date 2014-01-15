using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml
{
    /// <summary>
    /// Reads rwhtml document and creates an CLR object.
    /// </summary>
    public class RwHtmlReader
    {
        public object Load(string rwhtml)
        {
            ControlTreeBuilder treeBuilder = new ControlTreeBuilder();
            treeBuilder.Build(rwhtml);
            throw new NotImplementedException();
        }
    }
}
