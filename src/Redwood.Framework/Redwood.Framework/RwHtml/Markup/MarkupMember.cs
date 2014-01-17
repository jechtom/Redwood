using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    public class MarkupMember
    {
        public bool IsContentProperty { get; set; }
        public bool IsInlineDefinition { get; set; }
        public string RwHtmlNamespace { get; set; }
        public NameWithPrefix Name { get; set; }
        public bool IsAttachedProperty { get; set; }
        public Binding.IPropertyAccessor PropertyAccessor { get; set; }
        public Type AttachedPropertyOwnerType { get; set; }

        public override string ToString()
        {
            return string.Format(
                    "{0} (inline: {1}, is attached: {2}) {3}",
                    IsContentProperty ? "[ContentProperty]" : Name.ToString(), 
                    IsInlineDefinition, 
                    IsAttachedProperty,
                    RwHtmlNamespace
                );
        }
    }
}
