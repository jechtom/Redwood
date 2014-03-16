using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redwood.Framework.Binding
{
    public class BindingExpression : ExpressionBase
    {
        public BindingExpression(BindingMarkup markupExtension)
        {
            Path = markupExtension.Path;
            Mode = markupExtension.Mode;
        }

                /// <summary>
        /// Gets the name of the binding path.
        /// </summary>
        public Binding.Parsing.Expressions.BindingPathExpression Path { get; private set; }

        public BindingMode Mode { get; private set; }

        /// <summary>
        /// Gets or sets whether the binding should be rendered on server.
        /// </summary>
        public bool RenderOnServer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the string format.
        /// </summary>
        public bool Format
        {
            get;
            set;
        }

        /// <summary>
        /// Joins the paths.
        /// </summary>
        public static string JoinPaths(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1)) return path2;
            if (string.IsNullOrEmpty(path2)) return path1;
            return path1 + "." + path2;
        }

        public override object GetValue(RedwoodBindable target, RedwoodProperty property)
        {
 	        var dataContext = target.GetValue(Controls.RedwoodControl.DataContextProperty);
            var result = Path.Evaluate(dataContext);
            return result;
        }
    }

    [Flags]
    public enum BindingFlags : uint
    {
        OneTime = 0u,
        OneWay = 1u,
        OneWayToSource = 2u,
        TwoWay = OneWay | OneWayToSource
    }
}
