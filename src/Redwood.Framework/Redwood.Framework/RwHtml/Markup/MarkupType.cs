using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml.Markup
{
    public class MarkupType
    {
        public string RwHtmlNamespace { get; set; }
        public NameWithPrefix Name { get; set; }

        public override string ToString()
        {
            return Name.ToString() + " " + RwHtmlNamespace;
        }

        public Type ClrType { get; set; }
        public ConstructorInfo ClrConstructor { get; set; }

        public bool IsRedwoodBindable
        {
            get
            {
                if (ClrType == null)
                    throw new NullReferenceException("ClrType is not set.");

                return typeof(RedwoodBindable).IsAssignableFrom(ClrType);
            }
        }
    }
}
