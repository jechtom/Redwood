using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Redwood.Framework.Hosting
{
    public class RedwoodApp : Microsoft.Owin.OwinMiddleware
    {

        private RedwoodConfiguration configuration;
        private string applicationPhysicalPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedwoodApp"/> class.
        /// </summary>
        public RedwoodApp(OwinMiddleware next, string applicationPhysicalPath) : this(next, applicationPhysicalPath, RedwoodConfiguration.Default)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedwoodApp"/> class.
        /// </summary>
        public RedwoodApp(OwinMiddleware next, string applicationPhysicalPath, RedwoodConfiguration configuration) : base(next)
        {
            this.applicationPhysicalPath = applicationPhysicalPath;
            this.configuration = configuration;
        }

        /// <summary>
        /// Process an individual request.
        /// </summary>
        public override async Task Invoke(IOwinContext context)
        {
            // try resolve the route
            var url = context.Request.Path.Value.TrimStart('/').TrimEnd('/');
            IDictionary<string, object> parameters;
            var route = configuration.RouteTable.FirstOrDefault(r => r.IsMatch(url, out parameters));

            if (route != null)
            {
                // handle the request
                await route.ProcessRequest(new RedwoodRequestContext() { OwinContext = context, ApplicationPhysicalPath = applicationPhysicalPath });
            }
            else
            {
                // we cannot handle the request, pass it to another component
                await Next.Invoke(context);
            }
        }

    }
}
