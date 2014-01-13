using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml
{
    /// <summary>
    /// Maps rwhtml elements to the CLR types.
    /// </summary>
    public class ControlTypeMapper
    {
        static readonly ControlTypeMapper defaultMapper = new ControlTypeMapper();

        public static ControlTypeMapper Default
        {
            get
            {
                return defaultMapper;
            }
        }

        private RwHtmlCrlNamespaceMapper namespaceMapper;

        public ControlTypeMapper()
        {
            string currentAssembly = Assembly.GetExecutingAssembly().FullName;
            namespaceMapper = new RwHtmlCrlNamespaceMapper(new[] { currentAssembly });
        }

        public Type GetType(string rwhtmlNamespace, string name)
        {
            throw new NotImplementedException();
        }
    }
}
