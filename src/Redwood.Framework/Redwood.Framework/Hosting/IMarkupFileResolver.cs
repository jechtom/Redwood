using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Redwood.Framework.Hosting
{
    public interface IMarkupFileLoader
    {

        Task<MarkupFile> GetMarkup(RedwoodRequestContext context, string applicationPhysicalPath);

    }
}