using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Redwood.Framework.Hosting;

namespace Redwood.Framework.ViewModel
{
    public abstract class ViewModelBase
    {
        
        /// <summary>
        /// Initializes the view model default values. This method is executed by the runtime before view model values from the postback are loaded.
        /// </summary>
        protected internal virtual void Init(RedwoodRequestContext context)
        {
        }

        /// <summary>
        /// Loads the view model data. This method is executed by the runtime after view model values from the postback are loaded.
        /// </summary>
        protected internal virtual async Task Load(RedwoodRequestContext context, bool isPostBack)
        {
        }

        /// <summary>
        /// Initializes the view model default values. This method is executed after all viewmodel data are loaded.
        /// </summary>
        protected internal virtual void PreRender(RedwoodRequestContext context)
        {
        }
    }
}
