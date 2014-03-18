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
            SourceProperty = Controls.RedwoodControl.DataContextProperty;
        }

        public BindingExpression(BindingMode mode, Binding.Parsing.Expressions.BindingPathExpression path, RedwoodProperty sourceProperty = null, RedwoodBindable source = null)
        {
            Path = path;
            Mode = mode;
            SourceProperty = sourceProperty ?? Controls.RedwoodControl.DataContextProperty;
            Source = source;
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

        public RedwoodProperty SourceProperty
        {
            get;
            set;
        }

        public RedwoodBindable Source
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
            var dataContext = GetDataContext(Source ?? target, property);
            
            var result = Path.Evaluate(dataContext);
            return result;
        }

        private object GetDataContext(RedwoodBindable target, RedwoodProperty property)
        {
            object result = null;
            bool isDataContextBinding = (property == SourceProperty);
            
            if (isDataContextBinding)
            {
                // if this is binding on "DataContext" - read value from parent
                if (target.Parent != null)
                    result = target.Parent.GetRawValue(SourceProperty);
            }
            else
            {
                result = target.GetValue(SourceProperty);
            }

            return result;
        }
    }
}
