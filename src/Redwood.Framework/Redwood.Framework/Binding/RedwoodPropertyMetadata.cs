using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Binding
{
    public class RedwoodPropertyMetadata
    {
        private object defaultValue;
        private RedwoodPropertyFlags flags;

        public RedwoodPropertyMetadata(object defaultValue, RedwoodPropertyFlags flags = RedwoodPropertyFlags.None)
        {
            this.defaultValue = defaultValue;
            this.flags = flags;
        }

        public RedwoodPropertyFlags Flags
        {
            get
            {
                return flags;
            }
        }

        public bool IsInherited
        {
            get
            {
                return (flags & RedwoodPropertyFlags.IsInherited) > 0;
            }
        }

        public bool IsInheritanceSource
        {
            get
            {
                return (flags & RedwoodPropertyFlags.IsInheritanceSource) > 0;
            }
        }

        public bool IsAttached
        {
            get
            {
                return (flags & RedwoodPropertyFlags.IsAttached) > 0;
            }
        }

        public bool IsHtmlAttribute
        {
            get
            {
                return (flags & RedwoodPropertyFlags.IsHtmlAttribute) > 0;
            }
        }

        public object DefaultValue
        {
            get
            {
                return defaultValue;
            }
        }
    }
}
