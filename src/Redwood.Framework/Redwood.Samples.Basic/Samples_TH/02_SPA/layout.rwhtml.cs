using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Redwood.Framework;

namespace Redwood.Samples.Basic.Samples_TH._02_SPA
{
    // celý tenhle soubor (až na viewmodel) může být vygenerovaný nějakou šablonou, nebude potřeba do něj hrabat

    public class Layout : RedwoodPresenter
    {
        public override Task Load()
        {
            // Page bych přejmenoval na Root, ať to má smysl i u komponent
            Root.DataContext = new LayoutViewModel();

            return base.Load();
        }
    }

    public class LayoutViewModel
    {

        public string Title { get; set; }

    }

}