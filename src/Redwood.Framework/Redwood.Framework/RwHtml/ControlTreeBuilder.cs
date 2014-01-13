using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml
{
    public class ControlTreeBuilder
    {
        private RwHtmlNamespaceScope namespaceScope;
        private ControlTypeMapper typeMapper;
        
        public ControlTreeBuilder()
        {
            namespaceScope = new RwHtmlNamespaceScope();
            typeMapper = ControlTypeMapper.Default;
        }

        public void BuildControl(string prefix, string name)
        {
            string rwhtmlNamespace = namespaceScope.GetNamespaceByPrefix(prefix);
            if (rwhtmlNamespace == null)
                throw new InvalidOperationException("Invalid namespace prefix: " + prefix);

            var type = typeMapper.GetType(rwhtmlNamespace, name);

            if(type == null)
            {
                throw new InvalidOperationException(string.Format("Type {0} not found in rwhtml namespace \"{1}\".", name, rwhtmlNamespace));
            }

            // TODO build control
            throw new NotImplementedException();
        }

        public void RegisterNamespaceToPrefix(string prefix, string rwhtmlNamespace)
        {
            namespaceScope.AddNamespace(prefix, rwhtmlNamespace);
        }
    }
}
