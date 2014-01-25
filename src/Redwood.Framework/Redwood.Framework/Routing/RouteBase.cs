﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Redwood.Framework.Hosting;

namespace Redwood.Framework.Routing
{
    public abstract class RouteBase
    {

        /// <summary>
        /// Gets the URL pattern for the route.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Gets the default values of the optional parameters.
        /// </summary>
        public IDictionary<string, object> DefaultValues { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="RouteBase"/> class.
        /// </summary>
        public RouteBase(string url, object defaultValues = null)
            : this(url, new Dictionary<string, object>())
        {
            AddOrUpdateParameterCollection(DefaultValues, defaultValues);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteBase"/> class.
        /// </summary>
        public RouteBase(string url, IDictionary<string, object> defaultValues)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (defaultValues == null)
                throw new ArgumentNullException("defaultValues");

            Url = url;
            DefaultValues = defaultValues;
        }



        /// <summary>
        /// Gets the names of the route parameters in the order in which they appear in the URL.
        /// </summary>
        public abstract IEnumerable<string> ParameterNames { get; }

        /// <summary>
        /// Determines whether the route matches to the specified URL and extracts the parameter values.
        /// </summary>
        public abstract bool IsMatch(string url, out IDictionary<string, object> values);


        /// <summary>
        /// Builds the URL with the specified parameters.
        /// </summary>
        public string BuildUrl(IDictionary<string, object> currentRouteValues, IDictionary<string, object> newRouteValues)
        {
            if (currentRouteValues == null)
                throw new ArgumentNullException("currentRouteValues");
            if (newRouteValues == null)
                throw new ArgumentNullException("newRouteValues");

            var values = new Dictionary<string, object>(DefaultValues);
            AddOrUpdateParameterCollection(values, currentRouteValues);
            AddOrUpdateParameterCollection(values, newRouteValues);
            
            return BuildUrlCore(values);
        }
        
        /// <summary>
        /// Builds the URL.
        /// </summary>
        public string BuildUrl(IDictionary<string, object> currentRouteValues, object newRouteValues)
        {
            if (currentRouteValues == null)
                throw new ArgumentNullException("currentRouteValues");

            var values = new Dictionary<string, object>(DefaultValues);
            AddOrUpdateParameterCollection(values, currentRouteValues);
            AddOrUpdateParameterCollection(values, newRouteValues);

            return BuildUrl(values);
        }

        /// <summary>
        /// Builds the URL.
        /// </summary>
        public string BuildUrl(object routeValues)
        {
            var values = new Dictionary<string, object>(DefaultValues);
            AddOrUpdateParameterCollection(values, routeValues);

            return BuildUrl(values);
        }

        /// <summary>
        /// Builds the URL with the specified parameters.
        /// </summary>
        public string BuildUrl(IDictionary<string, object> routeValues)
        {
            if (routeValues == null)
                throw new ArgumentNullException("routeValues");

            var values = new Dictionary<string, object>(DefaultValues);
            AddOrUpdateParameterCollection(values, routeValues);

            return BuildUrlCore(values);
        }

        /// <summary>
        /// Builds the URL core from the parameters.
        /// </summary>
        /// <remarks>The default values are already included in the <paramref name="values"/> collection.</remarks>
        protected abstract string BuildUrlCore(Dictionary<string, object> values);





        /// <summary>
        /// Adds or updates the parameter collection with the specified values from the anonymous object.
        /// </summary>
        public static void AddOrUpdateParameterCollection(IDictionary<string, object> targetCollection, object anonymousObject)
        {
            if (anonymousObject != null)
            {
                foreach (var prop in anonymousObject.GetType().GetProperties())
                {
                    targetCollection[prop.Name] = prop.GetValue(anonymousObject);
                }
            }
        }

        /// <summary>
        /// Adds or updates the parameter collection with the specified values from the other parameter collection.
        /// </summary>
        public static void AddOrUpdateParameterCollection(IDictionary<string, object> targetCollection, IDictionary<string, object> newValues)
        {
            foreach (var pair in newValues)
            {
                targetCollection[pair.Key] = pair.Value;
            }
        }


        /// <summary>
        /// Processes the request.
        /// </summary>
        public abstract Task ProcessRequest(RedwoodRequestContext context);

    }
}