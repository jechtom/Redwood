using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Redwood.Framework;

namespace Redwood.Samples.Basic.Samples_TH._03_Controls
{

    public class Calendar : RedwoodUserControlPresenter
    {
        public override Task Load()
        {
            Root.DataContext = new CalendarViewModel();

            return base.Load();
        }
    }

    public class CalendarViewModel
    {

        public int VisibleYear { get; set; }

        public int VisibleMonth { get; set; }

        public DateTime SelectedDate { get; set; }

    }

}