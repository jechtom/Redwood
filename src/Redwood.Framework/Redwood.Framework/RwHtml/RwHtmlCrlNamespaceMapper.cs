using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml
{
    public class RwHtmlCrlNamespaceMapper
    {
        public Dictionary<string, List<ClrNamespaceWithAssembly>> mapping;
        
        public RwHtmlCrlNamespaceMapper()
        {
            Init();
        }

        public RwHtmlCrlNamespaceMapper(string[] assembliesToLoad)
        {
            Init();
            LoadAssemblies(assembliesToLoad);
        }

        private void Init()
        {
            mapping = new Dictionary<string, List<ClrNamespaceWithAssembly>>(StringComparer.OrdinalIgnoreCase);
        }

        public void LoadAssemblies(string[] assembliesToLoad)
        {
            foreach (var assemblyName in assembliesToLoad)
            {
                var assembly = Assembly.Load(assemblyName);
                foreach (var atr in assembly.GetCustomAttributes<RwHtmlNamespaceDefinitionAttribute>())
                {
                    var clrNamespace = new ClrNamespaceWithAssembly(atr.ClrNamespace, assemblyName);
                    RegisterNamespace(atr.RwHtmlNamespace, clrNamespace);
                }
            }
        }

        private void RegisterNamespace(string rwHtmlNamespace, ClrNamespaceWithAssembly clrNamespace)
        {
            List<ClrNamespaceWithAssembly> items;
            if (!mapping.TryGetValue(rwHtmlNamespace, out items))
            {
                items = new List<ClrNamespaceWithAssembly>();
                mapping.Add(rwHtmlNamespace, items);
            }

            items.Add(clrNamespace);
        }

        public IEnumerable<ClrNamespaceWithAssembly> GetClrNamespacesForRwHtmlNamespace(string rwHtmlNamespace)
        {
            List<ClrNamespaceWithAssembly> result;
            if (mapping.TryGetValue(rwHtmlNamespace, out result))
                return result;

            // try parse
            ClrNamespaceWithAssembly parsedClrNamespace;
            if(ClrNamespaceWithAssembly.TryParse(rwHtmlNamespace, out parsedClrNamespace))
            {
                result = new List<ClrNamespaceWithAssembly>(1);
                result.Add(parsedClrNamespace);
                mapping.Add(rwHtmlNamespace, result);
                return result;
            }

            return Enumerable.Empty<ClrNamespaceWithAssembly>();
        }
    }
}
