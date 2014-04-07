using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Hosting.ViewModel
{
    /// <summary>
    /// Represents a primitive value node in the view model.
    /// </summary>
    public class ViewModelMapPrimitiveNode : ViewModelMapNode
    {
        public object Value { get; set; }
        
    }
}