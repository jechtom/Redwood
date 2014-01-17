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
        private Dictionary<string, Assembly> loadedAssemblies;

        public ControlTypeMapper()
        {
            loadedAssemblies = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);
            string currentAssembly = Assembly.GetExecutingAssembly().FullName;
            namespaceMapper = new RwHtmlCrlNamespaceMapper(new[] { currentAssembly });
        }

        public ControlTypeMapper(RwHtmlCrlNamespaceMapper namespaceMapper)
        {
            if (namespaceMapper == null)
                throw new ArgumentNullException("namespaceMapper");

            this.namespaceMapper = namespaceMapper;
        }

        public Type GetType(string rwhtmlNamespace, string name)
        {
            // try find CLR type in corresponding CLR namespaces mapped to given rwhtml namespace
            foreach(var clrNamespace in namespaceMapper.GetClrNamespacesForRwHtmlNamespace(rwhtmlNamespace))
            {
                var result = GetTypeFromCrlNamespace(clrNamespace, name);
                if (result == null)
                    continue;

                return result;
            }

            return null;
        }

        private Type GetTypeFromCrlNamespace(ClrNamespaceWithAssembly clrNamespace, string name)
        {
            Assembly assembly;
            if(!loadedAssemblies.TryGetValue(clrNamespace.AssemblyName, out assembly))
            {
                assembly = Assembly.Load(clrNamespace.AssemblyName);
                loadedAssemblies.Add(clrNamespace.AssemblyName, assembly);
            }

            var result = assembly.GetType(clrNamespace.ClrNamespace + "." + name);
            return result;
        }
    }
}
