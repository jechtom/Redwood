using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Redwood.Framework.Hosting
{
    public interface IMarkupFileResolver
    {

        Task<string> GetMarkup(IOwinContext context, string applicationPhysicalPath);

    }
}