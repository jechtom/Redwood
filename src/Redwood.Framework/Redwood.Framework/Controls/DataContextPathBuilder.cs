using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redwood.Framework.Binding;

namespace Redwood.Framework.Controls
{
    public class DataContextPathBuilder
    {
        private static DataContextPathBuilder instance = new DataContextPathBuilder();

        public static DataContextPathBuilder Default
        {
            get { return instance; }
        }



        /// <summary>
        /// Builds the path.
        /// </summary>
        public string BuildPath(RedwoodBindable control)
        {
            var parents = GetDataContextBindingChain(control);
            
            var sb = new StringBuilder();
            while (parents.Count > 0)
            {
                var item = parents.Pop();

                if (sb.Length > 0)
                {
                    sb.Append(".");
                }
                sb.Append(KnockoutBindingHelper.TranslateToKnockoutProperty(item.Item1, RedwoodControl.DataContextProperty, item.Item2));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the chain of data context property values.
        /// </summary>
        private static Stack<Tuple<RedwoodBindable, BindingMarkup>> GetDataContextBindingChain(RedwoodBindable control)
        {
            var parents = new Stack<Tuple<RedwoodBindable, BindingMarkup>>();
            while (control != null)
            {
                var binding = (BindingMarkup)control.GetRawValue(RedwoodControl.DataContextProperty, true);
                if (binding != null)
                {
                    parents.Push(new Tuple<RedwoodBindable, BindingMarkup>(control, binding));
                }
                control = control.Parent;
            }
            return parents;
        }
    }
}
