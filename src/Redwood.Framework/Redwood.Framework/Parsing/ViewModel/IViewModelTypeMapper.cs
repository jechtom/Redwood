using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Parsing.ViewModel
{
    public interface IViewModelTypeMapper
    {

        string MapType(Type type);

    }
}