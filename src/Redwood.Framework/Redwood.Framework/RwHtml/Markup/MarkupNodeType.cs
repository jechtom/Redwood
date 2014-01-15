using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwood.Framework.RwHtml.Markup
{
    public enum MarkupNodeType
    {
        Value,
        BeginObject,
        EndObject,
        BeginMember,
        EndMember,
        NamespaceDeclaration,
        EndOfDocument
    }
}
