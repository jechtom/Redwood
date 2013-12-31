using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Controls;
using Redwood.Framework.ViewModel;

namespace Redwood.Framework.Hosting
{
    public interface IViewModelSerializer
    {

        string SerializeViewModel(ViewModelBase viewModel);

        void DeseralizePostData(string data, ViewModelBase target, out Action invokedCommand);

    }
}