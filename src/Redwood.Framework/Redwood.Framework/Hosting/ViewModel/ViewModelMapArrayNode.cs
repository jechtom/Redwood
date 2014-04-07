using System;
using System.Collections.Generic;
using System.Linq;

namespace Redwood.Framework.Hosting.ViewModel
{
    public class ViewModelMapArrayNode : ViewModelMapNode
    {

        public List<ViewModelMapNode> Items { get; private set; }

        public Dictionary<string, ViewModelMapNode> KeyMap { get; private set; }


        public ViewModelMapArrayNode()
        {
            Items = new List<ViewModelMapNode>();
            KeyMap = new Dictionary<string, ViewModelMapNode>();
        }

    }
}