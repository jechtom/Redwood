using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redwood.Framework.Routing;

namespace Redwood.Framework
{
    public class RedwoodConfiguration
    {
        #region Static

        private static RedwoodConfiguration defaultInstance = new RedwoodConfiguration();

        public static RedwoodConfiguration Default
        {
            get { return defaultInstance; }
        }

        #endregion



        /// <summary>
        /// Gets the route table.
        /// </summary>
        public RedwoodRouteTable RouteTable { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="RedwoodConfiguration"/> class.
        /// </summary>
        public RedwoodConfiguration()
        {
            RouteTable = new RedwoodRouteTable();
        }


    }
}
