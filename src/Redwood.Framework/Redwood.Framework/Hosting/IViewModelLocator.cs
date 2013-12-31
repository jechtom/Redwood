using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Redwood.Framework.Controls;
using Redwood.Framework.ViewModel;

namespace Redwood.Framework.Hosting
{
    public interface IViewModelLocator
    {

        ViewModelBase LocateViewModel(IOwinContext context, Page page);

    }
}