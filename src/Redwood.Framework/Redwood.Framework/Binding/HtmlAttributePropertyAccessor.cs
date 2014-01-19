using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Binding
{
    public class HtmlAttributePropertyAccessor : IPropertyAccessor
    {
        public HtmlAttributePropertyAccessor(string propName)
        {
            if (string.IsNullOrWhiteSpace(propName))
                throw new ArgumentNullException("Argument is null or white space.", "propName");

            this.PropertyName = propName;
        }

        public void SetValue(object instance, object value)
        {
            var instanceObj = (ICustomHtmlAttributes)instance;
            var valueStr = value == null ? null : value.ToString(); // should be always string
            instanceObj.SetAttributeValue(PropertyName, valueStr);
        }

        public Type Type
        {
            get { return typeof(string); }
        }

        public string PropertyName { get; private set; }
    }
}
