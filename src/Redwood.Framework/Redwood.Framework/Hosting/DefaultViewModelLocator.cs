using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Redwood.Framework.Controls;
using Redwood.Framework.ViewModel;

namespace Redwood.Framework.Hosting
{
    public class DefaultViewModelLocator : IViewModelLocator
    {
        public ViewModelBase LocateViewModel(IOwinContext context, Page page)
        {
            var integration = page.Controls.OfType<IntegrationScripts>().Single();
            return (ViewModelBase)Activator.CreateInstance(Type.GetType(integration.ViewModelTypeName, true));
        }
    }
}