using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Redwood.Framework.Controls;

namespace Redwood.Samples.Basic.Samples_TH
{
    public abstract class RedwoodPresenter
    {
        public virtual async Task Load()
        {
        }

        protected virtual async Task LoadView(string path)
        {
            // read markup
            var markup = await MarkupFileResolver.GetMarkup(Context, path);

            // build the page
            var page = PageBuilder.BuildPage(Context, markup);
            this.Page = page;
        }

        public Hosting.IMarkupFileResolver MarkupFileResolver { get; set; }

        public Hosting.IPageBuilder PageBuilder { get; set; }

        public RedwoodPresenter Page { get; set; }      // presenter, na který přišel HTTP request

        public ContainerControl Root { get; set; }      // komponenta, která obsahuje veškerý obsah v RwHtml

        public IOwinContext Context { get; set; }

    }
}