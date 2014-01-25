using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redwood.Framework.Routing;

namespace Redwood.Framework
{
    /// <summary>
    /// Represents the table of routes.
    /// </summary>
    public class RedwoodRouteTable : IEnumerable<RouteBase>
    {

        private List<KeyValuePair<string, RouteBase>> list = new List<KeyValuePair<string, RouteBase>>();


        /// <summary>
        /// Adds the specified name.
        /// </summary>
        public void Add(string name, RouteBase route)
        {
            list.Add(new KeyValuePair<string, RouteBase>(name, route));
        }


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<RouteBase> GetEnumerator()
        {
            return list.Select(l => l.Value).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
