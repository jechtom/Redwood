﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Controls
{
    public class IntegrationScripts : RedwoodControl
    {

        public string ViewModelTypeName { get; set; }

        public string ViewModelClientName
        {
            get
            {
                var type = Type.GetType(ViewModelTypeName);
                return type.Name;       // TODO: support folder hierarchy
            }
        }

        internal List<string> InternalScriptUrls { get; set; }

        internal string SerializedViewModel { get; set; }


        public override void Render(Generation.IHtmlWriter writer)
        {
            foreach (var script in InternalScriptUrls)
            {
                writer.RenderBeginTag("script");
                writer.AddAttribute("type", "text/javascript");
                writer.AddAttribute("src", script);
                writer.RenderEndTag(true);
            }
            
            writer.RenderBeginTag("script");
            writer.AddAttribute("type", "text/javascript");
            writer.WriteText("(function () { var viewModelData = ", false);
            writer.WriteText(SerializedViewModel, false);
            writer.WriteText(";var vm = Redwood.CreateViewModel(viewModelData, new " + ViewModelClientName + "());ko.applyBindings(vm);Redwood.ViewModels['Default'] = vm;})();", false);
            writer.RenderEndTag();
        }
    }
}