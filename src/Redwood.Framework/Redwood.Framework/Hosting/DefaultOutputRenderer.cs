using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Redwood.Framework.Controls;
using Redwood.Framework.Generation;

namespace Redwood.Framework.Hosting
{
    public class DefaultOutputRenderer : IOutputRenderer
    {
        public async Task RenderPage(RedwoodRequestContext context, Page page, string serializedViewModel)
        {
            // set up integration scripts
            //var integrationScripts = page.OfType<IntegrationScripts>().Single();
            //integrationScripts.SerializedViewModel = serializedViewModel;
            //integrationScripts.InternalScriptUrls = new List<string>() {
            //    context.OwinContext.Request.PathBase + "/Scripts/knockout-3.0.0.js",
            //    context.OwinContext.Request.PathBase + "/Scripts/knockout.mapping-latest.js",
            //    context.OwinContext.Request.PathBase + "/Scripts/knockout.validation.js",
            //    context.OwinContext.Request.PathBase + "/Scripts/Redwood.js",
            //    context.OwinContext.Request.PathBase + "/Data.js"
            //};

            // get the HTML
            var writer = new HtmlWriter();
            page.Render(writer);
            var html = writer.ToString();

            // return the response
            context.OwinContext.Response.ContentType = "text/html; charset=utf-8";
            await context.OwinContext.Response.WriteAsync(html);
        }

        public async Task RenderViewModel(RedwoodRequestContext context, Page page, string serializedViewModel)
        {
            // return the response
            context.OwinContext.Response.ContentType = "application/json; charset=utf-8";
            await context.OwinContext.Response.WriteAsync(serializedViewModel);
        }
    }
}