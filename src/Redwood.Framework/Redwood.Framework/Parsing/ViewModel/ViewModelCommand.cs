using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Parsing.ViewModel
{
    public class ViewModelCommand
    {

        public string CommandName { get; set; }

        public ViewModelCommandParameter[] ParameterTypes { get; set; }

        public string ClientFunctionName { get; set; }

    }
}