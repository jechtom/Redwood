using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Redwood.Framework.Annotations;

namespace Redwood.Framework.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {



        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when the property is changed.
        /// </summary>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        /// <summary>
        /// Initializes the view model default values. This method is executed by the runtime before view model values from the postback are loaded.
        /// </summary>
        protected internal virtual void Init(Microsoft.Owin.IOwinContext context)
        {
        }

        /// <summary>
        /// Loads the view model data. This method is executed by the runtime after view model values from the postback are loaded.
        /// </summary>
        protected internal virtual async Task Load(Microsoft.Owin.IOwinContext context, bool isPostBack)
        {
        }

        /// <summary>
        /// Initializes the view model default values. This method is executed after all viewmodel data are loaded.
        /// </summary>
        protected internal virtual void PreRender(Microsoft.Owin.IOwinContext context)
        {
        }
    }
}
