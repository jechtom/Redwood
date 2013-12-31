using System;
using System.Threading.Tasks;
using System.Web.Hosting;
using Microsoft.Owin;
using Owin;
using Redwood.Framework.Hosting;

[assembly: OwinStartup(typeof(Redwood.Samples.Basic.Startup))]

namespace Redwood.Samples.Basic
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use<RedwoodApp>(HostingEnvironment.ApplicationPhysicalPath);
            app.UseStaticFiles();
        }
    }
}
