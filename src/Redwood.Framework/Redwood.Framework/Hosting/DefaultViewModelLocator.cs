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
            Type type;
            try
            {
                type = context.Presenter.ResolveViewModelType();
            }
            catch (Exception ex)
            {
                throw new RedwoodHttpException("The viewmodel class for requested page was not found!", ex);
            }

            try
            {
                return (ViewModelBase)Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                throw new RedwoodHttpException(string.Format("The instance of viewmodel '{0}' could not be created. There is not a default parameterless constructor, or an exception was thrown when the constructor was called. See InnerException for details.", type.FullName), ex);
            }
        }
    }
}