using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Redwood.Framework.Controls;
using Redwood.Framework.Parsing.RwHtml;

namespace Redwood.Framework.Hosting
{
    public class RedwoodApp : Microsoft.Owin.OwinMiddleware 
    {

        public IMarkupFileResolver MarkupFileResolver { get; set; }

        public IPageBuilder PageBuilder { get; set; }

        public IViewModelLocator ViewModelLocator { get; set; }

        public IViewModelSerializer ViewModelSerializer { get; set; }

        public IOutputRenderer OutputRenderer { get; set; }

        private string applicationPhysicalPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedwoodApp"/> class.
        /// </summary>
        public RedwoodApp(OwinMiddleware next, string applicationPhysicalPath)  : base(next)
        {
            this.applicationPhysicalPath = applicationPhysicalPath;
            MarkupFileResolver = new DefaultMarkupFileResolver();
            PageBuilder = new DefaultPageBuilder();
            ViewModelLocator = new DefaultViewModelLocator();
            ViewModelSerializer = new DefaultViewModelSerializer();
            OutputRenderer = new DefaultOutputRenderer();
        }

        /// <summary>
        /// Process an individual request.
        /// </summary>
        public override async Task Invoke(Microsoft.Owin.IOwinContext context)
        {
            if (context.Request.Path.Value.EndsWith(".rwhtml"))
            {
                // process the request
                await ProcessRequest(context);
            }
            else
            {
                await Next.Invoke(context);
            }
        }

        /// <summary>
        /// Renders the error response.
        /// </summary>
        public static async Task RenderErrorResponse(IOwinContext context, HttpStatusCode code, string details)
        {
            context.Response.StatusCode = (int)code;
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(string.Format("<html><head><title>Application error<title></head><body><h1>HTTP Error {0}</h1><p>{1}</p></body></html>", (int)code, WebUtility.HtmlEncode(details)));
        }

        /// <summary>
        /// Processes the request and renders the output.
        /// </summary>
        private async Task ProcessRequest(IOwinContext context)
        {
            // get the page markup
            var markup = await MarkupFileResolver.GetMarkup(context, applicationPhysicalPath);

            // build the page
            var page = PageBuilder.BuildPage(context, markup);

            // locate view model
            var viewModel = ViewModelLocator.LocateViewModel(context, page);

            // init the view model lifecycle
            page.DataContext = viewModel;

            viewModel.Init(context);
            if (context.Request.Method == "GET")
            {
                // standard get
                await viewModel.Load(context, false);
            }
            else if (context.Request.Method == "POST")
            {
                // postback
                Action invokedCommand;
                using (var sr = new StreamReader(context.Request.Body))
                {
                    ViewModelSerializer.DeseralizePostData(sr.ReadToEnd(), viewModel, out invokedCommand);
                }
                await viewModel.Load(context, true);

                // invoke the postback event
                invokedCommand();
            }
            else 
            {
                // unknown HTTP method
                await RenderErrorResponse(context, HttpStatusCode.MethodNotAllowed, "Only GET and POST methods are supported!");
                return;
            }
            viewModel.PreRender(context);

            // render the output
            var serializedViewModel = ViewModelSerializer.SerializeViewModel(viewModel);
            if (context.Request.Method == "GET")
            {
                // standard get
                await OutputRenderer.RenderPage(context, page, serializedViewModel);
            }
            else if (context.Request.Method == "POST")
            {
                // postback
                await OutputRenderer.RenderViewModel(context, page, serializedViewModel);
            }
        }
    }
}
