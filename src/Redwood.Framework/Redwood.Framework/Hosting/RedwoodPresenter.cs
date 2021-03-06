﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Redwood.Framework.Parsing;

namespace Redwood.Framework.Hosting
{
    public abstract class RedwoodPresenter
    {

        /// <summary>
        /// Gets the presenter relative path within the assembly.
        /// </summary>
        public string PresenterPath
        {
            get
            {
                var type = GetType();
                var assemblyName = type.Assembly.FullName.Substring(0, type.Assembly.FullName.IndexOf(','));
                if (!type.FullName.StartsWith(assemblyName))
                {
                    throw new ArgumentException("The presenter name could not be resolved automatically because it is not in the namespace of the same name as the assembly!");
                }

                var name = type.FullName.Substring(assemblyName.Length).TrimStart('.');
                if (!name.EndsWith("Presenter"))
                {
                    throw new ArgumentException("The presenter name could not be resolved automatically because its name does not end with 'Presenter'!");
                }
                name = name.Substring(0, name.Length - "Presenter".Length);

                return name;
            }
        }

        /// <summary>
        /// Gets the presenter full path within the assembly.
        /// </summary>
        public string FullPresenterPath
        {
            get
            {
                var type = GetType();
                var assemblyName = type.Assembly.FullName.Substring(0, type.Assembly.FullName.IndexOf(','));
                if (!type.FullName.StartsWith(assemblyName))
                {
                    throw new ArgumentException("The presenter name could not be resolved automatically because it is not in the namespace of the same name as the assembly!");
                }

                var name = type.FullName;
                if (!name.EndsWith("Presenter"))
                {
                    throw new ArgumentException("The presenter name could not be resolved automatically because its name does not end with 'Presenter'!");
                }
                name = name.Substring(0, name.Length - "Presenter".Length);

                return name;
            }
        }


        public IMarkupFileLoader MarkupFileLoader { get; set; }

        public IPageBuilder PageBuilder { get; set; }

        public IViewModelLoader ViewModelLoader { get; set; }

        public IViewModelSerializer ViewModelSerializer { get; set; }

        public IOutputRenderer OutputRenderer { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="RedwoodPresenter"/> class.
        /// </summary>
        public RedwoodPresenter()
        {
            MarkupFileLoader = new DefaultMarkupFileLoader();
            PageBuilder = new DefaultPageBuilder();
            ViewModelLoader = new DefaultViewModelLoader();
            ViewModelSerializer = new DefaultViewModelSerializer();
            OutputRenderer = new DefaultOutputRenderer();
        }


        /// <summary>
        /// Resolves the name of the view file.
        /// </summary>
        public virtual string ResolveViewFileName()
        {
            return PresenterPath.Replace(".", "\\") + ".rwhtml";
        }

        /// <summary>
        /// Resolves the type of the view model.
        /// </summary>
        public virtual Type ResolveViewModelType()
        {
            var typeName = FullPresenterPath + "ViewModel";
            return GetType().Assembly.GetType(typeName, true);
        }


        /// <summary>
        /// Processes the request.
        /// </summary>
        public async Task ProcessRequest(RedwoodRequestContext context)
        {
            Exception error = null;
            try
            {
                await ProcessRequestCore(context);
            }
            catch (Exception ex)
            {
                error = ex;
            }

            if (error != null)
            {
                await RenderErrorResponse(context, HttpStatusCode.InternalServerError, error);
            }
        }


        /// <summary>
        /// Renders the error response.
        /// </summary>
        public static async Task RenderErrorResponse(RedwoodRequestContext context, HttpStatusCode code, Exception error)
        {
            context.OwinContext.Response.StatusCode = (int)code;
            context.OwinContext.Response.ContentType = "text/html";

            var template = new ErrorPageTemplate()
            {
                Error = error,
                ErrorCode = (int)code,
                ErrorDescription = code.ToString(),
                IpAddress = context.OwinContext.Request.RemoteIpAddress,
                CurrentUserName = context.OwinContext.Request.User.Identity.Name,
                Url = context.OwinContext.Request.Uri.ToString(),
                Verb = context.OwinContext.Request.Method
            };
            if (error is ParserException)
            {
                template.FileName = ((ParserException)error).FileName;
                template.LineNumber = ((ParserException)error).Position.LineNumber;
                template.PositionOnLine = ((ParserException)error).Position.PositionOnLine;
            }
            
            var text = template.TransformText();
            await context.OwinContext.Response.WriteAsync(text);
        }

        /// <summary>
        /// Processes the request and renders the output.
        /// </summary>
        private async Task ProcessRequestCore(RedwoodRequestContext context)
        {
            // get the page markup
            var markup = await MarkupFileLoader.GetMarkup(context, context.ApplicationPhysicalPath);

            // build the page
            var page = PageBuilder.BuildPage(context, markup);

            // locate view model
            var viewModel = ViewModelLoader.LocateViewModel(context, page);

            // init the view model lifecycle
            page.DataContext = viewModel;

            viewModel.Init(context);
            if (context.OwinContext.Request.Method == "GET")
            {
                // standard get
                await viewModel.Load(context, false);
            }
            else if (context.OwinContext.Request.Method == "POST")
            {
                // postback
                Action invokedCommand;
                using (var sr = new StreamReader(context.OwinContext.Request.Body))
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
                await RenderErrorResponse(context, HttpStatusCode.MethodNotAllowed, new RedwoodHttpException("Only GET and POST methods are supported!"));
                return;
            }
            viewModel.PreRender(context);

            // render the output
            var serializedViewModel = ViewModelSerializer.SerializeViewModel(viewModel);
            if (context.OwinContext.Request.Method == "GET")
            {
                // standard get
                await OutputRenderer.RenderPage(context, page, serializedViewModel);
            }
            else if (context.OwinContext.Request.Method == "POST")
            {
                // postback
                await OutputRenderer.RenderViewModel(context, page, serializedViewModel);
            }
        }
    }
}
