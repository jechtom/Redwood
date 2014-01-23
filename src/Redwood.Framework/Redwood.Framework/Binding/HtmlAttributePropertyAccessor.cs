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
            IHtmlAttributesStorage storage = ResolveAttributeStorageOrThrow(instance);
            
            var valueStr = value == null ? null : value.ToString(); // should be always string
            storage.SetAttributeValue(PropertyName, valueStr);
        }

        public static IHtmlAttributesStorage ResolveAttributeStorageOrThrow(object instance)
        {
            IHtmlAttributesStorage storage;

            if (instance is IHtmlAttributesStorageProvider)
                storage = ((IHtmlAttributesStorageProvider)instance).ProvideStorage();
            else if (instance is IHtmlAttributesStorage)
                storage = (IHtmlAttributesStorage)instance;
            else
                throw new InvalidOperationException(
                    string.Format("Custom HTML attributes are not supported on type {0}. It does not implements {1} or {2}.",
                        instance.GetType().Name,
                        typeof(IHtmlAttributesStorageProvider).Name,
                        typeof(IHtmlAttributesStorage).Name
                    ));

            return storage;
        }

        public Type Type
        {
            get { return typeof(string); }
        }

        public string PropertyName { get; private set; }
    }
}
