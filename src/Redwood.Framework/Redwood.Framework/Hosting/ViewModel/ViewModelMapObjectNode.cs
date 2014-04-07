using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Hosting.ViewModel
{
    public class ViewModelMapObjectNode : ViewModelMapNode
    {
        
        public Dictionary<string, ViewModelMapNode> Properties { get; set; }

        public ViewModelMapObjectNode()
        {
            Properties = new Dictionary<string, ViewModelMapNode>();
        }

    }
}
