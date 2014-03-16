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
        public async Task<MarkupFile> GetMarkup(RedwoodRequestContext context, string applicationPhysicalPath)
        {
            try
            {
                var fileName = Path.Combine(applicationPhysicalPath, context.Presenter.ResolveViewFileName());
                using (var sr = new StreamReader(fileName, Encoding.UTF8))
                {
                    return new MarkupFile()
                    {
                        Contents = await sr.ReadToEndAsync(),
                        FileName = fileName
                    };
                }
            }
            catch (IOException ex)
            {
                throw new RedwoodHttpException("The markup file '{0}' could not be loaded. See InnerException for details.", ex);
            }
        }
    }
}