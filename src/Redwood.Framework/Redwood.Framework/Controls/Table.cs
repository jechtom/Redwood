using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Controls
{
    public class Table : RedwoodControl
    {

        [Bindable(true)]
        public ICollection DataSource { get; set; }

        public RedwoodTemplate RowTemplate { get; set; }


        /// <summary>
        /// Renders the specified writer.
        /// </summary>
        public override void Render(Generation.IHtmlWriter writer)
        {
            writer.RenderBeginTag("table");

            if (HasClientSideBinding("DataSource"))
            {
                writer.RenderBeginTag("tbody");
                writer.AddBindingAttribute("foreach", TranslateToKnockoutProperty(Bindings["DataSource"].Path));
                RowTemplate.DataContext = null;
                RowTemplate.Render(writer);
                writer.RenderEndTag();
            }
            else
            {
                writer.RenderBeginTag("tbody");
                foreach (var item in DataSource)
                {
                    writer.RenderBeginTag("tr");
                    RowTemplate.DataContext = new BindingContext() { Parent = this.DataContext, ViewModel = item };
                    RowTemplate.Render(writer);
                    writer.RenderEndTag();
                }
                writer.RenderEndTag();    
            }
            
            writer.RenderEndTag();
        }



    }
}
