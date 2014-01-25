using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;
using Redwood.Framework.Controls;

namespace Redwood.Framework.Hosting
{
    public interface IOutputRenderer
    {

        Task RenderPage(RedwoodRequestContext context, Page page, string serializedViewModel);

        Task RenderViewModel(RedwoodRequestContext context, Page page, string serializedViewModel);
    }
}