using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Parsing.ViewModel
{
    public interface IViewModelMetadataExtractor
    {

        IEnumerable<Type> GetDependentTypes(Type type);

        IEnumerable<ViewModelProperty> GetProperties(Type type);

        IEnumerable<ViewModelCommand> GetCommands(Type type);

    }
}