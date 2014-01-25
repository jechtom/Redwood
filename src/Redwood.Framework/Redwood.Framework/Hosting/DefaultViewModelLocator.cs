using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Redwood.Framework.Controls;
using Redwood.Framework.ViewModel;

namespace Redwood.Framework.Hosting
{
    public class DefaultViewModelLoader : IViewModelLoader
    {
        public ViewModelBase LocateViewModel(RedwoodRequestContext context, Page page)
        {
            var type = context.Presenter.ResolveViewModelType();
            return (ViewModelBase)Activator.CreateInstance(type);
        }
    }
}