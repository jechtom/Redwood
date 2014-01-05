using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Redwood.Framework;

namespace Redwood.Samples.Basic.Samples_TH._02_SPA
{
    // celý tenhle soubor (až na viewmodel a nastavení title) může být vygenerovaný nějakou šablonou, nebude potřeba do něj hrabat

    public partial class Page1 : RedwoodContentPresenter  
    {

        protected override RedwoodPresenter ResolveMasterPresenter()
        {
            return new Layout();
        }

        public override Task Load()
        {
            // přes Master.DataContext lze přistupovat k viewmodelu rodičovské stránky (tohle by taky mohla umět ta bázová třída)
            ((LayoutViewModel)Master.Root.DataContext).Title = "Moje stránka";

            Content.DataContext = new Page1ViewModel();     // přístup ke komponentě ve stránce pomocí ID

            return base.Load();
        }
    }

    public class Page1ViewModel
    {

        public string TestProperty1 { get; set; }

    }
}