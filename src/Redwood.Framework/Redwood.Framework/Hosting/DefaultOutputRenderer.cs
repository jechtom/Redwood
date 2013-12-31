using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;
using Redwood.Framework.Controls;
using Redwood.Framework.Generation;

namespace Redwood.Framework.Hosting
{
    public class DefaultOutputRenderer : IOutputRenderer
    {
        public async Task RenderPage(IOwinContext context, Page page, string serializedViewModel)
        {
            // set up integration scripts
            var integrationScripts = page.Controls.OfType<IntegrationScripts>().Single();
            integrationScripts.SerializedViewModel = serializedViewModel;
            integrationScripts.InternalScriptUrls = new List<string>() {
                context.Request.PathBase + "/Scripts/knockout-3.0.0.js",
                context.Request.PathBase + "/Scripts/knockout.mapping-latest.js",
                context.Request.PathBase + "/Scripts/knockout.validation.js",
                context.Request.PathBase + "/Scripts/Redwood.js",
                context.Request.PathBase + "/Data.js"
            };

            // get the HTML
            var writer = new HtmlWriter();
            page.Render(writer);
            var html = writer.ToString();

            // return the response
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync(html);
        }

        public async Task RenderViewModel(IOwinContext context, Page page, string serializedViewModel)
        {
            // return the response
            context.Response.ContentType = "application/json; charset=utf-8";
            await context.Response.WriteAsync(serializedViewModel);
        }
    }
}