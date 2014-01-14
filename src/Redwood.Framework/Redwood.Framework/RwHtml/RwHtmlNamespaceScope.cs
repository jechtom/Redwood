using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.RwHtml
{
    public class RwHtmlNamespaceScope
    {
        private class Scope
        {
            public string RwhtmlNamespace;
            public string Prefix;
            public int Level;
        }

        Stack<Scope> scopesStack;
        private int currentScopeLevel;

        public RwHtmlNamespaceScope()
        {
            currentScopeLevel = 0;
            scopesStack = new Stack<Scope>();
        }

        public void PushScope()
        {
            // increase scope level
            currentScopeLevel++;
        }

        public void PopScope()
        {
            if (currentScopeLevel <= 0)
                throw new InvalidOperationException("Cannot pop scope before pushing scope.");

            // remove current scope (if any defined on current level)
            while (scopesStack.Count > 0 && scopesStack.Peek().Level >= currentScopeLevel)
            {
                scopesStack.Pop();
            }

            // decrease scope level
            currentScopeLevel--;
        }

        public void EnsureScopeLevelIsZero()
        {
            if (currentScopeLevel != 0)
                throw new InvalidOperationException("Scope leve is not zero. Current level: " + currentScopeLevel);
        }

        public void AddNamespace(string prefix, string rwhtmlNamespace)
        {
            scopesStack.Push(new Scope()
            {
                Level = currentScopeLevel,
                Prefix = prefix,
                RwhtmlNamespace = rwhtmlNamespace
            });
        }

        public string GetNamespaceByPrefix(string prefix)
        {
            var item = scopesStack.FirstOrDefault(s => string.Equals(s.Prefix, prefix, StringComparison.OrdinalIgnoreCase));
            
            if(item == null) // not found?
            {
                return null;
            }

            return item.RwhtmlNamespace;
        }

        public string NamespaceDefinitionNamespace
        {
            get
            {
                return "xmlns";
            }
        }
    }
}
