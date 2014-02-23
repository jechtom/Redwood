using System;
using System.Collections.Generic;
using System.Linq;
using Redwood.Framework.Binding.Parsing.Expressions;

namespace Redwood.Framework.Controls
{
    public class DataContextPathBuilder
    {

        /// <summary>
        /// Appends the property path.
        /// </summary>
        public static string AppendPropertyPath(string dataContextPath, BindingExpression path)
        {
            while (path != null)
            {
                if (!(path is BindingGetPropertyExpression))
                {
                    // TODO: The binding must contain only property chain
                    throw new NotSupportedException();
                }

                dataContextPath = dataContextPath + "." + ((BindingGetPropertyExpression)path).PropertyName;
                path = ((BindingGetPropertyExpression)path).NextExpression;
            }
            return dataContextPath;
        }

        /// <summary>
        /// Appends the collection index placeholder.
        /// </summary>
        public static string AppendCollectionIndexPlaceholder(string dataContextPath)
        {
            return dataContextPath + "[{$index}]";
        }

        /// <summary>
        /// Appends the index of the collection.
        /// </summary>
        public static string AppendCollectionIndex(string dataContextPath, int index)
        {
            return dataContextPath + "[" + index + "]";
        }
    }
}