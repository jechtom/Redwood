using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Hosting
{
    public class DefaultMarkupFileLoader : IMarkupFileLoader
    {
        public async Task<string> GetMarkup(RedwoodRequestContext context, string applicationPhysicalPath)
        {
            var fileName = Path.Combine(applicationPhysicalPath, context.Presenter.ResolveViewFileName());
            using (var sr = new StreamReader(fileName, Encoding.UTF8))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }
}