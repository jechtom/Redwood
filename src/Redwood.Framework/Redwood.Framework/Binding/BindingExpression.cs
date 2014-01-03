using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Redwood.Framework.Generation;

namespace Redwood.Framework.Binding
{
    public class BindingExpression
    {
        /// <summary>
        /// Gets the name of the binding path.
        /// </summary>
        public string Path { get; private set; }

        public BindingMode Mode { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingExpression"/> class.
        /// </summary>
        public BindingExpression(string path, BindingMode mode)
        {
            Path = path;
            Mode = mode;
        }

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
        /// Evaluates the specified target.
        /// </summary>
        public object Evaluate(RedwoodBindable target)
        {
            var dataContext = target.GetValue(Controls.RedwoodControl.DataContextProperty);

            if (string.IsNullOrEmpty(Path))
            {
                return dataContext;
            }

            var parts = Path.Split('.');
            foreach (var part in parts)
            {
                dataContext = dataContext.GetType().GetProperty(part).GetValue(dataContext);
            }
            return dataContext;
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
    }
}