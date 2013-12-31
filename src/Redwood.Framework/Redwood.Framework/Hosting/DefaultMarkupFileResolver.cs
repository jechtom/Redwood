using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Redwood.Framework.Hosting
{
    public class DefaultMarkupFileResolver : IMarkupFileResolver
    {
        public async Task<string> GetMarkup(IOwinContext context, string applicationPhysicalPath)
        {
            var fileName = Path.Combine(applicationPhysicalPath, context.Request.Path.Value.TrimStart('/'));
            using (var sr = new StreamReader(fileName, Encoding.UTF8))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }
}