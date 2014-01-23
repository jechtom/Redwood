using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Binding
{
    public interface IHtmlAttributesStorage
    {
        void SetAttributeValue(string name, string value);
    }
}
