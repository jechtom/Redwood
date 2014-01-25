using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Redwood.Framework.Hosting;
using Redwood.Framework.Routing;

namespace Redwood.Framework
{
    public static class RedwoodExtensions
    {

        /// <summary>
        /// Maps the page route.
        /// </summary>
        public static void MapPageRoute(this RedwoodRouteTable routeTable, string name, string url, object parameters, Func<RedwoodPresenter> presenterFactory)
        {
            routeTable.Add(name, new RedwoodRoute(url, parameters, presenterFactory));
        }

        /// <summary>
        /// Maps the page route.
        /// </summary>
        public static void MapPageRoute<TPresenter>(this RedwoodRouteTable routeTable, string url, object parameters = null) where TPresenter : RedwoodPresenter, new()
        {
            routeTable.Add(typeof(TPresenter).FullName, new RedwoodRoute(url, parameters, () => new TPresenter()));
        }

    }
}
