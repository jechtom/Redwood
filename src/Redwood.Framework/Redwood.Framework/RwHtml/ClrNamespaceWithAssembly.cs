using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml
{
    public struct ClrNamespaceWithAssembly
    {
        private string clrNamespace;
        
        private string assemblyName;

        public ClrNamespaceWithAssembly(string clrNamespace, string assemblyName)
        {
            this.clrNamespace = clrNamespace;
            this.assemblyName = assemblyName;
        }

        /// <summary>
        /// Gets CLR namespace name.
        /// </summary>
        public string ClrNamespace
        {
            get
            {
                return clrNamespace;
            }
        }

        /// <summary>
        /// Gets assembly name.
        /// </summary>
        public string AssemblyName
        {
            get
            {
                return assemblyName;
            }
        }

        // format: "clr-namespace:Namespace;assembly=Assembly
        static readonly Regex textFormatRegex = new Regex("^clr-namespace:(?<namespace>[^;=]+); *assembly=(?<assembly>.[^=]+)$", RegexOptions.IgnoreCase);

        public static bool TryParse(string text, out ClrNamespaceWithAssembly output)
        {
            var match = textFormatRegex.Match(text);
            if (!match.Success)
            {
                output = new ClrNamespaceWithAssembly();
                return false;
            }

            output = new ClrNamespaceWithAssembly(
                    match.Groups["namespace"].Value,
                    match.Groups["assembly"].Value
                );
            return true;
        }
    }
}
