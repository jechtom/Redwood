using System;
using System.Web.Hosting;
using Microsoft.Owin;
using Owin;
using Redwood.Framework;
using Redwood.Framework.Hosting;
using System.Collections.Generic;

[assembly: OwinStartup(typeof(Redwood.Samples.Basic.Startup))]

namespace Redwood.Samples.Basic
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use<RedwoodApp>(HostingEnvironment.ApplicationPhysicalPath);
            app.UseStaticFiles();

            RedwoodConfiguration.Default.RouteTable.MapPageRoute<TaskListPresenter>("");
        }
    }
}
