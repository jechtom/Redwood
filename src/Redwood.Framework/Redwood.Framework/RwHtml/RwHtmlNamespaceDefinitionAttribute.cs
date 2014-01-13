using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml
{
    /// <summary>
    /// Defines mapping of CLR namespace to rwhtml namespace.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class RwHtmlNamespaceDefinitionAttribute : Attribute
    {
        public RwHtmlNamespaceDefinitionAttribute(string rwHtmlNamespace, string clrNamespace)
        {
            if (string.IsNullOrWhiteSpace(rwHtmlNamespace))
                throw new ArgumentException("String is null or white space.", "rwHtmlNamespace");

            if (string.IsNullOrWhiteSpace(clrNamespace))
                throw new ArgumentException("String is null or white space.", "clrNamespace");

            RwHtmlNamespace = rwHtmlNamespace;
            ClrNamespace = clrNamespace;
        }

        public string RwHtmlNamespace
        {
            get;
            private set;
        }

        public string ClrNamespace
        {
            get;
            private set;
        }
    }
}
