using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml
{
    public class PropertyMapper
    {
        static readonly PropertyMapper defaultMapper = new PropertyMapper();

        public static PropertyMapper Default
        {
            get
            {
                return defaultMapper;
            }
        }

        public Binding.IPropertyAccessor GetPropertyOrThrowError(Type clrType, string propertyName, bool isAttachedProperty)
        {
            // try find redwood property
            var redwoodPropInfo = Binding.RedwoodProperty.GetByName(propertyName, clrType);
            if (redwoodPropInfo != null)
            {
                return new Binding.RedwoodPropertyAccessor(redwoodPropInfo);
            }

            // try find CLR property
            if (!isAttachedProperty)
            {
                var propName = propertyName;
                var propInfo = clrType.GetProperty(propName);
                if (propInfo != null)
                    return new Binding.PropertyBasicAccessor(propInfo);
            }

            throw new InvalidOperationException(string.Format("Property \"{0}\" not found on \"{1}\".", propertyName, clrType.FullName));
        }
    }
}
