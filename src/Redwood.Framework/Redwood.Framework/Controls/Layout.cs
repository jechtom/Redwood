using Redwood.Framework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Controls
{
    public class Layout
    {
        /// <summary>
        /// Gets or sets default name of layout section.
        /// </summary>
        public const string DefaultSectionName = "DefaultSection";

        /// <summary>
        /// Property definition. Gets or sets virtual path of layout view.
        /// </summary>
        public static readonly RedwoodProperty View = RedwoodProperty.RegisterAttached<string, Layout>("View");

        /// <summary>
        /// Property definition. Gets or sets name of this section.
        /// </summary>
        public static readonly RedwoodProperty Section = RedwoodProperty.RegisterAttached<string, Layout>("Section", DefaultSectionName);
    }
}
