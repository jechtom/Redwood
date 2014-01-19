using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Binding
{
    public class BindingBase : RwHtml.Markup.MarkupExpression
    {
        [Flags]
        private enum BindingFlags : uint
        {
            OneTime = 0u,
            OneWay = 1u,
            OneWayToSource = 2u,
            TwoWay = OneWay | OneWayToSource
        }

        private BindingFlags flags;

        private PropertyPath path;

        public bool IsOneTime
        {
            get
            {
                return flags == BindingFlags.OneTime;
            }
        }
            
        public object Evaluate(RedwoodBindable redwoodBindable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets binding property path. Can be null.
        /// </summary>
        public PropertyPath Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{{Binding{0}}}", 
                Path == null ? "" : Path.ToString()
            );
        }

        public override object Evaluate(RwHtml.Markup.MarkupExpressionEvaluationContext context)
        {
            throw new NotImplementedException(); // todo: set binding
        }
    }
}
