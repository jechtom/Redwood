using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Redwood.Framework;

namespace Redwood.Samples.Basic.Samples_TH._03_Controls
{

    public class Layout : RedwoodPresenter
    {
        public override Task Load()
        {
            Root.DataContext = new LayoutViewModel();

            return base.Load();
        }
    }

    public class LayoutViewModel
    {

        public string Title { get; set; }

        public DateTime Date1 { get; set; }

        public DateTime Date2 { get; set; }

    }

}